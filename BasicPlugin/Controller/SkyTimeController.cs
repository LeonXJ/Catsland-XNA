using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class SkyTimeController : CatComponent{

        public SkyTimeController(GameObject _gameObject)
            : base(_gameObject) { }

        public SkyTimeController() : base() { }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            Environment environment = m_gameObject.Scene.GetSharedObject(typeof(Environment).ToString())
                as Environment;
            ModelComponent modelComponent = m_gameObject.GetComponent(typeof(ModelComponent))
                as ModelComponent;
            if (environment != null && modelComponent != null 
                && modelComponent.GetCatModelInstance() != null
                && modelComponent.GetCatModelInstance().GetMaterial().HasParameter("Time")) {
                modelComponent.GetCatModelInstance().GetMaterial()
                    .SetParameter("Time", new CatFloat(environment.CurrentTimeInRatio));
            }
        }

        public static string GetMenuNames() {
            return "Controller|SkyTimeController";
        }
    }
}
