using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Core {
    public class BTTreeComponent : CatComponent{

#region Properties

        private BTNode m_root = null;
        public BTNode Root {
            set {
                m_root = value;
            }
        }
        private Dictionary<string, object> m_blackboard = new Dictionary<string, object>();
        private HashSet<BTActionNode> m_runningAction = new HashSet<BTActionNode>();
        private HashSet<BTActionNode> m_nextRunningAction = new HashSet<BTActionNode>();

#endregion

        public BTTreeComponent(GameObject _gameObject)
            : base(_gameObject) { }

        public BTTreeComponent() : base() { }

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

        public override void Update(int timeLastFrame) {
            UpdateBTTree(timeLastFrame);
        }

        private void UpdateBTTree(int _timeLastFrameInMS) {
            if (m_root == null) {
                return;
            }

            m_root.Execute(this);
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

        public static string GetMenuNames(){
            return "Controller|BtTreeComponent";
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
