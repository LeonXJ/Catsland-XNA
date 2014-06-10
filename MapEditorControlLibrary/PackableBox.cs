using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditorControlLibrary
{
    public partial class PackableBox : UserControl
    {
        private string title = "";
        private object extra_data;
        public object ExtraData
        {
            get { return extra_data; }
            set { extra_data = value; }
        }

        public string Text
        {
            set { 
                title = value;
                UpdateTitle();
            }
            get { return title; }
        }

        private void UpdateTitle()
        {
            if (container.Visible)
                {
                    label1.Text = " - " + title;
                }
                else
                {
                    label1.Text = " + " + title;
                }
        }

        public bool SetButtonVisible
        {
            get { return btn_update.Visible; }
            set 
            { 
                btn_update.Visible = value;
                btn_remove.Visible = value;
            }
        }

        public PackableBox()
        {
            InitializeComponent();
        }

        public void AddContent(Control content)
        {
            content.Dock = DockStyle.Top;
            container.Controls.Add(content);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            container.Visible = !container.Visible;
            UpdateTitle();
        }

        public delegate void UpdateEventHandler(object sender, EventArgs e);
        public event UpdateEventHandler UpdateHappen;
        protected virtual void OnUpdateHappen(EventArgs e)
        {
            if (UpdateHappen != null)
            {
                UpdateHappen(this, e);
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            OnUpdateHappen(e);
        }

        public delegate void RemoveEventHandler(object sender, EventArgs e);
        public event RemoveEventHandler RemoveHappen;
        protected virtual void OnRemoveHappen(EventArgs e)
        {
            if (RemoveHappen != null)
            {
                RemoveHappen(this, e);
            }
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            OnRemoveHappen(e);
        }
    }
}
