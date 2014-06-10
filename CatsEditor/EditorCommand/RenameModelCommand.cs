using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using Catsland.Core;
using System.Windows.Forms;

namespace Catsland.CatsEditor.EditorCommand {
    class RenameModelCommand : ICommand {
        string oldName;
        string newName;
        public RenameModelCommand(string _oldName, string _newName) {
            oldName = _oldName;
            newName = _newName;
        }

        public bool Execute(MapEditor _mapEditor) {
            // old name == new name?
            if (oldName == newName) {
                return false;
            }
            // judge if the name duplicate
            CatModelList list = Mgr<CatProject>.Singleton.modelList1;
            CatModel oldModel = list.GetModel(oldName);
            // old material dose not exit
            if (oldModel == null) {
                MessageBox.Show("Error, no model has a name: " + oldName);
                return false;
            }
            // new name is occupied
            if (list.ContainKey(newName)) {
                if (list.GetModel(newName) != oldModel) {
                    MessageBox.Show("Error, existing model named: " + newName);
                }
                // else, dose not change the name at all
                return false;
            }
            // rename
            list.RemoveItem(oldName);
            oldModel.SetName(newName);
            list.AddModel(oldModel);
            return true;
        }
    }
}
