using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTCompositeNode : BTNode{

#region Properties

        [SerialAttribute]
        protected List<BTNode> m_children = new List<BTNode>();
        public List<BTNode> Children {
            get {
                return m_children;
            }
        }

#endregion

        public void AddChild(BTNode _btNode) {
            m_children.Add(_btNode);
        }

        public override void RemoveChild(BTNode _btNode) {
            if (m_children != null & m_children.Contains(_btNode)) {
                m_children.Remove(_btNode);
            }
        }

        public sealed override BTNode FindParent(BTNode _target) {
            // check children
            if (m_children != null) {
                foreach (BTNode child in m_children) {
                    if (_target == child) {
                        return this;
                    }
                }
                // recursively find in children
                foreach (BTNode child in m_children) {
                    BTNode childResult = child.FindParent(_target);
                    if(childResult != null){
                        return childResult;
                    }
                }
            }
            return null;
        }

        public override bool CanAddMoreChild() {
            return true;
        }
    }
}
