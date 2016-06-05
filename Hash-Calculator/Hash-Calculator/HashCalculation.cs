using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
	static class HashCalculation
	{
		public static Hash calculateHash(Hash hash, String filePath)
		{
			if(filePath == null)
			{
				throw new NullReferenceException();
			}
			FileStream stream;
			using (stream = File.OpenRead(filePath))
			{
				switch (hash.HashAlgorithm)
				{
					case SupportedHashAlgorithm.MD5:
					{
						hash.HashValue = MD5.Create().ComputeHash(stream);
						break;
					}
					case SupportedHashAlgorithm.SHA1:
					{
						hash.HashValue = SHA1.Create().ComputeHash(stream);
						break;
					}

					case SupportedHashAlgorithm.SHA256:
					{
						hash.HashValue = SHA256.Create().ComputeHash(stream);
						break;
					}
					case SupportedHashAlgorithm.SHA384:
					{
						hash.HashValue = SHA384.Create().ComputeHash(stream);
						break;
					}
					case SupportedHashAlgorithm.SHA512:
					{
						hash.HashValue = SHA512.Create().ComputeHash(stream);
						break;
					}
					default:
					{
						throw new HashAlgorithmNotImplementedException();
					}
				}
			}
			return hash;
		}

		public static Boolean checkHash(Hash hash, String filePath)
		{
			StreamReader stream = null;
			try
			{
				String fileToCheck = HashFile.hashFilePath(hash, filePath);
				stream = new StreamReader(fileToCheck);
				String line = stream.ReadLine();
				String hashPart = line.Split(' ')[0].ToUpper();
				return hashPart.Equals(hash.HashString);
			}
			catch (Exception e)
			{
				throw new HashCheckException(e.Message);
			}
			finally
			{
				if(stream != null)
				{
					stream.Close();
				}
			}
		}

	}
}
