using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin
{
	class DialogTrigger : CatComponent
	{
		public GameObject m_owner;
		public DialogTrigger(GameObject gameObject)
            : base(gameObject)
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
                DialogResponser dialogResponser = (DialogResponser)invoker.m_gameObject.GetComponent(typeof(DialogResponser).Name);
                if (dialogResponser != null)
				{
                    dialogResponser.Response(m_owner);
					// destroy itself
                    Mgr<Scene>.Singleton._gameObjectList.RemoveGameObject(trigger.m_gameObject.GUID);
				}
			}
		}

		public override void ExitTrigger(Collider trigger, Collider invoker)
		{
		}

        public override CatComponent CloneComponent(GameObject gameObject) {
            DialogTrigger newDialogTrigger = new DialogTrigger(gameObject);
            newDialogTrigger.m_owner = m_owner;
            return newDialogTrigger;
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement dialogTrigger = doc.CreateElement(typeof(DialogTrigger).Name);
            node.AppendChild(dialogTrigger);
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
        }
	}
}
