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
			var nl = System.Environment.NewLine;
			var sb = new StringBuilder(dto.GetType().Name);

			sb.Append(" {{nl}");
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dto))
			{
				var name = descriptor.Name;
				object value = descriptor.GetValue(dto);
				if (value == null)
				{
					sb.Append($"{S1}{name} = null{nl}");
					continue;
				}

				var t = value.GetType();
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					var sbd = new StringBuilder("{{nl}");
					foreach (DictionaryEntry kvp in (IDictionary)value)
					{
						sbd.Append($"{S2}{kvp.Key} = {Quote(kvp.Value)}{nl}");
					}

					sbd.Append($"{S1}}}");
					sb.Append($"{S1}{name} = {sbd}{nl}");
				}
				else if (value is IEnumerable && !(value is string))
				{
					var values = value as IEnumerable;
					var xs = string.Join(", ", values.Cast<object>());
					if (xs.Contains(nl)) {
						var ys = xs.Replace("{nl}", $"{nl}{S2}");
						sb.Append($"{S1}{name} = [{nl}{S2}{ys}]{nl}");
					} else {
						sb.Append($"{S1}{name} = [{xs}]{nl}");
					}
				}
				else
				{
					var indented = Quote(value).Replace(nl, $"{nl}{S1}");
					sb.Append($"{S1}{name} = {indented}{nl}");
				}
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}

