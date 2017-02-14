using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
    public class Hash
    {
		private Boolean hmac;

        public Hash(SupportedHashAlgorithm hashAlgorithm, bool? hmac)
        {
            this.HashAlgorithm = hashAlgorithm;
			this.Hmac = hmac;
        }

		public SupportedHashAlgorithm HashAlgorithm;

		public bool? Hmac
		{

			set
			{
				if(value.HasValue)
				{
					hmac = (bool)value;
				}
				else
				{
					hmac = false;
				}
			}
		}

		public string HashString;
	}
}
