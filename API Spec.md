# Implied Order Grid

Subclass of Grid that defines row and column assignments of child views by the order they are defined in the grid.  User will define Row and Column definitions like a normal grid  RowSpan and ColumnSpan are honored during the auto assignment of row and column.

If the row or column definitions are changed after the grid is initialized, the row and column assignments of the child will be recalculated.

By default if more cells in the grid are needed by the number of child views then are defined and output warning will be generated.  If a row or column span exceeds the defined rows and columns output warnings will be generated.  The user may at their choice decide to have these warnings treated as exceptions.

# API

## AutoGrid (open to better names)

### Properties

| API                  | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| ThrowOnLayoutWarning | If any warnings generated should be logged or thrown as exceptions when true. |

All other behavior Grid is retained

# Scenarios

Allows a user to omit the Grid.Row and Grid.Column attributes are rely on the order child views are defined in XAML or added in code determine what grid cell they are assigned to.



# Platform Compatibility

- Target Frameworks: 
  - iOS:  
  - Android: 
  - UWP: 
# Backward Compatibility

# Difficulty : [low]



Prototype

https://github.com/bmacombe/AutoLayoutGrid