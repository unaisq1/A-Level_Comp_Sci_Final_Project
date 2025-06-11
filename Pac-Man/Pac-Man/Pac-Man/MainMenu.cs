using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pac_Man
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            PlayerTimer.Enabled = true;
            LastScore.Text = PacManLevel.score.ToString();
            LastLevel.Text = PacManLevel.level.ToString();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            PacManLevel p = new PacManLevel();
            p.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Settings s = new Settings();
            s.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PlayerTimer_Tick(object sender, EventArgs e)
        {
            if (Settings.setOption == 1)
            {
                PlayerChoice.Text = "Pac-Man";
            }
            else if (Settings.setOption == 2)
            {
                PlayerChoice.Text = "Ms. Pac-Man";
            }
        }
    }
}
