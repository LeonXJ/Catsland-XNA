using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin {
    public class Vulnerable : CatComponent{

#region Properties

        [SerialAttribute]
        protected CatInteger m_hp = new CatInteger(1);
        public int HP {
            set {
                m_hp.SetValue(value);
                OnGetHurt();
            }
            get {
                return m_hp;
            }
        }

#endregion

        public Vulnerable(GameObject _gameObject)
            : base(_gameObject) { }

        public Vulnerable()
            : base() { }

        public void GetHurt(int _point) {
            HP = HP - _point;
        }

        protected void OnGetHurt() {
            if (m_hp <= 0) {
                CatController controller = m_gameObject.GetComponent(typeof(CatController))
                    as CatController;
                if (controller != null) {
                    controller.CurrentState = StateStealthKillToDeath.GetState();
                }
                else {
                    m_gameObject.Scene._gameObjectList.RemoveGameObject(m_gameObject.GUID);
                }
            }
        }

        public static string GetMenuNames() {
            return "Weapon|Vulnerable";
        }
    }
}
