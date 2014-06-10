using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    public class PostProcessManager : Serialable {

        #region Properties

        [SerialAttribute]
        Dictionary<string, PostProcess> m_postProcesses =
            new Dictionary<string, PostProcess>();
        string m_endPostProcessName = null;

        #endregion

        public Dictionary<string, PostProcess> GetPostProcessDictionary() {
            return m_postProcesses;
        }

        public PostProcess GetPostProcess(string _name) {
            if (m_postProcesses.ContainsKey(_name)) {
                return m_postProcesses[_name];
            }
            else {
                return null;
            }
        }

        public bool AddPostProcess(string _name, PostProcess _postProcess,
            bool _isEndPostProcess = true) {
            if (m_postProcesses.ContainsKey(_name)) {
                return false;
            }
            else {
                m_postProcesses.Add(_name, _postProcess);
                m_endPostProcessName = _name;
                return true;
            }
        }

        public bool SetEndPostProcess(string _name) {
            if (!m_postProcesses.ContainsKey(_name)) {
                return false;
            }
            else {
                m_endPostProcessName = _name;
                return true;
            }
        }

        public PostProcess GetEndProcess() {
            if (m_endPostProcessName == null) {
                return null;
            }
            if (m_postProcesses.ContainsKey(m_endPostProcessName)) {
                return m_postProcesses[m_endPostProcessName];
            }
            else {
                return null;
            }
            
        }

        public void UpdateBuffers() {
            foreach (KeyValuePair<string, PostProcess> keyValues in m_postProcesses) {
                keyValues.Value.UpdateBuffer();
            }
        }

    }
}
