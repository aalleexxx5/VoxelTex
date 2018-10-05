using System.Collections.Generic;

namespace tex
{
    public class MultiDictionary<K, V> : Dictionary<K, LinkedList<V>>
    {
        
        public void Add(K key, V value)
        {
            LinkedList<V> values;
            if (!this.TryGetValue(key, out values))
            {
                values = new LinkedList<V>();
                this.Add(key,values);
            }
            values.AddLast(value);
        }
    }
}