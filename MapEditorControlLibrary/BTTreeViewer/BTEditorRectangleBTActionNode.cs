using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {
    public class BTEditorRectangleBTActionNode : BTEditorRectangle{

        public BTEditorRectangleBTActionNode(BTTreeViewer _treeViewer)
            : base(_treeViewer) {
        }

        public override BTEditorRectangle Clone(BTTreeViewer _treeViewer) {
            return new BTEditorRectangleBTActionNode(_treeViewer);
        }

        protected override bool IsThisType(BTNode _btNode) {
            return _btNode.GetType().IsSubclassOf(typeof(BTActionNode));
        }

        protected override void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) {
            return;
        }

        public override int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop) {
            if (m_node != null) {
                //BTActionNode node = m_node as BTActionNode;
                m_bound.X = _leftTop.X;
                m_bound.Y = _leftTop.Y;
                return m_bound.Height;
            }
            else {
                Debug.Assert(false, "Cannot find corresponding node");
                return 0;
            }
        }

        public override void OnPaint(PaintEventArgs e) {
            Graphics gc = e.Graphics;
            gc.FillRectangle(Brushes.OrangeRed, m_bound);
            gc.DrawRectangle(Pens.Black, m_bound);
        }
    }
}
