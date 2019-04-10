#include "COgSceneConverter.h"
#include "CFileSystem.h"
#include "CUtility.h"

using namespace IO;
using namespace Core;
using namespace Core::SceneLoader;

COgSceneConverter::COgSceneConverter()
{
}

COgSceneConverter::~COgSceneConverter()
{
}

bool COgSceneConverter::Convert(std::string FileName)
{
	CFileSystem FileSystem;
	IXMLReader* xml;

	m_FileName = FileName;
	xml = FileSystem.CreateXMLReader(FileName.c_str());
	if(!xml)
		return false;

	while(xml && xml->Read())
    {
		if (String("OBJECT") == xml->GetNodeName() && 0 != xml->GetAttributeValue("typename"))
		{
			if (String("OctreeSceneManager") == xml->GetAttributeValue("typename"))
			{
				ParseSceneManager(xml);
			}
			else if (String("Viewport Object") == xml->GetAttributeValue("typename"))
			{
				ParseViewport(xml);
			}
			else if (String("Entity Object") == xml->GetAttributeValue("typename"))
			{
				ParseEntity(xml);
			}
			else if (String("Light Object") == xml->GetAttributeValue("typename"))
			{
				ParseLight(xml);
			}
			else if (String("Camera Object") == xml->GetAttributeValue("typename"))
			{
				ParseCamera(xml);
			}
			else if (String("Terrain Group Object") == xml->GetAttributeValue("typename"))
			{
				ParseTerrain(xml);
			}
			else if (String("Caelum Object") == xml->GetAttributeValue("typename"))
			{
				ParseCaelum(xml);
			}
			else if (String("Hydrax Object") == xml->GetAttributeValue("typename"))
			{
				ParseHydrax(xml);
			}
			else if (String("Marker Object") == xml->GetAttributeValue("typename"))
			{
				ParseMarker(xml);
			}
			else if (String("Terrain Page Object") == xml->GetAttributeValue("typename"))
			{
				ParseTerrainName(xml);
			}
		}
    }

	// Finish checks and validating the data
	if(!Validate())
		return false;

    if (xml)
		delete xml;

	// Save a new file
	SaveData();

	return true;
}

void COgSceneConverter::ParseSceneManager(IXMLReader* xml)
{
	/*  Not collecting: rendering distance, locked, layer, position, orientation, scale,
	 *  shadows enabled, shadows::renderingdistance, shadows::resolutionfar, shadows::resolutionmiddle,
	 *  shadows::resolutionnear, shadows::technique, 
	 */

	string str = xml->GetNodeName();
	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		str = xml->GetNodeName();
		str = xml->GetAttributeValueSafe ("id");
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("ambient"))
				m_SceneManager.Ambient = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::colour"))
				m_SceneManager.FogColor = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::density"))
				m_SceneManager.FogDensity = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::end"))
				m_SceneManager.FogEnd = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::start"))
				m_SceneManager.FogStart = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("skybox::active"))
				m_SceneManager.SkyBoxActive = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("skybox::distance"))
				m_SceneManager.SkyBoxDistance = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("skybox::material"))
				m_SceneManager.SkyBoxMaterial = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("scenemanagertype"))
				m_SceneManager.Type = xml->GetAttributeValueSafe("value");

			else if(xml->GetAttributeValue("id") == String("fog::mode"))
			{
				if(xml->GetAttributeValueSafe("value") == String("0"))
					m_SceneManager.FogMode = FOG_NONE;
				else if(xml->GetAttributeValueSafe("value") == String("1"))
					m_SceneManager.FogMode = FOG_EXP;
				else if(xml->GetAttributeValueSafe("value") == String("2"))
					m_SceneManager.FogMode = FOG_EXP2;
				else if(xml->GetAttributeValueSafe("value") == String("3"))
					m_SceneManager.FogMode = FOG_LINEAR;
			}
		}
		xml->Read();
	}
	m_SceneManager.Name = "MySceneManager";
}

void COgSceneConverter::ParseViewport(IXMLReader* xml)
{
	if (String("Viewport Object") == xml->GetAttributeValueSafe("typename"))
	{
		m_Viewport.Name = String(xml->GetAttributeValueSafe("name"));
	}

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("colour"))
				m_Viewport.Color = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
		}
		xml->Read();
	}
}

void COgSceneConverter::ParseCamera(IXMLReader* xml)
{
	/*  Not collecting: autoaspectratio, autotracktarget
	 */

	CAMERA camera;

	// This needs to be set manually in the xml
	camera.Type = ECM_FREE;

	if (String("Camera Object") == xml->GetAttributeValueSafe("typename"))
	{
		camera.Name = String(xml->GetAttributeValueSafe("name"));
	}

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("clipdistance"))
				camera.ClipDistance = StringConverter::parseVector2(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fov"))
				camera.Fov = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("orientation"))
				camera.Orientation = StringConverter::parseQuaternion(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("position"))
				camera.Position = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
		}
		xml->Read();
	}

	camera.LookAt = reVector3Df(0.0, 0.0, 0.0);		// TODO: this information isn't saved in ogScene
	camera.Default = true;							// TODO: All are "default" for now
	m_Cameras.push_back(camera);
}

void COgSceneConverter::ParseLight(IXMLReader* xml)
{
	/*  Not collecting: parent, orientation
	 */

	LIGHT light;

	if (String("Light Object") == xml->GetAttributeValueSafe("typename"))
	{
		light.Name = String(xml->GetAttributeValueSafe("name"));
	}

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("attenuation"))
				light.Attenuation = StringConverter::parseVector4(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("castshadows"))
				light.Shadows = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("diffuse"))
				light.Diffuse = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("direction"))
				light.Direction = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lightrange"))
				light.Range = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("position"))
				light.Position = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("power"))
				light.Power = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("specular"))
				light.Specular = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lighttype"))
			{
				if(xml->GetAttributeValueSafe("value") == String("0"))
					light.LightType = LT_POINT;
				else if(xml->GetAttributeValueSafe("value") == String("1"))
					light.LightType = LT_DIRECTIONAL;
				else
					light.LightType = LT_SPOT;
			}
		}
		xml->Read();
	}

	m_Lights.push_back(light);
}

void COgSceneConverter::ParseEntity(IXMLReader* xml)
{
	/*  Not collecting: autotracktarget, renderingdistance, subentity
	 */

	GAMEOBJECT gameObject;
	if (String("Entity Object") == xml->GetAttributeValueSafe("typename"))
	{
		gameObject.Name = String(xml->GetAttributeValueSafe("name"));
	}

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("castshadows"))
				gameObject.Shadows = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("meshfile"))
				gameObject.MeshFile = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("orientation"))
				gameObject.Orientation = StringConverter::parseQuaternion(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("position"))
				gameObject.Position = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("scale"))
				gameObject.Scale = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else 
			{
				// TODO: sub-entities
				size_t found = String(xml->GetAttributeValue("id")).find("subentity");
				if(found != string::npos)
				{
				}
			}
		}
		xml->Read();
	}

	// Create a sphere obstacle by default
	gameObject.ObstacleType = "sphere";
	
	if(gameObject.Name.length() == 0)
	{
		CORE_THROW("Name for entity not found.", "COgSceneConverter::ParseEntity");
	}

	m_GameObjects.push_back(gameObject);
}

void COgSceneConverter::ParseTerrain(IXMLReader* xml)
{
	/*  Not collecting: tuning::compositemapdistance
	 */

	m_PagedTerrain.Name = "Terrain";
	m_PagedTerrain.MaxX = 0;
	m_PagedTerrain.MinX = 0;
	m_PagedTerrain.MaxY = 0;
	m_PagedTerrain.MinY = 0;

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("blendmap::texturesize"))
				m_PagedTerrain.TextureSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("colourmap::enabled"))
				m_PagedTerrain.isColorMap = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("colourmap::texturesize"))
				m_PagedTerrain.ColorMapTextureSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lightmap::texturesize"))
				m_PagedTerrain.LightMapTextureSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pagemapsize"))
				m_PagedTerrain.MapSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pageworldsize"))
				m_PagedTerrain.WorldSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pg::densitymapsize"))
				m_PagedTerrain.PageDesityMapSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pg::detaildistance"))
				m_PagedTerrain.DetailDistance = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pg::pagesize"))
				m_PagedTerrain.PageSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("tuning::maxbatchsize"))
				m_PagedTerrain.MaxBatchSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("tuning::maxpixelerror"))
				m_PagedTerrain.MaxPixelError = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("tuning::minbatchsize"))
				m_PagedTerrain.MinBatchSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
		}
		xml->Read();

		m_PagedTerrain.Position = reVector3Df(0, 0, 0);

		m_PagedTerrain.ResourceGroup = ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME;
		m_PagedTerrain.TerrainFile = "Page";
	}
}

void COgSceneConverter::ParseCaelum(IXMLReader* xml)
{
	/*  Not collecting: cloud data becaues there are multiple layers, and it is not a high priority at the moment
	 */

	m_Caelum.Name = "CaelumSky";

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("clock::day"))
				m_Caelum.TimeDay = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::hour"))
				m_Caelum.TimeHour = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::minute"))
				m_Caelum.TimeMinute = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::month"))
				m_Caelum.TimeMonth = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::second"))
				m_Caelum.TimeSec = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::speed"))
				m_Caelum.TimeSpeed = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clock::year"))
				m_Caelum.TimeYear = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clouds::enable"))
				m_Caelum.isClouds = true; //StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("clouds::layer_count"))
				m_Caelum.LayerOfClouds = 1; // StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::density_multiplier"))
				m_Caelum.FogDenMultiplier = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("fog::manage"))
				m_Caelum.isManageFog = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("layer"))
				m_Caelum.Layer = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lighting::ensure_single_lightsource"))
				m_Caelum.isSingleLightSource = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lighting::ensure_single_shadowsource"))
				m_Caelum.isSingleShadowSource = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lighting::manage_ambient_light"))
				m_Caelum.isManageAmbientLight = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("lighting::minimum_ambient_light"))
				m_Caelum.MinAmbientLight = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::ambient_multiplier"))
				m_Caelum.MoonAmbientMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::attenuation::constant_multiplier"))
				m_Caelum.MoonAttMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::attenuation::distance"))
				m_Caelum.MoonAttDistance = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::attenuation::linear_multiplier"))
				m_Caelum.MoonAttLinearMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::attenuation::quadric_multiplier"))
				m_Caelum.MoonQuadMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::auto_disable"))
				m_Caelum.isMoonAutoDisable = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::cast_shadow"))
				m_Caelum.isMoonCastShadow = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::diffuse_multiplier"))
				m_Caelum.MoonDiffuseMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::enable"))
				m_Caelum.isMoonEnabled = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("moon::specular_multiplier"))
				m_Caelum.MoonSpecularMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("observer::latitude"))
				m_Caelum.ObserverLatitude = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("observer::longitude"))
				m_Caelum.ObserverLongitude = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("stars::enable"))
				m_Caelum.isStarsEnabled = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("stars::mag0_pixel_size"))
				m_Caelum.StarsMag0PixelSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("stars::magnitude_scale"))
				m_Caelum.StarsMagScale = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("stars::max_pixel_size"))
				m_Caelum.StarsMaxPixelSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("stars::min_pixel_size"))
				m_Caelum.StarsMinPixelSize = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::ambient_multiplier"))
				m_Caelum.SunAmbientMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::attenuation::constant_multiplier"))
				m_Caelum.SunAttMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::attenuation::distance"))
				m_Caelum.SunDistance = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::attenuation::linear_multiplier"))
				m_Caelum.SunAttLinearMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::attenuation::quadric_multiplier"))
				m_Caelum.SunQuadMultipler = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::auto_disable"))
				m_Caelum.isSunAutoDisable = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::cast_shadow"))
				m_Caelum.isSunCastShadow = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::colour"))
				m_Caelum.SunColor = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::diffuse_multiplier"))
				m_Caelum.SunDiffuseMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::enable"))
				m_Caelum.isSunEnabled = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::lightcolour"))
				m_Caelum.SunLightColor = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::position"))
				m_Caelum.SunPosition = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::specular_multiplier"))
				m_Caelum.SunSpecularMultipler = StringConverter::parseColourValue(xml->GetAttributeValueSafe("value"));
		}
		xml->Read();
	}
}

void COgSceneConverter::ParseHydrax(IXMLReader* xml)
{
	/*  Not collecting: 
	 */

	m_Hydrax.Name = "HydraxWater";

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("caelumintegration"))
				m_Hydrax.isCaelumItegrated = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("caustics::end"))
				m_Hydrax.CausticsEnd = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("caustics::power"))
				m_Hydrax.CausticsPower = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("caustics::scale"))
				m_Hydrax.CausticsScale = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));

			// Componets
			else if(xml->GetAttributeValue("id") == String("components::caustics"))
				m_Hydrax.isComponentsCaustics = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::depth"))
				m_Hydrax.isComponentsDepth = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::foam"))
				m_Hydrax.isComponentsFoam = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::smooth"))
				m_Hydrax.isComponentsSmooth = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::sun"))
				m_Hydrax.isComponentsSun = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::underwater"))
				m_Hydrax.isComponentsUnderwater = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::underwater_godrays"))
				m_Hydrax.isComponentsGodrays = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("components::underwater_reflections"))
				m_Hydrax.isComponentsReflections = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));

			else if(xml->GetAttributeValue("id") == String("configfile"))
				m_Hydrax.ConfigFile = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("depth::limit"))
				m_Hydrax.DepthLimit = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));

			// Foam
			else if(xml->GetAttributeValue("id") == String("foam::max_distance"))
				m_Hydrax.FoamMaxDistance = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("foam::scale"))
				m_Hydrax.FoamScale = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("foam::start"))
				m_Hydrax.FoamStart = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("foam::transparency"))
				m_Hydrax.FoamTransparency = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			else if(xml->GetAttributeValue("id") == String("full_reflection_distance"))
				m_Hydrax.FullReflectionDistance = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("global_transparency"))
				m_Hydrax.GlobalTransparency = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			// Godrays
			else if(xml->GetAttributeValue("id") == String("godrays::exposure"))
				m_Hydrax.GodraysExposure = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("godrays::intensity"))
				m_Hydrax.GodraysIntensity = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("godrays::intersections"))
				m_Hydrax.GodraysIntersections = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("godrays::number_of_rays"))
				m_Hydrax.GodraysNumRays = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("godrays::rays_size"))
				m_Hydrax.GodraysSize = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("godrays::speed"))
				m_Hydrax.GodraysSpeed = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			else if(xml->GetAttributeValue("id") == String("layer"))
				m_Hydrax.Layers = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("module_name"))
				m_Hydrax.ModuleName = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("noise_module_name"))
				m_Hydrax.NoiseModuleName = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("normal_distortion"))
				m_Hydrax.NormalDistortion = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			// Perlin noise
			else if(xml->GetAttributeValue("id") == String("perlin::anim_speed"))
				m_Hydrax.PerlinAnimSpeed = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::falloff"))
				m_Hydrax.PerlinFalloff = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::gpu_lod"))
				m_Hydrax.PerlinGpuLod = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::gpu_strength"))
				m_Hydrax.PerlinGpuStrength = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::octaves"))
				m_Hydrax.PerlinOctaves = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::scale"))
				m_Hydrax.PerlinScale = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("perlin::time_multi"))
				m_Hydrax.PerlinTimeMulti = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			// PgModule
			else if(xml->GetAttributeValue("id") == String("pgmodule::choppy_strength"))
				m_Hydrax.PgmoduleChoppyStrength = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::choppy_waves"))
				m_Hydrax.isPgmoduleChoppyWaves = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::complexity"))
				m_Hydrax.PgmoduleComplexity = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::elevation"))
				m_Hydrax.PgmoduleElevation = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::force_recalculate_geometry"))
				m_Hydrax.isPgmoduleForceRecalculateGeometry = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::smooth"))
				m_Hydrax.isPgmoduleSmooth = StringConverter::parseBool(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("pgmodule::strength"))
				m_Hydrax.PgmoduleStrength = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));

			else if(xml->GetAttributeValue("id") == String("planes_error"))
				m_Hydrax.PlanesError = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("position"))
				m_Hydrax.Position = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));

			// RTT
			else if(xml->GetAttributeValue("id") == String("rtt_quality::depth"))
				m_Hydrax.RttQualityDepth = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("rtt_quality::depth_aip"))
				m_Hydrax.RttQualityDepthAip = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("rtt_quality::depth_reflection"))
				m_Hydrax.RttQualityDepthReflection = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("rtt_quality::gpu_normal_map"))
				m_Hydrax.RttQualityGpuNormalMap = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("rtt_quality::reflection"))
				m_Hydrax.RttQualityReflection = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("rtt_quality::refraction"))
				m_Hydrax.RttQualityRefraction = StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));

			// TODO: shader mode

			else if(xml->GetAttributeValue("id") == String("smooth::power"))
				m_Hydrax.SmoothPower = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			// Sun
			else if(xml->GetAttributeValue("id") == String("sun::area"))
				m_Hydrax.SunArea = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::colour"))
				m_Hydrax.SunColour = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::position"))
				m_Hydrax.SunPosition = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
			else if(xml->GetAttributeValue("id") == String("sun::strength"))
				m_Hydrax.SunStrength = StringConverter::parseReal(xml->GetAttributeValueSafe("value"));

			else if(xml->GetAttributeValue("id") == String("technique_add"))
				m_Hydrax.TechniqueAdd = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("technique_remove"))
				m_Hydrax.TechniqueRemove = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("updatescript"))
				m_Hydrax.UpdateScript = xml->GetAttributeValueSafe("value");
			else if(xml->GetAttributeValue("id") == String("watercolour"))
				m_Hydrax.WaterColour = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));

		}
		xml->Read();
	}
}

void COgSceneConverter::ParseMarker(IXMLReader* xml)
{
	/*  Not collecting: 
	 */

	reVector3Df pos;
	bool found = false;

	xml->Read();
	while(xml && !(String("OBJECT") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			if(xml->GetAttributeValue("id") == String("position"))
			{
				pos = StringConverter::parseVector3(xml->GetAttributeValueSafe("value"));
				found = true;
			}
		}
		else if(xml->GetNodeName() == String("CUSTOMPROPERTIES"))
		{
			if(found)
				ParseCustomProperty(xml, pos);
		}
		xml->Read();
	}
}

__declspec(deprecated("** This function is not being used anymore (COgSceneConverter::ParseCustomProperty) **"))
void COgSceneConverter::ParseCustomProperty(IXMLReader* xml, reVector3Df& pos)
{
	xml->Read();
	while(xml && !(String("CUSTOMPROPERTIES") == xml->GetNodeName()))
	{
		if(xml->GetNodeName() == String("PROPERTY") && !(xml->GetNodeType() == XML_NODE_ELEMENT_END))
		{
			// Create spawn point
			if(xml->GetAttributeValue("id") == String("SPAWN_POINT"))
			{
				SceneLoader::SPAWN_POINT sp;
				sp.Position = pos;
				sp.Team = (E_PLAYER_TEAM)StringConverter::parseUnsignedInt(xml->GetAttributeValueSafe("value"));
				m_SpawnPoint.push_back(sp);
			}
		}
		xml->Read();
	}
}

void COgSceneConverter::ParseTerrainName(IXMLReader* xml)
{
	// Get the page name first
	// Parse the page terrain name to get the X/Y min/max

	String Name;
	if(xml->GetAttributeValueSafe("name"))
		Name = xml->GetAttributeValueSafe("name");
	else
		return;
	
	CUtility::StringReplace(Name, "Page", "");

	Vector<String> values;
	CUtility::StringSplit(Name, 'x', values);

	if(values.size() == 2)
	{
		s32 tempY = StringConverter::parseInt(values[0]);
		s32 tempX = StringConverter::parseInt(values[1]);

		if(tempX > m_PagedTerrain.MaxX)
			m_PagedTerrain.MaxX = tempX;
		else if(tempX < m_PagedTerrain.MinX)
			m_PagedTerrain.MinX = tempX;

		if(tempY > m_PagedTerrain.MaxY)
			m_PagedTerrain.MaxY = tempY;
		else if(tempY < m_PagedTerrain.MinY)
			m_PagedTerrain.MinY = tempY;
	}

	xml->Read();
}

bool COgSceneConverter::Validate()
{
	// Make sure we have at lease one camera
	if(m_Cameras.size() == 0)
	{
		CAMERA camera;
		camera.Name = "Camera1";
		camera.ClipDistance = reVector2Df(0.1, 59994);
		camera.Fov = 0.785398;
		camera.Orientation = Quaternion(0, 0, 1, 0);
		camera.Position = reVector3Df(0, 0, 0);
		camera.LookAt = reVector3Df(0, 0, 0);
		camera.Type = ECM_FREE;
		m_Cameras.push_back(camera);
	}

	return true;
}

bool COgSceneConverter::SaveData()
{
	CFileSystem FileSystem;

	// remove the extension and replace it with custom extension
	std::string ext = FileSystem.GetFileExtension(m_FileName);
	m_FileName = m_FileName.replace(m_FileName.size() - ext.size(),
									ext.size(),
									CUSTOM_EXTENSION);

	if(!CreateFile(m_FileName.c_str()))
	{
		CORE_THROW("Could not create scene file", "COgSceneConverter::SaveData");
	}

	std::vector<SCENE_ELEMENT> elements;

#pragma region Scene Manager
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
	elements.push_back(SCENE_ELEMENT("SceneManagerType", m_SceneManager.Type));
	if(m_SceneManager.FogMode ==FOG_NONE)
		elements.push_back(SCENE_ELEMENT("FogMode", "None"));
	else if(m_SceneManager.FogMode ==FOG_EXP)
		elements.push_back(SCENE_ELEMENT("FogMode", "FOG_EXP"));
	else if(m_SceneManager.FogMode ==FOG_EXP2)
		elements.push_back(SCENE_ELEMENT("FogMode", "FOG_EXP2"));
	else
		elements.push_back(SCENE_ELEMENT("FogMode", "FOG_LINEAR"));
	AddElement("SceneManager", elements);

#pragma endregion

#pragma region Viewport and Cameras

	// add viewport
	elements.clear();
	elements.push_back(SCENE_ELEMENT("Color", StringConverter::toString(m_Viewport.Color)));
	elements.push_back(SCENE_ELEMENT("Compositor0Enabled", StringConverter::toString(m_Viewport.Compositor0Enabled)));
	elements.push_back(SCENE_ELEMENT("Compositor0Name", m_Viewport.Compositor0Name));
	elements.push_back(SCENE_ELEMENT("Name", m_Viewport.Name));
	elements.push_back(SCENE_ELEMENT("Parent", m_Viewport.Parent));
	AddElement("Viewport", elements);

	// add Cameras
	for(u32 i = 0; i < m_Cameras.size(); i++)
	{
		elements.clear();
		elements.push_back(SCENE_ELEMENT("Name", m_Cameras[i].Name));
		elements.push_back(SCENE_ELEMENT("ClipDistance", StringConverter::toString(m_Cameras[i].ClipDistance)));
		elements.push_back(SCENE_ELEMENT("Default", StringConverter::toString(m_Cameras[i].Default)));
		elements.push_back(SCENE_ELEMENT("FOV", StringConverter::toString(m_Cameras[i].Fov)));
		elements.push_back(SCENE_ELEMENT("LookAt", StringConverter::toString(m_Cameras[i].LookAt)));
		elements.push_back(SCENE_ELEMENT("Orientation", StringConverter::toString(m_Cameras[i].Orientation)));
		elements.push_back(SCENE_ELEMENT("Parent", m_Cameras[i].Parent));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_Cameras[i].Position)));
		AddElement("Camera", elements);
	}

#pragma endregion

#pragma region Game Objects and Lights

	// add enities
	for(u32 i = 0; i < m_GameObjects.size(); i++)
	{
		elements.clear();
		elements.push_back(SCENE_ELEMENT("Name", m_GameObjects[i].Name));
		elements.push_back(SCENE_ELEMENT("MeshFile", m_GameObjects[i].MeshFile));
		elements.push_back(SCENE_ELEMENT("Parent", m_GameObjects[i].Parent));
		elements.push_back(SCENE_ELEMENT("Orientation", StringConverter::toString(m_GameObjects[i].Orientation)));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_GameObjects[i].Position)));
		elements.push_back(SCENE_ELEMENT("Scale", StringConverter::toString(m_GameObjects[i].Scale)));
		elements.push_back(SCENE_ELEMENT("Shadows", StringConverter::toString(m_GameObjects[i].Shadows)));
		AddElement("GameObject", elements);
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
		AddElement("Light", elements);
	}
	elements.clear();

#pragma endregion

#pragma region Paged Terrain

	// add paged terrain
	if(m_PagedTerrain.Name.length() > 0)
	{
		elements.push_back(SCENE_ELEMENT("Name", m_PagedTerrain.Name));
		elements.push_back(SCENE_ELEMENT("ColorMapTextureSize", StringConverter::toString(m_PagedTerrain.ColorMapTextureSize)));
		elements.push_back(SCENE_ELEMENT("DetailDistance", StringConverter::toString(m_PagedTerrain.DetailDistance)));
		//elements.push_back(SCENE_ELEMENT("HeightMap", StringConverter::toString(m_PagedTerrain.HeightMap)));
		elements.push_back(SCENE_ELEMENT("isColorMap", StringConverter::toString(m_PagedTerrain.isColorMap)));
		elements.push_back(SCENE_ELEMENT("LightMapTextureSize", StringConverter::toString(m_PagedTerrain.LightMapTextureSize)));
		elements.push_back(SCENE_ELEMENT("MapSize", StringConverter::toString(m_PagedTerrain.MapSize)));
		elements.push_back(SCENE_ELEMENT("MaxBatchSize", StringConverter::toString(m_PagedTerrain.MaxBatchSize)));
		elements.push_back(SCENE_ELEMENT("MaxPixelError", StringConverter::toString(m_PagedTerrain.MaxPixelError)));
		elements.push_back(SCENE_ELEMENT("MinBatchSize", StringConverter::toString(m_PagedTerrain.MinBatchSize)));
		elements.push_back(SCENE_ELEMENT("PageDesityMapSize", StringConverter::toString(m_PagedTerrain.PageDesityMapSize)));
		elements.push_back(SCENE_ELEMENT("PageSize", StringConverter::toString(m_PagedTerrain.PageSize)));
		elements.push_back(SCENE_ELEMENT("MaxX", StringConverter::toString(m_PagedTerrain.MaxX)));
		elements.push_back(SCENE_ELEMENT("MinX", StringConverter::toString(m_PagedTerrain.MinX)));
		elements.push_back(SCENE_ELEMENT("MaxY", StringConverter::toString(m_PagedTerrain.MaxY)));
		elements.push_back(SCENE_ELEMENT("MinY", StringConverter::toString(m_PagedTerrain.MinY)));
		elements.push_back(SCENE_ELEMENT("TerrainFile", m_PagedTerrain.TerrainFile));
		elements.push_back(SCENE_ELEMENT("WorldSize", StringConverter::toString(m_PagedTerrain.WorldSize)));
		AddElement("PagedTerrain", elements);
		elements.clear();
	}

#pragma endregion

#pragma region Caelum
	// add caelum
	if(m_Caelum.Name.length() > 0)
	{
		elements.push_back(SCENE_ELEMENT("Name", m_Caelum.Name));
		elements.push_back(SCENE_ELEMENT("TimeDay", StringConverter::toString(m_Caelum.TimeDay)));
		elements.push_back(SCENE_ELEMENT("TimeHour", StringConverter::toString(m_Caelum.TimeHour)));
		elements.push_back(SCENE_ELEMENT("TimeMinute", StringConverter::toString(m_Caelum.TimeMinute)));
		elements.push_back(SCENE_ELEMENT("TimeMonth", StringConverter::toString(m_Caelum.TimeMonth)));
		elements.push_back(SCENE_ELEMENT("TimeSec", StringConverter::toString(m_Caelum.TimeSec)));
		elements.push_back(SCENE_ELEMENT("TimeSpeed", StringConverter::toString(m_Caelum.TimeSpeed)));
		elements.push_back(SCENE_ELEMENT("TimeYear", StringConverter::toString(m_Caelum.TimeYear)));
		elements.push_back(SCENE_ELEMENT("isClouds", StringConverter::toString(m_Caelum.isClouds)));
		elements.push_back(SCENE_ELEMENT("LayerOfClouds", StringConverter::toString(m_Caelum.LayerOfClouds)));
		elements.push_back(SCENE_ELEMENT("FogDenMultiplier", StringConverter::toString(m_Caelum.FogDenMultiplier)));
		elements.push_back(SCENE_ELEMENT("isManageFog", StringConverter::toString(m_Caelum.isManageFog)));
		elements.push_back(SCENE_ELEMENT("Layer", StringConverter::toString(m_Caelum.Layer)));
		elements.push_back(SCENE_ELEMENT("isSingleLightSource", StringConverter::toString(m_Caelum.isSingleLightSource)));
		elements.push_back(SCENE_ELEMENT("isSingleShadowSource", StringConverter::toString(m_Caelum.isSingleShadowSource)));
		elements.push_back(SCENE_ELEMENT("isManageAmbientLight", StringConverter::toString(m_Caelum.isManageAmbientLight)));
		elements.push_back(SCENE_ELEMENT("MinAmbientLight", StringConverter::toString(m_Caelum.MinAmbientLight)));
		elements.push_back(SCENE_ELEMENT("MoonAmbientMultipler", StringConverter::toString(m_Caelum.MoonAmbientMultipler)));
		elements.push_back(SCENE_ELEMENT("MoonAttMultipler", StringConverter::toString(m_Caelum.MoonAttMultipler)));
		elements.push_back(SCENE_ELEMENT("MoonAttDistance", StringConverter::toString(m_Caelum.MoonAttDistance)));
		elements.push_back(SCENE_ELEMENT("MoonAttLinearMultipler", StringConverter::toString(m_Caelum.MoonAttLinearMultipler)));
		elements.push_back(SCENE_ELEMENT("MoonQuadMultipler", StringConverter::toString(m_Caelum.MoonQuadMultipler)));
		elements.push_back(SCENE_ELEMENT("isMoonAutoDisable", StringConverter::toString(m_Caelum.isMoonAutoDisable)));
		elements.push_back(SCENE_ELEMENT("isMoonCastShadow", StringConverter::toString(m_Caelum.isMoonCastShadow)));
		elements.push_back(SCENE_ELEMENT("MoonDiffuseMultipler", StringConverter::toString(m_Caelum.MoonDiffuseMultipler)));
		elements.push_back(SCENE_ELEMENT("isMoonEnabled", StringConverter::toString(m_Caelum.isMoonEnabled)));
		elements.push_back(SCENE_ELEMENT("MoonSpecularMultipler", StringConverter::toString(m_Caelum.MoonSpecularMultipler)));
		elements.push_back(SCENE_ELEMENT("ObserverLatitude", StringConverter::toString(m_Caelum.ObserverLatitude)));
		elements.push_back(SCENE_ELEMENT("ObserverLongitude", StringConverter::toString(m_Caelum.ObserverLongitude)));
		elements.push_back(SCENE_ELEMENT("isStarsEnabled", StringConverter::toString(m_Caelum.isStarsEnabled)));
		elements.push_back(SCENE_ELEMENT("StarsMag0PixelSize", StringConverter::toString(m_Caelum.StarsMag0PixelSize)));
		elements.push_back(SCENE_ELEMENT("StarsMagScale", StringConverter::toString(m_Caelum.StarsMagScale)));
		elements.push_back(SCENE_ELEMENT("StarsMaxPixelSize", StringConverter::toString(m_Caelum.StarsMaxPixelSize)));
		elements.push_back(SCENE_ELEMENT("StarsMinPixelSize", StringConverter::toString(m_Caelum.StarsMinPixelSize)));
		elements.push_back(SCENE_ELEMENT("SunAmbientMultipler", StringConverter::toString(m_Caelum.SunAmbientMultipler)));
		elements.push_back(SCENE_ELEMENT("SunAttMultipler", StringConverter::toString(m_Caelum.SunAttMultipler)));
		elements.push_back(SCENE_ELEMENT("SunDistance", StringConverter::toString(m_Caelum.SunDistance)));
		elements.push_back(SCENE_ELEMENT("SunAttLinearMultipler", StringConverter::toString(m_Caelum.SunAttLinearMultipler)));
		elements.push_back(SCENE_ELEMENT("SunQuadMultipler", StringConverter::toString(m_Caelum.SunQuadMultipler)));
		elements.push_back(SCENE_ELEMENT("isSunAutoDisable", StringConverter::toString(m_Caelum.isSunAutoDisable)));
		elements.push_back(SCENE_ELEMENT("isSunCastShadow", StringConverter::toString(m_Caelum.isSunCastShadow)));
		elements.push_back(SCENE_ELEMENT("SunColor", StringConverter::toString(m_Caelum.SunColor)));
		elements.push_back(SCENE_ELEMENT("SunDiffuseMultipler", StringConverter::toString(m_Caelum.SunDiffuseMultipler)));
		elements.push_back(SCENE_ELEMENT("isSunEnabled", StringConverter::toString(m_Caelum.isSunEnabled)));
		elements.push_back(SCENE_ELEMENT("SunLightColor", StringConverter::toString(m_Caelum.SunLightColor)));
		elements.push_back(SCENE_ELEMENT("SunPosition", StringConverter::toString(m_Caelum.SunPosition)));
		elements.push_back(SCENE_ELEMENT("SunSpecularMultipler", StringConverter::toString(m_Caelum.SunSpecularMultipler)));
		AddElement("Caelum", elements);
		elements.clear();
	}
#pragma endregion

#pragma region hydrax
	// hydrax
	if(m_Hydrax.Name.length() > 0)
	{
		elements.push_back(SCENE_ELEMENT("Name", m_Hydrax.Name));
		elements.push_back(SCENE_ELEMENT("CausticsEnd", StringConverter::toString(m_Hydrax.CausticsEnd)));
		elements.push_back(SCENE_ELEMENT("CausticsPower", StringConverter::toString(m_Hydrax.CausticsPower)));
		elements.push_back(SCENE_ELEMENT("CausticsScale", StringConverter::toString(m_Hydrax.CausticsScale)));
		elements.push_back(SCENE_ELEMENT("ConfigFile", m_Hydrax.ConfigFile));
		elements.push_back(SCENE_ELEMENT("DepthLimit", StringConverter::toString(m_Hydrax.DepthLimit)));
		elements.push_back(SCENE_ELEMENT("FoamMaxDistance", StringConverter::toString(m_Hydrax.FoamMaxDistance)));
		elements.push_back(SCENE_ELEMENT("FoamScale", StringConverter::toString(m_Hydrax.FoamScale)));
		elements.push_back(SCENE_ELEMENT("FoamStart", StringConverter::toString(m_Hydrax.FoamStart)));
		elements.push_back(SCENE_ELEMENT("FoamTransparency", StringConverter::toString(m_Hydrax.FoamTransparency)));
		elements.push_back(SCENE_ELEMENT("FullReflectionDistance", StringConverter::toString(m_Hydrax.FullReflectionDistance)));
		elements.push_back(SCENE_ELEMENT("GlobalTransparency", StringConverter::toString(m_Hydrax.GlobalTransparency)));
		elements.push_back(SCENE_ELEMENT("GodraysExposure", StringConverter::toString(m_Hydrax.GodraysExposure)));
		elements.push_back(SCENE_ELEMENT("GodraysIntensity", StringConverter::toString(m_Hydrax.GodraysIntensity)));
		elements.push_back(SCENE_ELEMENT("GodraysIntersections", StringConverter::toString(m_Hydrax.GodraysIntersections)));
		elements.push_back(SCENE_ELEMENT("GodraysNumRays", StringConverter::toString(m_Hydrax.GodraysNumRays)));
		elements.push_back(SCENE_ELEMENT("GodraysSize", StringConverter::toString(m_Hydrax.GodraysSize)));
		elements.push_back(SCENE_ELEMENT("GodraysSpeed", StringConverter::toString(m_Hydrax.GodraysSpeed)));
		elements.push_back(SCENE_ELEMENT("isCaelumItegrated", StringConverter::toString(m_Hydrax.isCaelumItegrated)));
		elements.push_back(SCENE_ELEMENT("isComponentsCaustics", StringConverter::toString(m_Hydrax.isComponentsCaustics)));
		elements.push_back(SCENE_ELEMENT("isComponentsDepth", StringConverter::toString(m_Hydrax.isComponentsDepth)));
		elements.push_back(SCENE_ELEMENT("isComponentsFoam", StringConverter::toString(m_Hydrax.isComponentsFoam)));
		elements.push_back(SCENE_ELEMENT("isComponentsGodrays", StringConverter::toString(m_Hydrax.isComponentsGodrays)));
		elements.push_back(SCENE_ELEMENT("isComponentsReflections", StringConverter::toString(m_Hydrax.isComponentsReflections)));
		elements.push_back(SCENE_ELEMENT("isComponentsSmooth", StringConverter::toString(m_Hydrax.isComponentsSmooth)));
		elements.push_back(SCENE_ELEMENT("isComponentsSun", StringConverter::toString(m_Hydrax.isComponentsSun)));
		elements.push_back(SCENE_ELEMENT("isComponentsUnderwater", StringConverter::toString(m_Hydrax.isComponentsUnderwater)));
		elements.push_back(SCENE_ELEMENT("isPgmoduleChoppyWaves", StringConverter::toString(m_Hydrax.isPgmoduleChoppyWaves)));
		elements.push_back(SCENE_ELEMENT("isPgmoduleForceRecalculateGeometry", StringConverter::toString(m_Hydrax.isPgmoduleForceRecalculateGeometry)));
		elements.push_back(SCENE_ELEMENT("isPgmoduleSmooth", StringConverter::toString(m_Hydrax.isPgmoduleSmooth)));
		elements.push_back(SCENE_ELEMENT("Layers", StringConverter::toString(m_Hydrax.Layers)));
		elements.push_back(SCENE_ELEMENT("ModuleName", m_Hydrax.ModuleName));
		elements.push_back(SCENE_ELEMENT("NoiseModuleName", m_Hydrax.NoiseModuleName));
		elements.push_back(SCENE_ELEMENT("NormalDistortion", StringConverter::toString(m_Hydrax.NormalDistortion)));
		elements.push_back(SCENE_ELEMENT("PerlinAnimSpeed", StringConverter::toString(m_Hydrax.PerlinAnimSpeed)));
		elements.push_back(SCENE_ELEMENT("PerlinFalloff", StringConverter::toString(m_Hydrax.PerlinFalloff)));
		elements.push_back(SCENE_ELEMENT("PerlinGpuLod", StringConverter::toString(m_Hydrax.PerlinGpuLod)));
		elements.push_back(SCENE_ELEMENT("PerlinGpuStrength", StringConverter::toString(m_Hydrax.PerlinGpuStrength)));
		elements.push_back(SCENE_ELEMENT("PerlinOctaves", StringConverter::toString(m_Hydrax.PerlinOctaves)));
		elements.push_back(SCENE_ELEMENT("PerlinScale", StringConverter::toString(m_Hydrax.PerlinScale)));
		elements.push_back(SCENE_ELEMENT("PerlinTimeMulti", StringConverter::toString(m_Hydrax.PerlinTimeMulti)));
		elements.push_back(SCENE_ELEMENT("PgmoduleChoppyStrength", StringConverter::toString(m_Hydrax.PgmoduleChoppyStrength)));
		elements.push_back(SCENE_ELEMENT("PgmoduleComplexity", StringConverter::toString(m_Hydrax.PgmoduleComplexity)));
		elements.push_back(SCENE_ELEMENT("PgmoduleElevation", StringConverter::toString(m_Hydrax.PgmoduleElevation)));
		elements.push_back(SCENE_ELEMENT("PgmoduleStrength", StringConverter::toString(m_Hydrax.PgmoduleStrength)));
		elements.push_back(SCENE_ELEMENT("PlanesError", StringConverter::toString(m_Hydrax.PlanesError)));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_Hydrax.Position)));
		elements.push_back(SCENE_ELEMENT("RttQualityDepth", StringConverter::toString(m_Hydrax.RttQualityDepth)));
		elements.push_back(SCENE_ELEMENT("RttQualityDepthAip", StringConverter::toString(m_Hydrax.RttQualityDepthAip)));
		elements.push_back(SCENE_ELEMENT("RttQualityDepthReflection", StringConverter::toString(m_Hydrax.RttQualityDepthReflection)));
		elements.push_back(SCENE_ELEMENT("RttQualityGpuNormalMap", StringConverter::toString(m_Hydrax.RttQualityGpuNormalMap)));
		elements.push_back(SCENE_ELEMENT("RttQualityReflection", StringConverter::toString(m_Hydrax.RttQualityReflection)));
		elements.push_back(SCENE_ELEMENT("RttQualityRefraction", StringConverter::toString(m_Hydrax.RttQualityRefraction)));
		elements.push_back(SCENE_ELEMENT("ShaderMode", StringConverter::toString(m_Hydrax.ShaderMode)));
		elements.push_back(SCENE_ELEMENT("SmoothPower", StringConverter::toString(m_Hydrax.SmoothPower)));
		elements.push_back(SCENE_ELEMENT("SunArea", StringConverter::toString(m_Hydrax.SunArea)));
		elements.push_back(SCENE_ELEMENT("SunColour", StringConverter::toString(m_Hydrax.SunColour)));
		elements.push_back(SCENE_ELEMENT("SunPosition", StringConverter::toString(m_Hydrax.SunPosition)));
		elements.push_back(SCENE_ELEMENT("SunStrength", StringConverter::toString(m_Hydrax.SunStrength)));
		elements.push_back(SCENE_ELEMENT("TechniqueAdd", m_Hydrax.TechniqueAdd));
		elements.push_back(SCENE_ELEMENT("TechniqueRemove", m_Hydrax.TechniqueRemove));
		elements.push_back(SCENE_ELEMENT("UpdateScript", m_Hydrax.UpdateScript));
		elements.push_back(SCENE_ELEMENT("WaterColour", StringConverter::toString(m_Hydrax.WaterColour)));
		AddElement("Hydrax", elements);
		elements.clear();
	}

#pragma endregion

#pragma region Spawn Points

	for(u32 i = 0; i < m_SpawnPoint.size(); i++)
	{
		elements.push_back(SCENE_ELEMENT("Team", StringConverter::toString(m_SpawnPoint[i].Team)));
		elements.push_back(SCENE_ELEMENT("Position", StringConverter::toString(m_SpawnPoint[i].Position)));
		AddElement("SpawnPoint", elements);
	}
	elements.clear();

#pragma endregion

	return true;
}