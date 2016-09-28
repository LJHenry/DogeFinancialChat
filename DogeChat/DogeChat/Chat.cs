using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace DogeChat
{
    public partial class Chat : Form
    {
        //Globals
        //Name
        string name;
        //Connection 
        int port;
        string address;
        //Send/Receive clients
        UdpClient send;
        UdpClient receive;
        //Listening thread
        Thread listen;

        //Delegate for thread safety, showMessage called from one thread must be safely transfered to the other
        delegate void safeMessage(string msg);

        public Chat()
        {
            InitializeComponent();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            //Login form pops up on start
            using (Login getDetails = new Login())
            {
                //Get entered name
                getDetails.ShowDialog();
                name = getDetails.name;
                port = Convert.ToInt32(getDetails.port);
                address = getDetails.address;

                //For testing - error handling later
                checkValues(name, port, address);

                //Detect if user pressed cancel
                if (name == null)
                {
                    this.Close();
                }
                else
                {
                    //Set up Udp clients
                    setUp();
                }

            }
        }

        //Show values in console
        private Boolean checkValues(string name, int port, string address)
        {
            Console.WriteLine("Name: " + name);
            Console.WriteLine("IP: " + address);
            Console.WriteLine("Port: " + port);

            return true;
        }

        private void setUp()
        {
            //Sender
            send = new UdpClient(address, port);
            send.EnableBroadcast = true;

            //Receiver
            receive = new UdpClient(port);
            //Background listening thread
            ThreadStart lst = new ThreadStart(listener);
            listen = new Thread(lst);
            listen.IsBackground = true;
            listen.Start();
        }



        private void listener()
        {
            //Any IP, specific port
            IPEndPoint end = new IPEndPoint(IPAddress.Any, port);
            //Delegate
            safeMessage s = showMessage;

            while (true)
            {
                byte[] data = receive.Receive(ref end);
                string message = Encoding.ASCII.GetString(data);
                //Invode method with (delegate, parameter)
                Invoke(s, message);
            }
        }

        private void showMessage(string message)
        {
            //Append new lines for appearence
            StringBuilder str = new StringBuilder();
            str.AppendLine(message);
            textBoxWindow.Text += str.ToString();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string message;

            //Check if important
            if (checkBoxImportant.Checked)
            {
                //Add visual notifiers
                message = "> > > Important from " + name + ": " + textBoxMessage.Text;
            }
            else
            {
                //Vanilla message
                message = name + ": " + textBoxMessage.Text;
            }

            //Convert to bytes
            byte[] data = Encoding.ASCII.GetBytes(message);
            //Send 
            send.Send(data, data.Length);

            //Clean textbox and checkbox
            textBoxMessage.Text = "";
            checkBoxImportant.Checked = false;
        }

        //Handler for Send button when 'enter' is pressed
        private void buttonSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSend.PerformClick();
            }
        }
    }
}
