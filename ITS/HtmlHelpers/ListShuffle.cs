using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.HtmlHelpers
{
	public static class ListShuffle
	{
		private static Random rnd = new Random();

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, bool doShuffle = true)
		{
			if (doShuffle)
			{
				return source.OrderBy<T, int>(item => rnd.Next());
			}
			else
			{
				return source;
			}
		}
	}
}