using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.ComponentModel;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Catsland.Plugin.BasicPlugin {
    class Shadow : CatComponent {
        private GameObject target;
        private string target_guid;

        [EditorAttribute(typeof(PropertyGridGameObjectSelector),
         typeof(System.Drawing.Design.UITypeEditor))]
        public GameObject Target {
            set { target = value; }
            get { return target; }
        }

        private float height;
        public float Height {
            set { height = value; }
            get { return height; }
        }

        private float disappearHeight = 0.2f;
        public float DisappearHeight {
            set { disappearHeight = value; }
            get { return disappearHeight; }
        }

        public Shadow(GameObject gameObject)
			: base(gameObject)
		{
		}

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
        
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            Collider collider = null;
            if (target != null) {
                m_gameObject.PositionOld = target.AbsPositionOld + new Vector2(0.0f, 0.0001f);
                m_gameObject.HeightOld = target.AbsHeight;
                collider = ((CatsCollider)target.GetComponent(typeof(CatsCollider).Name)).m_collider;
            }
            // ray vertical down to get first hit, if not hit set to 0
            float logicalHeight = 0.0f;
            HitInfoPack hitInfoPack = Mgr<Scene>.Singleton._colliderList.VerticalDownRayHit(
                m_gameObject.PositionOld, m_gameObject.HeightOld + 0.1f, collider);
            if (hitInfoPack.IsHit) {
                logicalHeight = hitInfoPack.HitPoint.Z;
                // move the Y to the hit surface so that the shadow is drawn right on the surface
                m_gameObject.PositionOld = new Vector2(m_gameObject.PositionOld.X , hitInfoPack.HitGameObject.AbsPositionOld.Y - 0.0001f);
                // move height so that the shadow is on the stand point
                m_gameObject.HeightOld = hitInfoPack.HitPoint.Z 
                    - (m_gameObject.PositionOld.Y - target.AbsPositionOld.Y) * Mgr<Scene>.Singleton._ySin / Mgr<Scene>.Singleton._yCos;
            }
            else{
                m_gameObject.HeightOld = 0.0f;
            }
            
            
            float alpha = 1.0f;
            float deltaHeight = target.AbsHeight - logicalHeight;
            if(deltaHeight < 0.0f){
                deltaHeight = -deltaHeight;
            }
            if( deltaHeight > disappearHeight){
                alpha = 0.0f;
            }
            else{
                alpha = 1.0f - deltaHeight/disappearHeight;
            }
             
            ((QuadRender)m_gameObject.GetComponent("QuadRender")).Alpha = alpha;
             
        }

        public override bool SaveToNode(System.Xml.XmlNode node, System.Xml.XmlDocument doc) {
            XmlElement shadow = doc.CreateElement(typeof(Shadow).Name);
            node.AppendChild(shadow);

            if (target != null) {
                shadow.SetAttribute("target", target.GUID);
            }
            else {
                shadow.SetAttribute("target", "");
            }

            shadow.SetAttribute("height", "" + height);
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            base.ConfigureFromNode(node, scene, gameObject);
            // delay binding
            target_guid = node.GetAttribute("target");
            height = float.Parse(node.GetAttribute("height"));

        }

        public override void PostConfiguration(Scene scene, Dictionary<string, GameObject> tempList) {
            target = tempList[target_guid];
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            Shadow newShadow = new Shadow(gameObject);
            newShadow.height = height;
            // warning: do not bind the target, because it would change when be cloned
            return newShadow;
        }

        public static string GetMenuNames() {
            return "Controller|Shadow";
        }
    }
}
