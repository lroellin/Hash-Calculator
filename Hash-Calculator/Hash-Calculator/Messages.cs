using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dreami.Hash_Calculator
{
	class Messages
	{
		public static readonly String HASHFILE_FILENAMES = "- filename.md5\n- filename.sha1\n- filename.sha256\n- filename.sha384\n- filename.sha512\n- filename.ripemd160";
		public static readonly String USERINPUT_NORMALIZATION = "- Capitalized\n- Alphanumeric only";

		public static void CalculationOngoing()
		{
			MessageBox.Show("Calculation has not yet completed.", "Calculation ongoing", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public static void ReadException(Exception e)
		{
			MessageBox.Show(e.Message, "Could not read file", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static void SaveException(Exception e)
		{
			MessageBox.Show(e.Message, "Could not save file", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static MessageBoxResult SaveConfirmation()
		{
			return MessageBox.Show("This will create or overwrite the following files (when row is checked):\n" + Messages.HASHFILE_FILENAMES + "\n\nContinue?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);
		}

		public static String RuntimeDisplay(TimeSpan duration)
		{
			String front = Math.Round(duration.TotalSeconds, Constants.RUNTIME_SECONDS_DECIMALS) + "s";
			return front;
		}
	}
}
