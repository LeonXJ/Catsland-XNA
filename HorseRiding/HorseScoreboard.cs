using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;

namespace HorseRiding {
    public class HorseScoreboard : CatComponent{

#region Properties

        private int m_score = 0;

        [SerialAttribute]
        private readonly CatInteger m_targetScore = new CatInteger(12);
        public int TargetScore {
            set {
                m_targetScore.SetValue((int)MathHelper.Max(0, value));
            }
            get {
                return m_targetScore;
            }
        }

        [SerialAttribute]
        private string m_playerGameObjectName = "";
        public string PlayerGameObjectName {
            set {
                m_playerGameObjectName = value;
            }
            get {
                return m_playerGameObjectName;
            }
        }

        [SerialAttribute]
        private string m_destinationPrefabName = "";
        public string DestinationPrefabName {
            set {
                m_destinationPrefabName = value;
            }
            get {
                return m_destinationPrefabName;
            }
        }

#endregion

        public HorseScoreboard() : base() { }
        public HorseScoreboard(GameObject _gameObject)
            : base(_gameObject) {

        }

        public void AddScore() {
            m_score += 1;
            if (m_score >= m_targetScore) {
                OnSuccess();
            }
        }

        protected void OnSuccess() {
            // get player's position
            GameObject player =
                Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(
                    m_playerGameObjectName);
            if (player == null) {
                return;
            }
            // create and config destination object
            GameObject prefab = 
                Mgr<CatProject>.Singleton.prefabList.GetItem(m_destinationPrefabName);
            Serialable.BeginSupportingDelayBinding();
            GameObject destinationGameObject = prefab.DoClone() as GameObject;
            Serialable.EndSupportingDelayBinding();
            // set position
            destinationGameObject.Position = new Vector3(
                player.AbsPosition.X + 3.0f,
                destinationGameObject.Position.Y,
                destinationGameObject.Position.Z);
            Mgr<Scene>.Singleton._gameObjectList.AddGameObject(
                            destinationGameObject);
        }
    }
}
