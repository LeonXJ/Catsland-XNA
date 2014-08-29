using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Catsland.Core;
using System.Drawing;
using System.Windows.Forms;

namespace Catsland.MapEditorControlLibrary {

    public abstract class BTEditorRectangle : BTEditorSprite {

        #region Properties

        internal static int HorizontalInterval = 20;
        internal static int VerticalInterval = 7;

        private static Brush DefaultNodeBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
        private static Brush DefaultSelectedBrush = new SolidBrush(Color.FromArgb(10, 10, 10));

        protected static Brush FalseFillBrush = new SolidBrush(FalseFillColor);
        protected static Brush TrueFillBrush = new SolidBrush(TrueFillColor);
        protected static int runStateBorderWidth = 3;
 
        private static List<BTEditorRectangle> nodePrototype;

        protected BTNode m_node;
        internal BTNode Node {
            set {
                m_node = value;
            }
            get {
                return m_node;
            }
        }
       
        protected Rectangle m_bound = new Rectangle(0, 0, 100, 22);
        protected bool m_isSelected = false;

        #endregion

        internal BTEditorRectangle(BTTreeViewer _treeViewer)
            :base(_treeViewer) {
            m_treeViewer = _treeViewer;
        }

        /**
         * @brief initialize nodePrototype. In the creation of the chart, the viewer
         *  find appropriate node in nodePrototype for the given BTNode, clone it and add to
         *  sprite list.
         **/ 
        internal static void InitializePrototypes() {
            nodePrototype = new List<BTEditorRectangle>();
            nodePrototype.Add(new BTEditorRectangleBTCompositeNode(null));
            nodePrototype.Add(new BTEditorRectangleBTConditionNode(null));
            nodePrototype.Add(new BTEditorRectangleBTActionNode(null));
        }

        /**
         * @brief create a BTEditorRectangle according to the given BTNode
         **/ 
        protected static BTEditorRectangle CreateRectangleNodeFromBTNode(BTNode _btNode, BTTreeViewer _treeViewer) {
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

        internal abstract BTEditorRectangle Clone(BTTreeViewer _treeViewer);

        internal string GetKey() {
            return GetKey(m_node);
        }

        internal static string GetKey(BTNode _node) {
            if (_node != null) {
                return _node.GUID;
            }
            else {
                Debug.Assert(false, "node has not been set");
                return "";
            }
        }

        internal Point GetPosition() {
            return m_bound.Location;
        }

        protected abstract bool IsThisType(BTNode _btNode);

        /**
         * @brief recursively create sprites and insert into _sprites
         **/ 
        internal static void RecursivelyCreateSprites(Dictionary<string, BTEditorSprite> _sprites, BTNode _btNode, BTTreeViewer _btTreeViewer) {
            if (_btNode == null) {
                return;
            }
            if (!_sprites.ContainsKey(BTEditorRectangle.GetKey(_btNode))) {
                BTEditorRectangle node = CreateRectangleNodeFromBTNode(_btNode, _btTreeViewer);
                node.Node = _btNode;
                _sprites.Add(node.GetKey(), node);
            }
            (_sprites[BTEditorRectangle.GetKey(_btNode)] as BTEditorRectangle).RecursivelyCreatChildren(_sprites);
        }

        /**
         * @brief recursively create children (including lines and nodes) and insert into _sprites
         **/
        protected abstract void RecursivelyCreatChildren(Dictionary<string, BTEditorSprite> _sprites);

        /**
         * @brief layout the node and its children with lefttop on _leftTop
         **/ 
        internal abstract int AutoRecursivelyLayout(Dictionary<string, BTEditorSprite> _sprites, Point _leftTop);

        /**
         * @brief get parent attach point
         **/ 
        internal virtual Point GetParentPoint() {
            return new Point(m_bound.X, m_bound.Y + m_bound.Height / 2);
        }

        /**
         * @brief get child attach point
         **/ 
        internal virtual Point GetChildPoint() {
            return new Point(m_bound.X + m_bound.Width, m_bound.Y + m_bound.Height / 2);
        }

        /**
         * @brief get the bound of rectangle in draw coordinates
         **/
        protected Rectangle GetDrawBound() {
            return new Rectangle(
                new Point(m_treeViewer.AutoScrollPosition.X + m_bound.Location.X,
                    m_treeViewer.AutoScrollPosition.Y + m_bound.Y),
                m_bound.Size);
        }

        internal override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics gc = e.Graphics;
            DefaultPaintProcess(gc, DefaultNodeBrush, DefaultSelectedBrush);
        }

        /**
         * @brief declare the right bottom position for measuring the size of the canvas
         **/ 
        protected void DeclareRightBottom() {
            m_treeViewer.DeclareRightBottom(new Point(m_bound.Right, m_bound.Bottom));
        }

        internal override bool IsMouseOn(Point _pos) {
            if (_pos.X < m_bound.Left || _pos.X > m_bound.Right || _pos.Y < m_bound.Top || _pos.Y > m_bound.Bottom) {
                return false;
            }
            return true;
        }

        internal override void OnMouseDrag(Point _pos, Point _delta) {
//             m_bound.X = m_bound.X + _delta.X;
//             m_bound.Y = m_bound.Y + _delta.Y;
//             m_treeViewer.Refresh();
        }

        internal override void OnMouseClick(Point _pos) {
            base.OnMouseClick(_pos);
        }

        internal override void OnSelect() {
            base.OnSelect();
            m_isSelected = true;
            m_treeViewer.RaiseOnBTNodeSelected(m_node);
            m_treeViewer.Refresh();
        }

        internal override void OnDeselect() {
            base.OnDeselect();
            m_isSelected = false;
            m_treeViewer.RaiseOnBTNodeDeselected(m_node);
            m_treeViewer.Refresh();
        }

        /**
         * @brief draw text with _brush in center alignment
         **/ 
        protected void DrawStringCentreAlign(string _text, Graphics _gc, Brush _brush) {
            Rectangle rect = GetDrawBound();
            SizeF stringSize = _gc.MeasureString(_text, font);
            _gc.DrawString(_text, font, Brushes.Black,
                rect.X + (rect.Width - stringSize.Width) / 2,
                rect.Y + (rect.Height - stringSize.Height) / 2);
        }

        /**
         * @brief a default process to draw the rectangle
         **/ 
        protected void DefaultPaintProcess(Graphics _gc, Brush _nodeColor, Brush _selectColor) {
            DeclareRightBottom();
            Rectangle rect = GetDrawBound();
            DrawDebugTrail(_gc, rect);
            DrawMainPart(_gc, rect, _nodeColor, _selectColor);
        }

        /**
         * @brief draw the main part of rectangle
         **/ 
        protected void DrawMainPart(Graphics _gc, Rectangle _rect, Brush _nodeColor, Brush _selectColor) {
            if (m_isSelected) {
                _gc.FillRectangle(_selectColor, _rect);
            }
            else {
                _gc.FillRectangle(_nodeColor, _rect);
            }
            DrawStringCentreAlign(m_node.GetDisplayName(), _gc, Brushes.Black);
            _gc.DrawRectangle(Pens.Black, _rect);
        }

        /**
         * @brief draw the debug trail of the rectangle
         **/ 
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
