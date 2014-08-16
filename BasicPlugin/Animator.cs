using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Catsland.Core;
using Catsland.Editor;
using System.ComponentModel;

namespace Catsland.Plugin.BasicPlugin
{
    public class Animator : CatComponent
    {
#region Properties

        private int m_currentIndex;
        private int m_timeLastFrame;
        
        private bool m_isPong;

        [SerialAttribute]
        private readonly CatBool m_isPlaying = new CatBool(true);
        public bool IsPlaying {
            set {
                m_isPlaying.SetValue(value);
            }
            get {
                return m_isPlaying;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_millionSecondPreFrame;
        public int MillionSecondPerFrame {
            set {
                m_millionSecondPreFrame.SetValue(value);
            }
            get {
                return m_millionSecondPreFrame.GetValue();
            }
        }
        public CatInteger MillionSecondPerFrameRef {
            get {
                return m_millionSecondPreFrame;
            }
        }

        [SerialAttribute]
        private string m_currentAnimationName;
        [CategoryAttribute("Animation"),
            EditorAttribute(typeof(PropertyGridAnimationSelector),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string AnimationName
        {
            get { return m_currentAnimationName; }
            set { PlayAnimation(value); }
        }

        // TODO: deprecate
         public bool Mirror {
             set {
             }
             get {
                 return false;
             }
         }

#endregion

         public Animator() {
             m_millionSecondPreFrame = new CatInteger(50);
             m_currentAnimationName = "";
             m_currentIndex = 0;
             m_timeLastFrame = 0;
             m_isPlaying.SetValue(false);
             m_isPong = false;  
         }

        public Animator(GameObject gameObject)
            : base(gameObject)
        {
             m_millionSecondPreFrame = new CatInteger(50);
             m_currentAnimationName = "";
             m_currentIndex = 0;
             m_timeLastFrame = 0;
             m_isPlaying.SetValue(false);
             m_isPong = false;  
        }

//         public override CatComponent CloneComponent(GameObject gameObject)
//         {
//             Animator animator = new Animator(gameObject);
//             animator.AnimationName = m_currentAnimationName;
//             return animator;
//         }

        // needs: quadRender, quadRender.Animation, Model,

        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            // configuration
            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).Name);
            if (modelComponent == null || modelComponent.Model == null)
            {
                return;
            }
            Animation animation = modelComponent.Model.GetAnimation();

            m_timeLastFrame = 0;
            m_isPlaying.SetValue(animation.m_isAutoPlay);
            m_currentIndex = 0;
            if (m_currentAnimationName == null || m_currentAnimationName == "") {

                m_currentAnimationName = animation.m_defaultAnimationClipName;
            }
            m_isPong = false;
            m_millionSecondPreFrame.SetValue(animation.m_millionSecondPerFrame);
        }

        public override void EditorUpdate(int timeLastFrame)
        {
            base.EditorUpdate(timeLastFrame);

            Update(timeLastFrame);
        }

        public override void Update(int timeLastFrame)
        {
            base.Update(timeLastFrame);

            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).ToString());
            if (modelComponent == null)
            {
                Console.Out.WriteLine("No ModelComponent for Animator.");
                return;
            }

            Animation animation = modelComponent.Model.GetAnimation();
            QuadRender quadRender = (QuadRender)m_gameObject.GetComponent(typeof(QuadRender).ToString());

            float width_per_frame = 1.0f / animation.m_tiltUV.X;
            float height_per_frame = 1.0f / animation.m_tiltUV.Y;

            if (m_isPlaying)
            {
                m_timeLastFrame += timeLastFrame;
            }

            if (m_timeLastFrame >= m_millionSecondPreFrame)
            {
                m_timeLastFrame -= m_millionSecondPreFrame;
                AnimationClip current_clip = animation.getAnimationClip(m_currentAnimationName);
                if (current_clip == null)
                {
                    Console.WriteLine("Error! Can not find animation clip: " + m_currentAnimationName);
                    return;
                }
                if (current_clip.m_mode == AnimationClip.PlayMode.PINGPONG)
                {
                    if (!m_isPong) // ping
                    {
                        if (m_currentIndex < current_clip.m_endIndex)
                        {
                            ++m_currentIndex;
                        }
                        else
                        {
                            --m_currentIndex;
                            m_isPong = true;
                        }
                    }
                    else
                    { // pong
                        if (m_currentIndex > current_clip.m_beginIndex)
                        {
                            --m_currentIndex;
                        }
                        else
                        {
                            ++m_currentIndex;
                            m_isPong = false;
                        }
                    }
                }
                else
                {
                    if (m_currentIndex < current_clip.m_endIndex)
                    {
                        ++m_currentIndex;
                    }
                    else
                    {
                        if (current_clip.m_mode == AnimationClip.PlayMode.CLAMP)
                        {
                            m_isPlaying.SetValue(false);
                        }
                        else if (current_clip.m_mode == AnimationClip.PlayMode.LOOP)
                        {
                            m_currentIndex = current_clip.m_beginIndex;
                        }
                        else if (current_clip.m_mode == AnimationClip.PlayMode.STOP)
                        {
                            m_currentIndex = current_clip.m_beginIndex;
                            m_isPlaying.SetValue(false);
                        }
                    }
                }
            }

            Point current_nxm = new Point();
            current_nxm.X = m_currentIndex % animation.m_tiltUV.X;
            current_nxm.Y = m_currentIndex / animation.m_tiltUV.X;

            quadRender.m_vertex[0].TextureCoordinate.X = current_nxm.X * width_per_frame;
            quadRender.m_vertex[0].TextureCoordinate.Y = (current_nxm.Y + 1) * height_per_frame;
            quadRender.m_vertex[1].TextureCoordinate.X = current_nxm.X * width_per_frame;
            quadRender.m_vertex[1].TextureCoordinate.Y = current_nxm.Y * height_per_frame;
            quadRender.m_vertex[2].TextureCoordinate.X = (current_nxm.X + 1)* width_per_frame;
            quadRender.m_vertex[2].TextureCoordinate.Y = (current_nxm.Y + 1) * height_per_frame;
            quadRender.m_vertex[3].TextureCoordinate.X = (current_nxm.X + 1) * width_per_frame;
            quadRender.m_vertex[3].TextureCoordinate.Y = current_nxm.Y * height_per_frame;
        }

        public void fadeToAnimation(String animationClipName)
        {
            // cur_index
            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).Name);
            if (modelComponent == null)
            {
                Console.Out.WriteLine("No ModelComponent for Animator.");
                return;
            }
            Animation animation = modelComponent.Model.GetAnimation();
            AnimationClip cur_clip = animation.getAnimationClip(m_currentAnimationName);
            int cur_beginIndex = cur_clip.m_beginIndex;
            int cur_endIndex = cur_clip.m_endIndex;
            // target index
            AnimationClip target_clip = animation.getAnimationClip(animationClipName);
            if (target_clip != null) {
                int target_beginIndex = target_clip.m_beginIndex;
                int target_endIndex = target_clip.m_endIndex;

                int index = target_beginIndex
                    + (int)((target_endIndex - target_beginIndex + 1) * (float)(m_currentIndex - cur_beginIndex) / (cur_endIndex - cur_beginIndex + 1));
                m_currentIndex = index;
                m_currentAnimationName = animationClipName;
                m_isPlaying.SetValue(true);
            }
        }

        public void PlayAnimation(String animationClipName)
        {
            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).ToString());
            if (modelComponent == null || modelComponent.Model == null)
            {
                return;
            }
            Animation animation = modelComponent.Model.GetAnimation();
            AnimationClip target_clip = animation.getAnimationClip(animationClipName);
            if (target_clip == null)
            {
                return;
            }
            m_currentIndex= target_clip.m_beginIndex;
            m_currentAnimationName = animationClipName;
            m_isPlaying.SetValue(true);
        }

        public void CheckPlayAnimation(String animationClipName)
        {
            if (m_currentAnimationName != animationClipName)
            {
                PlayAnimation(animationClipName);
            }
            m_isPlaying.SetValue(true);
        }

        public void Pause()
        {
            m_isPlaying.SetValue(false);
        }

//         public override bool SaveToNode(XmlNode node, XmlDocument doc)
//         {
//             XmlElement animator = doc.CreateElement(typeof(Animator).Name);
//             node.AppendChild(animator);
// 
//             animator.SetAttribute("currentClip", m_currentAnimationName);
//             //animator.SetAttribute("isXMirror", "" + m_isMirror);
//             animator.SetAttribute("animationName", m_currentAnimationName);
// 
//             return true;
//         }

//         public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
//         {
//             base.ConfigureFromNode(node, scene, gameObject);
// 
//             m_currentAnimationName = node.GetAttribute("currentClip");
//             m_isMirror = bool.Parse(node.GetAttribute("isXMirror"));
//             AnimationName = node.GetAttribute("animationName");
//         }

        public static string GetMenuNames() {
            return "Animation|Animator";
        }

        public override CatModelInstance GetModel()
        {
            ModelComponent modelComponent = (ModelComponent)m_gameObject
                .GetComponent(typeof(ModelComponent).ToString());
            return modelComponent.GetCatModelInstance();
        }
    }
}
