using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin {
    public class AdvancedTriggerEnabler : CatComponent {

        public string renderObjectName { set; get; }

        public AdvancedTriggerEnabler(GameObject gameObject)
            : base(gameObject) {

        }

        public override void  EnterTrigger(Collider trigger, Collider invoker)
        {
            GameObject renderObject = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(renderObjectName);
            if (renderObject != null) {
                QuadRender quadRender = (QuadRender)renderObject.GetComponent(typeof(QuadRender).Name);
                if(quadRender!=null){
                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(1.0f), 800);
                }
            }
        }

        public override void ExitTrigger(Collider trigger, Collider invoker) {
            GameObject renderObject = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(renderObjectName);
            if (renderObject != null) {
                QuadRender quadRender = (QuadRender)renderObject.GetComponent(typeof(QuadRender).Name);
                if (quadRender != null) {
                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(0.0f), 800);
                }
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement enabler = doc.CreateElement(typeof(AdvancedTriggerEnabler).Name);
            node.AppendChild(enabler);

            enabler.SetAttribute("renderObjectName", renderObjectName);
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            renderObjectName = node.GetAttribute("renderObjectName");
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            AdvancedTriggerEnabler enabler = new AdvancedTriggerEnabler(gameObject);
            enabler.renderObjectName = renderObjectName;
            return enabler;
        }
    }
}
