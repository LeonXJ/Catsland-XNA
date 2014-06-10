using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

/**
 * @file Animation attach to a Model
 *
 * A Model can be attached to by an Animation. The Animation sustains 
 * a dictionary of AnimationPacks. The AnimationPacks can be get by
 * Name(string)
 *
 * @author LeonXie
 */

namespace Catsland.Core {

    /**
     *  @brief Animation holds a set of AnimationClips
     *  */
    public class Animation : Serialable{
        // HashMap: Animation Name -> AnimationClip
        [SerialAttribute]
        public Dictionary<string, AnimationClip> m_animationClips;
        public Dictionary<string, AnimationClip> AnimationClips {
            get {
                return m_animationClips;
            }
        }
        // the number of frames in width and height
        [SerialAttribute]
        public CatPoint m_tiltUV;
        public Point TiltUV {
            set {
                m_tiltUV.SetValue(value);
            }
            get {
                return m_tiltUV.GetValue();
            }
        }
        // the default AnimationClip. It would be played by default
        [SerialAttribute]
        public string m_defaultAnimationClipName;
        public string DefaultAnimationClipName {
            set {
                m_defaultAnimationClipName = value;
            }
            get {
                return m_defaultAnimationClipName;
            }
        }
        // dose the animation play automatically by default
        [SerialAttribute]
        public bool m_isAutoPlay;
        public bool IsAutoPlay {
            set {
                m_isAutoPlay = value;
            }
            get {
                return m_isAutoPlay;
            }
        }
        // how much time(in ms) it takes to flip a frame
        [SerialAttribute]
        public CatInteger m_millionSecondPerFrame;
        public int MillionSecondPerFrame {
            set {
                m_millionSecondPerFrame.SetValue(value);
            }
            get {
                return m_millionSecondPerFrame.GetValue();
            }
        }
        public CatInteger MillionSecondPerFrameRef {
            get {
                return m_millionSecondPerFrame;
            }
        }

        public Animation() {
            m_isAutoPlay = true;
            m_millionSecondPerFrame = new CatInteger(50);
            m_tiltUV = new CatPoint(1, 1);
            m_defaultAnimationClipName = "";
            m_animationClips = new Dictionary<string, AnimationClip>();
        }

        // dep
        public Animation(bool isAutoPlay = true, int millionSecondPerFrame = 50) {
            m_isAutoPlay = isAutoPlay;
            m_millionSecondPerFrame = new CatInteger(millionSecondPerFrame);
            m_tiltUV = new CatPoint(1, 1);
            m_defaultAnimationClipName = "";
            m_animationClips = new Dictionary<string, AnimationClip>();
        }

        public void addAnimationClip(AnimationClip animationClip) {
            if (m_animationClips == null) {
                m_animationClips = new Dictionary<String, AnimationClip>();
            }
            m_animationClips.Add(animationClip.m_name, animationClip);
        }

        public AnimationClip getAnimationClip(String name) {
            if (name == null || m_animationClips == null || !m_animationClips.ContainsKey(name)) {
                return null;
            }
            return m_animationClips[name];
        }

        /**
         * @brief Save the Animation to a XML node
         * 
         * @param node the parent XML node
         * @param doc the XML doc object
         * @return success?
         */
        public bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement animation = doc.CreateElement("Animation");
            node.AppendChild(animation);

            // tilt UV
            XmlElement tiltUV = doc.CreateElement("tiltUV");
            animation.AppendChild(tiltUV);
            tiltUV.SetAttribute("U", "" + m_tiltUV.X);
            tiltUV.SetAttribute("V", "" + m_tiltUV.Y);

            // default clip name
            XmlElement defaultClip = doc.CreateElement("defaultAnimationClipName");
            animation.AppendChild(defaultClip);
            defaultClip.InnerText = m_defaultAnimationClipName;

            // is auto play
            XmlElement isAutoPlay = doc.CreateElement("isAutoPlay");
            animation.AppendChild(isAutoPlay);
            isAutoPlay.InnerText = "" + m_isAutoPlay;

            // million second per frame
            XmlElement MSPF = doc.CreateElement("millionSecondPerFrame");
            animation.AppendChild(MSPF);
            MSPF.InnerText = "" + m_millionSecondPerFrame;

            // clips
            XmlElement clips = doc.CreateElement("AnimationClips");
            animation.AppendChild(clips);

            foreach (KeyValuePair<String, AnimationClip> pair in m_animationClips) {
                pair.Value.SaveToNode(clips, doc);
            }

            return true;
        }

        /**
         * @brief create a Animation object from XML node
         *
         * @param node the Animation XML node
         * @param scene the Scene the animation belongs to
         * return Animation Object
         */
        public static Animation LoadFromNode(XmlNode node, Scene scene=null) {
            XmlElement animation = (XmlElement)node;

            // tiltUV
            XmlElement tiltUV = (XmlElement)animation.SelectSingleNode("tiltUV");
            Point newTiltUV = new Point(int.Parse(tiltUV.GetAttribute("U")),
                                    int.Parse(tiltUV.GetAttribute("V")));

            // default clip name
            XmlElement defaultClipName = (XmlElement)animation.SelectSingleNode("defaultAnimationClipName");
            String newDefaultClipName = defaultClipName.InnerText;

            // is auto play
            XmlElement isAutoPlay = (XmlElement)animation.SelectSingleNode("isAutoPlay");
            bool newIsAutoPlay = bool.Parse(isAutoPlay.InnerText);

            // MSPF
            XmlElement MSPF = (XmlElement)animation.SelectSingleNode("millionSecondPerFrame");
            int newMSPF = int.Parse(MSPF.InnerText);

            Animation newAnimation = new Animation();
            newAnimation.m_tiltUV.SetValue(newTiltUV);
            newAnimation.m_defaultAnimationClipName = newDefaultClipName;
            newAnimation.m_isAutoPlay = newIsAutoPlay;
            newAnimation.m_millionSecondPerFrame.SetValue(newMSPF);

            // clips
            XmlNode clips = animation.SelectSingleNode("AnimationClips");
            foreach (XmlNode clip in clips.ChildNodes) {
                AnimationClip newClip = AnimationClip.LoadFromNode(clip, scene);
                newAnimation.addAnimationClip(newClip);
            }

            return newAnimation;
        }
    }
}
