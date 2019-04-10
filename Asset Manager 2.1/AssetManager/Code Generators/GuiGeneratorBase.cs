using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;

namespace AssetManager.Code_Generators
{
    abstract class GuiGeneratorBase
    {
        protected List<string> assigns = new List<string>();
        protected List<string> declares = new List<string>();
        protected List<string> event_declare = new List<string>();
        protected List<string> event_assigns = new List<string>();
        protected List<string> event_defines = new List<string>();

        public List<string> VariableAssignments
        {
            get
            {
                return assigns;
            }
        }

        public List<string> VariableDeclarations
        {
            get
            {
                return declares;
            }
        }

        public List<string> EventDeclarations
        {
            get
            {
                return event_declare;
            }
        }

        public List<string> EventAssignments
        {
            get
            {
                return event_assigns;
            }
        }

        public List<string> EventDefinitions
        {
            get
            {
                return event_defines;
            }
        }

        public virtual void Parse(XmlReader reader, string className)
        {
            
        }
    }
}
