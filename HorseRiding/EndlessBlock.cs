using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Catsland.Core;

namespace HorseRiding {
    public class EndlessBlock : CatComponent {

        #region Properties

        [SerialAttribute]
        protected readonly CatFloat m_blockWidth = new CatFloat(1.0f);
        public float BlockWidth {
            set {
                m_blockWidth.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_blockWidth.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatFloat m_prefatchWidth = new CatFloat(0.5f);
        public float PrefetchWidth {
            set {
                m_prefatchWidth.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_prefatchWidth.GetValue();
            }
        }

        protected Dictionary<int, GameObject> m_newBlock = 
                                            new Dictionary<int, GameObject>();
        [SerialAttribute]
        protected string m_blockPrefabName;
        public string BlocPrefabName {
            set {
                m_blockPrefabName = value;
            }
            get {
                return m_blockPrefabName;
            }
        }

        // This should be moved to child
        [SerialAttribute]
        private string m_barrierPrefabName;
        public string BarrierPrefabName {
            set {
                m_barrierPrefabName = value;
            }
            get {
                return m_barrierPrefabName;
            }
        }
        // This should be moved to child
        [SerialAttribute]
        private string m_flyPrefabName;
        public string FlyPrefabName {
            set {
                m_flyPrefabName = value;
            }
            get {
                return m_flyPrefabName;
            }
        }
        [SerialAttribute]
        protected readonly CatFloat m_flyHeight = new CatFloat(0.8f);
        public float FlyHeight {
            set {
                m_flyHeight.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_flyHeight.GetValue();
            }
        }
        [SerialAttribute]
        protected readonly CatFloat m_flyHorizontalVelocityBias = new CatFloat(0.3f);
        public float FlyHorizontalVelocityBias {
            set {
                m_flyHorizontalVelocityBias.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_flyHorizontalVelocityBias.GetValue();
            }
        }
        [SerialAttribute]
        private string m_wildgoosePrefabName;
        public string WildGoosefabName {
            set {
                m_wildgoosePrefabName = value;
            }
            get {
                return m_wildgoosePrefabName;
            }
        }
        [SerialAttribute]
        protected readonly CatFloat m_wildgooseHeight = new CatFloat(0.1f);
        public float WildgooseFlyHeight {
            set {
                m_wildgooseHeight.SetValue(value);
            }
            get {
                return m_wildgooseHeight.GetValue();
            }
        }
        [SerialAttribute]
        private string m_relicName;
        public string RelicName {
            set {
                m_relicName = value;
            }
            get {
                return m_relicName;
            }
        }

        [SerialAttribute]
        private readonly CatBool m_barrierOn = new CatBool(true);
        public bool BarrierOn {
            set {
                m_barrierOn.SetValue(value);
            }
            get {
                return m_barrierOn;
            }
        }


        private Random m_rand = new Random();

        #endregion

        public EndlessBlock() : base() { }
        public EndlessBlock(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            Camera camera = Mgr<Camera>.Singleton;
            UpdateBlock(new Vector2(camera.CameraPosition.X - 
                                 (camera.ViewSize.X + m_prefatchWidth) / 2.0f,
                                    camera.CameraPosition.X + 
                                 (camera.ViewSize.X + m_prefatchWidth) / 2.0f));
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
        }

        private void UpdateBlock(Vector2 _cameraRange) {
            float cameraRelativeLeftX = _cameraRange.X - m_gameObject.AbsPosition.X;
            float cameraRelativeRightX = _cameraRange.Y - m_gameObject.AbsPosition.X;
            int leftIndex = RelativePositionToBlockIndex(cameraRelativeLeftX);
            int rightIndex = RelativePositionToBlockIndex(cameraRelativeRightX);
            // remove
            List<int> removeList = new List<int>();
            foreach (KeyValuePair<int, GameObject> keyValue in m_newBlock) {
                if (keyValue.Key < leftIndex || keyValue.Key > rightIndex) {
                    DeleteBlock(keyValue.Value);
                    removeList.Add(keyValue.Key);
                }
            }
            foreach (int index in removeList) {  
                m_newBlock.Remove(index);
            }
            // insert
            for (int i = leftIndex; i <= rightIndex; ++i) {
                if (!m_newBlock.ContainsKey(i)) {
                    GameObject newBlock = CreateBlock(i);
                    m_newBlock.Add(i, newBlock);
                }
            }
        }

        protected int RelativePositionToBlockIndex(float _postion) {
            // the int right smaller than _position
            return (int)Math.Floor(_postion / m_blockWidth);
        }

        private GameObject CreateBlock(int _index) {
            GameObject prefab = Mgr<CatProject>.Singleton.prefabList.GetItem(m_blockPrefabName);
            Serialable.BeginSupportingDelayBinding();
            GameObject newGameObject = prefab.DoClone() as GameObject;
            Serialable.EndSupportingDelayBinding();
            newGameObject.Position = new Vector3(
                        _index * m_blockWidth,
                        newGameObject.Position.Y,
                        newGameObject.Position.Z);
            newGameObject.AttachToGameObject(m_gameObject);
            // do post create block
            PostCreatGameObject(newGameObject, _index);
            Mgr<Scene>.Singleton._gameObjectList.AddGameObject(newGameObject);
            return newGameObject;
        }

        private void DeleteBlock(GameObject _gameObject) {
            Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(
                _gameObject.GUID);
        }

        virtual protected void PostCreatGameObject(GameObject _gameObject,
                                                   int _index) {
            // do something special to the _gameObject according to _index
            float rand = (float)m_rand.NextDouble();
            if (m_barrierOn.GetValue() && 
                rand > 0.6f && m_barrierPrefabName != null && m_barrierPrefabName != "") {
                // generate barrier
                if (rand > 0.96f) {
                    CreateStack(_gameObject, _index, 4);
                }
                else if (rand > 0.88f) {
                    CreateStack(_gameObject, _index, 3);
                }
                else if (rand > 0.76) {
                    CreateStack(_gameObject, _index, 2);
                }
                else {
                    CreateStack(_gameObject, _index, 1);
                }
            }
            rand = (float)m_rand.NextDouble();
            if (rand > 0.5f && m_flyPrefabName != null && m_flyPrefabName != "") {
                CreateFly(_gameObject, _index);
            }
            rand = (float)m_rand.NextDouble();
            if (rand > 0.8f && m_wildgoosePrefabName != null && m_wildgoosePrefabName != "") {
                CreateWildGoose(_gameObject, _index, 1 + (int)(rand * 3));
            }
            rand = (float)m_rand.NextDouble();
            if (rand > 0.8f && m_relicName != null && m_relicName != "") {
                CreateRelic(_gameObject, _index);
            }
        }

        // move to child
        private void CreateStack(GameObject _blockGameObject, int _index, int _stackNum = 2) {
            GameObject prefab =
                    Mgr<CatProject>.Singleton.prefabList.GetItem(m_barrierPrefabName);
            _blockGameObject.ForceUpdateAbsTransformation();
            Random ran = new Random(_index);
            float randx = (float)ran.NextDouble() * m_blockWidth;
            Serialable.BeginSupportingDelayBinding();
            for (int i = 0; i < _stackNum; ++i) {
                GameObject barrier = prefab.DoClone() as GameObject;
                barrier.Position = _blockGameObject.AbsPosition +
                                    new Vector3(randx, 0.4f + 0.1f * i, 0.0f);
                
                Mgr<Scene>.Singleton._gameObjectList.AddGameObject( barrier);
            }
            Serialable.EndSupportingDelayBinding();
        }
        // move to child
        private void CreateFly(GameObject _blockGameObject, int _index) {
            GameObject prefab =
                    Mgr<CatProject>.Singleton.prefabList.GetItem(m_flyPrefabName);
            _blockGameObject.ForceUpdateAbsTransformation();
            //float randy = (float)m_rand.NextDouble() * m_blockWidth;
            Serialable.BeginSupportingDelayBinding();
            GameObject fly = prefab.DoClone() as GameObject;
            fly.Position = _blockGameObject.AbsPosition +
                                    new Vector3(0.0f, m_flyHeight, 0.0f);

            Mgr<Scene>.Singleton._gameObjectList.AddGameObject(fly);
            ButterflyController bfc =
                fly.GetComponent(typeof(ButterflyController).ToString())
                as ButterflyController;
            if (bfc != null) {
                bfc.HorizontalVelocity += (float)(m_rand.NextDouble() * 2.0f - 1.0f)
                    * m_flyHorizontalVelocityBias;
            }
            Serialable.EndSupportingDelayBinding();
        }

        private void CreateWildGoose(GameObject _blockGameObject, int _index, int _num) {
            GameObject prefab =
                    Mgr<CatProject>.Singleton.prefabList.GetItem(m_wildgoosePrefabName);
            _blockGameObject.ForceUpdateAbsTransformation();
            Random ran = new Random(_index);
            float randx = (float)ran.NextDouble() * m_blockWidth;
            Serialable.BeginSupportingDelayBinding();
            for (int i = 0; i < _num; ++i) {
                GameObject wildgoose = prefab.DoClone() as GameObject;
                wildgoose.Position = new Vector3(randx + 0.05f * i, 
                           m_wildgooseHeight + (float)ran.NextDouble() * 0.02f, 
                           0.0f);
                wildgoose.AttachToGameObject(_blockGameObject);
                Mgr<Scene>.Singleton._gameObjectList.AddGameObject(wildgoose);
            }
            Serialable.EndSupportingDelayBinding();
        }

        private void CreateRelic(GameObject _blockGameObject, int _index) {
            GameObject prefab =
                    Mgr<CatProject>.Singleton.prefabList.GetItem(m_relicName);
            _blockGameObject.ForceUpdateAbsTransformation();
            Serialable.BeginSupportingDelayBinding();
            GameObject relic = prefab.DoClone() as GameObject;
            relic.Position = new Vector3((float)m_rand.NextDouble() * m_blockWidth,
                                            relic.Position.Y,
                                            relic.Position.Z);
            relic.AttachToGameObject(_blockGameObject);
            Mgr<Scene>.Singleton._gameObjectList.AddGameObject(_blockGameObject);
            Serialable.EndSupportingDelayBinding();
        }
        
    }
}
