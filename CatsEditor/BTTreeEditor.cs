using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;

namespace Catsland.Editor {
    public partial class BTTreeEditor : Form {
        public BTTreeEditor() {
            InitializeComponent();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        public void OpenBTTree(BTTree _btTree) {
            btTreeViewer.SetBTTree(_btTree);
        }

        /**
         * @brief A BTNode is clicked
         **/ 
        private void btTreeViewer_OnBTNodeSelected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            if (e.BTNode != null) {
                propertyEditor.SelectedObject = e.BTNode;
            }
        }

        private void btTreeViewer_OnBTNodeDeselected(object sender, MapEditorControlLibrary.BTNodeSelectedArgs e) {
            propertyEditor.SelectedObject = null;
        }
    }
}
