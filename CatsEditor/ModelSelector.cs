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

namespace Catsland.Editor
{
    public partial class ModelSelector : Form
    {
        private string result = "";
        public string Result
        {
            get { return result; }
        }

        public ModelSelector()
        {
            InitializeComponent();
        }

        public void InitializeData(string selected)
        {
            model_list.Items.Clear();
            Dictionary<string, CatModel> modelList = Mgr<CatProject>.Singleton.modelList1.GetList();
            int selectedIndex = -1;
            if (model_list != null && modelList != null)
            {
                foreach (KeyValuePair<string, CatModel> key_value in modelList)
                {
                    model_list.Items.Add(key_value.Key);
                    if (key_value.Key == selected)
                    {
                        selectedIndex = model_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1)
            {
                model_list.SelectedIndex = selectedIndex;
            }
            else if (model_list.Items.Count > 0)
            {
                model_list.SelectedIndex = 0;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            result = (string)model_list.SelectedItem;
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        private void ModelSelector_Load(object sender, EventArgs e) {

        }
    }

    public class PropertyGridModelSelector : UITypeEditor
    {
        //Button button = new Button();

        public PropertyGridModelSelector()
        { }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    ModelSelector modelSelector = new ModelSelector();
                    string initName = "";
                    if (value is CatModel)
                    {
                        initName = ((CatModel)value).GetName();
                    }
                    modelSelector.InitializeData(initName);

                    edSvc.ShowDialog(modelSelector);

                    if (modelSelector.DialogResult == DialogResult.OK)
                    {
                        // get model
                        return Mgr<CatProject>.Singleton.modelList1.GetModel(modelSelector.Result);
                    }
                    else
                    {
                        return value;
                    }  
                }
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine("" + ex);
            }
            return value;
        }
    }
}
