using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CogniteSdk.Types.Common
{
	/// <summary>
	/// Class implementing ToString() for generic dto class.
	/// </summary>
	public static class Stringable
	{
		private const string S1 = "    ";
		private const string S2 = S1+S1;

		private static string Quote(object value) => value is string ? $"\"{value}\"" : value.ToString();

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public static string ToString<T>(T dto)
		{
			var sb = new StringBuilder(dto.GetType().Name);

			sb.Append(" {");
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dto))
			{
				var name = descriptor.Name;
				object value = descriptor.GetValue(dto);
				if (value == null)
				{
					sb.Append($"\n{S1}{name} = null");
					continue;
				}

				var t = value.GetType();
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					var sbd = new StringBuilder("{");
					foreach (DictionaryEntry kvp in (IDictionary)value)
					{
						sbd.Append($"\n{S2}{kvp.Key} = {Quote(kvp.Value)}");
					}

					sbd.Append($"\n{S1}}}");
					sb.Append($"\n{S1}{name} = {sbd}");
				}
				else if (value is IEnumerable && !(value is string))
				{
					var values = value as IEnumerable;
					var xs = string.Join(", ", values.Cast<object>());
					if (xs.Contains("\n")) {
						var ys = xs.Replace("\n", $"\n{S2}");
						sb.Append($"\n{S1}{name} = [\n{S2}{ys}]");
					} else {
						sb.Append($"\n{S1}{name} = [{xs}]");
					}
				}
				else
				{
					var indented = Quote(value).Replace("\n", $"\n{S2}");
					sb.Append($"\n{S1}{name} = {indented}");
				}
			}
			sb.Append("\n}");
			return sb.ToString();
		}
	}
}

