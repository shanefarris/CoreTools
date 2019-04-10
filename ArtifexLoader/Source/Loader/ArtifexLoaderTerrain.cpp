/* ===========================================================================
Artifex Loader v0.90 beta

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
#include "ArtifexLoader.h"

using namespace std;
using namespace Ogre;
using namespace Artifex;

void ArtifexLoader::loadTerrainSettings() {
	Ogre::ConfigFile cfgfile;		
	try {
		cfgfile.load(mZonePath+mZoneName+"/zonesettings.cfg");
		
		mTerrX = strToF(cfgfile.getSetting("TerrainWidth_X"));
		mTerrZ = strToF(cfgfile.getSetting("TerrainWidth_Z"));
		mTerrY = strToF(cfgfile.getSetting("TerrainHeight_Y"));
		
		if (mTerrX == 0) mTerrX = 5000;
		if (mTerrZ == 0) mTerrZ = 5000;
		if (mTerrY == 0) mTerrY = 1667;
		
		mTerrainLoD = strToI(cfgfile.getSetting("TerrainLoD"))==1?true:false;;
		if (cfgfile.getSetting("TerrainLoD")=="") mTerrainLoD = true;
		
		mTerrainPixelError = strToF(cfgfile.getSetting("TerrainPixelError"));
		if (mTerrainPixelError == 0) mTerrainPixelError = 2;
		
		mTerrainLoDMorphingFactor = strToF(cfgfile.getSetting("TerrainLoDMorphingFactor"));;
		if (mTerrainLoDMorphingFactor == 0) mTerrainLoDMorphingFactor = 0.2f;
	
		mTerrainMaxLoD = strToI(cfgfile.getSetting("TerrainMaxLoD"));;
		if (mTerrainMaxLoD == 0) mTerrainMaxLoD = 255;
	
		mTerrainTileSize = strToI(cfgfile.getSetting("TerrainTileSize"));;
		if (mTerrainTileSize == 0) mTerrainTileSize = 33;
		
		mTerrainVertexCount = strToI(cfgfile.getSetting("TerrainEdgeVertexCount"));;
		if (mTerrainVertexCount==0) mTerrainVertexCount = 513;
	
		mTerrainVertexNormals = strToI(cfgfile.getSetting("TerrainVertexNormals"))==1?true:false;;
		if (cfgfile.getSetting("TerrainVertexNormals") == "") mTerrainVertexNormals = true;
		
		mTerrainVertexTangents = strToI(cfgfile.getSetting("TerrainVertexTangents"))==1?true:false;;
		if (cfgfile.getSetting("TerrainVertexTangents") == "") mTerrainVertexTangents = true;
		
		mSplattScaleX = strToF(cfgfile.getSetting("TerrainSplattScale_X"));	
		mSplattScaleZ = strToF(cfgfile.getSetting("TerrainSplattScale_Z"));	
		
		if (mSplattScaleX == 0) mSplattScaleX = 192;
		if (mSplattScaleZ == 0) mSplattScaleZ = 192;
		
		for (int i=0; i<9; i++) {
			mTexturePath[i] = cfgfile.getSetting("Splatting"+StringConverter::toString(i));		
		}		
		
	} catch (Exception &e) {
		cout << "Error loading Terrain Settings: " << e.what() << ".\n";

	}
};

void ArtifexLoader::setupTextures() {
	String num = "";
	mTextureSize = 1024;
	
	for (int a=0;a<9;a++) {
		num = StringConverter::toString(a);
		mSplatting[a] = new Image();
		// mZonePath+mZoneName+"/"
		mSplatting[a]->load(mTexturePath[a],ETM_GROUP);

		// create a manual texture	  
		mSplattingTex[a] = TextureManager::getSingleton().createManual(
		"Splatting"+num, ETM_GROUP, Ogre::TEX_TYPE_2D, mTextureSize, mTextureSize, 1, mSplatting[a]->getFormat());	
		// load the imagefile 
		mSplattingTex[a]->loadImage(*mSplatting[a]);
	}
};

void ArtifexLoader::createColourLayer() 
{
	mOverlay = new Image();	
	try {	
		if (mZoneName!="") {
			//mZonePath+mZoneName+"/
			mOverlay->load("artifexterra3d_colourmap.png",ETM_GROUP);	  
		}

		mOverlayTex = TextureManager::getSingleton().createManual(
		"ArtifexTerra3D_ColourMap", ETM_GROUP, Ogre::TEX_TYPE_2D, 2048, 2048, 1, mOverlay->getFormat());	

		mOverlayTex->unload();
		mOverlayTex->loadImage(*mOverlay); 	
	  
	}catch(Exception &e) { 
		std::cout <<"Troubles loading colourlayer, DON'T PANIC, rolling my own.\n"; 	
		
		uchar *pImage = new uchar[2048 * 2048 * 4];
        mOverlay->loadDynamicImage(pImage, 2048, 2048, PF_A8R8G8B8);         
		
		mOverlayTex = TextureManager::getSingleton().createManual(
		"ArtifexTerra3D_ColourMap", ETM_GROUP, Ogre::TEX_TYPE_2D, 2048, 2048, 1, mOverlay->getFormat());	

		mOverlayTex->unload();
		mOverlayTex->loadImage(*mOverlay); 	
	}	
};

// TSM section
void ArtifexLoader::initTSM() {
	for (int a=0;a<3;a++) {
	
		cout << "Creating ETSplatting" << a << "\n";
	
		mCoverage[a].load("ETcoverage."+iToStr(a)+".png",ETM_GROUP);
	
		mCoverageTex[a] = TextureManager::getSingleton().createManual(
			"ETSplatting"+iToStr(a), ETM_GROUP, Ogre::TEX_TYPE_2D, 
			mCoverage[a].getWidth(), mCoverage[a].getHeight(), 1, mCoverage[a].getFormat()
		);
		
		mCoverageTex[a]->unload();
		mCoverageTex[a]->loadImage(mCoverage[a]);
	}	
};
void ArtifexLoader::loadTerrain() {
	try {
		mSceneMgr->setWorldGeometry("terrain.cfg");
	} catch (exception &e) {
		cout << e.what() << "\n";
	}
};
