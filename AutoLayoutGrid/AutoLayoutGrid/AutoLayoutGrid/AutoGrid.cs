using System;
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
			UpdateMatrix();
		}

		private void ColumnDefinitions_ItemSizeChanged(object sender, EventArgs e)
		{
			UpdateMatrix();
		}

		public bool ThrowOnLayoutWarning { get; set; }

		void LogWarning(string warning)
		{
			Log.Warning(nameof(AutoGrid), warning);
			if (ThrowOnLayoutWarning)
				throw new Exception(warning);
		}

		void UpdateMatrix()
		{
			if (_UsedMatrix == null)
				return;

			var previousRowCount = _rowCount;
			var previousColumnCount = _columnCount;

			var newMatrix = InitMatrix();
			for (var r = 0; r < previousRowCount; r++)
				for (var c = 0; c < previousColumnCount; c++)
					newMatrix[r][c] = _UsedMatrix[r][c];

			_UsedMatrix = newMatrix;
		}

		bool[][] InitMatrix()
		{
			_rowCount = RowDefinitions.Count;
			_columnCount = ColumnDefinitions.Count;
			var newMatrix = new bool[_rowCount][];
			for (var r = 0; r < _rowCount; r++)
				newMatrix[r] = new bool[_columnCount];
			return newMatrix;
		}

		private bool[][] _UsedMatrix;
		private int _rowCount;
		private int _columnCount;

		void FindNextCell(int rowSpan, int columnSpan, ref int rowIndex, ref int columnIndex)
		{
			if (_UsedMatrix == null) 
				_UsedMatrix = InitMatrix();

			// Use the row index provided if it was manually set or find the first available row
			bool[] row;
			if(rowIndex == 0)
				row = _UsedMatrix.FirstOrDefault(r => r.Any(c => !c));
			else
			{
				if (rowIndex < _rowCount)
					row = _UsedMatrix[rowIndex];
				else
					return;
			}

			// If no row is found, set cell to origin and log
			if (row == null)
			{
				LogWarning("Defined cells exceeded.");
				rowIndex = _rowCount == 0 ? 0 : _rowCount - 1;
				columnIndex = _columnCount == 0 ? 0 : _columnCount - 1;
				return;
			}
			rowIndex = _UsedMatrix.IndexOf(row);

			// Find the first available column
			if(columnIndex == 0)
				columnIndex = row.IndexOf(row.First(c => c == false));
		}

		void UpdateUsedCells(int row, int column, int rowSpan, int columnSpan)
		{
			var rowEnd = row + rowSpan;
			var columnEnd = column + columnSpan;

			if (columnEnd > _columnCount)
			{
				columnEnd = _columnCount;
				LogWarning($"View at row {row} column {columnEnd} with column span {columnSpan} exceeds the defined grid columns.");
			}

			if (rowEnd > _rowCount)
			{
				rowEnd = _rowCount;
				LogWarning($"View at row {row} column {columnEnd} with row span {rowSpan} exceeds the defined grid rows.");
			}

			for (var r = row; r < rowEnd; r++)
				for (var c = column; c < columnEnd; c++)
					_UsedMatrix[r][c] = true;
		}

		void ProcessView(View view)
		{
			// Get position request
			var column = GetColumn(view);
			var row = GetRow(view);
			var columnSpan = GetColumnSpan(view);
			var rowSpan = GetRowSpan(view);

			FindNextCell(rowSpan, columnSpan, ref row, ref column);
			UpdateUsedCells(row, column, rowSpan, columnSpan);

			// Set attributes
			view.SetValue(ColumnProperty, column);
			view.SetValue(RowProperty, row);
		}

		protected override void OnAdded(View view)
		{
			base.OnAdded(view);

			ProcessView(view);
		}
	}
}
