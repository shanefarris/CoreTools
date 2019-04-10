#ifndef SAMPLEAPP_H
#define SAMPLEAPP_H

// include for the masks and Spawn struct, needed for ArtifexLoader
#include "CustomTypes.h"

// utility include, a good collection of small and handy timesavers:
#include "StaticUtils.h"

// include Ogre and the InputManager for OIS
#include "Ogre.h"
#include "InputManager.h"

// include the zoneloader
#include "ArtifexLoader.h"
class ArtifexLoader;
//

class SampleApp : public OIS::MouseListener, public OIS::KeyListener, public Ogre::WindowEventListener
{
public:
	float mTimer1;

	// *** ArtifexLoader Setup ***
	ArtifexLoader* mArtifexLoader;
	// *********************

	// *** Basic Ogre Setup ***
	Ogre::Root* mRoot;
	Ogre::RenderWindow* mRenderWin;
	Ogre::SceneManager* mSceneMgr;

	InputManager* mInputMgr;
	Ogre::SceneNode* mCamNode;
	Ogre::Camera* mCamera;

	unsigned long mLastTime;
	Ogre::Timer mTimer;

	void parseResources();

	void setupEmptyScene();

	void loadInputSystem();

	bool mShouldQuit;

	bool mLMouseDown, mMMouseDown, mRMouseDown;

	float mRotateSpeed, mMoveSpeed, mRotation, mPitch;

	Ogre::Vector3 mDirection;

	SampleApp();
	~SampleApp();

	void Startup();
	void Update();
	void Shutdown();

	bool mouseMoved(const OIS::MouseEvent &arg);
	bool mousePressed(const OIS::MouseEvent &arg, OIS::MouseButtonID id);
	bool mouseReleased(const OIS::MouseEvent &arg, OIS::MouseButtonID id);

	bool keyPressed( const OIS::KeyEvent &arg );
	bool keyReleased( const OIS::KeyEvent &arg );

	void windowMoved(Ogre::RenderWindow* rw);
	void windowResized(Ogre::RenderWindow* rw);
	void windowClosed(Ogre::RenderWindow* rw);
	void windowFocusChange(Ogre::RenderWindow* rw);
	// *** End Basic Ogre Setup *** //

};
#endif

