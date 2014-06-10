using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class ActionClip : IMoiveClip {

        #region Properties

        private int m_startTick = 0;

        #endregion

        virtual public void Play() {

        }

        public void Stop() {
            return;
        }

        public PlayStatus GetPlayStatus() {
            return PlayStatus.STOP;
        }
        
        public int GetTotalTick() {
            return 0;
        }
        
        public void SetStartTick(int StartTick) {
            m_startTick = StartTick;
        }

        public int GetStartTick() {
            return m_startTick;
        }

        public bool Update(int lastTimeFrame) {
            return true;
        }

        public int CompareTo(object movieClip) {
            return m_startTick - ((IMoiveClip)movieClip).GetStartTick();
        }
    }
}
