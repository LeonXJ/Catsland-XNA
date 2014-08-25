using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTNode : Serialable {

        public virtual bool Execute(BTTreeRuntimePack _btTree) { return true; }

        public virtual BTNode FindParent(BTNode _target) { return null; }

        public virtual bool IsAncestorOf(BTNode _target) { return false; }

        public virtual void RemoveChild(BTNode _target) { }

    }
}
