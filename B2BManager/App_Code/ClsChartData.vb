Imports Microsoft.VisualBasic

<Serializable>
Public Class ClsChartData

    Public messageID As String
    Public label As String
    Public time As Double
    Public y As Integer
    Public ExpectedResponseTimeInMilliseconds As Integer
    Public WorstAcceptableResponseTimeInMilliseconds As Integer

    Sub New(_label As String, _time As DateTime, _y As Integer, _messageID As String, _ExpectedResponseTimeInMilliseconds As Integer, _WorstAcceptableResponseTimeInMilliseconds As Integer)
        label = _label
        time = _time.Ticks
        y = _y
        messageID = _messageID
        ExpectedResponseTimeInMilliseconds = _ExpectedResponseTimeInMilliseconds
        WorstAcceptableResponseTimeInMilliseconds = _WorstAcceptableResponseTimeInMilliseconds
    End Sub


End Class

