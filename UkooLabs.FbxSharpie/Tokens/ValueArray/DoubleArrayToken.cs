﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class DoubleArrayToken : Token
	{
		public List<double> Values { get; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var count = Values.Count;
			binaryWriter.Write((byte)'d');
			binaryWriter.Write(count);
			var uncompressedSize = count * sizeof(double);
			WriteBinaryArray(binaryWriter, uncompressedSize, (itemWriter) => {
				foreach (var value in Values)
				{
					itemWriter.Write(value);
				}
			});
		}

		internal override void WriteAscii(FbxVersion version, StringBuilder stringBuilder, int indentLevel, ref int lineStart)
		{
			var arrayLength = Values.Count;
			WriteAsciiArray(version, stringBuilder, arrayLength, indentLevel, ref lineStart, (itemWriter, currentLineStart) =>
			{
				bool pFirst = true;
				foreach (var value in Values)
				{
					var stringValue = value.ToString();
					if (!pFirst)
					{
						stringBuilder.Append(',');
					}
					if ((stringBuilder.Length - currentLineStart) + stringValue.Length >= Settings.MaxLineLength)
					{
						stringBuilder.Append('\n');
						currentLineStart = stringBuilder.Length;
					}
					stringBuilder.Append(stringValue);
					pFirst = false;
				}
				return currentLineStart;
			});
		}

		public DoubleArrayToken() : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Double)
		{
			Values = new List<double>();
		}

		public DoubleArrayToken(double[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Double)
		{
			Values = new List<double>(values);
		}
	}
}
