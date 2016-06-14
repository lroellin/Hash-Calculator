using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreami.Hash_Calculator
{
    public class Hash
    {
        private SupportedHashAlgorithm hashAlgorithm;
		private Boolean hmac;
		private String hashString;

        public Hash(SupportedHashAlgorithm hashAlgorithm, bool? hmac)
        {
            this.HashAlgorithm = hashAlgorithm;
			this.Hmac = hmac;
        }

		public SupportedHashAlgorithm HashAlgorithm
		{
			get
			{
				return hashAlgorithm;
			}

			set
			{
				hashAlgorithm = value;
			}
		}

		public bool? Hmac
		{
			get
			{
				return hmac;
			}

			set
			{
				if(value.HasValue)
				{
					hmac = (bool)value;
				} else
				{
					hmac = false;
				}
			}
		}

		public string HashString
		{
			get
			{
				return hashString;
			}

			set
			{
				hashString = value;
			}
		}
	}
}
