﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dreami.Hash_Calculator
{
	class AlertManager
	{
		public static void calculationOngoing()
		{
			MessageBox.Show("Calculation has not yet completed.", "Calculation ongoing", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public static void readException(Exception e, String path)
		{
			MessageBox.Show(e.Message, "File not readable", MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}
}
