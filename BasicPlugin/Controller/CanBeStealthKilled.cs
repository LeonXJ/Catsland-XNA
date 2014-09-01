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
    }
}
