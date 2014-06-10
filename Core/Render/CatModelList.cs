using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

/**
 * @file ModelList holds the list of CatsModel
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {

    /**
     * @brief ModelList holds the list of CatsModel and hold by Scene
     * */
    public class CatModelList : UniqueList<CatModel> {
        public void AddModel(CatModel model) {
            base.AddItem(model.GetName(), model);
            if (Mgr<GameEngine>.Singleton._gameEngineMode
                == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateModelList(contentList);
            }

        }

        public override void ReleaseAll() {
            base.ReleaseAll();

            if (Mgr<GameEngine>.Singleton._gameEngineMode
                == GameEngine.GameEngineMode.MapEditor) {
                Mgr<GameEngine>.Singleton.Editor.UpdateModelList(contentList);
            }
        }

        public CatModel GetModel(String name) {
            return base.GetItem(name);
        }

        /**
         * @brief Save models to _filepath (directory)
         * 
         * @param _filepath should be a directory
         * 
         * @result success?
         * */
        public bool SaveModels(string _filepath) {
            if (contentList == null) {
                return true;
            }
            foreach (KeyValuePair<string, CatModel> keyValue in contentList)
            {
                string modelFile = _filepath + "\\" + keyValue.Key + ".model";
                XmlDocument doc = new XmlDocument();
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(dec);
                XmlNode nodeModel = keyValue.Value.DoSerial(doc);
                doc.AppendChild(nodeModel);
                doc.Save(modelFile);
            }
            return true;
        }

        /**
         * @brief Load models under the directory
         * 
         * @param _modelDirectory the directory containing .model files
         * @param _project CatProject
         * 
         * @result the CatModelList
         * */
        public static CatModelList LoadModels(string _modelDirectory, CatProject _project) {
            // create
            CatModelList modelList = new CatModelList();

            // search for .material files under _materialDirectory
            if (!Directory.Exists(_modelDirectory)) {
                return modelList;
            }
            string[] files = Directory.GetFiles(_modelDirectory, "*.model");
            foreach (string file in files) {   
                XmlDocument doc = new XmlDocument();
                doc.Load(file);
                XmlNode nodeModel = doc.SelectSingleNode(
                                                typeof(CatModel).ToString());
                Serialable.BeginSupportingDelayBinding();
                CatModel catsModel = CatModel.DoUnserial(nodeModel) as CatModel;
                Serialable.EndSupportingDelayBinding();
                modelList.AddModel(catsModel);
            }
            return modelList;
        }
    }
}
