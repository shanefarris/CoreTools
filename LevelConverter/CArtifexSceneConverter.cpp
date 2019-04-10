#ifdef COMPILE_ARTIFEX
#include "CArtifexSceneConverter.h"
#include "CFileSystem.h"
#include "CConfig.h"
#include "Structures.h"
#include "CSceneWriterXml.h"

using namespace DAL;
using namespace IO;
using namespace Core::SceneLoader;

CArtifexSceneConverter::CArtifexSceneConverter()
{
	db = 0;
}

CArtifexSceneConverter::~CArtifexSceneConverter()
{
}

bool CArtifexSceneConverter::Convert(std::string FileName)
{
	return false;
}

bool CArtifexSceneConverter::Convert(std::string FileName, std::string Terrain, std::string Zone)
{
	try
	{
		m_FileName = FileName;
		// Iterate through each row in the objects database and load the entities and nodes
		StderrLog log;
		db = new Database(FileName, &log); // sqlite3 specific

		// read and display all records
		Query q(*db);
		q.get_result("select * from Objects");
		while (q.fetch_row())
		{
			// spawns an object from Query object
			Objects tbl(db,&q); 
						
			// parse the entity and node data
			ParseEntity(&tbl);

		}
		q.free_result();

		ReadTerrainConfig(Terrain);
		ReadZoneConfig(Zone);

		// Save a new file
		SaveData();
	}
	catch(...)
	{
		return false;
	}

	return true;
}

bool CArtifexSceneConverter::ReadTerrainConfig(std::string FileName)
{
	try
	{
		IO::CFileSystem FileSystem;
		// Parse the m_HeightTerrain.cfg file
		IO::CConfig config(&FileSystem);
		bool retVal = config.ReadConfig(FileName);

		// if the parsing didn't work return false
		if(!retVal)
			return false;

		// DetailTile
		if(config.GetValue("DetailTile") != "")
		{
			m_HeightTerrain.DetailTile = StringConverter::parseInt(config.GetValue("DetailTile"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- DetailTile is empty" << std::endl;
			return false;
		}

		// heightmap
		if(config.GetValue("Heightmap.image") != "")
		{
			m_HeightTerrain.HeightmapImage = config.GetValue("Heightmap.image");
		}
		else
		{
			std::cout << "ReadTerrainConfig -- Heightmap.image is empty" << std::endl;
			return false;
		}

		// MaxHeight
		if(config.GetValue("MaxHeight") != "")
		{
			m_HeightTerrain.MaxHeight = StringConverter::parseReal(config.GetValue("MaxHeight"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- MaxHeight is empty" << std::endl;
			return false;
		}

		// LODMorphStart
		if(config.GetValue("LODMorphStart") != "")
		{
			m_HeightTerrain.LODMorphStart = StringConverter::parseReal(config.GetValue("LODMorphStart"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- LODMorphStart is empty" << std::endl;
			return false;
		}

		// MaxMipMapLevel
		if(config.GetValue("MaxMipMapLevel") != "")
		{
			m_HeightTerrain.MaxMipMapLevel = StringConverter::parseUnsignedInt(config.GetValue("MaxMipMapLevel"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- MaxMipMapLevel is empty" << std::endl;
			return false;
		}

		// MaxPixelError
		if(config.GetValue("MaxPixelError") != "")
		{
			m_HeightTerrain.MaxPixelError = StringConverter::parseUnsignedInt(config.GetValue("MaxPixelError"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- MaxPixelError is empty" << std::endl;
			return false;
		}

		// MorphLODFactorParamIndex
		if(config.GetValue("MorphLODFactorParamIndex") != "")
		{
			m_HeightTerrain.MorphLODFactorParamIndex = StringConverter::parseUnsignedInt(config.GetValue("MorphLODFactorParamIndex"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- MorphLODFactorParamIndex is empty" << std::endl;
			m_HeightTerrain.MorphLODFactorParamIndex = 0;
		}

		// PageSize
		if(config.GetValue("PageSize") != "")
		{
			m_HeightTerrain.PageSize = StringConverter::parseUnsignedInt(config.GetValue("PageSize"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- PageSize is empty" << std::endl;
			return false;
		}

		// PageWorldX
		if(config.GetValue("PageWorldX") != "")
		{
			m_HeightTerrain.PageWorldX = StringConverter::parseReal(config.GetValue("PageWorldX"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- PageWorldX is empty" << std::endl;
			return false;
		}

		// PageWorldZ
		if(config.GetValue("PageWorldZ") != "")
		{
			m_HeightTerrain.PageWorldZ = StringConverter::parseReal(config.GetValue("PageWorldZ"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- PageWorldZ is empty" << std::endl;
			return false;
		}

		// TileSize
		if(config.GetValue("TileSize") != "")
		{
			m_HeightTerrain.TileSize = StringConverter::parseUnsignedInt(config.GetValue("TileSize"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- TileSize is empty" << std::endl;
			return false;
		}

		// UseTriStrips
		if(config.GetValue("UseTriStrips") != "")
		{
			m_HeightTerrain.UseTriStrips = StringConverter::parseBool(config.GetValue("UseTriStrips"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- UseTriStrips is empty" << std::endl;
			m_HeightTerrain.UseTriStrips = false;
		}

		// VertexColors
		if(config.GetValue("VertexColors") != "")
		{
			m_HeightTerrain.VertexColors = StringConverter::parseBool(config.GetValue("VertexColors"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- VertexColors is empty" << std::endl;
			m_HeightTerrain.VertexColors = false;
		}

		// VertexNormals
		if(config.GetValue("VertexNormals") != "")
		{
			m_HeightTerrain.VertexNormals = StringConverter::parseBool(config.GetValue("VertexNormals"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- VertexNormals is empty" << std::endl;
			m_HeightTerrain.VertexNormals = false;
		}

		// VertexProgramMorph
		if(config.GetValue("VertexProgramMorph") != "")
		{
			m_HeightTerrain.VertexProgramMorph = StringConverter::parseBool(config.GetValue("VertexProgramMorph"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- VertexProgramMorph is empty" << std::endl;
			m_HeightTerrain.VertexProgramMorph = false;
		}

		// HeightmapRawSize
		if(config.GetValue("HeightmapRawSize") != "")
		{
			m_HeightTerrain.HeightmapRawSize = StringConverter::parseBool(config.GetValue("HeightmapRawSize"));
		}
		else
		{
			std::cout << "ReadTerrainConfig -- HeightmapRawSize is empty" << std::endl;
			m_HeightTerrain.HeightmapRawSize = 0;
		}

		m_HeightTerrain.CustomMaterialName = config.GetValue("CustomMaterialName");
		if(m_HeightTerrain.CustomMaterialName != "")
		{
			if(!FileSystem.ExistFile(m_HeightTerrain.CustomMaterialName.c_str()))
			{
				std::cout << "*** CustomMaterialName: Warning file not found, make sure you get it." << std::endl;
			}
		}

		m_HeightTerrain.DetailTexture = config.GetValue("DetailTexture");
		if(m_HeightTerrain.CustomMaterialName != "")
		{
			if(!FileSystem.ExistFile(m_HeightTerrain.DetailTexture.c_str()))
			{
				std::cout << "*** DetailTexture: Warning file not found, make sure you get it." << std::endl;
			}
		}

		m_HeightTerrain.HeightmapRawBpp = config.GetValue("HeightmapRawBpp");
		if(m_HeightTerrain.HeightmapRawBpp != "")
		{
			if(!FileSystem.ExistFile(m_HeightTerrain.HeightmapRawBpp.c_str()))
			{
				std::cout << "*** HeightmapRawBpp: Warning file not found, make sure you get it." << std::endl;
			}
		}

		m_HeightTerrain.WorldTexture = config.GetValue("WorldTexture");
		if(m_HeightTerrain.WorldTexture != "")
		{
			if(!FileSystem.ExistFile(m_HeightTerrain.WorldTexture.c_str()))
			{
				std::cout << "*** WorldTexture: Warning file not found, make sure you get it." << std::endl;
			}
		}
		
		m_HeightTerrain.MorphLODFactorParamName = config.GetValue("MorphLODFactorParamName");
		m_HeightTerrain.Name = config.GetValue("name");
		if(m_HeightTerrain.Name == "")
			m_HeightTerrain.Name = "terrain";
		
	}
	catch(...)
	{
		return false;
	}

	return true;
}


bool CArtifexSceneConverter::ReadZoneConfig(std::string FileName)
{
	try
	{
		IO::CFileSystem FileSystem;
		// Parse the m_HeightTerrain.cfg file
		IO::CConfig config(&FileSystem);
		bool retVal = config.ReadConfig(FileName);

		// if the parsing didn't work return false
		if(!retVal)
			return false;

		// Terrain splatting
		u32 i = 0;
		while(config.GetValue("Splatting" + StringConverter::toString(i)) != "")
		{
			m_HeightTerrain.Splatting.push_back(config.GetValue("Splatting" + StringConverter::toString(i)));
			i++;
		}

		// Water
		if(config.GetValue("WaterVisible") != "")
		{
			m_Water.WaterVisible = StringConverter::parseBool(config.GetValue("WaterVisible"));
		}
		if(config.GetValue("WaterXPos") != "")
		{
			m_Water.WaterXPos = StringConverter::parseReal(config.GetValue("WaterXPos"));
		}
		if(config.GetValue("WaterYPos") != "")
		{
			m_Water.WaterYPos = StringConverter::parseReal(config.GetValue("WaterYPos"));
		}
		if(config.GetValue("WaterZPos") != "")
		{
			m_Water.WaterZPos = StringConverter::parseReal(config.GetValue("WaterZPos"));
		}
		if(config.GetValue("WaterWidth_X") != "")
		{
			m_Water.WaterWidthX = StringConverter::parseReal(config.GetValue("WaterWidth_X"));
		}
		if(config.GetValue("WaterWidth_Z") != "")
		{
			m_Water.WaterWidthZ = StringConverter::parseReal(config.GetValue("WaterWidth_Z"));
		}
		m_Water.WaterMaterial = config.GetValue("WaterMaterial");
	}
	catch(Exception& e)
	{
		std::cerr << e.getDescription() << std::endl;
		return false;
	}
	return true;
}


bool CArtifexSceneConverter::ParseEntity(Objects* tbl)
{
	ENTITY entity;
	entity.Parent = "";					// doesn't have this value
	entity.MeshFile = tbl->GetMeshfile();
	entity.Shadows = false;				// doesn't have this value
	entity.Name = tbl->GetEntity();
	m_Entities.push_back(entity);

	NODE node;
	node.Parent = "";					// doesn't have this value
	node.Orientation = Quaternion(Degree(tbl->GetRot_y()), reVector3Df::UNIT_Y);
	node.Position = reVector3Df(tbl->GetX(), tbl->GetY(), tbl->GetZ());
	node.Scale = reVector3Df(tbl->GetScale_x(), tbl->GetScale_y(), tbl->GetScale_z());
	node.Name = tbl->GetEntity();		// just use the entity name
	node.AttachTo = tbl->GetEntity();
	m_Nodes.push_back(node);

	return true;
}

bool CArtifexSceneConverter::SaveData()
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
		CORE_THROW("Could not create scene file", "CArtifexSceneConverter::SaveData");
	}

	std::vector<SCENE_ELEMENT> elements;

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

	elements.clear();
	elements.push_back(SCENE_ELEMENT("Name", m_HeightTerrain.Name));
	elements.push_back(SCENE_ELEMENT("Heightmap.image", m_HeightTerrain.HeightmapImage));
	elements.push_back(SCENE_ELEMENT("WorldTexture", m_HeightTerrain.WorldTexture));
	elements.push_back(SCENE_ELEMENT("DetailTexture", m_HeightTerrain.DetailTexture));
	elements.push_back(SCENE_ELEMENT("DetailTile", StringConverter::toString(m_HeightTerrain.DetailTile)));
	elements.push_back(SCENE_ELEMENT("Heightmap.raw.size", StringConverter::toString(m_HeightTerrain.HeightmapRawSize)));
	elements.push_back(SCENE_ELEMENT("Heightmap.raw.bpp", m_HeightTerrain.HeightmapRawBpp));
	elements.push_back(SCENE_ELEMENT("PageSize", StringConverter::toString(m_HeightTerrain.PageSize)));
	elements.push_back(SCENE_ELEMENT("TileSize", StringConverter::toString(m_HeightTerrain.TileSize)));
	elements.push_back(SCENE_ELEMENT("MaxPixelError", StringConverter::toString(m_HeightTerrain.MaxPixelError)));
	elements.push_back(SCENE_ELEMENT("PageWorldX", StringConverter::toString(m_HeightTerrain.PageWorldX)));
	elements.push_back(SCENE_ELEMENT("PageWorldZ", StringConverter::toString(m_HeightTerrain.PageWorldZ)));
	elements.push_back(SCENE_ELEMENT("MaxHeight", StringConverter::toString(m_HeightTerrain.MaxHeight)));
	elements.push_back(SCENE_ELEMENT("MaxMipMapLevel", StringConverter::toString(m_HeightTerrain.MaxMipMapLevel)));
	elements.push_back(SCENE_ELEMENT("VertexNormals", StringConverter::toString(m_HeightTerrain.VertexNormals)));
	elements.push_back(SCENE_ELEMENT("VertexColors", StringConverter::toString(m_HeightTerrain.VertexColors)));
	elements.push_back(SCENE_ELEMENT("UseTriStrips", StringConverter::toString(m_HeightTerrain.UseTriStrips)));
	elements.push_back(SCENE_ELEMENT("VertexProgramMorph", StringConverter::toString(m_HeightTerrain.VertexProgramMorph)));
	elements.push_back(SCENE_ELEMENT("LODMorphStart", StringConverter::toString(m_HeightTerrain.LODMorphStart)));
	elements.push_back(SCENE_ELEMENT("MorphLODFactorParamName", m_HeightTerrain.MorphLODFactorParamName));
	elements.push_back(SCENE_ELEMENT("MorphLODFactorParamIndex", StringConverter::toString(m_HeightTerrain.MorphLODFactorParamIndex)));
	elements.push_back(SCENE_ELEMENT("CustomMaterialName", m_HeightTerrain.CustomMaterialName));
	String splatting;
	for(u32 i = 0; i < m_HeightTerrain.Splatting.size(); i++)
	{
		if(splatting.length() > 0)
			splatting += ",";
		splatting += m_HeightTerrain.Splatting[i];
	}
	elements.push_back(SCENE_ELEMENT("Splatting", splatting));
	writer->AddElement("Terrain", elements);

	elements.clear();
	elements.push_back(SCENE_ELEMENT("WaterMaterial", m_Water.WaterMaterial));
	elements.push_back(SCENE_ELEMENT("WaterVisible", StringConverter::toString(m_Water.WaterVisible)));
	elements.push_back(SCENE_ELEMENT("WaterWidthX", StringConverter::toString(m_Water.WaterWidthX)));
	elements.push_back(SCENE_ELEMENT("WaterWidthZ", StringConverter::toString(m_Water.WaterWidthZ)));
	elements.push_back(SCENE_ELEMENT("WaterXPos", StringConverter::toString(m_Water.WaterXPos)));
	elements.push_back(SCENE_ELEMENT("WaterYPos", StringConverter::toString(m_Water.WaterYPos)));
	elements.push_back(SCENE_ELEMENT("WaterZPos", StringConverter::toString(m_Water.WaterZPos)));
	writer->AddElement("Water", elements);

	return true;
}
#endif