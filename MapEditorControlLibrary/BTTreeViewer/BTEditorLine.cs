﻿using System;
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

//         protected Point m_parentPoint;
//         protected Point m_childPoint;
//         public Point ParentPoint {
//             set {
//                 m_parentPoint = value;
//             }
//             get {
//                 return m_parentPoint;
//             }
//         }
//         public Point ChildPoint {
//             set {
//                 m_childPoint = value;
//             }
//             get {
//                 return m_childPoint;
//             }
//         }


        #endregion

        public BTEditorLine(BTTreeViewer _treeViewer)
            :base(_treeViewer) {
        }

        public string GetKey() {
            return GetKey(m_parentNode, m_childNode);
        }

        public static string GetKey(BTNode _parent, BTNode _child) {
            if (_parent != null && _child != null) {
                return _parent.GUID + _child.GUID;
            }
            else {
                Debug.Assert(false, "The nodes have not been set.");
                return "";
            }
        }

        public override void OnPaint(PaintEventArgs e) {     
            if (m_parentNode == null || m_childNode == null) {
                return;
            }
            Graphics gc = e.Graphics;
            Point parentPoint = m_treeViewer.GetRectangle(m_parentNode).GetChildPoint();
            Point childPoint = m_treeViewer.GetRectangle(m_childNode).GetParentPoint();
            gc.DrawLine(Pens.Black, m_treeViewer.GetDrawPosition(parentPoint), 
                m_treeViewer.GetDrawPosition(childPoint));
        }
    }
}
