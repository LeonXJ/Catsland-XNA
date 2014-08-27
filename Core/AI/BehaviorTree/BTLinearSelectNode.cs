using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTLinearSelectNode : BTCompositeNode{

        /**
         * @brief execute till the first True return
         **/
        protected override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_children != null) {
                foreach (BTNode node in m_children) {
                    if (node.DoExecute(_btTree)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string GetDisplayName() {
            return "Linear Select";
        }
    }
}
