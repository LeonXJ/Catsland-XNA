using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

/**
 * @file PrefabList
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief the list of prefabs
     * 
     * prefab is actually gameObject. but it is seemed as the template of gameObject.
     * prefab is distinguished by name(unique), while gameObject is by guid
     * */
    public class PrefabList : UniqueList<GameObject> {
        public override void AddItem(string key, GameObject value) {
            base.AddItem(key, value);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdatePrefabList(this);
            }
        }

        public override void RemoveItem(string key) {
            base.RemoveItem(key);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdatePrefabList(this);
            }
        }

        public bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement prefabs = doc.CreateElement("Prefabs");
            node.AppendChild(prefabs);

            // only root gameobjects are in contentList. when saving root gameobjects, we enqueue their children.
            if (contentList != null) {
                Queue<GameObject> tobeSave = new Queue<GameObject>();
                foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                    tobeSave.Enqueue(keyValue.Value);
                }
                while (tobeSave.Count > 0) {
                    GameObject gameObject = tobeSave.Dequeue();
                    XmlNode nodeGameObject = gameObject.DoSerial(doc);
                    prefabs.AppendChild(nodeGameObject);
                }
            }
            return true;
        }

        public bool SavePrefabs(string _filepath) {
            if (contentList == null) {
                return true;
            }
            foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                SavePrefab(keyValue.Value, _filepath + "\\" + keyValue.Key + ".prefab");
            }
            return true;
        }

        // save a single prefab file
        private bool SavePrefab(GameObject _prefab, string _filename) {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement elePrefab = doc.CreateElement("Prefab");
            doc.AppendChild(elePrefab);
            // store root gameobject and its children in a single file
            // when saving root gameobjects, we enqueue their children.
            Queue<GameObject> tobeSave = new Queue<GameObject>();
            GetGameObjectSubTree(_prefab, ref tobeSave);
            while (tobeSave.Count > 0) {
                GameObject gameObject = tobeSave.Dequeue();
                XmlNode nodeGameObject = gameObject.DoSerial(doc);
                elePrefab.AppendChild(nodeGameObject);
            }
            doc.Save(_filename);
            return true;
        }

        // search for gameobjects need to be saved in a single file
        private void GetGameObjectSubTree(GameObject _gameObject,
                         ref Queue<GameObject> _gameObjectInTree) {
            if (_gameObject == null) {
                return;
            }
            _gameObjectInTree.Enqueue(_gameObject);

            if (_gameObject.Children != null) {
                foreach (GameObject child in _gameObject.Children) {
                    GetGameObjectSubTree(child, ref _gameObjectInTree);
                }
            }
        }

        static public PrefabList LoadFromFiles(string _prefabDictionary) {
            // create
            PrefabList prefabList = new PrefabList();

            // search for .material files under _materialDirectory
            if (!Directory.Exists(_prefabDictionary)) {
                return prefabList;
            }
            string[] files = Directory.GetFiles(_prefabDictionary, "*.prefab");
            foreach (string file in files) {
                prefabList.LoadPrefab(file);
            }

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdatePrefabList(prefabList);
            }

            return prefabList;
        }

        // load single prefab file
        private void LoadPrefab(string _filepath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filepath);
            XmlNode nodePrefab = doc.SelectSingleNode("Prefab");

            // load gameobjects and build relationships
            /*Dictionary<string, GameObject> tempList = new Dictionary<string, GameObject>();*/
            // LoadFromNode
            List<GameObject> gameObjects = new List<GameObject>();
            Serialable.BeginSupportingDelayBinding();
            foreach (XmlNode nodeGameObject in nodePrefab.ChildNodes) {
                GameObject newGameObject = GameObject.DoUnserial(nodeGameObject) as GameObject;
                gameObjects.Add(newGameObject);
                /*GameObject newGameObject = GameObject.LoadFromNode(nodeGameObject, null);*/

                /*tempList.Add(newGameObject.GUID, newGameObject);*/
            }
            Serialable.EndSupportingDelayBinding();
            // add root to prefabeList
            foreach (GameObject gameObject in gameObjects) {
                if (gameObject.Parent == null) {
                    AddItem(gameObject.Name, gameObject);
                }
            }
        }
    }
}
