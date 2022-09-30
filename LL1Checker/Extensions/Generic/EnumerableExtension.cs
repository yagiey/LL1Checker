using System.Collections.Generic;
using System.Linq;

namespace LL1Checker.Extensions.Generic
{
	internal static class EnumerableExtension
	{
		public static bool Equal<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
		{
			return
				lhs.All(it => rhs.Contains(it)) &&
				rhs.All(it => lhs.Contains(it));
		}
	}
}
