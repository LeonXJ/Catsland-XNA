using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Catsland.Editor;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Xml;
using CatsEditor;

namespace Catsland.Plugin.BasicPlugin
{
    class CatsCollider : CatComponent, Drawable
    {
        [CategoryAttribute("Behavior"),
            EditorAttribute(typeof(PropertyGridComponentSelector),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string TriggerInvoker
        {
            get { return m_collider.m_triagerInvoker; }
            set { m_collider.m_triagerInvoker = value; }
        }

        [CategoryAttribute("Basic")]
        public bool AdvantageTrigger
        {
            get { return m_collider.AdvantageTrigger; }
            set { m_collider.AdvantageTrigger = value; }
        }

        [CategoryAttribute("Basic")]
        public Collider.ColliderType ColliderTypeAttribute
        {
            get { return m_collider.ColliderTypeAttribute; }
            set { m_collider.ColliderTypeAttribute = value; }
        }

        [CategoryAttribute("Bound")]
        public Vector2 XBound
        {
            get { return m_collider.m_rootBounding.m_XBound; }
            set
            {
                m_collider.XBound = value;
            }
        }

        [CategoryAttribute("Bound")]
        public Vector2 YBound
        {
            get { return m_collider.m_rootBounding.m_YBound; }
            set
            {
                m_collider.YBound = value;
            }
        }

        [CategoryAttribute("Bound")]
        public Vector2 ZBound
        {
            get { return m_collider.m_rootBounding.m_heightRange; }
            set
            {
                m_collider.ZBound = value;
            }
        }

        public Collider m_collider;
        public CatsCollider(GameObject gameObject)
            : base(gameObject)
        {
            m_collider = new Collider(gameObject);
        }

        public override CatComponent CloneComponent(GameObject gameObject)
        {
            CatsCollider catsCollider = new CatsCollider(gameObject);
            catsCollider.m_collider = (Collider)m_collider.CloneComponent(gameObject);

            return catsCollider;
        }

        public override void BindToScene(Scene scene)
        {
            base.BindToScene(scene);
            m_collider.BindToScene(scene);
        }

        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);
            m_collider.Initialize(scene);
        }

        public override void Update(int timeLastFrame)
        {
            base.Update(timeLastFrame);
            m_collider.Update(timeLastFrame);
        }

        void Drawable.Draw(int timeLastFrame)
        {
            ((Drawable)m_collider).Draw(timeLastFrame);
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc)
        {
            XmlElement catsCollider = doc.CreateElement("CatsCollider");
            node.AppendChild(catsCollider);
            m_collider.SaveToNode(catsCollider, doc);
            return true;
        }

        public override void ConfigureFromNode(System.Xml.XmlElement node, Scene scene, GameObject gameObject)
        {
            XmlElement collider = (XmlElement)node.SelectSingleNode("Collider");
            m_collider.ConfigureFromNode(collider, scene, gameObject);
        }

        public override void Destroy()
        {
            m_collider.Destroy();
            base.Destroy();
        }

        public float GetDepth()
        {
            return m_collider.GetDepth();
        }

        public int CompareTo(object obj)
        {
            return m_collider.CompareTo(obj);
        }
    }
}
