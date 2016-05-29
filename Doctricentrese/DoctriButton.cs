using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doctricentrese
{
    class DoctriButton : Panel
    {
        public event EventHandler Clicked;

        public Item Item { get; set; }

        public Button TheButton { get; set; }

        public DoctriButton(Item item, int width, int height, int index)
        {
            lock (this)
            {
                Item = item;
                Width = width;
                Height = height;
                TheButton = new Button();

                var button = new Button();

                button.Width = this.Width;
                button.Height = this.Height;

                button.Anchor = AnchorStyles.None;
                button.BackColor = Color.White;

                button.BackgroundImage = Properties.Resources.bible;
                button.BackgroundImageLayout = ImageLayout.Zoom;

                button.Click += button_Click;

                var bitmap = Item.Image;

                var picture = new PictureBox();
                picture.Width = this.Width;
                picture.Height = this.Height;

                picture.Anchor = AnchorStyles.None;
                picture.BackgroundImageLayout = ImageLayout.Zoom;
                picture.BackgroundImage = bitmap;

                TheButton = button;

                var lblIndex = new Label();
                lblIndex.Text = index.ToString();
                lblIndex.Location = new Point(10, 10);
                lblIndex.Font = new Font(lblIndex.Font.FontFamily, 11, FontStyle.Regular);
                lblIndex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                lblIndex.Width = width / 4;
                lblIndex.TextAlign = ContentAlignment.MiddleCenter;

                this.Controls.Add(lblIndex);
                this.Controls.Add(button);
                this.Controls.Add(picture);
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            Clicked(this, e);
        }
    }
}
