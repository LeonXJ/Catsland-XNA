using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using Catsland.Core;
using System.Windows.Forms;

namespace Catsland.CatsEditor.EditorCommand {
    class NewProjectCommand : ICommand {
        public bool Execute(MapEditor _mapEditor) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                string filename = saveFileDialog.FileName;
                int index = filename.LastIndexOf("\\");
                string projectName = filename.Substring(index + 1);
                string directory = filename.Substring(0, index + 1);
                
                CatProject newProject = CatProject.CreateEmptyProject(projectName, directory, _mapEditor.m_gameEngine);
                _mapEditor.ExecuteCommend(new OpenProjectCommand(newProject.GetProjectXMLAddress()));

                return true;
            }
            return false;
        }
    }
}
