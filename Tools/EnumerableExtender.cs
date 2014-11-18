using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace Tools {

	public static class EnumerableExtender {

		public static string Print(this IEnumerable source) {
			StringBuilder builder = new StringBuilder();
			foreach (object item in source) {
				builder.Append(item == null ? "(null)" : item.ToString());
				builder.Append(", ");
			}
			if(builder.Length > 2) builder.Remove(builder.Length - 2, 2);
			return (builder.ToString());
		}

		public static T Get<T>(this IEnumerable<T> source, Predicate<T> condition) {
			List<T> matches = source.Where(condition.Invoke).ToList();
			if(!matches.Any()) throw new Exception("No items match the condition");
			if(matches.Count > 1) throw new Exception("More than one item matched the condition");
			return (matches[0]);
		}

	}

}
