using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AutoLayoutGrid
{
	[ContentProperty(nameof(Children))]
	public class AutoGrid2 : ContentView
	{
		public AutoGrid2()
		{
			_InternalGrid = new Grid();
			Content = _InternalGrid;

			_InternalGrid.ChildAdded += _InternalGrid_ChildAdded;
			ColumnDefinitions.ItemSizeChanged += ColumnDefinitions_ItemSizeChanged;
			RowDefinitions.ItemSizeChanged += RowDefinitions_ItemSizeChanged;
		}

		private bool[][] _UsedMatrix;
		private int _rowCount;
		private int _columnCount;
		readonly Grid _InternalGrid;
		public new IList<View> Children => _InternalGrid.Children;

		public static readonly BindableProperty RowSpanProperty = BindableProperty.CreateAttached("RowSpan", typeof(int), typeof(AutoGrid), 1, validateValue: (bindable, value) => (int)value >= 1);
		public static int GetRowSpan(BindableObject bindable)
		{
			return (int)bindable.GetValue(RowSpanProperty);
		}

		public static readonly BindableProperty ColumnSpanProperty = BindableProperty.CreateAttached("ColumnSpan", typeof(int), typeof(AutoGrid2), 1, validateValue: (bindable, value) => (int)value >= 1);
		public static int GetColumnSpan(BindableObject bindable)
		{
			return (int)bindable.GetValue(ColumnSpanProperty);
		}

		public ColumnDefinitionCollection ColumnDefinitions => _InternalGrid.ColumnDefinitions;
		public RowDefinitionCollection RowDefinitions => _InternalGrid.RowDefinitions;
		public bool ThrowOnLayoutWarning { get; set; }

		void RowDefinitions_ItemSizeChanged(object sender, EventArgs e)
		{
			OnRowOrColumnCountChanged();
		}

		void ColumnDefinitions_ItemSizeChanged(object sender, EventArgs e)
		{
			OnRowOrColumnCountChanged();
		}

		void OnRowOrColumnCountChanged()
		{
			// If there are no children, there is no need to adjust anything
			if (!Children.Any())
				return;

			// Create a new matrix
			_UsedMatrix = InitMatrix();

			// Reassign children
			var orderedChildren = Children.OrderBy(Grid.GetRow).ThenBy(Grid.GetColumn);
			foreach (var child in orderedChildren)
				ProcessElement(child);
		}

		void LogWarning(string warning)
		{
			Log.Warning(nameof(AutoGrid), warning);
			if (ThrowOnLayoutWarning)
				throw new Exception(warning);
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
			// Strip any Grid attached properties
			view.SetValue(Grid.RowProperty, 0);
			view.SetValue(Grid.RowSpanProperty, 1);
			view.SetValue(Grid.ColumnProperty, 0);
			view.SetValue(Grid.ColumnSpanProperty, 1);

			var columnSpan = GetColumnSpan(view);
			var rowSpan = GetRowSpan(view);

			FindNextCell(rowSpan, columnSpan, out var row, out var column);
			UpdateUsedCells(row, column, rowSpan, columnSpan);

			// Set attributes
			view.SetValue(Grid.ColumnProperty, column);
			view.SetValue(Grid.RowProperty, row);
			view.SetValue(Grid.ColumnSpanProperty, GetColumnSpan(view));
			view.SetValue(Grid.RowSpanProperty, GetRowSpan(view));
		}

		private void _InternalGrid_ChildAdded(object sender, ElementEventArgs e)
		{
			ProcessElement(e.Element);
		}

	}
}
