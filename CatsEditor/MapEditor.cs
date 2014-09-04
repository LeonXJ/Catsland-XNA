using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MapEditorControlLibrary;
using System.IO;
using System.Xml;
using System.Reflection;
using Catsland.Core;
using CatsEditor;
using System.Diagnostics;
using Catsland.CatsEditor.EditorCommand;
using System.Collections;
using Catsland.CatsEditor.PropertyEditorWidget;

namespace Catsland.Editor {
    public partial class MapEditor : Form, IEditor {
        
        public GameEngine m_gameEngine;

        bool m_observingModelReady;
        bool m_boolWelcomeDialog;
        AnimationClip.PlayMode[] PlayModeLUT;

        CatModel m_observingModel;
        public CatMaterial m_observingMaterial;
        GameObject m_observingGameObject;
        public CatProject curProject;
        public string curProjectRoot;
        static string m_tmp_filename = "catsengine.tmp";
        Dictionary<string, TabPage> tabpages;
        // keys
        bool isCtrlDown = false;

        // display
        float widthDivHight = 0.0f;

        // move gameObject
        Vector3 movingGameObjectOriginalPosition;
        Vector3 mouseDownPositionInWorld;
        bool isMouseDown = false;
        enum MovingMode {
            XY,
            XZ
        };
        MovingMode curMovingMode = MovingMode.XY;
        float observingXYheight = 0.0f;

        // test
        public BTTreeEditor m_btTreeEditor;
        public BTTreeEditor BTTreeEditor{
            get {
                if (m_btTreeEditor == null || m_btTreeEditor.IsDisposed) {
                    m_btTreeEditor = new BTTreeEditor();
                }
                return m_btTreeEditor;
            }
        }

        public MapEditor(bool _showWelcomeDialog = true) {
            m_boolWelcomeDialog = _showWelcomeDialog;
            InitializeComponent();
            InitializeEditor();
            Mgr<MapEditor>.Singleton = this;
        }

        void InitializeEditor() {

            PlayModeLUT = new AnimationClip.PlayMode[4];
            PlayModeLUT[0] = AnimationClip.PlayMode.CLAMP;
            PlayModeLUT[1] = AnimationClip.PlayMode.LOOP;
            PlayModeLUT[2] = AnimationClip.PlayMode.PINGPONG;
            PlayModeLUT[3] = AnimationClip.PlayMode.STOP;

            // resolution_selector
            resolution_selector.Items.Add("4:3");
            resolution_selector.Items.Add("16:10");
            resolution_selector.Items.Add("16:9");
            resolution_selector.Items.Add("1366:768");
            resolution_selector.Items.Add("No restrict");
            resolution_selector.SelectedItem = "No restrict";

            // backup tabpages
            tabpages = new Dictionary<string, TabPage>();
            foreach (TabPage tabpage in attr_tab.TabPages) {
                tabpages.Add(tabpage.Text, tabpage);
            }
            ShowAttrTabPage("");

            //renderArea.LostFocus += renderAreaLostFocus;
            renderArea.GotFocus += renderAreaGetFocus;
            
        }

        public void ShowAttrTabPage(string _tabPageName) {
            attr_tab.TabPages.Clear();
            if (tabpages.ContainsKey(_tabPageName)) {
                attr_tab.TabPages.Add(tabpages[_tabPageName]);
            }
        }

        public void renderAreaLostFocus(object sender, EventArgs e) {
            Mgr<GameEngine>.Singleton.LostFocus();
        }

        private void renderAreaGetFocus(object sender, EventArgs e) {
            Mgr<GameEngine>.Singleton.GetFocus();
        }

        private void renderArea_Click(object sender, EventArgs e) {
            renderArea.Focus();
            
        }

        private void menu_insert_component(object sender, EventArgs e) {
            if (m_observingGameObject != null) {
                string componentName = (string)((ToolStripMenuItem)sender).Tag;

                Type componentType = Mgr<TypeManager>.Singleton.GetCatComponentType(componentName);
                ConstructorInfo constructorInfo = componentType.GetConstructor(new Type[1] { typeof(GameObject) });
                CatComponent component = (CatComponent)constructorInfo.Invoke(new Object[1] { m_observingGameObject });

                m_observingGameObject.AddComponent(component);
//                 component.BindToScene(Mgr<Scene>.Singleton);
//                 component.Initialize(Mgr<Scene>.Singleton);
                UpdateGameObjectAttribute(m_observingGameObject);
            }
        }

        private void menu_execute_editor_script(object sender, EventArgs e) {
            string editorScriptName = (string)((ToolStripMenuItem)sender).Tag;

            Type editorScriptType = Mgr<TypeManager>.Singleton.GetEditorScript(editorScriptName);
            ConstructorInfo constructorInfo = editorScriptType.GetConstructor(new Type[0] { });
            IEditorScript iEditorScript = (IEditorScript)constructorInfo.Invoke(new Object[0] { });

            iEditorScript.RunScript();
        }
        

        public void PostInitializeEditor() {
            // add the resource file names into lists
            //InitializeResourceList("image", attr_mtrl_texture);
            //InitializeResourceList("effect", attr_mtrl_effect);

            // component insert items
            //updateInsertComponentMenu();

            // compound gameObject menu
            //updateEditorScriptMenu();
            if (m_boolWelcomeDialog) {
                new WelcomeDialog(this).ShowDialog(this);
                Mgr<MapEditor>.Singleton = this;
            }
//             else {
//                 CatProject newProject = CatProject.CreateEmptyProject("defaultProject", System.AppDomain.CurrentDomain.BaseDirectory, m_gameEngine);
//                 ExecuteCommend(new OpenProjectCommand(newProject.GetProjectXMLAddress()));
//             }
        }

        public void updateInsertComponentMenu() {
            menu_component.DropDownItems.Clear();
            if (Mgr<TypeManager>.Singleton == null) {
                return;
            }
            Dictionary<string, Type> catComponents = Mgr<TypeManager>.Singleton.CatComponents;
            if (catComponents != null) {
                foreach (KeyValuePair<string, Type> key_value in catComponents) {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = key_value.Key;
                    item.Click += menu_insert_component;
                    item.Tag = key_value.Key;
                    ToolStripMenuItem currentMenuItem = menu_component;

                    MethodInfo method = key_value.Value.GetMethod("GetMenuNames");
                    if (method != null) {
                        string menuNames = (string)(method.Invoke(null, new object[0] { }));
                        string[] names = menuNames.Split('|');
                        
                        for (int i = 0; i < names.Length - 1; ++i) {
                            // find if the menu has existed
                            bool found = false;
                            foreach(ToolStripItem tsi in currentMenuItem.DropDownItems){
                                if(tsi.Text == names[i] && tsi is ToolStripMenuItem){
                                    currentMenuItem = (ToolStripMenuItem)tsi;
                                    found = true;
                                    break;
                                }
                            }
                            // if not found create it
                            if(!found){
                                ToolStripMenuItem newMenuItem = new ToolStripMenuItem();
                                newMenuItem.Text = names[i];
                                currentMenuItem.DropDownItems.Add(newMenuItem);
                                currentMenuItem = newMenuItem;
                            }
                        }
                        item.Text = names[names.Length -1];
                    }
                    currentMenuItem.DropDownItems.Add(item);
                }
            }
        }

        public void updateEditorScriptMenu() {
            compoundGameObjectMenu.DropDownItems.Clear();
            if (Mgr<CatProject>.Singleton == null 
                || Mgr<CatProject>.Singleton.typeManager == null) {
                return;
            }
            Dictionary<string, Type> editorScript = Mgr<CatProject>.Singleton.typeManager.EditorScripts;
            if (editorScript != null) {
                foreach (KeyValuePair<string, Type> keyValue in editorScript) {
                    ToolStripItem item = new ToolStripMenuItem();
                    item.Text = keyValue.Key;
                    item.Tag = keyValue.Key;
                    item.Click += menu_execute_editor_script;
                    ToolStripMenuItem currentMenuItem = compoundGameObjectMenu;

                    MethodInfo method = keyValue.Value.GetMethod("GetMenuNames");
                    if (method != null) {
                        string menuNames = (string)(method.Invoke(null, new object[0] { }));
                        string[] names = menuNames.Split('|');

                        for (int i = 0; i < names.Length - 1; ++i) {
                            // find if the menu has existed
                            bool found = false;
                            foreach (ToolStripItem tsi in currentMenuItem.DropDownItems) {
                                if (tsi.Text == names[i] && tsi is ToolStripMenuItem) {
                                    currentMenuItem = (ToolStripMenuItem)tsi;
                                    found = true;
                                    break;
                                }
                            }
                            // if not found create it
                            if (!found) {
                                ToolStripMenuItem newMenuItem = new ToolStripMenuItem();
                                newMenuItem.Text = names[i];
                                currentMenuItem.DropDownItems.Add(newMenuItem);
                                currentMenuItem = newMenuItem;
                            }
                        }
                        item.Text = names[names.Length - 1];
                    }
                    currentMenuItem.DropDownItems.Add(item);
                }
            }
        }

        public void SetCurrentProject(CatProject project, string root) {
            curProject = project;
            curProjectRoot = root;
        }

        public void SetGameEngine(GameEngine gameEngine) {
            m_gameEngine = gameEngine;
        }

        public Point GetRenderAreaSize() {
            return new Point(renderArea.Width, renderArea.Height);
        }

        public IntPtr GetRenderAreaHandle() {
            return renderArea.Handle;
        }

        void InitializeResourceList(String rootDirectory, ComboBox list) {
            list.Items.Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(m_gameEngine.Content.RootDirectory + "/" + rootDirectory);
            if (!dirInfo.Exists) {
                Console.WriteLine("Resource directory " + m_gameEngine.Content.RootDirectory + "/" + rootDirectory +
                    " does not exist.");
                return;
            }

            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files) {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                list.Items.Add(rootDirectory + "/" + key);
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            
        }

        private void renderArea_SizeChanged(object sender, EventArgs e) {
            if (m_gameEngine != null) {
                m_gameEngine.GameEngine_SizeChanged();
            }
        }

        public void UpdateGameObjectList(GameObjectList sceneGameObjectList) {
            // TODO: add or minus one?
            gameObjectTree.BeginInvoke(
                new UpdateGameObjectListDelegate(UpdateGameObjectList_u), new object[] { sceneGameObjectList });
        }

        delegate void UpdateGameObjectListDelegate(GameObjectList sceneGameObjectList);
        private void UpdateGameObjectList_u(GameObjectList sceneGameObjectList) {
            // move to gameObject tree
            gameObjectTree.Nodes.Clear();
            TreeNode sceneRoot = new TreeNode("Scene Root");
            sceneRoot.Tag = null;
            if (sceneGameObjectList != null && sceneGameObjectList.contentList != null) {
                foreach (KeyValuePair<string, GameObject> keyValue in sceneGameObjectList.contentList) {
                    if (keyValue.Value.Parent == null) {
                        TreeNode newNode = new TreeNode(keyValue.Value.Name);
                        newNode.Tag = keyValue.Value.GUID;
                        // children
                        IterAddChildrenGameObjectToNode(keyValue.Value, newNode, true);
                        // add this
                        sceneRoot.Nodes.Add(newNode);
                      
                        if (keyValue.Value.isExpend) {
                            newNode.Expand();
                        }
                    }
                }
            }
            gameObjectTree.Nodes.Add(sceneRoot);
            sceneRoot.Expand();
            gameObjectTree.Refresh();

        }

        private void IterAddChildrenGameObjectToNode(GameObject gameObject, TreeNode treeNode, bool isAddTag) {
            if (gameObject.Children != null) {
                foreach (GameObject child in gameObject.Children) {
                    TreeNode newNode = new TreeNode(child.Name);
                    if (isAddTag) {
                        newNode.Tag = child.GUID;
                    }
                    // children
                    IterAddChildrenGameObjectToNode(child, newNode, isAddTag);
                    // add this
                    treeNode.Nodes.Add(newNode);
                    
                    if (gameObject.isExpend) {
                        newNode.Expand();
                    }
                }
            }
        }

        public void UpdateSceneAttribute(Scene scene) {
            attr_tab_scene.BeginInvoke(
                new UpdateSceneAttributeDelegate(UpdateSceneAttribute_u), new object[] { scene });
        }

        delegate void UpdateSceneAttributeDelegate(Scene scene);
        private void UpdateSceneAttribute_u(Scene scene) {


            attr_tab_scene.Refresh();
        }

        public void UpdateModelList(Dictionary<String, CatModel> models) {
            modelList.BeginInvoke(
                new UpdateModelListDelegate(UpdateModelList_u), new object[] { models });
        }

        delegate void UpdateModelListDelegate(Dictionary<String, CatModel> models);
        private void UpdateModelList_u(Dictionary<String, CatModel> models) {
            modelList.Items.Clear();
            if (models != null) {
                foreach (KeyValuePair<String, Catsland.Core.CatModel> pair in models) {
                    modelList.Items.Add(pair.Key);
                }
            }
            modelList.Refresh();
        }

        [Obsolete("Don't use material anymore")]
        public void UpdateMaterialList(Dictionary<String, CatMaterial> materials) {
            modelList.BeginInvoke(
                new UpdateMaterialListDelegate(UpdateMaterialList_u), new object[] { materials });
        }

        delegate void UpdateMaterialListDelegate(Dictionary<String, CatMaterial> models);
        private void UpdateMaterialList_u(Dictionary<String, CatMaterial> materials) {
            materialList.Items.Clear();
            foreach (KeyValuePair<String, CatMaterial> pair in materials) {
                materialList.Items.Add(pair.Key);
            }
            materialList.Refresh();
        }

        // return:
        // 0 - ok
        // 1 - existed
        int AddGameObjectToPrefabs(GameObject gameObject, bool isOverride = false) {
            PrefabList scenePrefabList = Mgr<CatProject>.Singleton.prefabList;
            if (scenePrefabList.ContainKey(gameObject.Name)) {
                if (isOverride == false) {
                    return 1;
                }
                else {
                    scenePrefabList.RemoveItem(gameObject.Name);
                }
            }

            Serialable.BeginSupportingDelayBinding();
            GameObject newGameObject = gameObject.DoClone() as GameObject;
            // TODO: fix this, now the clone function can not copy value
            Serialable.EndSupportingDelayBinding();
                //gameObject.CloneGameObject();
            scenePrefabList.AddItem(newGameObject.Name, newGameObject);
            return 0;
        }


        public void UpdatePrefabList(PrefabList scenePrefabList) {
            prefabTree.Nodes.Clear();
            if (scenePrefabList != null && scenePrefabList.contentList != null) {
                foreach (KeyValuePair<string, GameObject> keyValue in scenePrefabList.contentList) {
                    TreeNode newNode = new TreeNode(keyValue.Key);
                    IterAddChildrenGameObjectToNode(keyValue.Value, newNode, false);
                    prefabTree.Nodes.Add(newNode);
                }
            }
            prefabTree.Update();
        }

        private void AddGameObjectToPrefabs_Agent(GameObject gameObject) {
            if (gameObject != null) {
                int result = AddGameObjectToPrefabs(gameObject);
                if (result == 1) // existed
                {
                    if (MessageBox.Show(gameObject.Name + " Prefab exits, overwrite?",
                        "Overwrite?", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes) {
                        AddGameObjectToPrefabs(gameObject, true);
                    }
                }
            }
        }

        public void GameObjectNameChanged() {
            UpdateGameObjectList(Mgr<Scene>.Singleton._gameObjectList);
        }



        public void ComponentUpdate(object sender, EventArgs e) {
            ((CatComponent)((PackableBox)sender).ExtraData).Initialize(Mgr<Scene>.Singleton);
        }

        public void ComponentRemove(object sender, EventArgs e) {
            CatComponent catComponent = (CatComponent)((PackableBox)sender).ExtraData;
            GameObject gameObject = catComponent.GameObject;
            gameObject.RemoveComponent(catComponent.GetType());
            UpdateGameObjectAttribute(m_observingGameObject);
        }

        public void UpdateGameObjectAttribute(GameObject gameObject) {
            attr_tab_newgo.Controls.Clear();
            m_observingGameObject = gameObject;

            if (m_observingGameObject != null) {
                PackableBox packableBox = new PackableBox();
                packableBox.Dock = DockStyle.Top;
                packableBox.AutoSize = true;
                packableBox.Text = "GameObject";
                packableBox.SetButtonVisible = false;

                PropertyGrid propertyGrid = new PropertyGrid();
                propertyGrid.SelectedObject = m_observingGameObject;
                propertyGrid.Dock = DockStyle.Top;
                propertyGrid.HelpVisible = false;

                packableBox.AddContent(propertyGrid);
                attr_tab_newgo.Controls.Add(packableBox);

                CatModelInstance modelInstance = null;

                if (m_observingGameObject.Components != null) {
                    foreach (KeyValuePair<string, CatComponent> key_value in
                    m_observingGameObject.Components) {

                        // pick out modelInstance
                        if (key_value.Value.GetType().ToString() == "Catsland.Plugin.BasicPlugin.ModelComponent") {
                            modelInstance = key_value.Value.GetModel();
                        }
                        
                        packableBox = new PackableBox();
                        packableBox.Dock = DockStyle.Top;
                        packableBox.AutoSize = true;
                        packableBox.Text = key_value.Key;
                        packableBox.ExtraData = key_value.Value;
                        packableBox.UpdateHappen += ComponentUpdate;
                        packableBox.RemoveHappen += ComponentRemove;

                        propertyGrid = new PropertyGrid();
                        propertyGrid.SelectedObject = key_value.Value;
                        propertyGrid.Text = key_value.Key;
                        propertyGrid.Dock = DockStyle.Top;
                        propertyGrid.HelpVisible = false;

                        packableBox.AddContent(propertyGrid);
                        attr_tab_newgo.Controls.Add(packableBox);
                    }
                }

                // material
                if (modelInstance != null) {
                    PropertyEditor propertyEditor = new PropertyEditor();
                    propertyEditor.SetObserve(modelInstance.GetMaterial());
                    propertyEditor.Dock = DockStyle.Top;
                    propertyEditor.AutoSize = true;
                    attr_tab_newgo.Controls.Add(propertyEditor);
                }
            }
        }

        private void float_only_KeyPress(KeyPressEventArgs e) {
            if (e.KeyChar != '\b' && e.KeyChar != 13) {
                if (((e.KeyChar < '0') || (e.KeyChar > '9'))
                    && (e.KeyChar != '.')
                    && (e.KeyChar != '-')) {
                    e.Handled = true;
                }
            }
        }

        private void positive_only_KeyPress(KeyPressEventArgs e) {
            if (e.KeyChar != '\b' && e.KeyChar != 13) {
                if ((e.KeyChar < '0') || (e.KeyChar > '9')) {
                    e.Handled = true;
                }
            }
        }

        // new gameObject
        private void toolStripButton1_Click(object sender, EventArgs e) {
            // create a gameObject in the center of screen
            GameObject newGameObject = new GameObject();
            newGameObject.Position = Vector3.Zero;
            Mgr<Scene>.Singleton._gameObjectList.AddGameObject(newGameObject);
            Mgr<Scene>.Singleton._debugDrawableList.AddItem(newGameObject);
        }

        // exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            // save the old project
            DialogResult result = MessageBox.Show("Would you like to save the current project?",
                "Save?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) {
                curProject.SaveProject(curProject.GetProjectXMLAddress());
                Mgr<Scene>.Singleton.SaveScene(curProject.GetSceneFileAddress(curProject.currentSceneName));
            }
            else if (result == DialogResult.Cancel) {
                return;
            }
            
            Application.Exit();
        }

        // insert components
        private void insertToolStripMenuItem_DropDownOpened(object sender, EventArgs e) {
            if (m_observingGameObject == null) {
                menu_component.Enabled = false;
            }
            else {
                menu_component.Enabled = true;
            }
        }

        private void modelList_MouseClick(object sender, MouseEventArgs e) {
            ShowAttrTabPage("Model");

            String modelName = (String)modelList.SelectedItem;
            if (modelName == null) {
                return;
            }
            Catsland.Core.CatModel model = Mgr<CatProject>.Singleton.modelList1.GetModel(modelName);

            if (model != null) {
                UpdateModelAttribute(model);
            }
        }

        public void UpdateModelAttribute(Catsland.Core.CatModel model) {
            m_observingModelReady = false;
            m_observingModel = model;

            if (m_observingModel != null) {
                // name
                attr_ml_name.Text = m_observingModel.GetName();

                // material
                attr_ml_mtrl.Items.Clear();
                int index = 0;
                int iter = 0;
                // new material
                foreach (KeyValuePair<string, CatMaterialTemplate> keyValue
                    in Mgr<CatProject>.Singleton.materialList1.GetMaterialTemplateList()) {

                    attr_ml_mtrl.Items.Add(keyValue.Key);
                    if (m_observingModel.GetMaterial() != null
                        && keyValue.Key == m_observingModel.GetMaterial().GetMaterialTemplate().GetName()) {
                            index = iter;
                    }
                    ++iter;
                }

                propertyBox.Controls.Clear();
                PropertyEditor propertyEditor = new PropertyEditor();
                propertyEditor.SetObserve(m_observingModel.GetMaterial());
                propertyBox.Controls.Add(propertyEditor);

//                 foreach (String mtrl in materialList.Items) {
//                     attr_ml_mtrl.Items.Add(mtrl);
//                     if (m_observingModel._material != null && 
//                         mtrl == m_observingModel._material._name) {
//                         index = iter;
//                     }
//                     ++iter;
//                 }
                if (Mgr<CatProject>.Singleton.materialList1.GetMaterialTemplateList().Count() > 0) {
                    attr_ml_mtrl.SelectedIndex = index;
                }
//                 if (m_observingModel._material != null) {
//                     m_observingModel._material.m_name = (String)attr_ml_mtrl.SelectedItem;
//                 }


                // animation and animation clips
                attr_ani_clips.Controls.Clear();
                Animation animation = m_observingModel.GetAnimation();
                if (animation != null) {
                    attr_ani_mpf.Text = "" + animation.m_millionSecondPerFrame.GetValue();
                    attr_ani_tiltwidth.Text = "" + animation.m_tiltUV.X;
                    attr_ani_tiltheight.Text = "" + animation.m_tiltUV.Y;
                    attr_ani_autoplay.Checked = animation.m_isAutoPlay;

                    index = 0;
                    iter = 0;

                    attr_ani_defaultclip.Items.Clear();
                    foreach (KeyValuePair<String, AnimationClip> pair
                        in animation.m_animationClips) {
                        if (pair.Key == animation.m_defaultAnimationClipName) {
                            index = iter;
                        }
                        attr_ani_defaultclip.Items.Add(pair.Key);
                        ++iter;

                        AnimationClipViewer animationClipViewer =
                            new AnimationClipViewer();
                        animationClipViewer.m_targetClip = pair.Value;
                        animationClipViewer.ClipName = pair.Key;
                        animationClipViewer.MinIndex = pair.Value.m_beginIndex;
                        animationClipViewer.MaxIndex = pair.Value.m_endIndex;
                        animationClipViewer.ClipMode = (int)pair.Value.m_mode;

                        animationClipViewer.ClipNameChanged +=
                            new AnimationClipViewer.ClipNameChangedEventHandler(animationClipViewer_ClipNameChanged);
                        animationClipViewer.MinIndexChanged +=
                            new AnimationClipViewer.MinIndexChangedEventHandler(animationClipViewer_MinIndexChanged);
                        animationClipViewer.MaxIndexChanged +=
                            new AnimationClipViewer.MaxIndexChangedEventHandler(animationClipViewer_MaxIndexChanged);
                        animationClipViewer.ClipModeChanged +=
                            new AnimationClipViewer.ClipModeChangedEventHandler(animationClipViewer_ClipModeChanged);
                        animationClipViewer.DeleteButtonClicked +=
                            new AnimationClipViewer.DeleteButtonClickedEventHandler(animationClipViewer_DeleteButtonClicked);

                        animationClipViewer.Dock = DockStyle.Top;
                        attr_ani_clips.Controls.Add(animationClipViewer);
                    }
                    if (attr_ani_defaultclip.Items.Count > 0) {
                        attr_ani_defaultclip.SelectedIndex = index;
                    }

                }


                // show all tab
                attr_ml_name.Visible = true;
                attr_mtl_group.Visible = true;
                attr_clips_group.Visible = true;
            }
            else {
                // hide all tab
                attr_ml_name.Visible = false;
                attr_mtl_group.Visible = false;
                attr_clips_group.Visible = false;
            }

            m_observingModelReady = true;
        }

        public void UpdatePostProcessPanel() {
            attr_postprocesses.Controls.Clear();
            Scene scene = Mgr<Scene>.Singleton;
            if(scene == null){
                return;
            }
            PostProcessManager postProcessManager = scene.PostProcessManager;
            if (postProcessManager == null) {
                return;
            }
            foreach (KeyValuePair<string, PostProcess> keyValue in
                postProcessManager.GetPostProcessDictionary()) {

                PackableBox packableBox = new PackableBox();
                packableBox.Dock = DockStyle.Top;
                packableBox.AutoSize = true;
                packableBox.Text = keyValue.Key;
                packableBox.ExtraData = keyValue.Value;

                PropertyGrid propertyGrid = new PropertyGrid();
                propertyGrid.SelectedObject = keyValue.Value;
                propertyGrid.Text = keyValue.Key;
                propertyGrid.Dock = DockStyle.Top;
                propertyGrid.HelpVisible = false;

                packableBox.AddContent(propertyGrid);
                attr_postprocesses.Controls.Add(packableBox);
            }
        }

        private void animationClipViewer_ClipModeChanged(object sender, EventArgs e) {
            AnimationClipViewer animationClipViewer =
                sender as AnimationClipViewer;
            AnimationClip animationClip =
                animationClipViewer.m_targetClip as AnimationClip;
            animationClip.m_mode = PlayModeLUT[animationClipViewer.ClipMode];
        }

        private void animationClipViewer_ClipNameChanged(object sender, EventArgs e) {
            AnimationClipViewer animationClipViewer =
                sender as AnimationClipViewer;
            AnimationClip animationClip =
                animationClipViewer.m_targetClip as AnimationClip;

            // find duplicate
            String name = animationClipViewer.ClipName;
            AnimationClip replaced = m_observingModel.GetAnimation().getAnimationClip(name);
            if (replaced == null) {
                m_observingModel.GetAnimation().m_animationClips.Remove(animationClip.m_name);
                animationClip.m_name = name;
                m_observingModel.GetAnimation().addAnimationClip(animationClip);
            }
            UpdateModelAttribute(m_observingModel);
        }

        private void animationClipViewer_MaxIndexChanged(object sender, EventArgs e) {
            AnimationClipViewer animationClipViewer =
                sender as AnimationClipViewer;
            AnimationClip animationClip =
                animationClipViewer.m_targetClip as AnimationClip;
            animationClip.m_endIndex.SetValue(animationClipViewer.MaxIndex);
        }

        private void animationClipViewer_MinIndexChanged(object sender, EventArgs e) {
            AnimationClipViewer animationClipViewer =
                sender as AnimationClipViewer;
            AnimationClip animationClip =
                animationClipViewer.m_targetClip as AnimationClip;
            animationClip.m_beginIndex.SetValue(animationClipViewer.MinIndex);
        }

        private void animationClipViewer_DeleteButtonClicked(object sender, EventArgs e) {
            AnimationClipViewer animationClipViewer =
                sender as AnimationClipViewer;
            AnimationClip animationClip =
                animationClipViewer.m_targetClip as AnimationClip;
            attr_clips_group.Controls.Remove(animationClipViewer);
            m_observingModel.GetAnimation().m_animationClips.Remove(animationClip.m_name);
            UpdateModelAttribute(m_observingModel);
        }

        private void button1_Click(object sender, EventArgs e) {
            if (m_observingModel.GetAnimation() == null) {
                m_observingModel.SetAnimation(new Animation());
            }
            int index = 1;
            String title = "untitled";
            while (m_observingModel.GetAnimation().getAnimationClip(title + index) != null) {
                ++index;
            }

            AnimationClip animationClip = new AnimationClip(title + index);
            m_observingModel.GetAnimation().addAnimationClip(animationClip);
            UpdateModelAttribute(m_observingModel);
        }

        private void attr_ani_mpf_KeyPress(object sender, KeyPressEventArgs e) {
            positive_only_KeyPress(e);
            if (e.KeyChar == 13 && m_observingGameObject != null) {
                m_observingModel.GetAnimation().MillionSecondPerFrame =
                    int.Parse(attr_ani_mpf.Text);
            }
        }

        private void attr_ani_tilt_KeyPress(object sender, KeyPressEventArgs e) {
            positive_only_KeyPress(e);
            if (e.KeyChar == 13 && m_observingModel != null) {
                m_observingModel.GetAnimation().TiltUV =
                    new Point(int.Parse(attr_ani_tiltwidth.Text),
                            int.Parse(attr_ani_tiltheight.Text));
            }
        }

        private void attr_ani_autoplay_CheckedChanged(object sender, EventArgs e) {
            m_observingModel.GetAnimation().m_isAutoPlay = attr_ani_autoplay.Checked;
        }

        private void attr_ani_defaultclip_SelectedIndexChanged(object sender, EventArgs e) {
            m_observingModel.GetAnimation().m_defaultAnimationClipName =
                (String)attr_ani_defaultclip.SelectedItem;
        }

        private void attr_ml_name_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13) {
                if (!ExecuteCommend(new RenameModelCommand(m_observingModel.GetName(), attr_ml_name.Text))) {
                    attr_ml_name.Text = m_observingModel.GetName();
                }
            }
        }

        // Create Model
        private void toolStripButton2_Click(object sender, EventArgs e) {
            // find title
            String title = "Untitled";
            int index = 1;
            CatModelList list = Mgr<CatProject>.Singleton.modelList1;
            while (list.GetModel(title + index) != null) {
                ++index;
            }

            // find material
            CatMaterialTemplate mtrl = null;
            if (Mgr<CatProject>.Singleton.materialList1.GetMaterialTemplateList() != null) {
                foreach (KeyValuePair<String, CatMaterialTemplate> pair in
                    Mgr<CatProject>.Singleton.materialList1.GetMaterialTemplateList()) {
                    mtrl = pair.Value;
                    break;
                }
            }

            CatModel newModel = new CatModel(title + index, mtrl.GetMaterialPrototype().Clone());
            list.AddModel(newModel);

            tabControl1.SelectedTab = tabPage2;

        }

        public void UpdateMaterialAttribute(CatMaterial material) {
//             m_observingMaterial = material;
//            
//             if (m_observingMaterial != null) {
//                 // show tab
//                 ShowAttrTabPage("Material");
//                 //attr_tab.SelectTab(attr_tab_material);
//                 // name
//                 attr_mtrl_name.Enabled = true;
//                 attr_mtrl_name.Text = m_observingMaterial.m_name;
//                 // texture
//                 if (m_observingMaterial._texture != null) {
//                     picbox_preview.Enabled = true;
//                     btn_textureSelector.Enabled = true;
//                     btn_textureSelector.Text = m_observingMaterial._texture.Name;
//                     FileStream fs = new FileStream(curProject.GetImageDirectory() + "\\" + m_observingMaterial._texture.Name,
//                         FileMode.Open);
//                     picbox_preview.Image = System.Drawing.Image.FromStream(fs);
//                     fs.Close();
//                 }
//                 else {
//                     picbox_preview.Image = picbox_preview.ErrorImage;
//                     btn_textureSelector.Text = "Undefined";
//                 }
// 
//                 // effect
//                 int index = 0;
//                 int iter = 0;
// 
//                 Console.WriteLine("Effect " + m_observingMaterial._effect.Name);
//                 foreach (String effect in attr_mtrl_effect.Items) {
//                     if (effect == m_observingMaterial._effect.Name) {
//                         index = iter;
//                     }
//                     ++iter;
//                 }
//                 if (attr_mtrl_effect.Items.Count > 0) {
//                     attr_mtrl_effect.SelectedIndex = index;
//                 }
// 
//                 attr_tab_material.Refresh();
//             }
        }

        private void materialList_MouseClick(object sender, MouseEventArgs e) {
            if (materialList.SelectedItem == null) {
                return;
            }
            
            ExecuteCommend(new SetObservingMaterialCommand((string)materialList.SelectedItem));
        }

        private void attr_mtrl_name_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13) {
                if (m_observingMaterial != null
                    && !ExecuteCommend(new RenameMaterialCommand(m_observingMaterial.GetName(), attr_mtrl_name.Text))) {
                    attr_mtrl_name.Text = m_observingMaterial.GetName();
                }
            }
        }
        
        // TODO: add effect
        
        bool SaveScene(String filename) {
            // scene
            Mgr<Scene>.Singleton.SaveScene(filename);
            return true;
        }

        public bool OpenScene(String filename) {
            // create new scene a load
//             Scene newScene = Scene.LoadScene(filename);
//             newScene.InitializeScene();
//             Scene oldScene = Mgr<Scene>.Singleton;
//             if (oldScene != null) {
//                 oldScene.Unload();
//             }

            Mgr<GameEngine>.Singleton.DoSwitchScene(filename);
            
            //newScene.ActivateScene();
            //pg_scene.SelectedObject = newScene;
            //attr_camera.SelectedObject = Mgr<Camera>.Singleton;
            //UpdatePostProcessPanel();
            return true;
        }

        public void LoadSceneComplete() {
            pg_scene.SelectedObject = Mgr<Scene>.Singleton;
            attr_camera.SelectedObject = Mgr<Camera>.Singleton;
            UpdatePostProcessPanel();
        }
        /*
        private void saveSceneToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Catland Map|*.xml";
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                String filename = saveFileDialog.FileName;
                SaveScene(filename);
            }
        }
         * */

        private void playScene() {
            SaveScene(m_tmp_filename);
            m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Playing;
            btn_play.Enabled = false;
            btn_stop.Enabled = true;
            btn_pause.Enabled = true;
        }

        private void stopScene() {
            m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
            m_btTreeEditor.ObserveLiveBTTree(null);
            OpenScene(m_tmp_filename);
            btn_play.Enabled = true;
            btn_stop.Enabled = false;
            btn_pause.Enabled = false;
            attr_tab_newgo.Controls.Clear();
        }

        private void pauseScene() {
            if (m_gameEngine._gameInEditorMode == GameEngine.InEditorMode.Playing) {
                m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
            }
            else {
                m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Playing;
            }
        }

        private void loadSceneToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Catsland Map|*.xml";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                String filename = openFileDialog.FileName;
                OpenScene(filename);
                m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
                btn_play.Enabled = true;
                btn_stop.Enabled = false;
                btn_pause.Enabled = false;
                Mgr<Camera>.Singleton.Reset();
                attr_camera.SelectedObject = Mgr<Camera>.Singleton;
                UpdatePostProcessPanel();
                attr_camera.Visible = true;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) {
            // find title
            String title = "Untitled";
            int index = 1;
            CatMaterialTemplateList list = Mgr<CatProject>.Singleton.materialList1;
            while (list.GetMaterialPrototype(title + index) != null) {
                ++index;
            }

            // combine
            
            //list.GetMaterialPrototype(material);

            // show material list and attribute panel
            
            tabControl1.SelectedTab = tabPage5;
            attr_tab.SelectedTab = attr_tab_material;
            //ExecuteCommend(new SetObservingMaterialCommand(material.m_name));
        }

        private void attr_ml_mtrl_SelectedIndexChanged(object sender, EventArgs e) {
            if (m_observingModelReady) {
                String materialName = (String)(attr_ml_mtrl.SelectedItem);

                CatMaterialTemplate materialTempalte =
                    Mgr<CatProject>.Singleton.materialList1.GetMaterialPrototype(materialName).GetMaterialTemplate();
                m_observingModel.SetMaterialToTemplate(materialTempalte);
                
//                 CatMaterial material = m_observingModel.GetMaterial();
//                 if (material != null) {
//                     material.ChangeMaterialTemplate(materialTempalte);
//                 }
//                 else {
//                     m_observingModel.
//                 }
                UpdateModelAttribute(m_observingModel);
                //m_observingModel._material = Mgr<CatProject>.Singleton.materialList1.GetMaterial(materialName);
            }
               
        }

       

        private void button2_Click(object sender, EventArgs e) {
            Mgr<Camera>.Singleton.TargetObject = m_observingGameObject;
        }


        private void button4_Click(object sender, EventArgs e) {
            //Mgr<Scene>.Singleton.scenePlayer = m_observingGameObject;
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            deleteGameObjectToolStripMenuItem.Enabled = (m_observingGameObject != null);
            addGameObjectToPrefabToolStripMenuItem.Enabled = (m_observingGameObject != null);
            deleteProfabToolStripMenuItem.Enabled = (prefabTree.SelectedNode != null);
        }

        private void deleteGameObjectToolStripMenuItem_Click(object sender, EventArgs e) {
            if (m_observingGameObject != null) {
                Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(m_observingGameObject.GUID);
                m_observingGameObject = null;
                UpdateGameObjectAttribute(null);
            }

        }

        private void addGameObjectToPrefabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (m_observingGameObject != null) {
                AddGameObjectToPrefabs_Agent(m_observingGameObject);
            }
        }

        private void Insert_GameObject_From_Prefab(string prefabName) {
            PrefabList scenePrefabList = Mgr<CatProject>.Singleton.prefabList;
            if (scenePrefabList.ContainKey(prefabName)) {
                GameObject prefab = scenePrefabList.GetItem(prefabName);
                Serialable.BeginSupportingDelayBinding();
                GameObject gameObject = prefab.DoClone() as GameObject;
                Serialable.EndSupportingDelayBinding();
                    //prefab.CloneGameObject();
                //gameObject.Initialize(Mgr<Scene>.Singleton);
                Mgr<Scene>.Singleton._gameObjectList.AddGameObject(gameObject);
            }
            else {
                Console.Out.WriteLine("Could not find prefab named: " + prefabName);
            }
        }

        private void deleteProfabToolStripMenuItem_Click(object sender, EventArgs e) {
            string prefabName = (string)prefabTree.SelectedNode.Text;
            if (prefabName != null) {
                Mgr<CatProject>.Singleton.prefabList.RemoveItem(prefabName);
            }
        }

        private void btn_play_Click(object sender, EventArgs e) {
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer2.IsSplitterFixed = true;
            playScene();
        }

        private void btn_stop_Click(object sender, EventArgs e) {
            stopScene();
            this.splitContainer1.IsSplitterFixed = false;
            this.splitContainer2.IsSplitterFixed = false;
        }

        private void btn_pause_Click(object sender, EventArgs e) {
            this.splitContainer1.IsSplitterFixed = false;
            this.splitContainer2.IsSplitterFixed = false;
            pauseScene();
        }

        private void gameObjectTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            // show editor gameObject panel
            ShowAttrTabPage("GameObject");
            string guid = (string)(e.Node.Tag);
            if (guid == null) {
                return;
            }
            GameObject selected = Mgr<Scene>.Singleton._gameObjectList.GetItem(guid);
            if (selected != null) {
                UpdateGameObjectAttribute(selected);
            }
        }

        private void gameObjectTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            // focus camera on the gameObject
            if (Mgr<GameEngine>.Singleton._gameInEditorMode == GameEngine.InEditorMode.Editing) {
                string guid = (string)(e.Node.Tag);
                GameObject selected = Mgr<Scene>.Singleton._gameObjectList.GetItem(guid);
                if (selected != null) {
                    Mgr<Camera>.Singleton.TargetObject = selected;
                }
            }
        }

        private void prefabTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            string prefabName = (string)(e.Node.Text);
            Insert_GameObject_From_Prefab(prefabName);
        }

        private void gameObjectTree_ItemDrag(object sender, ItemDragEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void gameObjectTree_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode")) {
                e.Effect = DragDropEffects.Move;
            }
            else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void gameObjectTree_DragDrop(object sender, DragEventArgs e) {
            TreeNode moveNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            // find target node according to position
            System.Drawing.Point pt = ((TreeView)(sender)).PointToClient(new System.Drawing.Point(e.X, e.Y));
            TreeNode targetNode = gameObjectTree.GetNodeAt(pt);
            // attach to parent
            string moveGuid = (string)(moveNode.Tag);
            string targetGuid = (string)(targetNode.Tag);
            Mgr<Scene>.Singleton._gameObjectList.GetItem(moveGuid).AttachToGameObject(
                Mgr<Scene>.Singleton._gameObjectList.GetItem(targetGuid));
            targetNode.Expand();   
        }

        private void gameObjectTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e != null && e.Label != null) {
                string guid = (string)(e.Node.Tag);
                Mgr<Scene>.Singleton._gameObjectList.GetItem(guid).Name = e.Label;
            }
        }

        private void menu_component_Click(object sender, EventArgs e) {

        }

        private void gameObjectFromPrefabToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void prefabTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            // show editor gameObject panel
           
            //attr_tab.SelectTab(attr_tab_newgo);
            string selected = (string)(e.Node.Text);
            GameObject selectedPrefab = Mgr<CatProject>.Singleton.prefabList.GetItem(selected);
            if (selectedPrefab != null) {
                UpdateGameObjectAttribute(selectedPrefab);
            }
        }

        public void GameEngineStarted() {
            PostInitializeEditor();
        }

        private void renderArea_MouseClick(object sender, MouseEventArgs e) {
            if (Mgr<Scene>.Singleton != null) {
                GameObject selected = Mgr<Scene>.Singleton.GetSelectedGameObject(
                2.0f * (float)(e.X) / renderArea.Width - 1.0f,
                1.0f - 2.0f * (float)(e.Y) / renderArea.Height);
                if (selected != null) {
                    ShowAttrTabPage("GameObject");
                    UpdateGameObjectAttribute(selected);
                }
                else {
                    UpdateGameObjectAttribute(null);
                }
            }
        }

        public GameObject GetSelectedGameObject() {
            return m_observingGameObject;
        }

        private Vector2 getCameraCoordinate(int x, int y) {
            return new Vector2(2.0f * (float)(x) / renderArea.Width - 1.0f,
                1.0f - 2.0f * (float)(y) / renderArea.Height);
        }

        private void renderArea_MouseDown(object sender, MouseEventArgs e) {
            Vector2 cameraCoordinate = getCameraCoordinate(e.X, e.Y);
            if (m_observingGameObject != null) {
                bool isObjectStillSelect = Mgr<Scene>.Singleton.IsGameObjectSelected(cameraCoordinate.X,
                    cameraCoordinate.Y, m_observingGameObject);
                GameObject selected = m_observingGameObject;
                if (isObjectStillSelect) {
                    
                    if (isCtrlDown) {
                        Serialable.BeginSupportingDelayBinding();
                        GameObject newGameObject = selected.DoClone() 
                            as GameObject;//selected.CloneGameObject();
                        Serialable.EndSupportingDelayBinding();
                        Mgr<Scene>.Singleton._gameObjectList.AddGameObject(newGameObject);
                        newGameObject.AttachToGameObject(selected.Parent);
                        selected = newGameObject;
                        m_observingGameObject = newGameObject;
                    }

                    // if mouse down on the observing object, move it
                    movingGameObjectOriginalPosition = m_observingGameObject.Position;
                    Vector3 worldPos = Vector3.Zero;
                    if (curMovingMode == MovingMode.XY) {
                        Mgr<Camera>.Singleton.CameraToWorldXY(cameraCoordinate.X, cameraCoordinate.Y, movingGameObjectOriginalPosition.Z, out worldPos);
                        
                    }
                    else if(curMovingMode == MovingMode.XZ){
                        Mgr<Camera>.Singleton.CameraToWorldXZ(cameraCoordinate.X, cameraCoordinate.Y, movingGameObjectOriginalPosition.Y, out worldPos);
                    }
                    mouseDownPositionInWorld = worldPos;

                    isMouseDown = true;
                }
            }
        }

        private void renderArea_MouseMove(object sender, MouseEventArgs e) {
            if (isMouseDown) {
                Vector2 cameraCoordinate = getCameraCoordinate(e.X, e.Y);
                Vector3 worldPos = Vector3.Zero;
                if (curMovingMode == MovingMode.XY) {
                    Mgr<Camera>.Singleton.CameraToWorldXY(cameraCoordinate.X, cameraCoordinate.Y, movingGameObjectOriginalPosition.Z, out worldPos);
                }
                else {
                    Mgr<Camera>.Singleton.CameraToWorldXZ(cameraCoordinate.X, cameraCoordinate.Y, movingGameObjectOriginalPosition.Y, out worldPos);
                }

                Vector4 targetPosition = new Vector4(worldPos, 1.0f);
                if (m_observingGameObject.Parent != null) {
                    // map to space of parent object
                    Matrix invModelMatrix = Matrix.Invert(m_observingGameObject.Parent.AbsTransform);
                    targetPosition = Vector4.Transform(targetPosition, invModelMatrix);
                }
                m_observingGameObject.Position = new Vector3(targetPosition.X, targetPosition.Y, targetPosition.Z);

            }
        }

        private void renderArea_MouseUp(object sender, MouseEventArgs e) {
            isMouseDown = false;
        }

        private void zToolStripMenuItem_Click(object sender, EventArgs e) {
            curMovingMode = MovingMode.XZ;
            movingAxis.Text = "Moving Axis: XZ";
        }

        private void xYToolStripMenuItem_Click(object sender, EventArgs e) {
            curMovingMode = MovingMode.XY;
            movingAxis.Text = "Moving Axis: XY";
        }

        private void movingAxis_ButtonClick(object sender, EventArgs e) {

        }

        private void MapEditor_KeyDown(object sender, KeyEventArgs e) {
            isCtrlDown = e.Control;
        }

        private void MapEditor_KeyUp(object sender, KeyEventArgs e) {
            isCtrlDown = e.Control;
        }

        private void assistXYheight_KeyPress(object sender, KeyPressEventArgs e) {
            float_only_KeyPress(e);
            if (e.KeyChar == 13) {
                observingXYheight = float.Parse(assistXYheight.Text);
            }
        }

        public float GetObservingXYHeight() {
            return observingXYheight;
        }

        private void quadRenderGameObjectToolStripMenuItem_Click(object sender, EventArgs e) {
            
        }

        private void gameObjectTree_AfterExpand(object sender, TreeViewEventArgs e) {
            string guid = (string)(e.Node.Tag);
            if (guid != null) {
                GameObject gameObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(guid);
                if (gameObject != null) {
                    gameObject.isExpend = true;
                }
            }   
        }

        private void gameObjectTree_AfterCollapse(object sender, TreeViewEventArgs e) {
            string guid = (string)(e.Node.Tag);
            if (guid != null) {
                GameObject gameObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(guid);
                if (gameObject != null) {
                    gameObject.isExpend = false;
                }
            }
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            // save the old project
            DialogResult result = MessageBox.Show("Would you like to save the current project?",
                "Save?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) {
                curProject.SaveProject(curProject.GetProjectXMLAddress());
                Mgr<Scene>.Singleton.SaveScene(curProject.projectRoot + CatProject.RESOURCE_DIR + "/scene/" + curProject.currentSceneName + ".scene");
            }
            else if (result == DialogResult.Cancel) {
                return;
            }

            ExecuteCommend(new NewProjectCommand());
            
        }

        public bool ExecuteCommend(ICommand _commend) {
            return _commend.Execute(this);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            // save the old project
            DialogResult result = MessageBox.Show("Would you like to save the current project?",
                "Save?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) {
                curProject.SaveProject(curProject.GetProjectXMLAddress());
                Mgr<Scene>.Singleton.SaveScene(curProject.projectRoot + CatProject.RESOURCE_DIR + "/scene/" + curProject.currentSceneName + ".scene");
            }
            else if (result == DialogResult.Cancel) {
                return;
            }

            ExecuteCommend(new OpenProjectCommand());
        }

        private void scenesToolStripMenuItem_Click(object sender, EventArgs e) {
            SceneSelection sceneSelection = new SceneSelection();
            sceneSelection.InitializeData(curProject);
            sceneSelection.ShowDialog(this);

            if (sceneSelection.DialogResult == DialogResult.OK) {
                if (sceneSelection.SceneToLoad != curProject.currentSceneName) {
                    DialogResult result = MessageBox.Show("Would you like to save " + curProject.currentSceneName + " first?",
                        "Save?", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes) {
                        Mgr<Scene>.Singleton.SaveScene(curProject.projectRoot + CatProject.RESOURCE_DIR + "/scene/" + curProject.currentSceneName + ".scene");
                    }
                    else if (result == DialogResult.Cancel) {
                        return;
                    }
                    OpenScene(curProject.GetSceneFileAddress(sceneSelection.SceneToLoad));
                    curProject.currentSceneName = sceneSelection.SceneToLoad;

                    m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
                    btn_play.Enabled = true;
                    btn_stop.Enabled = false;
                    btn_pause.Enabled = false;
                    Mgr<Camera>.Singleton.Reset();

                    UpdateEditorWindowFrame();
                }
            }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            // save project
            curProject.SaveProject(curProject.GetProjectXMLAddress());
            // save scene
            Mgr<Scene>.Singleton.SaveScene(curProject.projectRoot + CatProject.RESOURCE_DIR + "/scene/" + curProject.currentSceneName + ".scene");
        }

        public void UpdateEditorWindowFrame() {
            string basic = "CatsEngine Editor";
            if (curProject != null) {
                this.Text = curProject.currentSceneName + " - " + basic;
            }
            else {
                this.Text = basic;
            }
        }

        public void BindToScene(Scene scene) {
            Mgr<Scene>.Singleton = scene;
            pg_scene.SelectedObject = scene;
            attr_camera.SelectedObject = Mgr<Camera>.Singleton;
            m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
            UpdateGameObjectList(scene._gameObjectList);
            btn_play.Enabled = true;
            btn_stop.Enabled = false;
            btn_pause.Enabled = false;
            Mgr<Camera>.Singleton.Reset();
            updateInsertComponentMenu();
            updateEditorScriptMenu();
            UpdateEditorWindowFrame();
        }

        private void resolution_label_Click(object sender, EventArgs e) {

        }

        private void splitContainer2_Panel1_Resize(object sender, EventArgs e) {
            resizeRenderArea();
        }

        private void resizeRenderArea() {
            if (widthDivHight <= 1e-5f) {
                renderArea.Dock = DockStyle.Fill;
            }
            else {
                renderArea.Dock = DockStyle.None;
                // set layout
                int fullWidth = splitContainer2.Panel1.Width;
                int fullHeight = splitContainer2.Panel1.Height;
                float fullWidthDivHeight = (float)fullWidth / fullHeight;
                if (fullWidthDivHeight > widthDivHight) {
                    // so height is the critical
                    renderArea.Height = fullHeight;
                    renderArea.Width = (int)(fullHeight * widthDivHight);
                    renderArea.Left = (fullWidth - renderArea.Width) / 2;
                    renderArea.Top = 0;
                }
                else {
                    // width is the critical
                    renderArea.Width = fullWidth;
                    renderArea.Height = (int)(fullWidth / widthDivHight);
                    renderArea.Left = 0;
                    renderArea.Top = (fullHeight - renderArea.Height) / 2;
                }
            }
            resolution_size_label.Text = "" + renderArea.Width + " x " + renderArea.Height;
        }

        private void resolution_selector_SelectedIndexChanged(object sender, EventArgs e) {
            string selected = (string)(resolution_selector.SelectedItem);
            if (selected != "No restrict") {
                string[] raw = selected.Split(':');
                int width = int.Parse(raw[0]);
                int height = int.Parse(raw[1]);
                widthDivHight = (float)width / height;
            }
            else {
                widthDivHight = 0.0f;
            }
            resizeRenderArea();
            
        }

        private void rescanPluginToolStripMenuItem_Click(object sender, EventArgs e) {
            Mgr<TypeManager>.Singleton.Load_Plugins(curProject.projectRoot + CatProject.PLUGIN_DIR + '/');
            Mgr<TypeManager>.Singleton.LoadConsoleCommands(curProject.projectRoot + CatProject.PLUGIN_DIR + '/');
            Mgr<TypeManager>.Singleton.Load_EditorScripts(curProject.projectRoot + CatProject.PLUGIN_DIR + '/');
            Mgr<TypeManager>.Singleton.LoadBTTreeNodes(curProject.projectRoot + CatProject.PLUGIN_DIR + '/');
            updateEditorScriptMenu();
            updateInsertComponentMenu();
        }

        private void btn_textureSelector_Click(object sender, EventArgs e) {
            // select texture for curProject
            Debug.Assert(false, "This function has beent Deprecate");

//             if (m_observingMaterial != null) {
//                 OpenFileDialog openFileDialog = new OpenFileDialog();
//                 openFileDialog.Filter = "Image Files|*.png";
//                 openFileDialog.FilterIndex = 1;
//                 openFileDialog.InitialDirectory = curProject.GetImageDirectory();
//                 bool valid = false;
//                 while (!valid) {
//                     if (openFileDialog.ShowDialog() == DialogResult.OK) {
//                         // check if the file is under image directory
//                         if (openFileDialog.FileName.StartsWith(curProject.GetImageDirectory())) {
//                             valid = true;
//                         }
//                         else {
//                             MessageBox.Show("Images should be under Resource Directory of the project " + curProject.GetImageDirectory(),
//                                 "Error Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                         }
//                     }
//                     else {
//                         break;
//                     }
//                 }
//                 if (valid){
//                     FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open); 
//                     m_observingMaterial._texture
//                         = Texture2D.FromStream(Mgr<GraphicsDevice>.Singleton, fs);
//                     //Material.PreMultiplyAlphas(m_observingMaterial._texture);
//                     string textureName = openFileDialog.FileName.Substring(curProject.GetImageDirectory().Length + 1);
//                     btn_textureSelector.Text = textureName;
//                     m_observingMaterial._texture.Name = textureName;
//                     picbox_preview.Image = System.Drawing.Image.FromStream(fs);
//                     fs.Close();
// 
//                 }
//             }
        }

        private void publishProjectToolStripMenuItem_Click(object sender, EventArgs e) {
            // publish project
            // ask for publish path
             SaveFileDialog saveFileDialog = new SaveFileDialog();
             if (saveFileDialog.ShowDialog() != DialogResult.OK) {
                 return;
             }

             curProject.PublishProject(saveFileDialog.FileName);

            //TODO: move this to project
            /*
            // create folder
             string filepath = saveFileDialog.FileName;
             if (!System.IO.Directory.Exists(filepath)) {
                System.IO.Directory.CreateDirectory(filepath);
             }
            // copy proejct files
            System.IO.File.Copy(curProject.projectRoot + "\\" + "project.xml", filepath + "\\" + "project.xml");
            // copy plugin
            copyFolder(curProject.projectRoot + "\\" + CatProject.PLUGIN_DIR, filepath + "\\" + CatProject.PLUGIN_DIR);
            // copy scene
            copyFolder(curProject.projectRoot + "\\" + CatProject.SCENE_DIR, filepath + "\\" + CatProject.SCENE_DIR);
            // copy game engine
            System.IO.File.Copy("Core.dll", filepath + "\\" + "Core.dll");
            System.IO.File.Copy("GameEntrance.exe", filepath + "\\" + "GameEntrance.exe");

            // compile resource
            compileResources(curProject.projectRoot, filepath);
             * */
            
        }

        private void compileResources(string _sourceDirectory, string _outputDirectory){
            // modify output project file
            XmlDocument template = new XmlDocument();
            template.Load("compileResource.proj");
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(template.NameTable);
            nsMgr.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlNode outputPathNode = template.SelectSingleNode("/ns:Project/ns:PropertyGroup/ns:OutputPath", nsMgr);
            outputPathNode.InnerText = _outputDirectory;
            // save to project root
            template.Save(_sourceDirectory + "\\" + CatProject.RESOURCE_DIR + "\\" + CatProject.RESOURCE_DIR + ".proj");

            // run msbuild
            Process msbuild = new Process();
            msbuild.StartInfo.FileName =  Environment.ExpandEnvironmentVariables("%SystemRoot%") + @"\microsoft.NET\Framework\v4.0.30319\msbuild.exe";
            msbuild.StartInfo.Arguments = _sourceDirectory + "\\" + CatProject.RESOURCE_DIR + "\\" + CatProject.RESOURCE_DIR + ".proj";
            msbuild.Start();
        }

        private void copyFolder(string _from, string _to){
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

        private void attr_mtrl_name_Leave(object sender, EventArgs e) {
            if(!ExecuteCommend(new RenameMaterialCommand(m_observingMaterial.GetName(), attr_mtrl_name.Text))){
                attr_mtrl_name.Text = m_observingMaterial.GetName();
            }
        }

        private void attr_ml_name_Leave(object sender, EventArgs e) {
            if (!ExecuteCommend(new RenameModelCommand(m_observingModel.GetName(), attr_ml_name.Text))) {
                attr_ml_name.Text = m_observingModel.GetName();
            }
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowAttrTabPage("Camera");
        }

        private void renderArea_Paint(object sender, PaintEventArgs e) {
            return;
        }

        private void renderArea_MouseEnter(object sender, EventArgs e) {
            renderAreaGetFocus(sender, e);
        }

        private void renderArea_MouseLeave(object sender, EventArgs e) {
            renderAreaLostFocus(sender, e);
        }

        private void modelList_SelectedIndexChanged(object sender, EventArgs e) {

        }

        public void AdjustSelectedGameObjectPoistion(Vector2 _amount) {
            if (m_observingGameObject != null && Mgr<Camera>.Singleton != null) {
                float ratio = Mgr<Camera>.Singleton.ViewSize.X / 10000.0f;
                m_observingGameObject.Position = 
                    new Vector3(m_observingGameObject.Position.X + _amount.X * ratio,
                                m_observingGameObject.Position.Y + _amount.Y * ratio, 
                                0.0f);
            }
        }

        private void bTTreeEditorToolStripMenuItem_Click(object sender, EventArgs e) {
            BTTreeEditor.Show(this);
            BTTreeEditor.Focus();
        }

        private void MapEditor_FormClosing(object sender, FormClosingEventArgs e) {
            if (m_btTreeEditor != null && !m_btTreeEditor.IsDisposed) {
                m_btTreeEditor.Close();
                m_btTreeEditor.Dispose();
            }
            Application.Exit();
        }
    }

}
