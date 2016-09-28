namespace DogeChat
{
    partial class Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxWindow = new System.Windows.Forms.TextBox();
            this.checkBoxImportant = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonSend
            // 
            this.buttonSend.FlatAppearance.BorderSize = 2;
            this.buttonSend.Location = new System.Drawing.Point(297, 229);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(75, 20);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 229);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(279, 20);
            this.textBoxMessage.TabIndex = 0;
            this.textBoxMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buttonSend_KeyDown);
            // 
            // textBoxWindow
            // 
            this.textBoxWindow.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWindow.Location = new System.Drawing.Point(12, 12);
            this.textBoxWindow.Multiline = true;
            this.textBoxWindow.Name = "textBoxWindow";
            this.textBoxWindow.ReadOnly = true;
            this.textBoxWindow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxWindow.Size = new System.Drawing.Size(431, 211);
            this.textBoxWindow.TabIndex = 2;
            this.textBoxWindow.TabStop = false;
            // 
            // checkBoxImportant
            // 
            this.checkBoxImportant.AutoSize = true;
            this.checkBoxImportant.Location = new System.Drawing.Point(378, 232);
            this.checkBoxImportant.Name = "checkBoxImportant";
            this.checkBoxImportant.Size = new System.Drawing.Size(70, 17);
            this.checkBoxImportant.TabIndex = 3;
            this.checkBoxImportant.Text = "Important";
            this.checkBoxImportant.UseVisualStyleBackColor = true;
            // 
            // Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 261);
            this.Controls.Add(this.checkBoxImportant);
            this.Controls.Add(this.textBoxWindow);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.buttonSend);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(472, 300);
            this.MinimumSize = new System.Drawing.Size(472, 300);
            this.Name = "Chat";
            this.Text = "Doge Chat";
            this.Load += new System.EventHandler(this.Chat_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxWindow;
        private System.Windows.Forms.CheckBox checkBoxImportant;
    }
}

