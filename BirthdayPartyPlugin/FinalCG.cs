using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using System.Xml;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.BirthdayParty {
    public class FinalCG : CatComponent {

        public string cameraObjectName { set; get; }
        public string ankerObjectName { set; get; }
        public string playerObjectName { set; get; }

        private CatFloat cameraWidth = new CatFloat(0.0f);
        public float CameraWidth {
            get { return cameraWidth; }
            set { cameraWidth.SetValue(value); }
        }

        private bool UpdateZoom = false;
        private bool hasTrigger = false;

        public FinalCG(GameObject gameObject)
            : base(gameObject) {

        }

        public override void Initialize(Catsland.Core.Scene scene) {
            UpdateZoom = false;
            hasTrigger = false;
        }

        public override void Update(int timeLastFrame) {
            if (UpdateZoom) {
                /*Mgr<Camera>.Singleton.ScaleWidthTo(cameraWidth);*/
            }
        }

        public override void InTrigger(Collider trigger, Collider invoker) {
            if (invoker.m_gameObject.Name != playerObjectName) {
                return;
            }
            if (hasTrigger) {
                return;
            }
            hasTrigger = true;

            // disable controller
            GameObject player = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(playerObjectName);
            if (player != null) {
                KeyboardInput input = (KeyboardInput)player.GetComponent(typeof(KeyboardInput).Name);
                if (input != null) {
                    input.Enable = false;
                }
            }
            
            List<string> cameras = Mgr<Scene>.Singleton._gameObjectList.GetGameObjectsGuidByName(cameraObjectName);
            if (cameras != null && cameras.Count > 0) {
                GameObject cameraO = Mgr<Scene>.Singleton._gameObjectList.GetItem(cameras[0]);
                if (cameraO != null) {
                    // switch to absolute position
                    Vector2 absPoistion = cameraO.AbsPositionOld;
                    float absHeight = cameraO.AbsHeight;
                    cameraO.AttachToGameObject(null);
                    cameraO.PositionOld = absPoistion;
                    cameraO.HeightOld = absHeight;
                    // zoom out
                    CameraWidth = Mgr<Camera>.Singleton.ViewSize.X;
                    UpdateZoom = true;
                    // begin show
                    MovieClip movieClip = Mgr<MotionDelegator>.Singleton.AddMovieClip();
                    //1) zoom and move
                    int startTick = movieClip.GetEditCurClip();
                    movieClip.AppendMotion(cameraWidth, new CatFloat(4.0f), 10000);

                    GameObject ankerObject = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(ankerObjectName);
                    if (ankerObject != null) {
                        movieClip.AddMotion(cameraO.PositionRef, ankerObject.PositionRef, startTick, 16000,
                            AnimationClip.PlayMode.CLAMP, 0.6f, MotionDelegatorPack.AccelerationMode.AccelerateNDecelerate);
//                         movieClip.AddMotion(cameraO.m_height, ankerObject._height, startTick, 16000,
//                             AnimationClip.PlayMode.CLAMP, 0.6f, MotionDelegatorPack.AccelerationMode.AccelerateNDecelerate);;
                    }
                    //2) empty time
                    movieClip.AppendEmptyTime(1000);
                    //3) ending
                    string endString = "end0";
                    int i;
                    for (i = 0; i < 7; ++i) {
                        GameObject endObject = Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(endString + i);
                        if (endObject != null) {
                            QuadRender quadRender = (QuadRender)endObject.GetComponent(typeof(QuadRender).Name);
                            if (quadRender != null) {
                                movieClip.AppendMotion(quadRender.alpha, new CatFloat(1.0f), 1000);
                                movieClip.AppendEmptyTime(1000);
                            }
                        }
                    }
                    movieClip.Initialize();
                }
            }
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement finalCG = doc.CreateElement(typeof(FinalCG).Name);
            node.AppendChild(finalCG);

            finalCG.SetAttribute("cameraObjectName", cameraObjectName);
            finalCG.SetAttribute("ankerObjectName", ankerObjectName);
            finalCG.SetAttribute("playerObjectName", playerObjectName);

            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            cameraObjectName = node.GetAttribute("cameraObjectName");
            ankerObjectName = node.GetAttribute("ankerObjectName");
            playerObjectName = node.GetAttribute("playerObjectName");
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            FinalCG newFinalCG = new FinalCG(gameObject);
            newFinalCG.cameraObjectName = cameraObjectName;
            newFinalCG.ankerObjectName = ankerObjectName;
            newFinalCG.playerObjectName = playerObjectName;

            return newFinalCG;
        }
    }
}
