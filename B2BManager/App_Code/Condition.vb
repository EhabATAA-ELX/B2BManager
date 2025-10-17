Imports Microsoft.VisualBasic

Public Class Condition
    Public Property ConditionID As Guid
    Public Property ConditionName As String
    Public Property ConditionUpdateDate As String
    Public Property ConditionUpdatedBy As String
    Public Property UserID As Nullable(Of Guid)
    Public Property FocusRangeID As Nullable(Of Guid)
End Class
