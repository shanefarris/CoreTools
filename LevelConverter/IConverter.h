// Interface for any class that wants to convert from one format to the Core format.  This interface could
// be used to convert to any other format, but it is ment to convert to the Core format.

#ifndef __ICONVERTER_H__
#define __ICONVERTER_H__

#include <string>
#include "Defines.h"
#include "Structures.h"
#include "IXMLReader.h"
#include "IXMLWriter.h"
#include "CFileSystem.h"

#define CUSTOM_EXTENSION "scx"

using namespace IO;
using namespace Core::SceneLoader;

struct SCENE_ELEMENT
{
	SCENE_ELEMENT(const char* name, const char* value)
	{
		Name = name;
		Value = value;
	}

	SCENE_ELEMENT(const char* name, String& value)
	{
		Name = name;
		Value = value.c_str();
	}

	String Name;
	String Value;
};

class IConverter
{
public:
	virtual bool Convert(std::string FileName) = 0;

	void PrintDebug();

protected:
	virtual bool SaveData() = 0;

	SCENEMANAGER		m_SceneManager;
	VIEWPORT			m_Viewport;
	CAELUM				m_Caelum;
	HYDRAX				m_Hydrax;
	BGSOUND				m_BgSound;
	HEIGHT_TERRAIN		m_HeightTerrain;
	WATER				m_Water;
	PAGED_TERRAIN		m_PagedTerrain;

	std::vector<LIGHT>			 m_Lights;
	std::vector<CAMERA>			 m_Cameras;
	std::vector<GAMEOBJECT>		 m_GameObjects;
	std::vector<PHYSICS_PROFILE> m_PhysicsProfiles;
	std::vector<SceneLoader::SPAWN_POINT>	m_SpawnPoint;

	std::string			m_FileName;
	IO::IXMLWriter*		m_Writer;

	bool CreateFile(const char* FileName)
	{
		m_FileName = String(FileName);
		CFileSystem* FileSystem = new CFileSystem();
		m_Writer = FileSystem->CreateXMLWriter(FileName);
		if(!m_Writer)
			return false;

		return true;
	}

	bool AddElement(const char* Name, std::vector<SCENE_ELEMENT>& Elements)
	{
		if(!m_Writer)
			return false;

		std::vector<std::string> Names;
		std::vector<std::string> Values;
		std::vector<SCENE_ELEMENT>::iterator it;
		for(it = Elements.begin(); it != Elements.end(); it++)
		{
			String str = String((*it).Name);
			std::transform(str.begin(), str.end(), str.begin(), tolower); // set the attribute name to lower case
			Names.push_back(str);
			Values.push_back((*it).Value);
		}

		m_Writer->WriteElement(Name, false, Names, Values);

		m_Writer->WriteClosingTag(Name);
		m_Writer->WriteLineBreak();

		return true;
	}
};

#endif // __ICONVERTER_H__