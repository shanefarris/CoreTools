namespace AssetManager
{
    partial class ExportFormat
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoBMP = new System.Windows.Forms.RadioButton();
            this.rdoPNG = new System.Windows.Forms.RadioButton();
            this.rdoJPG = new System.Windows.Forms.RadioButton();
            this.rdoNone = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.rdoDDS = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDDS);
            this.groupBox1.Controls.Add(this.rdoBMP);
            this.groupBox1.Controls.Add(this.rdoPNG);
            this.groupBox1.Controls.Add(this.rdoJPG);
            this.groupBox1.Controls.Add(this.rdoNone);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(115, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image Formatting";
            // 
            // rdoBMP
            // 
            this.rdoBMP.AutoSize = true;
            this.rdoBMP.Location = new System.Drawing.Point(6, 88);
            this.rdoBMP.Name = "rdoBMP";
            this.rdoBMP.Size = new System.Drawing.Size(48, 17);
            this.rdoBMP.TabIndex = 4;
            this.rdoBMP.TabStop = true;
            this.rdoBMP.Text = "BMP";
            this.rdoBMP.UseVisualStyleBackColor = true;
            // 
            // rdoPNG
            // 
            this.rdoPNG.AutoSize = true;
            this.rdoPNG.Location = new System.Drawing.Point(6, 65);
            this.rdoPNG.Name = "rdoPNG";
            this.rdoPNG.Size = new System.Drawing.Size(48, 17);
            this.rdoPNG.TabIndex = 3;
            this.rdoPNG.TabStop = true;
            this.rdoPNG.Text = "PNG";
            this.rdoPNG.UseVisualStyleBackColor = true;
            // 
            // rdoJPG
            // 
            this.rdoJPG.AutoSize = true;
            this.rdoJPG.Location = new System.Drawing.Point(6, 42);
            this.rdoJPG.Name = "rdoJPG";
            this.rdoJPG.Size = new System.Drawing.Size(45, 17);
            this.rdoJPG.TabIndex = 2;
            this.rdoJPG.TabStop = true;
            this.rdoJPG.Text = "JPG";
            this.rdoJPG.UseVisualStyleBackColor = true;
            // 
            // rdoNone
            // 
            this.rdoNone.AutoSize = true;
            this.rdoNone.Location = new System.Drawing.Point(6, 19);
            this.rdoNone.Name = "rdoNone";
            this.rdoNone.Size = new System.Drawing.Size(51, 17);
            this.rdoNone.TabIndex = 1;
            this.rdoNone.TabStop = true;
            this.rdoNone.Text = "None";
            this.rdoNone.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(134, 179);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // rdoDDS
            // 
            this.rdoDDS.AutoSize = true;
            this.rdoDDS.Location = new System.Drawing.Point(6, 111);
            this.rdoDDS.Name = "rdoDDS";
            this.rdoDDS.Size = new System.Drawing.Size(48, 17);
            this.rdoDDS.TabIndex = 5;
            this.rdoDDS.TabStop = true;
            this.rdoDDS.Text = "DDS";
            this.rdoDDS.UseVisualStyleBackColor = true;
            // 
            // ExportFormat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 214);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Name = "ExportFormat";
            this.Text = "Export Format";
            this.Load += new System.EventHandler(this.ExportFormat_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoBMP;
        private System.Windows.Forms.RadioButton rdoPNG;
        private System.Windows.Forms.RadioButton rdoJPG;
        private System.Windows.Forms.RadioButton rdoNone;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RadioButton rdoDDS;
    }
}