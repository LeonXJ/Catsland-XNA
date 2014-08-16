using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.ComponentModel;

/**
 * @brief the base class of plug-in of CatEngine
 *
 * A GameObject can hold multi CatComponents.
 * In every frame, the Engine invokes update function of every CatComponent
 *
 * @author LeonXie
 */

namespace Catsland.Core {
    /**
     * @brief the base class for plug-in
     * */
    public class CatComponent : Serialable{
        // whether this component is enable?
        // if enable, update() will be invoked
        // else, the data can be accessed but no update() invoked
        [SerialAttribute]
        protected bool m_enable = true;
        [CategoryAttribute("Basic")]
        public bool Enable {
            set { m_enable = value; }
            get { return m_enable; }
        }

        // Delay binding is not used here because the component's xml node is
        //  always within its gameObject's node. So we never store m_gameObject
        //  in the node. To set m_gameObject, we do that in PostUnserial of
        //  gameObject.
        protected GameObject m_gameObject;
        public GameObject GameObject {
            set {
                m_gameObject = value;
            }
            get {
                return m_gameObject;
            }
        }

        // [Called by unserial mechanism]
        public CatComponent() {
        }

        public CatComponent(GameObject gameObject) {
            m_gameObject = gameObject;
        }

        public override Type GetThisType() {
            return GetType();
        }

        /**
         * @brief bind the component to scene
         * 
         * it supports convert a component from on scene to another
         * 
         * @param scene the target scene
         * */
        public virtual void BindToScene(Scene scene) {}

        /**
         * @brief [Called by GameObject only] initialize the component
         * 
         * invoked by catsEngine after created before update
         * 
         * @param scene the scene it belongs to
         * 
         * */
        public virtual void Initialize(Scene scene) {
        }

        /**
         * @brief [Called by GameObject only] logic update function
         * 
         * invoked by catsEngine
         * 
         * @param timeLastFrame
         * */
        public virtual void Update(int timeLastFrame) {
        }

        /**
         * @brief [Called by GameObject only] 
         **/ 
        public virtual void EditorUpdate(int timeLastFrame) {

        }

        /**
         * @brief destroy and release the component
         * */
        public virtual void UnbindFromScene(Scene _scene) {

        }

        /**
         * ModelComponent only
         */
        public virtual CatModelInstance GetModel() {
            return null;
        }
    }
}
