Public Class NonIListCollection(Of T)
    Implements IEnumerable(Of T)

    Private items As List(Of T)

    Public Sub New(ByVal items As List(Of T))
        Me.items = items
    End Sub

    Public Function InvalidContains(ByVal item As T) As Boolean
        Return Me.items.Contains(item)
    End Function

    Public Function Contains(ByVal item As T) As Boolean
        Return Me.items.Contains(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return Me.items.GetEnumerator()
    End Function

    Public Function GetUntypedEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return Me.items.GetEnumerator()
    End Function

End Class
