using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTLinearSequenceNode : BTCompositeNode {

        /**
         * @brief execute till the first False return
         **/
        public override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_children != null) {
                foreach (BTNode node in m_children) {
                    if (!node.Execute(_btTree)) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
