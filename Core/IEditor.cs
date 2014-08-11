using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/**
 * @file IEditor
 * 
 * @author LeonXie
 * */

namespace Catsland.Core
{
    /**
     * @brief the interface of editor
     * */
    public interface IEditor
    {
        Point GetRenderAreaSize();
        IntPtr GetRenderAreaHandle();
        void UpdateGameObjectList(GameObjectList gameObjectList);
        void UpdateModelList(Dictionary<String, CatModel> models);
        void UpdateMaterialList(Dictionary<String, CatMaterial> materials);
        void GameObjectNameChanged();
        void UpdatePrefabList(PrefabList prefabs);
        GameObject GetSelectedGameObject();
        float GetObservingXYHeight();
        void SetCurrentProject(CatProject project, string root);
        void BindToScene(Scene scene);
        void GameEngineStarted();
        void LoadSceneComplete();
        void AdjustSelectedGameObjectPoistion(Vector2 _amount);
    }
}
