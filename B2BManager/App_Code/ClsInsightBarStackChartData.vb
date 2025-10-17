Imports Microsoft.VisualBasic
<Serializable>
Public Class ClsInsightBarStackChartData
    Inherits ClsInsightPieChartData
    Public category As String
    Public stack As String

    Public Sub New(name As String, value As Decimal, category As String, stack As String)
        MyBase.New(name, value)
        Me.category = category
        Me.stack = stack
    End Sub
End Class
