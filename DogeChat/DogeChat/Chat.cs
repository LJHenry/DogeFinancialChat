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

        //Name of this client
        static string name;
        static string importantName;
        //Connection 
        int port;
        string address;
        //Send/Receive clients
        UdpClient send;
        UdpClient receive;
        //Listening thread
        Thread listen;
        //Name of chat log 'logName.txt'
        string logName;
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
                importantName = getDetails.name + "Important";
                port = Convert.ToInt32(getDetails.port);
                address = getDetails.address;

                //Initialise logName string, prevent null reference if no name error occurs
                logName = "";

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
                    //Send greeting
                    textBoxMessage.Text = "<is now chatting>";
                    sendMessage();
                }
            }
        }

        //ADD ERROR HANDLING HERE
        private Boolean checkValues(string name, int port, string address)
        {
            Console.WriteLine("Name: " + name);
            Console.WriteLine("IP: " + address);
            Console.WriteLine("Port: " + port);

            return true;
        }

        //Initialise UDP Client objects using connection info
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

        //Specified port listener
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
                string message = decrypt(data);

                //Determine if the message will be saved in the normal or important log
                setLogName(message);

                //Invoke method with (delegate, parameter)
                Invoke(s, message);
            }
        }

        //Get name + importance of message sender
        private void setLogName(string msg)
        {
            //Substring - name occurs before colon i.e. 'Louis:'
            int index = msg.IndexOf(':');
            logName = msg.Substring(0, index);

            if (logName.StartsWith("> > >"))
            {
                logName = logName.Substring(6) + "Important";
            }
        }

        //Send button handler
        private void buttonSend_Click(object sender, EventArgs e)
        {
            sendMessage();
        }

        //Format message and send 
        private void sendMessage()
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
            byte[] data = encrypt(message);
            //Send
            send.Send(data, data.Length);

            //Clean textbox and checkbox
            textBoxMessage.Text = "";
            checkBoxImportant.Checked = false;
        }

        //Encryption
        static byte[] encrypt(string message)
        {
            byte[] encrypted;
            //AES,  Symmetric encryption
            using (Aes algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.PKCS7;
                algorithm.Key = (Encoding.Default.GetBytes("DO4GE69420W0WSUCHM3M35P2"));
                algorithm.IV = (Encoding.Default.GetBytes("W0W5UC4D0G3CH4T5"));

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
        static string decrypt(byte[] cipher)
        {
            string message;

            using (Aes algorithm = Aes.Create())
            {
                algorithm.Padding = PaddingMode.PKCS7;
                algorithm.Key = (Encoding.Default.GetBytes("DO4GE69420W0WSUCHM3M35P2"));
                algorithm.IV = (Encoding.Default.GetBytes("W0W5UC4D0G3CH4T5"));

                //Create decryptor
                ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                //Streams for decrytpion
                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
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

        //Format message and display in window
        private void showMessage(string message)
        {
            //Modify string
            StringBuilder str = new StringBuilder();

            //Check if important
            if (message.StartsWith("> > >"))
            {
                //Alert
                importantAlert();

                //Save to important log if sent by this client
                if (logName.Equals(importantName))
                {
                    //Re-encrypt message
                    saveToChatLog(encrypt(message));
                }
            }
            else
            {
                //Save to normal log if sent by this client
                if (logName.Equals(name))
                {
                    //Re-encrypt message
                    saveToChatLog(encrypt(message));
                }
            }

            //Append line for appearence
            str.AppendLine(message);
            //Show message
            textBoxWindow.Text += str.ToString();
        }

        //Save the message to text file
        //Correct file determined by value of logName
        private void saveToChatLog(byte[] message)
        {
            //DateTime object
            DateTime currentDT = DateTime.Now;

            //Check if file already created
            if (checkExists(logName))
            {
                //Append
                File.AppendAllText(logName, currentDT.ToString());
                File.AppendAllText(logName, System.Text.Encoding.UTF8.GetString(message) + "\n");
            }
            else
            {
                //Create
                File.AppendAllText(logName, currentDT.ToString());
                File.WriteAllText(logName, System.Text.Encoding.UTF8.GetString(message) + "\n");
            }
        }

        //Does file exist in same directory as .exe
        private bool checkExists(string name)
        {
            if (File.Exists(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Important message alert
        private void importantAlert()
        {
            SoundPlayer impAlert = new SoundPlayer(@"C:\Windows\Media\Windows Balloon.wav");
            impAlert.Play();
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
