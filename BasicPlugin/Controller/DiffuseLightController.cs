using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class DiffuseLightController : CatComponent {

        public DiffuseLightController(GameObject _gameObject)
            : base(_gameObject) { }
        public DiffuseLightController() : base() { }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            Environment environment = m_gameObject.Scene.GetSharedObject(typeof(Environment).ToString())
                as Environment;
            Light light = null;
            foreach (KeyValuePair<string, CatComponent> keyValue in m_gameObject.Components) {
                if (keyValue.Value.GetType().IsSubclassOf(typeof(Light))) {
                    light = keyValue.Value as Light;
                }
            }
            if (environment != null && light != null) {
                light.DiffuseColor = environment.DiffuseColor;
            }
        }

        public static string GetMenuNames() {
            return "Controller|DiffuseLightController";
        }
    }
}
