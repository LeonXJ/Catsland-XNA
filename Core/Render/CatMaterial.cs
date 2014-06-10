using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Diagnostics;

/**
 * @file Material
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief the base class of Material
     * */
    public class CatMaterial {

        private string m_name;
        private Dictionary<string, IEffectParameter> m_parameters = new Dictionary<string, IEffectParameter>();
        private CatMaterialTemplate m_materialTemplate;

        public CatMaterial(CatMaterialTemplate _materialTemplate) {
            m_name = _materialTemplate.GetName();
            m_parameters = new Dictionary<string, IEffectParameter>();
            m_materialTemplate = _materialTemplate;
        }

        public string GetName() {
            return m_name;
        }

        /**
         * @brief Switch to another materialTemplate. Keep as many parameters as possible
         *        name and type are checked
         * 
         * @param _materialTemplate the target material template
         * */
        public void ChangeMaterialTemplate(CatMaterialTemplate _materialTemplate) {
            Debug.Assert(_materialTemplate!=null, "Null target material template.");

            Dictionary<string, IEffectParameter> newParameters = new Dictionary<string, IEffectParameter>();
            Dictionary<string, IEffectParameter> targetParameters = _materialTemplate.GetMaterialPrototype().m_parameters;
            foreach (KeyValuePair<string, IEffectParameter> keyValue in m_parameters) {
                string parameterName = keyValue.Key;
                if (targetParameters.ContainsKey(parameterName)) {
                    if (keyValue.Value.GetType() == targetParameters[parameterName].GetType()) {
                        // same name and same type, copy
                        newParameters[parameterName] = keyValue.Value.ParameterClone();
                    }
                    else {
                        // type not match
                        newParameters[parameterName] = targetParameters[parameterName].ParameterClone();
                    }
                }
            }
            // add missing parameters
            foreach (KeyValuePair<string, IEffectParameter> keyValue in targetParameters) {
                if (!newParameters.ContainsKey(keyValue.Key)) {
                    newParameters[keyValue.Key] = keyValue.Value.ParameterClone();
                }
            }

            m_name = _materialTemplate.GetName();
            m_parameters = newParameters;
            m_materialTemplate = _materialTemplate;   
        }

        public CatMaterialTemplate GetMaterialTemplate() {
            return m_materialTemplate;
        }

        /**
         * @brief Add a parameter, the _variable is cloned inside the function
         * 
         * @param _variable, the parameter to be added
         * 
         * @result success?
         * */
        public bool AddParameter(string _name, IEffectParameter _variable) {
            if (m_parameters.ContainsKey(_name)) {
                return false;
            }
            m_parameters.Add(_name, _variable.ParameterClone());
            return true;
        }

        /**
         * @brief Set the value of an existing parameter
         * 
         * @param _name name of the parameter
         * @param _value value of the parameter
         * 
         * @result success?
         * */
        // TODO: check whether it needs deep copy
        public bool SetParameter(string _name, IEffectParameter _value) {
            if (m_parameters.ContainsKey(_name)) {
                m_parameters[_name] = _value;
                return true;
            }
            return false;
        }

        public CatMaterial Clone() {
            CatMaterial newMaterial = new CatMaterial(m_materialTemplate);
            // parameters
            if (m_parameters != null) {
                foreach(KeyValuePair<string, IEffectParameter> keyValue in m_parameters){
                    newMaterial.AddParameter(keyValue.Key, keyValue.Value);
                }
            }
            return newMaterial;
        }

        /**
         * @brief Set the parameters into effect
         * 
         * @result the resulting effect
         * */
        public Effect ApplyMaterial() {
            Effect effect = m_materialTemplate.GetEffect();
            foreach (KeyValuePair<string, IEffectParameter> keyValue in m_parameters) {
                keyValue.Value.SetParameter(effect, keyValue.Key);
            }
            return effect;
        }

        /**
         * @brief Create a variable according to type and value
         * 
         * @param _type the string of the type
         * @param _value the string value
         * 
         * @result the variable
         * */
        public static IEffectParameter CreateVariable(string _type, string _value) {
            IEffectParameter newVariable = null;
            if (_type == typeof(CatMatrix).ToString()) {
                newVariable = new CatMatrix();
            }
            else if (_type == typeof(CatFloat).ToString()) {
                newVariable = new CatFloat();
            }
            else if (_type == typeof(CatTexture).ToString()) {
                newVariable = new CatTexture();
            }
            else if (_type == typeof(CatVector4).ToString()) {
                newVariable = new CatVector4();
            }
            else if (_type == typeof(CatColor).ToString()) {
                newVariable = new CatColor();
            }
            else {
                Debug.Assert(false, "No matching type found. type=" + _type + " value=" + _value);
            }
            if (newVariable != null && _value != null && _value != "") {
                newVariable.FromString(_value);
            }
            
            return newVariable;
        }

        public bool SaveToNode(XmlNode node, XmlDocument doc, bool _isTip = false) {
            XmlElement material = doc.CreateElement("Material");
            material.SetAttribute("name", m_name);
            node.AppendChild(material);

            foreach (KeyValuePair<string, IEffectParameter> keyValue in m_parameters) {
                // if the parameter is masked and this is a tip material, do not store it
                if (_isTip && m_materialTemplate.IsParameterMaskedInEditor(keyValue.Key)) {
                    continue;
                }

                XmlElement eleParameter = doc.CreateElement("Parameter");
                eleParameter.SetAttribute("name", keyValue.Key);
                eleParameter.SetAttribute("type", keyValue.Value.GetType().ToString());
                eleParameter.SetAttribute("value", keyValue.Value.ToValueString());
                material.AppendChild(eleParameter);
            }
            return true;
        }

        /**
         * @brief Load parameters in _node and keep them in material
         * 
         * @param _node the xml node, <Material></Material>
         * */
        public void ApplyParameterTipsFromXml(XmlNode _node) {
            foreach (XmlNode node in _node.ChildNodes) {
                XmlElement eleNode = (XmlElement)node;
                string parameterName = eleNode.GetAttribute("name");
                // name match ?
                if (!m_parameters.ContainsKey(parameterName)) {
                    continue;
                }
                string parameterType = eleNode.GetAttribute("type");
                // type match ?
                if (parameterType != m_parameters[parameterName].GetType().ToString()) {
                    continue;
                }
                // apply
                string parameterValue = eleNode.GetAttribute("value");
                m_parameters[parameterName].FromString(parameterValue);
            }
        }

        public IEffectParameter GetParameter(string _name) {
            if (m_parameters.ContainsKey(_name)) {
                return m_parameters[_name];
            }
            return null;
        }

        // for editor
        public Dictionary<string, IEffectParameter> GetMaterialParameters() {
            return m_parameters;
        }
    }
}
