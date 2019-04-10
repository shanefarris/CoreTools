// Converts 2 formats at this point but is ment to be used to convert any format to the Core format

#include <iostream>
#include <stdio.h>
#include <string>
#include <vector>
#include <windows.h>

#include "IConverter.h"
#include "CDotSceneConverter.h"
#include "COgSceneConverter.h"
#include "CArtifexSceneConverter.h"

void printHelp(void)
{
    // Print help message
    std::cout << std::endl << "LevelConverter " << std::endl;
    std::cout << "Usage: LevelConverter [options] FileToConvert " << std::endl;
    std::cout << "Options:" << std::endl;
    std::cout << "    -help\t= Prints this help text" << std::endl;
    std::cout << "    -scene\t= Converts from .scene file" << std::endl;
	std::cout << "    -ogscene\t= Converts from .ogscene file" << std::endl;
	std::cout << "    -artifex\t= Converts from .s3db artifex file" << std::endl;
    std::cout << std::endl;
	std::cout << "For -artifex use: LevelConverter -artifex database.db terrain.cfg zonesettings.cfg" << std::endl;
	std::cout << std::endl;
}

int main(int argc, const char** argv)
{
    if (argc < 3)
    {
        printHelp();
        return -1;
    }

	try
	{
		IConverter* converter = 0;
		std::string fileType = argv[1];

		if(fileType == "-ogscene")
		{
			std::string file = argv[2];
			converter = new COgSceneConverter();
			converter->Convert(file);
		}
		#ifdef COMPILE_DOTSCENE
		else if(fileType == "-scene")
		{
			std::string file = argv[2];
			converter = new CDotSceneConverter();
			converter->Convert(file);
		}
		#endif
		#ifdef COMPILE_ARTIFEX
		else if(fileType == "-artifex")
		{
			if(argc < 5)
			{
				std::cout << "Terrain.cfg and Zonesettings.cfg files requried" << std::endl;
				return -1;
			}
			else
			{
				std::string file = argv[2];
				converter = new CArtifexSceneConverter();
				((CArtifexSceneConverter*)converter)->Convert(file, argv[3], argv[4]);
			}
		}
		#endif
		else
		{
			printHelp();
			return -1;
		}
		converter->PrintDebug();
	}
    catch (std::exception& ex)
    {
		std::cout << "Parsing global options failed:" << std::endl;
        std::cout << ex.what() << std::endl;
        return -1;
    }
    catch (...)
    {
		std::cout << "Parsing global options failed." << std::endl;
        return -1;
    }

    return 0;
}

void IConverter::PrintDebug()
{
	cout << "SCENEMANAGER" << endl;
	cout << "Name: " << m_SceneManager.Name << endl;
	cout << "Type: " << m_SceneManager.Type << endl;
	cout << "FogDensity: " << m_SceneManager.FogDensity << endl;
	cout << "FogEnd: " << m_SceneManager.FogEnd << endl;
	cout << "FogStart: " << m_SceneManager.FogStart << endl;
	cout << "FogMode: " << m_SceneManager.FogMode << endl;
	cout << "SkyBoxActive: " << m_SceneManager.SkyBoxActive << endl;
	cout << "SkyBoxDistance: " << m_SceneManager.SkyBoxDistance << endl;
	cout << "SkyBoxMaterial: " << m_SceneManager.SkyBoxMaterial << endl;
	cout << "Ambient: " << m_SceneManager.Ambient.a << " " << m_SceneManager.Ambient.b << " " << m_SceneManager.Ambient.g << endl;
	cout << "FogColor: " << m_SceneManager.FogColor.a << " " << m_SceneManager.FogColor.b << " " << m_SceneManager.FogColor.g << endl;
	cout << endl;

	cout << "VIEWPORT" << endl;
	cout << "Name: " << m_Viewport.Name << endl;
	cout << "Parent: " << m_Viewport.Parent << endl;
	cout << "Color: " << m_Viewport.Color.a << " " << m_Viewport.Color.b << " " << m_Viewport.Color.g << endl;
	cout << "Compositor0Enabled: " << m_Viewport.Compositor0Enabled << endl;
	cout << "Compositor0Name: " << m_Viewport.Compositor0Name << endl;
	cout << endl;

	for(u32 i = 0; i < m_GameObjects.size(); i++)
	{
		cout << "Game Object" << endl;
		cout << "Name: " << m_GameObjects[i].Name << endl;
		cout << "Parent: " << m_GameObjects[i].Parent << endl;
		cout << "Orientation: " << m_GameObjects[i].Orientation.x << " " << m_GameObjects[i].Orientation.y << " " << m_GameObjects[i].Orientation.z << " " << m_GameObjects[i].Orientation.w << endl;
		cout << "Position: " << m_GameObjects[i].Position.x << " " << m_GameObjects[i].Position.y << " " << m_GameObjects[i].Position.y << endl;
		cout << "Scale: " << m_GameObjects[i].Scale.x << " " << m_GameObjects[i].Scale.y << " " << m_GameObjects[i].Scale.y << endl;
		cout << "MeshFile: " << m_GameObjects[i].MeshFile << endl;
		cout << "Shadows: " << m_GameObjects[i].Shadows << endl;
	}
	cout << endl;

	for(u32 i = 0; i < m_Lights.size(); i++)
	{
		cout << "LIGHT" << endl;
		cout << "Name: " << m_Lights[i].Name << endl;
		cout << "Parent: " << m_Lights[i].Parent << endl;
		cout << "Position: " << m_Lights[i].Position.x << " " << m_Lights[i].Position.y << " " << m_Lights[i].Position.y << endl;
		cout << "Attenuation: " << m_Lights[i].Attenuation.x << " " << m_Lights[i].Attenuation.y << " " << m_Lights[i].Attenuation.y << " " << m_Lights[i].Attenuation.x << endl;
		cout << "Diffuse: " << m_Lights[i].Diffuse.x << " " << m_Lights[i].Diffuse.y << " " << m_Lights[i].Diffuse.y << endl;
		cout << "Direction: " << m_Lights[i].Direction.x << " " << m_Lights[i].Direction.y << " " << m_Lights[i].Direction.y << endl;
		cout << "LightType: " << m_Lights[i].LightType << endl;
		cout << "Power: " << m_Lights[i].Power << endl;
		cout << "Range: " << m_Lights[i].Range.x << " " << m_Lights[i].Range.y << " " << m_Lights[i].Range.y << endl;
		cout << "Shadows: " << m_Lights[i].Shadows << endl;
		cout << "Specular: " << m_Lights[i].Specular.x << " " << m_Lights[i].Specular.y << " " << m_Lights[i].Specular.y << endl;
	}
	cout << endl;

	for(u32 i = 0; i < m_Cameras.size(); i++)
	{
		cout << "CAMERA" << endl;
		cout << "Name: " << m_Cameras[i].Name << endl;
		cout << "Parent: " << m_Cameras[i].Parent << endl;
		cout << "Position: " << m_Cameras[i].Position.x << " " << m_Cameras[i].Position.y << " " << m_Cameras[i].Position.y << endl;
		cout << "Orientation: " << m_Cameras[i].Orientation.x << " " << m_Cameras[i].Orientation.y << " " << m_Cameras[i].Orientation.y << " " <<  m_Cameras[i].Orientation.w << endl;
		cout << "Fov: " << m_Cameras[i].Fov << endl;
		cout << "ClipDistance: " << m_Cameras[i].ClipDistance.x << " " << m_Cameras[i].ClipDistance.y << endl;
	}

	cout << endl;

	for(u32 i = 0; i < m_PhysicsProfiles.size(); i++)
	{
		cout << "PHYSICS NODES" << endl;
		cout << "ObjectName: " << m_PhysicsProfiles[i].Name << endl;
		cout << "Shape: " << m_PhysicsProfiles[i].Shape << endl;
		cout << "Mass: " << m_PhysicsProfiles[i].Mass << endl;
		cout << "Friction: " << m_PhysicsProfiles[i].Friction << endl;
		cout << "Restitution: " << m_PhysicsProfiles[i].Restitution << endl;
	}
	cout << endl;

	cout << "CAELUM" << endl;
	cout << "Name: " << m_Caelum.Name << endl;
	cout << endl;

	cout << "HYDRAX" << endl;
	cout << "Name: " << m_Hydrax.Name << endl;
	cout << "ConfigFile: " << m_Hydrax.ConfigFile << endl;
	cout << endl;

	cout << "HEIGHT_TERRAIN" << endl;
	cout << "Name: " << m_HeightTerrain.Name << endl; 
	cout << "HeightmapImage: " << m_HeightTerrain.HeightmapImage << endl;
	cout << "WorldTexture: " << m_HeightTerrain.WorldTexture << endl;
	cout << "DetailTexture: " << m_HeightTerrain.DetailTexture << endl;
	cout << "DetailTile: " << m_HeightTerrain.DetailTile << endl;
	cout << "HeightmapRawSize: " << m_HeightTerrain.HeightmapRawSize << endl;
	cout << "HeightmapRawBpp: " << m_HeightTerrain.HeightmapRawBpp << endl;
	cout << "PageSize: " << m_HeightTerrain.PageSize << endl;
	cout << "TileSize: " << m_HeightTerrain.TileSize << endl;
	cout << "MaxPixelError: " << m_HeightTerrain.MaxPixelError << endl;
	cout << "PageWorldX: " << m_HeightTerrain.PageWorldX << endl;
	cout << "PageWorldZ: " << m_HeightTerrain.PageWorldZ << endl;
	cout << "MaxHeight: " << m_HeightTerrain.MaxHeight << endl;
	cout << "MaxMipMapLevel: " << m_HeightTerrain.MaxMipMapLevel << endl;
	cout << "VertexNormals: " << m_HeightTerrain.VertexNormals << endl;
	cout << "VertexColors: " << m_HeightTerrain.VertexColors << endl;
	cout << "UseTriStrips: " << m_HeightTerrain.UseTriStrips << endl;
	cout << "VertexProgramMorph: " << m_HeightTerrain.VertexProgramMorph << endl;
	cout << "LODMorphStart: " << m_HeightTerrain.LODMorphStart << endl;
	cout << "MorphLODFactorParamName: " << m_HeightTerrain.MorphLODFactorParamName << endl;
	cout << "MorphLODFactorParamIndex: " << m_HeightTerrain.MorphLODFactorParamIndex << endl;
	cout << "CustomMaterialName: " << m_HeightTerrain.CustomMaterialName << endl;
	cout << endl;

	cout << "WATER" << endl;
	cout << "WaterVisible: " << m_Water.WaterVisible << endl;
	cout << "WaterWidthX: " << m_Water.WaterWidthX << endl;
	cout << "WaterWidthZ: " << m_Water.WaterWidthZ << endl;
	cout << "WaterXPos: " << m_Water.WaterXPos << endl;
	cout << "WaterYPos: " << m_Water.WaterYPos << endl;
	cout << "WaterZPos: " << m_Water.WaterZPos << endl;
	cout << endl;
}