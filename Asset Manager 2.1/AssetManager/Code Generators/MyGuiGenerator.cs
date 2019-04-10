using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AssetManager.Code_Generators
{
    class MyGuiGenerator : GuiGeneratorBase
    {
        public override void Parse(XmlReader reader, string className)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Widget")
                {
                    // Get the widget type
                    string type = reader.GetAttribute("type");
                    if (string.IsNullOrEmpty(type))
                    {
                        MessageBox.Show(@"Unable to find widget type.");
                    }

                    // Get the widget name
                    string name = reader.GetAttribute("name");
                    if (string.IsNullOrEmpty(name))
                    {
                        if (type != "StaticText" && type != "Sheet" && type != "Tab")
                        {
                            MessageBox.Show(@"Unable to find widget name for type: " + type);
                        }
                    }
                    else if (name.Length > 0)
                    {
                        // Section off the code based on the window
                        if (type == "Window")
                        {
                            assigns.Add("// ==================== " + name + " ================\r\n");
                            declares.Add("// ==================== " + name + " ================\r\n");
                            event_declare.Add("// ==================== " + name + " ================\r\n");
                            event_assigns.Add("// ==================== " + name + " ================\r\n");
                            event_defines.Add("// ==================== " + name + " ================\r\n");
                        }

                        assigns.Add("\tStrategy->AssignWidget(" + name + ", \"" + name + "\");\r\n");

                        // Create boilerplate code for widget using the type and name
                        if (type == "Widget" || type == "Window")
                        {
                            declares.Add("\r\n");
                            declares.Add("\tMyGUI::Widget*\t" + name + ";\r\n");
                        }

                        else if (type == "Edit")
                        {
                            declares.Add("\tMyGUI::Edit*\t" + name + ";\r\n");
                        }

                        else if (type == "RenderBox")
                        {
                            declares.Add("\tMyGUI::RenderBoxPtr\t" + name + ";\r\n");
                        }

                        else if (type == "ComboBox")
                        {
                            declares.Add("\tMyGUI::ComboBoxPtr\t" + name + ";\r\n");
                            event_assigns.Add("\t" + name + "->eventComboChangePosition = MyGUI::newDelegate(this, &" + className + "::" + name + "_ComboAccept);\r\n");
                            event_declare.Add("\tvoid " + name + "_ComboAccept(MyGUI::ComboBox* _sender, size_t _index);\r\n");

                            string def = "void " + className + "::" + name + "_ComboAccept(MyGUI::ComboBox* _sender, size_t _index)\r\n{\r\n}\r\n";
                            event_defines.Add(def);
                        }

                        else if (type == "Button" || type == "StaticImage" || type == "ImageBox")
                        {
                            if (type == "Button")
                                declares.Add("\tMyGUI::ButtonPtr\t" + name + ";\r\n");
                            else
                                declares.Add("\tMyGUI::ImageBox*\t" + name + ";\r\n");

                            event_assigns.Add("\t" + name + "->eventMouseButtonClick = MyGUI::newDelegate(this, &" + className + "::" + name + "_Click);\r\n");

                            event_declare.Add("\tvoid " + name + "_Click(MyGUI::WidgetPtr _sender);\r\n");

                            string def = "void " + className + "::" + name + "_Click(MyGUI::WidgetPtr _sender)\r\n{\r\n}\r\n";
                            event_defines.Add(def);
                        }

                        else if (type == "HScroll" || type == "VScroll" || type == "ScrollBar")
                        {
                            declares.Add("\tMyGUI::ScrollBar*\t" + name + ";\r\n");

                            event_assigns.Add("\t" + name + "->eventScrollChangePosition = MyGUI::newDelegate(this, &" + className + "::" + name + "_Change);\r\n");

                            event_declare.Add("\tvoid " + name + "_Change(MyGUI::ScrollBar* _sender, size_t _position);\r\n");

                            string def = "void " + className + "::" + name + "_Change(MyGUI::ScrollBar* _sender, size_t _position)\r\n{\r\n}\r\n";
                            event_defines.Add(def);
                        }

                        else if (type == "ItemBox")
                        {
                            declares.Add("\tMyGUI::ItemBoxPtr\t" + name + ";\r\n");
                        }

                        else
                        {
                            // Updated control names
                            //StaticImage -> ImageBox
                            //StaticText -> TextBox
                            //List -> ListBox
                            //Tab -> TabControl
                            //Edit -> EditBox
                            //Progress -> ProgressBar
                            //MultiList -> MiltiListBox
                            //HScroll, VScroll -> ScrollBar
                            MessageBox.Show("Unsupported control: " + type);
                            return;
                        }
                    }
                } // if
            } // while
        }
    }
}
