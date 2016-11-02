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
using System.Media;
using System.Security.Cryptography;
using System.IO;

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

        Aes aes = Aes.Create();

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
                //Hardcoded encryption key
                aes.Key.Equals(Encoding.Default.GetBytes("wowsuchdoge"));

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
        //Bool so false can be returned if error encountered - will be added later
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
                //Decrypt and convert to string
                string message = decrypt(data, aes.Key, aes.IV);
                //Invoke method with (delegate, parameter)
                Console.WriteLine("Messaged received.");
                Invoke(s, message);
            }
        }

        private void showMessage(string message)
        {
            StringBuilder str = new StringBuilder();
            //Add current date and time
            DateTime currentDT = DateTime.Now;
            str.Append(currentDT.ToString() + " ");
            //Append new lines for appearence
            str.AppendLine(message);

            //Play sound
            if (message.StartsWith("> > >"))
            {
                importantAlert();
            }

            textBoxWindow.Text += str.ToString();
        }

        //Important message alert
        private void importantAlert()
        {
            SoundPlayer impAlert = new SoundPlayer(@"C:\Windows\Media\Windows Balloon.wav");
            impAlert.Play();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string message;

            //Check if important
            if (checkBoxImportant.Checked)
            {
                //Add visual notifiers
                message = "> > > " + name + ": " + textBoxMessage.Text;
            }
            else
            {
                //Vanilla message
                message = name + ": " + textBoxMessage.Text;
            }

            //Convert to bytes and encrypt
            byte[] data = encrypt(message, aes.Key, aes.IV);
            //Send 
            send.Send(data, data.Length);

            //Clean textbox and checkbox
            textBoxMessage.Text = "";
            checkBoxImportant.Checked = false;
        }

        //Encryption
        static byte[] encrypt(string message, byte[] key, byte[] IV)
        {
            //Check for null and throw exceptions
            if (message == null || message.Length <= 0)
                throw new ArgumentNullException("Message");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;
            //AES,  Symmetric encryption
            using (Aes algorithm = Aes.Create())
            {
                algorithm.Key = key;
                algorithm.IV = IV;

                //Create encryptor
                ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);

                //Streams for encryption
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(message);
                        }
                        //String to bytes[]
                        encrypted = ms.ToArray();
                    }
                }
            }

            //Test
            Console.WriteLine(Encoding.Default.GetString(encrypted));

            //Return the encrypted bytes
            return encrypted;
        }

        //Decryption
        static string decrypt(byte[] cipher, byte[] key, byte[] IV)
        {
            string message;

            using (Aes algorithm = Aes.Create())
            {
                algorithm.Key = key;
                algorithm.IV = IV;

                //Create decryptor
                ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                //Streams for decrytpion
                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            //Bytes[] to string
                            message = sr.ReadToEnd();
                        }
                    }
                }
            }

            return message;
        }

        //Handler for Send button when 'enter' is pressed
        private void buttonSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSend.PerformClick();
            }
        }

        //Scroll to end when textbox is changed
        private void textBoxWindow_TextChanged(object sender, EventArgs e)
        {
            textBoxWindow.SelectionStart = textBoxWindow.Text.Length;
            textBoxWindow.ScrollToCaret();
        }
    }
}
