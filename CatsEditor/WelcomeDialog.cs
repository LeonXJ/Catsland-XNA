using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.CatsEditor.EditorCommand;


namespace Catsland.Editor {
    public partial class WelcomeDialog : Form {
        MapEditor mapEditor = null;
        public WelcomeDialog(MapEditor _mapEditor) {
            mapEditor = _mapEditor;
            InitializeComponent();
        }

        private void btnNewProject_Click(object sender, EventArgs e) {
            if (mapEditor.ExecuteCommend(new NewProjectCommand())) {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnOpenProject_Click(object sender, EventArgs e) {
            if (mapEditor.ExecuteCommend(new OpenProjectCommand())) {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
