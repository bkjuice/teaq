Imports FluentAssertions

<TestClass()>
Public Class SanityCheck
    <TestMethod>
    Public Sub ExceptionsAreExpensive()

    End Sub

    <TestMethod>
    Public Sub ReturnValuesAreCheap()

    End Sub

    <TestMethod()>
    Public Sub NullIsAsItShouldBeAndTheWorldIsOk()
        Dim d As DataSet = Nothing
        Dim test As Action =
            Sub()
                If d.Tables Is Nothing Then Throw New NotSupportedException()
            End Sub

        test.ShouldThrow(Of NullReferenceException)()
    End Sub

    <TestMethod>
    Public Sub EmptyTablesCollectionThrowsIndexOutOfRangeException()
        Dim d As New DataSet()
        Dim test As Action =
            Sub()
                If d.Tables(0) Is Nothing Then Throw New InvalidOperationException()
            End Sub

        test.ShouldThrow(Of IndexOutOfRangeException)()
    End Sub

End Class
