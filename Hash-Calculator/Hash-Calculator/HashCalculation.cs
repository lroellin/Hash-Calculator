using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hash_Calculator
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
	}
}
