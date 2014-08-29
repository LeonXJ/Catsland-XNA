using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class BTNode : Serialable {

        protected virtual bool Execute(BTTreeRuntimePack _runtimePack) { return true; }

        public bool DoExecute(BTTreeRuntimePack _runtimePack) {
            bool res = Execute(_runtimePack);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                _runtimePack.UpdateNodeExecutionState(this, res);
            }
            return res;
        }

        public virtual BTNode FindParent(BTNode _target) { return null; }

        public virtual bool IsAncestorOf(BTNode _target) { return false; }

        public virtual void RemoveChild(BTNode _target) { }

        public virtual string GetDisplayName() { return GetType().Name; }

        /**
         * @brief can this node add one more child?
         **/ 
        public virtual bool CanAddMoreChild() { return false; }

    }
}
