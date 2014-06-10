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

namespace Catsland.Core
{
    public partial class GameObjectSelector : Form
    {
        private string result = "";
        public string Result
        {
            get { return result; }
        }

        public GameObjectSelector()
        {
            InitializeComponent();
        }

        public void InitializeData(string selected)
        {
            gameObject_list.Items.Clear();
            Dictionary<string, GameObject> gameObjectList = Mgr<Scene>.Singleton._gameObjectList.GetList();
            int selectedIndex = -1;
            if (gameObjectList != null)
            {
                foreach (KeyValuePair<string, GameObject> key_value in gameObjectList)
                {
                    gameObject_list.Items.Add(key_value.Value.Name + '|' + key_value.Key);
                    if (key_value.Key == selected)
                    {
                        selectedIndex = gameObject_list.Items.Count - 1;
                    }
                }
            }
            if (selectedIndex > -1)
            {
                gameObject_list.SelectedIndex = selectedIndex;
            }
            else if (gameObject_list.Items.Count > 0)
            {
                gameObject_list.SelectedIndex = 1;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            result = "";
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            result = (string)gameObject_list.SelectedItem;
            result = result.Split('|')[1];
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }
    }

    public class PropertyGridGameObjectSelector : UITypeEditor
    {
        //Button button = new Button();

        public PropertyGridGameObjectSelector()
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
                    string selected = "";
                    if (value is GameObject)
                    {
                        selected = ((GameObject)value).GUID;
                    }

                    GameObjectSelector gameObjectSelector = new GameObjectSelector();
                    gameObjectSelector.InitializeData(selected);
                    edSvc.ShowDialog(gameObjectSelector);

                    if (gameObjectSelector.DialogResult == DialogResult.OK)
                    {
                        // get model
                    return Mgr<Scene>.Singleton._gameObjectList.GetItem(gameObjectSelector.Result);
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
