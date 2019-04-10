using System;
using System.Windows.Forms;

namespace AssetManager
{
    public partial class Magazine : Form
    {
        private ItemProfileHelper _itemProfileHelper = null;

        public Magazine(ItemProfileHelper itemProfileHelper)
        {
            InitializeComponent();
            _itemProfileHelper = itemProfileHelper;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            if (txtCapacity.Text.Length == 0)
                isValid = false;
            else if (txtMesh.Text.Length == 0)
                isValid = false;
            else if (txtDisName.Text.Length == 0)
                isValid = false;
            else if (txtName.Text.Length == 0)
                isValid = false;
            else if (txtProjectile.Text.Length == 0)
                isValid = false;

            if (isValid)
            {
                if (_itemProfileHelper != null)
                {
                    _itemProfileHelper.AddMagazineProfile(txtName.Text, txtDisName.Text, txtProjectile.Text, txtCapacity.Text, txtMesh.Text);
                }
                return;
            }
        }
    }
}
