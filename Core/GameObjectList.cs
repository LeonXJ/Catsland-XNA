using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/**
 * @file GameObjectList
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief every scene holds a GameObjectList, storing the list of gameObject
     * 
     * adding or removing item should be performed ascyn while gaming is running
     * */
    public class AddPack {
        public GameObject AddGameObject;
        public GameObject Parent;
        public AddPack(GameObject _addGameObject,
                       GameObject _parent) {
            AddGameObject = _addGameObject;
            Parent = _parent;
        }
    }

    public class GameObjectList : UniqueList<GameObject> {

        

        // store the gameObjects waiting to be removed
        List<GameObject> m_removeList;
        // store the gameObjects waiting to be added
        List<GameObject> m_addList;
        Dictionary<string, AddPack> m_addList2;
        // store the gameObjects waiting to be called start
        // List<GameObject> m_startList;

        public List<GameObject> AddList {
            set;
            get;
        }
        // gameObject name list for get gameObject by name
        Dictionary<string, List<string>> nameDictionary = new Dictionary<string, List<string>>();

        /**
         * @brief add gameObject to list
         * 
         * the gameObject will be added to list in next adding phase
         * */
        public void AddItem(string guid, GameObject item,
                                                  GameObject _parent = null) {
            if (m_addList == null) {
                m_addList = new List<GameObject>();
            }
            if (contentList != null && contentList.ContainsKey(item.GUID)) {
                Console.Out.WriteLine("Has contain guid: " + item.GUID);
                return;
            }
            m_addList.Add(item);

            if (m_addList2 == null) {
                m_addList2 = new Dictionary<string, AddPack>();
            }
            // no duplicate allowed
            if (contentList != null && contentList.ContainsKey(item.GUID)) {
                Console.Out.WriteLine("Has contain guid: " + item.GUID);
                return;
            }
            else if (m_addList2.ContainsKey(item.GUID)) {
                Console.Out.WriteLine("Has contain guid: " + item.GUID);
                return;
            }
            m_addList2.Add(item.GUID, new AddPack(item, _parent));
        }

        public List<GameObject> GetAddList() {
            return m_addList;
        }

        /**
         * @brief add gameObjects in addList to UniqueList
         * 
         * invoked by gameEngine in adding phase
         * */
        public void UpdateAdd() {
            if (m_addList != null && m_addList2 != null) {
                bool doSomething = false;
//                 foreach (GameObject gameObject in m_addList) {
//                     // if gameObject has parent, do not add here, add to scene by its parent
//                     // or if its parent has been added (the child is created after its parent)
//                     if ((contentList == null || !contentList.ContainsKey(gameObject.GUID))  // essential
//                         &&
//                         (gameObject.Parent == null || contentList.ContainsKey(gameObject.Parent.GUID))) {
//                         gameObject.BindToScene(Mgr<Scene>.Singleton);
//                         gameObject.Initialize(Mgr<Scene>.Singleton);
//                         doSomething = true;
//                     }
//                 }
                foreach (KeyValuePair<string, AddPack> keyValue in m_addList2) {
                    if (keyValue.Value.Parent == null) {
                        // no parent, all depend on yourself
                        keyValue.Value.AddGameObject.BindToScene(Mgr<Scene>.Singleton);
                        keyValue.Value.AddGameObject.Initialize(Mgr<Scene>.Singleton);
                        doSomething = true;
                    }
                    else {
                        if (contentList.ContainsKey(keyValue.Value.Parent.GUID)) {
                            // parent has enter, all depend on yourself
                            keyValue.Value.AddGameObject.BindToScene(Mgr<Scene>.Singleton);
                            keyValue.Value.AddGameObject.Initialize(Mgr<Scene>.Singleton);
                            doSomething = true;
                        }
                        else if (m_addList2.ContainsKey(keyValue.Value.Parent.GUID)) {
                            // wait for your parent
                            keyValue.Value.AddGameObject.AttachToGameObject(
                                keyValue.Value.Parent);
                        }
                        else {
                            // do nothing
                        }
                    }
                }

                if (doSomething && Mgr<GameEngine>.Singleton._gameEngineMode
                            == GameEngine.GameEngineMode.MapEditor) {
                    Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
                }

                m_addList.Clear();
                m_addList2.Clear();
            }
        }

        /**
         * @brief release all gameObjects in the list
         * 
         * this should not be invoked while the scene is running
         * */
        public override void ReleaseAll() {
            base.ReleaseAll();
            if (Mgr<GameEngine>.Singleton._gameEngineMode
                == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
            }
        }

        /**
         * @brief remove a gameObject from list
         * 
         * the gameObject will be removed in next removing phase
         * */
        public override void RemoveItem(string guid) {
            if (m_removeList == null) {
                m_removeList = new List<GameObject>();
            }
            if (contentList.ContainsKey(guid)) {
                m_removeList.Add(contentList[guid]);
            }
            else if(m_addList != null){
                // scan add list
                m_addList.RemoveAll(item => item.GUID == guid);
            }
        }

        /**
         * @brief remove the gameObjects in removeList
         * 
         * invoked by gameEngine in removing phase
         * */
        public void UpdateRemove() {
            if (m_removeList != null) {
                bool doSomething = false;
                foreach (GameObject gameObject in m_removeList) {
                    // gameObject will destory its children by itself
                    gameObject.Destroy(Mgr<Scene>.Singleton);
                    doSomething = true;
                }

                if (doSomething && Mgr<GameEngine>.Singleton._gameEngineMode
                            == GameEngine.GameEngineMode.MapEditor) {
                    Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
                }

                m_removeList.Clear();
            }
        }


        /**
         * @brief Remove gameObject reference
         * 
         * @param gameObject the gameObject to be removed
         * */
        public void SimplyRemoveReference(GameObject gameObject) {
            base.RemoveItem(gameObject.GUID);
            // remove from name list
            RemoveFromNameList(gameObject);
            // update editor
            if (Mgr<GameEngine>.Singleton._gameEngineMode
                            == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
            }
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

        public void Rename(string oldName, GameObject gameObject) {
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
        }

        /**
         * @brief Add gameObject reference
         * 
         * @param gameObject the gameObject to be added
         * */
        public void SimplyAddReference(GameObject gameObject) {
            base.AddItem(gameObject.GUID, gameObject);
            // nameList
            AddToNameList(gameObject);

            List<string> nameList = nameDictionary[gameObject.Name];
            nameList.Add(gameObject.GUID);
            // update editor
            if (Mgr<GameEngine>.Singleton._gameEngineMode
                            == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateGameObjectList(this);
            }
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
                    keyValue.Value.Update(timeLastFrame);
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
                keyValue.Value.EditorUpdate(timeLastFrame);
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
            //Dictionary<string, GameObject> tempList = new Dictionary<string, GameObject>();
            // enable delay binding
            Serialable.BeginSupportingDelayBinding();
            
            foreach (XmlNode gameObject in node.ChildNodes) {
                GameObject newGameObject = (GameObject)(Serialable.DoUnserial(gameObject));
                    //GameObject.LoadFromNode(gameObject, scene);
                gameObjectList.AddItem(newGameObject.GUID, newGameObject);
                //tempList.Add(newGameObject.GUID, newGameObject);
            }

            Serialable.EndSupportingDelayBinding();
            return gameObjectList;
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
    }
}