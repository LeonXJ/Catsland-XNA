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

namespace Catsland.Editor {
    public partial class MaterialSelector : Form {
        private string result = "";
        public string Result {
            get { return result; }
        }

        public MaterialSelector() {
            InitializeComponent();
        }

        public void InitializeData(string selected) {
            material_list.Items.Clear();
            Dictionary<string, CatMaterial> materialList = Mgr<CatProject>.Singleton.materialList1.GetList();
            int selectedIndex = -1;
            if (material_list != null) {
                foreach (KeyValuePair<string, CatMaterial> key_value in materialList) {
                    material_list.Items.Add(key_value.Key);
                    if (key_value.Key == selected) {
                        selectedIndex = material_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1) {
                material_list.SelectedIndex = selectedIndex;
            }
            else if (material_list.Items.Count > 0) {
                material_list.SelectedIndex = 1;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_ok_Click(object sender, EventArgs e) {
            result = (string)material_list.SelectedItem;
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }
    }

    public class PropertyGridMaterialSelector : UITypeEditor {
        //Button button = new Button();

        public PropertyGridMaterialSelector() { }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
//             try {
//                 IWindowsFormsEditorService edSvc =
//                     (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
//                 if (edSvc != null) {
//                     string selected = "";
//                     if (value is Material) {
//                         selected = ((Material)value).m_name;
//                     }
//                     MaterialSelector materialSelector = new MaterialSelector();
//                     materialSelector.InitializeData(selected);
//                     edSvc.ShowDialog(materialSelector);
// 
//                     if (materialSelector.DialogResult == DialogResult.OK) {
//                         // get model
//                         return Mgr<CatProject>.Singleton.materialList1.GetMaterialPrototype(materialSelector.Result);
//                     }
//                     else {
//                         return value;
//                     }
//                 }
//             }
//             catch (System.Exception ex) {
//                 Console.Out.WriteLine("" + ex);
//             }
//            return value;
            return null;
        }
    }
}
