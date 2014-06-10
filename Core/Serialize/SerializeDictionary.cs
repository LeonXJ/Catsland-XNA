using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Xml;

namespace Catsland.Core {
    public class SerializeDictionary : ISerializeType {
        public bool IsThisType(Type _type) {
            return (_type.GetInterface(typeof(IDictionary).ToString()) != null);
        }

        public XmlNode Serial(Object _object, SerialAttribute _attribute, XmlDocument _doc, string _nameField) {
            if (_object != null) {
                // get value <key, value>
                IDictionary dictionary = (IDictionary)(_object);
                Type[] keyValueType = _object.GetType().GetGenericArguments();
                ISerializeType keyIType = Serialable.FindSuitableSerialType(keyValueType[0]);
                ISerializeType valueIType = Serialable.FindSuitableSerialType(keyValueType[1]);

                XmlElement root = _doc.CreateElement(typeof(IDictionary).Name);
                root.SetAttribute("name", _nameField);
                root.SetAttribute("typeinfo", _object.GetType().ToString());

                IDictionaryEnumerator it = dictionary.GetEnumerator();
                while (it.MoveNext()) {
                    XmlNode keyValueNode = _doc.CreateElement("KeyValue");
                    root.AppendChild(keyValueNode);
                    // key
                    XmlNode keyNodeHead = _doc.CreateElement("Key");
                    keyValueNode.AppendChild(keyNodeHead);
                    XmlNode keyNode = keyIType.Serial(it.Key, _attribute,_doc, "");
                    keyNodeHead.AppendChild(keyNode);
                    // value
                    XmlNode valueNodeHead = _doc.CreateElement("Value");
                    keyValueNode.AppendChild(valueNodeHead);
                    XmlNode valueNode = valueIType.Serial(it.Value, _attribute, _doc, "");
                    valueNodeHead.AppendChild(valueNode);
                }
                return root;
            }
            else {
                return null;
            }
        }

        public object Unserial(Pointer _pointer, SerialAttribute _attribute, XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable) {
            // get dictionary
            IDictionary dictionary = (IDictionary)(_pointer.GetValue());
            if (dictionary == null) {
                Type fieldType = Type.GetType(((XmlElement)_fieldNode).GetAttribute("typeinfo"));
                ConstructorInfo dictionaryConstructor = fieldType.GetConstructor(new Type[0]);
                Debug.Assert(dictionaryConstructor != null, "Cannot find valid constructor for dictionary");
                dictionary = (IDictionary)(dictionaryConstructor.Invoke(new object[0]));
            }
            // get key value type
            Type[] keyValueType = dictionary.GetType().GetGenericArguments();
            ISerializeType keyIType = Serialable.FindSuitableSerialType(keyValueType[0]);
            ISerializeType valueIType = Serialable.FindSuitableSerialType(keyValueType[1]);

            foreach (XmlNode keyValueNode in _fieldNode.ChildNodes) {
                // foreach keyValue
                // key
                XmlNode keyNode = keyValueNode.SelectSingleNode("Key");
                XmlNode keyContentNode = keyNode.FirstChild;
                // do not support delay binding for key
                object keyObject = keyIType.Unserial(null, _attribute, keyContentNode, _delayBindingTable);
                // value
                XmlNode valueNode = keyValueNode.SelectSingleNode("Value");
                XmlNode valueContentNode = valueNode.FirstChild;
                object valueObject = valueIType.Unserial(new Pointer(dictionary, keyObject), _attribute, valueContentNode, _delayBindingTable);
                // insert into dictionary
                if (valueObject != null) {
                    dictionary.Add(keyObject, valueObject);
                }
            }
            return dictionary;
        }

        public Object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable) {
            // get dictionary
            IDictionary dictionary = (IDictionary)(_pointer.GetValue());
            if (dictionary == null) {
                Type fieldType = _original.GetType();
                ConstructorInfo dictionaryConstructor = fieldType.GetConstructor(new Type[0]);
                Debug.Assert(dictionaryConstructor != null, "Cannot find valid constructor for dictionary");
                dictionary = (IDictionary)(dictionaryConstructor.Invoke(new object[0]));
            }
            // get key value type
            Type[] keyValueType = dictionary.GetType().GetGenericArguments();
            ISerializeType keyIType = Serialable.FindSuitableSerialType(keyValueType[0]);
            ISerializeType valueIType = Serialable.FindSuitableSerialType(keyValueType[1]);
            IDictionary orignialDictionary = (IDictionary)_original;

            IDictionaryEnumerator it = orignialDictionary.GetEnumerator();
            while (it.MoveNext()){
                object keyObject = it.Key;
                object valueObject = valueIType.Clone(new Pointer(dictionary, keyObject), _attribute, it.Value, _delayBindingTable);
                if (valueObject != null) {
                    dictionary.Add(keyObject, valueObject);
                }
            }
            return dictionary;
        }
    }
}
