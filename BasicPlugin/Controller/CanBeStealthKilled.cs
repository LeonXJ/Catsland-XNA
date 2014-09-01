using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class CanBeStealthKilled : CatComponent {
        public CanBeStealthKilled(GameObject _gameObject)
            : base(_gameObject) { }

        public CanBeStealthKilled()
            : base() { }

        public void Killed() {
            CatController controller = m_gameObject.GetComponent(typeof(CatController)) as CatController;
            if (controller != null) {
                controller.CurrentState = StateStealthKillToDeath.GetState();
            }
        }
    }
}
