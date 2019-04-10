#if COMPILE_DOTSCENE
#include "CDotSceneConverter.h"
#include "CFileSystem.h"

using namespace IO;
using namespace Core;
using namespace Core::SceneLoader;

CDotSceneConverter::CDotSceneConverter()
{
}

CDotSceneConverter::~CDotSceneConverter()
{
}

bool CDotSceneConverter::Convert(std::string FileName)
{
	CFileSystem FileSystem;
	IXMLReader* xml;

	m_FileName = FileName;
	xml = FileSystem.CreateXMLReader(FileName.c_str());
	if(!xml)
		return false;

	while(xml && xml->Read())
    {
		if (String("environment") == xml->GetNodeName())
		{
			ParseEnvironment(xml);
		}
		else if (String("node") == xml->GetNodeName())
		{
			ParseNode(xml);
		}
    }

    if (xml)
		delete xml;

	// Save a new file
	SaveData();

	return true;
}

void CDotSceneConverter::ParseEnvironment(IXMLReader* xml)
{
	while(xml && !(String("environment") == xml->GetNodeName() && xml->GetNodeType() == XML_NODE_ELEMENT_END))
	{
		std::string name = xml->GetNodeName();
		if ("colourAmbient" == name)
		{
			m_SceneManager.Ambient.r = xml->GetAttributeValueAsFloat("r");
			m_SceneManager.Ambient.g = xml->GetAttributeValueAsFloat("g");
			m_SceneManager.Ambient.b = xml->GetAttributeValueAsFloat("b");
			m_SceneManager.Ambient.a = xml->GetAttributeValueAsFloat("a");
		}
		else if ("colourBackground" == name)
		{
			m_Viewport.Color.b = xml->GetAttributeValueAsFloat("r");
			m_Viewport.Color.g = xml->GetAttributeValueAsFloat("g");
			m_Viewport.Color.b = xml->GetAttributeValueAsFloat("b");
		}
		else if ("clipping" == name)
		{
			// TODO
		}
		else if ("scenemanager" == name)
		{
			m_SceneManager.Type = xml->GetAttributeValueSafe("type");
			m_SceneManager.Name = xml->GetAttributeValueSafe("name");
		}
		else if ("skybox" == name)
		{
			m_SceneManager.SkyBoxActive = StringConverter::parseBool(xml->GetAttributeValueSafe("active"));
			m_SceneManager.SkyBoxDistance = xml->GetAttributeValueAsFloat("distance");
			m_SceneManager.SkyBoxMaterial = xml->GetAttributeValueSafe("material");
		}
		else if ("skyDome" == name)
		{
			// TODO
		}
		else if ("fog" == name)
		{
			int mode = StringConverter::parseInt(xml->GetAttributeValueSafe("mode"));
			if(mode == 0)
				m_SceneManager.FogMode = FOG_NONE;
			else if(mode == 1)
				m_SceneManager.FogMode = FOG_EXP;
			else if(mode == 2)
				m_SceneManager.FogMode = FOG_EXP2;
			else if(mode == 3)
				m_SceneManager.FogMode = FOG_LINEAR;

			m_SceneManager.FogStart = StringConverter::parseInt(xml->GetAttributeValueSafe("start"));
			m_SceneManager.FogEnd = StringConverter::parseInt(xml->GetAttributeValueSafe("end"));
			m_SceneManager.FogDensity = StringConverter::parseReal(xml->GetAttributeValueSafe("density"));
			// TODO: fog color (child tag)
		}

		xml->Read();
	}
}

void CDotSceneConverter::ParseNode(IXMLReader* xml)
{
	NODE node;
	ENTITY entity;
	while(xml && !(String("node") == xml->GetNodeName() && xml->GetNodeType() == XML_NODE_ELEMENT_END))
	{
		std::string name = xml->GetNodeName();
		if ("node" == name)
		{
			node.Name = xml->GetAttributeValue("name");
		}
		else if ("position" == name)
		{
			node.Position.x = xml->GetAttributeValueAsFloat("x");
			node.Position.y = xml->GetAttributeValueAsFloat("y");
			node.Position.z = xml->GetAttributeValueAsFloat("z");
		}
		else if ("scale" == name)
		{
			node.Scale.x = xml->GetAttributeValueAsFloat("x");
			node.Scale.y = xml->GetAttributeValueAsFloat("y");
			node.Scale.z = xml->GetAttributeValueAsFloat("z");
		}
		else if ("rotation" == name)
		{
			node.Orientation.x = xml->GetAttributeValueAsFloat("qx");
			node.Orientation.y = xml->GetAttributeValueAsFloat("qy");
			node.Orientation.z = xml->GetAttributeValueAsFloat("qz");
			node.Orientation.w = xml->GetAttributeValueAsFloat("qw");
		}
		else if ("entity" == name)
		{
			entity.Name = xml->GetAttributeValue("name");
			entity.Shadows = xml->GetAttributeValue("castShadows") == "true" ? true : false;
			entity.MeshFile = xml->GetAttributeValue("meshFile");
		}
		else if ("subentities" == name)
		{
			ParseSubEntities(xml, entity);
		}

		// Add attach to
		node.AttachTo = entity.Name;
		
		name = xml->Read();
		name = xml->GetNodeName();
	}
	m_Nodes.push_back(node);
	m_Entities.push_back(entity);
}

void CDotSceneConverter::ParseSubEntities(IXMLReader* xml, ENTITY &Entity)
{
	while(xml && !(String("entity") == xml->GetNodeName() && xml->GetNodeType() == XML_NODE_ELEMENT_END))
	{
		std::string name = xml->GetNodeName();
		if ("subentity" == name)
		{
			SUBENTITY sub;
			sub.Index = xml->GetAttributeValueAsInt("index");
			sub.Material = xml->GetAttributeValue("materialName");
			sub.Visible = xml->GetAttributeValue("visible") == "false" ? false : true;
			sub.Parent = Entity.Name;
			Entity.SubEntities.push_back(sub);
		}
		name = xml->Read();
	}
}

bool CDotSceneConverter::SaveData()
{
	CFileSystem FileSystem;

	// remove the extension and replace it with custom extension
	std::string ext = FileSystem.GetFileExtension(m_FileName);
	m_FileName = m_FileName.replace(m_FileName.size() - ext.size(),
									ext.size(),
									CUSTOM_EXTENSION);

	CSceneWriterXml* writer = new CSceneWriterXml();
	if(!writer->CreateSceneFile(m_FileName))
	{
		CORE_THROW("Could not create scene file", "CDotSceneConverter::SaveData");
	}


	std::vector<SCENE_ELEMENT> elements;

	// add scene manager
	elements.push_back(SCENE_ELEMENT("Ambient", StringConverter::toString(m_SceneManager.Ambient)));
	elements.push_back(SCENE_ELEMENT("FogColor", StringConverter::toString(m_SceneManager.FogColor)));
	elements.push_back(SCENE_ELEMENT("FogDensity", StringConverter::toString(m_SceneManager.FogDensity)));
	elements.push_back(SCENE_ELEMENT("FogEnd", StringConverter::toString(m_SceneManager.FogEnd)));
	elements.push_back(SCENE_ELEMENT("FogStart", StringConverter::toString(m_SceneManager.FogStart)));
	elements.push_back(SCENE_ELEMENT("Name", m_SceneManager.Name));
	elements.push_back(SCENE_ELEMENT("SkyBoxActive", StringConverter::toString(m_SceneManager.SkyBoxActive)));
	elements.push_back(SCENE_ELEMENT("SkyBoxDistance", StringConverter::toString(m_SceneManager.SkyBoxDistance)));
	elements.push_back(SCENE_ELEMENT("SkyBoxMaterial", m_SceneManager.SkyBoxMaterial));
	elements.push_back(SCENE_ELEMENT("Type", m_SceneManager.Type));
	writer->AddElement("SceneManager", elements);

	// add viewport
	elements.clear();
	elements.push_back(SCENE_ELEMENT("Color", StringConverter::toString(m_Viewport.Color)));
	elements.push_back(SCENE_ELEMENT("Compositor0Enabled", StringConverter::toString(m_Viewport.Compositor0Enabled)));
	elements.push_back(SCENE_ELEMENT("Compositor0Name", m_Viewport.Compositor0Name));
	elements.push_back(SCENE_ELEMENT("Name", m_Viewport.Name));
	elements.push_back(SCENE_ELEMENT("Parent", m_Viewport.Parent));
	writer->AddElement("Viewport", elements);

	// add nodes
	for(u32 i = 0; i < m_Nodes.size(); i++)
	{
		elements.clear();
		elements.push_back(SCENE_ELEMENT("Name", m_Nodes[i].Name));
		elements.push_back(SCENE_ELEMENT("Orientation", StringConverter::toString(m_Nodes[i].Orientation)));
		elements.push_back(SCENE_ELEMENT("Parent", m_Nodes[i].Parent));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_Nodes[i].Position)));
		elements.push_back(SCENE_ELEMENT("Scale", StringConverter::toString(m_Nodes[i].Scale)));
		elements.push_back(SCENE_ELEMENT("AttachTo", m_Nodes[i].AttachTo));
		writer->AddElement("Node", elements);
	}

	// add enities
	for(u32 i = 0; i < m_Entities.size(); i++)
	{
		elements.clear();
		elements.push_back(SCENE_ELEMENT("Name", m_Entities[i].Name));
		elements.push_back(SCENE_ELEMENT("MeshFile", m_Entities[i].MeshFile));
		elements.push_back(SCENE_ELEMENT("Parent", m_Entities[i].Parent));
		elements.push_back(SCENE_ELEMENT("Shadows", StringConverter::toString(m_Entities[i].Shadows)));
		//elements.push_back(SCENE_ELEMENT("SubEntities", StringConverter::toString(m_Entities[i].SubEntities)));
		writer->AddElement("Entity", elements);
	}

	// add subenities
	for(u32 i = 0; i < m_Entities.size(); i++)
	{
		elements.clear();
		for(u32 j = 0; j < m_Entities[i].SubEntities.size(); j++)
		{
			elements.push_back(SCENE_ELEMENT("Parent", m_Entities[i].SubEntities[j].Parent));
			elements.push_back(SCENE_ELEMENT("Index", StringConverter::toString(m_Entities[i].SubEntities[j].Index)));
			elements.push_back(SCENE_ELEMENT("Material", m_Entities[i].SubEntities[j].Material));
			elements.push_back(SCENE_ELEMENT("Visible", StringConverter::toString(m_Entities[i].SubEntities[j].Visible)));
			writer->AddElement("Subentity", elements);
		}
	}

	// add lights
	for(u32 i = 0; i < m_Lights.size(); i++)
	{
		elements.clear();
		elements.push_back(SCENE_ELEMENT("Name", m_Lights[i].Name));
		elements.push_back(SCENE_ELEMENT("Attenuation", StringConverter::toString(m_Lights[i].Attenuation)));
		elements.push_back(SCENE_ELEMENT("Parent", m_Lights[i].Parent));
		elements.push_back(SCENE_ELEMENT("Diffuse", StringConverter::toString(m_Lights[i].Diffuse)));
		elements.push_back(SCENE_ELEMENT("Direction", StringConverter::toString(m_Lights[i].Direction)));
		elements.push_back(SCENE_ELEMENT("LightType", StringConverter::toString(m_Lights[i].LightType)));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_Lights[i].Position)));
		elements.push_back(SCENE_ELEMENT("Power", StringConverter::toString(m_Lights[i].Power)));
		elements.push_back(SCENE_ELEMENT("Range", StringConverter::toString(m_Lights[i].Range)));
		elements.push_back(SCENE_ELEMENT("Shadows", StringConverter::toString(m_Lights[i].Shadows)));
		elements.push_back(SCENE_ELEMENT("Specular", StringConverter::toString(m_Lights[i].Specular)));
		writer->AddElement("Light", elements);
	}

	return true;
}

#endif