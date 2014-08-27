using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTActionNode : BTNode {

        public virtual void OnEnter(BTTreeRuntimePack _btTree) { }

        public virtual bool OnRunning(BTTreeRuntimePack _btTree) { return true; }

        public virtual void OnExit(BTTreeRuntimePack _btTree) { }

        sealed protected override bool Execute(BTTreeRuntimePack _btTree) {
            if (!_btTree.IsActionRunning(this)) {
                OnEnter(_btTree);
            }
            _btTree.DeclareRunning(this);
            return OnRunning(_btTree);
        }
    }
}
