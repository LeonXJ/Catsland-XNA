using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin {
    public class ParticleController : CatComponent {

        private bool preOnGround = true;
        public float dustVelocity { get; set;}

        public ParticleController(GameObject gameObject)
            : base(gameObject) {
        }

        public override void Update(int timeLastFrame) {
            ParticleEmitter particleEmitter =
                (ParticleEmitter)m_gameObject.GetComponent(typeof(ParticleEmitter).Name);
            CharacterController characterController =
                (CharacterController)m_gameObject.GetComponent(typeof(CharacterController).Name);
            if (particleEmitter != null && characterController != null) {
                bool curOnGround = characterController.m_isOnGround;
                // drop on ground
                if (!preOnGround && curOnGround) {
                    particleEmitter.OneShot(32);
                }
                // running dust
//                 if (curOnGround) {
//                     float runningSpeed_2 = characterController.m_XYspeed.LengthSquared();
//                     if (runningSpeed_2 >= dustVelocity * dustVelocity) {
//                         particleEmitter.m_isEmitting = true;
//                     }
//                     else {
//                         particleEmitter.m_isEmitting = false;
//                     }
//                 }
//                 else {
//                     particleEmitter.m_isEmitting = false;
//                 }
                preOnGround = curOnGround;
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement particleController = doc.CreateElement(typeof(ParticleController).Name);
            node.AppendChild(particleController);

            particleController.SetAttribute("dustVelocity", "" + dustVelocity);
            
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            dustVelocity = float.Parse(node.GetAttribute("dustVelocity"));
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            ParticleController newParticleController = new ParticleController(gameObject);
            newParticleController.dustVelocity = dustVelocity;
            return newParticleController;
        }
    }
}
