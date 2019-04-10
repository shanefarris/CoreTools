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
    public partial class Projectiles : Form
    {
        private ItemProfileHelper _itemProfileHelper = null;

        public Projectiles(ItemProfileHelper itemProfileHelper)
        {
            InitializeComponent();
            _itemProfileHelper = itemProfileHelper;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            if (txtBBBullet.Text.Length == 0)
                isValid = false;
            else if (txtBBFlash.Text.Length == 0)
                isValid = false;
            else if (txtDisName.Text.Length == 0)
                isValid = false;
            else if (txtName.Text.Length == 0)
                isValid = false;
            else if (txtDamage.Text.Length == 0)
                isValid = false;
            else if (txtMuzzleVelocity.Text.Length == 0)
                isValid = false;
            else if (txtPitch.Text.Length == 0)
                isValid = false;
            else if (txtRoll.Text.Length == 0)
                isValid = false;

            if (isValid)
            {
                if (_itemProfileHelper != null)
                {
                    _itemProfileHelper.AddProjectileProfile(txtName.Text, txtDisName.Text, txtDamage.Text, chkPenetrating.Checked, txtMuzzleVelocity.Text,
                        txtMesh.Text, txtPitch.Text, txtRoll.Text, txtBBBullet.Text, txtBBFlash.Text );
                }
                return;
            }
        }
    }
}
