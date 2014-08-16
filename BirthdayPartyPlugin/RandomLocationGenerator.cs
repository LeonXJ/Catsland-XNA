using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Reflection;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class RandomLocationGenerator : CatComponent {

        private CatVector2 xBound = new CatVector2();
        public Vector2 XBound {
            set { xBound.SetValue(value); }
            get { return xBound; }
        }

        private CatVector2 yBound = new CatVector2();
        public Vector2 YBound {
            set { yBound.SetValue(value); }
            get { return yBound; }
        }

        private CatVector2 zBound = new CatVector2();
        public Vector2 ZBound {
            set { zBound.SetValue(value); }
            get { return zBound; }
        }

        private string prefabName;
        public string PrefabName {
            set { prefabName = value; }
            get { return prefabName; }
        }

        public int MillionSecondPerGen { set; get; }
        public float MinScaleFactor { set; get; }

        public bool Emitting { set; get; }

        Random m_random;
        private int accumulatedTime = 0;

        public RandomLocationGenerator(GameObject gameObject)
            : base(gameObject) {

        }

        public override void Initialize(Catsland.Core.Scene scene) {
            m_random = new Random();
        }

        public override void Update(int timeLastFrame) {
            if (!Emitting) {
                return;
            }

            accumulatedTime += timeLastFrame;
            while (accumulatedTime > MillionSecondPerGen) {
                accumulatedTime -= MillionSecondPerGen;
                generateOne();
            }
  
        }

        public void Shot(int number) {
            int i;
            for (i = 0; i < number; ++i) {
                generateOne();
            }
        }

        private void generateOne() {
            // random position
            float X = m_gameObject.AbsPositionOld.X + (float)(XBound.X + (XBound.Y - XBound.X) * m_random.NextDouble()); 
            float Z = m_gameObject.AbsHeight + (float)(ZBound.X + (ZBound.Y - ZBound.X) * m_random.NextDouble());

            // random speed, size and alpha
            float scaleFactor = (float)(MinScaleFactor + (1.0f - MinScaleFactor) * m_random.NextDouble());

            float Y = m_gameObject.AbsPositionOld.Y + (float)(YBound.Y - (YBound.Y - YBound.X) * scaleFactor);

            // generate gameObject
            GameObject prefab = Mgr<CatProject>.Singleton.prefabList.GetItem(prefabName);
            if (prefab != null) {
                GameObject newGameObject = prefab.DoClone() as GameObject;
                // set position
                newGameObject.PositionOld = new Vector2(X, Y);
                newGameObject.HeightOld = Z;

                // set lamtent
                Lamtent lamtent = (Lamtent)newGameObject.GetComponent(typeof(Lamtent).Name);
                if (lamtent != null) {
                    lamtent.SetLamtent(scaleFactor);
                }

                // add to scene
                Mgr<Scene>.Singleton._gameObjectList.AddGameObject(newGameObject);
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement randomLG = doc.CreateElement(typeof(RandomLocationGenerator).Name);
            node.AppendChild(randomLG);

            randomLG.SetAttribute("prefabName", prefabName);
            randomLG.SetAttribute("generatePerSecond", "" + MillionSecondPerGen);
            randomLG.SetAttribute("minScaleFator", "" + MinScaleFactor);
            randomLG.SetAttribute("emitting", "" + Emitting);

            XmlElement xbound = doc.CreateElement("XBound");
            randomLG.AppendChild(xbound);
            xbound.SetAttribute("min", "" + XBound.X);
            xbound.SetAttribute("max", "" + XBound.Y);

            XmlElement ybound = doc.CreateElement("YBound");
            randomLG.AppendChild(ybound);
            ybound.SetAttribute("min", "" + YBound.X);
            ybound.SetAttribute("max", "" + YBound.Y);

            XmlElement zbound = doc.CreateElement("ZBound");
            randomLG.AppendChild(zbound);
            zbound.SetAttribute("min", "" + ZBound.X);
            zbound.SetAttribute("max", "" + ZBound.Y);

            /*
            PropertyInfo[] properties = typeof(RandomLocationGenerator).GetProperties();

            foreach (PropertyInfo property in properties) {
                randomeLG.SetAttribute(property.Name, "" + property.GetValue(this, null));
            }
             */ 
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {

            prefabName = node.GetAttribute("prefabName");
            MillionSecondPerGen = int.Parse(node.GetAttribute("generatePerSecond"));
            MinScaleFactor = float.Parse(node.GetAttribute("minScaleFator"));
            Emitting = bool.Parse(node.GetAttribute("emitting"));

            XmlElement xBound = (XmlElement)node.SelectSingleNode("XBound");
            XBound = new Vector2(float.Parse(xBound.GetAttribute("min")),
                float.Parse(xBound.GetAttribute("max")));

            XmlElement yBound = (XmlElement)node.SelectSingleNode("YBound");
            YBound = new Vector2(float.Parse(yBound.GetAttribute("min")),
                float.Parse(yBound.GetAttribute("max")));

            XmlElement zBound = (XmlElement)node.SelectSingleNode("ZBound");
            ZBound = new Vector2(float.Parse(zBound.GetAttribute("min")),
                float.Parse(zBound.GetAttribute("max")));

            /*
            PropertyInfo[] properties = typeof(RandomLocationGenerator).GetProperties();
            foreach (PropertyInfo property in properties) {
                if (property.PropertyType == typeof(int)) {
                    property.SetValue(this, int.Parse(node.GetAttribute(property.Name)), null);
                }
                else {
                    // TODO
                }
                
            }
             */ 
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            RandomLocationGenerator newRandomLG = new RandomLocationGenerator(gameObject);
            PropertyInfo[] properties = typeof(RandomLocationGenerator).GetProperties();

            foreach (PropertyInfo property in properties) {
                property.SetValue(newRandomLG, property.GetValue(this, null), null);
            }
            return newRandomLG;
        }
    }
}
