using System;
using System.Linq;
using Xamarin.Forms;

namespace AutoLayoutGrid
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void ButtonAddRow_OnClicked(object sender, EventArgs e)
		{
			TestGrid.RowDefinitions.Add(new RowDefinition());
		}

		private void ButtonAddColumn_OnClicked(object sender, EventArgs e)
		{
			TestGrid.ColumnDefinitions.Add(new ColumnDefinition());
		}

		private void ButtonRemoveRow_OnClicked(object sender, EventArgs e)
		{
			var lastRow = TestGrid.RowDefinitions.Last();
			TestGrid.RowDefinitions.Remove(lastRow);
		}

		private void ButtonRemoveColumn_OnClicked(object sender, EventArgs e)
		{
			var lastColumn = TestGrid.ColumnDefinitions.Last();
			TestGrid.ColumnDefinitions.Remove(lastColumn);
		}
	}
}
