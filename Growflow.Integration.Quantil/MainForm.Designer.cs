namespace Growflo.Integration.Windows
{
    partial class MainForm
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
            this.uploadCustomerDetailsButton = new System.Windows.Forms.Button();
            this.actionsGroupBox = new System.Windows.Forms.GroupBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.viewSettingsButton = new System.Windows.Forms.Button();
            this.downloadOrdersButton = new System.Windows.Forms.Button();
            this.uploadTaxCodesButton = new System.Windows.Forms.Button();
            this.uploadNominalCodesButton = new System.Windows.Forms.Button();
            this.downloadSingleOrderButton = new System.Windows.Forms.Button();
            this.actionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // uploadCustomerDetailsButton
            // 
            this.uploadCustomerDetailsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadCustomerDetailsButton.Location = new System.Drawing.Point(3, 27);
            this.uploadCustomerDetailsButton.Name = "uploadCustomerDetailsButton";
            this.uploadCustomerDetailsButton.Size = new System.Drawing.Size(534, 59);
            this.uploadCustomerDetailsButton.TabIndex = 0;
            this.uploadCustomerDetailsButton.Text = "Upload Customer Details";
            this.uploadCustomerDetailsButton.UseVisualStyleBackColor = true;
            this.uploadCustomerDetailsButton.Click += new System.EventHandler(this.uploadCustomerDetailsButtonClick);
            // 
            // actionsGroupBox
            // 
            this.actionsGroupBox.Controls.Add(this.exitButton);
            this.actionsGroupBox.Controls.Add(this.viewSettingsButton);
            this.actionsGroupBox.Controls.Add(this.downloadSingleOrderButton);
            this.actionsGroupBox.Controls.Add(this.downloadOrdersButton);
            this.actionsGroupBox.Controls.Add(this.uploadTaxCodesButton);
            this.actionsGroupBox.Controls.Add(this.uploadNominalCodesButton);
            this.actionsGroupBox.Controls.Add(this.uploadCustomerDetailsButton);
            this.actionsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.actionsGroupBox.Location = new System.Drawing.Point(12, 12);
            this.actionsGroupBox.Name = "actionsGroupBox";
            this.actionsGroupBox.Size = new System.Drawing.Size(540, 491);
            this.actionsGroupBox.TabIndex = 1;
            this.actionsGroupBox.TabStop = false;
            this.actionsGroupBox.Text = "Actions";
            // 
            // exitButton
            // 
            this.exitButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.exitButton.Location = new System.Drawing.Point(3, 381);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(534, 59);
            this.exitButton.TabIndex = 14;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // viewSettingsButton
            // 
            this.viewSettingsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.viewSettingsButton.Location = new System.Drawing.Point(3, 322);
            this.viewSettingsButton.Name = "viewSettingsButton";
            this.viewSettingsButton.Size = new System.Drawing.Size(534, 59);
            this.viewSettingsButton.TabIndex = 11;
            this.viewSettingsButton.Text = "View Settings";
            this.viewSettingsButton.UseVisualStyleBackColor = true;
            this.viewSettingsButton.Click += new System.EventHandler(this.viewSettingsButton_Click);
            // 
            // downloadOrdersButton
            // 
            this.downloadOrdersButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.downloadOrdersButton.Location = new System.Drawing.Point(3, 204);
            this.downloadOrdersButton.Name = "downloadOrdersButton";
            this.downloadOrdersButton.Size = new System.Drawing.Size(534, 59);
            this.downloadOrdersButton.TabIndex = 7;
            this.downloadOrdersButton.Text = "Download All Customers && Orders";
            this.downloadOrdersButton.UseVisualStyleBackColor = true;
            this.downloadOrdersButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // uploadTaxCodesButton
            // 
            this.uploadTaxCodesButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadTaxCodesButton.Location = new System.Drawing.Point(3, 145);
            this.uploadTaxCodesButton.Name = "uploadTaxCodesButton";
            this.uploadTaxCodesButton.Size = new System.Drawing.Size(534, 59);
            this.uploadTaxCodesButton.TabIndex = 5;
            this.uploadTaxCodesButton.Text = "Upload Tax Codes";
            this.uploadTaxCodesButton.UseVisualStyleBackColor = true;
            this.uploadTaxCodesButton.Click += new System.EventHandler(this.uploadTaxCodesButton_Click);
            // 
            // uploadNominalCodesButton
            // 
            this.uploadNominalCodesButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.uploadNominalCodesButton.Location = new System.Drawing.Point(3, 86);
            this.uploadNominalCodesButton.Name = "uploadNominalCodesButton";
            this.uploadNominalCodesButton.Size = new System.Drawing.Size(534, 59);
            this.uploadNominalCodesButton.TabIndex = 4;
            this.uploadNominalCodesButton.Text = "Upload Nominal Codes";
            this.uploadNominalCodesButton.UseVisualStyleBackColor = true;
            this.uploadNominalCodesButton.Click += new System.EventHandler(this.uploadNominalCodesButton_Click);
            // 
            // downloadSingleOrderButton
            // 
            this.downloadSingleOrderButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.downloadSingleOrderButton.Location = new System.Drawing.Point(3, 263);
            this.downloadSingleOrderButton.Name = "downloadSingleOrderButton";
            this.downloadSingleOrderButton.Size = new System.Drawing.Size(534, 59);
            this.downloadSingleOrderButton.TabIndex = 10;
            this.downloadSingleOrderButton.Text = "Download Single Order";
            this.downloadSingleOrderButton.UseVisualStyleBackColor = true;
            this.downloadSingleOrderButton.Visible = false;
            this.downloadSingleOrderButton.Click += new System.EventHandler(this.downloadSingleOrderButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 520);
            this.Controls.Add(this.actionsGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "Growflo / Sage Integration";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.actionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uploadCustomerDetailsButton;
        private System.Windows.Forms.GroupBox actionsGroupBox;
        private System.Windows.Forms.Button uploadTaxCodesButton;
        private System.Windows.Forms.Button uploadNominalCodesButton;
        private System.Windows.Forms.Button downloadOrdersButton;
        private System.Windows.Forms.Button viewSettingsButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button downloadSingleOrderButton;
    }
}

