using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Editor;
using System.Windows.Forms;
using Catsland.Core;

namespace Catsland.CatsEditor.EditorCommand {
    class OpenProjectCommand : ICommand {
        string projectPath = "";
        public OpenProjectCommand() {
            projectPath = "";
        }
        public OpenProjectCommand(string _projectPath) {
            projectPath = _projectPath;
        }

        public bool Execute(MapEditor _mapEditor) {
            if (projectPath == "") {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Catsland Project|*.xml";
                openFileDialog.FilterIndex = 1;
                if(openFileDialog.ShowDialog() == DialogResult.OK){
                    projectPath = openFileDialog.FileName;
                }
                else {
                    return false;
                }
                
            }

            String filename = projectPath;
                CatProject newProject = CatProject.OpenProject(filename, _mapEditor.m_gameEngine);
                _mapEditor.curProject = newProject;
                Mgr<CatProject>.Singleton = _mapEditor.curProject;

                // load current project
                _mapEditor.OpenScene(_mapEditor.curProject.GetSceneFileAddress(_mapEditor.curProject.currentSceneName));

                _mapEditor.m_gameEngine._gameInEditorMode = GameEngine.InEditorMode.Editing;
                _mapEditor.btn_play.Enabled = true;
                _mapEditor.btn_stop.Enabled = false;
                _mapEditor.btn_pause.Enabled = false;
                //Mgr<Camera>.Singleton.Reset();

                _mapEditor.updateEditorScriptMenu();
                _mapEditor.updateInsertComponentMenu();
                _mapEditor.UpdateEditorWindowFrame();

                return true;
        }
    }
}
