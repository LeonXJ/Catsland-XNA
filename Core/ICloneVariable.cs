using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public interface IEffectParameter {
        IEffectParameter Clone();
        void FromString(string _value);
        void SetParameter(Effect _effect, string _name);
    }
}
