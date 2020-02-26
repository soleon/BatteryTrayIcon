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
            this.label3 = new System.Windows.Forms.Label();
            this.CriticalColorButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ChargingColorButton = new System.Windows.Forms.Button();
            this.LowColorButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.CriticalNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.FullNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.HighNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.LowNotificationCheckBox = new System.Windows.Forms.CheckBox();
            this.ResetButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ChargingColorButton);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.CriticalColorButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.LowColorButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(342, 128);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Colors";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 25);
            this.label3.TabIndex = 2;
            this.label3.Text = "Critical";
            // 
            // CriticalColorButton
            // 
            this.CriticalColorButton.Location = new System.Drawing.Point(102, 33);
            this.CriticalColorButton.Name = "CriticalColorButton";
            this.CriticalColorButton.Size = new System.Drawing.Size(34, 34);
            this.CriticalColorButton.TabIndex = 6;
            this.CriticalColorButton.Tag = "CriticalColor";
            this.CriticalColorButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(197, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 25);
            this.label4.TabIndex = 3;
            this.label4.Text = "Charging";
            // 
            // ChargingColorButton
            // 
            this.ChargingColorButton.Location = new System.Drawing.Point(287, 33);
            this.ChargingColorButton.Name = "ChargingColorButton";
            this.ChargingColorButton.Size = new System.Drawing.Size(34, 34);
            this.ChargingColorButton.TabIndex = 7;
            this.ChargingColorButton.Tag = "ChargingColor";
            this.ChargingColorButton.UseVisualStyleBackColor = true;
            // 
            // LowColorButton
            // 
            this.LowColorButton.Location = new System.Drawing.Point(102, 73);
            this.LowColorButton.Name = "LowColorButton";
            this.LowColorButton.Size = new System.Drawing.Size(34, 34);
            this.LowColorButton.TabIndex = 5;
            this.LowColorButton.Tag = "LowColor";
            this.LowColorButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Low";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CriticalNotificationCheckBox);
            this.groupBox2.Controls.Add(this.FullNotificationCheckBox);
            this.groupBox2.Controls.Add(this.HighNotificationCheckBox);
            this.groupBox2.Controls.Add(this.LowNotificationCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(342, 118);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Notifications";
            // 
            // CriticalNotificationCheckBox
            // 
            this.CriticalNotificationCheckBox.AutoSize = true;
            this.CriticalNotificationCheckBox.Location = new System.Drawing.Point(12, 36);
            this.CriticalNotificationCheckBox.Name = "CriticalNotificationCheckBox";
            this.CriticalNotificationCheckBox.Size = new System.Drawing.Size(90, 29);
            this.CriticalNotificationCheckBox.TabIndex = 0;
            this.CriticalNotificationCheckBox.Tag = "CriticalNotification";
            this.CriticalNotificationCheckBox.Text = "Critical";
            this.CriticalNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // FullNotificationCheckBox
            // 
            this.FullNotificationCheckBox.AutoSize = true;
            this.FullNotificationCheckBox.Location = new System.Drawing.Point(197, 71);
            this.FullNotificationCheckBox.Name = "FullNotificationCheckBox";
            this.FullNotificationCheckBox.Size = new System.Drawing.Size(65, 29);
            this.FullNotificationCheckBox.TabIndex = 3;
            this.FullNotificationCheckBox.Tag = "FullNotification";
            this.FullNotificationCheckBox.Text = "Full";
            this.FullNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // HighNotificationCheckBox
            // 
            this.HighNotificationCheckBox.AutoSize = true;
            this.HighNotificationCheckBox.Location = new System.Drawing.Point(197, 36);
            this.HighNotificationCheckBox.Name = "HighNotificationCheckBox";
            this.HighNotificationCheckBox.Size = new System.Drawing.Size(76, 29);
            this.HighNotificationCheckBox.TabIndex = 2;
            this.HighNotificationCheckBox.Tag = "HighNotification";
            this.HighNotificationCheckBox.Text = "High";
            this.HighNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // LowNotificationCheckBox
            // 
            this.LowNotificationCheckBox.AutoSize = true;
            this.LowNotificationCheckBox.Location = new System.Drawing.Point(12, 71);
            this.LowNotificationCheckBox.Name = "LowNotificationCheckBox";
            this.LowNotificationCheckBox.Size = new System.Drawing.Size(70, 29);
            this.LowNotificationCheckBox.TabIndex = 1;
            this.LowNotificationCheckBox.Tag = "LowNotification";
            this.LowNotificationCheckBox.Text = "Low";
            this.LowNotificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(242, 280);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(112, 34);
            this.ResetButton.TabIndex = 2;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 326);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
    }
}