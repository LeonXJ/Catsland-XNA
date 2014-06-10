using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Catsland.Editor;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin
{
    class SubLocation : CatComponent
    {
        // TODO: to bind to other gameObject, every gameObject should be given a unique id

        GameObject m_parent = null;

        [CategoryAttribute("Basic"),
            EditorAttribute(typeof(PropertyGridGameObjectSelector),
            typeof(System.Drawing.Design.UITypeEditor))]
        public GameObject Parent
        {
            get { return m_parent; }
            set 
            { 
                m_parent = value;
                parent_guid = m_parent.GUID;
            }
        }
        
        string parent_guid;


        private Vector2 m_relatedPosition = new Vector2();
        [CategoryAttribute("Location")]
        public Vector2 RelatedPosition
        {
            get { return m_relatedPosition; }
            set { m_relatedPosition = value; }
        }
        private float m_relatedHeight = 0.0f;
        [CategoryAttribute("Location")]
        public float RelatedHeight
        {
            get { return m_relatedHeight; }
            set { m_relatedHeight = value; }
        }


        public SubLocation(GameObject gameObject)
            : base(gameObject)
        {}

        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);
            if (parent_guid == null)
            {
                Console.Out.WriteLine("Guid not defined yet.");
                return;
            }

            BindToParent(scene);
        }

        public void BindToParent(Scene scene)
        {
            if (scene._gameObjectList.ContainKey(parent_guid))
            {
                m_parent = scene._gameObjectList.GetItem(parent_guid);
            }
            else
            {
                Console.Out.WriteLine("Could not find parent. Parent guid: " + parent_guid);
            }
        }

        public override void Update(int timeLastFrame)
        {
            base.Update(timeLastFrame);

            if (m_parent != null)
            {
                m_gameObject.PositionOld = m_parent.PositionOld + m_relatedPosition;
                m_gameObject.HeightOld = m_parent.HeightOld + m_relatedHeight;
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc)
        {
            XmlElement subLocation = doc.CreateElement(typeof(SubLocation).Name);
            node.AppendChild(subLocation);

            if (m_parent != null)
            {
                subLocation.SetAttribute("parentGameObject", parent_guid);
            }
            else
            {
                subLocation.SetAttribute("parentGameObject", "");
            }

            // location
            XmlElement location = doc.CreateElement("Location");
            subLocation.AppendChild(location);
            location.SetAttribute("X", "" + m_relatedPosition.X);
            location.SetAttribute("Y", "" + m_relatedPosition.Y);
            location.SetAttribute("height", "" + m_relatedHeight);

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);

            string guid = node.GetAttribute("parentGameObject");
            if (guid != "")
            {
                parent_guid = guid;
            }
            else
            {
                parent_guid = null;
            }

            // Location
            XmlElement location = (XmlElement)node.SelectSingleNode("Location");
            m_relatedPosition = new Vector2(float.Parse(location.GetAttribute("X")),
                                            float.Parse(location.GetAttribute("Y")));
            m_relatedHeight = float.Parse(location.GetAttribute("height"));
        }

        public override CatComponent CloneComponent(GameObject gameObject)
        {
            SubLocation subLocation = new SubLocation(gameObject);
            subLocation.m_relatedPosition.X = m_relatedPosition.X;
            subLocation.m_relatedPosition.Y = m_relatedPosition.Y;
            subLocation.m_relatedHeight = m_relatedHeight;
            subLocation.parent_guid = parent_guid;

            return subLocation;
        }

        public static string GetMenuNames() {
            return "Controller|Follow";
        }
    }
}
