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

        protected void CreateChart() {
            if (m_btTree == null) {
                m_sprites = null;
            }
            else {
                m_sprites = new Dictionary<string, BTEditorSprite>();
                BTEditorRectangle.RecursivelyCreateSprites(m_sprites, m_btTree.Root);
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
    }

    public class BTEditorSprite {

        public virtual bool IsMouseOn(Point _pos) { return false; }

        public virtual void OnPaint(PaintEventArgs e) { }
    }


}
