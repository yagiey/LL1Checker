using System.Collections.Generic;

namespace LL1Checker.Extensions.Generic
{
	internal static class HashSetExtension
	{
		public static bool Add<T>(this HashSet<T> hashSet, IEnumerable<T> items)
		{
			bool result = true;
			foreach (var item in items)
			{
				if (!hashSet.Add(item))
				{
					result = false;
				}
			}
			return result;
		}
	}
}
