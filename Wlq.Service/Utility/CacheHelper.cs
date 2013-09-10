using System;
using System.Collections.Generic;

namespace Wlq.Service.Utility
{
	public class CacheHelper<T>
	{
		private static Dictionary<string, CacheItem> _cache;
		private static object _locker;

		private CacheHelper() { }

		static CacheHelper()
		{
			_cache = new Dictionary<string, CacheItem>();
			_locker = new object();
		}

		public static T Get(string key)
		{
			CacheItem item = null;

			if (_cache.TryGetValue(key, out item))
			{
				if (item.Expired > DateTime.Now)
				{
					return item.Value;
				}
			}

			return default(T);
		}

		public static void Set(string key, T value, TimeSpan cacheTime)
		{
			var item = new CacheItem 
			{ 
				Value = value, 
				Expired = DateTime.Now.Add(cacheTime) 
			};

			lock (_locker)
			{
				if (_cache.ContainsKey(key))
				{
					_cache[key] = item;
				}
				else
				{
					_cache.Add(key, item);
				}
			}
		}

		public static void RemoveKey(string key)
		{
			lock (_locker)
			{
				if (_cache.ContainsKey(key))
				{
					_cache.Remove(key);
				}
			}
		}

		internal class CacheItem
		{
			public T Value { get; set; }
			public DateTime Expired { get; set; }
		}
	}
}
