using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Discord;

namespace InfiniBot
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Bot bot = new Bot();

        public Form()
        {
            InitializeComponent();
            List<TokenContainer> tokenContainers = Data.GetContainers<TokenContainer>(Data.FILE_PATH + Data.TOKEN_FILE);
            string[] names = new string[tokenContainers.Count];
            for (int i = 0; i < tokenContainers.Count; i++)
            {
                names[i] = tokenContainers[i].name;
            }
            comboBoxToken.Items.AddRange(names);
            if (comboBoxToken.Items.Count > 0)
            {
                comboBoxToken.Text = comboBoxToken.Items[0].ToString();
            }
        }

        public Task Log(LogMessage message)
        {
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString());
            lvi.SubItems.Add(message.Severity.ToString());

            if (!string.IsNullOrEmpty(message.Source))
            {
                lvi.SubItems.Add(message.Source);
            }
            else
            {
                lvi.SubItems.Add("Unknown");
            }

            if (!string.IsNullOrEmpty(message.Message))
            {
                lvi.SubItems.Add(message.Message);
            }
            else
            {
                if(message.Exception != null)
                {
                    lvi.SubItems.Add("Exception thrown");
                }
                else
                {
                    lvi.SubItems.Add("Empty");
                }
            }

            if (message.Exception != null)
            {
                lvi.SubItems.Add(message.Exception.ToString());
            }
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    lvi.ForeColor = System.Drawing.Color.Red;
                    break;
                case LogSeverity.Warning:
                    lvi.ForeColor = System.Drawing.Color.DarkOrange;
                    break;
                case LogSeverity.Info:
                    lvi.ForeColor = System.Drawing.Color.Black;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    lvi.ForeColor = System.Drawing.Color.DarkGray;
                    break;
            }
            Invoke((Action)delegate
            {
                listViewConsole.Items.Add(lvi);
            });
            #region Console
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            //Console.WriteLine(message);
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,-8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();
            #endregion
            return Task.CompletedTask;
        }

        private async void buttonStartBot_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(comboBoxToken.Text))
            {
                /*
                if(!comboBoxToken.Items.Contains(comboBoxToken.Text))
                {
                    comboBoxToken.Items.Add(comboBoxToken.Text);
                    Data.AddToken(comboBoxToken.Text);
                }*/
                string token = comboBoxToken.Text;
                TokenContainer tc = Data.GetContainers<TokenContainer>(Data.FILE_PATH + Data.TOKEN_FILE).FirstOrDefault(x => x.name == comboBoxToken.Text || x.token == comboBoxToken.Text);
                if(tc != null)
                {
                    token = tc.token;
                }
                await bot.RunBotAsync(token);
            }
            else
            {
                MessageBox.Show("Please pick a token before starting the bot", "ERROR: Missing token!");
            }
        }

        private async void buttonStopBot_Click(object sender, EventArgs e)
        {
            await bot.StopBotAsync();
        }

        public Task BotConnected()
        {
            Invoke((Action)delegate
            {
                buttonStartBot.Enabled = false;
                buttonStopBot.Enabled = true;

                if (Data.GetContainers<TokenContainer>(Data.FILE_PATH + Data.TOKEN_FILE).FirstOrDefault(x => x.name == comboBoxToken.Text || x.token == comboBoxToken.Text) == null)
                {
                    try
                    {
                        Console.WriteLine(bot.client.CurrentUser);
                        Data.AddContainer(new TokenContainer(bot.client.CurrentUser.Username, comboBoxToken.Text), Data.FILE_PATH + Data.TOKEN_FILE);
                        comboBoxToken.Items.Add(bot.client.CurrentUser.Username);
                        comboBoxToken.Text = bot.client.CurrentUser.Username;
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong while trying to add the token to the list.", "ERROR: Could not add token");
                    }
                }
            });
            return Task.CompletedTask;
        }

        public Task BotDisconnected(Exception e)
        {
            Invoke((Action)delegate
            {
                buttonStopBot.Enabled = false;
                buttonStartBot.Enabled = true;
            });
            return Task.CompletedTask;
        }

        // Compiles a message with the information from the selected listView item and displays it to the user.
        private void buttonConsoleView_Click(object sender, EventArgs e)
        {
            if (listViewConsole.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewConsole.SelectedItems)
                {
                    string message = "";
                    if (item.SubItems.Count > 0)
                    {
                        message += $"{listViewConsole.Columns[0].Text}: {item.SubItems[0].Text}";
                    }
                    if (item.SubItems.Count > 1)
                    {
                        message += $"\n{listViewConsole.Columns[1].Text}: {item.SubItems[1].Text}";
                    }
                    if (item.SubItems.Count > 2)
                    {
                        message += $"\n{listViewConsole.Columns[2].Text}: {item.SubItems[2].Text}";
                    }
                    if (item.SubItems.Count > 3)
                    {
                        message += $"\n{listViewConsole.Columns[3].Text}:\n{item.SubItems[3].Text}";
                    }
                    if (item.SubItems.Count > 4)
                    {
                        message += $"\n\n{listViewConsole.Columns[4].Text}:\n{item.SubItems[4].Text}";
                    }

                    MessageBox.Show(message,
                        "InfiniBot");
                }
            }
        }

        private void buttonConsoleClear_Click(object sender, EventArgs e)
        {
            listViewConsole.Items.Clear();
        }

        private void buttonTokenRemove_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxToken.Text))
            {
                TokenContainer tc = Data.GetContainers<TokenContainer>(Data.FILE_PATH + Data.TOKEN_FILE).FirstOrDefault(x => x.name == comboBoxToken.Text || x.token == comboBoxToken.Text);
                if (tc != null)
                {
                    Data.RemoveContainer(tc, Data.FILE_PATH + Data.TOKEN_FILE);
                    comboBoxToken.Items.Remove(comboBoxToken.Text);
                    comboBoxToken.Text = "";
                }
                else
                {
                    MessageBox.Show("The token you entered does not exist.", "ERROR: Could not find token!");
                }
            }
            else
            {
                MessageBox.Show("You need to select a token before using this button.", "ERROR: Missing token!");
            }
        }

        private void comboBoxToken_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form_Load(object sender, EventArgs e)
        {

        }
    }
}
