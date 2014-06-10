using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class Enlight : CatComponent {
        private bool enlighted = false;

        private string scoreCounterName = "";
        public string ScoreCounterName {
            set { scoreCounterName = value; }
            get { return scoreCounterName; }
        }

        private string startEmitObjectName = "";
        public string StartEmitObjectName {
            set { startEmitObjectName = value; }
            get { return startEmitObjectName; }
        }

        private string hideObjectName = "";
        public string HideObjectName {
            set { hideObjectName = value; }
            get { return hideObjectName; }
        }

        private string enlighterName = "";
        public string EnlighterName {
            set { enlighterName = value; }
            get { return enlighterName; }
        }

        private string fireName = "";
        public string FireName {
            set { fireName = value; }
            get { return fireName; }
        }

        private string lightName = "";
        public string LightName {
            set { lightName = value; }
            get { return lightName; }
        }

        private string animateObjectName = "";
        public string AnimateObjectName {
            set { animateObjectName = value; }
            get { return animateObjectName; }
        }

        public string animationName {
            get;
            set;
        }

        public Enlight(GameObject gameObject)
            : base(gameObject) { }

        public override void Initialize(Scene scene) {
            enlighted = false;
        }

        public override void InTrigger(Collider trigger, Collider invoker) {
            if (enlighted) {
                return;
            }

            MotionDelegator motionDelegator = Mgr<MotionDelegator>.Singleton;


            if (invoker.m_gameObject.Name == enlighterName) {
                List<GameObject> fires = m_gameObject.GetGameObjectsByNameFromChildren(fireName);
                if (fires != null && fires.Count > 0) {
                    QuadRender quadRender = (QuadRender)fires[0].GetComponent(typeof(QuadRender).Name);
                    if (quadRender != null) {
                        quadRender.Alpha = 0.0f;
                        quadRender.Enable = true;
                        if (motionDelegator != null) {
                            motionDelegator.AddMotion(quadRender.alpha, new CatFloat(1.0f),
                                1000);
                        }
                    }
                }
                List<GameObject> lights = m_gameObject.GetGameObjectsByNameFromChildren(lightName);
                if (lights != null && lights.Count > 0) {
                    QuadRender quadRender = (QuadRender)lights[0].GetComponent(typeof(QuadRender).Name);
                    if (quadRender != null) {
                        quadRender.Alpha = 0.0f;
                        quadRender.Enable = true;
                        if (motionDelegator != null) {
                            motionDelegator.AddMotion(quadRender.alpha, new CatFloat(1.0f),
                                1000);
                        }
                    }
                }

                List<GameObject> animates = m_gameObject.GetGameObjectsByNameFromChildren(animateObjectName);
                if (animates != null && animates.Count > 0) {
                    Animator animator = (Animator)animates[0].GetComponent(typeof(Animator).Name);
                    if (animates != null) {
                        animator.PlayAnimation(animationName);
                    }
                }

                List<string> counterNames = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(scoreCounterName);
                if (counterNames != null && counterNames.Count > 0) {
                    GameObject counterObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(counterNames[0]);
                    if (counterObject != null) {
                        ScoreCounter counter = (ScoreCounter)counterObject.GetComponent(typeof(ScoreCounter).Name);
                        if (counter != null) {
                            counter.score += 1;
                        }
                    }
                }

                List<GameObject> hides = m_gameObject.GetGameObjectsByNameFromChildren(hideObjectName);
                if (hides != null && hides.Count > 0) {
                    QuadRender quadRender = (QuadRender)hides[0].GetComponent(typeof(QuadRender).Name);
                    if (quadRender != null) {
                        if (motionDelegator != null) {
                            motionDelegator.AddMotion(quadRender.alpha, new CatFloat(0.0f),
                                1000);
                        }
                    }
                }

                List<GameObject> emits = m_gameObject.GetGameObjectsByNameFromChildren(startEmitObjectName);
                if (emits != null && emits.Count > 0) {
                    ParticleEmitter emitter = (ParticleEmitter)emits[0].GetComponent(typeof(ParticleEmitter).Name);
                    if (emitter != null) {
                        /*emitter.m_isEmitting = true;*/
                    }
                }

                enlighted = true;
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement enlight = doc.CreateElement(typeof(Enlight).Name);
            node.AppendChild(enlight);

            enlight.SetAttribute("enlighter", EnlighterName);
            enlight.SetAttribute("fireName", fireName);
            enlight.SetAttribute("lightName", lightName);
            enlight.SetAttribute("scoreCounterName", scoreCounterName);
            enlight.SetAttribute("animateObjectName", animateObjectName);
            enlight.SetAttribute("animationName", animationName);
            enlight.SetAttribute("hideObjectName", hideObjectName);
            enlight.SetAttribute("emitterObjectName", startEmitObjectName);

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            EnlighterName = node.GetAttribute("enlighter");
            fireName = node.GetAttribute("fireName");
            lightName = node.GetAttribute("lightName");
            scoreCounterName = node.GetAttribute("scoreCounterName");
            animateObjectName = node.GetAttribute("animateObjectName");
            animationName = node.GetAttribute("animationName");
            hideObjectName = node.GetAttribute("hideObjectName");
            startEmitObjectName = node.GetAttribute("emitterObjectName");
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            Enlight newEnlight = new Enlight(gameObject);
            newEnlight.EnlighterName = EnlighterName;
            newEnlight.fireName = fireName;
            newEnlight.lightName = lightName;
            newEnlight.scoreCounterName = scoreCounterName;
            newEnlight.animateObjectName = animateObjectName;
            newEnlight.animationName = animationName;
            newEnlight.hideObjectName = hideObjectName;
            newEnlight.startEmitObjectName = startEmitObjectName;

            return newEnlight;
        }
    }

}
