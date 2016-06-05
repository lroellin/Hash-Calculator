using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
	static class HashFile
	{

		public static void writeHashFile(Hash hash, String filePath)
		{
			String hashFile = HashFile.hashFilePath(hash, filePath);
			// Format: ea912a289186e5120eac3a722fe23c2f *apr-1.5.2-win32-src.zip
			String line = hash.HashString.ToLower() + " *" + filePath;
			File.WriteAllText(hashFile, line);
		}

		public static String hashFilePath(Hash hash, String filePath)
		{
			return filePath + "." + (hash.HashAlgorithm.ToString().ToLower());
		}

	}
}
