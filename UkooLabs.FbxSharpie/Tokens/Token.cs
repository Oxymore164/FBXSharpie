﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UkooLabs.FbxSharpie.Tokens.Value;

namespace UkooLabs.FbxSharpie.Tokens
{
	public enum TokenTypeEnum
	{
		EndOfStream,
		Comment,
		WhiteSpace,
		OpenBrace,
		CloseBrace,
		Comma,
		Asterix,
		Identifier,
		String,
		Value,
		ValueArray
	}

	public enum ValueTypeEnum
	{
		None,
		Boolean,
		Byte,  // valid for array only
		Short,  //not valid for array
		Integer, 
		Long,
		Float,
		Double
	}

	public class Token : IEquatable<Token>
	{

		public TokenTypeEnum TokenType { get; }

		public ValueTypeEnum ValueType { get; }

		internal virtual void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			throw new NotImplementedException();
		}

		internal virtual void WriteAscii(FbxVersion version, StringBuilder stringBuilder, int indentLevel, ref int lineStart)
		{
			throw new NotImplementedException();
		}

		internal void WriteBinaryArray(BinaryWriter stream, int uncompressedSize, Action<BinaryWriter> itemWriterAction)
		{
			bool compress = uncompressedSize >= Settings.CompressionThreshold;

			stream.Write(compress ? 1 : 0);

			if (compress)
			{
				var compressLengthPos = stream.BaseStream.Position;
				stream.Write(0);
				var dataStart = stream.BaseStream.Position;
				stream.Write(new byte[] { 0x58, 0x85 }, 0, 2);

				uint checksum;
				using (var deflateStream = new DeflateStream(stream.BaseStream, CompressionMode.Compress, true))
				using (var checksumBinaryWriter = new ChecksumBinaryWriter(deflateStream))
				{
					itemWriterAction.Invoke(checksumBinaryWriter);
					checksum = checksumBinaryWriter.Checksum;
				}

				var checksumBytes = new byte[]
				{
					(byte)((checksum >> 24) & 0xFF),
					(byte)((checksum >> 16) & 0xFF),
					(byte)((checksum >> 8) & 0xFF),
					(byte)(checksum & 0xFF),
				};
				stream.Write(checksumBytes);

				var dataEnd = stream.BaseStream.Position;
				stream.BaseStream.Position = compressLengthPos;
				var compressedSize = (int)(dataEnd - dataStart);
				stream.Write(compressedSize);
				stream.BaseStream.Position = dataEnd;
			}
			else
			{
				stream.Write(uncompressedSize);
				itemWriterAction.Invoke(stream);
			}
		}

		internal void WriteAsciiArray(FbxVersion version, StringBuilder stringBuilder, int arrayLength, int indentLevel, ref int lineStart, Func<StringBuilder, int, int> itemWriterAction)
		{
			if (version >= FbxVersion.v7_1)
			{
				stringBuilder.Append('*').Append(arrayLength).Append(" {\n");
				lineStart = stringBuilder.Length;
				for (int i = -1; i < indentLevel; i++)
				{
					stringBuilder.Append('\t');
				}

				stringBuilder.Append("a: ");
			}

			lineStart = itemWriterAction.Invoke(stringBuilder, lineStart);

			if (version >= FbxVersion.v7_1)
			{
				stringBuilder.Append('\n');
				for (int i = 0; i < indentLevel; i++)
				{
					stringBuilder.Append('\t');
				}
				stringBuilder.Append('}');
			}
		}

		//private bool SameType<T, U, V>(T first, U second) where T : BooleanToken where U : BooleanToken
		//{
		//	return first.GetType().Equals(second.GetType());
		//}

		//private bool SameType<T, U>(T first, U second) where T : DoubleToken where U : DoubleToken
		//{
		//	return first.GetType().Equals(second.GetType());
		//}

		//private bool Compare<T>(T first, T second) where T : IValueToken<T>
		//{
		//	return first.Value.Equals(second.Value);
		//}

		public bool Equals(Token other)
		{
			if (other != null)
			{
				if (this is BooleanToken booleanToken && other is BooleanToken booleanTokenOther)
				{
					return booleanToken.Value = booleanTokenOther.Value;
				}
				if (this is ShortToken shortToken && other is ShortToken shortTokenOther)
				{
					return shortToken.Value == shortTokenOther.Value;
				}
				if (this is IntegerToken integerToken && other is IntegerToken integerTokenOther)
				{
					return integerToken.Value == integerTokenOther.Value;
				}
				if (this is LongToken longToken && other is LongToken longTokenOther)
				{
					return longToken.Value == longTokenOther.Value;
				}
				if (this is FloatToken floatToken && other is FloatToken floatTokenOther)
				{
					return floatToken.Value == floatTokenOther.Value;
				}
				if (this is DoubleToken doubleToken && other is DoubleToken doubleTokenOther)
				{
					return doubleToken.Value == doubleTokenOther.Value;
				}
				if (this is StringToken stringToken && other is StringToken stringTokenOther)
				{
					return stringToken.Value == stringTokenOther.Value;
				}
				if (this is CommentToken commentToken && other is CommentToken commentTokenOther)
				{
					return commentToken.Value == commentTokenOther.Value;
				}
				if (this is IdentifierToken identifierToken && other is IdentifierToken identifierTokenOther)
				{
					return identifierToken.Value == identifierTokenOther.Value;
				}
				if (TokenType == other.TokenType && TokenType != TokenTypeEnum.ValueArray && ValueType == other.ValueType)
				{
					return true;
				}
			}
			return false;
		}

		internal Token(TokenTypeEnum tokenType, ValueTypeEnum valueType)
		{
			TokenType = tokenType;
			ValueType = valueType;
		}
	}
}
