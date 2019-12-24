﻿using System;
using System.Collections.Generic;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// An error with the FBX data input
	/// </summary>
	public class FbxException : Exception
	{
		/// <summary>
		/// An error at a binary stream offset
		/// </summary>
		/// <param name="position"></param>
		/// <param name="message"></param>
		public FbxException(long position, string message) :
			base($"{message}, near offset {position}")
		{
		}

		/// <summary>
		/// An error in a text file
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <param name="message"></param>
		public FbxException(FbxAsciiFileInfo fbxAsciiFileInfo, string message) :
			base($"{message}, near line {fbxAsciiFileInfo.Line} column {fbxAsciiFileInfo.Column}")
		{
		}
	}
}
