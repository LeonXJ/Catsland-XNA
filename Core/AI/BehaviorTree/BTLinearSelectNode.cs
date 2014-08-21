using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTLinearSelectNode : BTCompositeNode{

        /**
         * @brief execute till the first True return
         **/ 
        public override bool Execute(BTTreeComponent _btTree) {
            if (m_children != null) {
                foreach (BTNode node in m_children) {
                    if (node.Execute(_btTree)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
