using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Catsland.Core {
    public class MotionDelegator{

        List<IMoiveClip> motions = new List<IMoiveClip>();
        List<IMoiveClip> removeList = new List<IMoiveClip>();

        public MotionDelegator():
            base(){}

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

        public void Update(int timeLastFrame) {
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
    }

    
}
