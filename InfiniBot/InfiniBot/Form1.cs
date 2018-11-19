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
        }

        public Task Log(LogMessage message)
        {
            ListViewItem lvi = new ListViewItem(DateTime.Now.ToString());
            lvi.SubItems.Add(message.Severity.ToString());
            if (!string.IsNullOrEmpty(message.Source))
            {
                lvi.SubItems.Add(message.Source);
                if (!string.IsNullOrEmpty(message.Message))
                {
                    lvi.SubItems.Add(message.Message);
                    if (message.Exception != null)
                    {
                        lvi.SubItems.Add(message.Exception.ToString());
                    }
                }
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

        private void buttonView_Click(object sender, EventArgs e)
        {
            // Compiles a message with the information from the selected listView item and displays it to the user.
            if (listViewConsole.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewConsole.SelectedItems)
                {
                    string message = "";
                    if (item.SubItems.Count > 0)
                    {

                        message += $"Time: {item.SubItems[0].Text}";
                    }
                    if (item.SubItems.Count > 1)
                    {
                        message += $"\nSeverity: {item.SubItems[1].Text}";
                    }
                    if (item.SubItems.Count > 2)
                    {
                        message += $"\nSource: {item.SubItems[2].Text}";
                    }
                    if (item.SubItems.Count > 3)
                    {
                        message += "\nMessage:\n" + item.SubItems[3].Text;
                    }
                    if (item.SubItems.Count > 4)
                    {
                        message += "\n\nException:\n" + item.SubItems[4].Text;
                    }

                    MessageBox.Show(message,
                        "InfiniBot");
                }
            }
        }

        private async void buttonStartBot_Click(object sender, EventArgs e)
        {
            await bot.RunBotAsync();
            buttonStartBot.Enabled = false;
            buttonStopBot.Enabled = true;
        }

        private async void buttonStopBot_Click(object sender, EventArgs e)
        {
            await bot.StopBotAsync();
            buttonStopBot.Enabled = false;
            buttonStartBot.Enabled = true;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            listViewConsole.Items.Clear();
        }
    }
}
