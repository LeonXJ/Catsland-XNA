using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace Catsland.Core {
    public class Serialable {
        /**
         * @file Serialable
         * 
         * The base class supporting automatic serialization/unserialization
         * 
         * @author LeonXie
         * */
#region Properties
        [SerialAttribute]
        protected string m_guid;                                  // guid for global reference
        public string GUID {
            set {
                m_guid = value;
            }
            get {
                return m_guid;
            }
        }
        private Dictionary<Pointer, string> m_delayBindingTable;  // for delay binding pointer -> wanted guid
        static private List<ISerializeType> iserializeType;       // the serialize/unserialize solver
        static private Dictionary<string, Serialable> guidTable;  // for delay binding guid -> serialable
#endregion

        /**
         * @brief initialize serialize/unserilize solver table
         * */
        public static void InitializeSerializeTypeTable() {
            iserializeType = new List<ISerializeType>();
            Assembly assembly = Assembly.GetAssembly(typeof(ISerializeType));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types) {
                if (type.GetInterface(typeof(ISerializeType).ToString()) != null) {
                    ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    ISerializeType iserializeObject = (ISerializeType)(constructor.Invoke(new object[0]));
                    iserializeType.Add(iserializeObject);
                }
            }
        }

        /**
         * @brief find suitable ISerialType for the type
         * 
         * @param _type the target type
         * 
         * @result if not find, return null
         * */
        public static ISerializeType FindSuitableSerialType(Type _type) {
            if (iserializeType != null) {
                foreach (ISerializeType iType in iserializeType) {
                    if (iType.IsThisType(_type)) {
                        return iType;
                    }
                }
            }
            return null;
        }

        /**
         * @brief enabling delay binding supporting
         * */
        static public void BeginSupportingDelayBinding() {
            if (guidTable == null) {
                guidTable = new Dictionary<string, Serialable>();
            }
        }

        /**
         * @brief do delay binding and clear binding table
         * 
         * @param _isClearGuidTable whether clear binding table
         * */
        static public void EndSupportingDelayBinding(bool _isClearGuidTable = true) {
            if (guidTable != null) {
                foreach (KeyValuePair<string, Serialable> keyValue in guidTable) {
                    // foreach serialable object, do its delay binding
                    keyValue.Value.DelayBinding();
                }
            }
            // clear guidTable
            if (_isClearGuidTable) {
                guidTable = null;
            }
        }
        
        protected Serialable() {
            m_guid = Guid.NewGuid().ToString();
        }

        /**
         * @brief do serialize by generating an XmlNode
         * 
         * @param _doc XmlDocument
         * 
         * @result the XmlNode
         * */
        public XmlNode DoSerial(XmlDocument _doc) {
            PreSerial();
            XmlNode node = Serial(_doc);
            PostSerial(ref node, _doc);
            return node;
        }

        /**
         * @brief unserilize a serialable class
         * 
         * @param _node the XmlNode
         * 
         * @result the Serialable object
         * */
        public static Serialable DoUnserial(XmlNode _node) {
            // instantiate
            string typeName = _node.Name;
            Type objectType = Type.GetType(typeName);
            if (objectType == null) {
                // check in type manager
                objectType = Mgr<TypeManager>.Singleton.GetCatComponentType(typeName);
            }
            Debug.Assert(objectType != null, "Cannot find type: " + typeName);
            Debug.Assert(objectType.IsSubclassOf(typeof(Serialable)), "Instantiating non-serialiable type: " + typeName);

            if (null != objectType) {
                // instance a object
                ConstructorInfo constructorInfo = objectType.GetConstructor(new Type[0]);
                Debug.Assert(constructorInfo != null, "Cannot find valid constructor for type: " + typeName);
                Serialable resObject = (Serialable)(constructorInfo.Invoke(new Object[0]));
                Debug.Assert(resObject != null, "Cannot instantiate object: " + typeName);

                resObject.m_delayBindingTable = new Dictionary<Pointer, string>();
                resObject.PreUnserial(ref _node);
                resObject.Unserial(_node);
                resObject.PostUnserial(_node);
                // add to guid table for delay binding request
                guidTable.Add(resObject.GUID, resObject);
                return resObject;
            }
            return null;
        }

        public Serialable DoClone(){
            Type type = GetThisType();
            ConstructorInfo constructorInfo = type.GetConstructor(new Type[0]);
            Debug.Assert(constructorInfo != null, "Cannot find valid constructor for type: " + type.ToString());
            Serialable resObject = (Serialable)(constructorInfo.Invoke(new Object[0]));
            Debug.Assert(resObject != null, "Cannot instantiate object: " + type.ToString());

            resObject.m_delayBindingTable = new Dictionary<Pointer, string>();
            resObject.PreClone(this);
            resObject.Clone(this);
            // don't clone guid
            resObject.m_guid = Guid.NewGuid().ToString();
            resObject.PostClone(this);
            // add to guid table for delay binding request
            guidTable.Add(resObject.GUID, resObject);

            return resObject;
        }

        /**
         * @brief overwrite this to return type of class
         * 
         * @result the type
         * */
        virtual public Type GetThisType() {
            return this.GetType();
        }

        /**
         * @brief overwrite to preform before serial
         * */
        virtual protected void PreSerial() { }

        /**
         * @brief overwrite to preform after serial
         * 
         * @param _node the automatically generated node
         * */
        virtual protected void PostSerial(ref XmlNode _node, XmlDocument _doc) { }

        /**
         * @brief overwrite to preform before unserialization
         * 
         * @param the XmlNode
         * */
        virtual protected void PreUnserial(ref XmlNode _node) { }

        /**
         * @brief overwrite to preform after unserialization
         * 
         * @param the XmlNode
         * */
        virtual protected void PostUnserial(XmlNode _node) { }

        /**
         * @brief overwrite to preform after delay binding
         * 
         * @param the XmlNode
         * */
        virtual public void PostDelayBinding() { }

        virtual protected void PreClone(Serialable _object){}
        virtual protected void PostClone(Serialable _object){}

        /**
         * @brief core part of automate serialization
         * 
         * @param _doc XmlDocument
         * 
         * @result 
         * */
        private XmlNode Serial(XmlDocument _doc) {
            // name of object
            Type type = GetThisType();
            XmlElement root = _doc.CreateElement(type.ToString());

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields) {
                // get attributes
                SerialAttribute[] attributes = (SerialAttribute[])(field.GetCustomAttributes(typeof(SerialAttribute), true));
                if (attributes.Length == 0) {
                    continue;
                }
                // get other
                string fieldName = field.Name;
                Type fieldType = field.FieldType;
                ISerializeType iserializer = FindSuitableSerialType(fieldType);
                if (iserializer != null) {
                    XmlNode node = iserializer.Serial(field.GetValue(this), attributes[0], _doc, fieldName);
                    if (node != null) {
                        root.AppendChild(node);
                    }
                }
            }
            return root;
        }

        /**
         * @brief core part of automate unserialization
         * 
         * @param _node the XmlNode
         * */
        private void Unserial(XmlNode _node) {
            Type objectType = GetThisType();
            // set property
            FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            // build xml tree map
            Dictionary<string, XmlNode> xmlNodeDictionary = new Dictionary<string, XmlNode>();
            foreach(XmlNode node in _node.ChildNodes){
                string name = ((XmlElement)(node)).GetAttribute("name");
                if (name != null) {
                    xmlNodeDictionary.Add(name, node);
                }
            }
            // scan field
            foreach (FieldInfo field in fields) {
                // get attributes
                SerialAttribute[] attributes = (SerialAttribute[])(field.GetCustomAttributes(typeof(SerialAttribute), true));
                if (attributes.Length == 0) {
                    continue;
                }
                // get other
                string fieldName = field.Name;
                Type fieldType = field.FieldType;
                // find fields in node
                if (!xmlNodeDictionary.ContainsKey(fieldName)) {
                    //Debug.Assert(false, "Cannot find field information in xml. Field name: " + fieldName);
                    continue;
                }
                XmlElement fieldNode = (XmlElement)(xmlNodeDictionary[fieldName]);
                string xmlType = fieldNode.Name;

                // find a suitable unserializor to deal with it
                ISerializeType iserializer = FindSuitableSerialType(fieldType);
                if (iserializer != null) {
                    Object resultObject = iserializer.Unserial(new Pointer(this, field), attributes[0], fieldNode, m_delayBindingTable);
                    if (resultObject != null) {
                        field.SetValue(this, resultObject);
                    }
                }
            }
        }

        /**
         * @brief core part of delay binding
         * */
        private void DelayBinding() {
            if (m_delayBindingTable == null) {
                return;
            }
            foreach (KeyValuePair<Pointer, string> keyValue in m_delayBindingTable) {
                Pointer field = keyValue.Key;
                string targetGuid = keyValue.Value;
                if (guidTable.ContainsKey(targetGuid)) {
                    Serialable target = guidTable[targetGuid];
                    field.SetValue(target);
                    //field.SetValue(this, target);
                }
            }
            PostDelayBinding();
            // clear
            m_delayBindingTable = null;
        }

        private void Clone(Serialable _object){
            Type objectType = GetThisType();
            // set property
            FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | 
                                                      BindingFlags.NonPublic | 
                                                      BindingFlags.Public);
            // scan field
            foreach (FieldInfo field in fields) {
                // get attributes
                SerialAttribute[] attributes = 
                    (SerialAttribute[])(field.GetCustomAttributes(typeof(SerialAttribute), true));
                if (attributes.Length == 0) {
                    continue;
                }
                // get other
                string fieldName = field.Name;
                Type fieldType = field.FieldType;

                // find a suitable unserializor to deal with it
                ISerializeType iserializer = FindSuitableSerialType(fieldType);
                if (iserializer != null) {
                    // the field may be null
                    Object fieldObject = field.GetValue(_object);
                    if (fieldObject != null) { 
                        Object resultObject = iserializer.Clone(
                            new Pointer(this, field), 
                            attributes[0], 
                            fieldObject, 
                            m_delayBindingTable);
                        if (resultObject != null) {
                            field.SetValue(this, resultObject);
                        }
                    }
                }
            }
        }
    }
}
