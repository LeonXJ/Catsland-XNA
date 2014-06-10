using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Catsland.Core{
    public class SerializeString : ISerializeType{
        public bool IsThisType(Type _type) {
            return (_type == typeof(string));
        }
        public XmlNode Serial(Object _object, SerialAttribute _attribute, XmlDocument _doc, string _nameField) {
            if (_object != null) {
                XmlElement root = _doc.CreateElement(typeof(string).Name);
                root.SetAttribute("name", _nameField);
                root.SetAttribute("value", (string)(_object));
                return root;
            }
            else {
                return null;
            }
        }
        public Object Unserial(Pointer _pointer, SerialAttribute _attribute, XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable) {
            return ((XmlElement)_fieldNode).GetAttribute("value");
        }
        public Object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable) {
            string res;
            res = (string)_original;
            return res;
        }
    }
}
