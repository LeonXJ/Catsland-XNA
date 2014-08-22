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

        sealed public override bool Execute(BTTreeRuntimePack _btTree) {
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
    }
}
