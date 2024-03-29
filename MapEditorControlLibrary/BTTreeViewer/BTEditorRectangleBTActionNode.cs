﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {
    public class BTEditorRectangleBTActionNode : BTEditorRectangle {

        #region Properties

        protected static Brush NodeColor = new SolidBrush(Color.FromArgb(244, 204, 189));
        protected static Brush SelectedColor = new SolidBrush(Color.FromArgb(76, 65, 54));

        #endregion

        internal BTEditorRectangleBTActionNode(BTTreeViewer _treeViewer)
            : base(_treeViewer) {
        }

        internal override BTEditorRectangle Clone(BTTreeViewer _treeViewer) {
            return new BTEditorRectangleBTActionNode(_treeViewer);
        }

        protected override bool IsThisType(BTNode _btNode) {
            return _btNode.GetType().IsSubclassOf(typeof(BTActionNode));
        }

        protected override void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) {
            return;
        }

        internal override int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop) {
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

        internal override void OnPaint(PaintEventArgs e) {
            Graphics gc = e.Graphics;
            DefaultPaintProcess(gc, NodeColor, SelectedColor);
        }
    }
}
