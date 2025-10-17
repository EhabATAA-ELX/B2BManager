Imports Microsoft.VisualBasic
<Serializable>
Public Class ClsInsightPieChartData
    Public name As String
    Public value As Decimal

    Public Sub New(name As String, value As Decimal)
        Me.name = name
        Me.value = value
    End Sub
End Class
