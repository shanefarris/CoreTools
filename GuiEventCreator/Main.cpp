// Converts Xml layouts from supported gui libraires to c++ events ready to code

#include <iostream>
#include <stdio.h>
#include <string>
#include <vector>
#include <windows.h>

#include "CFileSystem.h"

using namespace IO;

std::string ClassName = "CLASS_NAME";

std::vector<std::string> declares;
std::vector<std::string> assigns;
std::vector<std::string> event_declare;
std::vector<std::string> event_assigns;
std::vector<std::string> event_defines;

void printHelp(void)
{
    // Print help message
    std::cout << std::endl << "GuiEventCreator " << std::endl;
    std::cout << "Usage: GuiEventCreator [options] FileName " << std::endl;
	std::cout << std::endl;
}

std::string Create(const std::string& filename)
{
	// Only support mygui for now

	// See if the file exists
	if(!CFileSystem::IsFileExist(filename.c_str()))
		return "File does not exist";


	CFileSystem FileSystem;
	IXMLReader* xml = FileSystem.CreateXMLReader(filename.c_str());
	if(!xml)
		return "Could not create the XML driver";

	while(xml && xml->Read())
    {
		if (String("Widget") == xml->GetNodeName() && xml->GetNodeType() == XML_NODE_ELEMENT)
		{
			// Code for a new widget
			std::string type = xml->GetAttributeValueSafe("type");
			if(type.length() == 0)
			{
				return "Unable to parse widget type";
			}

			std::string name = xml->GetAttributeValueSafe("name");
			if(name.length() == 0)
			{
				if(type != "StaticText" && type != "Sheet" && type != "Tab")
				{
					return "Unable to parse widget name for type: " + type;
				}
			}
			else if(name.length() > 0)
			{
				assigns.push_back(std::string("\tStrategy->GetBaseLayout()->AssignWidget(" + name + ", \"" + name + "\");\n"));

				// Create boilerplate code for widget using the type and name
				if(type == std::string("Widget") ||
					type == std::string("Window"))
				{
					declares.push_back("\n");
					declares.push_back(std::string("\tMyGUI::Widget*\t\t" + name + ";\n"));
				}
				else if(type == std::string("Edit"))
				{
					declares.push_back(std::string("\tMyGUI::EditPtr\t\t" + name + ";\n"));
				}
				else if(type == std::string("RenderBox"))
				{
					declares.push_back(std::string("\tMyGUI::RenderBoxPtr\t" + name + ";\n"));
				}
				else if(type == std::string("ComboBox"))
				{
					declares.push_back(std::string("\tMyGUI::ComboBoxPtr\t" + name + ";\n"));
					event_assigns.push_back(std::string("\t" + name + "->eventComboChangePosition = MyGUI::newDelegate(this, &" + ClassName + "::" + name + "_ComboAccept);\n"));

					event_declare.push_back("\tvoid " + name + "_ComboAccept(MyGUI::ComboBox* _sender, size_t _index);\n");

					std::string def = "void " + ClassName + "::" + name + "_ComboAccept(MyGUI::ComboBox* _sender, size_t _index)\n{\n}\n";
					event_defines.push_back(def);
				}
				else if(type == std::string("Button") || type == std::string("StaticImage"))
				{
					if(type == "Button")
						declares.push_back(std::string("\tMyGUI::ButtonPtr\t" + name + ";\n"));
					else
						declares.push_back(std::string("\tMyGUI::StaticImage*\t" + name + ";\n"));

					event_assigns.push_back(std::string("\t" + name + "->eventMouseButtonClick = MyGUI::newDelegate(this, &" + ClassName + "::" + name + "_Click);\n"));

					event_declare.push_back("\tvoid " + name + "_Click(MyGUI::WidgetPtr _sender);\n");

					std::string def = "void " + ClassName + "::" + name + "_Click(MyGUI::WidgetPtr _sender)\n{\n}\n";
					event_defines.push_back(def);
				}
				else if(type == std::string("HScroll") || type == std::string("VScroll"))
				{
					if(type == "HScroll")
						declares.push_back(std::string("\tMyGUI::HScrollPtr\t" + name + ";\n"));
					else
						declares.push_back(std::string("\tMyGUI::VScrollPtr\t" + name + ";\n"));

					event_assigns.push_back(std::string("\t" + name + "->eventScrollChangePosition = MyGUI::newDelegate(this, &" + ClassName + "::" + name + "_Change);\n"));

					event_declare.push_back("\tvoid " + name + "_Change(MyGUI::VScroll* _sender, size_t _position);\n");

					std::string def = "void " + ClassName + "::" + name + "_Change(MyGUI::VScroll* _sender, size_t _position)\n{\n}\n";
					event_defines.push_back(def);
				}
				else if(type == std::string("ItemBox"))
				{
					declares.push_back(std::string("\tMyGUI::ItemBoxPtr\t" + name + ";\n"));
				}
			}
		}
    }

	// Create the output
	std::string output;
	for(uint i = 0; i < declares.size(); i++)
	{
		output += declares[i];
	}
	output += "\n\n\n\n";
	for(uint i = 0; i < event_declare.size(); i++)
	{
		output += event_declare[i];
	}
	output += "\n\n\n\n";
	for(uint i = 0; i < assigns.size(); i++)
	{
		output += assigns[i];
	}
	output += "\n\n\n\n";
	for(uint i = 0; i < event_assigns.size(); i++)
	{
		output += event_assigns[i];
	}
	output += "\n\n\n\n";
	for(uint i = 0; i < event_defines.size(); i++)
	{
		output += event_defines[i];
		output += "\n";
	}

	IWriteFile* writer = FileSystem.CreateAndWriteFile("./out.cpp", false, false);
	if(writer)
	{
		writer->Write(output.c_str(), output.length());
		delete writer;
	}
	else
	{
		return "Unable to create ./out.cpp";
	}

	if (xml)
		delete xml;

	return "";
}

int main(int argc, const char** argv)
{
    if (argc < 2)
    {
        printHelp();
        return -1;
    }

	try
	{
		std::string filename = argv[1];
		std::string ret;

		if(filename.length() > 0)
		{
			ret = Create(filename);
		}
		else
		{
			printHelp();
			return -1;
		}
		
		if(ret.length() > 0)
			std::cout << ret << std::endl;
	}
    catch (std::exception& ex)
    {
		std::cout << "Parsing global options failed:" << std::endl;
        std::cout << ex.what() << std::endl;
    }
    catch (...)
    {
		std::cout << "Parsing global options failed." << std::endl;
    }
    
    char ch;
	std::cout << "Press a key then press enter: ";
	std::cin  >> ch;

    return 0;
}
