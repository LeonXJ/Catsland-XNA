using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * @file UniqueList maps a name to an object
 *
 * @author LeonXie
 */

namespace Catsland.Core {
    /**
     * @brief UniqueList maps a name to an object
     */

    public class UniqueList<typename> {
        public Dictionary<String, typename> contentList;

        /**
         * @brief add an item to the list
         *
         * @param key
         * @param value
         */
        public virtual void AddItem(String key, typename value) {
            if (contentList == null) {
                contentList = new Dictionary<String, typename>();
            }
            if (!contentList.ContainsKey(key)) {
                contentList.Add(key, value);
            }
#if DEBUG
            else {
                Console.WriteLine("Warning! Repeat insert in UniqueList. Key: " + key);
            }
#endif
        }

        /**
         * @brief return an item by key
         *
         * @param key
         */
        public virtual typename GetItem(String key) {
            if (contentList == null || !contentList.ContainsKey(key)) {
                return default(typename);
            }
            return contentList[key];
        }

        /**
         * @brief return whether the list contain the key
         *
         * @param key
         * @return contain key?
         */
        public virtual bool ContainKey(string key) {
            if (contentList == null) {
                return false;
            }
            if (contentList.ContainsKey(key)) {
                return true;
            }
            return false;
        }

        /**
         * @brief clear the list
         */
        public virtual void ReleaseAll() {
            if (contentList != null) {
                contentList.Clear();
            }
        }

        /**
         * @brief remove the key if exists
         */
        public virtual void RemoveItem(string key) {
            if (contentList != null && contentList.ContainsKey(key)) {
                contentList.Remove(key);
            }
        }

        public Dictionary<String, typename> GetList() {
            return contentList;
        }
    }
}
