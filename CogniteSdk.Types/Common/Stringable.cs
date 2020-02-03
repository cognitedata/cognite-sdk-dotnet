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
		private static string Quoted(object value) => value is string ? $"\"{value}\"" : value.ToString();

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public static string ToString<T>(T dto)
		{
			var sb = new StringBuilder(dto.GetType().Name);

			sb.Append(" {\n");
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dto))
			{
				var name = descriptor.Name;
				object value = descriptor.GetValue(dto);
				if (value == null)
				{
					sb.Append($"\n\t{name} = null");
					continue;
				}

				var t = value.GetType();
				if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				{
					var sbd = new StringBuilder("{");
					foreach (DictionaryEntry kvp in (IDictionary)value)
					{
						sbd.Append($"\n\t\t{kvp.Key} = {Quoted(kvp.Value)}");
					}

					sbd.Append("\n\t}");
					sb.Append($"\n\t{name} = {sbd}");
				}
				else if (value is IEnumerable && !(value is string))
				{
					var values = value as IEnumerable;
					var xs = String.Join(", ", values.Cast<object>());
					sb.Append($"\n\t{name} = [{xs}]");
				}
				else
				{
					sb.Append($"\n\t{name} = {Quoted(value)}");
				}
			}
			sb.Append("\n}");
			return sb.ToString();
		}
	}
}

