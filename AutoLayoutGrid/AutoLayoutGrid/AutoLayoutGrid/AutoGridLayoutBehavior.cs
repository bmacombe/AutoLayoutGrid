using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AutoLayoutGrid
{
	public class AutoGridLayoutBehavior : Behavior<Grid>
	{
		private bool[][] _UsedMatrix;
		private int _rowCount;
		private int _columnCount;
		private Grid _AttachedGrid;

		public bool ThrowOnLayoutWarning { get; set; }

		protected override void OnAttachedTo(Grid bindable)
		{
			base.OnAttachedTo(bindable);

			bindable.ChildAdded += _InternalGrid_ChildAdded;
			_AttachedGrid = bindable;
		}

		protected override void OnDetachingFrom(Grid bindable)
		{
			base.OnDetachingFrom(bindable);

			bindable.ChildAdded -= _InternalGrid_ChildAdded;
			_AttachedGrid = null;
		}

		private void _InternalGrid_ChildAdded(object sender, ElementEventArgs e)
		{
			ProcessElement(e.Element);
		}


		void LogWarning(string warning)
		{
			Log.Warning(nameof(AutoGrid), warning);
			if (ThrowOnLayoutWarning)
				throw new Exception(warning);
		}

		bool[][] InitMatrix()
		{
			_rowCount = _AttachedGrid.RowDefinitions.Count;
			_columnCount = _AttachedGrid.ColumnDefinitions.Count;
			var newMatrix = new bool[_rowCount][];
			for (var r = 0; r < _rowCount; r++)
				newMatrix[r] = new bool[_columnCount];
			return newMatrix;
		}

		void FindNextCell(int rowSpan, int columnSpan, out int rowIndex, out int columnIndex)
		{
			if (_UsedMatrix == null)
				_UsedMatrix = InitMatrix();

			// Find the first available row
			var row = _UsedMatrix.FirstOrDefault(r => r.Any(c => !c));

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

		void ProcessElement(BindableObject view)
		{
			var columnSpan = Grid.GetColumnSpan(view);
			var rowSpan = Grid.GetRowSpan(view);

			FindNextCell(rowSpan, columnSpan, out var row, out var column);
			UpdateUsedCells(row, column, rowSpan, columnSpan);

			// Set attributes
			view.SetValue(Grid.ColumnProperty, column);
			view.SetValue(Grid.RowProperty, row);
			//view.SetValue(Grid.ColumnSpanProperty, columnSpan);
			//view.SetValue(Grid.RowSpanProperty, rowSpan);
		}
	}
}
