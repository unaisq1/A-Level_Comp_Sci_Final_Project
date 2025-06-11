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
    public partial class Settings : Form
    {
        //Option indicates which character the player will play as, 1 = Pac-Man, 2 = Ms. Pac-Man
        public static int setOption = 1; //By default, Player will be playing as Pac-Man if they haven't picked another option
        
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setPacManOption(); //Sets value of setOption to be 1, so that player image will be Pac-Man
        }

        public void setPacManOption()
        {
            setOption = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setMsPacManOption(); //Sets value of setOption to be 2, so that player image will be Ms. Pac-Man
        }

        public void setMsPacManOption()
        {
            setOption = 2;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            MainMenu m = new MainMenu();
            m.Show();
        }
    }
}
