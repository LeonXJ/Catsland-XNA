using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class MovieClip : IMoiveClip {
        List<IMoiveClip> movieClips;
        PlayStatus playStatus;
        MotionDelegator motionDelegator;

        int editCurTick;
        int curTick;
        int startTick = 0;
        int curIndex = 0;

        public MovieClip(MotionDelegator MotionDelegator) {
            movieClips = new List<IMoiveClip>();
            editCurTick = 0;
            playStatus = PlayStatus.STOP;
            motionDelegator = MotionDelegator;
        }

        public int CompareTo(object movieClip) {
            return startTick - ((IMoiveClip)movieClip).GetStartTick();
        }

        public void Initialize(){
            movieClips.Sort();
        }

        public void AddMovieClip(IMoiveClip movieClip) {
            movieClips.Add(movieClip);   
        }

        public void AppendMovieClip(IMoiveClip movieClip) {
            movieClips.Add(movieClip);
            movieClip.SetStartTick(editCurTick);
            editCurTick += movieClip.GetTotalTick();
        }

        public int GetEditCurClip() {
            return editCurTick;
        }

        public void Play() {
            curTick = 0;
            curIndex = 0;
            playStatus = PlayStatus.PLAYING;
        }

        public void Stop() {
            curTick = 0;
            playStatus = PlayStatus.STOP;
        }

        public void SetStartTick(int StartTick) {
            startTick = StartTick;
        }

        public int GetStartTick() {
            return startTick;
        }

        public int GetTotalTick() {
            // calculate total tick
            if (movieClips != null) {
                int totalTime = 0;
                foreach (IMoiveClip movieClip in movieClips) {
                    totalTime += movieClip.GetTotalTick();
                }
                return totalTime;
            }
            return 0;
        }

        public bool Update(int timeLastFrame) {
            if (playStatus != PlayStatus.PLAYING) {
                return false;
            }
            // update
            curTick += timeLastFrame;

            while (curIndex < movieClips.Count 
                && curTick > movieClips[curIndex].GetStartTick()) {
                movieClips[curIndex].Play();
                ++ curIndex;
            }

            if (curIndex >= movieClips.Count) {
                return true;
            }
            return false;
        }

        public PlayStatus GetPlayStatus() {
            return playStatus;
        }

        public IMoiveClip AddMotion(object RefValue, object ToValue,
            int StartTick, int TotalTick,
            AnimationClip.PlayMode PlayMode = AnimationClip.PlayMode.CLAMP,
            float ConstanceVelocityPercent = 1.0f, 
            MotionDelegatorPack.AccelerationMode AccMode = MotionDelegatorPack.AccelerationMode.Constant) {

                MotionDelegatorPack motionDelegatorPack = motionDelegator.AddMotion(RefValue, ToValue,
                    TotalTick, PlayMode, AccMode, ConstanceVelocityPercent);
                motionDelegatorPack.Stop();
                motionDelegatorPack.SetStartTick(StartTick);
                movieClips.Add(motionDelegatorPack);

                if (StartTick + TotalTick > editCurTick) {
                    editCurTick = StartTick + TotalTick;
                }
            return motionDelegatorPack;
                
        }

        public IMoiveClip AppendMotion(object RefValue, object ToValue,
            int TotalTick,
            AnimationClip.PlayMode PlayMode = AnimationClip.PlayMode.CLAMP,
            float ConstanceVelocityPercent = 1.0f,
            MotionDelegatorPack.AccelerationMode AccMode = MotionDelegatorPack.AccelerationMode.Constant) {
                MotionDelegatorPack motionDelegatorPack = motionDelegator.AddMotion(RefValue, ToValue,
                    TotalTick, PlayMode, AccMode, ConstanceVelocityPercent);
                motionDelegatorPack.Stop();
                motionDelegatorPack.SetStartTick(editCurTick);
                editCurTick += motionDelegatorPack.GetTotalTick();
                movieClips.Add(motionDelegatorPack);
                return motionDelegatorPack;
        }

        public void AppendEmptyTime(int length) {
            editCurTick += length;
        }
    }
}
