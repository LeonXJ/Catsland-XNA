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
    public partial class ComponentSelector : Form {
        private string result = "";
        public string Result {
            get { return result; }
        }
        public ComponentSelector() {
            InitializeComponent();
        }

        public void InitializeData(GameObject gameObject, string selected) {
            component_list.Items.Clear();
            Dictionary<string, CatComponent> componentList = gameObject.GetComponents();
            int selectedIndex = -1;
            if (componentList != null) {
                foreach (KeyValuePair<string, CatComponent> key_value in componentList) {
                    component_list.Items.Add(key_value.Key);
                    if (key_value.Key == selected) {
                        selectedIndex = component_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1) {
                component_list.SelectedIndex = selectedIndex;
            }
            else if (component_list.Items.Count > 0) {
                component_list.SelectedIndex = 1;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e) {
            result = (string)component_list.SelectedItem;
            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }
    }

    public class PropertyGridComponentSelector : UITypeEditor {
        //Button button = new Button();

        public PropertyGridComponentSelector() { }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
            try {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null) {
                    string selected = "";
                    if (value is string) {
                        selected = (string)value;
                    }

                    ComponentSelector componentSelector = new ComponentSelector();
                    componentSelector.InitializeData(((CatComponent)context.Instance).m_gameObject, (string)selected);
                    edSvc.ShowDialog(componentSelector);

                    // get model
                    if (componentSelector.DialogResult == DialogResult.OK) {
                        return componentSelector.Result;
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
