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
                }
            }
        }

        private void btTreeViewer_OnBTNodeDeselected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            propertyEditor.SelectedObject = null;
            m_selectedNode = null;
            menuInsert.Enabled = false;
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
    }
}
