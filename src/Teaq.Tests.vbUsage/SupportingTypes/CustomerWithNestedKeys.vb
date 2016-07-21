Public Class CustomerWithNestedKeys

    Private _nestedKeys As New NestedKeys

    Public ReadOnly Property NestedKeys As NestedKeys
        Get
            Return Me._nestedKeys
        End Get
    End Property

    Public Property CustomerKey As String

    Public Property Inception As DateTime

    Public Property Modified As DateTimeOffset

    Public Property Change As Byte

    Public Property Deleted As DateTimeOffset?
End Class
