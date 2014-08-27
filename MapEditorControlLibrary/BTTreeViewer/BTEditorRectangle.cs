﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Catsland.Core;
using System.Drawing;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {

    public class BTEditorRectangle : BTEditorSprite {

        #region Properties

        public static int HorizontalInterval = 20;
        public static int VerticalInterval = 7;
        protected static Brush FalseFillBrush = new SolidBrush(Color.FromArgb(149,68,68));
        protected static Brush TrueFillBrush = new SolidBrush(Color.FromArgb(78,148,68));
        protected static Brush NonSelectedBrush = new SolidBrush(Color.FromArgb(94,94,94));
        protected static int runStateBorderWidth = 3;
        protected static int selectedBarHeight = 5;

        private static Brush DefaultNodeColor = new SolidBrush(Color.FromArgb(180,180,180));
        private static Brush DefaultSelectedColor = new SolidBrush(Color.FromArgb(10,10,10));

        private static List<BTEditorRectangle> nodePrototype;
        protected BTNode m_node;
        public BTNode Node {
            set {
                m_node = value;
            }
            get {
                return m_node;
            }
        }
        protected Rectangle m_bound = new Rectangle(0, 0, 100, 30);
        public Point GetPosition() {
            return m_bound.Location;
        }

        protected bool m_isSelected = false;


        #endregion

        public BTEditorRectangle(BTTreeViewer _treeViewer)
            :base(_treeViewer) {
            m_treeViewer = _treeViewer;
        }

        public static void InitializePrototypes() {
            nodePrototype = new List<BTEditorRectangle>();
            nodePrototype.Add(new BTEditorRectangleBTCompositeNode(null));
            nodePrototype.Add(new BTEditorRectangleBTConditionNode(null));
            nodePrototype.Add(new BTEditorRectangleBTActionNode(null));
        }

        public virtual BTEditorRectangle Clone(BTTreeViewer _treeViewer) { return new BTEditorRectangle(_treeViewer); }

        protected virtual bool IsThisType(BTNode _btNode) {
            return false;
        }

        protected static BTEditorRectangle GetRectangleNodeFromBTNode(BTNode _btNode, BTTreeViewer _treeViewer) {
            if (nodePrototype != null) {
                foreach (BTEditorRectangle prototype in nodePrototype) {
                    if (prototype.IsThisType(_btNode)) {
                        return prototype.Clone(_treeViewer);
                    }
                }

            }
            Debug.Assert(false, "Cannot find rectangle for type: " + _btNode.GetType().Name);
            return null;
        }

        public string GetKey() {
            return GetKey(m_node);
        }

        public static string GetKey(BTNode _node) {
            if (_node != null) {
                return _node.GUID;
            }
            else {
                Debug.Assert(false, "node has not been set");
                return "";
            }
        }

        protected virtual void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites) { }

        public static void RecursivelyCreateSprites(Dictionary<string, BTEditorSprite> _sprites, BTNode _btNode, BTTreeViewer _btTreeViewer) {
            if (_btNode == null) {
                return;
            }
            if (!_sprites.ContainsKey(BTEditorRectangle.GetKey(_btNode))) {
                BTEditorRectangle node = GetRectangleNodeFromBTNode(_btNode, _btTreeViewer);
                node.Node = _btNode;
                _sprites.Add(node.GetKey(), node);
            }
            (_sprites[BTEditorRectangle.GetKey(_btNode)] as BTEditorRectangle).RecursivelyCreatChildren(_sprites);
        }

        public virtual int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop) {
            return 0;
        }

        public virtual Point GetParentPoint() {
            return new Point(m_bound.X, m_bound.Y + m_bound.Height / 2);
        }

        public virtual Point GetChildPoint() {
            return new Point(m_bound.X + m_bound.Width, m_bound.Y + m_bound.Height / 2);
        }

        public override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics gc = e.Graphics;
            DefaultPaintProcess(gc, DefaultNodeColor, DefaultSelectedColor);
        }

        protected Rectangle GetDrawBound() {
            return new Rectangle(
                new Point(m_treeViewer.AutoScrollPosition.X + m_bound.Location.X,
                    m_treeViewer.AutoScrollPosition.Y + m_bound.Y), 
                m_bound.Size);
        }

        protected void DeclareRightBottom() {
            m_treeViewer.DeclareRightBottom(new Point(m_bound.Right, m_bound.Bottom));
        }

        public override bool IsMouseOn(Point _pos) {
            if (_pos.X < m_bound.Left || _pos.X > m_bound.Right || _pos.Y < m_bound.Top || _pos.Y > m_bound.Bottom) {
                return false;
            }
            return true;
        }

        public override void OnMouseDrag(Point _pos, Point _delta) {
//             m_bound.X = m_bound.X + _delta.X;
//             m_bound.Y = m_bound.Y + _delta.Y;
//             m_treeViewer.Refresh();
        }

        public override void OnMouseClick(Point _pos) {
            base.OnMouseClick(_pos);

        }

        public override void OnSelect() {
            base.OnSelect();
            m_isSelected = true;
            m_treeViewer.RaiseOnBTNodeSelected(m_node);
            m_treeViewer.Refresh();
        }

        public override void OnDeselect() {
            base.OnDeselect();
            m_isSelected = false;
            m_treeViewer.RaiseOnBTNodeDeselected(m_node);
            m_treeViewer.Refresh();
        }

        protected void DrawStringCentreAlign(string _text, Graphics _gc, Brush _brush) {
            Rectangle rect = GetDrawBound();
            SizeF stringSize = _gc.MeasureString(_text, font);
            _gc.DrawString(_text, font, Brushes.Black,
                rect.X + (rect.Width - stringSize.Width) / 2,
                rect.Y + (rect.Height - stringSize.Height) / 2);
        }

        protected void DefaultPaintProcess(Graphics _gc, Brush _nodeColor, Brush _selectColor) {
            DeclareRightBottom();
            Rectangle rect = GetDrawBound();
            DrawDebugTrail(_gc, rect);
            DrawSelectBar(_gc, rect, _selectColor);
            DrawMainPart(_gc, rect, _nodeColor);
        }

        protected void DrawMainPart(Graphics _gc, Rectangle _rect, Brush _nodeColor) {
            Rectangle main = new Rectangle(_rect.Left, _rect.Top + selectedBarHeight, _rect.Width, _rect.Height - 2 * selectedBarHeight);
            _gc.FillRectangle(_nodeColor, main);
            DrawStringCentreAlign(m_node.GetDisplayName(), _gc, Brushes.Black);
            _gc.DrawRectangle(Pens.Black, _rect);
        }

        protected void DrawSelectBar(Graphics _gc, Rectangle _rect, Brush _selectColor) {
            Rectangle upper = new Rectangle(_rect.Left, _rect.Top, _rect.Width, selectedBarHeight);
            Rectangle lower = new Rectangle(_rect.Left, _rect.Bottom - selectedBarHeight, _rect.Width, selectedBarHeight);
            //Brush brush = NonSelectedBrush;
            if (m_isSelected) {
                //brush = _selectColor;
                _gc.FillRectangles(_selectColor, new Rectangle[2] { upper, lower });
            }
            
        }

        protected void DrawDebugTrail(Graphics _gc, Rectangle _rect) {
            if (m_treeViewer.IsObservingRuntimePack()) {
                BTTreeRuntimePack.RuntimeState state = m_treeViewer.GetRuntimeState(m_node);
                Point location = new Point(_rect.Location.X - runStateBorderWidth, 
                    _rect.Location.Y - runStateBorderWidth);
                Size size = new Size(_rect.Size.Width + 2 * runStateBorderWidth, 
                                    _rect.Size.Height + 2* runStateBorderWidth);
                Rectangle rect= new Rectangle(location, size);
                if (state == BTTreeRuntimePack.RuntimeState.True) {
                    _gc.FillRectangle(TrueFillBrush, rect);
                }
                else if (state == BTTreeRuntimePack.RuntimeState.False) {
                    _gc.FillRectangle(FalseFillBrush, rect);
                }
            }
        }
    }
}
