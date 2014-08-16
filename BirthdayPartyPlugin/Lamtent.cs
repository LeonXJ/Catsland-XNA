using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class Lamtent : CatComponent {

        public float RiseSpeed { set; get; }
        private float actualSpeed = 0.2f;
        public float ExpireHight { set; get; }

        public Lamtent(GameObject gameObject)
            : base(gameObject) {

        }


        public void SetLamtent(float scaleFactor) {
            // scale Factor [0,1] decide size, speed and alpha
            QuadRender quadRender = (QuadRender)m_gameObject.GetComponent(typeof(QuadRender).Name);
            if (quadRender != null) {
                // size
                quadRender.Size *= scaleFactor;
                // alpha
                //quadRender.Alpha = scaleFactor;
            }
            // background controller
            BackgroundController bkController = (BackgroundController)m_gameObject.GetComponent(typeof(BackgroundController).Name);
            if (bkController != null) {
                bkController.UseThisWidth = true;
                bkController.PictureWidth = Mgr<Camera>.Singleton.maxWidth + 
                    (bkController.PictureWidth - Mgr<Camera>.Singleton.maxWidth) * scaleFactor;
                bkController.PictureOffsetX = m_gameObject.PositionOld.X;
            }

            // speed
            actualSpeed = RiseSpeed * scaleFactor;
        }

        public override void Update(int timeLastFrame) {
            m_gameObject.HeightOld += actualSpeed * timeLastFrame / 1000;
            if (m_gameObject.HeightOld > ExpireHight) {
                Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(m_gameObject.GUID);
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement lamtent = doc.CreateElement(typeof(Lamtent).Name);
            node.AppendChild(lamtent);

            lamtent.SetAttribute("rizeSpeed", "" + RiseSpeed);
            lamtent.SetAttribute("expireHight", "" + ExpireHight);
            
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            RiseSpeed = float.Parse(node.GetAttribute("rizeSpeed"));
            ExpireHight = float.Parse(node.GetAttribute("expireHight"));
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            Lamtent newLamtent = new Lamtent(gameObject);
            newLamtent.ExpireHight = ExpireHight;
            newLamtent.RiseSpeed = RiseSpeed;
            return newLamtent;
        }
    }
}
