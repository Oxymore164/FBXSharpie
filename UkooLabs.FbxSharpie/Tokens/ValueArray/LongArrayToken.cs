﻿using System.IO;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class LongArrayToken : Token
	{
		public long[] Values { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var count = Values.Length;
			binaryWriter.Write((byte)'l');
			binaryWriter.Write(count);
			var uncompressedSize = count * sizeof(long);
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

		public LongArrayToken(long[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Long)
		{
			Values = values;
		}
	}
}
