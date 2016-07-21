Public Class ConcreteEntity
    Implements IAbstractVbEntity

    Public Property Data As String Implements IAbstractVbEntity.Data

    Public Overloads Property Id As Integer Implements IAbstractVbEntity.Id, ICommonAbstractVbEntity.Id

End Class
