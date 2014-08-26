using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
/**
 * @file CatProject
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    public class CatProject {
        // file path rule: use '/' and should end with '/'

        // name settings
        /*public static string CONFIG_DIR = "config";*/
/*        public static string SCENE_DIR = "scene";*/
        public static string PLUGIN_DIR = "plugin";
        public static string RESOURCE_DIR = "resource";
        public static string ASSET_DIR = "asset";
        /*public static string IMAGE_DIR = "image";*/
        public static string BASIC_PLUGIN = "BasicPlugin.dll";

        // resource directory
        public string projectRoot;              // full path of the project root
//         string materialConfigure;
//         string modelConfigure;
//         string prefabConfigure;
        /*public string resourceDirectory;*/
        /*public string imageDirectory;*/
/*        public string sceneDirectory;*/
        /*public string pluginDirectory;*/

        public CatMaterialTemplateList materialList1;
        public CatModelList modelList1;

        // XNA resource manager
        public ContentManager contentManger;

        // resource lists
        public PrefabList prefabList;
   //     public ModelList modelList;
  //      public MaterialList materialList;
        public TypeManager typeManager;
        private BTTreeManager m_btTreeManager;
        public BTTreeManager BTTreeManager {
            get {
                return m_btTreeManager;
            }
        }
        public SoundManager m_soundManager;
        public SoundManager SoundManager {
            get {
                return m_soundManager;
            }
        }

        private MotionDelegator m_motionDelegator;
        public MotionDelegator MotionDelegator {
            get {
                if (m_motionDelegator == null) {
                    m_motionDelegator = new MotionDelegator();
                }
                return m_motionDelegator;
            }
        }
        // project attributes
        string projectName;
        public string currentSceneName;
        public string startupSceneName;

        // runtime
        Scene runningScene;

        public CatProject(Game _game) {
            prefabList = new PrefabList();
            modelList1 = new CatModelList();
            materialList1 = null;
            typeManager = new TypeManager();
            m_soundManager = new SoundManager();
        }

        public static CatProject CreateEmptyProject(string _projectName, 
            string _projectUnderDirectory, Game _game) {
            
            // 1. create project directory structure
            string projectRoot = _projectUnderDirectory;
            projectRoot += _projectName + '/';
            // project root
            TestCreateDirectory(projectRoot);
            // sub directory
            // plugin
            string pluginDirectory = projectRoot + '/' + PLUGIN_DIR;
            TestCreateDirectory(pluginDirectory);
            if (File.Exists("plugin/"  + BASIC_PLUGIN)) {
                File.Copy("plugin/" + BASIC_PLUGIN, pluginDirectory + '/' + BASIC_PLUGIN, true);
            }
            // resource
            string resourceDirectory = projectRoot + '/' + RESOURCE_DIR + '/';
            TestCreateDirectory(resourceDirectory);
            TestCreateDirectory(resourceDirectory + "image");
            TestCreateDirectory(resourceDirectory + "effect");
            TestCreateDirectory(resourceDirectory + "material");
            TestCreateDirectory(resourceDirectory + "model");
            TestCreateDirectory(resourceDirectory + "prefab");
            TestCreateDirectory(resourceDirectory + "scene");
            TestCreateDirectory(resourceDirectory + "ai");
            // copy basic resource
            string[] fxs = Directory.GetFiles("resource/effect");
            foreach (String fx in fxs) {
                File.Copy(fx, resourceDirectory + "effect/" + Path.GetFileName(fx), true);
            }
            string[] mtrls = Directory.GetFiles("resource/material");
            foreach (String mtrl in mtrls) {
                File.Copy(mtrl, resourceDirectory + "material/" + Path.GetFileName(mtrl), true);
            }
            // asset
            string assetDirectory = projectRoot + '/' + ASSET_DIR;
            TestCreateDirectory(assetDirectory);

           // 2. create project instance

            CatProject newProject = new CatProject(_game);
            newProject.projectName = _projectName;
            newProject.projectRoot = projectRoot;
            newProject.currentSceneName = "UntitleScene";
            newProject.startupSceneName = newProject.currentSceneName;

            newProject.contentManger = new ContentManager(_game.Services);
            newProject.contentManger.RootDirectory = projectRoot + ASSET_DIR + '/' + RESOURCE_DIR;
            newProject.m_btTreeManager = new BTTreeManager();
            newProject.m_btTreeManager.BTTreeDirectoryRoot = newProject.projectRoot + '/' + ASSET_DIR + '/' + RESOURCE_DIR + "/ai";

            // save project files
            newProject.SaveProject(newProject.GetProjectXMLAddress(), true);
            Mgr<CatProject>.Singleton = newProject;
            // create a empty scene
            Scene scene = Scene.CreateEmptyScene(newProject);
            scene.SaveScene(newProject.projectRoot + RESOURCE_DIR + "/scene/" + newProject.currentSceneName + ".scene");
            newProject.SynchronizeScene();
            newProject.SynchronizeBTTrees();

            return newProject;
        }

        public bool SaveProject(string _fileName, bool _synchronized = false) {
            // save project file
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement eleProject = doc.CreateElement("Project");
            doc.AppendChild(eleProject);

            XmlElement eleProjectName = doc.CreateElement("ProjectName");
            eleProject.AppendChild(eleProjectName);
            eleProjectName.InnerText = projectName;

            XmlElement eleSceneLoad = doc.CreateElement("SceneLoad");
            eleProject.AppendChild(eleSceneLoad);
            eleSceneLoad.SetAttribute("startup", startupSceneName);
            eleSceneLoad.SetAttribute("current", currentSceneName);

            doc.Save(_fileName);

            // save other configures
            modelList1.SaveModels(projectRoot + RESOURCE_DIR + "/model/");
            /*prefabList.SavePrefabs(projectRoot + RESOURCE_DIR + "/prefab/");*/
            prefabList.SavePrefabs(projectRoot + RESOURCE_DIR + "/prefab");
            m_btTreeManager.SaveAllBTTree(projectRoot + RESOURCE_DIR + "/ai");
            // consider to recompile here
            CompileResource(_synchronized);
            SynchronizeBTTrees();
            SynchronizeScene();

            return true;
        }

        public string GetBTTreeWriteDirectory() {
            return projectRoot + RESOURCE_DIR + "/ai";
        }

        public static string GetStandardPath(string _path) {
            if (!_path.EndsWith("/") && !_path.EndsWith("\\")) {
                return _path + "/";
            }
            return _path;
        }

        public static CatProject OpenProject(string _fileName, Game _game) {

            if (!File.Exists(_fileName)) {
                return null;
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(_fileName);
            // get project root
            int index = _fileName.LastIndexOf('/');
            if (index < 0) {
                index = _fileName.LastIndexOf('\\');
            }
            string projectRoot = "";
            if (index >= 0) {
                projectRoot = _fileName.Substring(0, index) + '/';
            }

            // read project itself
            CatProject newProject = new CatProject(_game);

            XmlNode nodeProject = doc.SelectSingleNode("Project");
            newProject.projectRoot = projectRoot;
            newProject.projectName = nodeProject.SelectSingleNode("ProjectName").InnerText;
            XmlElement nodeSceneLoad = (XmlElement)nodeProject.SelectSingleNode("SceneLoad");
            newProject.startupSceneName = nodeSceneLoad.GetAttribute("startup");
            newProject.currentSceneName = nodeSceneLoad.GetAttribute("current");
            newProject.contentManger = new ContentManager(_game.Services);
            newProject.contentManger.RootDirectory = projectRoot + ASSET_DIR + '/' + RESOURCE_DIR + '/';
            Mgr<CatProject>.Singleton = newProject;

            // load shader
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                newProject.CompileResource();
                newProject.SynchronizeScene();
                newProject.SynchronizeBTTrees();
            }
            //newProject.CompileShader(newProject.projectRoot);

            // read configures
            // test new materiallist
            newProject.materialList1 = CatMaterialTemplateList.LoadMaterialTemplates(
                newProject.projectRoot + ASSET_DIR + '/' + RESOURCE_DIR + "/material/");

//             newProject.materialList = MaterialList.LoadMaterilas(
//                 newProject.GetConfigurePath(newProject.materialConfigure), 
//                 newProject);
            
            // test new model
            newProject.modelList1 = CatModelList.LoadModels(
                newProject.projectRoot + ASSET_DIR + '/' + RESOURCE_DIR + "/model/",
                newProject);

//             newProject.modelList = ModelList.LoadModels(
//                 newProject.GetConfigurePath(newProject.modelConfigure),
//                 newProject);

            // load plugins
            newProject.typeManager = new TypeManager();
            newProject.typeManager.Load_Plugins(newProject.projectRoot + PLUGIN_DIR + '/');
            newProject.typeManager.LoadConsoleCommands(newProject.projectRoot + PLUGIN_DIR + '/');
            newProject.typeManager.LoadBTTreeNodes(newProject.projectRoot + PLUGIN_DIR + '/');
            Mgr<GameEngine>.Singleton.CatConsole.SetTypeManager(newProject.typeManager);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                newProject.typeManager.Load_EditorScripts(newProject.projectRoot + PLUGIN_DIR + '/');
            }
            Mgr<TypeManager>.Singleton = newProject.typeManager;
            newProject.m_btTreeManager = new BTTreeManager();
            newProject.m_btTreeManager.BTTreeDirectoryRoot = newProject.projectRoot + '/' + ASSET_DIR + '/' + RESOURCE_DIR + "/ai";

//             newProject.prefabList = PrefabList.LoadPrefabs(
//                 newProject.GetConfigurePath(newProject.prefabConfigure),
//                 newProject);
            newProject.prefabList = PrefabList.LoadFromFiles(
               newProject.projectRoot + ASSET_DIR + '/' + RESOURCE_DIR + "/prefab/");


            // load current scene
//             newProject.runningScene = Scene.LoadScene(
//                 newProject.GetSceneFileAddress(newProject.currentSceneName));

            return newProject;
        }

        public string GetSceneFileAddress(string _filename_no_xml) {
            return projectRoot + "asset/" + RESOURCE_DIR + "/scene/" + _filename_no_xml + ".scene";
        }

        public string GetResourceSceneFileAddress(string _filename_no_xml) {
            return projectRoot + RESOURCE_DIR + "/scene/" + _filename_no_xml + ".scene"; 
        }

        public string GetProjectXMLAddress() {
            return projectRoot + "/project.xml";
        }

        private XmlNode MakeTextureProcessNode(string _imageName, XmlDocument _doc, XmlNamespaceManager _nsMgr) {
//          <ItemGroup>
//              <Compile Include="image\grassland_land.png">
//                  <Name>grassland_land</Name>
//                  <Importer>TextureImporter</Importer>
//                  <Processor>TextureProcessor</Processor>
//                  <ProcessorParameters_ColorKeyEnabled>False</ProcessorParameters_ColorKeyEnabled>
//                  <ProcessorParameters_PremultiplyAlpha>False</ProcessorParameters_PremultiplyAlpha>
//                  <ProcessorParameters_TextureFormat>Color</ProcessorParameters_TextureFormat>
//              </Compile>   
//          </ItemGroup>
            XmlElement eleItemGroup = _doc.CreateElement("ItemGroup", 
                _doc.DocumentElement.NamespaceURI);
            XmlElement eleCompile = _doc.CreateElement("Compile", 
                _doc.DocumentElement.NamespaceURI);
            eleItemGroup.AppendChild(eleCompile);
            eleCompile.SetAttribute("Include", "image\\" + _imageName);
            XmlElement eleName = _doc.CreateElement("Name", 
                _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleName);
            eleName.InnerText = Path.GetFileNameWithoutExtension(_imageName);
            XmlElement eleImport = _doc.CreateElement("Importer", 
                _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleImport);
            eleImport.InnerText = "TextureImporter";
            XmlElement eleProcessor = _doc.CreateElement("Processor", 
                _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleProcessor);
            eleProcessor.InnerText = "TextureProcessor";
            XmlElement eleParameterColorKey = _doc.CreateElement("ProcessorParameters_ColorKeyEnabled", 
                _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleParameterColorKey);
            eleParameterColorKey.InnerText = "False";
            XmlElement eleParameterAlpha = _doc.CreateElement("ProcessorParameters_PremultiplyAlpha",
                 _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleParameterAlpha);
            eleParameterAlpha.InnerText = "False";
            XmlElement eleParameterColor = _doc.CreateElement("ProcessorParameters_TextureFormat",
                 _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleParameterColor);
            eleParameterColor.InnerText = "Color";
            XmlElement eleParameterMipmap = _doc.CreateElement("ProcessorParameters_GenerateMipmaps",
                 _doc.DocumentElement.NamespaceURI);
            eleCompile.AppendChild(eleParameterMipmap);
            eleParameterMipmap.InnerText = "True";

            return eleItemGroup;
        }

        private void AddTextureNodes(string _textureDirectory, XmlNode _parent, XmlDocument _doc, XmlNamespaceManager _nsMgr) {
            string[] files = Directory.GetFiles(_textureDirectory, "*.png");
            foreach (string file in files) {
                XmlNode nodeCurTexture = MakeTextureProcessNode(Path.GetFileName(file), _doc, _nsMgr);
                _parent.AppendChild(nodeCurTexture);
            }
        }

        private void CompileResource(bool _synchronized = false) {
            // modify output project file
            XmlDocument template = new XmlDocument();
            template.Load("compileResource.proj");
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(template.NameTable);
            nsMgr.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlNode outputPathNode = template.SelectSingleNode("/ns:Project/ns:PropertyGroup/ns:OutputPath", nsMgr);
            XmlNode nodeProject = template.SelectSingleNode("/ns:Project", nsMgr);
            outputPathNode.InnerText = projectRoot + "asset";

            AddTextureNodes(projectRoot + CatProject.RESOURCE_DIR + '/' + "image/", nodeProject, template, nsMgr);

            // save to project root
            template.Save(projectRoot + '/' + CatProject.RESOURCE_DIR + '/' + CatProject.RESOURCE_DIR + ".proj");

            // run msbuild
            Process msbuild = new Process();
            msbuild.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%") + @"\microsoft.NET\Framework\v4.0.30319\msbuild.exe";
            msbuild.StartInfo.Arguments = projectRoot + CatProject.RESOURCE_DIR + '/' + CatProject.RESOURCE_DIR + ".proj";
            msbuild.Start();
            if (_synchronized) {
                msbuild.WaitForExit();
            }
        }

        private void SynchronizeDirectory(string _dirName) {
            // delete files in scene in asset folder
            String assetDir = projectRoot + '/' + ASSET_DIR + '/' + RESOURCE_DIR + '/' + _dirName;
            if (Directory.Exists(assetDir)) {
                foreach (String file in Directory.GetFiles(assetDir)) {
                    File.Delete(file);
                }
            }
            else {
                Directory.CreateDirectory(assetDir);
            }
            // copy from resource to files
            foreach (String file in Directory.GetFiles(projectRoot + '/' + RESOURCE_DIR + '/' + _dirName)) {
                File.Copy(file, assetDir + '/' + Path.GetFileName(file));
            }
        }

        public void SynchronizeScene() {
            SynchronizeDirectory("scene");
        }

        public void SynchronizeBTTrees() {
            SynchronizeDirectory("ai");
        }

        public void PublishProject(string _path) {
            // create folder
            TestCreateDirectory(_path);
            // copy proejct files
            System.IO.File.Copy(projectRoot + "project.xml", _path + "/project.xml");
            // copy plugin
            copyFolder(projectRoot + CatProject.PLUGIN_DIR, _path + '/' + CatProject.PLUGIN_DIR);
           
            // copy game engine
            System.IO.File.Copy("Core.dll", _path + '/' + "Core.dll");
            System.IO.File.Copy("GameEntrance.exe", _path + '/' + "GameEntrance.exe");
            System.IO.File.Copy("FarseerPhysics XNA.dll", _path + "/" + "FarseerPhysics XNA.dll");
            // compile resource
            CompileResource();
            SynchronizeBTTrees();
            SynchronizeScene();

            copyFolder(projectRoot + CatProject.ASSET_DIR, _path + '/' + CatProject.ASSET_DIR + '/');  
        }

        private void copyFolder(string _from, string _to) {
            if (!Directory.Exists(_to)) {
                Directory.CreateDirectory(_to);
            }

            // directories
            foreach (string sub in Directory.GetDirectories(_from)) {
                copyFolder(sub + "\\", _to + Path.GetFileName(sub) + "\\");
            }
            // files
            foreach (string file in Directory.GetFiles(_from)) {
                File.Copy(file, _to + "\\" + Path.GetFileName(file), true);
            }
        }

        static public void TestCreateDirectory(string _path) {
            if (!System.IO.Directory.Exists(_path)) {
                System.IO.Directory.CreateDirectory(_path);
            }
        }
    
    }
}
