using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doctricentrese
{
    public partial class frmMain : Form
    {
        string[] Files;
        Item[] Items;

        DoctriButton CurrentItem = null;
        DoctriButton PreviousItem = null;

        int StartTimePlayer1 = 0;
        int StartTimePlayer2 = 0;
        DateTime CurrentPlayerTime = new DateTime();

        bool running = false;

        bool isPlayer1 = true;

        Timer TheTimer = new Timer();

        Timer runningTimer = new Timer();

        Jugador currentPlayer = null;

        int GoodCounter = 0;

        int TotalItems = 0;

        List<Jugador> jugadores = new List<Jugador>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            runningTimer.Interval = 1000;
            runningTimer.Tick += runningTimer_Tick;

            var files = Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "Images"));
            var imageExtensions = new string[] { ".bmp", ".jpg", ".png" };

            Files = files.Where(r => imageExtensions.Contains(Path.GetExtension(r))).ToArray();

            Items = Files.Select(r => new Item { Address = r }).ToArray();

            cklItems.Items.AddRange(Items);

            chkSelectAll_CheckedChanged(chkSelectAll, e);

            TheTimer.Interval = 1000;

            TheTimer.Tick += TheTimer_Tick;

            cklItems.SelectedIndex = 0;

            SetGame();
        }

        void runningTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = CurrentPlayerTime.ToString("HH:mm:ss");

            if (isPlayer1)
            {
                StartTimePlayer1++;
                CurrentPlayerTime = DateTime.MinValue.AddSeconds(StartTimePlayer1);
            }
            else
            {
                StartTimePlayer2++;
                CurrentPlayerTime = DateTime.MinValue.AddSeconds(StartTimePlayer2);
            }
        }

        void TheTimer_Tick(object sender, EventArgs e)
        {

            if (CurrentItem.Item.Equals(PreviousItem.Item))
            {
                CurrentItem.Visible = false;
                PreviousItem.Visible = false;

                GoodCounter++;
            }
            else
            {
                CurrentItem.TheButton.Visible = true;
                PreviousItem.TheButton.Visible = true;

                isPlayer1 = !isPlayer1;

                lblTime.ForeColor = isPlayer1 ? Color.Blue : Color.Red;
            }

            CurrentItem = null;
            PreviousItem = null;

            running = false;
            TheTimer.Stop();

            if (TotalItems == GoodCounter)
            {
                lblMessage.Text = "Lo lograste en " + lblTime.Text + "!!! ¿Puedes mejorarlo?";

                if (!string.IsNullOrEmpty(txtPlayer1.Text) && !string.IsNullOrEmpty(txtPlayer2.Text))
                {
                    currentPlayer = new Jugador() { Nombre = (isPlayer1 ? txtPlayer1.Text : txtPlayer2.Text), Fichas = cklItems.CheckedItems.Count };
                    currentPlayer.Tiempo = isPlayer1 ? --StartTimePlayer1 : --StartTimePlayer2;
                    jugadores.Add(currentPlayer);

                    lblMessage.Text = currentPlayer.Nombre + " lo lograste en " + lblTime.Text + "!!! ¿Puedes mejorarlo?";

                    dataGridView1.DataSource = jugadores.OrderByDescending(r => r.Fichas).ThenBy(r => r.Tiempo).ToArray();

                    //txtPlayer2.DataSource = jugadores.Select(r => r.Nombre).Distinct().ToArray();
                }

                runningTimer.Stop();
                pnlMessage.Visible = true;

                //Task.Delay(5000).ContinueWith((x)=>SetGame());

            }
        }

        private void cklItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var chk = (CheckedListBox)sender;
            var item = (Item)chk.SelectedItem;
            pbxPreview.Image = new Bitmap(item.Address);
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            var chkSelectAll = (CheckBox)sender;

            for (int i = 0; i < cklItems.Items.Count; i++)
            {
                cklItems.SetItemChecked(i, chkSelectAll.Checked);
            }
        }

        private int[] GetMatrixSides(int number)
        {
            var result = new int[2];

            for (int i = number - 1; i > 0; i--)
            {
                var x = number % i;
                var y = number / i;

                if (y >= i && i * y == number)
                {
                    result[0] = i;
                    result[1] = y;
                    break;
                }
            }

            return result;
        }

        private void Juego_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabControl = (TabControl)sender;
            runningTimer.Stop();

            pnlMessage.Visible = false;

            if (tabControl.SelectedIndex == 0)
            {
                var itemsCount = cklItems.CheckedItems.Count;

                if (itemsCount < 2)
                {
                    MessageBox.Show("Debe seleccionar por lo menos 1 tema a estudiar", "Seleccione tema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tabControl.SelectedIndex = 1;
                }

                SetGame();
            }
        }

        private void SetGame()
        {
            var totalItems = cklItems.CheckedItems.Count;

            StartTimePlayer1 = 0;
            StartTimePlayer2 = 0;

            CurrentPlayerTime = DateTime.MinValue;

            TotalItems = totalItems;
            GoodCounter = 0;

            lblTime.Text = "00:00:00";
            lblTime.ForeColor = Color.Blue;
            runningTimer.Stop();

            pnlMessage.Visible = false;

            var cheCheckedItems = new List<string>();

            foreach (var item in cklItems.CheckedItems)
            {
                cheCheckedItems.Add(((Item)item).Address);
            }

            Items = cheCheckedItems.Select(r => new Item { Address = r }).ToArray();

            var itemsCount = totalItems * 2;
            var dimensions = GetMatrixSides(itemsCount);

            var x = dimensions[0];
            var y = dimensions[1];

            Shuffle<Item>(Items);

            var counter = 0;

            panel1.Controls.Clear();

            var indexCounter = 0;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var containerWidth = panel1.Width;
                    var containerHeight = panel1.Height;

                    var width = containerWidth / x;
                    var height = containerHeight / y;

                    var posX = width * i;
                    var posY = height * j;

                    indexCounter++;

                    var button = new DoctriButton(Items[counter], width, height, indexCounter);

                    button.Location = new Point(posX, posY);

                    button.Clicked += button_Clicked;

                    panel1.Controls.Add(button);

                    counter++;
                    if (counter >= totalItems)
                    {
                        counter = 0;
                        Shuffle<Item>(Items);
                    }
                }
            }
        }

        void button_Clicked(object sender, EventArgs e)
        {
            if (CurrentPlayerTime == DateTime.MinValue)
            {
                runningTimer.Start();
            }

            if (!running)
            {
                var button = (DoctriButton)sender;

                button.TheButton.Visible = false;

                if (CurrentItem == null)
                {
                    CurrentItem = button;
                }
                else
                {
                    running = true;

                    PreviousItem = CurrentItem;
                    CurrentItem = button;

                    TheTimer.Start();
                }
                //lblDescription.Text = button.Item.ToString();
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            //SetGame();
        }

        private static Random rng = new Random((int)DateTime.Now.Ticks);
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            SetGame();
        }

        private void btnDeleteScores_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Deseas Borrar los puntajes?", "Borrar puntajes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                jugadores = null;
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

            }
        }
    }

    class Item
    {
        public string Name
        {
            get
            {
                return GetName();
            }
        }
        public string Address { get; set; }

        public Bitmap Image
        {
            get
            {
                return new Bitmap(Address);
            }
        }

        private string GetName()
        {
            var name = Path.GetFileNameWithoutExtension(Address);

            name = SplitCamelCase(name);

            return name;
        }

        private static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class Jugador
    {
        public string Nombre { get; set; }

        public int Tiempo { get; set; }

        public int Fichas { get; set; }
    }
}
