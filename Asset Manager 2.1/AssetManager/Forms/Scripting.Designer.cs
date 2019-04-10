namespace AssetManager
{
    partial class Scripting
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.txtReport = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstScriptingIgnore = new System.Windows.Forms.ListBox();
            this.clbScriptingIncludes = new System.Windows.Forms.CheckedListBox();
            this.txtRoot = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDir = new System.Windows.Forms.Button();
            this.cmdLanguage = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(639, 481);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 39;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtReport
            // 
            this.txtReport.Location = new System.Drawing.Point(12, 481);
            this.txtReport.Multiline = true;
            this.txtReport.Name = "txtReport";
            this.txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReport.Size = new System.Drawing.Size(621, 159);
            this.txtReport.TabIndex = 38;
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(639, 264);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 37;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(639, 235);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 36;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // lstScriptingIgnore
            // 
            this.lstScriptingIgnore.FormattingEnabled = true;
            this.lstScriptingIgnore.Location = new System.Drawing.Point(12, 237);
            this.lstScriptingIgnore.Name = "lstScriptingIgnore";
            this.lstScriptingIgnore.Size = new System.Drawing.Size(621, 238);
            this.lstScriptingIgnore.TabIndex = 35;
            // 
            // clbScriptingIncludes
            // 
            this.clbScriptingIncludes.FormattingEnabled = true;
            this.clbScriptingIncludes.Location = new System.Drawing.Point(12, 32);
            this.clbScriptingIncludes.Name = "clbScriptingIncludes";
            this.clbScriptingIncludes.Size = new System.Drawing.Size(702, 199);
            this.clbScriptingIncludes.TabIndex = 34;
            // 
            // txtRoot
            // 
            this.txtRoot.Location = new System.Drawing.Point(94, 6);
            this.txtRoot.Name = "txtRoot";
            this.txtRoot.Size = new System.Drawing.Size(375, 20);
            this.txtRoot.TabIndex = 32;
            this.txtRoot.Text = "D:\\Code\\Core-Engine\\Trunk\\Include";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Root Includes";
            // 
            // btnDir
            // 
            this.btnDir.Location = new System.Drawing.Point(475, 4);
            this.btnDir.Name = "btnDir";
            this.btnDir.Size = new System.Drawing.Size(103, 23);
            this.btnDir.TabIndex = 33;
            this.btnDir.Text = "Directory Listing";
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // cmdLanguage
            // 
            this.cmdLanguage.FormattingEnabled = true;
            this.cmdLanguage.Items.AddRange(new object[] {
            "C#",
            "Lua",
            "Lua++"});
            this.cmdLanguage.Location = new System.Drawing.Point(585, 4);
            this.cmdLanguage.Name = "cmdLanguage";
            this.cmdLanguage.Size = new System.Drawing.Size(129, 21);
            this.cmdLanguage.TabIndex = 40;
            // 
            // Scripting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 652);
            this.Controls.Add(this.cmdLanguage);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lstScriptingIgnore);
            this.Controls.Add(this.clbScriptingIncludes);
            this.Controls.Add(this.txtRoot);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Scripting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scripting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtReport;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstScriptingIgnore;
        private System.Windows.Forms.CheckedListBox clbScriptingIncludes;
        private System.Windows.Forms.TextBox txtRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.ComboBox cmdLanguage;

    }
}