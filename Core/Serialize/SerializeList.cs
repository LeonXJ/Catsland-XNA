using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.Diagnostics;

namespace Catsland.Core {
    public class SerializeList : ISerializeType{
        public bool IsThisType(Type _type) {
            return (_type.GetInterface(typeof(IList).ToString()) != null);
        }

        public System.Xml.XmlNode Serial(object _object, SerialAttribute _attribute, System.Xml.XmlDocument _doc, string _nameField) {
            if (_object != null) {
                IList list = (IList)(_object);
                Type valueType = _object.GetType().GetGenericArguments()[0];
                ISerializeType valueIType = Serialable.FindSuitableSerialType(valueType);

                XmlElement root = _doc.CreateElement(typeof(IList).ToString());
                root.SetAttribute("name", _nameField);
                root.SetAttribute("typeinfo", _object.GetType().ToString());

                IEnumerator it = list.GetEnumerator();
                while (it.MoveNext()) {
//                     XmlNode valueNodeHead = _doc.CreateElement("Value");
//                     root.AppendChild(valueNodeHead);
                    XmlNode valueNode = valueIType.Serial(it.Current, _attribute, _doc, "");
                    root.AppendChild(valueNode);
                }
                return root;
            }
            else {
                return null;
            }
        }

        public object Unserial(Pointer _pointer, SerialAttribute _attribute, System.Xml.XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable) {
            // get list
            IList list = (IList)(_pointer.GetValue());
            if (list == null) {
                Type fieldType = Type.GetType(((XmlElement)_fieldNode).GetAttribute("typeinfo"));
                ConstructorInfo listConstructor = fieldType.GetConstructor(new Type[0]);
                Debug.Assert(listConstructor != null, "Cannot find valid constructor for list");
                list = (IList)(listConstructor.Invoke(new object[0]));
            }
            // get value type
            Type valueType = list.GetType().GetGenericArguments()[0];
            ISerializeType valueIType = Serialable.FindSuitableSerialType(valueType);

            int index = 0;
            foreach (XmlNode valueNode in _fieldNode.ChildNodes) {
                //XmlNode valueContentNode = valueNode.FirstChild;
                object valueObject = valueIType.Unserial(new Pointer(list, index), _attribute, valueNode, _delayBindingTable);
                if (valueObject != null) {
                    list.Add(valueObject);
                }
                ++index;
            }
            return list;
        }
        

        public object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable) {
            // get list
            IList list = (IList)(_pointer.GetValue());
            if (list == null) {
                Type fieldType = _original.GetType();
                ConstructorInfo listConstructor = fieldType.GetConstructor(new Type[0]);
                Debug.Assert(listConstructor != null, "Connot find valid constructor for list");
                list = (IList)(listConstructor.Invoke(new object[0]));
            }
            // get value type
            Type valueType = list.GetType().GetGenericArguments()[0];
            ISerializeType valueIType = Serialable.FindSuitableSerialType(valueType); 
            IList originalList = (IList)_original;

            IEnumerator it = originalList.GetEnumerator();
            int index = 0;
            while (it.MoveNext()) {
                object valueObject = valueIType.Clone(new Pointer(list, index), _attribute, it.Current, _delayBindingTable);
                if (valueObject != null) {
                    list.Add(valueObject);
                }
                ++index;
            }
            return list;
        }
    }
}
