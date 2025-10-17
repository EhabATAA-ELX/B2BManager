Imports System.Data
Imports Microsoft.VisualBasic

''' <summary>
''' Holds the multiple results from the PS_ConditionsByDocuments stored procedure.
''' </summary>
Public Class ClsDocument
    Public Property Conditions As DataTable
    Public Property StaticConditionExists As Boolean
    Public Property AssignedCustomersNumber As Integer

    Public Sub New()
        ' Initialize with empty values to prevent null reference errors
        Me.Conditions = New DataTable()
        Me.StaticConditionExists = False
        Me.AssignedCustomersNumber = 0
    End Sub
End Class
