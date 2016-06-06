using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dreami.Hash_Calculator
{

	public static class UserInput
	{
		public static String normalize(String input)
		{
			String normal;
			normal = input.ToUpper();
			normal = Regex.Replace(normal, @"[^A-Za-z0-9]+", "");
			return normal;
		}
	}
}
