using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using Catsland.Plugin.BasicPlugin;

namespace Catsland.Plugin.TestPlugin {
    public class GuardBTPack : CatComponent {

#region Properties
        // Guard
        [SerialAttribute]
        private string m_guardPointGameObjectName = "";
        public string GuardPointGameObjectName {
            set {
                m_guardPointGameObjectName = value;
            }
            get {
                return m_guardPointGameObjectName;
            }
        }
        [SerialAttribute]
        private CatFloat m_arriveDistance = new CatFloat(0.2f);
        public float ArriveDistance {
            set {
                m_arriveDistance.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_arriveDistance;
            }
        }
        // check
        [SerialAttribute]
        private CatInteger m_suspectThreshold = new CatInteger(200);
        public int SuspectThreshold {
            set {
                m_suspectThreshold.SetValue((int)(MathHelper.Max(value, 1)));
            }
            get {
                return m_suspectThreshold;
            }
        }
#endregion

        public GuardBTPack(GameObject _gameObject)
            : base(_gameObject) { }

        public GuardBTPack() : base() { }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            BTTreeComponent btTreeComponent = m_gameObject.GetComponent(typeof(BTTreeComponent)) as BTTreeComponent;
            if (btTreeComponent != null) {
                btTreeComponent.RuntimePack.AddToBlackboard("Mode", "guard");
            }
        }

        public static string GetMenuNames() {
            return "Controller|GuardBTPack";
        }

    }

    public class BTConditionInMode : BTConditionNode {
        
#region Properties

        [SerialAttribute]
        private string m_mode = "";
        public string Mode {
            set {
                m_mode = value;
            }
            get {
                return m_mode;
            }
        }

#endregion

        protected override bool JudgeCondition(BTTreeRuntimePack _btTree) {
            string mode = _btTree.GetFromBlackboard("Mode", "") as string;
            return (mode == m_mode);
        }

        public override string GetDisplayName() {
            return "In Mode?";
        }
    }

    public class BTConditionSpotAny : BTConditionNode {
        /**
         * @brief BTConditionNode, judge if the hunter spot anything.
         **/
        protected override bool JudgeCondition(BTTreeRuntimePack _runTimePack) {
            Hunter hunter = _runTimePack.GameObject.GetComponent(typeof(Hunter))
                as Hunter;
            if (hunter != null) {
                return hunter.SpotAny();
            }
            return false;
        }

        public override string GetDisplayName() {
            return "Spot?";
        }
    }

    public class BTConditionNotArrivePoint : BTConditionNode {
        /**
         * @brief BTConditionNode, judge if gameObject hasn't arrived at the given point yet.
         *      Need to work with GuardBTPack.
         **/

        [SerialAttribute]
        private CatBool m_isGuardMode = new CatBool(true);
        public bool IsGuardMode {
            set {
                m_isGuardMode.SetValue(value);
            }
            get {
                return m_isGuardMode;
            }
        }

        [SerialAttribute]
        private string m_hotspotName = "Hotspot";
        public string HotspotName {
            set {
                m_hotspotName = value;
            }
            get {
                return m_hotspotName;
            }
        }

        [SerialAttribute]
        private string m_outputDestinationNmae = "Destination";
        public string OutputDestinationName {
            set {
                m_outputDestinationNmae = value;
            }
            get {
                return m_outputDestinationNmae;
            }
        }

        [SerialAttribute]
        private string m_outputDeltaName = "Delta";
        public string OutputDeltaName {
            set {
                m_outputDeltaName = value;
            }
            get {
                return m_outputDeltaName;
            }
        }

        protected override bool JudgeCondition(BTTreeRuntimePack _runTimePack) {
            Vector3 currentPosition = _runTimePack.GameObject.AbsPosition;
            Vector3 distinationPosition = currentPosition;
            GuardBTPack btpack = _runTimePack.GameObject.GetComponent(typeof(GuardBTPack)) as GuardBTPack;
            if (btpack == null) {
                return false;
            }
            if (m_isGuardMode) {
                    GameObject distinationGameObject = btpack.GameObject.Scene._gameObjectList
                        .GetOneGameObjectByName(btpack.GuardPointGameObjectName);
                    if (distinationPosition != null) {
                        distinationPosition = distinationGameObject.AbsPosition;
                    }
            }
            else {
                Vector3 hotspot = _runTimePack.GetFromBlackboard(m_hotspotName, "") as CatVector3;
                if (hotspot != null) {
                    distinationPosition = hotspot;
                }
            }

            Vector3 delta = distinationPosition - currentPosition;
            _runTimePack.AddToBlackboard(m_outputDestinationNmae, new CatVector3(distinationPosition));
            _runTimePack.AddToBlackboard(m_outputDeltaName, new CatVector3(delta));
            return delta.X * delta.X > btpack.ArriveDistance * btpack.ArriveDistance;
        }

        public override string GetDisplayName() {
            return "Not Arrive?";
        }
    }

    public class BTActionMoveToPoint : BTActionNode {

#region Properties

        [SerialAttribute]
        private string m_deltaName = "Delta";
        public string DeltaName {
            set {
                m_deltaName = value;
            }
            get {
                return m_deltaName;
            }
        }

#endregion

        public override bool OnRunning(BTTreeRuntimePack _runtimePack) {

            CatController controller = _runtimePack.GameObject.GetComponent(typeof(CatController))
                as CatController;
            if (controller == null) {
                return false;
            }
            CatVector3 delta = _runtimePack.GetFromBlackboard(m_deltaName) as CatVector3;
            if (delta != null) {
                if (delta.X > 0.0) {
                    controller.m_wantRight = true;
                }
                else {
                    controller.m_wantLeft = true;
                }
            }
            return true;
        }

        public override string GetDisplayName() {
            return "Move To";
        }
    }

    public class BTActionIdle : BTActionNode {
        public override bool OnRunning(BTTreeRuntimePack _runtimePack) {
            CatController controller = _runtimePack.GameObject.GetComponent(typeof(CatController))
                as CatController;
            if (controller == null) {
                return false;
            }
            controller.m_wantJump = true;
            return true;
        }

        public override string GetDisplayName() {
            return "Idle";
        }
    }

    public class BTActionChangeMode : BTActionNode {

#region Properties

        [SerialAttribute]
        private string m_mode = "";
        public string Mode {
            set {
                m_mode = value;
            }
            get {
                return m_mode;
            }
        }

#endregion

        public override bool OnRunning(BTTreeRuntimePack _runtimePack) {
            _runtimePack.AddToBlackboard("Mode", m_mode);
            return false;
        }

        public override string GetDisplayName() {
            return "Change Mode";
        }
    }

    public class BTActionSetHotspot : BTActionNode {

#region Properties

        [SerialAttribute]
        private string m_hotspotName = "Hotspot";
        public string HotspotName {
            set {
                m_hotspotName = value;
            }
            get {
                return m_hotspotName;
            }
        }

#endregion
        public override bool OnRunning(BTTreeRuntimePack _runtimePack) {

            Hunter hunter = _runtimePack.GameObject.GetComponent(typeof(Hunter))
                as Hunter;
            if (hunter != null) {
                Vector2 hotspot = hunter.LastSpot.GetPointInWorld();
                _runtimePack.AddToBlackboard(m_hotspotName, new CatVector3(hotspot.X, hotspot.Y, 0.0f));
            }
            return false;
        }

        public override string GetDisplayName() {
            return "Set Hotspot";
        }

    }

    public class BTActionIncreaseSuspection : BTActionNode {
#region Properties

        [SerialAttribute]
        private string m_suspectName = "suspect";
        public string SuspectName {
            set {
                m_suspectName = value;
            }
            get {
                return m_suspectName;
            }
        }

        [SerialAttribute]
        private CatInteger m_increament = new CatInteger(1);
        public int Increament {
            set {
                m_increament.SetValue(value);
            }
            get {
                return m_increament;
            }
        }

#endregion

        public override bool OnRunning(BTTreeRuntimePack _runtimePack) {
            CatInteger suspect = _runtimePack.GetFromBlackboard(m_suspectName, new CatInteger(0))
                as CatInteger;
            suspect.SetValue(suspect + m_increament);
            _runtimePack.AddToBlackboard(m_suspectName, suspect);
            return true;
        }

        public override string GetDisplayName() {
            return "Change Suspect";
        }

    }

    public class BTConditionSuspectThreshold : BTConditionNode {

#region Properties

        [SerialAttribute]
        private string m_suspectName = "suspect";
        public string SuspectName {
            set {
                m_suspectName = value;
            }
            get {
                return m_suspectName;
            }
        }

        [SerialAttribute]
        private CatBool m_isOver = new CatBool(true);
        public bool IsOver {
            set {
                m_isOver.SetValue(value);
            }
            get {
                return m_isOver;
            }
        }

#endregion

        protected override bool JudgeCondition(BTTreeRuntimePack _runtimePack) {
            CatInteger suspect = _runtimePack.GetFromBlackboard(m_suspectName, new CatInteger(0))
                as CatInteger;
            if (m_isOver) {
                GuardBTPack btpack = _runtimePack.GameObject.GetComponent(typeof(GuardBTPack)) as GuardBTPack;
                if (btpack != null) {
                    return suspect > btpack.SuspectThreshold;
                }
                return false;
            }
            else {
                return suspect <= 0;
            }
        }

        public override string GetDisplayName() {
            return "Suspect Limit?";
        }
    }
}
