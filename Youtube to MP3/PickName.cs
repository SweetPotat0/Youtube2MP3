using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Youtube_to_MP3
{
    public partial class PickName : Form
    {
        public static String name;
        private String[] strings;
        private string temp;
        public PickName()
        {
            InitializeComponent();
            textBox1.Text = name;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
                label1.Text = "You must enter a name!";
            else
            {
                temp = textBox1.Text;
                strings = Directory.GetFiles(Downloads_List.path);
                for (int i = 0; i < strings.Length; i++) {
                    if (strings[i].Equals(Downloads_List.path+temp+".mp3"))
                    {
                        label1.Text = "The name of this file exists at the directory you chose.";
                        return;
                    }
                }
                name = textBox1.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
