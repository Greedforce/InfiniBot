namespace InfiniBot
{
    partial class Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewConsole = new System.Windows.Forms.ListView();
            this.Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Severity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Source = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Exception = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonConsoleView = new System.Windows.Forms.Button();
            this.buttonStartBot = new System.Windows.Forms.Button();
            this.buttonStopBot = new System.Windows.Forms.Button();
            this.buttonConsoleClear = new System.Windows.Forms.Button();
            this.comboBoxToken = new System.Windows.Forms.ComboBox();
            this.buttonTokenRemove = new System.Windows.Forms.Button();
            this.groupBoxToken = new System.Windows.Forms.GroupBox();
            this.groupBoxToken.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewConsole
            // 
            this.listViewConsole.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Time,
            this.Severity,
            this.Source,
            this.Message,
            this.Exception});
            this.listViewConsole.FullRowSelect = true;
            this.listViewConsole.GridLines = true;
            this.listViewConsole.Location = new System.Drawing.Point(12, 69);
            this.listViewConsole.Name = "listViewConsole";
            this.listViewConsole.ShowItemToolTips = true;
            this.listViewConsole.Size = new System.Drawing.Size(640, 367);
            this.listViewConsole.TabIndex = 0;
            this.listViewConsole.UseCompatibleStateImageBehavior = false;
            this.listViewConsole.View = System.Windows.Forms.View.Details;
            // 
            // Time
            // 
            this.Time.Text = "Time";
            this.Time.Width = 110;
            // 
            // Severity
            // 
            this.Severity.Text = "Severity";
            // 
            // Source
            // 
            this.Source.Text = "Source";
            // 
            // Message
            // 
            this.Message.Text = "Message";
            this.Message.Width = 300;
            // 
            // Exception
            // 
            this.Exception.Text = "Exception";
            this.Exception.Width = 110;
            // 
            // buttonConsoleView
            // 
            this.buttonConsoleView.Location = new System.Drawing.Point(658, 70);
            this.buttonConsoleView.Name = "buttonConsoleView";
            this.buttonConsoleView.Size = new System.Drawing.Size(75, 23);
            this.buttonConsoleView.TabIndex = 1;
            this.buttonConsoleView.Text = "View";
            this.buttonConsoleView.UseVisualStyleBackColor = true;
            this.buttonConsoleView.Click += new System.EventHandler(this.buttonConsoleView_Click);
            // 
            // buttonStartBot
            // 
            this.buttonStartBot.Location = new System.Drawing.Point(658, 9);
            this.buttonStartBot.Name = "buttonStartBot";
            this.buttonStartBot.Size = new System.Drawing.Size(75, 23);
            this.buttonStartBot.TabIndex = 2;
            this.buttonStartBot.Text = "Start Bot";
            this.buttonStartBot.UseVisualStyleBackColor = true;
            this.buttonStartBot.Click += new System.EventHandler(this.buttonStartBot_Click);
            // 
            // buttonStopBot
            // 
            this.buttonStopBot.Enabled = false;
            this.buttonStopBot.Location = new System.Drawing.Point(658, 38);
            this.buttonStopBot.Name = "buttonStopBot";
            this.buttonStopBot.Size = new System.Drawing.Size(75, 23);
            this.buttonStopBot.TabIndex = 3;
            this.buttonStopBot.Text = "Stop Bot";
            this.buttonStopBot.UseVisualStyleBackColor = true;
            this.buttonStopBot.Click += new System.EventHandler(this.buttonStopBot_Click);
            // 
            // buttonConsoleClear
            // 
            this.buttonConsoleClear.Location = new System.Drawing.Point(658, 99);
            this.buttonConsoleClear.Name = "buttonConsoleClear";
            this.buttonConsoleClear.Size = new System.Drawing.Size(75, 23);
            this.buttonConsoleClear.TabIndex = 4;
            this.buttonConsoleClear.Text = "Clear";
            this.buttonConsoleClear.UseVisualStyleBackColor = true;
            this.buttonConsoleClear.Click += new System.EventHandler(this.buttonConsoleClear_Click);
            // 
            // comboBoxToken
            // 
            this.comboBoxToken.FormattingEnabled = true;
            this.comboBoxToken.Location = new System.Drawing.Point(15, 23);
            this.comboBoxToken.Name = "comboBoxToken";
            this.comboBoxToken.Size = new System.Drawing.Size(529, 21);
            this.comboBoxToken.TabIndex = 5;
            this.comboBoxToken.SelectedIndexChanged += new System.EventHandler(this.comboBoxToken_SelectedIndexChanged);
            // 
            // buttonTokenRemove
            // 
            this.buttonTokenRemove.Location = new System.Drawing.Point(550, 22);
            this.buttonTokenRemove.Name = "buttonTokenRemove";
            this.buttonTokenRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonTokenRemove.TabIndex = 7;
            this.buttonTokenRemove.Text = "Remove";
            this.buttonTokenRemove.UseVisualStyleBackColor = true;
            this.buttonTokenRemove.Click += new System.EventHandler(this.buttonTokenRemove_Click);
            // 
            // groupBoxToken
            // 
            this.groupBoxToken.Controls.Add(this.comboBoxToken);
            this.groupBoxToken.Controls.Add(this.buttonTokenRemove);
            this.groupBoxToken.Location = new System.Drawing.Point(12, 3);
            this.groupBoxToken.Name = "groupBoxToken";
            this.groupBoxToken.Size = new System.Drawing.Size(640, 60);
            this.groupBoxToken.TabIndex = 8;
            this.groupBoxToken.TabStop = false;
            this.groupBoxToken.Text = "Token";
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 447);
            this.Controls.Add(this.groupBoxToken);
            this.Controls.Add(this.buttonConsoleClear);
            this.Controls.Add(this.buttonStopBot);
            this.Controls.Add(this.buttonStartBot);
            this.Controls.Add(this.buttonConsoleView);
            this.Controls.Add(this.listViewConsole);
            this.Name = "Form";
            this.Text = "InfiniBot";
            this.Load += new System.EventHandler(this.Form_Load);
            this.groupBoxToken.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewConsole;
        private System.Windows.Forms.ColumnHeader Time;
        private System.Windows.Forms.ColumnHeader Severity;
        private System.Windows.Forms.ColumnHeader Source;
        private System.Windows.Forms.ColumnHeader Message;
        private System.Windows.Forms.ColumnHeader Exception;
        private System.Windows.Forms.Button buttonConsoleView;
        private System.Windows.Forms.Button buttonStartBot;
        private System.Windows.Forms.Button buttonStopBot;
        private System.Windows.Forms.Button buttonConsoleClear;
        private System.Windows.Forms.ComboBox comboBoxToken;
        private System.Windows.Forms.Button buttonTokenRemove;
        private System.Windows.Forms.GroupBox groupBoxToken;
    }
}

