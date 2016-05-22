using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hash_Calculator
{
    class Hash
    {
        private byte[] hashValue;
        private SupportedHashAlgorithm hashAlgorithm;
		private Boolean hmac;

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

        public byte[] HashValue
        {
            get
            {
                return hashValue;
            }

            set
            {
                hashValue = value;
            }
        }

        public String HashString
        {
			get
			{
				return BitConverter.ToString(HashValue).Replace("-", string.Empty);
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
	}
}
