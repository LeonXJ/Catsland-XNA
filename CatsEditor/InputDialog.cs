using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CatsEditor {
    public partial class InputDialog : Form {

        string result;
        public string Result {
            get {
                return result;
            }
        }
        
        public InputDialog() {
            InitializeComponent();
        }

        public void InitializeDate(string _title, string _tip, string _value) {
            Text = _title;
            tipBox.Text = _tip;
            valueBox.Text = _value;
        }

        private void button2_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e) {
            result = valueBox.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
