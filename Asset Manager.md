
== Asset Manager: ==

=== Purpose: ===
This is an overview and manual on what the Asset Manager tools can do, and how to use them.

=== Asset Manager 2.0: ===
This is a DB driven tool where all the assets and the dependencies are stored in a Sql Server and tracked throughout the cycle of the application.

**Asset Overview:**
* Organizes the assets by type, and list the dependencies.
* Shows most types of images.
* Shows materials when double clicked, and modify them.
* Scale, and rotate meshes.
* Shows the mesh if "Meshy" is installed.
* Optimizes and upgrades the mesh if selected.
* Allows you to assign a new material to the mesh.
* Allows you to assign a new mesh type to the mesh.
** By default a new mesh is imported and assigned as "Game Object".

**Categories/Asset Types:**
* Add different categories
* Modify the asset types the manager will work with.

**Add Assets:**
* Add a single asset, or a bulk load of assets in a root directory.
* If the assign being added has a dependency that is not in the database, the addition will fail and report an error.

**Profiles:**
* Add or modify and existing profile.
* Creates a profile with a given Core config file. (does not add terrain files)

**Archives:**
* Given a profile, it will compress and output all the assets in that profile.
* With "Script File Options", this allows you to define what the export will do with the numerous script files native to Ogre.
** '''Leave Files:''' Will not modify the files at all.
* **Merge Scripts:** Will merge all material files into one material file, same with overlays, and particles.
* **2000 Line Increments:** Will merge into approximately into 2000 line files (not exact).  This is because Ogre has problems reading material files that were larger then about 3500 lines in 1.7 versions.

(((Note: The preferred option now is to output materials and fonts as C++ code, so if you do that, these options don't matter.)))

**Generated Code:**
* Given a profile, will generate boilerplate plugin code for every mesh file in the asset directory.  
* Optional unit test code is generated for the boilerplate code.
* Able to add inventory items; tools, weapons, magazines and projectiles.
* **Output** button will generate all the xml for the inventory created.
* Option to output fonts, and materials into C++ code.
* If **Enable Scripting** is checked, LUA entry points will be added for scripting support.
* '''Scene File Conversion''' will convert an Ogre scene file to C++ code usable for Core.
* Ability to add inventory items into the Core XML output.
* The scripting button will merge all header files and use the **Swig** tool (if locally installed) to create a C-Sharp wrapper for the entire engine.
* Used to generate script bindings.  Get a directory listing of the include directories you want to use for binding.  Then generate the packaging files.

**GUI Generator:**
* Reads a **MyGUI** XML layout, and converts it to C++ code usable in Core.

**Level Converter:**
* Has the ability to convert an **Ogitor** scene into the '''Core XML''' format.  (This has not been tested with Ogitor 0.5+)

**Exporting Image Formats:**
The AM supports the ability to export images to standard formats, the default should be DDS in general.  If a format is selected, the AM will ensure that all materials that are created either in C++ generated code, or outputted material files will reference the new file name.  It's still important that the DB keeps a cope of the original file in its original format for backup purposes, so the images will only be converted when an archive is created.

**Converting Meshes:**
Thanks to Assimp and the Ogre extension, the AM will convert a number of formats to the Ogre mesh format.  This can be done either on a single mesh, or an entire directory.  Right now, this should only be used on models that do not have animations because the Ogre extension does not support animations very well.

**External Tools:**
The AM makes extensive use of other tools (executable) and are copied to the build directory when compiled.  These are included in the project and can be found in the '''Tools''' directory.
* OgreMeshMagic
* OgreXmlConverter
* OgreUpgrader
* Assimp, with the Ogre extension

=== Asset Manager 1.0: ===
The first version of the AM is all file based, meaning all the assets are in the HDD all in a root directory.  There is some dependency checks, but it is very limited because files could be moved, or changed somehow.

**Copy Assets:**
Copy Assets:
This tab will copy all assets given a root directory, and will output it all into the output directory.  Using the Merge Materials checkbox will copy all the materials into one file.  This is useful when having to manage hundreds of lines of materials, and finding multiple material names.

**Material Check:**
Used to run validation rules on a material file.  The only validation rule coded now is a check for multiple names.

**Archive:**
Used to generate config XML for specifying directory and zip archives.

**Profiles:**
Used to generate XML for inventory profiles in scene files (scx).

**Code Generator:**
Game Object Plugin, given an asset directory, will generate boilerplate plugin code for every mesh file in the asset directory.  Compress button will take all the files in the asset directory and compress it in into a signle zip file.  This button is ment to be used once all the game oject code is generated, because the code will fail unless the asset can be foud so you might as well add it to the archive.

**Scripting:**
Used to generate script bindings.  Get a directory listing fo the include directories you want to use for binding.  Then generate the packaging files.

=== Notes: ===
==== TODO List: ====
# Finish writing the image conversion feature.  This allows the user to select what format all images should be exported as.  Most of this is done except materials, and particles need to be modified to use that new extension.
* Material editor
* Asset edit history

==== Excluded Files for Scripting: ====
```
lstToLuaScriptingIgnore.Items.Add("CBaseLayout_MyGui.h");			// Source in header
            lstToLuaScriptingIgnore.Items.Add("CBillboardManager.h");			// Sub classes
            lstToLuaScriptingIgnore.Items.Add("CTextOutput.h");					// Fix
            lstToLuaScriptingIgnore.Items.Add("OgreConsole.h");					// Fix
            lstToLuaScriptingIgnore.Items.Add("CScriptManager.h");				// Fix
            lstToLuaScriptingIgnore.Items.Add("OldPathway.h");					// Not needed
            lstToLuaScriptingIgnore.Items.Add("SoundEnginePrereqs.h");			// Not needed
            lstToLuaScriptingIgnore.Items.Add("SharedPointer.h");				// Not needed
            lstToLuaScriptingIgnore.Items.Add("CFpsFrameListener.h");			// Fix
            lstToLuaScriptingIgnore.Items.Add("CFrameListenerFramework.h");		// Fix
            lstToLuaScriptingIgnore.Items.Add("CPlayerAttributesComponent.h");	// Fix
            lstToLuaScriptingIgnore.Items.Add("IPhysicsStrategy.h");				// Not needed
            lstToLuaScriptingIgnore.Items.Add("ICharacterController.h");			// Not needed
            lstToLuaScriptingIgnore.Items.Add("IPhysicsRagdoll.h");				// Not needed
            lstToLuaScriptingIgnore.Items.Add("IItemProfile.h");					// Not needed
            lstToLuaScriptingIgnore.Items.Add("IItemFactory.h");					// Not needed
            lstToLuaScriptingIgnore.Items.Add("IInventoryItem.h");				// Not needed
            lstToLuaScriptingIgnore.Items.Add("IGuiStrategy.h");					// Not needed
            lstToLuaScriptingIgnore.Items.Add("IMission.h");						// Not needed
            lstToLuaScriptingIgnore.Items.Add("IAiReactionComponent.h");			// Not needed
            lstToLuaScriptingIgnore.Items.Add("ICamera.h");						// Not needed
            lstToLuaScriptingIgnore.Items.Add("Defines.h");						// Not needed
            lstToLuaScriptingIgnore.Items.Add("CUtility.h");						// Not needed
            lstToLuaScriptingIgnore.Items.Add("CRagdollBoneInformation.h");		// Not needed
            lstToLuaScriptingIgnore.Items.Add("QueryStrings.h");					// Not needed
```
