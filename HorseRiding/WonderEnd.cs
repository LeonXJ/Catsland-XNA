using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class WonderEnd : StaticSensor{

#region Properties

        [SerialAttribute]
        private string m_candleName = "";
        public string CandleName {
            set {
                m_candleName = value;
            }
            get {
                return m_candleName;
            }
        }

        [SerialAttribute]
        private string m_cakeName = "";
        public string CakeName {
            set{
                m_cakeName = value;
            }
            get{
                return m_cakeName;
            }
        }

        [SerialAttribute]
        private string m_playerName = "";
        public string PlayerName {
            set {
                m_playerName = value;
            }
            get {
                return m_playerName;
            }
        }

        [SerialAttribute]
        private string m_finalCameraPositionObjectName = "";
        public string FinalCameraPositionObjectName {
            set {
                m_finalCameraPositionObjectName = value;
            }
            get {
                return m_finalCameraPositionObjectName;
            }
        }

        [SerialAttribute]
        private string m_fireworkControllerName = "";
        public string FireworkControllerName {
            set {
                m_fireworkControllerName = value;
            }
            get {
                return m_fireworkControllerName;
            }
        }

        [SerialAttribute]
        private string m_wishObjectName = "";
        public string WishObjectName {
            set {
                m_wishObjectName = value;
            }
            get {
                return m_wishObjectName;
            }
        }

        [SerialAttribute]
        private string m_hbObjectName = "";
        public string HBObjectName {
            set {
                m_hbObjectName = value;
            }
            get {
                return m_hbObjectName;
            }
        }

        [SerialAttribute]
        private string m_quitObjectName = "";
        public string QuitObjectName {
            set {
                m_quitObjectName = value;
            }
            get {
                return m_quitObjectName;
            }
        }

#endregion

        public WonderEnd() : base() { }
        public WonderEnd(GameObject _gameObject) :
            base(_gameObject) {

        }

        protected override bool Enter(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {

            if (_fixtureA.UserData == null && _fixtureB.UserData == null) {
                return true;
            }
            
            // stop walking and disable camera follower
            GameObject player = Mgr<Scene>.Singleton._gameObjectList.
                GetOneGameObjectByName(m_playerName);
            if (player != null) {
                HorseController controller = player.GetComponent(
                    typeof(HorseController).ToString()) as HorseController;
                if (controller != null) {
                    controller.MotorSpeed = 0.0f;
                }
                CameraFollower cameraFollower = player.GetComponent(
                    typeof(CameraFollower).ToString()) as CameraFollower;
                if (cameraFollower != null) {
                    cameraFollower.Enable = false;
                }
            }

            // light candle
            List<string> candles = Mgr<Scene>.Singleton.
                _gameObjectList.GetGameObjectsGuidByName(m_candleName);
            foreach (string guid in candles) {
                GameObject candle = Mgr<Scene>.Singleton._gameObjectList.GetItem(guid);
                GameObject fire = candle.Children[0];
                ParticleEmitter particleEmitter = fire.GetComponent(
                    typeof(ParticleEmitter).ToString()) as ParticleEmitter;
                if (particleEmitter != null) {
                    particleEmitter.IsEmitting = true;
                }
            }

            int lightDurationInMS = 200;
            // screen light
            MotionDelegator motionDelegator = Mgr<CatProject>.Singleton.MotionDelegator;
            MovieClip movieClip = motionDelegator.AddMovieClip();

            PostProcessColorAdjustment colorAdjustment =
                Mgr<Scene>.Singleton.PostProcessManager.GetPostProcess(
                    typeof(PostProcessColorAdjustment).ToString())
                    as PostProcessColorAdjustment;
            movieClip.AppendMotion(colorAdjustment.IllumiateRef, new CatFloat(1.0f), lightDurationInMS);
            // show cake
            GameObject cake = Mgr<Scene>.Singleton._gameObjectList.
                GetOneGameObjectByName(m_cakeName);
            if(cake != null){
                ModelComponent modelComponent = 
                    cake.GetComponent(typeof(ModelComponent).ToString())
                    as ModelComponent;
                movieClip.AddMotion(
                    modelComponent.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f),
                    movieClip.GetStartTick(),
                    lightDurationInMS);
            }
            // screen dark
            movieClip.AppendMotion(colorAdjustment.IllumiateRef, new CatFloat(0.0f), lightDurationInMS);
            
            // camera up
            int cameraUpTime = movieClip.GetEditCurClip();
            Camera camera = Mgr<Camera>.Singleton;
            camera.ForceUpdateMatrixByCameraUpdate = true;
            camera.TargetObject = null;
            if(m_finalCameraPositionObjectName != ""){
                GameObject finalPosition = 
                    Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(
                        m_finalCameraPositionObjectName);
                if(finalPosition != null){
                    movieClip.AppendEmptyTime(3000);
                    cameraUpTime = movieClip.GetEditCurClip();
                    movieClip.AppendMotion(camera.TargetPositionRef, 
                        new CatVector3(finalPosition.AbsPosition) , 8000, 
                        AnimationClip.PlayMode.CLAMP,
                        0.0f,
                        MotionDelegatorPack.AccelerationMode.AccelerateNDecelerate);
                }
            }
 
            // show wish
            int wishTime = cameraUpTime + 5000;
            ModelComponent wishModel =
                ModelComponent.GetModelOfGameObjectInCurrentScene(m_wishObjectName);
            if (wishModel != null) {
                movieClip.AddMotion(
                    wishModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f), wishTime, 2000);
            }
            int hbTime = wishTime + 3000;
            ModelComponent hbModel =
                ModelComponent.GetModelOfGameObjectInCurrentScene(m_hbObjectName);
            if (hbModel != null) {
                movieClip.AddMotion(
                    hbModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f), hbTime, 2000);
            }
            int escTime = hbTime + 3000;
            ModelComponent escModel =
                ModelComponent.GetModelOfGameObjectInCurrentScene(m_quitObjectName);
            if (escModel != null) {
                movieClip.AddMotion(
                    escModel.GetCatModelInstance().GetMaterial().GetParameter("Alpha"),
                    new CatFloat(1.0f), escTime, 2000);
            }

            // speed up firework
            if (m_fireworkControllerName != "") {
                GameObject firework =
                    Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(
                        m_fireworkControllerName);
                if (firework != null) {
                    FireworkController fc =
                        firework.GetComponent(typeof(FireworkController).ToString())
                        as FireworkController;
                    if (fc != null) {
                        fc.IntervalInMS = 100;
                    }
                }
            }

            movieClip.Initialize();
            return true;
        }
    }
}
