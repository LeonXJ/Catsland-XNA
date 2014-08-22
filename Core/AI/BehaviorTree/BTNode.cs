using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTNode : Serialable {

        public virtual bool Execute(BTTreeRuntimePack _btTree) { return true; }
    }
}
