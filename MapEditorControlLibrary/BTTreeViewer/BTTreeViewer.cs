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

        public enum MouseAction {
            MouseDown = 0,
            MouseMove,
            MouseUp,
        };

        private BTTree m_btTree;
        public BTTree BTTree {
            get {
                return m_btTree;
            }
        }

        private Dictionary<string, BTEditorSprite> m_sprites;
        private BTTreeRuntimePack m_observingRuntimePack;
        private string m_mouseDownSpriteKey = "";
        private string m_selectedSpriteID = "";
        private Point m_mouseLastPosition = new Point();
        private Point m_currentRightBottom = new Point(0, 0);
        private Point m_ongoingRightBottom = new Point(0, 0);

#endregion

        public BTTreeViewer() {
            InitializeComponent();
            BTEditorRectangle.InitializePrototypes();
        }

        /**
         * @brief set BTTree
         *   Warning: if it is observing a runtimepack, this action will stop
         *      the observation
         **/
        public void SetBTTree(BTTree _btree) {
            if (IsObservingRuntimePack()) {
                m_observingRuntimePack = null;
            }
            m_btTree = _btree;
            CreateChart();
            AutoLayoutChart();
            Refresh();
        }

        /**
         * @brief set BTTreeRuntimePack and start observing the tree
         **/ 
        public void SetBTTreeAndObservingRuntimePack(BTTreeRuntimePack _runtimePack) {
            if (!IsObservingRuntimePack()
                || m_observingRuntimePack.BTTree != m_btTree) {
                SetBTTree(_runtimePack.BTTree);
            }
            m_observingRuntimePack = _runtimePack;
        }

        internal bool IsObservingRuntimePack() {
            return (m_observingRuntimePack != null);
        }

        /**
         * @brief call this after _treeNode is added to bttree
         **/
        public void DeclareAddNode(BTNode _treeNode) {
            BTNode parent = m_btTree.FindParent(_treeNode);
            string parentKey = BTEditorRectangle.GetKey(parent);
            if (m_sprites.ContainsKey(parentKey)) {
                BTEditorRectangle.RecursivelyCreateSprites(m_sprites, parent, this);
                AutoLayoutChart();
                Refresh();
            }
        }

        /**
         * @brief call this after _treeNode is removed from bttree
         **/
        public void DeclareRemoveNode(BTNode _treeNode) {
            DoDeselect();
            CreateChart();
            AutoLayoutChart();
            Refresh();
        }

        /**
         * @brief [Only called by BTEditorSprite] announce the position of the sprite
         *  in order to get the size of the canvas
         **/ 
        internal void DeclareRightBottom(Point _point) {
            if (_point.X > m_ongoingRightBottom.X) {
                m_ongoingRightBottom.X = _point.X;
            }
            if (_point.Y > m_ongoingRightBottom.Y) {
                m_ongoingRightBottom.Y = _point.Y;
            }
        }

        /** 
         * @brief Convert the world position into draw position
         *    because we use scrollbar.
         **/ 
        internal Point GetDrawPosition(Point _point) {
            Point offset = AutoScrollPosition;
            return new Point(_point.X + offset.X, _point.Y + offset.Y);
        }

        /** 
         * @brief Convert the draw position into world position
         *    because we use scrollbar.
         **/ 
        internal Point GetWorldPosition(Point _pointInDraw) {
            Point offset = AutoScrollPosition;
            return new Point(_pointInDraw.X - offset.X, _pointInDraw.Y - offset.Y);
        }

        /**
         * @brief [Only called by BTEditorSprite] Get the runtime state of a BTnode
         **/
        internal BTTreeRuntimePack.RuntimeState GetRuntimeState(BTNode _btNode) {
            if (m_observingRuntimePack != null) {
                return m_observingRuntimePack.GetRuntimeState(_btNode);
            }
            return BTTreeRuntimePack.RuntimeState.Norun;
        }

        /**
         * @brief get the existing BTEditorRectangle for the given BTNode
         **/ 
        internal BTEditorRectangle GetRectangle(BTNode _node) {
            if (_node == null) {
                return null;
            }
            string key = BTEditorRectangle.GetKey(_node);
            if (m_sprites != null && m_sprites.ContainsKey(key)) {
                return (m_sprites[key] as BTEditorRectangle);
            }
            return null;
        }

        /**
         * @brief create chart according to m_btTree. if it is null, it will clear
         *  the view
         **/ 
        private void CreateChart() {
            if (m_btTree == null) {
                m_sprites = null;
            }
            else {
                m_sprites = new Dictionary<string, BTEditorSprite>();
                BTEditorRectangle.RecursivelyCreateSprites(m_sprites, m_btTree.Root, this);
            }
        }

        /**
         * @brief recursively layout the nodes
         **/ 
        private void AutoLayoutChart() {
            if (m_btTree != null && m_btTree.Root != null && m_sprites != null) {
                string rootKey = BTEditorRectangle.GetKey(m_btTree.Root);
                // auto layout
                Point leftTop = new Point(10, 10);
                if (m_sprites.ContainsKey(rootKey)) {
                    (m_sprites[rootKey] as BTEditorRectangle).AutoRecursivelyLayout(m_sprites, leftTop);
                }
                
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (m_sprites != null) {
                foreach (KeyValuePair<string, BTEditorSprite> keyValue in m_sprites) {
                    keyValue.Value.OnPaint(e);
                }
            }
            DecideAndUpdateScroll();
        }

        /**
         * @brief decide whether to update scrollbar by checking if the m_ongoingRightBottom changed
         **/ 
        private void DecideAndUpdateScroll() {
            if (!m_ongoingRightBottom.Equals(m_currentRightBottom)) {
                this.AutoScrollMinSize = new Size(m_ongoingRightBottom.X, m_ongoingRightBottom.Y);
                m_currentRightBottom = m_ongoingRightBottom;
                Refresh();
            }
            m_ongoingRightBottom.X = 0;
            m_ongoingRightBottom.Y = 0;
        }

        /**
         * @brief get the key of the sprite with the given position
         *      return "" if no sprite is hit
         **/
        private string GetEditorSpriteKeyByPosition(Point _pos, MouseAction _mouseAction) {
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

        /**
         * @brief mouse-down event
         **/ 
        private void BTTreeViewer_MouseDown(object sender, MouseEventArgs e) {
            Point worldPosition = GetWorldPosition(e.Location);
            m_mouseDownSpriteKey = GetEditorSpriteKeyByPosition(worldPosition, MouseAction.MouseDown);
            m_mouseLastPosition = worldPosition;
        }

        /**
         * @brief mouse-up event
         **/ 
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

        /**
         * @brief mouse-move event
         **/ 
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

        /**
         * @brief select the given sprite
         **/ 
        private void DeSelect(string _spriteID) {
            if (m_sprites.ContainsKey(m_mouseDownSpriteKey)) {
                m_sprites[m_mouseDownSpriteKey].OnSelect();
                m_selectedSpriteID = _spriteID;
            }
        }

        /**
         * @brief deselect the given sprite
         **/ 
        private void DoDeselect() {
            if (m_selectedSpriteID != "" && m_sprites.ContainsKey(m_selectedSpriteID)) {
                m_sprites[m_selectedSpriteID].OnDeselect();
                m_selectedSpriteID = "";
            }
        }

        /**
         * @brief change _node's order among its siblings according to _worldPos
         *      in BTEditorRectangle's world y positions
         **/
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

        /**
         * @brief [Only called by BTEditorRectangle] invoke OnBTNodeSelected event by BTEditorRectangle
         **/ 
        internal void RaiseOnBTNodeSelected(BTNode _node) {
            BTNodeSelectedArgs args = new BTNodeSelectedArgs(_node);
            OnBTNodeSelected(this, args);
        }

        /**
         * @brief [Only called by BTEditorRectangle] invoke OnBTNodeDeselected event by BTEditorRectangle
         **/ 
        internal void RaiseOnBTNodeDeselected(BTNode _node) {
            BTNodeSelectedArgs args = new BTNodeSelectedArgs(_node);
            OnBTNodeDeselected(this, args);
        }

        /**
         * @brief scollbar move event
         **/ 
        private void BTTreeViewer_Scroll(object sender, ScrollEventArgs e) {
            Refresh();
        }

        /**
         * @brief Set child's parent to _newParent. the function do the following things
         *  1. check whether the child and parent exist
         *  2. check whether there would be cycle structure: if the _child is _newParent's ancestor
         *  3. if the _newParent is ConditionNode, check if it has child. If it does, the action is illege
         *  4. break the old edge
         *  5. make new connection
         **/ 
        internal bool SetParent(string _child, string _newParent) {
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
}
