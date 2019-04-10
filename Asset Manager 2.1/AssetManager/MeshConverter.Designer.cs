namespace AssetManager
{
    partial class MeshConverter
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
            this.btnSingleConvert = new System.Windows.Forms.Button();
            this.btnSingleBrowse = new System.Windows.Forms.Button();
            this.txtSingleFile = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbBatchFileType = new System.Windows.Forms.ComboBox();
            this.btnBatchConvert = new System.Windows.Forms.Button();
            this.txtBatchDir = new System.Windows.Forms.TextBox();
            this.btnBatchDir = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSingleConvert);
            this.groupBox1.Controls.Add(this.btnSingleBrowse);
            this.groupBox1.Controls.Add(this.txtSingleFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 77);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single File";
            // 
            // btnSingleConvert
            // 
            this.btnSingleConvert.Location = new System.Drawing.Point(381, 45);
            this.btnSingleConvert.Name = "btnSingleConvert";
            this.btnSingleConvert.Size = new System.Drawing.Size(59, 22);
            this.btnSingleConvert.TabIndex = 3;
            this.btnSingleConvert.Text = "Convert";
            this.btnSingleConvert.UseVisualStyleBackColor = true;
            this.btnSingleConvert.Click += new System.EventHandler(this.btnSingleConvert_Click);
            // 
            // btnSingleBrowse
            // 
            this.btnSingleBrowse.Location = new System.Drawing.Point(409, 19);
            this.btnSingleBrowse.Name = "btnSingleBrowse";
            this.btnSingleBrowse.Size = new System.Drawing.Size(31, 20);
            this.btnSingleBrowse.TabIndex = 2;
            this.btnSingleBrowse.Text = "...";
            this.btnSingleBrowse.UseVisualStyleBackColor = true;
            this.btnSingleBrowse.Click += new System.EventHandler(this.btnSingleBrowse_Click);
            // 
            // txtSingleFile
            // 
            this.txtSingleFile.Enabled = false;
            this.txtSingleFile.Location = new System.Drawing.Point(6, 19);
            this.txtSingleFile.Name = "txtSingleFile";
            this.txtSingleFile.Size = new System.Drawing.Size(397, 20);
            this.txtSingleFile.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbBatchFileType);
            this.groupBox2.Controls.Add(this.btnBatchConvert);
            this.groupBox2.Controls.Add(this.txtBatchDir);
            this.groupBox2.Controls.Add(this.btnBatchDir);
            this.groupBox2.Location = new System.Drawing.Point(12, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(446, 73);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Batch Files";
            // 
            // cmbBatchFileType
            // 
            this.cmbBatchFileType.FormattingEnabled = true;
            this.cmbBatchFileType.Items.AddRange(new object[] {
            "3D GameStudio 3DGS ( .mdl )",
            "3D GameStudio 3DGS Terrain ( .hmp )",
            "3ds Max 3DS ( .3ds )",
            "3ds Max ASE ( .ase )",
            "AC3D ( .ac )",
            "AutoCAD DXF ( .dxf )",
            "Biovision BVH ( .bvh )",
            "Blender 3D ( .blend )",
            "BlitzBasic 3D ( .b3d )",
            "CharacterStudio Motion ( .csm )",
            "Collada ( .dae )",
            "DirectX X ( .x )",
            "Doom 3 ( .md5 )",
            "Industry Foundation Classes IFC/Step ( .ifc )",
            "Irrlicht Mesh ( .irrmesh )",
            "Irrlicht Scene ( .irr )",
            "Izware Nendo ( .ndo )",
            "LightWave ( .lwo )",
            "LightWave Scene ( .lws )",
            "Milkshape 3D ( .ms3d )",
            "Modo ( .lxo )",
            "Neutral File Format ( .nff )",
            "Object File Format ( .off )",
            "Ogre XML ( .xml )",
            "PovRAY Raw ( .raw )",
            "Quake I ( .mdl )",
            "Quake II ( .md2 )",
            "Quake III Map/BSP ( .pk3 )",
            "Quake III Mesh ( .md3 )",
            "Quick3D ( .q3d )",
            "Quick3D ( .q3s )",
            "Return to Castle Wolfenstein ( .mdc )",
            "Sense8 WorldToolKit ( .nff )",
            "Stanford Polygon Library ( .ply )",
            "Starcraft II M3 ( .m3 )",
            "Stereolithography ( .stl )",
            "Terragen Terrain ( .ter )",
            "TrueSpace ( .cob )",
            "TrueSpace ( .scn )",
            "Unreal ( .3d )",
            "Valve Model ( .smd )",
            "Valve Model ( .vta )",
            "Wavefront Object ( .obj )",
            "XGL ( .xgl )",
            "XGL ( .zgl )"});
            this.cmbBatchFileType.Location = new System.Drawing.Point(223, 45);
            this.cmbBatchFileType.Name = "cmbBatchFileType";
            this.cmbBatchFileType.Size = new System.Drawing.Size(152, 21);
            this.cmbBatchFileType.TabIndex = 2;
            // 
            // btnBatchConvert
            // 
            this.btnBatchConvert.Location = new System.Drawing.Point(381, 45);
            this.btnBatchConvert.Name = "btnBatchConvert";
            this.btnBatchConvert.Size = new System.Drawing.Size(59, 22);
            this.btnBatchConvert.TabIndex = 6;
            this.btnBatchConvert.Text = "Convert";
            this.btnBatchConvert.UseVisualStyleBackColor = true;
            this.btnBatchConvert.Click += new System.EventHandler(this.btnBatchConvert_Click);
            // 
            // txtBatchDir
            // 
            this.txtBatchDir.Enabled = false;
            this.txtBatchDir.Location = new System.Drawing.Point(6, 19);
            this.txtBatchDir.Name = "txtBatchDir";
            this.txtBatchDir.Size = new System.Drawing.Size(397, 20);
            this.txtBatchDir.TabIndex = 4;
            // 
            // btnBatchDir
            // 
            this.btnBatchDir.Location = new System.Drawing.Point(409, 19);
            this.btnBatchDir.Name = "btnBatchDir";
            this.btnBatchDir.Size = new System.Drawing.Size(31, 20);
            this.btnBatchDir.TabIndex = 5;
            this.btnBatchDir.Text = "...";
            this.btnBatchDir.UseVisualStyleBackColor = true;
            this.btnBatchDir.Click += new System.EventHandler(this.btnBatchDir_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(383, 385);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // txtReport
            // 
            this.txtReport.Location = new System.Drawing.Point(12, 174);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReport.Size = new System.Drawing.Size(446, 205);
            this.txtReport.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 385);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Status";
            // 
            // MeshConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 420);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "MeshConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mesh Converter";
            this.Load += new System.EventHandler(this.MeshConverter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSingleConvert;
        private System.Windows.Forms.Button btnSingleBrowse;
        private System.Windows.Forms.TextBox txtSingleFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbBatchFileType;
        private System.Windows.Forms.Button btnBatchConvert;
        private System.Windows.Forms.TextBox txtBatchDir;
        private System.Windows.Forms.Button btnBatchDir;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Label lblStatus;
    }
}