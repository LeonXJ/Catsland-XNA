using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public interface ISelectable {
        /**
         * @brief whether the gameobject/component is selected
         * 
         * @param cameraX the position of mouse cursor in camera space
         * @param cameraY the position of mouse cursor in camera space
         * 
         * @result if it is selected
         * */
        bool IsSelected(float cameraX, float cameraY);
        GameObject GetGameObject();
        void DrawSelection();
        bool IsLocked();
    }
}
