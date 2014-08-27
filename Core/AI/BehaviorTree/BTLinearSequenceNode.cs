using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTLinearSequenceNode : BTCompositeNode {

        /**
         * @brief execute till the first False return
         **/
        protected override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_children != null) {
                foreach (BTNode node in m_children) {
                    if (!node.DoExecute(_btTree)) {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string GetDisplayName() {
            return "Linear Sequence";
        }
    }
}
