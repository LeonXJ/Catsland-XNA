using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;
using Catsland.Editor;

namespace Catsland.Plugin.BasicPlugin
{
    public class ModelComponent : CatComponent
    {
        CatModel m_model;
        [CategoryAttribute("Basic"), 
            EditorAttribute(typeof(PropertyGridModelSelector), 
            typeof(System.Drawing.Design.UITypeEditor))]
        public CatModel Model
        {
            set
            {
                m_model = value;
                m_catModelInstance = CatModelInstance.CreateFromCatsModel(m_model);
            }
            get {
                //return m_model;
                if (m_catModelInstance != null) {
                    return m_catModelInstance.GetModel();
                }
                return null;
            }
        }

        public ModelComponent() { }

        public ModelComponent(GameObject gameObject) 
            : base(gameObject)
        {
        }

        public override CatComponent CloneComponent(GameObject gameObject)
        {
            ModelComponent modelComponent = new ModelComponent(gameObject);
            modelComponent.Model = Model;

            // new material model
            modelComponent.m_catModelInstance = m_catModelInstance.Clone();
            return modelComponent;
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc)
        {
            XmlElement modelComponent = doc.CreateElement(typeof(ModelComponent).Name);
            node.AppendChild(modelComponent);

            modelComponent.SetAttribute("name", Model.GetName());
            // save parameter tips
            m_catModelInstance.GetMaterial().SaveToNode(modelComponent, doc, true);

            return true;
        }

        protected override void PostSerial(ref XmlNode _node, XmlDocument _doc) {
            // <Post_ModelName value="Cat" />
            XmlElement eleModelName = _doc.CreateElement("Post_ModelName");
            _node.AppendChild(eleModelName);
            eleModelName.SetAttribute("value", m_model.GetName());
            // <Material>
            m_catModelInstance.GetMaterial().SaveToNode(_node, _doc, true);
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject)
        {
            base.ConfigureFromNode(node, scene, gameObject);
            Model = Mgr<CatProject>.Singleton.modelList1.GetModel(node.GetAttribute("name"));
            // new material and model
            m_catModelInstance = CatModelInstance.CreateFromCatsModel(Model);
            // apply material tip
            XmlNode nodeMaterial = node.SelectSingleNode("Material");
            if (nodeMaterial != null) {
                m_catModelInstance.GetMaterial().ApplyParameterTipsFromXml(nodeMaterial);
            }
        }

        protected override void PostUnserial(XmlNode _node) {
            XmlElement eleModelName = 
                (XmlElement)_node.SelectSingleNode("Post_ModelName");
            if (eleModelName != null) {
                Model = Mgr<CatProject>.Singleton.modelList1.GetModel(
                                eleModelName.GetAttribute("value"));
            }
            // apply material tip
            XmlNode nodeMaterial = _node.SelectSingleNode("Material");
            if (nodeMaterial != null) {
                m_catModelInstance.GetMaterial().
                    ApplyParameterTipsFromXml(nodeMaterial);
            }

        }

        protected override void PostClone(Serialable _object) {
            ModelComponent target = _object as ModelComponent;
            m_model = target.Model;
            m_catModelInstance = target.m_catModelInstance.Clone();
        }

        public static string GetMenuNames() {
            return "Render|Model Component";
        }

        // new model
        public CatModelInstance GetCatModelInstance() {
            return m_catModelInstance;
        }

        public override CatModelInstance GetModel() {
            return m_catModelInstance;
        }

        private CatModelInstance m_catModelInstance;

        // tool
        public static ModelComponent GetModelOfGameObjectInCurrentScene(string _name) {
            GameObject gameObject = 
                Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(_name);
            if (gameObject != null) {
                return gameObject.GetComponent(typeof(ModelComponent).ToString())
                    as ModelComponent;
            }
            return null;
        }
    }
}
