using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/**
 * @file AnimationPack holds a record of an Animation
 *
 * @author LeonXie
 */

namespace Catsland.Core {

    /**
     *  @brief AnimationPack holds a record of an Animation
     *  */
    public class AnimationClip : Serialable{
        // the name of the AnimationClip
        [SerialAttribute]
        public String m_name;
        public string Name {
            set {
                m_name = value;
            }
            get {
                return m_name;
            }
        }
        // the frame this animation begins
        [SerialAttribute]
        public CatInteger m_beginIndex;
        public int BeginIndex {
            set {
                m_beginIndex.SetValue(value);
            }
            get {
                return m_beginIndex.GetValue();
            }
        }
        // the frame this animation ends
        [SerialAttribute]
        public CatInteger m_endIndex;
        public int EndIndex {
            set {
                m_endIndex.SetValue(value);
            }
            get {
                return m_endIndex.GetValue();
            }
        }
        /** 
         * clamp will stop playing at last frame,
         * loop will play this animation over and over again,
         * pingpoing will play from begin -> end and then end -> begin, and gose on
         * stop will stop playing and jump to the first frame
         */
        public enum PlayMode {
            CLAMP,
            LOOP,
            PINGPONG,
            STOP
        };
        
        public PlayMode m_mode { get; set; }

        public AnimationClip() {
            m_name = "UntitledAnimationClip";
            m_beginIndex = new CatInteger(0);
            m_endIndex = new CatInteger(0);
            m_mode = PlayMode.LOOP;
        }

        public AnimationClip(String name, int begin_index = 0, int end_index = 0, PlayMode mode = PlayMode.CLAMP) {
            m_name = name;
            m_beginIndex = new CatInteger(begin_index);
            m_endIndex = new CatInteger(end_index);
            m_mode = mode;
        }

        /**
         * @brief save the animationClip to an XML node
         *
         * @param node the parent node
         * @param doc the XML doc object
         * @return success?
         */
        public bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement clip = doc.CreateElement("AnimationClip");
            node.AppendChild(clip);

            clip.SetAttribute("name", m_name);
            clip.SetAttribute("beginIndex", "" + m_beginIndex);
            clip.SetAttribute("endIndex", "" + m_endIndex);
            String mode = "";
            switch (m_mode) {
                case PlayMode.CLAMP:
                    mode = "CLAMP";
                    break;
                case PlayMode.LOOP:
                    mode = "LOOP";
                    break;
                case PlayMode.PINGPONG:
                    mode = "PINGPONG";
                    break;
                case PlayMode.STOP:
                    mode = "STOP";
                    break;
            }
            clip.SetAttribute("mode", mode);

            return true;
        }

        protected override void PostSerial(ref XmlNode _node, XmlDocument _doc) {
            // serialize enum
            XmlElement eleMode = _doc.CreateElement("Post_PlayMode");
            _node.AppendChild(eleMode);
            String mode = "";
            switch (m_mode) {
                case PlayMode.CLAMP:
                    mode = "CLAMP";
                    break;
                case PlayMode.LOOP:
                    mode = "LOOP";
                    break;
                case PlayMode.PINGPONG:
                    mode = "PINGPONG";
                    break;
                case PlayMode.STOP:
                    mode = "STOP";
                    break;
            }
            eleMode.SetAttribute("value", mode);
        }

        /**
         * @brief create an AnimationClip from an XML node
         *
         * @param node the AnimationClip node
         * @param scene not need here
         * @return AnimationCliP
         */
        public static AnimationClip LoadFromNode(XmlNode node, Scene scene) {
            XmlElement clip = (XmlElement)node;
            String name = clip.GetAttribute("name");
            int beginIndex = int.Parse(clip.GetAttribute("beginIndex"));
            int endIndex = int.Parse(clip.GetAttribute("endIndex"));
            String modeString = clip.GetAttribute("mode");
            PlayMode mode = PlayMode.STOP;
            switch (modeString) {
                case "CLAMP":
                    mode = PlayMode.CLAMP;
                    break;
                case "LOOP":
                    mode = PlayMode.LOOP;
                    break;
                case "PINGPONG":
                    mode = PlayMode.PINGPONG;
                    break;
                case "STOP":
                    mode = PlayMode.STOP;
                    break;
            }
            AnimationClip newClip = new AnimationClip(name, beginIndex, endIndex, mode);
            return newClip;
        }

        protected override void PostUnserial(XmlNode _node) {
            // parse enum
            XmlElement eleMode = _node.SelectSingleNode("Post_PlayMode")
                as XmlElement;
            string mode = eleMode.GetAttribute("value");
            switch (mode) {
                case "CLAMP":
                    m_mode = PlayMode.CLAMP;
                    break;
                case "LOOP":
                    m_mode = PlayMode.LOOP;
                    break;
                case "PINGPONG":
                    m_mode = PlayMode.PINGPONG;
                    break;
                case "STOP":
                    m_mode = PlayMode.STOP;
                    break;
            }
        }
    }
}
