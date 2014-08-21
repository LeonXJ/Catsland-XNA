using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTCompositeNode : BTNode{

#region Properties

        [SerialAttribute]
        protected List<BTNode> m_children = new List<BTNode>();

#endregion

        public void AddChild(BTNode _btNode) {
            m_children.Add(_btNode);
        }
    }
}
