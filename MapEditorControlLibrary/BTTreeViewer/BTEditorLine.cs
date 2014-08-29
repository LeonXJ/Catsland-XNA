using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Catsland.Core;
using System.Drawing;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {

    public class BTEditorLine : BTEditorSprite {

        #region Properties

        protected static int TrailPenWidth = 5;
        protected static Pen FalseDrawPen = new Pen(FalseFillColor, TrailPenWidth);
        protected static Pen TrueDrawPen = new Pen(TrueFillColor, TrailPenWidth);

        protected BTNode m_parentNode;
        protected BTNode m_childNode;
        public BTNode ParentNode {
            set {
                m_parentNode = value;
            }
            get {
                return m_parentNode;
            }
        }
        public BTNode ChildNode {
            set {
                m_childNode = value;
            }
            get {
                return m_childNode;
            }
        }

        #endregion

        internal BTEditorLine(BTTreeViewer _treeViewer)
            : base(_treeViewer) {
        }

        internal string GetKey() {
            return GetKey(m_parentNode, m_childNode);
        }

        internal static string GetKey(BTNode _parent, BTNode _child) {
            if (_parent != null && _child != null) {
                return _parent.GUID + _child.GUID;
            }
            else {
                Debug.Assert(false, "The nodes have not been set.");
                return "";
            }
        }

        internal override void OnPaint(PaintEventArgs e) {
            if (m_parentNode == null || m_childNode == null) {
                return;
            }
            Graphics gc = e.Graphics;
            Point parentPoint = m_treeViewer.GetRectangle(m_parentNode).GetChildPoint();
            Point childPoint = m_treeViewer.GetRectangle(m_childNode).GetParentPoint();
            if (m_treeViewer.IsObservingRuntimePack()) {
                BTTreeRuntimePack.RuntimeState parentState = m_treeViewer.GetRuntimeState(m_parentNode);
                BTTreeRuntimePack.RuntimeState childState = m_treeViewer.GetRuntimeState(m_childNode);
                if (parentState != BTTreeRuntimePack.RuntimeState.Norun &&
                    childState != BTTreeRuntimePack.RuntimeState.Norun) {
                    Pen pen = TrueDrawPen;
                    if (parentState == BTTreeRuntimePack.RuntimeState.False &&
                        childState == BTTreeRuntimePack.RuntimeState.False) {
                        pen = FalseDrawPen;
                    }
                    gc.DrawLine(pen, m_treeViewer.GetDrawPosition(parentPoint),
                        m_treeViewer.GetDrawPosition(childPoint));
                }
            }
            gc.DrawLine(Pens.Black, m_treeViewer.GetDrawPosition(parentPoint),
            m_treeViewer.GetDrawPosition(childPoint));
        }
    }
}
