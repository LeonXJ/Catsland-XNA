﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public class ShadowSystem : Renderer {

#region Properties

        private Dictionary<int, Light> m_lightDict = new Dictionary<int, Light>();
        private Dictionary<int, ShadingBody> m_shadingBodyDict = new Dictionary<int,ShadingBody>();
        private Dictionary<int, LightShadow> m_lightShadowEdgeDict = new Dictionary<int,LightShadow>();

        private Dictionary<int, HashSet<int>> m_shadingBody2LightDict = new Dictionary<int, HashSet<int>>();
        private Dictionary<int, HashSet<int>> m_light2ShadingBodyDict = new Dictionary<int, HashSet<int>>();

        private Stack<int> m_freeLightIDList;
        private Stack<int> m_freeShadowBodyIDList;

        private static int MaxLightNumber = 64;
        private static int MaxShadowBodyNumber = 128;
        private static int MaxEdgeNumber = 128;

        private Effect m_shadowingEffect;
        private Effect m_accumulateEffect;

        // TODO: save this to file
        private CatColor m_ambientColor = new CatColor(0.5f, 0.5f, 0.5f, 1.0f);
        public Color AmbientColor {
            set {
                m_ambientColor.SetValue(value);
            }
            get {
                return m_ambientColor;
            }
        }

        private RenderTarget2D m_accumulateLight;
        private RenderTarget2D m_lightMap;


#endregion

        public ShadowSystem() {
            Initialize();
        }

        public void Initialize() {
            InitFreeList();
            UpdateBuffer();
            UpdateShadingEffect();
        }

        private void InitFreeList() {
            m_freeLightIDList = new Stack<int>();
            for (int i = 0; i < MaxLightNumber; ++i) {
                m_freeLightIDList.Push(i);
            }
            m_freeShadowBodyIDList = new Stack<int>();
            for (int i = 0; i < MaxShadowBodyNumber; ++i) {
                m_freeShadowBodyIDList.Push(i);
            }
        }

        public void UpdateBuffer() {
            m_accumulateLight = TestAndCreateColorBuffer(m_accumulateLight);
            m_lightMap = TestAndCreateColorBuffer(m_lightMap);
        }

        private void UpdateShadingEffect() {
            m_shadowingEffect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\Shadowing");
            m_accumulateEffect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>
                ("effect\\AccumulateLight");
        }

        // TODO: combine with that in PostProcess
        private RenderTarget2D TestAndCreateColorBuffer(
            RenderTarget2D _oldRenderTarget,
            float _widthRatio = 1.0f, float _heightRatio = 1.0f) {

            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            if (_oldRenderTarget != null) {
                //_oldRenderTarget.Dispose();
            }
            return new RenderTarget2D(
                graphicsDevice,
                (int)(graphicsDevice.PresentationParameters.BackBufferWidth * _widthRatio),
                (int)(graphicsDevice.PresentationParameters.BackBufferHeight * _heightRatio));
        }

        public void AddLight(Light _light) {
            // assign an id
            _light.ID = FetchNewID(m_freeLightIDList);
            m_lightDict.Add(_light.ID, _light);
            // TODO: considering
        }

        public void AddShadowBody(ShadingBody _shadowBody) {
            // assign an id
            _shadowBody.ID = FetchNewID(m_freeShadowBodyIDList);
            m_shadingBodyDict.Add(_shadowBody.ID, _shadowBody);
            // TODO: considering
        }

        // TODO: remove light
        // TODO: remove shadow body

       

        private int FetchNewID(Stack<int> _freeList) {
            if (_freeList.Count == 0) {
                // error
                Console.Out.WriteLine("Error! Not free id.");
                return -1;
            }
            return _freeList.Pop();
        }

        public void UpdateShadowBody(ShadingBody _shadowBody) {
            HashSet<int> oldLight;
            if (m_shadingBody2LightDict.ContainsKey(_shadowBody.ID)) {
                oldLight = m_shadingBody2LightDict[_shadowBody.ID];
            }
            else {
                oldLight = new HashSet<int>();
            }

            foreach (KeyValuePair<int, Light> keyValue in m_lightDict) {
                // if body in light
                if (keyValue.Value.IsBodyInLight(_shadowBody)) {
                    // update shadow
                    int edgeNumber = _shadowBody.GetVerticesNumber();
                    for (int e = 0; e < edgeNumber; ++e) {
                        int lightShadowBodyEdgeID = GetLightShadowBodyEdgeID(keyValue.Key, _shadowBody.ID, e);
                        // if edge under light
                        if (keyValue.Value.ShouldEdgeHasShadow(_shadowBody, e)) {
                            m_lightShadowEdgeDict[lightShadowBodyEdgeID] =
                                LightShadow.UpdateShadow(m_lightShadowEdgeDict[lightShadowBodyEdgeID],
                                keyValue.Value, _shadowBody, e);
                            m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = true;
                        }
                        // else
                        else {
                            // TODO: not only disable
                            m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = false;
                        }
                    }
                    // update light 2 shadow and shadow 2 light
                    if (!m_light2ShadingBodyDict.ContainsKey(keyValue.Key)) {
                        m_light2ShadingBodyDict[keyValue.Key] = new HashSet<int>();
                    }
                    m_light2ShadingBodyDict[keyValue.Key].Add(_shadowBody.ID);
                    if (!m_shadingBody2LightDict.ContainsKey(_shadowBody.ID)) {
                        m_shadingBody2LightDict[_shadowBody.ID] = new HashSet<int>();
                    }
                    m_shadingBody2LightDict[_shadowBody.ID].Add(keyValue.Key);
                }
                // else
                else {
                    // if light in old-light
                    if (oldLight.Contains(keyValue.Key)) {
                        // remove shadow
                        int edgeNumber = _shadowBody.GetVerticesNumber();
                        for (int e = 0; e < edgeNumber; ++e) {
                            int lightShadowBodyEdgeID = GetLightShadowBodyEdgeID(keyValue.Key, _shadowBody.ID, e);
                            if (m_lightShadowEdgeDict.ContainsKey(lightShadowBodyEdgeID)) {
                                // TODO: not only disable
                                m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = false;
                            }
                        }
                        // update light2 2 shadow and shadow 2 light
                        oldLight.Remove(keyValue.Key);
                        if (m_light2ShadingBodyDict.ContainsKey(keyValue.Key)) {
                            m_light2ShadingBodyDict[keyValue.Key].Remove(_shadowBody.ID);
                        }
                    }
                }       
            }
        }

        public void UpdateLight(Light _light) {
            HashSet<int> oldShadowBody;
            if (m_light2ShadingBodyDict.ContainsKey(_light.ID)) {
                oldShadowBody = m_light2ShadingBodyDict[_light.ID];
            }
            else {
                oldShadowBody = new HashSet<int>();
            }

            foreach(KeyValuePair<int, ShadingBody> KeyValuePair in m_shadingBodyDict){
                if (_light.IsBodyInLight(KeyValuePair.Value)) {
                    int edgeNumber = KeyValuePair.Value.GetVerticesNumber();
                    for (int e = 0; e < edgeNumber; ++e) {
                        int lightShadowBodyEdgeID = GetLightShadowBodyEdgeID(_light.ID, KeyValuePair.Key, e);
                        // if edge under light
                        if (_light.ShouldEdgeHasShadow(KeyValuePair.Value, e)) {
                            if (!m_lightShadowEdgeDict.ContainsKey(lightShadowBodyEdgeID)) {
                                m_lightShadowEdgeDict.Add(lightShadowBodyEdgeID, LightShadow.UpdateShadow(null,
                                _light, KeyValuePair.Value, e));
                            }
                            else{
                                m_lightShadowEdgeDict[lightShadowBodyEdgeID] =
                                    LightShadow.UpdateShadow(m_lightShadowEdgeDict[lightShadowBodyEdgeID],
                                    _light, KeyValuePair.Value, e);
                            }
                            m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = true;
                        }
                        else {
                            if (m_lightShadowEdgeDict.ContainsKey(lightShadowBodyEdgeID)) {
                                m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = false;
                            }
                        }
                    }
                    // update light 2 shadow and shadow 2 light
                    if(!m_light2ShadingBodyDict.ContainsKey(_light.ID)){
                        m_light2ShadingBodyDict[_light.ID] = new HashSet<int>();
                    }
                    m_light2ShadingBodyDict[_light.ID].Add(KeyValuePair.Key);
                    if(!m_shadingBody2LightDict.ContainsKey(KeyValuePair.Key)){
                        m_shadingBody2LightDict[KeyValuePair.Key] = new HashSet<int>();
                    }
                    m_shadingBody2LightDict[KeyValuePair.Key].Add(_light.ID);
                }
                //else
                else{
                    if(oldShadowBody.Contains(KeyValuePair.Key)){
                        // remove shadow
                        int edgeNumber = KeyValuePair.Value.GetVerticesNumber();
                        for(int e = 0; e < edgeNumber; ++e){
                            int lightShadowBodyEdgeID = GetLightShadowBodyEdgeID(_light.ID, KeyValuePair.Key, e);
                            if(m_lightShadowEdgeDict.ContainsKey(lightShadowBodyEdgeID)){
                                // TODO: not only disable
                                m_lightShadowEdgeDict[lightShadowBodyEdgeID].Enable = false;
                            }
                        }
                        // update light 2 shadow and shadow 2 light
                        oldShadowBody.Remove(KeyValuePair.Key);
                        if(m_light2ShadingBodyDict.ContainsKey(_light.ID)){
                            m_light2ShadingBodyDict[_light.ID].Remove(KeyValuePair.Key);
                        }
                    }
                }
            }
        }

        public void Update(int _timeLastFrame) {
            foreach (KeyValuePair<int, Light> KeyValuePair in m_lightDict) {
                UpdateLight(KeyValuePair.Value);
            }
        }

        public int GetLightShadowBodyEdgeID(int _lightID, int _shadowBodyID, int _edge) {
            return _lightID * (MaxShadowBodyNumber * MaxEdgeNumber) +
                _shadowBodyID * MaxEdgeNumber + _edge;
        }

        // directly render out shadow
        public override void DoRender(int _timeLastFrame) {
            // Prepare accumulate
            GraphicsDevice graphicsDevice = Mgr<GraphicsDevice>.Singleton;
            graphicsDevice.Clear(m_ambientColor);
           
            // for each light
            foreach (KeyValuePair<int, Light> keyValue in m_lightDict) {
                // TODO: test if the light affects current scene
                // init light map
                Renderer.SetColorTarget(m_lightMap);
                graphicsDevice.Clear(Color.Black);
                // draw light
                // TODO: draw light
                keyValue.Value.Draw(_timeLastFrame);
                // substract shadow
                if (m_light2ShadingBodyDict.ContainsKey(keyValue.Key)) {
                    foreach (int shadingBodyID in m_light2ShadingBodyDict[keyValue.Key]) {
                        ShadingBody shadingBody = m_shadingBodyDict[shadingBodyID];
                        for (int i = 0; i < shadingBody.GetVerticesNumber(); ++i) {
                            int lightShadingBodyEdgeID = GetLightShadowBodyEdgeID(keyValue.Key, shadingBodyID, i);
                            if (m_lightShadowEdgeDict.ContainsKey(lightShadingBodyEdgeID)) {
                                m_lightShadowEdgeDict[lightShadingBodyEdgeID].Draw(_timeLastFrame);
                            }
                        }
                    }
                }

                Renderer.CancelColorTarget();
                // add to accumulate
                m_accumulateEffect.CurrentTechnique = m_accumulateEffect.Techniques["Main"];
                m_accumulateEffect.Parameters["LightMap"].SetValue((Texture2D)m_lightMap);
                m_accumulateEffect.CurrentTechnique.Passes["P0"].Apply();
                RenderQuad();
            }
        }

        // shadow the given map
        public void ShadowObject(int _timeLastFrame, RenderTarget2D _toBeShadow) {
            Renderer.SetColorTarget(m_accumulateLight);
            DoRender(_timeLastFrame);
            Renderer.CancelColorTarget();

            m_shadowingEffect.CurrentTechnique = m_shadowingEffect.Techniques["Main"];
            m_shadowingEffect.Parameters["ColorMap"].SetValue((Texture2D)_toBeShadow);
            m_shadowingEffect.Parameters["LightMap"].SetValue((Texture2D)m_accumulateLight);
            m_shadowingEffect.CurrentTechnique.Passes["P0"].Apply();
            RenderQuad();
        }



    }
}