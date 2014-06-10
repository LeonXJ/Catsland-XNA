using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using Catsland.Core;

namespace Catsland.Plugin.BasicPlugin
{
	public class DialogAnimator : CatComponent
	{
		public List<Dialog> m_dialogs;
        public List<Dialog> Dialogs
        {
            get { return m_dialogs; }
            set { m_dialogs = value; }
        }

		int m_beginIndex = 0;
		int m_endIndex = 0;
		int m_currentIndex = 0;

		int m_minFlipTime = 1000;
		int m_elipseTime = 0;

		bool m_isPlaying = false;

		public DialogAnimator(GameObject gameObject)
			: base(gameObject)
		{
			m_dialogs = new List<Dialog>();
		}

		public override void Initialize(Scene scene)
		{
			base.Initialize(scene);
			m_beginIndex = 0;
			m_endIndex = 0;
			m_currentIndex = 0;

			m_isPlaying = false;
		}

		public void AddDialog(String text, Texture2D texture = null, int index = -1)
		{
			Dialog newDialog = new Dialog();
			newDialog.m_text = text;
			newDialog.m_texture = texture;
			if (index == -1)
			{
				m_dialogs.Add(newDialog);
			}
			else
			{
				m_dialogs.Insert(index, newDialog);
			}
		}

		public override void Update(int timeLastFrame)
		{
			base.Update(timeLastFrame);
			m_elipseTime += timeLastFrame;

            //CharacterController characterController = null;// (CharacterController)Mgr<Scene>.Singleton.scenePlayer
                // .GetComponent(typeof(CharacterController).Name);

			if (m_isPlaying)
			{
				KeyboardState keyboardState = Keyboard.GetState();
				if (keyboardState.IsKeyDown(Keys.Space) && m_elipseTime > m_minFlipTime)
				{
					if (m_currentIndex == m_endIndex)
					{
						// stop animation
						m_isPlaying = false;
						Mgr<DialogBox>.Singleton.m_enable = false;
						//if (Mgr<Scene>.Singleton.scenePlayer != null &&
						//	characterController != null)
						//{
						//	characterController.Enable = true;
						//}
					}
					else
					{
						++ m_currentIndex;
						PlaySingleDialog(m_currentIndex);
					}
				}
			}
		}

		public void CheckAndPlayDialog(int beginIndex, int endIndex = -1)
		{
			if (endIndex == -1)
			{
				endIndex = m_dialogs.Count() - 1;
			}
			if(m_beginIndex == beginIndex && m_endIndex == endIndex
				&& m_isPlaying)
			{
				return;
			}

			if (!m_isPlaying)
			{
				PlayDialog(beginIndex, endIndex);
			}
			else
			{
				m_endIndex = endIndex;
			}
		}

		public void PlayDialog(int beginIndex, int endIndex = -1)
		{
			if (endIndex == -1)
			{
				endIndex = m_dialogs.Count() - 1;
			}

			if (beginIndex < 0 || beginIndex > endIndex || endIndex >= m_dialogs.Count())
			{
				Console.WriteLine("Error! Play index range exceed.");
				return;
			}

			m_beginIndex = beginIndex;
			m_endIndex = endIndex;
			m_currentIndex = beginIndex;
			PlaySingleDialog(m_currentIndex);
			m_isPlaying = true;
		}

		void PlaySingleDialog(int index)
		{
			if (index >= m_dialogs.Count() || index < 0)
			{
				Console.WriteLine("Error! Dialog animator play index exceed.");
				return;
			}

			Mgr<DialogBox>.Singleton.m_text = m_dialogs[index].m_text;
			Mgr<DialogBox>.Singleton.m_enable = true;
            /*
            CharacterController characterController = (CharacterController)Mgr<Scene>.Singleton.
                scenePlayer.GetComponent(typeof(CharacterController).Name);
			if (Mgr<Scene>.Singleton.scenePlayer != null &&
				characterController != null)
			{
				characterController.Enable = false;
			}*/
			
			m_elipseTime = 0;
			// TODO: set texture;
		}

		public override bool SaveToNode(XmlNode node, XmlDocument doc)
		{
			XmlElement dialogAnimation = doc.CreateElement("DialogAnimator");
			node.AppendChild(dialogAnimation);

			XmlElement dialogs = doc.CreateElement("Dialogs");
			dialogAnimation.AppendChild(dialogs);

			foreach (Dialog dia in m_dialogs)
			{
				XmlElement dialog = doc.CreateElement("Dialog");
				dialogs.AppendChild(dialog);
				dialog.SetAttribute("text", dia.m_text);
			}

			return true;
		}

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);

            XmlElement dialogs = (XmlElement)node.SelectSingleNode("Dialogs");
            foreach (XmlNode dialog in dialogs.ChildNodes)
            {
                XmlElement dis = (XmlElement)dialog;
                String text = dis.GetAttribute("text");
                AddDialog(text);
            }
        }
	}

	public class Dialog
	{
		public String m_text {get;set;}
		public Texture2D m_texture {get;set;}
	}

}
