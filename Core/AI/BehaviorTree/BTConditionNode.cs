using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTConditionNode : BTNode{

#region Properties

        [SerialAttribute]
        protected BTNode m_child;
        public BTNode Child {
            set {
                m_child = value;
            }
            get {
                return m_child;
            }
        }

#endregion

        protected virtual bool JudgeCondition(BTTreeRuntimePack _btTree) {
            return true;
        }

        public sealed override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_child != null) {
                if (JudgeCondition(_btTree)) {
                    return m_child.Execute(_btTree);
                }
                return false;
            }
            else {
                return JudgeCondition(_btTree);
            }
        }

        public override BTNode FindParent(BTNode _target) {
            // check children
            if (m_child != null) {
                if (_target == m_child) {
                    return this;
                }
                return m_child.FindParent(_target);
            }
            return null;
        }

        public override bool IsAncestorOf(BTNode _target) {
            if (m_child != null) {
                if (_target == m_child) {
                    return true;
                }
                else {
                    return m_child.IsAncestorOf(_target);
                }
            }
            return false;
        }

        public override void RemoveChild(BTNode _target) {
            base.RemoveChild(_target);
            if (m_child == _target) {
                m_child = null;
            }
        }
    }
}
