using System;
using System.Linq;
using System.Collections.Generic;

namespace CSharp.TemplateEngine
{
	/// <summary>
	/// String-object dictionary. By default, it does not distinguish between uppercase and lowercase
	/// in the key and returns null when a key that does not exist is requested instead of throwing an exception.
	/// </summary>
	sealed class Map : Map<object>
    {
    }

	/// <summary>
	/// String-value dictionary. By default, it does not distinguish between uppercase and lowercase
	/// in the key and returns null when a key that does not exist is requested instead of throwing an exception.
	/// </summary>
	class Map<T> : Dictionary<string, T>
    {
		public Map() : base (StringComparer.OrdinalIgnoreCase)
        {
        }

		public new IEnumerable<string> Keys
		{
			get{ return base.Keys.Select(t => t); }
		}

		public KeyValuePair<string, T>[] DebugValue
		{
			get{ return this.ToArray(); }
		}

        public new T this[string key]
        {
            get
            {
                T value;

                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return default(T);
                }
            }
            set
            {
                if (this.ContainsKey(key))
                {
                    base[key] = value;
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }
		
		public static Map<T> Copy(IDictionary<string, T> items)
		{
			var map = new Map<T>();
			map.AddRange(items);
			return map;
		}

		public void AddRange (IDictionary<string, T> items)
		{
			if (items != null)
			{
				foreach (var item in items)
				{
					this.Add (item.Key, item.Value);
				}
			}
        }
    }
}
