using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hash_Calculator
{
	class HashAlgorithmNotImplementedException : Exception
	{
		public HashAlgorithmNotImplementedException()
		{
		}

		public HashAlgorithmNotImplementedException(string message) : base(message)
		{
		}

		public HashAlgorithmNotImplementedException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
