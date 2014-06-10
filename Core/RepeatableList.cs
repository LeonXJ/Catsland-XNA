using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @file RepeatableList store a list of objects
 *
 * @author LeonXie
 */
namespace Catsland.Core {
    public class RepeatableList<typename> {
        /**
         * @brief RepeatableList store a list of objects
         */
        protected List<typename> contentList;

        public List<typename> GetContentList() {
            return contentList;
        }
        
        public virtual void AddItem(typename item) {
            if (contentList == null) {
                contentList = new List<typename>();
            }
            contentList.Add(item);
        }

        public virtual void RemoveItem(typename item) {
            contentList.Remove(item);
        }

        public virtual void ReleaseAll() {
            if (contentList != null) {
                contentList.Clear();
            }
        }

        public List<typename> GetList() {
            return contentList;
        }
    }
}
