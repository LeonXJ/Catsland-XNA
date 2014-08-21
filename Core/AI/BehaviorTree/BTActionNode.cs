using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTActionNode : BTNode {

        public virtual void OnEnter(BTTreeComponent _btTree) { }

        public virtual bool OnRunning(BTTreeComponent _btTree) { return true; }

        public virtual void OnExit(BTTreeComponent _btTree) { }

        sealed public override bool Execute(BTTreeComponent _btTree) {
            if (!_btTree.IsActionRunning(this)) {
                OnEnter(_btTree);
            }
            _btTree.DeclareRunning(this);
            return OnRunning(_btTree);
        }
    }
}
