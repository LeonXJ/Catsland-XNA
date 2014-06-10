using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Catsland.Core {
    /**
     * @file ISerializeType
     * 
     * The interface of SerializeType, which defines how to serialize/unserilize a type
     * 
     * @author LeonXie
     * */
    public interface ISerializeType {
        /**
         * @brief Answers whether this object can unserialize and serialize the type
         * 
         * @param _type the type
         * 
         * @result if this class can serialize/unserialize _type
         * */
        bool IsThisType(Type _type);

        /**
         * @brief Serial _object into XmlNode
         * 
         * The name of root node should be typeof(_object).toString(). However, if it contain illegal character,
         *      name it in attribute typeinfo
         * Only Serilable class support reference, for it contain guid
         * 
         * @param _object the object whose type get confirmed by IsThisType
         * @param _attribute contain tips about the serialization, e.g. if it is a reference
         * @param _doc you should create XmlElement/XmlNode with _doc.Create...
         * @param _nameField the root Node should contain attribute name="_nameField"
         * 
         * @result the XmlNode
         * */
        XmlNode Serial(Object _object, SerialAttribute _attribute, XmlDocument _doc, string _nameField);

        /**
         * @brief Unserialize the XmlNode
         * 
         * @param _pointer the _object in the view of caller
         * @param _attribute contain tips about the serialization, e.g. if it is a reference
         * @param _fieldNode is the XmlNode
         * @param _delayBindingTable is the caller's delay binding table. If _attribute tells this is a 
         *          reference, we should push guid into _delayBindingTable
         * 
         * @result the object.
         * @cautious if it is not necessary to create a new object, return null. e.g. for read only 
         *          IEffectParameter, set its value via _pointer, instead of return a new one
         * */
        object Unserial(Pointer _pointer, SerialAttribute _attribute, XmlNode _fieldNode, Dictionary<Pointer, string> _delayBindingTable);

        object Clone(Pointer _pointer, SerialAttribute _attribute, object _original, Dictionary<Pointer, string> _delayBindingTable);
    }
}
