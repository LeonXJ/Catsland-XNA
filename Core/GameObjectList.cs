using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

/**
 * @file GameObjectList
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief every scene holds a GameObjectList, storing the list of gameObject
     * 
     * adding or removing item should be performed asynchronously while gaming is running
     * */
    public class GameObjectList : UniqueList<GameObject> {

        #region Properties

        private List<string> m_removeList;
        private List<GameObject> m_addList;
        private Dictionary<string, List<string>> nameDictionary = new Dictionary<string, List<string>>();

        #endregion

        /**
         * @brief add GameObject to list. The GameObject will be added in
         *      UpdateAdd phase
         * */
        public void AddGameObject(GameObject _gameObject) {
            if (m_addList == null) {
                m_addList = new List<GameObject>();
            }
            m_addList.Add(_gameObject);
        }

        public override void AddItem(string guid, GameObject item) {
            Debug.Assert(false, "Don't use this function. Use AddGameObject instead.");
            return;
        }

        /**
         * @brief remove GameObject from list. the GameObject will be removed
         *      at UpdateRemove phase. Removing a GameObject may result in the
         *      removal of the whole GameObject sub-tree.
         *      Don't call this function to parent and child at the same time.
         * */
        public void RemoveGameObject(string _guid) {
            if (m_removeList == null) {
                m_removeList = new List<string>();
            }
            m_removeList.Add(_guid);
        }

        /**
         * @brief remove a gameObject from list
         * 
         * the gameObject will be removed in next removing phase
         * */
        public override void RemoveItem(string guid) {
            Debug.Assert(false, "Don't use this function. Use RemoveGameObject instead.");
            return;
        }

        /**
         * @brief declare the renaming of gameObject
         * */
        public void Rename(string oldName, GameObject gameObject) {
            if (oldName == gameObject.Name) {
                return;
            }
            // remove
            if (nameDictionary.ContainsKey(oldName)) {
                List<string> nameList = nameDictionary[oldName];
                nameList.Remove(gameObject.GUID);
                if (nameList.Count == 0) {
                    nameDictionary.Remove(oldName);
                }
            }
            // add
            AddToNameList(gameObject);
            // update editor
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.GameObjectNameChanged();
            }
        }

        /**
         * @brief return gameobject guid list by name
         * 
         * @param name name of the gameobject(s)
         * 
         * @result the guids of the gameobjects with name
         * */
        public List<string> GetGameObjectsGuidByName(string name) {
            if (nameDictionary.ContainsKey(name)) {
                return nameDictionary[name];
            }
            return null;
        }

        public GameObject GetOneGameObjectByName(string name) {
            List<string> guids = GetGameObjectsGuidByName(name);
            if (guids != null && guids.Count > 0) {
                return GetItem(guids[0]);
            }
            return null;
        }

        /**
         * @brief [Called by GameEngine only] add gameObjects in addList to list
         * 
         * invoked by gameEngine in adding phase
         * */
        public void UpdateAdd(Scene _scene) {
            if (m_addList != null) {
                List<GameObject> addedRootGameObject = new List<GameObject>();
                // Add to contentlist
                foreach (GameObject gameObject in m_addList) {
                    string guid = gameObject.GUID;
                    // do not add to list in two cases:
                    // 1. it has been in list
                    // 2. it has parent && parent isn't in the list by far (it will be added by its parent later on)
                    if ((ContainKey(guid)) ||
                        (gameObject.Parent != null && !ContainKey(gameObject.Parent.GUID))) {
                        continue;
                    }
                    gameObject.AddGameObjectTreeToGameObjectList(this); ;
                    addedRootGameObject.Add(gameObject);
                }
                // Bind to scene
                foreach (GameObject gameObject in addedRootGameObject) {
                    gameObject.BindToScene(_scene);
                }
                // Initialize
                foreach (GameObject gameObject in addedRootGameObject) {
                    gameObject.Initialize();
                }
                // TODO: Start
                if (addedRootGameObject.Count > 0 && Mgr<GameEngine>.Singleton._gameEngineMode
            == GameEngine.GameEngineMode.MapEditor) {
                    Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
                }
                m_addList.Clear();
            }
        }

        /**
         * @brief [Called by GameEngine only] remove the gameObjects in removeList
         * 
         * invoked by gameEngine in removing phase
         * */
        public void UpdateRemove(Scene _scene) {
            if (m_removeList != null) {
                bool doSomething = false;
                // UpdateAdd is called before UpdateRemove, so all the gameObjects
                //  are in contentlist
                //  put child and parent in removelist at the same time is not allowed
                foreach (string guid in m_removeList) {
                    if (ContainKey(guid)) {
                        contentList[guid].UnbindFromScene();
                        contentList[guid].RemoveGameObjectTreeFromGameObjectList(this);
                        doSomething = true;
                    }
                }
                if (doSomething && Mgr<GameEngine>.Singleton._gameEngineMode
                            == GameEngine.GameEngineMode.MapEditor) {
                    Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
                }
                m_removeList.Clear();
            }
        }
        
        /**
         * @brief [Called by Scene only] release all gameObjects in the list
         * 
         * this should not be invoked while the scene is running
         * */
        public override void ReleaseAll() {
            if (contentList.Count > 0) {
                foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                    if (keyValue.Value.Parent == null) {
                        keyValue.Value.UnbindFromScene();
                    }
                }
            }
            base.ReleaseAll();
            if (Mgr<GameEngine>.Singleton._gameEngineMode
                == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
            }
        }

        /**
         * @brief [Called by GameObject only] Add a single gameObject (its 
         *      children are excluded) to list
         * 
         * @param gameObject the gameObject to be added
         * */
        public void AddSingleGameObject(GameObject _gameObject) {
            string guid = _gameObject.GUID;
            if (!ContainKey(guid)) {
                base.AddItem(guid, _gameObject);
            }
            AddToNameList(_gameObject);
        }

        /**
         * @brief [Called by GameObject only] Remove gameObject (its children are excluded)
         * 
         * @param gameObject the gameObject to be removed
         * */
        public void RemoveSingleGameObject(GameObject gameObject) {
            base.RemoveItem(gameObject.GUID);
            RemoveFromNameList(gameObject);
        }

        /**
         * @brief update all gameObjects in game mode
         * 
         * invoked by gameEngine
         * */
        public void Update(int timeLastFrame) {
            if (contentList == null) {
                return;
            }
            foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                if (keyValue.Value.Parent == null) {
                    keyValue.Value.Update(timeLastFrame, this);
                }
            }
        }

        /**
         * @brief update all gameObjects in editor mode
         * 
         * invoked by gameEngine
         * */
        public void EditorUpdate(int timeLastFrame) {
            if (contentList == null) {
                return;
            }
            foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                keyValue.Value.EditorUpdate(timeLastFrame, this);
            }
        }

        public bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement gameobjects = doc.CreateElement("GameObjects");
            node.AppendChild(gameobjects);

            if (contentList != null) {
                foreach (KeyValuePair<string, GameObject> keyValue in contentList) {
                    XmlNode nodeGameObject = keyValue.Value.DoSerial(doc);
                    gameobjects.AppendChild(nodeGameObject);
                }
            }
            return true;
        }

        public static GameObjectList LoadFromNode(XmlNode node, Scene scene) {
            GameObjectList gameObjectList = new GameObjectList();
            Serialable.BeginSupportingDelayBinding();
            foreach (XmlNode gameObject in node.ChildNodes) {
                GameObject newGameObject = (GameObject)(Serialable.DoUnserial(gameObject));
                gameObjectList.AddGameObject(newGameObject);
            }
            Serialable.EndSupportingDelayBinding();
            return gameObjectList;
        }

        /**
         * @brief add gameObject guid to namelist
         * 
         * @param gameObject the gameObject
         * */
        private void AddToNameList(GameObject gameObject) {
            if (!nameDictionary.ContainsKey(gameObject.Name)) {
                nameDictionary.Add(gameObject.Name, new List<string>());
            }
            nameDictionary[gameObject.Name].Add(gameObject.GUID);
        }

        /**
         * @brief remove gameObject guid from namelist
         * 
         * @param gameObject 
         * */
        private void RemoveFromNameList(GameObject gameObject) {
            if (nameDictionary.ContainsKey(gameObject.Name)) {
                List<string> nameList = nameDictionary[gameObject.Name];
                nameList.Remove(gameObject.GUID);
                if (nameList.Count == 0) {
                    nameDictionary.Remove(gameObject.Name);
                }
            }
        }    
    }
}