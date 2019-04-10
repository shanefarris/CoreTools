using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using AssetManager.Common.Enums;
using AssetManager.Dal;

namespace AssetManager.Code_Generators
{
    class GameObjectGenerator
    {
        public void CreateHeader(string fileName, string outputDir)
        {
            var className = GetClassName(fileName);
            var streamWriter = new StreamWriter(outputDir + "\\" + className + ".h");
            streamWriter.WriteLine("#ifndef __" + className.ToUpper() + "_H__");
            streamWriter.WriteLine("#define __" + className.ToUpper() + "_H__");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"CGameObject.h\"");
            streamWriter.WriteLine("#include \"Plugins/IGameObjectFactory.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace Core");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("namespace Plugin");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tclass " + className + " : public CGameObject");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\tpublic:");
            streamWriter.WriteLine("\t\t" + className + "(const char* Name, Vector3& Pos);");
            streamWriter.WriteLine("\t\t~" + className + "();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\tvoid Update(const f32& elapsedTime);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t};");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tclass " + className + "Factory : public IGameObjectFactory");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\tpublic:");
            streamWriter.WriteLine("\t\t" + className + "Factory();");
            streamWriter.WriteLine("\t\t~" + className + "Factory();");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\tCGameObject* CreateObject(const char* Name, Vector3& Pos);");
            streamWriter.WriteLine("\t};");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#endif // __" + className.ToUpper() + "_H__");
            streamWriter.Close();
        }

        public void CreateSource(string fileName, string outputDir, bool enableScripting, string materialName = null)
        {
            var typeName = fileName.Replace(".mesh", string.Empty).Replace(" ", "");
            var className = GetClassName(fileName);
            className = className.Replace(" ", "");
            typeName = typeName.ToUpper();
            var streamWriter = new StreamWriter(outputDir + "\\" + className + ".cpp");
            streamWriter.WriteLine("#include \"" + className + ".h\"");
            streamWriter.WriteLine("#include \"CGameManager.h\"");
            streamWriter.WriteLine("#include \"CPhysicsManager.h\"");
            streamWriter.WriteLine("#include \"CPhysicsProfile.h\"");
            streamWriter.WriteLine("#include \"IPhysicsStrategy.h\"");
            streamWriter.WriteLine();
            streamWriter.WriteLine("#include \"OgreEntity.h\"");
            streamWriter.WriteLine("#include \"OgreSceneNode.h\"");
            streamWriter.WriteLine("#include \"OgreSceneManager.h\"");
            if (materialName != null)
                streamWriter.WriteLine("#include \"OgreMaterialManager.h\"");
            streamWriter.WriteLine();
            if (enableScripting)
            {
                streamWriter.WriteLine("#define COMPILE_SCRIPT");
                streamWriter.WriteLine("#include \"Defines.h\"");
                streamWriter.WriteLine("#include \"CScriptManager.h\"");
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine("using namespace Core;");
            streamWriter.WriteLine();
            streamWriter.WriteLine("namespace Core");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("namespace Plugin");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "::" + className + "(const char* Name, Vector3& Pos) : CGameObject(Name)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tNode = CGameManager::Instance()->GetSceneManager()->getRootSceneNode()->createChildSceneNode(Name + String(\"_node\"), Pos);");
            streamWriter.WriteLine("\tEntity = CGameManager::Instance()->GetSceneManager()->createEntity(Name + String(\"_entity\"), \"" + fileName + "\");");
            streamWriter.WriteLine("\tEntity->setQueryFlags(SQF_GAMEOBJECT);");
            streamWriter.WriteLine("\tNode->attachObject(Entity);");

            if (materialName != null)
            {
                streamWriter.WriteLine("\tEntity->setMaterialName(\"" + materialName + "\");");
                streamWriter.WriteLine("\tMaterialManager::getSingleton().getByName(\"" + materialName + "\")->load();");
            }

            streamWriter.WriteLine("\t//Node->setScale(1.0f, 1.0f, 1.0f);");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// TODO: Customize physics profile here");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine("\tPhysicsProfile = Physics::CPhysicsManager::Instance()->GetPhysicsProfile(Physics::CPhysicsManager::DEFAULT_LIGHT_CUBE);");
            streamWriter.WriteLine("\tif(PhysicsProfile)");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tPhysics::CPhysicsManager::Instance()->GetStrategy()->AddShape(this);");
            streamWriter.WriteLine("\t}");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\t// ====== Set the load type ========");
            streamWriter.WriteLine("\tm_LoadType = ELT_INSTNACE;");
            streamWriter.WriteLine("\t// ===================================");
            streamWriter.WriteLine();
            streamWriter.WriteLine("\tGameObjectType = \"" + typeName + "\";");
            if (enableScripting)
                streamWriter.WriteLine("\tCALL_VOIDFUNCGAMEOBJECT(\"GameObjectCreated\", this);");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("void " + className + "::Update(const f32& elapsedTime)");
            streamWriter.WriteLine("{");
            if (enableScripting)
                streamWriter.WriteLine("\tCALL_VOIDFUNCFLOAT(\"GameObjectUpdate\", elapsedTime);");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "::~" + className + "()");
            streamWriter.WriteLine("{");
            if (enableScripting)
                streamWriter.WriteLine("\tCALL_VOIDFUNCGAMEOBJECT(\"GameObjectRemoved\", this);");
            streamWriter.WriteLine("\tPhysics::CPhysicsManager::Instance()->GetStrategy()->DestroyPhysicsBody(this);");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "Factory::" + className + "Factory()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\tGameObjectType = \"" + typeName + "\";");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine(className + "Factory::~" + className + "Factory()");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine();
            streamWriter.WriteLine("Core::CGameObject* " + className + "Factory::CreateObject(const char* Name, Vector3& Pos)");
            streamWriter.WriteLine("{");
            streamWriter.WriteLine("\treturn new Plugin::" + className + "(Name, Pos);");
            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");
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
            streamWriter.WriteLine("#include \"CGameObject.h\"");
            streamWriter.WriteLine("#include \"Plugins/IGameObjectFactory.h\"");
            streamWriter.WriteLine("#include \"Plugins/PluginTypes.h\"");
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
            streamWriter.WriteLine();

            streamWriter.WriteLine("\textern \"C\"");
            streamWriter.WriteLine("\t{");
            streamWriter.WriteLine("\t\tDECLDIR void GetFactories(Vector<IGameObjectFactory*>& list);");
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
            streamWriter.WriteLine("\t\tDECLDIR void GetFactories(Vector<IGameObjectFactory*>& list)");
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
            streamWriter.WriteLine("\t\t\treturn Core::Plugin::EP_OBJECT_FACTORY;");
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine("\t}");

            streamWriter.WriteLine("}");
            streamWriter.WriteLine("}");

            streamWriter.Close();
        }

        public bool CreateUnitTests(string profileName, string outputDir)
        {
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

        private static string GetClassName(string fileName)
        {
            var className = fileName.Replace(".mesh", string.Empty);
            className = className.Replace(" ", "");
            className = className.Replace("-", "");
            className = char.ToUpper(className[0]) + className.Substring(1);
            className = "CGameObject" + className;
            return className;
        }
    }
}
