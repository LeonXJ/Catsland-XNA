using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catsland.Core {
    [AttributeUsage(AttributeTargets.All)]
    public class SerialAttribute : Attribute {
        public enum AttributePolicy {
            PolicyNone,
            PolicyCopy,
            PolicyReference,
        };

        private AttributePolicy m_isReference = AttributePolicy.PolicyCopy;
        private AttributePolicy m_isCloneReference = AttributePolicy.PolicyCopy;
        public SerialAttribute(AttributePolicy _isReference = AttributePolicy.PolicyCopy, 
                               AttributePolicy _isCloneReference = AttributePolicy.PolicyCopy) {
            m_isReference = _isReference;
            m_isCloneReference = _isCloneReference;
        }
        public AttributePolicy GetIsReference() {
            return m_isReference;
        }
        public AttributePolicy GetIsCloneReference() {
            return m_isCloneReference;
        }
    }
}
