namespace AssetManager
{
    partial class MeshControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.txtScaleX = new System.Windows.Forms.TextBox();
            this.txtScaleY = new System.Windows.Forms.TextBox();
            this.txtScaleZ = new System.Windows.Forms.TextBox();
            this.txtAngle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMaterial = new System.Windows.Forms.ComboBox();
            this.btnOptimize = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.chkRotateX = new System.Windows.Forms.CheckBox();
            this.chkRotateY = new System.Windows.Forms.CheckBox();
            this.chkRotateZ = new System.Windows.Forms.CheckBox();
            this.rdoGameObject = new System.Windows.Forms.RadioButton();
            this.rdoCharacter = new System.Windows.Forms.RadioButton();
            this.rdoBuilding = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtZm = new System.Windows.Forms.TextBox();
            this.txtXm = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtYm = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Scale:";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(74, 8);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(154, 20);
            this.txtFileName.TabIndex = 1;
            // 
            // txtScaleX
            // 
            this.txtScaleX.Location = new System.Drawing.Point(74, 34);
            this.txtScaleX.Name = "txtScaleX";
            this.txtScaleX.Size = new System.Drawing.Size(48, 20);
            this.txtScaleX.TabIndex = 100;
            this.txtScaleX.Leave += new System.EventHandler(this.txtScaleX_Leave);
            // 
            // txtScaleY
            // 
            this.txtScaleY.Location = new System.Drawing.Point(126, 34);
            this.txtScaleY.Name = "txtScaleY";
            this.txtScaleY.Size = new System.Drawing.Size(48, 20);
            this.txtScaleY.TabIndex = 101;
            // 
            // txtScaleZ
            // 
            this.txtScaleZ.Location = new System.Drawing.Point(180, 34);
            this.txtScaleZ.Name = "txtScaleZ";
            this.txtScaleZ.Size = new System.Drawing.Size(48, 20);
            this.txtScaleZ.TabIndex = 103;
            // 
            // txtAngle
            // 
            this.txtAngle.Location = new System.Drawing.Point(74, 60);
            this.txtAngle.Name = "txtAngle";
            this.txtAngle.Size = new System.Drawing.Size(48, 20);
            this.txtAngle.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Rotate:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Material:";
            // 
            // cmbMaterial
            // 
            this.cmbMaterial.FormattingEnabled = true;
            this.cmbMaterial.Location = new System.Drawing.Point(74, 86);
            this.cmbMaterial.Name = "cmbMaterial";
            this.cmbMaterial.Size = new System.Drawing.Size(154, 21);
            this.cmbMaterial.TabIndex = 11;
            // 
            // btnOptimize
            // 
            this.btnOptimize.Location = new System.Drawing.Point(180, 113);
            this.btnOptimize.Name = "btnOptimize";
            this.btnOptimize.Size = new System.Drawing.Size(75, 23);
            this.btnOptimize.TabIndex = 4;
            this.btnOptimize.Text = "Optimize";
            this.btnOptimize.UseVisualStyleBackColor = true;
            this.btnOptimize.Click += new System.EventHandler(this.btnOptimize_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(261, 113);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 23);
            this.btnUpgrade.TabIndex = 5;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // txtReport
            // 
            this.txtReport.Location = new System.Drawing.Point(3, 196);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReport.Size = new System.Drawing.Size(333, 122);
            this.txtReport.TabIndex = 9;
            // 
            // chkRotateX
            // 
            this.chkRotateX.AutoSize = true;
            this.chkRotateX.Location = new System.Drawing.Point(128, 59);
            this.chkRotateX.Name = "chkRotateX";
            this.chkRotateX.Size = new System.Drawing.Size(33, 17);
            this.chkRotateX.TabIndex = 15;
            this.chkRotateX.Text = "X";
            this.chkRotateX.UseVisualStyleBackColor = true;
            // 
            // chkRotateY
            // 
            this.chkRotateY.AutoSize = true;
            this.chkRotateY.Location = new System.Drawing.Point(167, 59);
            this.chkRotateY.Name = "chkRotateY";
            this.chkRotateY.Size = new System.Drawing.Size(33, 17);
            this.chkRotateY.TabIndex = 16;
            this.chkRotateY.Text = "Y";
            this.chkRotateY.UseVisualStyleBackColor = true;
            // 
            // chkRotateZ
            // 
            this.chkRotateZ.AutoSize = true;
            this.chkRotateZ.Location = new System.Drawing.Point(206, 59);
            this.chkRotateZ.Name = "chkRotateZ";
            this.chkRotateZ.Size = new System.Drawing.Size(33, 17);
            this.chkRotateZ.TabIndex = 17;
            this.chkRotateZ.Text = "Z";
            this.chkRotateZ.UseVisualStyleBackColor = true;
            // 
            // rdoGameObject
            // 
            this.rdoGameObject.AutoSize = true;
            this.rdoGameObject.Location = new System.Drawing.Point(10, 19);
            this.rdoGameObject.Name = "rdoGameObject";
            this.rdoGameObject.Size = new System.Drawing.Size(87, 17);
            this.rdoGameObject.TabIndex = 6;
            this.rdoGameObject.TabStop = true;
            this.rdoGameObject.Text = "Game Object";
            this.rdoGameObject.UseVisualStyleBackColor = true;
            this.rdoGameObject.CheckedChanged += new System.EventHandler(this.rdoGameObject_CheckedChanged);
            // 
            // rdoCharacter
            // 
            this.rdoCharacter.AutoSize = true;
            this.rdoCharacter.Location = new System.Drawing.Point(103, 19);
            this.rdoCharacter.Name = "rdoCharacter";
            this.rdoCharacter.Size = new System.Drawing.Size(71, 17);
            this.rdoCharacter.TabIndex = 7;
            this.rdoCharacter.TabStop = true;
            this.rdoCharacter.Text = "Character";
            this.rdoCharacter.UseVisualStyleBackColor = true;
            this.rdoCharacter.CheckedChanged += new System.EventHandler(this.rdoCharacter_CheckedChanged);
            // 
            // rdoBuilding
            // 
            this.rdoBuilding.AutoSize = true;
            this.rdoBuilding.Location = new System.Drawing.Point(180, 19);
            this.rdoBuilding.Name = "rdoBuilding";
            this.rdoBuilding.Size = new System.Drawing.Size(62, 17);
            this.rdoBuilding.TabIndex = 8;
            this.rdoBuilding.TabStop = true;
            this.rdoBuilding.Text = "Building";
            this.rdoBuilding.UseVisualStyleBackColor = true;
            this.rdoBuilding.CheckedChanged += new System.EventHandler(this.rdoBuilding_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoGameObject);
            this.groupBox1.Controls.Add(this.rdoBuilding);
            this.groupBox1.Controls.Add(this.rdoCharacter);
            this.groupBox1.Location = new System.Drawing.Point(14, 142);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 48);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mesh Type";
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(24, 113);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(75, 23);
            this.btnShow.TabIndex = 3;
            this.btnShow.Text = "Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtZm);
            this.groupBox2.Controls.Add(this.txtXm);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtYm);
            this.groupBox2.Location = new System.Drawing.Point(3, 322);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(333, 44);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Size in Meters";
            // 
            // txtZm
            // 
            this.txtZm.Location = new System.Drawing.Point(254, 13);
            this.txtZm.Name = "txtZm";
            this.txtZm.Size = new System.Drawing.Size(57, 20);
            this.txtZm.TabIndex = 3;
            // 
            // txtXm
            // 
            this.txtXm.Location = new System.Drawing.Point(48, 13);
            this.txtXm.Name = "txtXm";
            this.txtXm.Size = new System.Drawing.Size(57, 20);
            this.txtXm.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "X (m):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Blue;
            this.label5.Location = new System.Drawing.Point(111, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y (m):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(214, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Z (m):";
            // 
            // txtYm
            // 
            this.txtYm.Location = new System.Drawing.Point(151, 13);
            this.txtYm.Name = "txtYm";
            this.txtYm.Size = new System.Drawing.Size(57, 20);
            this.txtYm.TabIndex = 2;
            this.txtYm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtYm_KeyDown);
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkRotateZ);
            this.Controls.Add(this.chkRotateY);
            this.Controls.Add(this.chkRotateX);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnOptimize);
            this.Controls.Add(this.cmbMaterial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAngle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtScaleZ);
            this.Controls.Add(this.txtScaleY);
            this.Controls.Add(this.txtScaleX);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MeshControl";
            this.Size = new System.Drawing.Size(339, 405);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.TextBox txtScaleX;
        private System.Windows.Forms.TextBox txtScaleY;
        private System.Windows.Forms.TextBox txtScaleZ;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMaterial;
        private System.Windows.Forms.Button btnOptimize;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.CheckBox chkRotateX;
        private System.Windows.Forms.CheckBox chkRotateY;
        private System.Windows.Forms.CheckBox chkRotateZ;
        private System.Windows.Forms.RadioButton rdoGameObject;
        private System.Windows.Forms.RadioButton rdoCharacter;
        private System.Windows.Forms.RadioButton rdoBuilding;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtZm;
        private System.Windows.Forms.TextBox txtXm;
        private System.Windows.Forms.TextBox txtYm;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}
