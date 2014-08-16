using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin
{
	public class AttackInvoke : CatComponent
	{
        public GameObject m_owner;

        public enum AttackType {
            Natural,
            Normal,
            Heavy
        };
        public AttackType attackType {get;set;}

        private bool isSelfDestroy;
        public bool IsSelfDestroy {
            get { return isSelfDestroy; }
            set { isSelfDestroy = value; }
        }

		public AttackInvoke(GameObject gameObject)
            :base(gameObject)
		{
		}

        public void SetOwner(GameObject owner)
        {
            m_owner = owner;
        }

		public override void EnterTrigger(Collider trigger, Collider invoker)
		{
		}

		public override void InTrigger(Collider trigger, Collider invoker)
		{
			if (invoker.m_gameObject != m_owner) // make sure not talk to yourself
			{
                Breakable breakable = (Breakable)invoker.m_gameObject.GetComponent(typeof(Breakable).Name);
                
                if (breakable != null)
				{
					breakable.GetHurt(m_owner, trigger.m_gameObject, attackType);
					// destroy itself
                    if (isSelfDestroy) {
                        Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(trigger.m_gameObject.GUID);
                    }
				}
			}
		}

		public override void ExitTrigger(Collider trigger, Collider invoker)
		{
		}

        public override CatComponent CloneComponent(GameObject gameObject){
            AttackInvoke newAttackInvoke = new AttackInvoke(gameObject);
            newAttackInvoke.SetOwner(m_owner);
            newAttackInvoke.isSelfDestroy = isSelfDestroy;
            newAttackInvoke.attackType = attackType;
            return newAttackInvoke;
        }
        
        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement attackInvoke = doc.CreateElement(typeof(AttackInvoke).Name);
            // do not set m_owner because it would be set at running time
            node.AppendChild(attackInvoke);

            attackInvoke.SetAttribute("isSelfDestroy", "" + isSelfDestroy);
            attackInvoke.SetAttribute("attackType", "" + attackType);
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            IsSelfDestroy = bool.Parse(node.GetAttribute("isSelfDestroy"));
            string strAttackType = node.GetAttribute("attackType");
            switch (strAttackType) {
                case "Natural":
                    attackType = AttackType.Natural;
                    break;
                case "Normal":
                    attackType = AttackType.Normal;
                    break;
                case "Heavy":
                    attackType = AttackType.Heavy;
                    break;
            }
        }
	}
}
