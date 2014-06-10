using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace CatsEditor {
    public partial class PrefabSelector : Form {

        private string result = "";
        public string Result {
            get { return result; }
        }
        
        public PrefabSelector() {
            InitializeComponent();
        }

        public void InitializeData(string selected) {
            prefab_list.Items.Clear();
            Dictionary<string, GameObject> prefabList = Mgr<CatProject>.Singleton.prefabList.GetList();
            int selectedIndex = -1;
            if (prefabList != null) {
                foreach (KeyValuePair<string, GameObject> key_value in prefabList) {
                    prefab_list.Items.Add(key_value.Key);
                    if (key_value.Key == selected) {
                        selectedIndex = prefab_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1) {
                prefab_list.SelectedIndex = selectedIndex;
            }
            else if (prefab_list.Items.Count > 0) {
                prefab_list.SelectedIndex = 1;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_ok_Click(object sender, EventArgs e) {
            result = (string)prefab_list.SelectedItem;
            this.DialogResult = DialogResult.OK;
        }
    }

    public class PropertyGridPrefabSelector : UITypeEditor {
        public PropertyGridPrefabSelector() {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            try {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null) {
                    string selected = "";
                    if (value is string) {
                        selected = (string)value;
                    }
                    PrefabSelector prefabSelector = new PrefabSelector();
                    prefabSelector.InitializeData(selected);
                    edSvc.ShowDialog(prefabSelector);
                    if (prefabSelector.DialogResult == DialogResult.OK) {
                        return prefabSelector.Result;
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
