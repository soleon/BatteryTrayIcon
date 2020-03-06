namespace Percentage
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LowColorButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ChargingColorButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.CriticalColorButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.HighNotificationComboBox = new System.Windows.Forms.ComboBox();
            this.LowNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.HighNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.FullNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.CriticalNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.AutoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.RefreshSecondsComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LowColorButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ChargingColorButton);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.CriticalColorButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(11, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(159, 185);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Colors";
            // 
            // LowColorButton
            // 
            this.LowColorButton.Location = new System.Drawing.Point(104, 86);
            this.LowColorButton.Margin = new System.Windows.Forms.Padding(2);
            this.LowColorButton.Name = "LowColorButton";
            this.LowColorButton.Size = new System.Drawing.Size(34, 34);
            this.LowColorButton.TabIndex = 1;
            this.LowColorButton.Tag = "LowColor";
            this.LowColorButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Low";
            // 
            // ChargingColorButton
            // 
            this.ChargingColorButton.Location = new System.Drawing.Point(104, 139);
            this.ChargingColorButton.Margin = new System.Windows.Forms.Padding(2);
            this.ChargingColorButton.Name = "ChargingColorButton";
            this.ChargingColorButton.Size = new System.Drawing.Size(34, 34);
            this.ChargingColorButton.TabIndex = 2;
            this.ChargingColorButton.Tag = "ChargingColor";
            this.ChargingColorButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 142);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Charging";
            // 
            // CriticalColorButton
            // 
            this.CriticalColorButton.Location = new System.Drawing.Point(104, 32);
            this.CriticalColorButton.Margin = new System.Windows.Forms.Padding(2);
            this.CriticalColorButton.Name = "CriticalColorButton";
            this.CriticalColorButton.Size = new System.Drawing.Size(34, 34);
            this.CriticalColorButton.TabIndex = 0;
            this.CriticalColorButton.Tag = "CriticalColor";
            this.CriticalColorButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Critical";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.HighNotificationComboBox);
            this.groupBox2.Controls.Add(this.LowNotificationCheckBox);
            this.groupBox2.Controls.Add(this.HighNotificationCheckBox);
            this.groupBox2.Controls.Add(this.FullNotificationCheckBox);
            this.groupBox2.Controls.Add(this.CriticalNotificationCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(174, 11);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(179, 184);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Notifications";
            // 
            // HighNotificationComboBox
            // 
            this.HighNotificationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HighNotificationComboBox.FormattingEnabled = true;
            this.HighNotificationComboBox.Location = new System.Drawing.Point(105, 104);
            this.HighNotificationComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.HighNotificationComboBox.Name = "HighNotificationComboBox";
            this.HighNotificationComboBox.Size = new System.Drawing.Size(53, 33);
            this.HighNotificationComboBox.TabIndex = 3;
            // 
            // LowNotificationCheckBox
            // 
            this.LowNotificationCheckBox.AutoSize = true;
            this.LowNotificationCheckBox.Location = new System.Drawing.Point(18, 71);
            this.LowNotificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.LowNotificationCheckBox.Name = "LowNotificationCheckBox";
            this.LowNotificationCheckBox.Size = new System.Drawing.Size(70, 29);
            this.LowNotificationCheckBox.TabIndex = 1;
            this.LowNotificationCheckBox.Tag = "LowNotification";
            this.LowNotificationCheckBox.Text = "Low";
            this.LowNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // HighNotificationCheckBox
            // 
            this.HighNotificationCheckBox.AutoSize = true;
            this.HighNotificationCheckBox.Location = new System.Drawing.Point(18, 106);
            this.HighNotificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.HighNotificationCheckBox.Name = "HighNotificationCheckBox";
            this.HighNotificationCheckBox.Size = new System.Drawing.Size(80, 29);
            this.HighNotificationCheckBox.TabIndex = 2;
            this.HighNotificationCheckBox.Tag = "HighNotification";
            this.HighNotificationCheckBox.Text = "High:";
            this.HighNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // FullNotificationCheckBox
            // 
            this.FullNotificationCheckBox.AutoSize = true;
            this.FullNotificationCheckBox.Location = new System.Drawing.Point(18, 141);
            this.FullNotificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.FullNotificationCheckBox.Name = "FullNotificationCheckBox";
            this.FullNotificationCheckBox.Size = new System.Drawing.Size(65, 29);
            this.FullNotificationCheckBox.TabIndex = 4;
            this.FullNotificationCheckBox.Tag = "FullNotification";
            this.FullNotificationCheckBox.Text = "Full";
            this.FullNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // CriticalNotificationCheckBox
            // 
            this.CriticalNotificationCheckBox.AutoSize = true;
            this.CriticalNotificationCheckBox.Location = new System.Drawing.Point(18, 36);
            this.CriticalNotificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.CriticalNotificationCheckBox.Name = "CriticalNotificationCheckBox";
            this.CriticalNotificationCheckBox.Size = new System.Drawing.Size(90, 29);
            this.CriticalNotificationCheckBox.TabIndex = 0;
            this.CriticalNotificationCheckBox.Tag = "CriticalNotification";
            this.CriticalNotificationCheckBox.Text = "Critical";
            this.CriticalNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 204);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "Refresh every";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(241, 283);
            this.ResetButton.Margin = new System.Windows.Forms.Padding(2);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(112, 34);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // AutoStartCheckBox
            // 
            this.AutoStartCheckBox.AutoSize = true;
            this.AutoStartCheckBox.Location = new System.Drawing.Point(11, 241);
            this.AutoStartCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AutoStartCheckBox.Name = "AutoStartCheckBox";
            this.AutoStartCheckBox.Size = new System.Drawing.Size(191, 29);
            this.AutoStartCheckBox.TabIndex = 3;
            this.AutoStartCheckBox.Tag = "AutoStart";
            this.AutoStartCheckBox.Text = "Start with Windows";
            this.AutoStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // RefreshSecondsComboBox
            // 
            this.RefreshSecondsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RefreshSecondsComboBox.FormattingEnabled = true;
            this.RefreshSecondsComboBox.Location = new System.Drawing.Point(139, 200);
            this.RefreshSecondsComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.RefreshSecondsComboBox.Name = "RefreshSecondsComboBox";
            this.RefreshSecondsComboBox.Size = new System.Drawing.Size(53, 33);
            this.RefreshSecondsComboBox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 204);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 25);
            this.label5.TabIndex = 7;
            this.label5.Text = "seconds";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 330);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.RefreshSecondsComboBox);
            this.Controls.Add(this.AutoStartCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button LowColorButton;
        private System.Windows.Forms.Button CriticalColorButton;
        private System.Windows.Forms.Button ChargingColorButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox FullNotificationCheckBox;
        private System.Windows.Forms.CheckBox HighNotificationCheckBox;
        private System.Windows.Forms.CheckBox LowNotificationCheckBox;
        private System.Windows.Forms.CheckBox CriticalNotificationCheckBox;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.CheckBox AutoStartCheckBox;
        private System.Windows.Forms.ComboBox HighNotificationComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox RefreshSecondsComboBox;
        private System.Windows.Forms.Label label5;
    }
}