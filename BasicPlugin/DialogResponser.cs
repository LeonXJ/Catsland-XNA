using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin
{
	public class DialogResponser : CatComponent
	{
		public DialogResponser(GameObject gameObject)
			: base(gameObject)
		{

		}

		public virtual void Response(GameObject who)
		{
            DialogAnimator dialogAnimator = (DialogAnimator)m_gameObject.GetComponent(typeof(DialogAnimator).Name);

			if (dialogAnimator != null)
			{
                dialogAnimator.CheckAndPlayDialog(0);

			}
		}

		public override bool SaveToNode(XmlNode node, XmlDocument doc)
		{
			XmlElement dialogResponser = doc.CreateElement("DialogResponser");
			node.AppendChild(dialogResponser);

			return true;
		}

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);
            return;
        }
	}
}
