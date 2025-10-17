Imports Microsoft.VisualBasic
<Serializable>
Public Class ClsInsightLineChartData
    Public label As String
    Public x As String
    Public y As Decimal?

    Sub New(_label As String, _x As String, _y As Decimal?)
        label = _label
        y = _y
        x = _x
    End Sub


End Class
