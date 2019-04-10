namespace AssetManager
{
    partial class Magazine
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDisName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProjectile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCapacity = new System.Windows.Forms.TextBox();
            this.txtMesh = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(97, 9);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(175, 20);
            this.txtName.TabIndex = 30;
            // 
            // txtDisName
            // 
            this.txtDisName.Location = new System.Drawing.Point(97, 35);
            this.txtDisName.Name = "txtDisName";
            this.txtDisName.Size = new System.Drawing.Size(175, 20);
            this.txtDisName.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Display Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Name";
            // 
            // txtProjectile
            // 
            this.txtProjectile.Location = new System.Drawing.Point(97, 61);
            this.txtProjectile.Name = "txtProjectile";
            this.txtProjectile.Size = new System.Drawing.Size(175, 20);
            this.txtProjectile.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Projectile";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Capacity";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 37;
            this.label5.Text = "Mesh";
            // 
            // txtCapacity
            // 
            this.txtCapacity.Location = new System.Drawing.Point(97, 87);
            this.txtCapacity.Name = "txtCapacity";
            this.txtCapacity.Size = new System.Drawing.Size(175, 20);
            this.txtCapacity.TabIndex = 38;
            // 
            // txtMesh
            // 
            this.txtMesh.Location = new System.Drawing.Point(97, 113);
            this.txtMesh.Name = "txtMesh";
            this.txtMesh.Size = new System.Drawing.Size(175, 20);
            this.txtMesh.TabIndex = 39;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(197, 139);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Magazine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 174);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtMesh);
            this.Controls.Add(this.txtCapacity);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtProjectile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtDisName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Magazine";
            this.Text = "Magazine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDisName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProjectile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCapacity;
        private System.Windows.Forms.TextBox txtMesh;
        private System.Windows.Forms.Button btnSave;
    }
}