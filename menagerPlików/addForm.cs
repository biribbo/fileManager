using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace menagerPlików
{
    public partial class addForm : Form
    {
        private mainForm main;
        private ListView lv = null;
        private string currentPath = "";

        public addForm(mainForm m, ListView l, string cp)
        {
            InitializeComponent();
            main = m;
            lv = l;
            currentPath = cp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || comboBox1.Text == "")
                MessageBox.Show("Uzupełnij wszystkie pola!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                try
                {
                    if (comboBox1.Text == comboBox1.Items[0].ToString())
                        File.Create(currentPath + textBox1.Text);
                    else
                        Directory.CreateDirectory(currentPath + textBox1.Text);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Enter")
            {
                button1_Click(sender, e);
            }
        }
    }
}
