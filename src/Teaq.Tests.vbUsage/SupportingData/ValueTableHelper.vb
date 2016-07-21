Imports System.Data

Public Class ValueTableHelper(Of T)
    Implements IDataHelper

    Private _table As DataTable = CreateTable()

    Public Sub AddRow(ByVal item As T)
        Dim row As DataRow = Me._table.NewRow()
        row.ItemArray = New Object() {item}
        Me._table.Rows.Add(row)
    End Sub

    Public Function GetReader() As IDataReader Implements IDataHelper.GetReader
        Return Me._table.CreateDataReader()
    End Function

    Private Shared Function CreateTable() As DataTable
        Dim table As New DataTable
        table.Columns.Add("value", GetType(T))
        Return table
    End Function
End Class
