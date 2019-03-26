namespace TestClient
{
    partial class ClientForm
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
            this.txtChat = new System.Windows.Forms.TextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.picHostUL = new System.Windows.Forms.PictureBox();
            this.picNickUL = new System.Windows.Forms.PictureBox();
            this.picTxtChatUL = new System.Windows.Forms.PictureBox();
            this.lblNick = new System.Windows.Forms.Label();
            this.lblChat = new System.Windows.Forms.Label();
            this.lblMessages = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picHostUL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNickUL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTxtChatUL)).BeginInit();
            this.SuspendLayout();
            // 
            // txtChat
            // 
            this.txtChat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.txtChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtChat.Enabled = false;
            this.txtChat.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtChat.ForeColor = System.Drawing.Color.White;
            this.txtChat.Location = new System.Drawing.Point(48, 35);
            this.txtChat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtChat.MaxLength = 200;
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(597, 22);
            this.txtChat.TabIndex = 3;
            this.txtChat.EnabledChanged += new System.EventHandler(this.txtChat_EnabledChanged);
            this.txtChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChat_KeyDown);
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.txtHost.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHost.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtHost.ForeColor = System.Drawing.Color.White;
            this.txtHost.Location = new System.Drawing.Point(357, 3);
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(288, 22);
            this.txtHost.TabIndex = 2;
            this.txtHost.EnabledChanged += new System.EventHandler(this.txtHost_EnabledChanged);
            this.txtHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHost_KeyDown);
            // 
            // txtNick
            // 
            this.txtNick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.txtNick.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNick.Font = new System.Drawing.Font("Segoe UI Semilight", 12F);
            this.txtNick.ForeColor = System.Drawing.Color.White;
            this.txtNick.Location = new System.Drawing.Point(47, 3);
            this.txtNick.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNick.MaxLength = 32;
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(262, 22);
            this.txtNick.TabIndex = 1;
            this.txtNick.EnabledChanged += new System.EventHandler(this.txtNick_EnabledChanged);
            this.txtNick.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNick_KeyDown);
            // 
            // rtbMessages
            // 
            this.rtbMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.rtbMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbMessages.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbMessages.ForeColor = System.Drawing.Color.White;
            this.rtbMessages.Location = new System.Drawing.Point(3, 90);
            this.rtbMessages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.ReadOnly = true;
            this.rtbMessages.Size = new System.Drawing.Size(643, 384);
            this.rtbMessages.TabIndex = 4;
            this.rtbMessages.Text = "";
            this.rtbMessages.Visible = false;
            this.rtbMessages.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RtbMessages_LinkClicked);
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.lblHost.Location = new System.Drawing.Point(313, 4);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(40, 19);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host:";
            // 
            // picHostUL
            // 
            this.picHostUL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picHostUL.BackColor = System.Drawing.Color.White;
            this.picHostUL.Location = new System.Drawing.Point(357, 25);
            this.picHostUL.Name = "picHostUL";
            this.picHostUL.Size = new System.Drawing.Size(288, 1);
            this.picHostUL.TabIndex = 4;
            this.picHostUL.TabStop = false;
            // 
            // picNickUL
            // 
            this.picNickUL.BackColor = System.Drawing.Color.White;
            this.picNickUL.Location = new System.Drawing.Point(47, 25);
            this.picNickUL.Name = "picNickUL";
            this.picNickUL.Size = new System.Drawing.Size(262, 1);
            this.picNickUL.TabIndex = 5;
            this.picNickUL.TabStop = false;
            // 
            // picTxtChatUL
            // 
            this.picTxtChatUL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picTxtChatUL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.picTxtChatUL.Location = new System.Drawing.Point(48, 57);
            this.picTxtChatUL.Name = "picTxtChatUL";
            this.picTxtChatUL.Size = new System.Drawing.Size(597, 1);
            this.picTxtChatUL.TabIndex = 5;
            this.picTxtChatUL.TabStop = false;
            // 
            // lblNick
            // 
            this.lblNick.AutoSize = true;
            this.lblNick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.lblNick.Location = new System.Drawing.Point(4, 4);
            this.lblNick.Name = "lblNick";
            this.lblNick.Size = new System.Drawing.Size(39, 19);
            this.lblNick.TabIndex = 0;
            this.lblNick.Text = "Nick:";
            // 
            // lblChat
            // 
            this.lblChat.AutoSize = true;
            this.lblChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.lblChat.Location = new System.Drawing.Point(4, 36);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(40, 19);
            this.lblChat.TabIndex = 0;
            this.lblChat.Text = "Chat:";
            // 
            // lblMessages
            // 
            this.lblMessages.AutoSize = true;
            this.lblMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.lblMessages.Location = new System.Drawing.Point(4, 69);
            this.lblMessages.Name = "lblMessages";
            this.lblMessages.Size = new System.Drawing.Size(71, 19);
            this.lblMessages.TabIndex = 0;
            this.lblMessages.Text = "Messages:";
            // 
            // ClientForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.ClientSize = new System.Drawing.Size(649, 477);
            this.Controls.Add(this.picTxtChatUL);
            this.Controls.Add(this.picNickUL);
            this.Controls.Add(this.picHostUL);
            this.Controls.Add(this.lblMessages);
            this.Controls.Add(this.lblChat);
            this.Controls.Add(this.lblNick);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.rtbMessages);
            this.Controls.Add(this.txtNick);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.txtChat);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Semilight", 10F);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client - Not connected";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picHostUL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNickUL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTxtChatUL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChat;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.PictureBox picHostUL;
        private System.Windows.Forms.PictureBox picNickUL;
        private System.Windows.Forms.PictureBox picTxtChatUL;
        private System.Windows.Forms.Label lblNick;
        private System.Windows.Forms.Label lblChat;
        private System.Windows.Forms.Label lblMessages;
    }
}

