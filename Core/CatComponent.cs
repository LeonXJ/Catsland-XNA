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
        public GameObject m_gameObject;

        public CatComponent() {
        }

        public CatComponent(GameObject gameObject) {
            m_gameObject = gameObject;
        }

        public override Type GetThisType() {
            return GetType();
        }

        /**
         * @brief create a CatComponent from an XML node
         *
         * @param node the XML node
         * @param scene the scene the component bind to
         * @param gameObject the gameObject the component bind to
         * @return a new CatComponent
         */
        public static CatComponent LoadFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            string component_type = node.Name;
            Type type = Mgr<TypeManager>.Singleton.GetCatComponentType(component_type);
            if (type != null) {
                ConstructorInfo constructorInfo = type.GetConstructor(new Type[1] { typeof(GameObject) });
                CatComponent component = (CatComponent)constructorInfo.Invoke(new Object[1] { gameObject });
                // configure the component, the function will be inherited
                component.ConfigureFromNode(node, scene, gameObject);
                return component;
            }
            return null;
        }

        /**
         * @brief initialize the component
         * 
         * invoked by catsEngine after created before update
         * 
         * @param scene the scene it belongs to
         * 
         * */
        public virtual void Initialize(Scene scene) {
        }

        /**
         * @brief logic update function
         * 
         * invoked by catsEngine
         * 
         * @param timeLastFrame
         * */
        public virtual void Update(int timeLastFrame) {
        }

        public virtual void EditorUpdate(int timeLastFrame) {

        }

        /**
         * @brief bind the component to scene
         * 
         * it supports convert a component from on scene to another
         * 
         * @param scene the target scene
         * */
        public virtual void BindToScene(Scene scene) {

        }

        /**
         * @brief deep clone the component
         * 
         * @param gameObject the new gameObject the new component attaches to
         * 
         * @result
         * */
        public virtual CatComponent CloneComponent(GameObject gameObject) {
            return new CatComponent(gameObject);
        }

        /**
         * @brief save the component to XML node
         * 
         * @param node
         * @param doc
         * 
         * @result
         * */
        public virtual bool SaveToNode(XmlNode node, XmlDocument doc) {
            return true;
        }

        /**
         * @brief configure the node according to XML node
         * 
         * @param node
         * @param scene
         * @param gameObject
         * */
        public virtual void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {

        }

        /**
         * @brief destroy and release the component
         * */
        public virtual void Destroy() {

        }

        /**
         * ModelComponent only
         */
        public virtual CatModelInstance GetModel() {
            return null;
        }


        /**
         * @brief called after first pass of loading
         * */
        public virtual void PostConfiguration(Scene scene, Dictionary<string, GameObject> tempList) {
        }

        public virtual void EnterTrigger(Collider trigger, Collider invoker){
        }

        public virtual void InTrigger(Collider trigger, Collider invoker) {
        }

        public virtual void ExitTrigger(Collider trigger, Collider invoker) {
        }
    }
}
