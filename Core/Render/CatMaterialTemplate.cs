using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace Catsland.Core {
    public class CatMaterialTemplate {
        private string m_name;
        private Effect m_effect;
        private CatMaterial m_materialPrototype;
        private Dictionary<string, bool> m_maskedParameters;    // attributes not shown in editor

        private CatMaterialTemplate() {
            m_effect = null;
            m_materialPrototype = new CatMaterial(this);
            m_maskedParameters = new Dictionary<string, bool>();
        }

        public void SetName(string _name) {
            m_name = _name;
        }

        public string GetName() {
            return m_name;
        }

        public Effect GetEffect() {
            return m_effect;
        }

        public CatMaterial GetMaterialPrototype(){
            return m_materialPrototype;
        }

        /**
         * @brief Whether the parameter should be shown in editor
         * 
         * @param _parameterName
         * 
         * @result true/false
         * */
        public bool IsParameterMaskedInEditor(string _parameterName) {
            return m_maskedParameters.ContainsKey(_parameterName);
        }

        /**
         * @brief Create materialTemplate from .material file
         * 
         * @param _filepath filepath
         * 
         * @result the materialTemplate
         * */
        public static CatMaterialTemplate LoadMaterialTemplate(string _filepath){
            // create material template
            CatMaterialTemplate newMaterialTemplate = new CatMaterialTemplate();
            CatMaterial materialPrototype = newMaterialTemplate.GetMaterialPrototype();
            // load .material file
            XmlDocument doc = new XmlDocument();
            doc.Load(_filepath);
            XmlNode nodeMaterial = doc.SelectSingleNode("Material");
            XmlElement eleMaterial = (XmlElement)nodeMaterial;
            string materialName = eleMaterial.GetAttribute("fxname");
            // read material parameter
            if (nodeMaterial != null) {
                foreach (XmlNode node in nodeMaterial.ChildNodes) {
                    XmlElement eleNode = (XmlElement)node;
                    string parameterName = eleNode.GetAttribute("name");
                    string parameterType = eleNode.GetAttribute("type");
                    string parameterValue = eleNode.GetAttribute("value");
                    IEffectParameter variable = CatMaterial.CreateVariable(parameterType, parameterValue);
                    if (variable != null) {
                        materialPrototype.AddParameter(parameterName, variable);
                    }
                    // check if should show in editor, by checking whether note="NotInEditor"
                    string parameterNote = eleNode.GetAttribute("note");
                    if (parameterNote != null && parameterNote.Contains("NotInEditor")) {
                        newMaterialTemplate.m_maskedParameters.Add(parameterName, true);
                    }
                }
            }
            // load effect
            newMaterialTemplate.m_effect = Mgr<CatProject>.Singleton.contentManger.Load<Effect>("effect\\" + materialName);
            
            return newMaterialTemplate;
        } 
    }
}
