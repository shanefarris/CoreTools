/* ===========================================================================
Artifex Loader v0.90 RC2 beta

Freeware

Copyright (c) 2008 MouseVolcano (Thomas Gradl, Karolina Sefyrin,  Erik Biermann)

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
=========================================================================== */
#pragma once
#ifndef ARTIFEXLOADER_H
#define ARTIFEXLOADER_H

#pragma warning (disable : 4530)

#include <cstdlib>
#include <iostream>

// include Ogre
#include "Ogre.h"

// include for the masks and Spawn struct, needed for ArtifexLoader
#include "CustomTypes.h"

// utility include, a good collection of small and handy timesavers:
#include "StaticUtils.h"

// include the Database Manager for SQLite support
#include "DBManager.h"
class DBManager;

#define ETM_GROUP "ETM" // the resourcegroup for the zone

using namespace std;
using namespace Ogre;
using namespace Artifex;


class ArtifexLoader {
public:
	ArtifexLoader(Root *root, SceneManager *scenemgr, SceneNode *camnode, Camera *camera, string zonepath="");
	~ArtifexLoader();

	bool isZoneLoaded();
	bool loadZone(std::string zonename, bool use_cfg_settings=true, bool fog=true, bool water=true, 
				bool skye=true, bool light=true, bool statics=true, bool groundcover=true, bool movables=true, 
				bool mobiles=true);
	void unloadZone();
	float getMeshYAdjust(Ogre::String meshfile);
	float getHeightAt(const float &x, const float &z);

	SceneNode *mCamNode;
	SceneManager *mSceneMgr;
	std::vector <Spawn2> mObjectFile;
	std::vector <Spawn2> mObjectTrashCan;

private:
	Root *mRoot;
	bool mZoneLoaded;
	Image mCoverage[3];
	TexturePtr mCoverageTex[3];
	
	// TSM
	void initTSM();
	void loadTerrain();

	// SQLite
	DBManager *mDBManager;

	string mZoneVersion;
	string mLoadObjectsFrom;

	//grass
	string mGrassCoverMap;
	string mGrassCoverChannel;
	string mGrassColourMap;
	string mGrassMaterial;
	bool mGrassAnimate;
	float mGrassDensity;
	float mGrassRange;

	// fog
	float mFogR;
	float mFogG;
	float mFogB;
	float mFogDensity;
	float mFogStart;
	float mFogEnd;
	bool mFogVisible;

	// skye
	string mSkyBoxName;
	float mSkyRadius;

	// camera
	Camera *mCamera;
	float mFarClipDistance;
	float mCamX;
	float mCamY;
	float mCamZ;

	//light
	Light *mLight;
	float mLightPosX;
	float mLightPosY;
	float mLightPosZ;
	float mAmbientR;
	float mAmbientG;
	float mAmbientB;

	// water
	int mWaterVisible;
	float mWaterXPos;
	float mWaterYPos;
	float mWaterZPos;
	float mWaterWidth;
	float mWaterLength;
	string mWaterMaterial;
	SceneNode* mWaterNode;
	Entity *mWaterEntity;

	// terrain
	bool mTerrainLoD;

	float mTerrainPixelError;
	float mTerrainLoDMorphingFactor;

	int mTerrainMaxLoD;

	int mTerrainTileSize;
	int mTerrainVertexCount;

	bool mTerrainVertexNormals;
	bool mTerrainVertexTangents;

	// *** terrainDimensions
	float mTerrX, mTerrZ, mTerrY;

	float mSplattScaleX;
	float mSplattScaleZ;
	int mTextureCount;

	Image* mOverlay;
	TexturePtr mOverlayTex;

	Image* mSplatting[9];
	String mTexturePath[9];
	TexturePtr mSplattingTex[9];

	int mTextureSize;
	int mCoverMapSize;

	void setupTextures();

	void loadTerrainSettings();

	void createColourLayer();

	void loadCameraSettings();

	void loadWaterSettings();
	void createWater();

	void loadSkyeSettings();
	void createSkye();

	void loadFogSettings();
	void createFog();

	void loadGrassSettings();

	void loadLightSettings();
	void createLight();

	string mZonePath;
	string mZoneName;

	bool mLightmap;
	float mLightmapShadow;

	bool mLoadGrass;

	void spawnLoader(String which);
	void staticsLoader(String which, float renderDistance);

	void loadZoneCFG();
};
#endif
