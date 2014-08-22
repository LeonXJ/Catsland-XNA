using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.TestPlugin {
    public class TestLoadBTTree : IConsoleCommand {
        private string m_btTreeName;
        public string GetCommandName() {
            return "TestLoadBTTree";
        }
        public void ParseStringParameter(string _parameters) {
            m_btTreeName = _parameters;
        }
        public object Execute() {
            BTTree btTree = Mgr<CatProject>.Singleton.BTTreeManager.LoadBTTree(m_btTreeName);
            if (btTree != null) {
                return "The btree has been loaded.";
            }
            else {
                return "Fail to load btree: " + m_btTreeName;
            }
        }
    }

    public class TestSaveBTTree : IConsoleCommand {
        private string m_btTreeName;
        public string GetCommandName() {
            return "TestSaveBTTree";
        }
        public void ParseStringParameter(string _parameters) {
            m_btTreeName = _parameters;
        }
        public object Execute() {
            BTTree btTree = new BTTree();
            btTree.Root = CreateBTTree();
            Mgr<CatProject>.Singleton.BTTreeManager.AddBTTree(m_btTreeName, btTree);

            return "BTTree has been created and inserted into BTTreeManager";
        }

        private BTNode CreateBTTree() {
            // root
            BTLinearSelectNode lSelectRoot = new BTLinearSelectNode();
            // guard
            BTConditionInMode conInGuardMode = new BTConditionInMode();
            conInGuardMode.Mode = "guard";
            BTLinearSelectNode lSelectGuard = new BTLinearSelectNode();
            BTConditionSpotAny conSpotAny = new BTConditionSpotAny();
            BTParallelSelectNode pSelectGuard2Check = new BTParallelSelectNode();
            BTActionSetHotspot actSetHotspot = new BTActionSetHotspot();
            actSetHotspot.HotspotName = "Hotspot";
            BTActionChangeMode actChangeMode = new BTActionChangeMode();
            actChangeMode.Mode = "check";
            pSelectGuard2Check.AddChild(actSetHotspot);
            pSelectGuard2Check.AddChild(actChangeMode);
            conSpotAny.Child = pSelectGuard2Check;
            BTConditionNotArrivePoint conNotArrivePoint = new BTConditionNotArrivePoint();
            conNotArrivePoint.IsGuardMode = true;
            conNotArrivePoint.OutputDeltaName = "GuardDelta";
            conNotArrivePoint.OutputDestinationName = "GuardDestination";
            BTActionMoveToPoint actMoveToPoint = new BTActionMoveToPoint();
            actMoveToPoint.DeltaName = "GuardDelta";
            conNotArrivePoint.Child = actMoveToPoint;
            BTActionIdle actIdle = new BTActionIdle();
            lSelectGuard.AddChild(conSpotAny);
            lSelectGuard.AddChild(conNotArrivePoint);
            lSelectGuard.AddChild(actIdle);
            conInGuardMode.Child = lSelectGuard;
            // check
            BTConditionInMode conInCheckMode = new BTConditionInMode();
            conInCheckMode.Mode = "check";
            BTParallelSelectNode pSelectCheck = new BTParallelSelectNode();
            BTConditionSpotAny conCheckSpotAny = new BTConditionSpotAny();
            BTParallelSelectNode pSelectSpot = new BTParallelSelectNode();
            BTActionIncreaseSuspection actIncreaseSuspect = new BTActionIncreaseSuspection();
            pSelectSpot.AddChild(actIncreaseSuspect);
            pSelectSpot.AddChild(actSetHotspot);
            conCheckSpotAny.Child = pSelectSpot;
            BTLinearSelectNode lSelectCheckMove = new BTLinearSelectNode();
            // move 
            BTConditionNotArrivePoint conCheckNotArrivePoint = new BTConditionNotArrivePoint();
            conCheckNotArrivePoint.IsGuardMode = false;
            conCheckNotArrivePoint.HotspotName = "Hotspot";
            conCheckNotArrivePoint.OutputDeltaName = "CheckDelta";
            conCheckNotArrivePoint.OutputDestinationName = "CheckDestination";
            BTActionMoveToPoint actCheckMoveToPoint = new BTActionMoveToPoint();
            actCheckMoveToPoint.DeltaName = "CheckDelta";
            conCheckNotArrivePoint.Child = actCheckMoveToPoint;
            // change back
            BTActionIncreaseSuspection actDecreaseSuspect = new BTActionIncreaseSuspection();
            actDecreaseSuspect.Increament = -1;
            lSelectCheckMove.AddChild(conCheckNotArrivePoint);
            lSelectCheckMove.AddChild(actDecreaseSuspect);
            BTConditionSuspectThreshold conOver = new BTConditionSuspectThreshold();
            conOver.IsOver = true;
            BTActionChangeMode actCheckChangeMode = new BTActionChangeMode();
            actCheckChangeMode.Mode = "check"; // TODO
            conOver.Child = actCheckChangeMode;
            BTConditionSuspectThreshold conLower = new BTConditionSuspectThreshold();
            conLower.IsOver = false;
            BTActionChangeMode actCheckChangeLowerMode = new BTActionChangeMode();
            actCheckChangeLowerMode.Mode = "guard";
            conLower.Child = actCheckChangeLowerMode;
            pSelectCheck.AddChild(conCheckSpotAny);
            pSelectCheck.AddChild(lSelectCheckMove);
            pSelectCheck.AddChild(conOver);
            pSelectCheck.AddChild(conLower);
            conInCheckMode.Child = pSelectCheck;
            // /root
            lSelectRoot.AddChild(conInGuardMode);
            lSelectRoot.AddChild(conInCheckMode);

            return lSelectRoot;
        }
    }
}
