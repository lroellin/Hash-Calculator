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
		public static Hash CalculateHash(Hash hash, String filePath)
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
						hash.HashString = BytesToString(MD5.Create().ComputeHash(stream));
						break;
					}
					case SupportedHashAlgorithm.SHA1:
					{
							hash.HashString = BytesToString(SHA1.Create().ComputeHash(stream));
							break;
					}

					case SupportedHashAlgorithm.SHA256:
					{
							hash.HashString = BytesToString(SHA256.Create().ComputeHash(stream));
							break;
					}
					case SupportedHashAlgorithm.SHA384:
					{
							hash.HashString = BytesToString(SHA384.Create().ComputeHash(stream));
							break;
					}
					case SupportedHashAlgorithm.SHA512:
					{
							hash.HashString = BytesToString(SHA512.Create().ComputeHash(stream));
							break;
					}
					case SupportedHashAlgorithm.RIPEMD160:
					{
							hash.HashString = BytesToString(RIPEMD160.Create().ComputeHash(stream));
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

		private static String BytesToString(byte[] bytes)
		{
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		public static Boolean CheckHash(Hash hash, String filePath)
		{
			StreamReader stream = null;
			try
			{
				String fileToCheck = HashFile.HashFilePath(hash, filePath);
				stream = new StreamReader(fileToCheck);
				String line = stream.ReadLine();
				String hashPart = UserInput.Normalize(line.Split(' ')[0]);
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
