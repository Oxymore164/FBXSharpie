﻿using UkooLabs.FbxSharpie.Tokens;
using UkooLabs.FbxSharpie.Tokens.Value;
using System.Globalization;

namespace UkooLabs.FbxSharpie.Extensions
{
	internal static class StringExtension
	{
		public static bool TryParseNumberToken(this string value, out Token numberToken)
		{
			if (value.Contains("."))
			{
				var splitValue = value.Split('.', 'e', 'E');
				if (splitValue.Length > 1 && splitValue[1].Length > 6)
				{
					if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
					{
						numberToken = null;
						return false;
					}
					numberToken = new DoubleToken(d);
					return true;
				}
				if (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
				{
					numberToken = null;
					return false;
				}
				numberToken = new FloatToken(f);
				return true;
			}
			if (!long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var l))
			{
				numberToken = null;
				return false;
			}
			if (l >= int.MinValue && l <= int.MaxValue)
			{
				numberToken = new IntegerToken((int)l);
				return true;
			}
			numberToken = new LongToken(l);
			return true;
		}

	}
}
