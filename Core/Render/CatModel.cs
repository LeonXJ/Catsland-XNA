using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/**
 * @file CatsModel holds Material and Animation
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief CatModel holds Material and Animation
     *        CatModel is the template of ModelInstance
     * */
    public class CatModel : Serialable{
        [SerialAttribute]
        private string m_name;
        private CatMaterial m_material;
        [SerialAttribute]
        private Animation m_animation;

        public CatModel() {
            m_name = "UntitledModel";
            m_material = null;
            m_animation = null;
        }

        public CatModel(string _name, CatMaterial _material, Animation _animation = null) {
            m_name = _name;
            m_material = _material;
            m_animation = _animation;
            if (m_animation == null) {
                m_animation = new Animation();
            }
        }

        protected override void PostUnserial(XmlNode _node) {
            XmlNode materialNode = _node.SelectSingleNode("Material");
            m_material = Mgr<CatProject>.Singleton.materialList1
                .CreateMaterialInstanceByXml(materialNode);
        }

        protected override void PostSerial(ref XmlNode _node, XmlDocument _doc) {
            m_material.SaveToNode(_node, _doc);            
        }

        public string GetName() {
            return m_name;
        }

        public void SetName(string _name) {
            m_name = _name;
        }

        public CatMaterial GetMaterial() {
            return m_material;
        }

        public void SetMaterialToTemplate(CatMaterialTemplate _materialTemplate) {
            if (m_material == null) {
                m_material = _materialTemplate.GetMaterialPrototype().Clone();
            }
            else {
                m_material.ChangeMaterialTemplate(_materialTemplate);
            }
        }

        public Animation GetAnimation() {
            return m_animation;
        }

        public void SetAnimation(Animation _animation) {
            m_animation = _animation;
        }
    }
}
