using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Catsland.Core;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Catsland.Editor;

namespace Catsland.CatsEditor {
    public partial class ResourceSelectorWindow : Form {
        public enum ObserveType {
            Texture = 0,
            Model,
            BTTree,
            All,
        };
        string m_selectedString;

        public ResourceSelectorWindow() {
            InitializeComponent();
            // other initialize
            typeSelector.Items.Clear();
            for (int i = 0; i <= (int)ObserveType.All; ++i) {
                typeSelector.Items.Add(((ObserveType)i).ToString());
            }
        }

        private void InitializeMenu(ObserveType _type) {
            if (_type == ObserveType.BTTree) {
                InitializeMenuForBTTree();
            }
        }

        private void InitializeMenuForBTTree() {
            toolNew.Enabled = true;
            toolNew.Click += OnNewBTNodeClick;
            toolEdit.Enabled = true;
            toolEdit.Click += OnEditBTNodeClick;
        }

        public void OnNewBTNodeClick(object sender, EventArgs e) {
            if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.BTTreeManager != null) {
                Mgr<CatProject>.Singleton.BTTreeManager.CreateAndSaveEmptyBTTree();
                Mgr<CatProject>.Singleton.BTTreeManager.SaveAllBTTree(Mgr<CatProject>.Singleton.GetBTTreeWriteDirectory());
                Mgr<CatProject>.Singleton.SynchronizeBTTrees();
                UpdateList();
            }
        }

        public void OnEditBTNodeClick(object sender, EventArgs e) {
            if (Mgr<CatProject>.Singleton != null && Mgr<CatProject>.Singleton.BTTreeManager != null) {
                string selected = (string)(contentList.SelectedItem);
                if (selected != null) {
                    BTTree btTree = Mgr<CatProject>.Singleton.BTTreeManager.LoadBTTree(selected);
                    Mgr<MapEditor>.Singleton.BTTreeEditor.OpenBTTree(btTree);
                    Mgr<MapEditor>.Singleton.BTTreeEditor.ShowDialog(this);
                }
            }
        }

        public static string SelectResource(ObserveType _resourceType, 
            string _defaultResourceName = "", 
            IWin32Window _owner = null) {
            ResourceSelectorWindow resourceSelector = new ResourceSelectorWindow();
            resourceSelector.SetType(_resourceType);
            resourceSelector.CanTypeModify(false);
            resourceSelector.UpdateList();
            resourceSelector.Select(_defaultResourceName);
            resourceSelector.ShowDialog(_owner);

            if (resourceSelector.DialogResult == DialogResult.OK) {
                return resourceSelector.GetSelectedString();
            }
            else {
                return "";
            }
        }

        // called by user function
        public void SetType(ObserveType _type) {
            typeSelector.SelectedIndex = (int)_type;
            InitializeMenu(_type);
        }

        public void CanTypeModify(bool _can) {
            typeSelector.Enabled = _can;
        }

        public void UpdateList() {
            string strType = (string)(typeSelector.SelectedItem);
            if (strType == ObserveType.Texture.ToString()) {
                UpdateListBySuffix(Mgr<CatProject>.Singleton.projectRoot + "\\asset\\resource\\image", "xnb");
            }
            else if (strType == ObserveType.Model.ToString()) {
                UpdateListBySuffix(Mgr<CatProject>.Singleton.projectRoot + "\\asset\\resource\\model", "model");
            }
            else if (strType == ObserveType.BTTree.ToString()) {
                UpdateListBySuffix(Mgr<CatProject>.Singleton.projectRoot + "\\asset\\resource\\ai", "btt");
            }
        }

        public void Select(string _name) {
            int selectedIndex = -1;
            int iter = 0;
            foreach (string item in contentList.Items) {
                if (_name == item) {
                    selectedIndex = iter;
                }
                ++iter;
            }
            if (selectedIndex >= 0) {
                contentList.SelectedIndex = selectedIndex;
            }
        }

        public string GetSelectedString() {
            return m_selectedString;
        }

        private void UpdateListBySuffix(string _directory, string _surfix) {
            contentList.Items.Clear();
            string[] files = Directory.GetFiles(_directory, "*." + _surfix);
            foreach (string file in files) {
                // get file name without extension
                int iBegin = file.LastIndexOf('\\');
                int iEnd = file.LastIndexOf('.');
                string name = file.Substring(iBegin + 1, iEnd - iBegin - 1);
                // filter
                if (FilterOut(name)) {
                    continue;
                }
                // add
                contentList.Items.Add(name);
            }
        }

        private bool FilterOut(string _name) {
            string reg = filterBox.Text;
            if (reg == "") {
                return false;
            }
            return (_name.IndexOf(reg) < 0);
        }

        private void button1_Click(object sender, EventArgs e) {
            filterBox.Text = "";
        }

        private void filterBox_TextChanged(object sender, EventArgs e) {
            UpdateList();
        }

        private void contentList_DoubleClick(object sender, EventArgs e) {
            string selected = (string)(contentList.SelectedItem);
            if (selected != null) {
                m_selectedString = selected;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void ResourceSelectorWindow_Load(object sender, EventArgs e) {

        }

        private void toolNew_Click(object sender, EventArgs e) {

        }
    }

    public class PropertyGridBTTreeSelector : UITypeEditor {

        public PropertyGridBTTreeSelector() { }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
            try {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null) {
                    string btName = ResourceSelectorWindow.SelectResource(ResourceSelectorWindow.ObserveType.BTTree, value as string);
                    if (btName != "") {
                        return btName;
                    }
                    else {
                        return value;
                    }
                }
            }
            catch (System.Exception ex) {
                Console.Out.WriteLine("" + ex);
            }
            return value;
        }
    }
}
