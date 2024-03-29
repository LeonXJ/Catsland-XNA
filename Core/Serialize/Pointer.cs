﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Catsland.Core {
    public class Pointer {
        /**
         * @file Pointer
         * 
         * The general pointer to variables
         * 
         * @author LeonXie
         * */
        #region members
        private FieldInfo m_fieldInfo;
        private Object m_fieldObject;
        private IDictionary m_dictionary;
        private Object m_dictionaryKey;
        private IEffectParameter m_ieffectParameter;
        private IList m_list;
        private int m_listIndex;

        private enum ContentType {
            ContentField,
            ContentIDictionaryEnumerator,
            ContentIEffectParameter,
            ContentIListEnumerator,
        };
        private ContentType m_contentType;
#endregion

        /**
         * @brief point to a field of class
         * 
         * @param _object
         * @param _fieldInfo
         * */
        public Pointer(Object _object, FieldInfo _fieldInfo) {
            m_fieldObject = _object;
            m_fieldInfo = _fieldInfo;
            m_contentType = ContentType.ContentField;
        }
         /**
         * @brief point to a value of dictionary
         * 
         * @param _dictionary
         * @param _key
         * */
        public Pointer(IDictionary _dictionary, Object _key) {
            m_dictionary = _dictionary;
            m_dictionaryKey = _key;
            m_contentType = ContentType.ContentIDictionaryEnumerator;
        }

        /**
         * @brief point to an IEffectParameter
         * 
         * @param _ieffectParameter
         * */
        public Pointer(IEffectParameter _ieffectParameter) {
            m_ieffectParameter = _ieffectParameter;
            m_contentType = ContentType.ContentIEffectParameter;
        }

        /**
         * @brief point to a value of list
         * 
         * @param _list
         * @param _index
         * */
        public Pointer(IList _list, int _index) {
            m_list = _list;
            m_listIndex = _index;
            m_contentType = ContentType.ContentIListEnumerator;
        }

        /**
         * @brief set *pointer
         * have not check type here
         * 
         * @param _value 
         * */
        public void SetValue(object _value) {
            if (m_contentType == ContentType.ContentField) {
                m_fieldInfo.SetValue(m_fieldObject, _value);
            }
            else if (m_contentType == ContentType.ContentIDictionaryEnumerator) {
                m_dictionary[m_dictionaryKey] = _value;
            }
            else if (m_contentType == ContentType.ContentIEffectParameter) {
                m_ieffectParameter.FromString((string)(_value));
            }
            else if (m_contentType == ContentType.ContentIListEnumerator) {
                while (m_list.Count <= m_listIndex) {
                    m_list.Add(_value);
                }
                m_list[m_listIndex] = _value;
            }
        }

        /**
         * @brief return *pointer
         * 
         * @result *point
         * */
        public Object GetValue() {
            if (m_contentType == ContentType.ContentField) {
                return m_fieldInfo.GetValue(m_fieldObject);
            }
            else if (m_contentType == ContentType.ContentIDictionaryEnumerator) {
                return m_dictionary[m_dictionaryKey];
            }
            else if (m_contentType == ContentType.ContentIEffectParameter) {
                return m_ieffectParameter;
            }
            else if (m_contentType == ContentType.ContentIListEnumerator) {
                return m_list[m_listIndex];
            }
            return null;
        }

    }
}
