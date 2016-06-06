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

		public static void WriteHashFile(Hash hash, String filePath)
		{
			String hashFile = HashFile.HashFilePath(hash, filePath);
			// Format: ea912a289186e5120eac3a722fe23c2f *apr-1.5.2-win32-src.zip
			String line = hash.HashString.ToLower() + " *" + filePath + "\n";
			File.WriteAllText(hashFile, line);
		}

		public static String HashFilePath(Hash hash, String filePath)
		{
			return filePath + "." + (hash.HashAlgorithm.ToString().ToLower());
		}

	}
}
