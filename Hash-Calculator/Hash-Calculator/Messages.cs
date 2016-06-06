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
		public static readonly String infHashFileFormats = "- filename.md5\n- filename.sha1\n- filename.sha256\n- filename.sha384\n- filename.sha512";
		public static readonly String infNormalization = "- Capitalized\n- Alphanumeric only";

		public static void calculationOngoing()
		{
			MessageBox.Show("Calculation has not yet completed.", "Calculation ongoing", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public static void readException(Exception e)
		{
			MessageBox.Show(e.Message, "Could not read file", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static void saveException(Exception e)
		{
			MessageBox.Show(e.Message, "Could not save file", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static MessageBoxResult saveConfirmation()
		{
			return MessageBox.Show("This will create or overwrite the following files:\n" + Messages.infHashFileFormats + "\n\nContinue?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.Cancel);

		}
	}
}
