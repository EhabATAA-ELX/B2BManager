
Imports System.Data
Imports System.Data.SqlClient

Partial Class FocusRangeAssignment
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Response.Redirect("UnauthorizedBasic.aspx?HideHeader=true", True)
        Else
            Dim EnvironmentID As Integer = 0
            Dim focusRangeid As Guid = Guid.Empty
            Dim sopid As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), focusRangeid)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
                sopid = Request.QueryString("sopid").ToString()
            End If

            If Not IsPostBack Then
                If EnvironmentID > 0 AndAlso focusRangeid <> Guid.Empty Then
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_FOCUS_RANGE) Then
                        Response.Redirect("UnauthorizedBasic.aspx?HideHeader=true", True)
                    Else
                        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_FOCUS_RANGE) Then
                            DisableControls()
                            BasicAccessScript.Visible = True
                        Else
                            ManagementScript.Visible = True
                        End If
                        RenderRepeaters(EnvironmentID, sopid, focusRangeid)
                    End If
                End If
            Else
                Dim __EVENTTARGET As String = Request("__EVENTTARGET")
                Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
                If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                    If __EVENTTARGET.Contains("RefreshHD") Then
                        If __EVENTARGUMENT = "ConfirmAssignment" Then
                            RenderRepeaters(EnvironmentID, sopid, focusRangeid)
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "ConfirmAssignment();", True)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DisableControls()
        ManageAssignedTH.Visible = False
        ManageUnassignedTH.Visible = False
        managementTR1.Visible = False
        managementTR2.Visible = False
        managementTR3.Visible = False
        managementTR4.Visible = False
        managementTR5.Visible = False
        managementTR6.Visible = False
        managementTR7.Visible = False
    End Sub

    Private Sub RenderRepeaters(environmentID As Integer, sopId As String, focusRangeid As Guid)
        Dim customersDt As DataTable = ClsEbusinessHelper.GetCustomersForFocusRange(environmentID, sopId, focusRangeid)
        If customersDt IsNot Nothing Then
            Dim countAttached As Integer = customersDt.Select("IsAttached = 1").Count
            Dim countNotAttached As Integer = customersDt.Select("IsAttached = 0").Count
            assignedCount.InnerText = "(" + countAttached.ToString() + " customer" + IIf(countAttached <= 1, "", "s") + ")"
            unassignedCount.InnerText = "(" + countNotAttached.ToString() + " customer" + IIf(countNotAttached <= 1, "", "s") + ")"
            If countAttached > 0 Then
                ListView1.DataSource = customersDt.Select("IsAttached = 1").AsEnumerable().CopyToDataTable()
                ListView1.DataBind()
            Else
                ListView1.DataSource = New DataTable()
                ListView1.DataBind()
            End If
            If countNotAttached > 0 Then
                ListView2.DataSource = customersDt.Select("IsAttached = 0").AsEnumerable().CopyToDataTable()
                ListView2.DataBind()
            Else
                ListView2.DataSource = New DataTable()
                ListView2.DataBind()
            End If
            customersDt = Nothing
        End If
    End Sub



    Protected Function GetComplementaryInfo() As String
        Dim EnvironmentID As Integer = 0
        Dim focusRangeid As Guid = Guid.Empty
        Dim sopid As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            Guid.TryParse(Request.QueryString("id"), focusRangeid)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
            sopid = Request.QueryString("sopid").ToString()
        End If

        Return String.Format(", envid : {0} , sopid : '{1}', focusRangeId : '{2}'", EnvironmentID, sopid, focusRangeid.ToString())
    End Function

End Class
