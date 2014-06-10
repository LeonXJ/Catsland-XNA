using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class BirthdayInit : CatComponent {

        GameObject rabCatty = null;
        private bool veryFirst = true;

        public BirthdayInit(GameObject gameObject)
            : base(gameObject) {

        }

        public override void Initialize(Scene scene) {
            List<string> rabcattys = scene._gameObjectList.GetGameObjectsGuidByName("rabcatty");
            if (rabcattys != null && rabcattys.Count > 0) {
                rabCatty = scene._gameObjectList.GetItem(rabcattys[0]);
            }
           // Mgr<Camera>.Singleton.ScaleWidthTo(1.0f);
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            if (veryFirst) {
                //Mgr<Camera>.Singleton.ViewSize();
                veryFirst = false;

                // CG
                MovieClip movieClip = Mgr<MotionDelegator>.Singleton.AddMovieClip();
                // 1) logo
                movieClip.AppendEmptyTime(1000);
                GameObject logo = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName("logo");
                if (logo != null) {
                    QuadRender quadRender = (QuadRender)logo.GetComponent(typeof(QuadRender).Name);
                    if (quadRender != null) {
                        movieClip.AppendMotion(quadRender.alpha, new CatFloat(1.0f), 1000);
                        movieClip.AppendEmptyTime(1000);
                        movieClip.AppendMotion(quadRender.alpha, new CatFloat(0.0f), 1000);
                    }
                }
                // 2) black
                GameObject black = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName("black");
                if (logo != null) {
                    QuadRender quadRender = (QuadRender)black.GetComponent(typeof(QuadRender).Name);
                    if (quadRender != null) {
                        movieClip.AppendMotion(quadRender.alpha, new CatFloat(0.0f), 3000);
                    }
                }

                movieClip.Initialize();
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement birthdayInit = doc.CreateElement(typeof(BirthdayInit).Name);
            node.AppendChild(birthdayInit);
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            base.ConfigureFromNode(node, scene, gameObject);
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            return base.CloneComponent(gameObject);
        }
    }
}
