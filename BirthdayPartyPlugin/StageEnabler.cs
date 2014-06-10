using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class StageEnabler : CatComponent {

        private string enablerName = "";
        public string EnablerName {
            set { enablerName = value; }
            get { return enablerName; }
        }

        private string bulletinObjectName = "";
        public string BulletinObjectName {
            set { bulletinObjectName = value; }
            get { return bulletinObjectName; }
        }

        private bool hasEnable = false;
        private string scoreObjectName = "";

        private List<string> stageNames;
        public string StageNameA {
            set {
                stageNames[0] = value;
            }
            get {
                return stageNames[0];
            }
        }
        public string StageNameB {
            set {
                stageNames[1] = value;
            }
            get {
                return stageNames[1];
            }
        }
        public string StageNameC {
            set {
                stageNames[2] = value;
            }
            get {
                return stageNames[2];
            }
        }

        public List<float> targetHeight {
            set;
            get;
        }

        public string ScoreObjectName {
            set { scoreObjectName = value; }
            get { return scoreObjectName; }
        }

        private string lightObjectName = "";
        public string LightObjectName {
            set { lightObjectName = value; }
            get { return lightObjectName; }
        }

        public int TargetScore { set; get; }

        public string BackgroundLightName { set; get; }
        public string BackgroundDarkName { set; get; }
        public string LamentEmitterName { set; get; }

        public StageEnabler(GameObject gameObject)
            : base(gameObject) {
            stageNames = new List<string>();
            stageNames.Add("");
            stageNames.Add("");
            stageNames.Add("");
            targetHeight = new List<float>();
            targetHeight.Add(0);
            targetHeight.Add(0);
            targetHeight.Add(0);
        }

        public override void Initialize(Scene scene) {
            hasEnable = false;
        }

        public override void EnterTrigger(Collider trigger, Collider invoker) {
            if (invoker.m_gameObject.Name != enablerName) {
                return;
            }

            if (hasEnable) {
                return;
            }

            List<string> scores = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(scoreObjectName);
            if (scores != null && scores.Count > 0) {
                GameObject scoreObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(scores[0]);
                ScoreCounter scoreCounter = (ScoreCounter)scoreObject.GetComponent(typeof(ScoreCounter).Name);
                if (scoreCounter != null) {
                    if (scoreCounter.score >= TargetScore) {
                        // pass level
                        #region
                        if (stageNames != null) {
                            int i;
                            for (i = 0; i < stageNames.Count; ++i) {
                                List<string> stages = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(stageNames[i]);
                                if (stages != null & stages.Count > 0) {
                                    GameObject stage = Mgr<Scene>.Singleton._gameObjectList.GetItem(stages[0]);
                                    MovieClip movieClip = Mgr<MotionDelegator>.Singleton.AddMovieClip();
                                    movieClip.AppendEmptyTime(i * 1000);
                                    movieClip.AppendMotion(stage.HeightOld, new CatFloat(targetHeight[i]), 3000);
                                    movieClip.Initialize();
                                }
                            }
                        }
                        // into dark
                        List<string> background = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(BackgroundLightName);
                        if (background != null & background.Count > 0) {
                            GameObject backObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(background[0]);
                            if (backObject != null) {
                                QuadRender quadRender = (QuadRender)backObject.GetComponent(typeof(QuadRender).Name);
                                if (quadRender != null) {
                                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(0.0f), 10000);
                                }
                            }
                        }
                        background = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(BackgroundDarkName);
                        if (background != null & background.Count > 0) {
                            GameObject backObject = Mgr<Scene>.Singleton._gameObjectList.GetItem(background[0]);
                            if (backObject != null) {
                                QuadRender quadRender = (QuadRender)backObject.GetComponent(typeof(QuadRender).Name);
                                if (quadRender != null) {
                                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(1.0f), 5000);
                                }
                            }
                        }
                        // lamtent shot
                        List<string> lamtentShot = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(LamentEmitterName);
                        if (lamtentShot != null && lamtentShot.Count > 0) {
                            GameObject lamtentShotO = Mgr<Scene>.Singleton._gameObjectList.GetItem(lamtentShot[0]);
                            if (lamtentShotO != null) {
                                RandomLocationGenerator randomLG = (RandomLocationGenerator)lamtentShotO.GetComponent(
                                    typeof(RandomLocationGenerator).Name);
                                if (randomLG != null) {
                                    randomLG.Shot(20);
                                    randomLG.Emitting = true;
                                }
                            }
                        }
                        hasEnable = true;
                        #endregion
                    }
                    else {
                        // not pass the level
                        GameObject bulletinO = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(BulletinObjectName);
                        if (bulletinO != null) {
                            QuadRender quadRender = (QuadRender)bulletinO.GetComponent(typeof(QuadRender).Name);
                            if (quadRender != null) {
                                Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(1.0f), 500);
                            }
                        }
                    }

                    // not pass level
                    List<GameObject> lights = m_gameObject.GetGameObjectsByNameFromChildren(lightObjectName);
                    if (lights != null & lights.Count > 0) {
                        QuadRender quadRender = (QuadRender)lights[0].GetComponent(typeof(QuadRender).Name);
                        if (quadRender != null) {
                            quadRender.Alpha = 0.0f;
                            Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(1.0f), 1000);
                        }
                    }

                }
            }

        }

        public override void ExitTrigger(Collider trigger, Collider invoker) {
            if (invoker.m_gameObject.Name != enablerName) {
                return;
            }
            if (hasEnable) {
                return;
            }

            // not pass level
            List<GameObject> lights = m_gameObject.GetGameObjectsByNameFromChildren(lightObjectName);
            if (lights != null & lights.Count > 0) {
                QuadRender quadRender = (QuadRender)lights[0].GetComponent(typeof(QuadRender).Name);
                if (quadRender != null) {
                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(0.0f), 1000);
                }
            }

            GameObject bulletinO = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(BulletinObjectName);
            if (bulletinO != null) {
                QuadRender quadRender = (QuadRender)bulletinO.GetComponent(typeof(QuadRender).Name);
                if (quadRender != null) {
                    Mgr<MotionDelegator>.Singleton.AddMotion(quadRender.alpha, new CatFloat(0.0f), 500);
                }
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement stageEnabler = doc.CreateElement(typeof(StageEnabler).Name);
            node.AppendChild(stageEnabler);

            stageEnabler.SetAttribute("scoreObjectName", scoreObjectName);
            stageEnabler.SetAttribute("lightObjectName", lightObjectName);
            stageEnabler.SetAttribute("targetScore", "" + TargetScore);
            stageEnabler.SetAttribute("enablerName", enablerName);
            stageEnabler.SetAttribute("backgroundLightName", BackgroundLightName);
            stageEnabler.SetAttribute("backgroundDarkName", BackgroundDarkName);
            stageEnabler.SetAttribute("lamtentEmitterName", LamentEmitterName);
            stageEnabler.SetAttribute("bulletinObjectName", BulletinObjectName);

            XmlElement stages = doc.CreateElement("Stages");
            stageEnabler.AppendChild(stages);

            int i;
            for (i = 0; i < stageNames.Count; ++i) {
                XmlElement stage = doc.CreateElement("Stage");
                stages.AppendChild(stage);
                stage.SetAttribute("objectName", stageNames[i]);
                stage.SetAttribute("targetHeight", "" + targetHeight[i]);
            }

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            scoreObjectName = node.GetAttribute("scoreObjectName");
            lightObjectName = node.GetAttribute("lightObjectName");
            TargetScore = int.Parse(node.GetAttribute("targetScore"));
            enablerName = node.GetAttribute("enablerName");
            BackgroundLightName = node.GetAttribute("backgroundLightName");
            BackgroundDarkName = node.GetAttribute("backgroundDarkName");
            LamentEmitterName = node.GetAttribute("lamtentEmitterName");
            BulletinObjectName = node.GetAttribute("bulletinObjectName");

            stageNames = new List<string>();
            targetHeight = new List<float>();
            XmlNode stages = node.SelectSingleNode("Stages");
            foreach (XmlNode stage in stages.ChildNodes) {
                XmlElement stageE = (XmlElement)stage;
                stageNames.Add(stageE.GetAttribute("objectName"));
                targetHeight.Add(float.Parse(stageE.GetAttribute("targetHeight")));
            }

        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            StageEnabler stageEnabler = new StageEnabler(gameObject);
            stageEnabler.scoreObjectName = scoreObjectName;
            stageEnabler.lightObjectName = lightObjectName;
            stageEnabler.TargetScore = TargetScore;
            stageEnabler.enablerName = enablerName;
            stageEnabler.BackgroundDarkName = BackgroundDarkName;
            stageEnabler.BackgroundLightName = BackgroundLightName;
            stageEnabler.LamentEmitterName = LamentEmitterName;
            stageEnabler.BulletinObjectName = BulletinObjectName;

            stageEnabler.stageNames = new List<string>();
            if (stageNames != null) {
                foreach (string stageName in stageNames)
                    stageEnabler.stageNames.Add(stageName);
            }

            stageEnabler.targetHeight = new List<float>();
            if (targetHeight != null) {
                foreach (float target in targetHeight) {
                    stageEnabler.targetHeight.Add(target);
                }
            }
            return stageEnabler;
        }
    }
}
