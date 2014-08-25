using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Catsland.Core {
    public class BTTree {

#region Properties

        private BTNode m_root;
        public BTNode Root{
            set {
                m_root = value;
            }
            get {
                return m_root;
            }
        }

#endregion

        /**
         * @brief load bttree from file
         **/ 
        public static BTTree Load(string _filepath) {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filepath);

            BTTree newBTTree = new BTTree();
            
            XmlNode nodeBTTree = doc.SelectSingleNode("BTTree");
            XmlNode nodeRoot = nodeBTTree.SelectSingleNode("Root");
            XmlNode nodeRealRoot = nodeRoot.FirstChild;

            Serialable.BeginSupportingDelayBinding();
            newBTTree.m_root = Serialable.DoUnserial(nodeRealRoot) as BTNode;
            Serialable.EndSupportingDelayBinding(true);
            return newBTTree;
        }

        /**
         * @brief save bttree to file
         **/ 
        public void Save(string _filepath) {
            if (m_root == null) {
                return;
            }
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);

            XmlElement eleBTTree = doc.CreateElement("BTTree");
            doc.AppendChild(eleBTTree);
            XmlElement eleRoot = doc.CreateElement("Root");
            eleBTTree.AppendChild(eleRoot);
            XmlNode nodeRealRoot = m_root.DoSerial(doc);
            eleRoot.AppendChild(nodeRealRoot);

            doc.Save(_filepath);
        }

        public BTNode FindParent(BTNode _target) {
            if (_target == null || _target == m_root) {
                return null;
            }
            if (m_root != null) {
                return m_root.FindParent(_target);
            }
            return null;
            
        }

        public bool RemoveSubTree(BTNode _subRoot) {
            if (_subRoot != null) {
                BTNode parent = FindParent(_subRoot);
                if (parent != null) {
                    parent.RemoveChild(_subRoot);
                    return true;
                }
            }
            return false;
        }
    }
}
