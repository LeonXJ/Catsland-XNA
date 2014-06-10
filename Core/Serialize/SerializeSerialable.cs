using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Catsland.Core {
    public class SerializeSerialable : ISerializeType {
        public bool IsThisType(Type _type) {
            return (_type.IsSubclassOf(typeof(Serialable)));
        }

        public XmlNode Serial(Object _object, SerialAttribute _attribute, XmlDocument _doc, string _nameField) {
            if (_object != null) {
                XmlElement root = null;
                Serialable serialable = (Serialable)(_object);
                if (serialable == null) {
                    return null;
                }
                if (_attribute.GetIsReference() == 
                    SerialAttribute.AttributePolicy.PolicyReference) {
                    
                    root = _doc.CreateElement(((Serialable)(_object)).GetThisType().Name);
                    root.SetAttribute("value", serialable.GUID);
                }
                else if (_attribute.GetIsReference() ==
                    SerialAttribute.AttributePolicy.PolicyCopy) {

                    root = (XmlElement)serialable.DoSerial(_doc);
                }
                root.SetAttribute("name", _nameField);
                return root;
            }
            else {
                return null;
            }
        }

        public Object Unserial(Pointer _pointer, SerialAttribute _attribute, XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable) {
            if (_attribute.GetIsReference() == 
                    SerialAttribute.AttributePolicy.PolicyReference) {
                _delayBindingTable.Add(_pointer, ((XmlElement)_fieldNode).GetAttribute("value"));
                return null;
            }
            else if(_attribute.GetIsReference() == 
                SerialAttribute.AttributePolicy.PolicyCopy){

                Serialable obj = Serialable.DoUnserial(_fieldNode);
                return obj;
            }
            return null;
        }

        public Object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable) {
            if (_attribute.GetIsCloneReference() == 
                SerialAttribute.AttributePolicy.PolicyReference) {
                
                _delayBindingTable.Add(_pointer, ((Serialable)_original).GUID);
                return null;
            }
            else if(_attribute.GetIsCloneReference() ==
                SerialAttribute.AttributePolicy.PolicyCopy) {

                Serialable obj = ((Serialable)_original).DoClone();
                return obj;
            }
            return null;
        }
    }
}
