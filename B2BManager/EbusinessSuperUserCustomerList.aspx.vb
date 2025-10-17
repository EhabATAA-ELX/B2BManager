
Imports System.Data
Imports System.Data.SqlClient

Partial Class EbusinessSuperUserCustomerList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Response.Redirect("UnauthorizedBasic.aspx?HideHeader=true", True)
        Else
            Dim EnvironmentID As Integer = 0
            Dim uid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
                Guid.TryParse(Request.QueryString("uid"), uid)
            End If
            If Not IsPostBack Then
                If EnvironmentID > 0 AndAlso uid <> Guid.Empty Then
                    If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_CUSTOMER_LIST_TAB_IN_SUPER_USER_PROFILE) Then
                        Response.Redirect("UnauthorizedBasic.aspx?HideHeader=true", True)
                    Else
                        If Not String.IsNullOrEmpty(Request.QueryString("iscustomerdisplay")) Then
                            BindSuperUserInformation(EnvironmentID, uid)
                        End If
                        If Not clsUser.Actions.Contains(ClsHelper.ActionDesignation.MAINTAIN_SUPER_USER_CUSTOMER_LIST) Then
                            DisableControls()
                            BasicAccessScript.Visible = True
                        Else
                            ManagementScript.Visible = True
                        End If
                        RenderRepeaters(EnvironmentID, uid)
                    End If
                End If
            Else
                Dim __EVENTTARGET As String = Request("__EVENTTARGET")
                Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
                If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                    If __EVENTTARGET.Contains("RefreshHD") Then
                        If __EVENTARGUMENT = "ConfirmAssignment" Then
                            RenderRepeaters(EnvironmentID, uid)
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

    Private Sub RenderRepeaters(environmentID As Integer, uid As Guid)
        Dim customersDt As DataTable = ClsEbusinessHelper.GetCustomersForSuperUser(environmentID, uid)
        If customersDt IsNot Nothing Then
            Dim countAttached As Integer = customersDt.Select("IsAttached = 1").Count
            Dim countNotAttached As Integer = customersDt.Select("IsAttached = 0").Count
            assignedCount.InnerText = "(" + countAttached.ToString() + " customer" + IIf(countAttached <= 1, "", "s") + ")"
            unassignedCount.InnerText = "(" + countNotAttached.ToString() + " customer" + IIf(countNotAttached <= 1, "", "s") + ")"
            If countAttached > 0 Then
                ListView1.DataSource = customersDt.Select("IsAttached = 1").AsEnumerable().CopyToDataTable()
                ListView1.DataBind()
                ''''RemoveAllBtn.Enabled = True
                ''''RemoveAllBtn.CssClass = "btn red width180px"
            Else
                ListView1.DataSource = New DataTable()
                ListView1.DataBind()
                ''''RemoveAllBtn.Enabled = False
                ''''RemoveAllBtn.CssClass = "btn width180px btn-disabled"
            End If
            If countNotAttached > 0 Then
                ListView2.DataSource = customersDt.Select("IsAttached = 0").AsEnumerable().CopyToDataTable()
                ListView2.DataBind()
                ''''AssignAllBtn.Enabled = True
                ''''AssignAllBtn.CssClass = "btn lightblue width180px"
            Else
                ListView2.DataSource = New DataTable()
                ListView2.DataBind()
                ''''AssignAllBtn.Enabled = False
                ''''AssignAllBtn.CssClass = "btn width180px btn-disabled"
            End If
            ' Free up memory
            customersDt = Nothing
        End If
    End Sub

    Private Sub BindSuperUserInformation(environmentID As Integer, uid As Guid)
        Dim superUserInfoDt As DataTable = ClsEbusinessHelper.GetSuperUserInformation(environmentID, uid)
        If superUserInfoDt IsNot Nothing Then
            If superUserInfoDt.Rows.Count = 1 Then
                Dim superUserRow As DataRow = superUserInfoDt.Rows(0)
                lblLoginName.Text = ClsDataAccessHelper.GetText(superUserRow, "varName")
                lblDisplayName.Text = ClsDataAccessHelper.GetText(superUserRow, "varUserName")
                lblEmail.Text = ClsDataAccessHelper.GetText(superUserRow, "varEmail")
                superUserInformationPanel.Visible = True
                CType(Master.FindControl("title"), HtmlTitle).Text = "Super user customer list - " + ClsDataAccessHelper.GetText(superUserRow, "varUserName")
            End If
        End If
    End Sub

    Protected Function GetSuperUserInfo() As String
        Dim EnvironmentID As Integer = 0
        Dim uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), uid)
        End If

        Return String.Format(", envid : {0} , uid : '{1}'", EnvironmentID, uid.ToString())
    End Function
    Protected Sub AssignAllBtn_Click(sender As Object, e As EventArgs)
        ''''ManageCustomerList(True)
    End Sub

    Protected Sub RemoveAllBtn_Click(sender As Object, e As EventArgs)
        ''''ManageCustomerList(False)
    End Sub

    Private Sub ManageCustomerList(assignAll As Boolean)
        Dim EnvironmentID As Integer = 0
        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.GlobalID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@UID", Uid))
        parameters.Add(New SqlParameter("@OperationType", IIf(assignAll, 2, 3)))
        If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[SuperUsr_ManageSuperUserCustomerList]", parameters) Then
            RenderRepeaters(EnvironmentID, Uid)
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "CloseWindow", "ConfirmAssignment();", True)
        Else
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Alert", "ErrorPopup('Unable to update customer list, please try again later');", True)
        End If
    End Sub

End Class
