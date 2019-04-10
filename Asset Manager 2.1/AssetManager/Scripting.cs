using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager
{
    public partial class Scripting : Form
    {
        private readonly ScriptingHelper _scriptHelper = new ScriptingHelper();

        public Scripting()
        {
            InitializeComponent();

            cmdLanguage.SelectedIndex = 0;

            var excludes = Properties.Settings.Default.ScriptingExcludes;
            if (excludes != null)
            {
                foreach (String value in excludes)
                {
                    // ToLua++
                    lstScriptingIgnore.Items.Add(value);
                }
            }
        }

        private void btnDir_Click(object sender, EventArgs e)
        {
            _scriptHelper.PopulateListControl(clbScriptingIncludes, txtRoot.Text);
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            ScriptingHelper.Language language = ScriptingHelper.Language.CS;
            if (cmdLanguage.SelectedItem.ToString() == "Lua")
                language = ScriptingHelper.Language.Lua;
            else if (cmdLanguage.SelectedItem.ToString() == "Lua++")
                language = ScriptingHelper.Language.LuaPP;

            _scriptHelper.GeneratePackages(language, clbScriptingIncludes, txtRoot.Text + "\\pkg", lstScriptingIgnore, txtReport);
        }
    }
}
