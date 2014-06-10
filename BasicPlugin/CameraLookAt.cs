using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Microsoft.Xna.Framework;
namespace Catsland.Plugin.BasicPlugin {
    class CameraLookAt : CatComponent {
              
#region Properties

        [SerialAttribute]
        private readonly CatBool m_isOnlyX = new CatBool(false);
        public bool IsOnlyX {
            set {
                m_isOnlyX.SetValue(value);
            }
            get {
                return m_isOnlyX;
            }
        }

#endregion

        public CameraLookAt(GameObject gameObject)
            : base(gameObject) {

        }

        public CameraLookAt() : base() { }

        public override void Update(int timeLastFrame) {
            Camera camera = Mgr<Camera>.Singleton;
            if (m_isOnlyX) {
                camera.TargetPosition = new Vector3(m_gameObject.AbsPosition.X,
                                                    camera.TargetPosition.Y,
                                                    camera.TargetPosition.Z);
            }
            else {
                camera.TargetObject = m_gameObject;
            }
            
            
        }

        public override void EditorUpdate(int timeLastFrame) {
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            base.ConfigureFromNode(node, scene, gameObject);
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement cameraLookAt = doc.CreateElement(typeof(CameraLookAt).Name);
            node.AppendChild(cameraLookAt);
            return true;
        }

        public static string GetMenuNames() {
            return "Controller|Camera Look At";
        }
    }
}
