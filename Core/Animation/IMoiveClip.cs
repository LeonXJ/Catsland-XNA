using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public enum PlayStatus  {
        STOP,
        PLAYING,
    };

    public interface IMoiveClip : IComparable {
        void Play();
        void Stop();
        
        PlayStatus GetPlayStatus();
        int GetTotalTick();
        void SetStartTick(int StartTick);
        int GetStartTick();
        bool Update(int lastTimeFrame);
    }
}
