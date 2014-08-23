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


        private Dictionary<string, BTEditorSprite> m_sprites;
        private BTTree m_btTree;

        private string m_mouseDownSpriteKey = "";
        private Point m_mouseLastPosition = new Point();

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
            m_mouseDownSpriteKey = GetEditorSpriteKeyByPosition(e.Location, MouseAction.MouseDown);
            m_mouseLastPosition = e.Location;
        }

        private void BTTreeViewer_MouseUp(object sender, MouseEventArgs e) {
            string mouseUpSpriteKey = GetEditorSpriteKeyByPosition(e.Location, MouseAction.MouseUp);
            if (m_mouseDownSpriteKey == mouseUpSpriteKey && m_mouseDownSpriteKey != ""
                && m_sprites != null && m_sprites.ContainsKey(m_mouseDownSpriteKey)) {
                m_sprites[m_mouseDownSpriteKey].OnMouseClick(e.Location);
            }
            m_mouseDownSpriteKey = "";
        }

        private void BTTreeViewer_MouseMove(object sender, MouseEventArgs e) {
            if (m_mouseDownSpriteKey != "") {
//                 string mouseOnSpriteKey = GetEditorSpriteKeyByPosition(e.Location, MouseAction.MouseMove);
                 if (//m_mouseDownSpriteKey == mouseOnSpriteKey &&
                     m_sprites != null && m_sprites.ContainsKey(m_mouseDownSpriteKey)) {
                    m_sprites[m_mouseDownSpriteKey].OnMouseDrag(e.Location, 
                        new Point(e.Location.X - m_mouseLastPosition.X, e.Location.Y - m_mouseLastPosition.Y));
                    m_mouseLastPosition = e.Location;
                }
            }
        }

       
    }

    public class BTEditorSprite {

        protected BTTreeViewer m_treeViewer;

        public BTEditorSprite(BTTreeViewer _treeViewer) {
            m_treeViewer = _treeViewer;
        }

        public virtual bool IsMouseOn(Point _pos) { return false; }

        public virtual void OnPaint(PaintEventArgs e) { }

        public virtual void OnMouseDown(Point _pos) { }

        public virtual void OnMouseUp(Point _pos) { }

        public virtual void OnMouseClick(Point _pos) { }

        public virtual void OnMouseDrag(Point _pos, Point _delta) { }
    }


}
