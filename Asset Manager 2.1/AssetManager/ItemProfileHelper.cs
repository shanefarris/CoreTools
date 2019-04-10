using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssetManager
{
    public class ItemProfileHelper
    {
        private readonly List<ItemToolProfile> _itemProfiles = new List<ItemToolProfile>();
        private readonly List<MagazinProfile> _magazineProfiles = new List<MagazinProfile>();
        private readonly List<ProjectileProfile> _projectilesProfiles = new List<ProjectileProfile>();

        public void AddToolItemProfile(string name, string displayName, string magName, string rof, string overlay, string accuracy, string power,
                                       string scaleX, string scaleY, string scaleZ, string range, bool isAuto, bool isSemi, bool isBurst, string meshName)
        {
            try
            {
                var itemProfile = new ItemToolProfile
                                      {
                                          Name = name,
                                          DisplayName = displayName,
                                          MagName = magName,
                                          RoF = Convert.ToInt32(rof),
                                          Overlay = overlay,
                                          Accuracy = Convert.ToInt32(accuracy),
                                          Power = Convert.ToInt32(power),
                                          ScaleX = Convert.ToSingle(scaleX),
                                          ScaleY = Convert.ToSingle(scaleY),
                                          ScaleZ = Convert.ToSingle(scaleZ),
                                          Range = Convert.ToSingle(range),
                                          Semi = isSemi,
                                          Auto = isAuto,
                                          Burst = isBurst,
                                          MeshName = meshName
                                      };
                _itemProfiles.Add(itemProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AddMagazineProfile(string name, string displayName, string projectileName, string capacity, string meshName)
        {
            try
            {
                var magazinProfile = new MagazinProfile
                {
                    Name = name,
                    DisplayName = displayName,
                    ProjectileName = projectileName,
                    Capacity = Convert.ToInt32(capacity),
                    MeshName = meshName
                };
                _magazineProfiles.Add(magazinProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AddProjectileProfile(string name, string displayName, string damage, bool penetrating, string muzzleVelocity, string meshName, string pitch,
            string roll, string bbbullet, string bbflash)
        {
            try
            {
                var projectileProfile = new ProjectileProfile
                {
                    Name = name,
                    DisplayName = displayName,
                    Damage = Convert.ToInt32(damage),
                    Penetrating = penetrating,
                    MuzzleVelocity = Convert.ToInt32(muzzleVelocity),
                    MeshName = meshName,
                    Pitch = Convert.ToInt32(pitch),
                    Roll = Convert.ToInt32(roll),
                    BBBullet = bbbullet,
                    BBFlash = bbflash
                };
                _projectilesProfiles.Add(projectileProfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string CreateToolProfileOutput()
        {
            var output = string.Empty;
            foreach (var p in _itemProfiles)
            {
                output += "<Weapon ";
                output += "name=\"" + p.Name + "\" ";
                output += "disp_name=\"" + p.DisplayName + "\" ";
                output += "semi=\"" + p.Semi.ToString() + "\" ";
                output += "burst=\"" + p.Burst.ToString() + "\" ";
                output += "auto=\"" + p.Auto.ToString() + "\" ";
                output += "mag=\"" + p.MagName + "\" ";
                output += "rof=\"" + p.RoF.ToString() + "\" ";
                output += "mesh=\"" + p.MeshName + "\" ";
                output += "overlay=\"" + p.Overlay + "\" ";
                output += "accuracy=\"" + p.Accuracy.ToString() + "\" ";
                output += "power=\"" + p.Power.ToString() + "\" ";
                output += "scale=\"" + p.ScaleX.ToString() + " " + p.ScaleY.ToString() + " " + p.ScaleZ.ToString() +
                          "\" ";
                output += "range=\"" + p.Range.ToString() + "\" ";
                output += "/Weapon>\r\n";
            }

            foreach (var p in _magazineProfiles)
            {
                output += "<Magazine ";
                output += "name=\"" + p.Name + "\" ";
                output += "disp_name=\"" + p.DisplayName + "\" ";
                output += "projectiles=\"" + p.ProjectileName + "\" ";
                output += "capacity=\"" + p.Capacity.ToString() + "\" ";
                output += "mesh=\"" + p.MeshName + "\" ";
                output += "/Magazine>\r\n";
            }

            foreach (var p in _projectilesProfiles)
            {
                output += "<Projectile ";
                output += "name=\"" + p.Name + "\" ";
                output += "disp_name=\"" + p.DisplayName + "\" ";
                output += "damage=\"" + p.Damage.ToString() + "\" ";
                output += "penetrating=\"" + p.Penetrating.ToString() + "\" ";
                output += "muzzle_velocity=\"" + p.MuzzleVelocity.ToString() + "\" ";
                output += "mesh=\"" + p.MeshName + "\" ";
                output += "pitch=\"" + p.Pitch.ToString() + "\" ";
                output += "roll=\"" + p.Roll.ToString() + "\" ";
                output += "bbbullet=\"" + p.BBBullet + "\" ";
                output += "bbflash=\"" + p.BBFlash + "\" ";
                output += "/Projectile>\r\n";
            }
            return output;
        }

        #region Nested type: ItemToolProfile

        private class ItemToolProfile
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string MagName { get; set; }
            public int RoF { get; set; }
            public string MeshName { get; set; }
            public string Overlay { get; set; }
            public int Accuracy { get; set; }
            public int Power { get; set; }
            public float ScaleX { get; set; }
            public float ScaleY { get; set; }
            public float ScaleZ { get; set; }
            public float Range { get; set; }
            public bool Semi { get; set; }
            public bool Auto { get; set; }
            public bool Burst { get; set; }
        }

        private class MagazinProfile
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string ProjectileName { get; set; }
            public int Capacity { get; set; }
            public string MeshName { get; set; }
        }

        private class ProjectileProfile
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public int Damage { get; set; }
            public bool Penetrating { get; set; }
            public int MuzzleVelocity { get; set; }
            public string MeshName { get; set; }
            public int Pitch { get; set; }
            public int Roll { get; set; }
            public string BBBullet { get; set; }
            public string BBFlash { get; set; }
        }

        #endregion
    }
}