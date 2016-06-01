using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
	class HMACNotSupportedException : Exception
	{
		public HMACNotSupportedException()
		{
		}

		public HMACNotSupportedException(string message) : base(message)
		{
		}

		public HMACNotSupportedException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
