using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;
using System.Diagnostics;
using System.Reflection;
using CatsEditor;
using Catsland.CatsEditor;

namespace Catsland.Editor {
    public partial class BTTreeEditor : Form {

#region Properties

        private BTNode m_selectedNode;
        private BTTreeRuntimePack m_observingRuntimePack;
        private Dictionary<string, int> m_blackboardMap;

#endregion

        public BTTreeEditor() {
            InitializeComponent();
        }

        /**
         * @brief open BTTree
         **/
        public void OpenBTTree(BTTree _btTree) {
            InitializeAll();
            btTreeViewer.SetBTTree(_btTree);
        }

        /**
         * @brief open BTTree and observe RuntimePack
         **/ 
        public void ObserveLiveBTTree(BTTreeRuntimePack _runtimePack) {
            InitializeAll();
            BindRuntimePack(_runtimePack);
            btTreeViewer.SetBTTreeAndObservingRuntimePack(_runtimePack);
        }

        /**
         * @brief if the given runtimePack is being observed.
         *  Note that if _runtimePack == null and no runtimePack is observing, 
         *  the result is true
         * */
        public bool IsObservingThisRuntimePack(BTTreeRuntimePack _runtimePack) {
            return m_observingRuntimePack == _runtimePack;
        }

        /**
         * @brief [Called by BTTreeComponent only] impulse to show runtimePack
         **/
        public void UpdateBTTreeEditor() {
            if (Visible) {
                UpdateBlackboard();
                btTreeViewer.Refresh();
            }
        }

        private void InitializeAll() {
            InitializeMenu();
            InitializeForm();
        }

        private void InitializeForm() {
            m_selectedNode = null;
            m_observingRuntimePack = null;
            m_blackboardMap = null;
        }

        private void InitializeMenu() {
            menuInsert.Enabled = false;
            menuRemoveNode.Enabled = false;
            InitializeInsertMenu();
        }

        private void InitializeInsertMenu() {
            // clear
            menuInsertCompositeNode.DropDownItems.Clear();
            menuInsertConditionNode.DropDownItems.Clear();
            menuInsertActionNode.DropDownItems.Clear();
            // create
            if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.typeManager != null) {
                Dictionary<string, Type> bttreeNodes = Mgr<CatProject>.Singleton.typeManager.BTTreeNodes;
                foreach (KeyValuePair<string, Type> keyValue in bttreeNodes) {
                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = keyValue.Key;
                    item.Click += ExecuteAddTreeNode;
                    item.Tag = keyValue.Key;
                    if (keyValue.Value.IsSubclassOf(typeof(BTCompositeNode))) {
                        menuInsertCompositeNode.DropDownItems.Add(item);
                    }
                    else if (keyValue.Value.IsSubclassOf(typeof(BTConditionNode))) {
                        menuInsertConditionNode.DropDownItems.Add(item);
                    }
                    else if (keyValue.Value.IsSubclassOf(typeof(BTActionNode))) {
                        menuInsertActionNode.DropDownItems.Add(item);
                    }
                }
            }
        }

        /**
         * @brief [Only called by insert menu item] create and insert the node as child node
         **/ 
        private void ExecuteAddTreeNode(object sender, EventArgs e) {
            if (m_selectedNode != null) {
                string nodeName = (string)((ToolStripMenuItem)sender).Tag;
                if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.typeManager != null) {
                    Type type = Mgr<CatProject>.Singleton.typeManager.GetBTTreeNodeType(nodeName);
                    if (type != null) {
                        ConstructorInfo constructor = type.GetConstructor(new Type[0]{});
                        BTNode btNode = constructor.Invoke(new object[0] { }) as BTNode;
                        if (m_selectedNode.GetType().IsSubclassOf(typeof(BTCompositeNode))) {
                            (m_selectedNode as BTCompositeNode).AddChild(btNode);
                        }
                        else if (m_selectedNode.GetType().IsSubclassOf(typeof(BTConditionNode))) {
                            (m_selectedNode as BTConditionNode).Child = btNode;
                        }
                        else {
                            Debug.Assert(false, "Trying to insert node to illegal parent.");
                        }
                        btTreeViewer.DeclareAddNode(btNode);
                    }
                }
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        /**
         * @brief A BTNode is selected
         **/ 
        private void btTreeViewer_OnBTNodeSelected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            if (e.BTNode != null) {
                propertyEditor.SelectedObject = e.BTNode;
                m_selectedNode = e.BTNode;
                UpdateMenu();
            }
        }

        /**
         * @brief A BTNode is deselected
         **/ 
        private void btTreeViewer_OnBTNodeDeselected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            propertyEditor.SelectedObject = null;
            m_selectedNode = null;
            UpdateMenu();
        }

        /**
         * @brief set the menu according to current m_selectedNode
         **/ 
        private void UpdateMenu() {
            if (m_selectedNode != null) {
                if (m_selectedNode.CanAddMoreChild()) {
                    menuInsert.Enabled = true;
                }
                else {
                    menuInsert.Enabled = false;
                }
                if (m_selectedNode != btTreeViewer.BTTree.Root) {
                    // cannot remove root
                    menuRemoveNode.Enabled = true;
                }
                else {
                    menuRemoveNode.Enabled = false;
                }
            }
            else {
                menuInsert.Enabled = false;
                menuRemoveNode.Enabled = false;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e) {
            
        }

        /**
         * @brief set the runtime pack to blackboard
         **/ 
        private void BindRuntimePack(BTTreeRuntimePack _runtimePack) {
            m_observingRuntimePack = _runtimePack;
            blackboard.Rows.Clear();
            m_blackboardMap = new Dictionary<string, int>();
        }

        private void UpdateBlackboard() {
            if (!Visible) {
                return;
            }
            if (m_observingRuntimePack.Blackboard == null) {
                return;
            }
            foreach (KeyValuePair<string, object> keyValue in m_observingRuntimePack.Blackboard) {
                string key = keyValue.Key;
                if (!m_blackboardMap.ContainsKey(key)) {
                    int index = blackboard.RowCount;
                    blackboard.Rows.Add(new object[2]{key, ""});
                    m_blackboardMap.Add(key, index-1);
                }
                string valueString = "";
                if (keyValue.Value.GetType().GetInterface(typeof(IEffectParameter).ToString()) != null) {
                    valueString = (keyValue.Value as IEffectParameter).ToValueString();
                }
                else {
                    valueString = keyValue.Value.ToString();
                }
                int i = m_blackboardMap[key];
                if(blackboard.RowCount > i){
                    blackboard.Rows[i].Cells[1].Value = valueString;
                } 
            }
        }

        /**
         * @brief not actually close, we just hide it. we close it along with the editor
         **/ 
        private void BTTreeEditor_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }

        /**
         * @brief create new tree command
         **/ 
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            CreateNewBTTree();
        }
        private void CreateNewBTTree() {
            string treeName = "UntitledBTTree";
            InputDialog inputDialog = new InputDialog();
            inputDialog.InitializeDate("Input a name for BTTree", "Input a name for BTTree", treeName);
            if (inputDialog.ShowDialog(this) == DialogResult.OK) {
                treeName = inputDialog.Result;
                if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.BTTreeManager != null) {
                    while (Mgr<CatProject>.Singleton.BTTreeManager.HasBTTreeWithName(treeName)) {
                        inputDialog = new InputDialog();
                        inputDialog.InitializeDate("Tree " + treeName + "exists", "Input a new name for BTTree", treeName);
                        if (inputDialog.ShowDialog(this) == DialogResult.OK) {
                            treeName = inputDialog.Result;
                        }
                        else {
                            return;
                        }
                    }
                    BTTree btTree = Mgr<CatProject>.Singleton.BTTreeManager.CreateAndSaveEmptyBTTree(treeName);
                    OpenBTTree(btTree);
                }
                else {
                    Debug.Assert(false, "Cannot find CatProject instance.");
                    return;
                }
            }
            else {
                return;
            }
        }

        /**
         * @brief load tree command
         **/
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            string treeName = ResourceSelectorWindow.SelectResource(ResourceSelectorWindow.ObserveType.BTTree, "", this);
            if (treeName != "") {
                LoadBTTree(treeName);
            }
        }
        private void LoadBTTree(string _btTreeName) {
            if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.BTTreeManager != null) {
                BTTree btTree = Mgr<CatProject>.Singleton.BTTreeManager.LoadBTTree(_btTreeName);
                if (btTree != null) {
                    OpenBTTree(btTree);
                }
                else {
                    Debug.Assert(false, "Cannot open BTTree: " + _btTreeName);
                }
            }
        }

        /**
         * @brief remove node command
         **/ 
        private void menuRemoveNode_Click(object sender, EventArgs e) {
            if (m_selectedNode != null) {
                btTreeViewer.BTTree.RemoveSubTree(m_selectedNode);
                btTreeViewer.DeclareRemoveNode(m_selectedNode);
                btTreeViewer_OnBTNodeDeselected(null, null);
            }
        }

        /**
         * @brief synchronize command
         **/ 
        private void synchronizeAllEditToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.BTTreeManager != null) {
                Mgr<CatProject>.Singleton.BTTreeManager.SaveAllBTTree();
                Mgr<CatProject>.Singleton.SynchronizeBTTrees();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Hide();
        }
    }
}