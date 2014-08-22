using System;
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
        public static int VerticalInterval = 5;

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
        protected Rectangle m_bound = new Rectangle(0, 0, 50, 20);

        #endregion

        public static void InitializePrototypes() {
            nodePrototype = new List<BTEditorRectangle>();
            nodePrototype.Add(new BTEditorRectangleBTCompositeNode());
            nodePrototype.Add(new BTEditorRectangleBTConditionNode());
            nodePrototype.Add(new BTEditorRectangleBTActionNode());
        }

        public virtual BTEditorRectangle Clone() { return new BTEditorRectangle(); }

        protected virtual bool IsThisType(BTNode _btNode) {
            return false;
        }

        protected static BTEditorRectangle GetRectangleNodeFromBTNode(BTNode _btNode) {
            if (nodePrototype != null) {
                foreach (BTEditorRectangle prototype in nodePrototype) {
                    if (prototype.IsThisType(_btNode)) {
                        return prototype.Clone();
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

        public static void RecursivelyCreateSprites(Dictionary<string, BTEditorSprite> _sprites, BTNode _btNode) {
            if (_btNode == null) {
                return;
            }
            BTEditorRectangle node = GetRectangleNodeFromBTNode(_btNode);
            node.Node = _btNode;
            _sprites.Add(_btNode.GUID, node);
            node.RecursivelyCreatChildren(_sprites);
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
            gc.FillRectangle(Brushes.Black, m_bound);
            gc.DrawRectangle(Pens.Black, m_bound);
        }
    }
}
