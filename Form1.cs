using System;
using System.Windows.Forms;
using SKYPE4COMLib;
using System.Collections.Generic;
using System.IO; //Skype api

namespace CCBot
{
    public partial class CCBot : Form
    {
        private Skype skype;
        private const string trigger = "!"; // Say !help
        private const string nick = "CCBot";
        List<string> langList = new List<string>();

        public CCBot()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            skype = new Skype();
            skype.Attach(8, false);
            // Listen 
            skype.MessageStatus += new _ISkypeEvents_MessageStatusEventHandler(skype_MessageStatus);
        }
        private void skype_MessageStatus(ChatMessage msg, TChatMessageStatus status)
        {
            WriteLine(msg.Chat.Topic + ": " + msg.Sender.FullName.ToString() + ": " + msg.Body);
            // Proceed only if the incoming message is a trigger
            if (msg.Body.Length > 20 && msg.Body.Equals(msg.Body.ToUpper()))
            {
                msg.Chat.SendMessage(msg.Sender.FullName + ", why are you using caps?");
                return;
            }
            langList = new List<string>(msg.Body.Split());
            int count = 0;
            if (!File.Exists("badwords.txt")) File.Create("badwords.txt");
            foreach (string i in langList)
            {
                foreach (string line in File.ReadAllLines("badwords.txt"))
                {
                    if (i.ToLower().StartsWith(line))
                    {
                        count = count + 1;
                    }
                }
            }
            if (count > 0)
            {
                msg.Chat.SendMessage(msg.Sender.FullName + ", why are you using bad language?");
                return;
            }
            if (msg.Body.IndexOf(trigger) == 0)
            {
                // Remove trigger string and make lower case
                string command = msg.Body.Remove(0, trigger.Length).ToLower();
                // Send processed message back to skype chat window
                //skype.SendMessage(msg.Sender.Handle, nick + " Says: " + ProcessCommand(command));
                msg.Chat.SendMessage(ProcessCommand(command));
                WriteLine(ProcessCommand(command));
            }
        }
        public void WriteLine(string s)
        {
            txtLog.AppendText(s);
            txtLog.AppendText("\r\n");
        }
        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string text = txtMessage.Text.Trim();
                if (String.IsNullOrEmpty(text)) return;
                WriteLine(text);
                IChat ichat = skype.get_Chat(Contacts.GetItemText(Contacts.SelectedItem));
                ichat.SendMessage("test");
                txtMessage.Clear();
            }
        }
        private string ProcessCommand(string str)
        {
            string result = "";
            switch (str)
            {
                case "hello":
                    result = "Hello!";
                    break;
                case "test":
                    foreach (IChat chat in skype.Chats)
                    {
                        result += chat.FriendlyName + ", ";
                    }
                    break;
                case "help":
                    result = "Sorry no help available";
                    break;
                case "spam":
                    result = "spam";//will cause spamm if haz a ! added
                    break;
                case "date":
                    result = "Current Date is: " +
                             DateTime.Now.ToLongDateString();
                    break;
                case "time":
                    result = "Current Time is: " +
                             DateTime.Now.ToLongTimeString();
                    break;
                default:
                    result = "Sorry, I do not recognize your command";
                    break;
            }
            return result;
        }
        private void contacts()
        {
            Contacts.Items.Clear();
            foreach (IChat chat in skype.Chats)
            {
                Contacts.Items.Add(chat.Name);
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            contacts();
        }
    }
}