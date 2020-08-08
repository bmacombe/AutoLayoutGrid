using System;
using Xamarin.Forms;

namespace AutoLayoutGrid
{
	public class AutoGrid : Grid
	{
		private int _currentRow = 0;
		private int _currentColumn = 0;

		protected override void OnAdded(View view)
		{
			base.OnAdded(view);

			// Get counts
			var definedColumns = ColumnDefinitions.Count;
			var definedRows = RowDefinitions.Count;

			if (_currentRow == RowDefinitions.Count)
				throw new Exception($"Rows required '{_currentRow + 1}' exceeds rows defined '{definedRows}'.");

			// Calculate position
			var column = (int)view.GetValue(ColumnProperty) == 0 ? 0 : _currentColumn;
			var row = (int)view.GetValue(RowProperty) == 0 ? 0 : _currentColumn;
			var columnSpan = (int)view.GetValue(ColumnSpanProperty);
			var rowSpan = (int)view.GetValue(RowSpanProperty);

			// Find any conflicts
			// TODO

			// Set attributes
			view.SetValue(ColumnProperty, _currentColumn);
			view.SetValue(RowProperty, _currentRow);

			//TODO check for existing conflicts from spans

			if (_currentColumn + columnSpan > definedColumns)
				throw new Exception($"Column Span '{columnSpan}' with column '{column}' exceeds columns defined '{definedColumns}'.");
			if (_currentRow + rowSpan > definedRows)
				throw new Exception($"Row Span '{rowSpan}' with row '{row}' exceeds rows defined '{definedRows}'.");

			// Increment Counters
			_currentColumn += columnSpan;
			if (_currentColumn >= ColumnDefinitions.Count)
			{
				_currentColumn = 0;
				_currentRow++;
			}
		}
	}
}
