using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;

/**
 * @file MaterialList
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    public class CatMaterialTemplateList : UniqueList<CatMaterial> {

        private Dictionary<string, CatMaterialTemplate> m_materialTemplates;
        
        protected CatMaterialTemplateList() {
            m_materialTemplates = new Dictionary<string, CatMaterialTemplate>();
        }


        /**
         * @brief Load material template under a directory
         * 
         * @param _materialDirectory material templates directory
         * 
         * @result materialTemplateList containing the templates
         * */
        public static CatMaterialTemplateList LoadMaterialTemplates(string _materialDirectory) {

            CatMaterialTemplateList materialList = new CatMaterialTemplateList();

            // search for .material files under _materialDirectory
            if (!Directory.Exists(_materialDirectory)) {
                return materialList;
            }
            string[] files = Directory.GetFiles(_materialDirectory, "*.material");
            foreach (string file in files) {
                CatMaterialTemplate materialTempalte = CatMaterialTemplate.LoadMaterialTemplate(file);
                // get name from file
                // TODO: add sub-directory support
                string materialName = Path.GetFileNameWithoutExtension(file);
                materialTempalte.SetName(materialName);
                materialList.m_materialTemplates.Add(materialName, materialTempalte);
            }
            return materialList;
        }

        // for editor use only
        public Dictionary<string, CatMaterialTemplate> GetMaterialTemplateList() {
            return m_materialTemplates;
        }

        /**
         * @brief Create a material according to _node and corresponding template in the list
         * 
         * @param _node xml node
         * 
         * @result the material
         * */
        public CatMaterial CreateMaterialInstanceByXml(XmlNode _node) {
            // get name of materialTemplate
            XmlElement node = (XmlElement)_node;
            if (node == null) {
                return null;
            }
            string name = node.GetAttribute("name");

            if (m_materialTemplates != null && m_materialTemplates.ContainsKey(name)) {
                // fetch the material prototype
                // TODO: set name in constructor
                CatMaterial newMaterial = m_materialTemplates[name].GetMaterialPrototype().Clone();
                //newMaterial.m_name = name;
                // read and set parameters
                foreach (XmlNode child in node.ChildNodes) {
                    XmlElement eleChild = (XmlElement)child;
                    string childName = eleChild.GetAttribute("name");
                    string childType = eleChild.GetAttribute("type");
                    string childValue = eleChild.GetAttribute("value");
                    IEffectParameter parameter = CatMaterial.CreateVariable(childType, childValue);
                    if (parameter != null) {
                        newMaterial.SetParameter(childName, parameter);
                    }
                }
                return newMaterial;
            }
            Debug.Assert(false, "Cannot find material template name: " + name);
            return null;
        }

        public CatMaterial GetMaterialPrototype(string _name) {
            if (m_materialTemplates!=null && m_materialTemplates.ContainsKey(_name)) {
                return m_materialTemplates[_name].GetMaterialPrototype();
            }
            return null;
        }
    }
}
