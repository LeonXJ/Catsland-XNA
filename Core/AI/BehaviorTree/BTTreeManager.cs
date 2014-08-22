using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Catsland.Core {
    public class BTTreeManager {

#region Properties

        private string m_btTreeReadDirectoryRoot = "";
        public string BTTreeDirectoryRoot{
            set {
                m_btTreeReadDirectoryRoot = value;
            }
        }

//         private string m_btTreeWriteDirectory = "";
//         public string BTTreeWriteDirectory {
//             set {
//                 m_btTreeWriteDirectory = value;
//             }
//         }

        private Dictionary<string, BTTree> m_btTrees = new Dictionary<string, BTTree>();
    
#endregion

        /**
         * @brief load and update bttree. (if exist, it update the existing tree)
         **/
        public BTTree LoadBTTree(string _name, bool _forceLoad = false) {
            if (!_forceLoad && m_btTrees.ContainsKey(_name)) {
                return m_btTrees[_name];
            }
            if(!m_btTreeReadDirectoryRoot.EndsWith("/") && !m_btTreeReadDirectoryRoot.EndsWith("\\")){
                m_btTreeReadDirectoryRoot += '/';
            }
            string filepath = m_btTreeReadDirectoryRoot + _name + ".btt";
            if(File.Exists(filepath)){
                BTTree btTree = BTTree.Load(filepath);
                if(m_btTrees.ContainsKey(_name)){
                    // Update tree
                    m_btTrees[_name] = btTree;
                }
                m_btTrees.Add(_name, btTree);
                return btTree;
            }
            else{
                Debug.Assert(false, "Cannot find BTTree: " + filepath);
                return null;
            }
        }

        [Obsolete]
        public void AddBTTree(string _name, BTTree _btTree){
            m_btTrees.Add(_name, _btTree);
        }

        public void SaveAllBTTree(string _directory){
            if(m_btTrees == null){
                return;
            }
            if(Directory.Exists(_directory)){
                string fineDirectory = _directory;
                if(!_directory.EndsWith("/") && !_directory.EndsWith("\\")){
                    fineDirectory = _directory + '/';
                }
                foreach(KeyValuePair<string, BTTree> keyValue in m_btTrees){
                    string filepath = fineDirectory + keyValue.Key + ".btt";
                    keyValue.Value.Save(filepath);
                }
            }
            else{
                Debug.Assert(false, "Cannot find directory: " + _directory);
            }
        }
    }
}
