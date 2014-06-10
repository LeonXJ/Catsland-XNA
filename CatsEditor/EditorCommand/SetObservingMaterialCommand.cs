using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using Catsland.Core;

namespace Catsland.CatsEditor.EditorCommand {
    class SetObservingMaterialCommand : ICommand {
        string observingMaterialName = "";
        public SetObservingMaterialCommand(string _observingMaterialName) {
            observingMaterialName = _observingMaterialName;
        }
        public bool Execute(MapEditor _mapEditor) {
            // find the material
//             if (Mgr<CatProject>.Singleton != null
//                 && Mgr<CatProject>.Singleton.materialList1 != null) {
//                 MaterialTemplateList materialList = Mgr<CatProject>.Singleton.materialList1;
//                 Material material = materialList.GetMaterial(observingMaterialName);
//                 if (material != null) {
//                     _mapEditor.UpdateMaterialAttribute(material);
//                     return true;
//                 }
//             }
//             _mapEditor.UpdateMaterialAttribute(null);
            return false;
        }
    }
}
