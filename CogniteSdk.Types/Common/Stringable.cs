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
		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public static string ToString<T>(T dto)
		{
			var type = dto.GetType();
			var className = type.Name;
			var props = new List<string>();
			var s = new StringBuilder(className);
			s.Append("\n{\n");
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dto))
			{
				var name = descriptor.Name;
				object value = descriptor.GetValue(type);
				var t = value.GetType();
				value ??= "null";
				IEnumerable values = value as IEnumerable;
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					var sb = new StringBuilder("\n\t{\n");
					var props2 = new List<string>();

					foreach (DictionaryEntry kvp in (IDictionary)value)
					{
						props2.Add("\t\t" + kvp.Key.ToString() + " = " + kvp.Value.ToString());
					}

					sb.Append(String.Join("\n", props2));
					sb.Append("\n\t}");
					var res = sb.ToString();
					props.Add($"\t{name} = {res}");
				}
				else if (values != null && !(value is string))
				{
					var xs = String.Join(", ", values.Cast<object>());
					props.Add($"\t{name} = [{xs}]");
				}
				else
				{
					props.Add($"\t{name} = {value}");
				}
			}
			s.Append(String.Join("\n", props));
			s.Append("\n}");

			return s.ToString();
		}
	}
}

