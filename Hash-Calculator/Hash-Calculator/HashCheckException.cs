using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
	class HashCheckException : Exception
	{
		public HashCheckException()
		{
		}

		public HashCheckException(string message) : base(message)
		{
		}

		public HashCheckException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
