
Imports System.Data
Imports System.Diagnostics

Partial Class MonitoringTool
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = TryCast(Request("__EVENTTARGET"), String)
        If Not IsPostBack Then
            PopulateSearchGrid()
        Else
            If target IsNot Nothing Then
                If target.ToLower().Contains("gridsearch") Or target.Equals("") Then
                    PopulateSearchGrid()
                End If
            End If
        End If
    End Sub

    Private Sub PopulateSearchGrid()
        Dim watch As Stopwatch = Stopwatch.StartNew()
        gridSearch.DataSource = ClsDataAccessHelper.FillDataTable("Monitoring.GetMonitoringMessages")
        gridSearch.DataBind()
        watch.Stop()
        ClsHelper.Log("Get Monitoring Messages", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
    End Sub

End Class
