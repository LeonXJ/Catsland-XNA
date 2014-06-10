using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Catsland.Core {
    public class SerializeIEffectParameter : ISerializeType {
        public bool IsThisType(Type _type) {
            return (_type.GetInterface(typeof(IEffectParameter).ToString()) != null);
        }

        public XmlNode Serial(Object _object, SerialAttribute _attribute, XmlDocument _doc, string _nameField) {
            if (_object != null) {
                XmlElement root = _doc.CreateElement(_object.GetType().Name);
                root.SetAttribute("name", _nameField);
                root.SetAttribute("value", ((IEffectParameter)(_object)).ToValueString());
                return root;
            }
            else {
                return null;
            }
        }

        public object Unserial(Pointer _pointer, SerialAttribute _attribute, XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable) {
            ((IEffectParameter)_pointer.GetValue()).FromString(((XmlElement)_fieldNode).GetAttribute("value"));
            return null;
        }

        public Object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable) {
            ((IEffectParameter)_pointer.GetValue()).FromString(((IEffectParameter)_original).ToValueString());
            return null;
        }
    }
}
