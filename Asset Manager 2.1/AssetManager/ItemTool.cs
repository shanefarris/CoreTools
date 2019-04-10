using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AssetManager
{
    public partial class ItemTool : Form
    {
        private ItemProfileHelper _itemProfileHelper = null;

        public ItemTool(ItemProfileHelper itemProfileHelper)
        {
            InitializeComponent();
            _itemProfileHelper = itemProfileHelper;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            if (txtAccuracy.Text.Length == 0)
                isValid = false;
            else if (txtOverlay.Text.Length == 0)
                isValid = false;
            else if (txtMesh.Text.Length == 0)
                isValid = false;
            else if (txtRoF.Text.Length == 0)
                isValid = false;
            else if (txtMagName.Text.Length == 0)
                isValid = false;
            else if (txtDisName.Text.Length == 0)
                isValid = false;
            else if (txtName.Text.Length == 0)
                isValid = false;
            else if (txtPower.Text.Length == 0)
                isValid = false;
            else if (txtScaleX.Text.Length == 0)
                isValid = false;
            else if (txtScaleY.Text.Length == 0)
                isValid = false;
            else if (txtScaleZ.Text.Length == 0)
                isValid = false;
            else if (txtRange.Text.Length == 0)
                isValid = false;

            if (isValid)
            {
                if (_itemProfileHelper != null)
                {
                    _itemProfileHelper.AddToolItemProfile(txtName.Text, txtDisName.Text, txtMagName.Text, txtRoF.Text, txtOverlay.Text, txtAccuracy.Text,
                        txtPower.Text, txtScaleX.Text, txtScaleY.Text, txtScaleZ.Text, txtRange.Text, chkAuto.Checked, chkSemi.Checked,
                        chkBurst.Checked, txtMesh.Text);
                }
                return;
            }
            MessageBox.Show("All values are required.");
        }
    }
}
