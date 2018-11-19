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
            this.buttonView = new System.Windows.Forms.Button();
            this.buttonStartBot = new System.Windows.Forms.Button();
            this.buttonStopBot = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
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
            this.listViewConsole.Location = new System.Drawing.Point(13, 13);
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
            // buttonView
            // 
            this.buttonView.Location = new System.Drawing.Point(578, 386);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(75, 23);
            this.buttonView.TabIndex = 1;
            this.buttonView.Text = "View";
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // buttonStartBot
            // 
            this.buttonStartBot.Location = new System.Drawing.Point(13, 386);
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
            this.buttonStopBot.Location = new System.Drawing.Point(94, 386);
            this.buttonStopBot.Name = "buttonStopBot";
            this.buttonStopBot.Size = new System.Drawing.Size(75, 23);
            this.buttonStopBot.TabIndex = 3;
            this.buttonStopBot.Text = "Stop Bot";
            this.buttonStopBot.UseVisualStyleBackColor = true;
            this.buttonStopBot.Click += new System.EventHandler(this.buttonStopBot_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(497, 386);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 419);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonStopBot);
            this.Controls.Add(this.buttonStartBot);
            this.Controls.Add(this.buttonView);
            this.Controls.Add(this.listViewConsole);
            this.Name = "Form";
            this.Text = "InfiniBot";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewConsole;
        private System.Windows.Forms.ColumnHeader Time;
        private System.Windows.Forms.ColumnHeader Severity;
        private System.Windows.Forms.ColumnHeader Source;
        private System.Windows.Forms.ColumnHeader Message;
        private System.Windows.Forms.ColumnHeader Exception;
        private System.Windows.Forms.Button buttonView;
        private System.Windows.Forms.Button buttonStartBot;
        private System.Windows.Forms.Button buttonStopBot;
        private System.Windows.Forms.Button buttonClear;
    }
}

