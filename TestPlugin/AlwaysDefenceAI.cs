using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.TestPlugin {
    class AlwaysDefenceAI : CatComponent {
        public AlwaysDefenceAI(GameObject gameObject)
            : base(gameObject) {
        }

        public override void Update(int timeLastFrame) {
            CharacterController characterController =
                (CharacterController)m_gameObject.GetComponent(typeof(CharacterController).Name);
            if (characterController != null) {
                characterController.m_wantDefence = true;
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement alwaysDefenceAI = doc.CreateElement(typeof(AlwaysDefenceAI).Name);
            node.AppendChild(alwaysDefenceAI);
            return true;
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            return new AlwaysDefenceAI(gameObject);
        }
    }
}
