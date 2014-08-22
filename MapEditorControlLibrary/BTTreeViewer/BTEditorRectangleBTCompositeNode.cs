﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {
    public class BTEditorRectangleBTCompositeNode : BTEditorRectangle {

        public override BTEditorRectangle Clone() {
            return new BTEditorRectangleBTCompositeNode();
        }

        protected override bool IsThisType(BTNode _btNode) {
            return _btNode.GetType().IsSubclassOf(typeof(BTCompositeNode));
        }

        protected override void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) {
            if (m_node != null) {
                BTCompositeNode node = m_node as BTCompositeNode;
                if (node.Children != null) {
                    foreach (BTNode child in node.Children) {
                        // create link
                        BTEditorLine line = new BTEditorLine();
                        line.ParentNode = m_node;
                        line.ChildNode = child;
                        _sprites.Add(line.GetKey(), line);
                        // do for children
                        RecursivelyCreateSprites(_sprites, child);
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
                // lines
                if (node.Children != null) {
                    foreach (BTNode child in node.Children) {
                        string keyChild = GetKey(child);
                        string keyLine = BTEditorLine.GetKey(m_node, child);
                        if (_sprites.ContainsKey(keyLine) && _sprites.ContainsKey(keyChild)) {
                            BTEditorLine line = _sprites[keyLine] as BTEditorLine;
                            BTEditorRectangle childNode = _sprites[keyChild] as BTEditorRectangle;
                            line.ParentPoint = GetChildPoint();
                            line.ChildPoint = childNode.GetParentPoint();
                        }
                        else {
                            Debug.Assert(false, "Cannot find line or node sprite");
                        }
                    }
                }
                return needHeight;
            }
            else {
                Debug.Assert(false, "Cannot find corresponding node");
                return 0;
            }
        }

        public override void OnPaint(PaintEventArgs e) {
            Graphics gc = e.Graphics;
            gc.FillRectangle(Brushes.LightSeaGreen, m_bound);
            gc.DrawRectangle(Pens.Black, m_bound);
        }
    
    }
}