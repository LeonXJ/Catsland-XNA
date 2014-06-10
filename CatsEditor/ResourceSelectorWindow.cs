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

namespace Catsland.CatsEditor {
    public partial class ResourceSelectorWindow : Form {
        public enum ObserveType {
            Texture = 0,
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


        // called by user function
        public void SetType(ObserveType _type) {
            typeSelector.SelectedIndex = (int)_type;
        }

        public void CanTypeModify(bool _can) {
            typeSelector.Enabled = _can;
        }

        public void UpdateList() {
            string strType = (string)(typeSelector.SelectedItem);
            if (strType == ObserveType.Texture.ToString()) {
                UpdateTextureList(Mgr<CatProject>.Singleton.projectRoot + "\\asset\\resource\\image");
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

        private void UpdateTextureList(string _directory) {
            contentList.Items.Clear();
            string[] files = Directory.GetFiles(_directory, "*.xnb");
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
    }
}
