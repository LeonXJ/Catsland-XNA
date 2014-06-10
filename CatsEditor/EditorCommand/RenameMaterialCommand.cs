using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using Catsland.Core;
using System.Windows.Forms;

namespace Catsland.CatsEditor.EditorCommand {
    class RenameMaterialCommand : ICommand {
        string oldName;
        string newName;
        public RenameMaterialCommand(string _oldName, string _newName) {
            oldName = _oldName;
            newName = _newName;
        }

        public bool Execute(MapEditor _mapEditor) {
            // old name == new name?
//             if (oldName == newName) {
//                 return false;
//             }
//             // judge if the name duplicate
//             MaterialTemplateList list = Mgr<CatProject>.Singleton.materialList1;
//             Material oldMaterial = list.GetMaterial(oldName);
//             // old material dose not exit
//             if (oldMaterial == null){
//                 MessageBox.Show("Error, no material has a name: " + oldName);
//                 return false;
//             }
//             // new name is occupied
//             if (list.ContainKey(newName)) {
//                 if (list.GetMaterial(newName) != oldMaterial) {
//                     MessageBox.Show("Error, existing material named: " + newName);
//                 }
//                 // else, dose not change the name at all
//                 return false;
//             }
//             // rename
//             list.RemoveItem(oldName);
//             oldMaterial._name = newName;
//             list.AddMaterial(oldMaterial);
            return true;
        }
    }
}
