using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Catsland.Plugin.BasicPlugin;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Catsland.Plugin.TestPlugin {
    public class SkyController : CatComponent{

#region Properties

        [SerialAttribute]
        private readonly CatInteger m_dayDurationInMS = new CatInteger(10000);
        public int DayDurationInMS {
            set {
                m_dayDurationInMS.SetValue((int)MathHelper.Max(value, 1.0f));
            }
            get {
                return m_dayDurationInMS;
            }
        }

        [SerialAttribute]
        private string m_ambientLightMapName = "";
        public string AmbientLightMapName{
            set{
                m_ambientLightMapName = value;
                ApplyAmbientColorMap();
            }
            get{
                return m_ambientLightMapName;
            }
        }

        [SerialAttribute]
        private string m_diffuseLightMapName = "";
        public string DiffuseLightMapName {
            set {
                m_diffuseLightMapName = value;
                ApplyDiffuseColorMap();
            }
            get {
                return m_diffuseLightMapName;
            }
        }

        [SerialAttribute]
        private string m_diffuseLightGameObject = "";
        public string DiffuseLightGameObject {
            set {
                List<Light> candidates = new List<Light>();
                List<string> objs = m_gameObject.Scene._gameObjectList.GetGameObjectsGuidByName(value);
                foreach (string obj in objs) {
                    GameObject gameObject = m_gameObject.Scene._gameObjectList.GetItem(obj);
                    if(gameObject.Components != null){
                        foreach (KeyValuePair<string, CatComponent> keyValue in gameObject.Components) {
                            if (keyValue.Value.GetType().IsSubclassOf(typeof(Light))) {
                                candidates.Add(keyValue.Value as Light);
                            }
                        }
                    }
                }
                m_diffuseLights = candidates.ToArray();
                m_diffuseLightGameObject = value;
            }
            get {
                return m_diffuseLightGameObject;
            }
        }
        private Light[] m_diffuseLights;
    

        private float m_currentTime = 0.0f;
        private Color[] m_diffuseLightArray;

#endregion

        public SkyController(GameObject _gameObject)
            : base(_gameObject) { }

        public SkyController() : base() { }


        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            ApplyAmbientColorMap();
        }

        public void ApplyAmbientColorMap() {
            Texture2D tex = null;
            try {
                tex = Mgr<CatProject>.Singleton.contentManger.Load<Texture2D>("image\\" + m_ambientLightMapName);
            }
            catch (ContentLoadException) {
            }
            if (Mgr<Scene>.Singleton != null && Mgr<Scene>.Singleton.m_shadowSystem != null) {
                Mgr<Scene>.Singleton.m_shadowSystem.SetAmbientColorMap(tex);
            }
        }

        public void ApplyDiffuseColorMap() {
            Texture2D tex = null;
            try {
                tex = Mgr<CatProject>.Singleton.contentManger.Load<Texture2D>("image\\" + m_diffuseLightMapName);
                m_diffuseLightArray = new Color[tex.Width];
                tex.GetData<Color>(0, new Rectangle(0, 0, tex.Width, 1), m_diffuseLightArray, 0, tex.Width);
            }
            catch (ContentLoadException) {
                m_diffuseLightArray = null;
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            m_currentTime += (float)timeLastFrame / m_dayDurationInMS;
            m_currentTime -= (int)m_currentTime;

            if (m_ambientLightMapName != "") {
                if (Mgr<Scene>.Singleton != null && Mgr<Scene>.Singleton.m_shadowSystem != null) {
                    Mgr<Scene>.Singleton.m_shadowSystem.Time = m_currentTime;
                }
            }

            ModelComponent modelComponent = m_gameObject.GetComponent(typeof(ModelComponent)) as ModelComponent;
            if (modelComponent != null && modelComponent.GetCatModelInstance() != null) {
                if (modelComponent.GetCatModelInstance().GetMaterial() != null
                    && modelComponent.GetCatModelInstance().GetMaterial().HasParameter("Time")) {
                        modelComponent.GetCatModelInstance().GetMaterial().SetParameter("Time", new CatFloat(m_currentTime));
                }
            }

            if (m_diffuseLights != null && m_diffuseLightArray != null) {
                Color diffuseColor = m_diffuseLightArray[(int)(m_diffuseLightArray.Length * m_currentTime) % m_diffuseLightArray.Length];
                foreach (Light light in m_diffuseLights) {
                    if (light != null) {
                        light.DiffuseColor = diffuseColor;
                    }
                }
            }
        }

        public static string GetMenuNames() {
            return "Test|SkyController";
        }

    }
}
