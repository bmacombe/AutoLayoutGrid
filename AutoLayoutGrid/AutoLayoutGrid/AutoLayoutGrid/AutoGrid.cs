using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AutoLayoutGrid
{
	public class AutoGrid : Grid
	{
		public AutoGrid()
		{
			ColumnDefinitions.ItemSizeChanged += ColumnDefinitions_ItemSizeChanged;
			RowDefinitions.ItemSizeChanged += RowDefinitions_ItemSizeChanged;
		}

		private void RowDefinitions_ItemSizeChanged(object sender, EventArgs e)
		{
		
		}

		private void ColumnDefinitions_ItemSizeChanged(object sender, EventArgs e)
		{
			
		}

		private bool[][] _UsedMatrix;
		private int _rowCount;
		private int _columnCount;

		void FindNextCell(int rowSpan, int columnSpan, out int rowIndex, out int columnIndex)
		{
			if (_UsedMatrix == null)
			{
				_rowCount = RowDefinitions.Count;
				_columnCount = ColumnDefinitions.Count;
				_UsedMatrix = new bool[_rowCount][];
				for (var r = 0; r < _columnCount; r++)
					_UsedMatrix[r] = new bool[_columnCount];
			}

			//Find the first open row
			var firstOpenRow = _UsedMatrix.First(r => r.Any(c => c == false));
			rowIndex = _UsedMatrix.IndexOf(firstOpenRow);

			//Find the first open column
			columnIndex = firstOpenRow.IndexOf(firstOpenRow.First(c => c == false));
		}

		void UpdateUsedCells(int row, int column, int rowSpan, int columnSpan)
		{
			var rowEnd = row + rowSpan;
			var columnEnd = column + columnSpan;

			if (columnEnd > _columnCount)
			{
				columnEnd = _columnCount;
				Log.Warning(nameof(AutoGrid), $"View at row {row} column {columnEnd} with column span {columnSpan} exceeds the defined grid columns.");
			}

			if (rowEnd > _rowCount)
			{
				rowEnd = _rowCount;
				Log.Warning(nameof(AutoGrid), $"View at row {row} column {columnEnd} with row span {rowSpan} exceeds the defined grid rows.");
			}

			for (var r = row; r < rowEnd; r++)
				for (var c = column; c < columnEnd; c++)
					_UsedMatrix[r][c] = true;
		}

		protected override void OnAdded(View view)
		{
			base.OnAdded(view);

			// Get position request
			var column = GetColumn(view);
			var row = GetRow(view);
			var columnSpan = GetColumnSpan(view);
			var rowSpan = GetRowSpan(view);

			FindNextCell(rowSpan, columnSpan, out row, out column);
			UpdateUsedCells(row, column, rowSpan, columnSpan);

			// Set attributes
			view.SetValue(ColumnProperty, column);
			view.SetValue(RowProperty, row);

			//// Get counts
			//var definedColumns = ColumnDefinitions.Count;
			//var definedRows = RowDefinitions.Count;

			//if (_UsedMatrix == null)
			//	_UsedMatrix = new bool[definedRows, definedColumns];

			//if (_currentRow == RowDefinitions.Count)
			//	throw new Exception($"Rows required '{_currentRow + 1}' exceeds rows defined '{definedRows}'.");

			//// Calculate position
			//var column = GetColumn(view) == 0 ? 0 : _currentColumn;
			//var row = GetRow(view) == 0 ? 0 : _currentColumn;
			//var columnSpan = GetColumnSpan(view);
			//var rowSpan = GetRowSpan(view);

			//// Find any conflicts
			//// TODO

			//// Set attributes
			//view.SetValue(ColumnProperty, _currentColumn);
			//view.SetValue(RowProperty, _currentRow);

			////TODO check for existing conflicts from spans

			//if (_currentColumn + columnSpan > definedColumns)
			//	throw new Exception($"Column Span '{columnSpan}' with column '{column}' exceeds columns defined '{definedColumns}'.");
			//if (_currentRow + rowSpan > definedRows)
			//	throw new Exception($"Row Span '{rowSpan}' with row '{row}' exceeds rows defined '{definedRows}'.");

			//// Increment Counters
			//_currentColumn += columnSpan;
			//if (_currentColumn >= ColumnDefinitions.Count)
			//{
			//	_currentColumn = 0;
			//	_currentRow++;
			//}
		}
	}
}
