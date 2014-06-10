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
    public partial class AnimationSelector : Form {
        private string result = "";
        public string Result {
            get { return result; }
        }

        public AnimationSelector() {
            InitializeComponent();
        }

        public void InitializeData(CatModel model, string selected) {
            animation_list.Items.Clear();
            Dictionary<string, AnimationClip> clipList = model.GetAnimation().AnimationClips;
            int selectedIndex = -1;
            if (animation_list != null) {
                foreach (KeyValuePair<string, AnimationClip> key_value in clipList) {
                    animation_list.Items.Add(key_value.Key);
                    if (key_value.Key == selected) {
                        selectedIndex = animation_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1) {
                animation_list.SelectedIndex = selectedIndex;
            }
            else if (animation_list.Items.Count > 0) {
                animation_list.SelectedIndex = 0;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e) {
            result = (string)animation_list.SelectedItem;
            this.DialogResult = DialogResult.OK;
        }

        private void btn_cancel_Click(object sender, EventArgs e) {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }

    }

    public class PropertyGridAnimationSelector : UITypeEditor {
        //Button button = new Button();

        public PropertyGridAnimationSelector() { }

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

                    AnimationSelector animationSelector = new AnimationSelector();
                    

                    animationSelector.InitializeData(((CatComponent)context.Instance).GetModel().GetModel(), (string)value);
                    edSvc.ShowDialog(animationSelector);

                    // get model
                    if (animationSelector.DialogResult == DialogResult.OK) {
                        return animationSelector.Result;
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
