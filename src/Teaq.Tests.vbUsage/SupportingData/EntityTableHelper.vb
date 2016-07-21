Imports System.Reflection
Imports Teaq.FastReflection

Public Class EntityTableHelper(Of T As Class)
    Implements IDataHelper, IDisposable

    Private _table As DataTable

    Public Sub New(Optional ByVal looseTypes As Boolean = False)
        Me._table = CreateTable(looseTypes)
    End Sub

    Public Sub AddRow(ByVal target As T)
        Dim t As TypeDescription = GetType(T).GetTypeDescription(MemberTypes.Property)
        Dim properties As PropertyDescription() = t.GetProperties()
        Dim items As New List(Of Object)

        For i As Integer = 0 To properties.GetLength(0) - 1
            items.Add(properties(i).GetValue(target))
        Next

        Me.AddRow(items.ToArray())
    End Sub

    Public Sub AddRow(ByVal ParamArray items As Object())
        Dim row As DataRow = Me._table.NewRow()
        row.ItemArray = items
        Me._table.Rows.Add(row)
    End Sub

    Public Function GetReader() As IDataReader Implements IDataHelper.GetReader
        Return Me._table.CreateDataReader()
    End Function

    Private Shared Function CreateTable(Optional ByVal looseTypes As Boolean = False) As DataTable
        Dim table As New DataTable
        Dim t As TypeDescription =
            GetType(T).GetTypeDescription(MemberTypes.Property)

        Dim properties As PropertyDescription() = t.GetProperties()
        For i As Integer = 0 To properties.GetLength(0) - 1
            table.Columns.Add(properties(i).MemberName,
                              If(looseTypes, GetType(Object), properties(i).PropertyType))
        Next

        Return table
    End Function


#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me._table.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
