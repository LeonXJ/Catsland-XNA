using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace HorseRiding {
    public interface IQTEAction {
        void OnSuccess();
        void OnFail();
        void Announce(Keys _key, int _nonSkewedTimeInMS);
    }
}
