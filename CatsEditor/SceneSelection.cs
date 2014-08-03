using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;
using System.IO;
using Catsland.Editor;

namespace CatsEditor {
    public partial class SceneSelection : Form {
        CatProject project;
        public string SceneToLoad;

        public SceneSelection() {
            InitializeComponent();
        }

        public void InitializeData(CatProject _project) {
            _project.SynchronizeScene();
            project = _project;
            // scene for scene files
            sceneList.Items.Clear();
            string[] files = Directory.GetFiles(project.projectRoot + CatProject.RESOURCE_DIR + "/scene/", "*.scene");
            int selectionIndex = -1;
            int index = 0;
            foreach (string file in files) {
                string filename = Path.GetFileNameWithoutExtension(file);
                string displayName = filename;
                if (filename == project.startupSceneName) {
                    displayName += "<Startup>";
                }
                sceneList.Items.Add(displayName);
                if (filename == project.currentSceneName) {
                    selectionIndex = index;
                }
                ++index;
            }
            if (selectionIndex != -1) {
                sceneList.SetSelected(selectionIndex, true);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            // add new scene
            // find valid file name
            string sceneName = "UntitleScene";
            int index = 1;
            while (File.Exists(project.GetResourceSceneFileAddress(sceneName + index))) {
                ++index;
            }
            sceneName = sceneName + index;
            // create scene
            Scene newScene = Scene.CreateEmptyScene();
            newScene._sceneName = sceneName;
            // save scene
            newScene.SaveScene(project.GetResourceSceneFileAddress(sceneName));
            // update list
            InitializeData(project);
        }

        private void button2_Click(object sender, EventArgs e) {
            string selectionSceneName = (string)sceneList.SelectedItem;
            if (selectionSceneName != null) {
                // cannot remove the last one
                if (sceneList.Items.Count <= 1) {
                    MessageBox.Show("The project must have at least one scene.", "Error", MessageBoxButtons.OK);
                    return;
                }
                
                // remove appendix
                int index = selectionSceneName.IndexOf("<");
                if (index >= 0) {
                    selectionSceneName = selectionSceneName.Substring(0, index);
                }
                // dialog box
                MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("Remove scene " + selectionSceneName, "Remove?", messButton);
                if (dr == DialogResult.OK) {
                    // TODO: close current scene if it is removed
                    if (selectionSceneName == project.currentSceneName) {
                        DialogResult yeson = MessageBox.Show("Would you close and remove the scene being edited?", "Remove?", MessageBoxButtons.YesNo);
                        if (yeson == DialogResult.No) {
                            return;
                        }
                    }
                    // remove file
                    File.Delete(project.GetResourceSceneFileAddress(selectionSceneName));
                    // set current scene and startup scene if removed it
                    if (selectionSceneName == project.currentSceneName || selectionSceneName == project.startupSceneName) {
                        // find an existing scene file
                        string file = Directory.GetFiles(project.projectRoot + CatProject.RESOURCE_DIR + "/scene/", "*.scene")[0];
                        string filename = Path.GetFileNameWithoutExtension(file);

                        if (project.currentSceneName == selectionSceneName) {
                            project.currentSceneName = filename;
                        }
                        if (project.startupSceneName == selectionSceneName) {
                            project.startupSceneName = filename;
                        }
                        project.SaveProject(project.GetProjectXMLAddress());
                    }
                    InitializeData(project);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            string selectionSceneName = (string)sceneList.SelectedItem;
            if (selectionSceneName != null) {
                // remove appendix
                int index = selectionSceneName.IndexOf("<");
                if (index >= 0) {
                    selectionSceneName = selectionSceneName.Substring(0, index);
                }
                // update info
                project.startupSceneName = selectionSceneName;
                // update list
                InitializeData(project);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            string selectionSceneName = (string)sceneList.SelectedItem;
            if (selectionSceneName != null) {
                // remove appendix
                int index = selectionSceneName.IndexOf("<");
                if (index >= 0) {
                    selectionSceneName = selectionSceneName.Substring(0, index);
                }
                SceneToLoad = selectionSceneName;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button6_Click(object sender, EventArgs e) {
            string selectionSceneName = (string)sceneList.SelectedItem;
            if (selectionSceneName != null) {
                // remove appendix
                int index = selectionSceneName.IndexOf("<");
                if (index >= 0) {
                    selectionSceneName = selectionSceneName.Substring(0, index);
                }
                // show dialog box
                InputDialog inputDialog = new InputDialog();
                inputDialog.InitializeDate("Input Name for Scene", "Input a new name for the scene", selectionSceneName);
                if (inputDialog.ShowDialog(this) == DialogResult.OK) {
                    // rename
                    string newName = inputDialog.Result;
                    // update if it is current scene
                    bool isProjectFileModified = false;
                    if (selectionSceneName == project.currentSceneName) {
                        project.currentSceneName = newName;
                        isProjectFileModified = true;
                    }
                    if (selectionSceneName == project.startupSceneName) {
                        project.startupSceneName = newName;
                        isProjectFileModified = true;
                    }
                    if (isProjectFileModified) {
                        project.SaveProject(project.GetProjectXMLAddress());
                    }
                    // update file
                    File.Move(project.GetResourceSceneFileAddress(selectionSceneName),
                        project.GetResourceSceneFileAddress(newName));
                }
                // update list
                InitializeData(project);
            }
        }

    }
}
