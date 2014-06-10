using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.Core {
    public interface IUIDrawable {
        void Draw(SpriteBatch _spriteBatch, int _timeInMS);
    }
}
