Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports FluentAssertions
Imports Teaq.Configuration
Imports Teaq.QueryGeneration

<TestClass()>
Public Class Usages

    <TestMethod()>
    Public Sub UseDataConfigurationToConfigureAbstractEntity()
        Dim model =
            Repository.BuildModel(
                Sub(builder)
                    builder.Entity(Of ConcreteEntity)(tableName:="ConcreteEntity")
                    builder.MapAbstractType(Of IAbstractVbEntity, ConcreteEntity)()
                End Sub)

        Dim command = model.BuildSelectCommand(Of IAbstractVbEntity)(
            Function(a) a.Id = 6)

        command.CommandText.Should().Contain("where ConcreteEntity.Id")
    End Sub

    <TestMethod()>
    Public Sub UseJoinHandlerToSplitIntoComplexTypes()
        Dim model = Repository.BuildModel(
            Sub(builder)
                builder.Entity(Of CustomerWithAddress)().
                    Exclude(Function(x) x.Inception).
                    Exclude(Function(x) x.Modified)

                builder.Entity(Of Address)().
                    Column(Function(a) a.CustomerId).HasMapping("AddressCustomerId").
                    Column(Function(a) a.Change).HasMapping("AddressChange")

            End Sub)

        Dim handler = New JoinHandler(Of CustomerWithAddress)(model)
        Dim map = handler.GetMap()
        map.AddJoinSplit(Of Address)(Sub(c, a) c.Address = a)

        Dim change1 As Byte = 2
        Dim change2 As Byte = 3
        Dim tableHelper = New TableHelper("CustomerId", "CustomerKey", "Change", "AddressId", "AddressCustomerId", "AddressChange")
        tableHelper.AddRow(1, "test1", change1, 5, 1, change2)
        tableHelper.AddRow(2, "test2", change1, 6, 2, change2)

        Dim results As List(Of CustomerWithAddress)
        Using reader = tableHelper.GetReader()
            results = reader.ReadEntities(handler)
        End Using

        results.Count.Should().Be(2)
        results(0).CustomerId.Should().Be(1)
        results(0).Address.Should().NotBeNull()
        results(0).Address.CustomerId.Should().Be(1)

        results(1).CustomerId.Should().Be(2)
        results(1).Address.Should().NotBeNull()
        results(1).Address.CustomerId.Should().Be(2)

    End Sub

End Class