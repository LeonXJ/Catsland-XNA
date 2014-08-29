using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.ComponentModel;
using Catsland.CatsEditor;
using Catsland.Editor;

namespace Catsland.Plugin {
    public class BTTreeComponent : CatComponent{

#region Properties

        private BTTreeRuntimePack m_btTreeRuntimePack;
        public BTTreeRuntimePack RuntimePack {
            get {
                return m_btTreeRuntimePack;
            }
        }
        [SerialAttribute]
        private string m_btTreeName = "";
        [EditorAttribute(typeof(PropertyGridBTTreeSelector),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string BTTreeName {
            set {
                m_btTreeName = value;
                UpdateBTTreeRuntimePack();
            }
            get {
                return m_btTreeName;
            }
        }

        public bool ObserveBTTree {
            get {
                return false;
            }
            set {
                if (Mgr<MapEditor>.Singleton != null && Mgr<MapEditor>.Singleton.BTTreeEditor != null) {
                    Mgr<MapEditor>.Singleton.BTTreeEditor.ObserveLiveBTTree(m_btTreeRuntimePack);
                    Mgr<MapEditor>.Singleton.BTTreeEditor.Show(Mgr<MapEditor>.Singleton);
                }
            }
        }

#endregion

        public BTTreeComponent(GameObject _gameObject)
            : base(_gameObject) { }

        public BTTreeComponent() : base() { }

        public override void Initialize(Catsland.Core.Scene scene) {
            base.Initialize(scene);
            UpdateBTTreeRuntimePack();   
        }

        private void UpdateBTTreeRuntimePack() {
            if (m_btTreeRuntimePack == null) {
                m_btTreeRuntimePack = new BTTreeRuntimePack(m_gameObject, m_btTreeName);
            }
            else {
                m_btTreeRuntimePack.BTTreeName = m_btTreeName;
            }

        }
        
        public override void Update(int timeLastFrame) {
            if (m_btTreeRuntimePack != null) {
                m_btTreeRuntimePack.UpdateBTTree(timeLastFrame);
                if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor
                    && Mgr<MapEditor>.Singleton != null && Mgr<MapEditor>.Singleton.BTTreeEditor != null &&
                    Mgr<MapEditor>.Singleton.BTTreeEditor.IsObservingThisRuntimePack(m_btTreeRuntimePack)) {
                        Mgr<MapEditor>.Singleton.BTTreeEditor.UpdateBTTreeEditor();
                }
            }
        }

        public static string GetMenuNames(){
            return "Controller|BtTreeComponent";
        }
    }
}
