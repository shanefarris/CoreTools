using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetManager.Common
{
    public class DbId
    {
        public int Id { get; set; }

        public DbId(int id)
        {
            Id = id;
        }

        public static implicit operator DbId(int value)
        {
            return new DbId(value);
        }

        public static implicit operator int(DbId id)
        {
            return id.Id;
        }

        public static bool operator ==(DbId one, DbId two)
        {
            try
            {
                return one.Id == two.Id;
            }
            catch
            {
                return true;
            }
        }

        public static bool operator !=(DbId one, DbId two)
        {
            if (one == null && two == null)
                return true;

            return one.Id != two.Id;
        }

        public override bool Equals(object o)
        {
            try
            {
                return (bool)(this == (DbId)o);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
