﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DogeChat
{
    public partial class Login : Form
    {
        //Name of user
        public string name;

        public Login()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            
            if (textBoxName.Text.Equals(""))
            {
                MessageBox.Show("Name empty, please provide a name.");
                return;
            }
            else
            {
                //Set name from textbox
                name = textBoxName.Text;
                //Close login form and open the chat window
                this.Close();
            }
        }


    }
}
