using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Dreami.Hash_Calculator
{
	public class GUIRow
	{
		private SupportedHashAlgorithm hashAlgorithm;
		private CheckBox checkBox;
		private TextBox textBox;
		private Button copyButton;
		private ProgressBar progressBar;
		private Image hashCheckImage;
		private Hash hash;

		public GUIRow(SupportedHashAlgorithm hashAlgorithm, CheckBox checkBox, TextBox textBox, Button copyButton, ProgressBar progressBar, Image hashCheckImage)
		{ 
			this.hashAlgorithm = hashAlgorithm;
			this.CheckBox = checkBox;
			this.TextBox = textBox;
			this.CopyButton = copyButton;
			this.progressBar = progressBar;
			this.HashCheckImage = hashCheckImage;
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

		public CheckBox CheckBox
		{
			get
			{
				return checkBox;
			}

			set
			{
				checkBox = value;
			}
		}

		public TextBox TextBox
		{
			get
			{
				return textBox;
			}

			set
			{
				textBox = value;
			}
		}

		public Button CopyButton
		{
			get
			{
				return copyButton;
			}

			set
			{
				copyButton = value;
			}
		}

		public ProgressBar ProgressBar
		{
			get
			{
				return progressBar;
			}

			set
			{
				progressBar = value;
			}
		}

		public Image HashCheckImage
		{
			get
			{
				return hashCheckImage;
			}

			set
			{
				hashCheckImage = value;
			}
		}

		public Hash Hash
		{
			get
			{
				return hash;
			}

			set
			{
				hash = value;
			}
		}

		public Boolean isChecked()
		{
			return (bool)checkBox.IsChecked;
		}
	}
}
