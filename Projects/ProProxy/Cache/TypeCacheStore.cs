//using System;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace ProProxy
//{
//    public class TypeCacheStore : IDictionary<Type, ActionCache<>>
//    {
//        private TypeCacheStore()
//        {
//
//        }
//
//        public static TypeCacheStore Instance { get; set; } = new TypeCacheStore();
//
//        private static Dictionary<Type, ActionCache<>> Cache { get; set; } = new Dictionary<Type, ActionCache<>>();
//
//        public static ActionCache<> GetCache(Type type)
//        {
//            if (Cache.ContainsKey(type))
//            {
//                return Cache[type];
//            }
//
//            var product = new ActionCache<>();
//            Cache.Add(type, product);
//            return Cache[type];
//        }
//
//        public IEnumerator<KeyValuePair<Type, ActionCache<>>> GetEnumerator()
//        {
//            return Cache.GetEnumerator();
//        }
//
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return Cache.GetEnumerator();
//        }
//
//        public void Add(KeyValuePair<Type, ActionCache<>> item)
//        {
//            Cache.Add(item.Key, item.Value);
//        }
//
//        public void Clear()
//        {
//            Cache.Clear();
//        }
//
//        public bool Contains(KeyValuePair<Type, ActionCache<>> item)
//        {
//            return Cache.ContainsKey(item.Key) && Cache.ContainsValue(item.Value);
//        }
//
//        public void CopyTo(KeyValuePair<Type, ActionCache<>>[] array, int arrayIndex)
//        {
//            throw new NotSupportedException("This singleton cache should not be copied.");
//        }
//
//        public bool Remove(KeyValuePair<Type, ActionCache<>> item)
//        {
//            return Cache.Remove(item.Key);
//        }
//
//        public int Count => Cache.Count;
//
//        public bool IsReadOnly => false;
//
//        public bool ContainsKey(Type key)
//        {
//            return Cache.ContainsKey(key);
//        }
//
//        public void Add(Type key, ActionCache<> value)
//        {
//            Cache.Add(key, value);
//        }
//
//        public bool Remove(Type key)
//        {
//            return Cache.Remove(key);
//        }
//
//        public bool TryGetValue(Type key, out ActionCache<> value)
//        {
//            return Cache.TryGetValue(key, out value);
//        }
//
//        public ActionCache<> this[Type key]
//        {
//            get => Cache[key];
//            set => Cache[key] = value;
//        }
//
//        public ICollection<Type> Keys => Cache.Keys;
//
//        public ICollection<ActionCache<>> Values => Cache.Values;
//    }
//}