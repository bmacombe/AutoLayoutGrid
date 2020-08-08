using System;
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
	}
}
