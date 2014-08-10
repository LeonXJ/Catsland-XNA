using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/**
 * @file CatModelInstance the instance of CatModel
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    public class CatModelInstance {
        private CatModel m_catModel;
        private CatMaterial m_materialParameterTips;

        /**
         * @brief Create an instance from model
         * 
         * @param _catModel prototype model
         * 
         * @result the instance
         * */
        public static CatModelInstance CreateFromCatsModel(CatModel _catsModel) {
            if (_catsModel == null) {
                return null;
            }
            CatModelInstance newCatModelInstance = new CatModelInstance();
            newCatModelInstance.m_catModel = _catsModel;
            newCatModelInstance.m_materialParameterTips = _catsModel.GetMaterial().Clone();

            return newCatModelInstance;
        }

        public CatModel GetModel() {
            return m_catModel;
        }

        /**
         * @brief Get the material
         *      in instance level, material only holds parameter tips, which means its materialTemplate
         *      is on in effect.
         * 
         * @result the material tips
         * */
        public CatMaterial GetMaterial() {
            // check whether the materialTemplate of model has changed
            if (m_materialParameterTips.GetMaterialTemplate() != m_catModel.GetMaterial().GetMaterialTemplate()) {
                // the material template has changed, change the material
                m_materialParameterTips.ChangeMaterialTemplate(m_catModel.GetMaterial().GetMaterialTemplate());
            }
            return m_materialParameterTips;
        }

        public CatModelInstance Clone() {
            CatModelInstance newCatModelInstance = new CatModelInstance();
            newCatModelInstance.m_catModel = m_catModel;
            newCatModelInstance.m_materialParameterTips = m_materialParameterTips.Clone();

            return newCatModelInstance;
        }

        public Animation GetAnimation() {
            return m_catModel.GetAnimation();
        }
    }
}
