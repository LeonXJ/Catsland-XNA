using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using Catsland.Core;

namespace Catsland.CatsEditor.EditorCommand {
    class SetObservingModelCommand : ICommand {
        string observingModelName = "";
        public SetObservingModelCommand(string _observingModelName) {
            observingModelName = _observingModelName;
        }
        public bool Execute(MapEditor _mapEditor) {
            // find the model
            if (Mgr<CatProject>.Singleton != null
                && Mgr<CatProject>.Singleton.modelList1 != null) {

                CatModelList modelList = Mgr<CatProject>.Singleton.modelList1;
                CatModel model = modelList.GetModel(observingModelName);
                if (model != null) {
                    _mapEditor.UpdateModelAttribute(model);
                    return true;
                }
            }
            _mapEditor.UpdateModelAttribute(null);
            return false;
        }
    }
}
