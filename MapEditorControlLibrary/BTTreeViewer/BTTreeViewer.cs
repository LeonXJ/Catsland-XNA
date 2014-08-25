using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;
using System.Diagnostics;

namespace Catsland.MapEditorControlLibrary {
    public partial class BTTreeViewer : UserControl {

        #region Properties

        public event EventHandler<BTNodeSelectedArgs> OnBTNodeSelected;
        public event EventHandler<BTNodeSelectedArgs> OnBTNodeDeselected;

        private Dictionary<string, BTEditorSprite> m_sprites; 
        private BTTree m_btTree;

        private string m_mouseDownSpriteKey = "";
        private Point m_mouseLastPosition = new Point();

        private Point m_currentRightBottom = new Point(0, 0);
        private Point m_ongoingRightBottom = new Point(0, 0);
        public void DeclareRightBottom(Point _point) {
            if (_point.X > m_ongoingRightBottom.X) {
                m_ongoingRightBottom.X = _point.X;
            }
            if (_point.Y > m_ongoingRightBottom.Y) {
                m_ongoingRightBottom.Y = _point.Y;
            }
        }

        public Point GetDrawPosition(Point _point) {
            Point offset = AutoScrollPosition;
            return new Point(_point.X + offset.X, _point.Y + offset.Y);
        }
        public Point GetWorldPosition(Point _pointInDraw) {
            Point offset = AutoScrollPosition;
            return new Point(_pointInDraw.X - offset.X, _pointInDraw.Y - offset.Y);
        }

        public enum MouseAction{
            MouseDown = 0,
            MouseMove,
            MouseUp,
        };

        private string m_selectedSpriteID = "";

        #endregion

        public BTTreeViewer() {
            InitializeComponent();
            BTEditorRectangle.InitializePrototypes();
        }

        public void SetBTTree(BTTree _btree) {
            m_btTree = _btree;
            CreateChart();
            AutoLayoutChart();
            Refresh();
        }

        public BTEditorRectangle GetRectangle(BTNode _node) {
            if (_node == null) {
                return null;
            }
            string key = BTEditorRectangle.GetKey(_node);
            if (m_sprites != null && m_sprites.ContainsKey(key)) {
                return (m_sprites[key] as BTEditorRectangle);
            }
            return null;
        }

        protected void CreateChart() {
            if (m_btTree == null) {
                m_sprites = null;
            }
            else {
                m_sprites = new Dictionary<string, BTEditorSprite>();
                BTEditorRectangle.RecursivelyCreateSprites(m_sprites, m_btTree.Root, this);
            }
        }

        protected void AutoLayoutChart() {
            if (m_btTree != null && m_btTree.Root != null && m_sprites != null) {
                string rootKey = BTEditorRectangle.GetKey(m_btTree.Root);
                // auto layout
                Point leftTop = new Point(0, 0);
                if (m_sprites.ContainsKey(rootKey)) {
                    (m_sprites[rootKey] as BTEditorRectangle).AutoRecursivelyLayout(m_sprites, leftTop);
                }
                
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            // background
            e.Graphics.FillRectangle(Brushes.AliceBlue, new Rectangle(0, 0, this.Width, this.Height));
            if (m_sprites != null) {
                foreach (KeyValuePair<string, BTEditorSprite> keyValue in m_sprites) {
                    keyValue.Value.OnPaint(e);
                }
            }
            DecideAndUpdateScroll();
        }

        private void DecideAndUpdateScroll() {
            if (!m_ongoingRightBottom.Equals(m_currentRightBottom)) {
                this.AutoScrollMinSize = new Size(m_ongoingRightBottom.X, m_ongoingRightBottom.Y);
                m_currentRightBottom = m_ongoingRightBottom;
                Refresh();
            }
            m_ongoingRightBottom.X = 0;
            m_ongoingRightBottom.Y = 0;
        }

        protected string GetEditorSpriteKeyByPosition(Point _pos, MouseAction _mouseAction) {
            if (m_sprites != null) {
                foreach (KeyValuePair<string, BTEditorSprite> keyValue in m_sprites) {
                    if (keyValue.Value.IsMouseOn(_pos)) {
                        if (_mouseAction == MouseAction.MouseDown) {
                            keyValue.Value.OnMouseDown(_pos);
                        }
                        else if(_mouseAction == MouseAction.MouseUp) {
                            keyValue.Value.OnMouseUp(_pos);
                        }
                        return keyValue.Key;
                    }
                }
            }
            return "";
        }

        private void BTTreeViewer_MouseDown(object sender, MouseEventArgs e) {
            Point worldPosition = GetWorldPosition(e.Location);
            m_mouseDownSpriteKey = GetEditorSpriteKeyByPosition(worldPosition, MouseAction.MouseDown);
            m_mouseLastPosition = worldPosition;
        }

        private void BTTreeViewer_MouseUp(object sender, MouseEventArgs e) {
            Point worldPosition = GetWorldPosition(e.Location);
            string mouseUpSpriteKey = GetEditorSpriteKeyByPosition(worldPosition, MouseAction.MouseUp);
            if (m_sprites != null && m_sprites.ContainsKey(m_mouseDownSpriteKey)) { // mouse down on object 
                if (mouseUpSpriteKey != "") {   // mouse up on object
                    if (mouseUpSpriteKey == m_mouseDownSpriteKey) {   // on the same object, click
                        m_sprites[m_mouseDownSpriteKey].OnMouseClick(worldPosition);
                        if (m_selectedSpriteID != m_mouseDownSpriteKey) {   // not the previous selected object
                            DoDeselect();
                            DeSelect(m_mouseDownSpriteKey);
                        }
                    }
                    else {  // on different object
                        DoDeselect();
                        m_sprites[mouseUpSpriteKey].OnDragOn(worldPosition, m_sprites[m_mouseDownSpriteKey]);
                    }
                }
                else {  // mouse up on space
                    DoDeselect();
                    UpdateChildrenSequence(m_mouseDownSpriteKey, GetWorldPosition(e.Location));
                }
            }
            else {
                DoDeselect();
            }
            m_mouseDownSpriteKey = "";
        }

        private void DeSelect(string _spriteID) {
            m_sprites[m_mouseDownSpriteKey].OnSelect();
            m_selectedSpriteID = _spriteID;
        }

        private void DoDeselect() {
            if (m_selectedSpriteID != "") {
                m_sprites[m_selectedSpriteID].OnDeselect();
                m_selectedSpriteID = "";
            }
        }

        private void UpdateChildrenSequence(string _node, Point _worldPos) {
            if (m_sprites.ContainsKey(_node)) {
                BTEditorRectangle editorNode = m_sprites[_node] as BTEditorRectangle;
                BTNode node = editorNode.Node;
                if (node != null) {
                    BTNode parent = m_btTree.FindParent(node);
                    if (parent != null && parent.GetType().IsSubclassOf(typeof(BTCompositeNode))) {
                        (GetRectangle(parent) as BTEditorRectangleBTCompositeNode).AdjustChildrenSequence(editorNode, _worldPos);
                        AutoLayoutChart();
                        Refresh();
                    }
                }
            }

        }

        private void BTTreeViewer_MouseMove(object sender, MouseEventArgs e) {
            if (m_mouseDownSpriteKey != "") {
                Point worldPosition = GetWorldPosition(e.Location);
                if (m_sprites != null && m_sprites.ContainsKey(m_mouseDownSpriteKey)) {
                    m_sprites[m_mouseDownSpriteKey].OnMouseDrag(worldPosition,
                        new Point(worldPosition.X - m_mouseLastPosition.X, worldPosition.Y - m_mouseLastPosition.Y));
                    m_mouseLastPosition = worldPosition;
                }
            }
        }

        public void RaiseOnBTNodeSelected(BTNode _node) {
            BTNodeSelectedArgs args = new BTNodeSelectedArgs(_node);
            OnBTNodeSelected(this, args);
        }

        public void RaiseOnBTNodeDeselected(BTNode _node) {
            BTNodeSelectedArgs args = new BTNodeSelectedArgs(_node);
            OnBTNodeDeselected(this, args);
        }

        private void BTTreeViewer_Scroll(object sender, ScrollEventArgs e) {
            Refresh();
        }

        public bool SetParent(string _child, string _newParent) {
            if (!m_sprites.ContainsKey(_child) || !m_sprites.ContainsKey(_newParent)) {
                return false;
            }
            BTEditorRectangle child = m_sprites[_child] as BTEditorRectangle;
            BTEditorRectangle newParent = m_sprites[_newParent] as BTEditorRectangle;
            // check cycle
            if (child.Node.FindParent(newParent.Node) != null) {
                System.Windows.Forms.MessageBox.Show("Cycle structor is illegal.");
                return false;
            }
            // check newParent
            if (newParent.Node.GetType().IsSubclassOf(typeof(BTConditionNode))) {
                BTNode otherChild = (newParent.Node as BTConditionNode).Child;
                if (otherChild != null) {
                    System.Windows.Forms.MessageBox.Show("Condition node cannot have more than one child.");
                    return false;
                }
            }
            // destroy old edge
            BTNode oldBTParent = m_btTree.FindParent(child.Node);
            if (oldBTParent != null) {
                string oldEdgeKey = BTEditorLine.GetKey(oldBTParent, child.Node);
                if (m_sprites.ContainsKey(oldEdgeKey)) {
                    m_sprites.Remove(oldEdgeKey);
                }
                if (oldBTParent.GetType().IsSubclassOf(typeof(BTCompositeNode))) {
                    (oldBTParent as BTCompositeNode).RemoveChild(child.Node);
                }
                else if (oldBTParent.GetType().IsSubclassOf(typeof(BTConditionNode))) {
                    (oldBTParent as BTConditionNode).Child = null;
                }
            }
            // create new link
            // destroy other edge if necessary
            if (newParent.Node.GetType().IsSubclassOf(typeof(BTConditionNode))) {
                (newParent.Node as BTConditionNode).Child = child.Node;
                
            }
            else if (newParent.Node.GetType().IsSubclassOf(typeof(BTCompositeNode))) {
                (newParent.Node as BTCompositeNode).AddChild(child.Node);
            }
            else {
                Debug.Assert(false, "Cannot add key to node with type: " + newParent.Node.GetType().Name);
                return false;
            }
            BTEditorLine newLine = new BTEditorLine(this);
            newLine.ParentNode = newParent.Node;
            newLine.ChildNode = child.Node;
            m_sprites.Add(newLine.GetKey(), newLine);

            AutoLayoutChart();
            Refresh();
            return true;
        }
    }

   

    public class BTNodeSelectedArgs : EventArgs {
        private BTNode m_btNode;
        public BTNode BTNode {
            get {
                return m_btNode;
            }
        }

        public BTNodeSelectedArgs(BTNode _btNode) {
            m_btNode = _btNode;
        }
    }

    public class BTEditorSprite {

        protected BTTreeViewer m_treeViewer;
        protected static Font font;

        public BTEditorSprite(BTTreeViewer _treeViewer) {
            m_treeViewer = _treeViewer;
            if (font == null) {
                font = new Font("Arial", 10.5f);
            }
        }

        public virtual bool IsMouseOn(Point _pos) { return false; }

        public virtual void OnPaint(PaintEventArgs e) { }

        public virtual void OnMouseDown(Point _pos) { }

        public virtual void OnMouseUp(Point _pos) { }

        public virtual void OnMouseClick(Point _pos) { }

        public virtual void OnMouseDrag(Point _pos, Point _delta) { }

        public virtual void OnDragOn(Point _pos, BTEditorSprite _source) { }

        public virtual void OnSelect() { }

        public virtual void OnDeselect() { }
    
    }


}
