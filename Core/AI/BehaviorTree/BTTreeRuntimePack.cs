using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Core {
    public class BTTreeRuntimePack {

#region Properties

        private GameObject m_gameObject;
        public GameObject GameObject {
            get {
                return m_gameObject;
            }
        }

        private string m_btTreeName = "";
        public string BTTreeName {
            set {
                m_btTreeName = value;
            }
        }
        // to keep constance, we don't store the reference to bttree
        // case: editor update the tree by create a new one with the same name.
        public BTTree BTTree {
            get{
                return Mgr<CatProject>.Singleton.BTTreeManager.LoadBTTree(m_btTreeName);
            }
        }
        private Dictionary<string, object> m_blackboard = new Dictionary<string, object>();
        private HashSet<BTActionNode> m_runningAction = new HashSet<BTActionNode>();
        private HashSet<BTActionNode> m_nextRunningAction = new HashSet<BTActionNode>();

#endregion

        public BTTreeRuntimePack(GameObject _gameObject, string _btTreeName) {
            m_gameObject = _gameObject;
            m_btTreeName = _btTreeName;
        }

        public bool IsActionRunning(BTActionNode _actionNode) {
            if (_actionNode == null || m_runningAction == null) {
                return false;
            }
            return m_runningAction.Contains(_actionNode);
        }

        public void DeclareRunning(BTActionNode _actionNode) {
            if (_actionNode != null && m_nextRunningAction != null) {
                m_nextRunningAction.Add(_actionNode);
            }
        }

        public void UpdateBTTree(int _timeLastFrameInMS) {
            BTTree btTree = BTTree;
            if (btTree == null || btTree.Root == null) {
                return;
            }

            btTree.Root.Execute(this);
            // on exit
            foreach (BTActionNode actionNode in m_runningAction) {
                if (!m_nextRunningAction.Contains(actionNode)) {
                    actionNode.OnExit(this);
                }
            }

            // swap
            HashSet<BTActionNode> tmp = m_nextRunningAction;
            m_nextRunningAction = m_runningAction;
            m_runningAction = tmp;
            m_nextRunningAction.Clear();
        }

        //blackboard
        public void AddToBlackboard(string _key, object _value) {
            if (!m_blackboard.ContainsKey(_key)) {
                m_blackboard.Add(_key, _value);
            }
            else {
                m_blackboard[_key] = _value;
            }
        }

        public object GetFromBlackboard(string _key, object _backup = null) {
            if (m_blackboard.ContainsKey(_key)) {
                return m_blackboard[_key];
            }
            return _backup;
        }
    }
}
