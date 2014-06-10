using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @file Drawable
 * 
 * define thing which would be rendered
 * 
 * @author LeonXie
 * */
namespace Catsland.Core {
    /**
     * @brief thing can be rendered
     * 
     * it should be able to compared with depth,
     * in order to conduct painter algorithm
     * */
    public interface Drawable : IComparable {
        void Draw(int timeLastFrame);

        /**
         * @brief the painting depth. deeper object will be covered by others
         * 
         * @result the depth
         * */
        float GetDepth();
    }
}
