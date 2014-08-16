using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;
using System.Diagnostics;

namespace Catsland.Plugin.BasicPlugin
{
	public class BackgroundController : CatComponent
	{
		private float m_zOffset = 0.0f;
        [CategoryAttribute("Basic")]
        public float ZOffset
        {
            get { return m_zOffset; }
            set { m_zOffset = value; }
        }

		private bool m_yAdjust = true;
        [CategoryAttribute("Basic")]
        public bool AdjustY
        {
            get { return m_yAdjust; }
            set { m_yAdjust = value; }
        }

        public bool UseThisWidth { get; set; }
        private CatFloat pictureWidth = new CatFloat(0.0f);
        public float PictureWidth {
            set { pictureWidth.SetValue(value); }
            get { return pictureWidth.GetValue(); }
        }
        private CatFloat pictureOffsetX = new CatFloat(0.0f);
        public float PictureOffsetX {
            set { pictureOffsetX.SetValue(value); }
            get { return pictureOffsetX.GetValue(); }
        }

		public BackgroundController(GameObject gameObject)
			:base(gameObject)
		{

		}

        public override void EditorUpdate(int timeLastFrame)
        {
            base.EditorUpdate(timeLastFrame);
            Update(timeLastFrame);
        }

		public override void Update(int timeLastFrame)
		{
			base.Update(timeLastFrame);

            QuadRender quadRender = (QuadRender)m_gameObject.GetComponent(typeof(QuadRender));

            if (quadRender == null)
			{
				Console.WriteLine("Error! BackgroundController needs QuadRender Component.");
				return;
            }

            float quadWidth = 0.0f;
            if (UseThisWidth) {
                quadWidth = PictureWidth;
            }
            else {
                
                quadWidth = quadRender.Size.X;
            }            
            
            float viewWidth = Mgr<Camera>.Singleton.maxWidth;
			float cameraX = Mgr<Camera>.Singleton.CameraPosition.X;

			float activeWidth = quadWidth - viewWidth;

			Vector2 sceneXBound = Mgr<Scene>.Singleton._XBound;

            float cameraPositionPercent = (cameraX - sceneXBound.X) / (sceneXBound.Y - sceneXBound.X);

            if (!UseThisWidth) {
                m_gameObject.Position = new Vector3(cameraX + activeWidth / 2.0f - cameraPositionPercent * activeWidth,
                    m_gameObject.Position.Y, m_gameObject.Position.Z);
            }
            else {
                m_gameObject.Position = new Vector3(pictureOffsetX + cameraX + activeWidth / 2.0f - cameraPositionPercent * activeWidth,
                m_gameObject.Position.Y, m_gameObject.Position.Z);
            }
            
			
            if (m_yAdjust)
			{
                Debug.Assert(false, "Rewrite this");
			}
             
		}

// 		public override bool SaveToNode(XmlNode node, XmlDocument doc)
// 		{
// 			XmlElement backgroundController = doc.CreateElement("BackgroundController");
// 			node.AppendChild(backgroundController);
// 
// 			backgroundController.SetAttribute("zOffset", "" + m_zOffset);
// 			backgroundController.SetAttribute("adjustY", "" + m_yAdjust);
//             backgroundController.SetAttribute("useThisWidth", "" + UseThisWidth);
//             backgroundController.SetAttribute("pictureWidth", "" + PictureWidth);
//             backgroundController.SetAttribute("pictureOffsetX", "" + PictureOffsetX);
// 
// 			return true;
// 		}
// 
//         public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
//         {
//             base.ConfigureFromNode(node, scene, gameObject);
// 
//             m_zOffset = float.Parse(node.GetAttribute("zOffset"));
//             m_yAdjust = bool.Parse(node.GetAttribute("adjustY"));
//             
//             UseThisWidth = bool.Parse(node.GetAttribute("useThisWidth"));
//             PictureWidth = float.Parse(node.GetAttribute("pictureWidth"));
//         
//             
//         }

        public static string GetMenuNames() {
            return "Controller|Background Controller";
        }

//         public override CatComponent CloneComponent(GameObject gameObject) {
//             BackgroundController bkController = new BackgroundController(gameObject);
// 
//             bkController.m_zOffset = m_zOffset;
//             bkController.m_yAdjust = m_yAdjust;
//             bkController.UseThisWidth = UseThisWidth;
//             bkController.PictureWidth = PictureWidth;
//             bkController.PictureOffsetX = PictureOffsetX;
// 
//             return bkController;
//         }
	}
}
