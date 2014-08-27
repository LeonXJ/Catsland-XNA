using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTParallelSelectNode : BTCompositeNode {

        protected override bool Execute(BTTreeRuntimePack _btTree) {
            if (m_children != null) {
                bool res = false;
                foreach (BTNode node in m_children) {
                    res = node.DoExecute(_btTree) || res;
                }
                return res;
            }
            return false;
        }

        public override string GetDisplayName() {
            return "Parallel Select";
        }
    }
}
