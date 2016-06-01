﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
	public class HashFile
	{

		private StreamWriter writer;
		public HashFile(String filepath)
		{
			this.writer = new StreamWriter(filepath, false);
		}

		public void addHeader(String file)
		{
			writer.WriteLine("File: " + file);
		}

		public void addHashline(SupportedHashAlgorithm algorithm, String hash)
		{
			writer.WriteLine(algorithm.ToString() + new String('\t', Constants.HASHFILE_TAB_COUNT) + hash);
		}

		public void closeFile()
		{
			writer.Close();
		}

	}
}
