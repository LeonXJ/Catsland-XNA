using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;

namespace Catsland.Plugin.BasicPlugin
{
	public class Breakable : CatComponent
	{
		private int m_hp=6;
        [CategoryAttribute("Basic")]
        public int HP
        {
            get { return m_hp; }
            set { m_hp = value; }
        }

        private bool removeAfterDie;
        public bool RemoveAfterDie {
            get { return removeAfterDie; }
            set { removeAfterDie = value; }
        }

		public Breakable(GameObject gameObject)
			: base(gameObject)
		{
		}

		public void GetHurt(GameObject attacker, GameObject attackObject, AttackInvoke.AttackType attackType)
		{
            CharacterController characterController =
                (CharacterController)m_gameObject.GetComponent(typeof(CharacterController).Name);
            bool getHurted = true;
            if (characterController != null) {
                if (attackType == AttackInvoke.AttackType.Normal
                    && characterController.IsDefending()) {
                    getHurted = false;
                    characterController.Block(attackObject);
                }
                else {
                    characterController.GetHurt(200, attackObject);
                }
                
            }
            if (!getHurted) {
                return;
            }
			m_hp -= 1;
            /*
            Animator animator = (Animator)m_gameObject.GetComponent(typeof(Animator).Name);
			// animation
			if (m_hp > 3)
			{
                if (animator != null)
				{
                    animator.PlayAnimation("AllBreak");
				}
			}

			// destroy
			else if (m_hp <= 3 && m_hp > 0)
			{
                if (animator != null)
				{
                    animator.PlayAnimation("HalfBreak");
				}
				
			}
            */
			if (m_hp <= 0)
			{
                if (characterController != null) {
                    characterController.Die();
                }
                if (removeAfterDie) {
                    Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(m_gameObject.GUID);
                }
			}
             
		}

		public override bool SaveToNode(XmlNode node, XmlDocument doc)
		{
			XmlElement breakable = doc.CreateElement("Breakable");
			node.AppendChild(breakable);

			return true;
		}

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);

            return;
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            Breakable newBreakable = new Breakable(gameObject);
            newBreakable.m_hp = m_hp;
            newBreakable.removeAfterDie = removeAfterDie;
            return newBreakable;
        }
	}
}
