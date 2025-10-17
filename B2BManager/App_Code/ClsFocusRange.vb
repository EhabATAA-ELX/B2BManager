Imports System.Data
Imports Microsoft.VisualBasic

''' <summary>
''' Holds the results from the PS_ConditionsByFocusRange stored procedure.
''' </summary>
Public Class ClsFocusRange
    Public Property Conditions As DataTable
    Public Property StaticConditionExists As Boolean

    Public Sub New()
        Me.Conditions = New DataTable()
        Me.StaticConditionExists = False
    End Sub
End Class
