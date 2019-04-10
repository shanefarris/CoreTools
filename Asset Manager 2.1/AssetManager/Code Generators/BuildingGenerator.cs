using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using AssetManager.Common.Enums;
using AssetManager.Dal;


namespace AssetManager.Code_Generators
{
    public class BuildingGenerator
    {
        public void CreateHeader(string fileName, string outputDir)
        {
            var className = GetClassName(fileName);
            var streamWriter = new StreamWriter(outputDir + "\\" + className + ".h");
            streamWriter.WriteLine("#ifndef __" + className.ToUpper() + "_H__");
            streamWriter.WriteLine("#define __" + className.ToUpper() + "_H__");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"Defines.h\"");
            streamWriter.WriteLine("#include \"CBuilding.h\"");
            streamWriter.WriteLine("#include \"Plugins/IBuildingFactory.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace Core");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("namespace Plugin");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tclass " + className + " : public CBuilding");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\tpublic:");
            streamWriter.WriteLine("\t\t" + className + "(const char* Name, Physics::IPhysicsStrategy* PhysicsStrategy);");
            streamWriter.WriteLine("\t\t~" + className + "();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\tvoid Update(const f32& elapsedTime);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tprivate:");
            streamWriter.WriteLine("\t\tvoid SetupMapping();");
            streamWriter.WriteLine("\t};");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tclass " + className + "Factory : public IBuildingFactory");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\tpublic:");
            streamWriter.WriteLine("\t\t" + className + "Factory();");
            streamWriter.WriteLine("\t\t~" + className + "Factory();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\tCBuilding* CreateBuilding(const char* Name, Physics::IPhysicsStrategy* PhysicsStrategy);");
            streamWriter.WriteLine("\t};");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine("#endif // __" + className.ToUpper() + "_H__");
            streamWriter.Close();
        }

        public void CreateSource(string fileName, string outputDir, string materialName = null)
        {
            var typeName = fileName.Replace(".mesh", string.Empty).Replace(" ", "");
            var className = GetClassName(fileName);
            typeName = typeName.ToUpper();
            var streamWriter = new StreamWriter(outputDir + "\\" + className + ".cpp");
            streamWriter.WriteLine("#include \"" + className + ".h\"");
            streamWriter.WriteLine("#include \"CGameManager.h\"");
            streamWriter.WriteLine("#include \"CGameObject.h\"");
            streamWriter.WriteLine("#include \"IPhysicsStrategy.h\"");
            streamWriter.WriteLine("#include \"CPhysicsManager.h\"");
            streamWriter.WriteLine("#include \"CPhysicsProfile.h\"");
            streamWriter.WriteLine("#include \"CPlayerSoundComponent.h\"");
            streamWriter.WriteLine("#include \"AnimationMapping.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"OgreEntity.h\"");
            streamWriter.WriteLine("#include \"OgreSceneNode.h\"");
            streamWriter.WriteLine("#include \"OgreSceneManager.h\"");
            streamWriter.WriteLine("#include \"OgreAnimationState.h\"");
            if (materialName != null)
                streamWriter.WriteLine("#include \"OgreMaterialManager.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("using namespace Core;");
            streamWriter.WriteLine("using namespace Core::Plugin;");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "::" + className + "(const char* Name, Physics::IPhysicsStrategy* PhysicsStrategy) : CBuilding(Name, PhysicsStrategy)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tm_MeshName = \"" + fileName + "\";");
            streamWriter.WriteLine("\tauto sceneManager = CGameManager::Instance()->GetSceneManager();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tm_BuildingNode = sceneManager->getRootSceneNode()->createChildSceneNode(Name);");
            streamWriter.WriteLine("\tm_BuildingEntity = sceneManager->createEntity(Name, m_MeshName);");
            streamWriter.WriteLine("\tm_BuildingEntity->setQueryFlags(SQF_BUILDING);");

            if (materialName != null)
            {
                streamWriter.WriteLine("\tm_BuildingEntity->setMaterialName(\"" + materialName + "\");");
                streamWriter.WriteLine("\tMaterialManager::getSingleton().getByName(\"" + materialName + "\")->load();");
            }

            streamWriter.WriteLine("\tm_BuildingNode->attachObject(m_BuildingEntity);");
            streamWriter.WriteLine("\tm_BuildingNode->setPosition(0, 0, 0);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// ====== Set the building type ========");
            streamWriter.WriteLine("\tm_BuildingType = \"" + GetBuildingType(className) + "\";");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// ====== Set the load type ========");
            streamWriter.WriteLine("\tm_LoadType = ELT_INSTNACE;");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// ====== Set the category ===========");
            streamWriter.WriteLine("\tm_BuildingCategory = EBC_SUPPLY;");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// ====== Setup default physics ======");
            streamWriter.WriteLine("\tif(PhysicsStrategy && m_BuildingNode && m_BuildingEntity)");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tauto profile = Physics::CPhysicsManager::Instance()->GetPhysicsProfile(\"DEFAULT_LIGHT_CUBE\");");
            streamWriter.WriteLine("\t\tif(profile)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\tauto boundingBox = m_BuildingEntity->getBoundingBox();");
            streamWriter.WriteLine("\t\t\tauto height = boundingBox.getMaximum().y / 2;");
            streamWriter.WriteLine("\t\t\tprofile->SetPositionOffset(Vector3(0, height, 0));");
            streamWriter.WriteLine("\t\t\tString objName = String(Name) + \"_obj\";");
            streamWriter.WriteLine("\t\t\tauto gameObject = new CDummyGameObject(objName.c_str(), m_BuildingNode, m_BuildingEntity);");
            streamWriter.WriteLine("\t\t\tif(gameObject)");
            streamWriter.WriteLine("\t\t\t{");
            streamWriter.WriteLine("\t\t\t\tm_GameObjects.push_back(gameObject);");
            streamWriter.WriteLine("\t\t\t\tgameObject->PhysicsProfile = profile;");
            streamWriter.WriteLine("\t\t\t\tPhysicsStrategy->AddShape(gameObject);");
            streamWriter.WriteLine("\t\t\t}");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tPostInit();");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "::~" + className + "()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("void " + className + "::SetupMapping()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("void " + className + "::Update(const f32& elapsedTime)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();

            var factoryName = className + "Factory";
            streamWriter.WriteLine("// " + factoryName);
            streamWriter.WriteLine(factoryName + "::" + factoryName + "()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tBuildingType = \"" + GetBuildingType(className) + "\";");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();

            streamWriter.WriteLine(factoryName + "::~" + factoryName + "()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();

            streamWriter.WriteLine("Core::CBuilding* " + factoryName + "::CreateBuilding(const char* Name, Physics::IPhysicsStrategy* PhysicsStrategy)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\treturn new " + className + "(Name, PhysicsStrategy);");
            streamWriter.WriteLine("}");

            streamWriter.Close();
        }

        public void CreateFactoryExportHeader(string outputDir)
        {
            var streamWriter = new StreamWriter(outputDir + "\\Exports.h");
            streamWriter.WriteLine("#ifndef __EXPORTS_HPP__");
            streamWriter.WriteLine("#define __EXPORTS_HPP__");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"Defines.h\"");
            streamWriter.WriteLine();

            streamWriter.WriteLine("#if defined DLL_EXPORT");
            streamWriter.WriteLine("#define DECLDIR __declspec(dllexport)");
            streamWriter.WriteLine("#else");
            streamWriter.WriteLine("#define DECLDIR __declspec(dllimport)");
            streamWriter.WriteLine("#endif");
            streamWriter.WriteLine();

            streamWriter.WriteLine("namespace Core");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("namespace Plugin");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tclass IBuildingFactory;");
            streamWriter.WriteLine("\tenum E_PLUGIN;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\textern \"C\"");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tDECLDIR void GetFactories(Vector<IBuildingFactory*>& list);");
            streamWriter.WriteLine("\t\tDECLDIR E_PLUGIN GetPluginType(void);");
            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#endif //__EXPORTS_HPP__");

            streamWriter.Close();
        }

        public void CreateFactoryExportSource(List<string> fileList, string outputDir)
        {
            var streamWriter = new StreamWriter(outputDir + "\\Exports.cpp");
            streamWriter.WriteLine("#define DLL_EXPORT");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"Exports.h\"");
            streamWriter.WriteLine("#include \"Plugins/PluginTypes.h\"");
            streamWriter.WriteLine();

            foreach (string fileName in fileList)
            {
                var headerName = GetClassName(fileName);
                streamWriter.WriteLine("#include \"" + headerName + ".h\"");
            }

            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace Core");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("namespace Plugin");
            streamWriter.WriteLine("{");

            foreach (string fileName in fileList)
            {
                var className = GetClassName(fileName) + "Factory";
                var instanceName = className.Remove(0, 1);
                streamWriter.WriteLine("\t" + className + "*\t" + instanceName + " = nullptr;");
            }

            streamWriter.WriteLine();
            streamWriter.WriteLine("\textern \"C\"");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tDECLDIR void GetFactories(Vector<IBuildingFactory*>& list)");
            streamWriter.WriteLine("\t\t{");

            foreach (string fileName in fileList)
            {
                var className = GetClassName(fileName);
                var instanceName = className.Remove(0, 1);
                streamWriter.WriteLine("\t\t\t" + instanceName + "Factory = new " + className + "Factory();");
            }

            streamWriter.WriteLine();

            foreach (string fileName in fileList)
            {
                var className = GetClassName(fileName);
                var instanceName = className.Remove(0, 1) + "Factory";
                streamWriter.WriteLine("\t\t\tif (" + instanceName + ")");
                streamWriter.WriteLine("\t\t\t\tlist.push_back(" + instanceName + ");");
                streamWriter.WriteLine();
            }

            streamWriter.WriteLine("\t\t}");

            streamWriter.WriteLine("\t\tDECLDIR E_PLUGIN GetPluginType(void)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\treturn Core::Plugin::EP_BUILDING;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine("\t}");

            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");

            streamWriter.Close();
        }

        public bool CreateUnitTests(string profileName, string outputDir)
        {
            // TODO: unit test is still for GO.
            if (Mesh.GetMeshes(profileName, MeshTypes.GameObject).Count == 0)
            {
                return true;
            }

            var streamWriter = new StreamWriter(outputDir + "\\GameObjectsTest.cpp");
            streamWriter.WriteLine("#include \"../UnitTest++/UnitTest++.h\"");
            streamWriter.WriteLine("#include \"RecordingReporter.h\"");
            streamWriter.WriteLine("#include \"Core/CoreEngine.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("using namespace UnitTest;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine();
            streamWriter.WriteLine("struct GameObjectFixture");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tCGameObjectManager* GameObjectManager;");
            streamWriter.WriteLine("\tGameObjectFixture()");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tCGameManager::Instance();");
            streamWriter.WriteLine("\t\tGameObjectManager = CGameObjectManager::Instance();");
            streamWriter.WriteLine("\t\tauto physics = Core::Physics::CPhysicsManager::Instance()->CreateStrategy(DEFAULT_PHYSICS_STRATEGY);");
            streamWriter.WriteLine("\t\tphysics->InitWorld();");
            streamWriter.WriteLine("\t\tphysics->ShowDebug(true);");
            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine("};");
            streamWriter.WriteLine();

            foreach (var fileName in Mesh.GetMeshes(profileName).Select(m => m.FileName))
            {
                var typeName = fileName.Replace(".mesh", string.Empty).Replace(" ", "").ToUpper();
                var className = GetClassName(fileName);
                className = className.Replace(" ", "");

                // Add
                streamWriter.WriteLine("TEST_FIXTURE(GameObjectFixture, GameObjectAdd_" + className + "_Success)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tif(!GameObjectManager)");
                streamWriter.WriteLine("\t\tCHECK(false);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\tauto obj = GameObjectManager->CreateObject(\"" + typeName + "\", \"" + typeName + "\", Vector3(0, 0, 0));");
                streamWriter.WriteLine("\tCHECK(obj);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\tVector<CGameObject*> gameObjects;");
                streamWriter.WriteLine("\tGameObjectManager->GetGameObjects(gameObjects);");
                streamWriter.WriteLine("\tCHECK_EQUAL(gameObjects.size(), 1);");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine();

                // Get
                streamWriter.WriteLine("TEST_FIXTURE(GameObjectFixture, GameObjectGet_" + className + "_Success)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tif(!GameObjectManager)");
                streamWriter.WriteLine("\t\tCHECK(false);");
                streamWriter.WriteLine("\t");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\tCHECK(GameObjectManager->GetGameObject(\"" + typeName + "\"));");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine();

                // Remove
                streamWriter.WriteLine("TEST_FIXTURE(GameObjectFixture, GameObjectRemove_" + className + "_Success)");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tif(!GameObjectManager)");
                streamWriter.WriteLine("\t\tCHECK(false);");
                streamWriter.WriteLine("\tGameObjectManager->RemoveGameObject(GameObjectManager->GetGameObject(\"" + typeName + "\"));");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\tVector<CGameObject*> gameObjects;");
                streamWriter.WriteLine("\tGameObjectManager->GetGameObjects(gameObjects);");
                streamWriter.WriteLine("\tCHECK_EQUAL(gameObjects.size(), 0);");
                streamWriter.WriteLine("}");
                streamWriter.WriteLine();
            }
            streamWriter.WriteLine("}");
            streamWriter.Close();
            return true;
        }

        private string GetBuildingType(string className)
        {
            return className.ToUpper().Replace("CBUILDING", string.Empty);
        }

        private static string GetClassName(string fileName)
        {
            var className = fileName.Replace(".mesh", string.Empty);
            className = className.Replace(" ", "");
            className = className.Replace("-", "");
            className = char.ToUpper(className[0]) + className.Substring(1);
            className = "CBuilding" + className;
            return className;
        }
    }
}
