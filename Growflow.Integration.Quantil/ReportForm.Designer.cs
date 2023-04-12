namespace Growflo.Integration.Windows
{
    partial class ReportForm
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
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.fromLabel = new System.Windows.Forms.Label();
            this.fromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.buttonPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Controls.Add(this.okButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 83);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.buttonPanel.Size = new System.Drawing.Size(367, 50);
            this.buttonPanel.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.cancelButton.Location = new System.Drawing.Point(98, 5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(132, 40);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.okButton.Location = new System.Drawing.Point(230, 5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(132, 40);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.fromLabel);
            this.mainPanel.Controls.Add(this.fromDateTimePicker);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(367, 83);
            this.mainPanel.TabIndex = 1;
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(98, 33);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(38, 15);
            this.fromLabel.TabIndex = 1;
            this.fromLabel.Text = "From:";
            // 
            // fromDateTimePicker
            // 
            this.fromDateTimePicker.CustomFormat = "dd/MM/yy";
            this.fromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDateTimePicker.Location = new System.Drawing.Point(173, 33);
            this.fromDateTimePicker.Name = "fromDateTimePicker";
            this.fromDateTimePicker.Size = new System.Drawing.Size(80, 23);
            this.fromDateTimePicker.TabIndex = 0;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 133);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.buttonPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ReportForm";
            this.Text = "Run Report";
            this.buttonPanel.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.DateTimePicker fromDateTimePicker;
    }
}