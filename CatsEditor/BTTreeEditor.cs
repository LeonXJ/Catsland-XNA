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

namespace Catsland.Editor {
    public partial class BTTreeEditor : Form {

#region Properties

        private BTNode m_selectedNode;

#endregion

        public BTTreeEditor() {
            InitializeComponent();
        }

        private void InitializeAttribute() {
            m_selectedNode = null;
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

        public void OpenBTTree(BTTree _btTree) {
            btTreeViewer.SetBTTree(_btTree);
            InitializeMenu();
            InitializeAttribute();
        }

        /**
         * @brief A BTNode is clicked
         **/ 
        private void btTreeViewer_OnBTNodeSelected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            if (e.BTNode != null) {
                propertyEditor.SelectedObject = e.BTNode;
                m_selectedNode = e.BTNode;
                if (CanBeParentNode(e.BTNode)) {
                    menuInsert.Enabled = true;
                    if (m_selectedNode != btTreeViewer.BTTree.Root) {
                        // cannot remove root
                        menuRemoveNode.Enabled = true;
                    }
                }
            }
        }

        private void btTreeViewer_OnBTNodeDeselected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            propertyEditor.SelectedObject = null;
            m_selectedNode = null;
            menuInsert.Enabled = false;
            menuRemoveNode.Enabled = false;
        }

        private bool CanBeParentNode(BTNode _node) {
            if(_node == null){
                return false;
            }
            if (_node.GetType().IsSubclassOf(typeof(BTCompositeNode))) {
                return true;
            }
            else if (_node.GetType().IsSubclassOf(typeof(BTConditionNode)) &&
                (_node as BTConditionNode).Child == null) {
                    return true;
            }
            return false;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (m_selectedNode != null) {
                btTreeViewer.BTTree.RemoveSubTree(m_selectedNode);
                btTreeViewer.DeclareRemoveNode(m_selectedNode);
                btTreeViewer_OnBTNodeDeselected(null, null);
            }
        }

        private BTTreeRuntimePack m_observingRuntimePack;
        public bool IsObservingThisRuntimePack(BTTreeRuntimePack _runtimePack) {
            return m_observingRuntimePack == _runtimePack;
        }
        private Dictionary<string, int> m_blackboardMap;

        public void ObserveLiveBTTree(BTTreeRuntimePack _runtimePack) {
            if (_runtimePack == null) {
                OpenBTTree(null);
            }
            else {
                OpenBTTree(_runtimePack.BTTree);
            }
            BindRuntimePack(_runtimePack);
            btTreeViewer.SetObservingRuntimePack(_runtimePack);
        }

        public void UpdateBTTreeViewer() {
            btTreeViewer.Refresh();
        }

        private void BindRuntimePack(BTTreeRuntimePack _runtimePack) {
            m_observingRuntimePack = _runtimePack;
            blackboard.Rows.Clear();
            m_blackboardMap = new Dictionary<string, int>();
        }

        public void UpdateBlackboard() {
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
    }
}
