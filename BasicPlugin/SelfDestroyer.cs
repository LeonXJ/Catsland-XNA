using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.ComponentModel;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin
{
	public class SelfDestroyer : CatComponent
	{
		int m_timeElipse = 0;
		
        public int m_time = 1000;
        [CategoryAttribute("Behavior")]
        public int LifeTime
        {
            get { return m_time; }
            set { m_time = value; }
        }

		public SelfDestroyer(GameObject gameObject)
			: base(gameObject)
		{

		}

		public override void Update(int timeLastFrame)
		{
			base.Update(timeLastFrame);
			m_timeElipse += timeLastFrame;
			if (m_timeElipse > m_time)
			{
				Mgr<Scene>.Singleton._gameObjectList.RemoveItem(m_gameObject.GUID);
			}
		}

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            m_time = int.Parse(node.GetAttribute("time"));
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            SelfDestroyer newSelfDestroyer = new SelfDestroyer(gameObject);
            newSelfDestroyer.m_time = m_time;
            return newSelfDestroyer;
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement selfDestroyer = doc.CreateElement(typeof(SelfDestroyer).Name);
            selfDestroyer.SetAttribute("time", "" + m_time);
            node.AppendChild(selfDestroyer);
            return true;
        }

	}
}
