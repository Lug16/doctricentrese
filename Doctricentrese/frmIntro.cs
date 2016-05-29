using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doctricentrese
{
    public partial class frmIntro : Form
    {
        Timer timer = new Timer();

        public frmIntro()
        {
            InitializeComponent();
        }

        private void frmIntro_Load(object sender, EventArgs e)
        {
            timer.Interval = 3500;
            timer.Tick += timer_Tick;
            timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            var frmMain = new frmMain();
            frmMain.FormClosing += (s, ev) => { this.Close(); };
            frmMain.Show();
            this.Hide();
            timer.Stop();
        }
    }
}
