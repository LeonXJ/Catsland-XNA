using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Catsland.Plugin.BasicPlugin {
    public class MotionDelegator : CatComponent {

        List<IMoiveClip> motions;
        List<IMoiveClip> removeList;

        public MotionDelegator():
            base(){}

        public MotionDelegator(GameObject gameObject) :
            base(gameObject) {
            if (Mgr<MotionDelegator>.Singleton != null) {
                Console.Out.WriteLine("Warning! Duplicated MotionDelegator.");
            }
            Mgr<MotionDelegator>.Singleton = this;
        }

        public override void Initialize(Scene scene) {
            motions = new List<IMoiveClip>();
            removeList = new List<IMoiveClip>();
        }

        public MotionDelegatorPack AddMotion(object RefValue, object TargetValue, int TotalTick,
            AnimationClip.PlayMode PlayMode = AnimationClip.PlayMode.CLAMP,
            MotionDelegatorPack.AccelerationMode AccelerationMode = MotionDelegatorPack.AccelerationMode.Constant,
            float ConstantVelocityPercent = 1.0f) {
            

            MotionDelegatorPack motionDelegatorPack = new MotionDelegatorPack(RefValue, TargetValue, TotalTick, PlayMode,
                ConstantVelocityPercent, AccelerationMode, PlayStatus.PLAYING);
            motions.Add(motionDelegatorPack);
            return motionDelegatorPack;
        }

        public MovieClip AddMovieClip() {
            MovieClip movieClip = new MovieClip(this);
            motions.Add(movieClip);
            movieClip.Play();
            return movieClip;
        }

        public override void Update(int timeLastFrame) {
            // update
            foreach (IMoiveClip imovieClip in motions) {
                bool end = imovieClip.Update(timeLastFrame);
                if (end) {
                    removeList.Add(imovieClip);
                }
            }
            // remove
            foreach (IMoiveClip imovieClip in removeList) {
                motions.Remove(imovieClip);
            }
            removeList.Clear();
        }

        public static string GetMenuNames() {
            return "Controller|Unique Motion Controller";
        }
    }

    
}
