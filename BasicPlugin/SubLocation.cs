using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Catsland.Editor;
using System.Xml;
using System.Diagnostics;

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


        private Vector3 m_relatedPosition = new Vector3();
        [CategoryAttribute("Location")]
        public Vector3 RelatedPosition
        {
            get { return m_relatedPosition; }
            set { m_relatedPosition = value; }
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
                m_gameObject.Position = m_parent.Position + m_relatedPosition;

            }
        }

//         public override bool SaveToNode(XmlNode node, XmlDocument doc)
//         {
// 
//             Debug.Assert(false, "Rewrite this");
//             XmlElement subLocation = doc.CreateElement(typeof(SubLocation).Name);
//             node.AppendChild(subLocation);
// 
//             if (m_parent != null)
//             {
//                 subLocation.SetAttribute("parentGameObject", parent_guid);
//             }
//             else
//             {
//                 subLocation.SetAttribute("parentGameObject", "");
//             }
// 
//             // location
//             XmlElement location = doc.CreateElement("Location");
//             subLocation.AppendChild(location);
//             location.SetAttribute("X", "" + m_relatedPosition.X);
//             location.SetAttribute("Y", "" + m_relatedPosition.Y);
// 
//             return true;
//         }
// 
//         public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
//         {
//             Debug.Assert(false, "Rewrite this");
// //             base.ConfigureFromNode(node, scene, gameObject);
// // 
// //             string guid = node.GetAttribute("parentGameObject");
// //             if (guid != "")
// //             {
// //                 parent_guid = guid;
// //             }
// //             else
// //             {
// //                 parent_guid = null;
// //             }
// // 
// //             // Location
// //             XmlElement location = (XmlElement)node.SelectSingleNode("Location");
// //             m_relatedPosition = new Vector2(float.Parse(location.GetAttribute("X")),
// //                                             float.Parse(location.GetAttribute("Y")));
//         }
// 
//         public override CatComponent CloneComponent(GameObject gameObject)
//         {
//             Debug.Assert(false, "Rewrite this");
//             return null;
// //             SubLocation subLocation = new SubLocation(gameObject);
// //             subLocation.m_relatedPosition.X = m_relatedPosition.X;
// //             subLocation.m_relatedPosition.Y = m_relatedPosition.Y;
// //             subLocation.m_relatedHeight = m_relatedHeight;
// //             subLocation.parent_guid = parent_guid;
// // 
// //             return subLocation;
//         }

        public static string GetMenuNames() {
            return "Controller|Follow";
        }
    }
}
