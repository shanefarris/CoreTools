using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace AssetManager.Code_Generators
{
    public class SceneFile
    {
        private int _nodeDepth = 0;

        public string ConvertFile(string fileName)
        {
            string results = string.Empty;

            using (var reader = new XmlTextReader(fileName))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "node")
                    {
                        results += CreateNode(reader, reader.GetAttribute("name"));
                    }
                }
            }

            return results;
        }

        private string CreateNode(XmlTextReader reader, string name, string rootName = null)
        {
            _nodeDepth++;

            var result = string.Empty;
            var materials = new List<string>();
            var node = name;
            var entity = string.Empty;
            var children = string.Empty;
            var meshFile = string.Empty;
            var posX = string.Empty;
            var posY = string.Empty;
            var posZ = string.Empty;
            var scaleX = string.Empty;
            var scaleY = string.Empty;
            var scaleZ = string.Empty;
            var rotX = string.Empty;
            var rotY = string.Empty;
            var rotZ = string.Empty;
            var rotW = string.Empty;

            // Parse the XML
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "position")
                {
                    posX = reader.GetAttribute("x");
                    posY = reader.GetAttribute("y");
                    posZ = reader.GetAttribute("z");
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "scale")
                {
                    scaleX = reader.GetAttribute("x");
                    scaleY = reader.GetAttribute("y");
                    scaleZ = reader.GetAttribute("z");
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "rotation")
                {
                    rotX = reader.GetAttribute("qx");
                    rotY = reader.GetAttribute("qy");
                    rotZ = reader.GetAttribute("qz");
                    rotW = reader.GetAttribute("qw");
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "entity")
                {
                    entity = reader.GetAttribute("name");
                    meshFile = reader.GetAttribute("meshFile");
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "subentities")
                {
                    materials.Add(reader.GetAttribute("materialName"));
                }
                else if (reader.Name == "node")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        children += CreateNode(reader, reader.GetAttribute("name"), rootName != null ? rootName : node);
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        _nodeDepth--;
                        break;
                    }
                }
            }

            // Construct the data into code
            var nodeName = node + "Node";
            var entityName = entity + "Entity";
            if (rootName == null)
            {
                result += "auto " + nodeName + " = CGameManager::Instance()->GetSceneManager()->getRootSceneNode()->createChildSceneNode(\"" + node + "_node\");\r\n";
                result += "auto " + entityName + " = CGameManager::Instance()->GetSceneManager()->createEntity(\"" + entity + "_entity\", \"" + meshFile + "\");\r\n";
            }
            else
            {
                result += "auto " + nodeName + " = " + rootName + "Node->createChildSceneNode(\"" + node + "_node\");\r\n";
                result += "auto " + entityName + " = CGameManager::Instance()->GetSceneManager()->createEntity(\"" + entity + "_entity\", \"" + meshFile + "\");\r\n";
            }

            result += nodeName + "->attachObject(" + entityName + ");\r\n";
            result += nodeName + "->setPosition(" + posX + "f, " + posY + "f, " + posZ + "f);\r\n";
            result += nodeName + "->setScale(" + scaleX + "f, " + scaleY + "f, " + scaleZ + "f);\r\n";

            if (rotX.Length > 0 && rotY.Length > 0 && rotZ.Length > 0 && rotW.Length > 0)
                result += nodeName + "->rotate(Quaternion(" + rotX + "f, " + rotY + "f, " + rotZ + "f, " + rotW + "f));\r\n";

            result += "\r\n";

            return result + children;
        }
    }
}
