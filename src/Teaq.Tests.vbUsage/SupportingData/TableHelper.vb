Public Class TableHelper
    Implements IDataHelper, IDisposable

    Private _table As DataTable

    Public Sub New(ByVal ParamArray columnNames As String())
        Me._table = New DataTable()
        For i As Integer = 0 To columnNames.GetLength(0) - 1
            Me._table.Columns.Add(columnNames(i), GetType(Object))
        Next
    End Sub

    Public Sub AddRow(ByVal ParamArray items As Object())
        Dim row As DataRow = Me._table.NewRow()
        row.ItemArray = items
        Me._table.Rows.Add(row)
    End Sub

    Public Function GetReader() As IDataReader Implements IDataHelper.GetReader
        Return Me._table.CreateDataReader()
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
