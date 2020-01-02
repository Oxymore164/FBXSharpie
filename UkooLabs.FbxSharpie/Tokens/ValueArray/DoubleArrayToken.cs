﻿using System.IO;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class DoubleArrayToken : Token
	{
		public double[] Values { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var count = Values.Length;
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

		internal override void WriteAscii(FbxVersion version, LineStringBuilder lineStringBuilder, int indentLevel)
		{
			var arrayLength = Values.Length;
			WriteAsciiArray(version, lineStringBuilder, arrayLength, indentLevel, (itemWriter) =>
			{
				for (var i = 0; i < Values.Length; i++)
				{
					if (i > 0)
					{
						lineStringBuilder.Append(",");
					}
					lineStringBuilder.Append(Values[i].ToString());
				}
			});
		}

		public DoubleArrayToken(double[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Double)
		{
			Values = values;
		}
	}
}
