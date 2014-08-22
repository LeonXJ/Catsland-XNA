using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTParallelSelectNode : BTCompositeNode {

        public override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_children != null) {
                bool res = false;
                foreach (BTNode node in m_children) {
                    res = res || node.Execute(_btTree);
                }
                return res;
            }
            return false;
        }
    }
}
