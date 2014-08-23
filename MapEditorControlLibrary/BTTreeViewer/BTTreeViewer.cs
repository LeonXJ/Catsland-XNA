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

        #endregion

        public BTTreeViewer() {
            InitializeComponent();
            BTEditorRectangle.InitializePrototypes();
        }

        public void SetBTTree(BTTree _btree) {
            m_btTree = _btree;
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
                // refresh()?
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
            if (m_mouseDownSpriteKey == mouseUpSpriteKey && m_mouseDownSpriteKey != ""
                && m_sprites != null && m_sprites.ContainsKey(m_mouseDownSpriteKey)) {
                m_sprites[m_mouseDownSpriteKey].OnMouseClick(worldPosition);
            }
            m_mouseDownSpriteKey = "";
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

        private void BTTreeViewer_Scroll(object sender, ScrollEventArgs e) {
            Refresh();
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
    }


}
