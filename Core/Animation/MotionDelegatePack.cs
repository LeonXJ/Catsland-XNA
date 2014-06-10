using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class MotionDelegatorPack : IMoiveClip {
        object fromValue;
        object refValue;
        object toValue;
        int startTick;
        int totalTick;
        int curTick;
        bool pingPongVerse;
        AnimationClip.PlayMode playMode;
        public delegate int MotionCallback();
        MotionCallback actionFinished;

        float constantVelocityPercent = 1.0f;
        PlayStatus playStatus = PlayStatus.STOP;
        public enum AccelerationMode {
            Constant,
            Accelerate,
            Decelerate,
            AccelerateNDecelerate
        };

        AccelerationMode accelerationMode = AccelerationMode.Constant;

        // todo add callback function

        public MotionDelegatorPack(object RefValue, object ToValue,
            int TotalTick,
            AnimationClip.PlayMode PlayMode = AnimationClip.PlayMode.CLAMP,
            float ConstanceVelocityPercent = 1.0f, AccelerationMode AccMode = AccelerationMode.Constant,
            PlayStatus PlayStatus = PlayStatus.PLAYING) {
            if (RefValue.GetType() != ToValue.GetType()) {
                Console.Out.WriteLine("Error in " + typeof(MotionDelegatorPack).Name
                    + ", RefValue should equal TargetValue. Now RefValue is "
                    + RefValue.GetType().Name + ", but TargetValue is " + ToValue.GetType().Name);
            }
            refValue = RefValue;
            SetFromValue();
            SetFromAndToValue(ToValue);
            totalTick = TotalTick;
            curTick = 0;
            playMode = PlayMode;
            pingPongVerse = false;
            playStatus = PlayStatus;

            accelerationMode = AccMode;
            constantVelocityPercent = ConstanceVelocityPercent;
        }

        public void SetFinishMotion(MotionCallback motionCallback) {
            actionFinished = motionCallback;
        }

        private void SetFromValue() {
            Type type = refValue.GetType();
            if (type == typeof(CatVector3)) {
                CatVector3 value = (CatVector3)refValue;
                fromValue = value.ParameterClone();
            }
            else if (type == typeof(CatVector2)) {
                CatVector2 value = (CatVector2)refValue;
                fromValue = value.ParameterClone();
            }
            else if (type == typeof(CatFloat)) {
                CatFloat value = (CatFloat)refValue;
                fromValue = value.ParameterClone();
            }
            else if (type == typeof(CatInteger)) {
                CatInteger value = (CatInteger)refValue;
                fromValue = value.ParameterClone();
            }
        }

        private void SetFromAndToValue(object ToValue) {
            Type type = ToValue.GetType();
            if (type == typeof(CatVector3)) {
                CatVector3 value = (CatVector3)ToValue;
                toValue = value.ParameterClone();
            }
            else if (type == typeof(CatVector2)) {
                CatVector2 value = (CatVector2)ToValue;

                toValue = value.ParameterClone();
            }
            else if (type == typeof(CatFloat)) {
                CatFloat value = (CatFloat)ToValue;
                toValue = value.ParameterClone();
            }
            else if (type == typeof(CatInteger)) {
                CatInteger value = (CatInteger)ToValue;
                toValue = value.ParameterClone();
            }
        }

        public bool Update(int timelastFrame) {
            if (playStatus != PlayStatus.PLAYING) {
                return false;
            }

            bool end = false;
            switch (playMode) {
                case AnimationClip.PlayMode.STOP:
                case AnimationClip.PlayMode.CLAMP:
                    curTick += timelastFrame;
                    if (curTick > totalTick) {
                        curTick = totalTick;
                        UpdateValue();
                        FinishMotion();
                        end = true;
                    }
                    else {
                        UpdateValue();
                    }
                    break;
                case AnimationClip.PlayMode.LOOP:
                    curTick += timelastFrame;
                    while (curTick > totalTick) {
                        curTick -= totalTick;
                    }
                    UpdateValue();
                    break;
                case AnimationClip.PlayMode.PINGPONG:
                    if (pingPongVerse) {
                        curTick -= timelastFrame;
                        if (curTick < 0) {
                            curTick = -curTick;
                            pingPongVerse = false;
                        }
                    }
                    else {
                        curTick += timelastFrame;
                        if (curTick > totalTick) {
                            curTick = totalTick - (curTick - totalTick);
                            pingPongVerse = true;
                        }
                    }
                    UpdateValue();
                    break;
            }
            return end;
        }

        private void UpdateValue() {
            Type type = refValue.GetType();
            if (type == typeof(CatVector3)) {
                // delta
                CatVector3 _fromValue = (CatVector3)fromValue;
                CatVector3 _toValue = (CatVector3)toValue;
                CatVector3 delta = _toValue - _fromValue;
                // set value
                CatVector3 value = (CatVector3)refValue;
                float percent = (float)curTick / totalTick;
                value.SetValue(_fromValue.GetValue() + delta.GetValue() * GetCurveValue(percent));
            }
            else if (type == typeof(CatVector2)) {
                //delta
                CatVector2 _fromValue = (CatVector2)fromValue;
                CatVector2 _toValue = (CatVector2)toValue;
                CatVector2 delta = _toValue - _fromValue;
                // set value
                CatVector2 value = (CatVector2)refValue;
                float percent = (float)curTick / totalTick;
                value.SetValue(_fromValue.GetValue() + delta.GetValue() * GetCurveValue(percent));
            }
            else if (type == typeof(CatFloat)) {
                // delta
                float _fromValue = ((CatFloat)fromValue);
                float _toValue = ((CatFloat)toValue);
                float delta = _toValue - _fromValue;
                // set value
                CatFloat value = (CatFloat)refValue;
                value.SetValue(_fromValue + delta * GetCurveValue((float)curTick / totalTick));
            }
            else if (type == typeof(CatInteger)) {
                // delta
                int _fromValue = ((CatInteger)fromValue);
                int _toValue = ((CatInteger)toValue);
                int delta = _toValue - _fromValue;
                // set value
                CatInteger value = (CatInteger)refValue;
                value.SetValue(_fromValue + (int)(delta * GetCurveValue((float)curTick / totalTick)));
            }
        }

        public void Play() {
            SetFromValue();
            playStatus = PlayStatus.PLAYING;
        }

        public void Stop() {
            playStatus = PlayStatus.STOP;
        }

        public PlayStatus GetPlayStatus() {
            return playStatus;
        }

        public int GetTotalTick() {
            return totalTick;
        }

        public int GetStartTick() {
            return startTick;
        }

        private float GetCurveValue(float time) {
            if (accelerationMode == AccelerationMode.Constant) {
                return time;
            }
            else if (accelerationMode == AccelerationMode.Accelerate) {
                if (time < (1.0f - constantVelocityPercent)) {
                    return time * time / (1.0f - constantVelocityPercent * constantVelocityPercent);
                }
                else {
                    return (1.0f - constantVelocityPercent) / (1 + constantVelocityPercent) +
                        2.0f * (time + constantVelocityPercent - 1.0f) / (1.0f + constantVelocityPercent);
                }
            }
            else if (accelerationMode == AccelerationMode.Decelerate) {
                if (time < constantVelocityPercent) {
                    return 2.0f * time / (1 + constantVelocityPercent);
                }
                else {
                    float t = time - constantVelocityPercent;
                    return 2.0f * time / (1.0f + constantVelocityPercent)
                        + t * t / (constantVelocityPercent * constantVelocityPercent - 1.0f);
                }
            }
            else if (accelerationMode == AccelerationMode.AccelerateNDecelerate) {
                if (time < 0.5 * (1.0f - constantVelocityPercent)) {
                    return 2.0f * time * time / (1.0f - constantVelocityPercent * constantVelocityPercent);
                }
                else if (time < 0.5f * (1.0f + constantVelocityPercent)) {
                    float t = time - 0.5f * (1 - constantVelocityPercent);
                    return 0.5f * (1.0f - constantVelocityPercent) / (1.0f + constantVelocityPercent)
                        + 2.0f * t / (constantVelocityPercent + 1.0f);
                }
                else {
                    float t = time - constantVelocityPercent - 0.5f * (1.0f - constantVelocityPercent);
                    return 0.5f * (1.0f + 3.0f * constantVelocityPercent) / (1.0f + constantVelocityPercent) +
                        2.0f * t / (1.0f + constantVelocityPercent) - 2.0f * t * t / (1.0f - constantVelocityPercent * constantVelocityPercent);
                }
            }
            return 0.0f;
        }

        public void SetStartTick(int StartTick) {
            startTick = StartTick;
        }

        public int CompareTo(object iMovieClip) {
            return startTick - ((IMoiveClip)iMovieClip).GetStartTick();
        }

        private void FinishMotion() {
            if (actionFinished != null) {
                actionFinished();
            }
        }
    }
}
