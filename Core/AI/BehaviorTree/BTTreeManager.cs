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

        private string m_btTreeWriteDirectory = "";
        public string BTTreeWriteDirectory {
            set {
                m_btTreeWriteDirectory = value;
            }
        }

        private Dictionary<string, BTTree> m_btTrees = new Dictionary<string, BTTree>();
    
#endregion

        public BTTree CreateAndSaveEmptyBTTree(string _name = "UntitleBTTree") {
            BTTree newTree = BTTree.CreateEmptyBTTree();
            // find a name
            string baseName = _name;
            string surfix = ".btt";
            int index = 0;
            string name = baseName;
            string fullname = name + surfix;
            string path = CatProject.GetStandardPath(m_btTreeReadDirectoryRoot);
            while (File.Exists(path + fullname)) {
                ++index;
                name = baseName + index;
                fullname = name + surfix;
            }
            m_btTrees.Add(name, newTree);
            newTree.Save(CatProject.GetStandardPath(m_btTreeWriteDirectory) + fullname);
            if (Mgr<CatProject>.Singleton != null) {
                Mgr<CatProject>.Singleton.SynchronizeBTTrees();
            }
            return newTree;
        }

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

        public bool HasBTTreeWithName(string _name) {
            // check in memory: new trees might have not been saved to files yet.
            if (m_btTrees.ContainsKey(_name)) {
                return true;
            }
            string[] files = Directory.GetFiles(m_btTreeReadDirectoryRoot, "*.btt");
            foreach (string file in files) {
                if (Path.GetFileNameWithoutExtension(file) == _name) {
                    return true;
                }
            }
            return false;
        }

        public void SaveAllBTTree(){
            if(m_btTrees == null){
                return;
            }
            
            if(Directory.Exists(m_btTreeWriteDirectory)){
                string fineDirectory = m_btTreeWriteDirectory;
                if (!m_btTreeWriteDirectory.EndsWith("/") && !m_btTreeWriteDirectory.EndsWith("\\")) {
                    fineDirectory = m_btTreeWriteDirectory + '/';
                }
                foreach(KeyValuePair<string, BTTree> keyValue in m_btTrees){
                    string filepath = fineDirectory + keyValue.Key + ".btt";
                    keyValue.Value.Save(filepath);
                }
            }
            else{
                Debug.Assert(false, "Cannot find directory: " + m_btTreeWriteDirectory);
            }
        }
    }
}
