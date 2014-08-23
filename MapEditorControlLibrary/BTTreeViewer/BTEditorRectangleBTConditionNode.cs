using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {
    public class BTEditorRectangleBTConditionNode : BTEditorRectangle {

        public BTEditorRectangleBTConditionNode(BTTreeViewer _treeViewer) 
            :base(_treeViewer) {

        }

        public override BTEditorRectangle Clone(BTTreeViewer _treeViewer) {
            return new BTEditorRectangleBTConditionNode(_treeViewer);
        }

        protected override bool IsThisType(BTNode _btNode) {
            return _btNode.GetType().IsSubclassOf(typeof(BTConditionNode));
        }

        protected override void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) {
            if (m_node != null) {
                BTConditionNode node = m_node as BTConditionNode;
                if (node.Child != null) {
                    // create link
                    BTEditorLine line = new BTEditorLine(m_treeViewer);
                    line.ParentNode = m_node;
                    line.ChildNode = node.Child;
                    _sprites.Add(line.GetKey(), line);
                    // do for children
                    RecursivelyCreateSprites(_sprites, node.Child, m_treeViewer);
                }
            }
        }

        public override int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop) {
            if (m_node != null) {
                // children
                BTConditionNode node = m_node as BTConditionNode;
                int x = _leftTop.X + m_bound.Width + HorizontalInterval;
                int y = _leftTop.Y;
                if (node.Child != null) {
                    BTNode child = node.Child;
                    string key = GetKey(child);
                    if (_sprites.ContainsKey(key)) {
                        BTEditorRectangle childNode = _sprites[key] as BTEditorRectangle;
                        int height = childNode.AutoRecursivelyLayout(_sprites, new Point(x, y));
                        y += height + VerticalInterval;
                    }
                    else {
                        Debug.Assert(false, "Cannot find sprite for node");
                    }
                }
                // self
                if (y != _leftTop.Y) {
                    y -= VerticalInterval;
                }
                int needHeight = (m_bound.Height > (y - _leftTop.Y)) ? (m_bound.Height) : (y - _leftTop.Y);
                m_bound.X = _leftTop.X;
                m_bound.Y = _leftTop.Y + (needHeight - m_bound.Height) / 2;
                return needHeight;
            }
            else {
                Debug.Assert(false, "Cannot find corresponding node");
                return 0;
            }
        }

        public override void OnPaint(PaintEventArgs e) {
            Graphics gc = e.Graphics;
            gc.FillRectangle(Brushes.BlueViolet, m_bound);
            gc.DrawRectangle(Pens.Black, m_bound);
        }
    }
}
