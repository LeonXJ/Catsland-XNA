using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class AmbientLightController : CatComponent{

        public AmbientLightController(GameObject _gameObject)
            : base(_gameObject) { }

        public AmbientLightController() : base() { }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            
            Environment environment = m_gameObject.Scene.GetSharedObject(typeof(Environment).ToString())
                as Environment;
            if(environment != null){
                m_gameObject.Scene.m_shadowSystem.AmbientColor = environment.AmbientColor;
            }
            
        }

        public static string GetMenuNames() {
            return "Controller|AmbientLightController";
        }
    }
}
