using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {
    public class BTEditorRectangleBTCompositeNode : BTEditorRectangle {

        public BTEditorRectangleBTCompositeNode(BTTreeViewer _treeViewer)
            : base(_treeViewer) {

        }

        public override BTEditorRectangle Clone(BTTreeViewer _treeViewer) {
            return new BTEditorRectangleBTCompositeNode(_treeViewer);
        }

        protected override bool IsThisType(BTNode _btNode) {
            return _btNode.GetType().IsSubclassOf(typeof(BTCompositeNode));
        }

        protected override void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) {
            if (m_node != null) {
                BTCompositeNode node = m_node as BTCompositeNode;
                if (node.Children != null) {
                    foreach (BTNode child in node.Children) {
                        string lineKey = BTEditorLine.GetKey(m_node, child);
                        if (!_sprites.ContainsKey(lineKey)) {
                            // create link
                            BTEditorLine line = new BTEditorLine(m_treeViewer);
                            line.ParentNode = m_node;
                            line.ChildNode = child;
                            _sprites.Add(line.GetKey(), line);
                        }
                        // do for children
                        RecursivelyCreateSprites(_sprites, child, m_treeViewer);
                    }
                }
            }
        }

        public override int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop) {
            if (m_node != null) {
                // children
                BTCompositeNode node = m_node as BTCompositeNode;
                int x = _leftTop.X + m_bound.Width + HorizontalInterval;
                int y = _leftTop.Y;
                if (node.Children != null) {
                    foreach (BTNode child in node.Children) {
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
            Pen edge = Pens.Black;
            if (m_isSelected) {
                edge = Pens.Red;
            }
            DeclareRightBottom();
            Rectangle rect = GetDrawBound();
            gc.FillRectangle(Brushes.LightSeaGreen, rect);
            gc.DrawRectangle(edge, rect);
            DrawStringCentreAlign(m_node.GetType().Name, gc, Brushes.Black);
        }

        public override void OnDragOn(Point _pos, BTEditorSprite _source) {
            if (_source.GetType().IsSubclassOf(typeof(BTEditorRectangle))) {
                m_treeViewer.SetParent((_source as BTEditorRectangle).GetKey(), GetKey());
            }
        }

        public void AdjustChildrenSequence(BTEditorRectangle _rectangle, Point _worldPos) {
            BTCompositeNode node = m_node as BTCompositeNode;
            if (node.Children != null) {
                node.Children.Remove(_rectangle.Node);
                int targetIndex = 0;
                foreach (BTNode child in node.Children) {
                    if (m_treeViewer.GetRectangle(child).GetPosition().Y > 
                        _worldPos.Y) {
                        break;
                    }
                    ++targetIndex;
                }
                node.Children.Insert(targetIndex, _rectangle.Node);   
            }
        }

    }
}
