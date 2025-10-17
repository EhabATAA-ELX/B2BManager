
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Drawing

Partial Class UserControls_UserInformation
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            txtBoxPassword.Attributes.Add("type", "password")
            PopulateControls()
        End If
    End Sub

    Private Sub PopulateControls()
        Dim dataSet As DataSet = ClsUsersManagementHelper.GetGroupsDataSet()
        If dataSet IsNot Nothing Then
            If dataSet.Tables.Count = 3 Then
                For Each row As DataRow In dataSet.Tables(2).Rows
                    userGroupBoxList.Items.Add(New ListItem(row("U_LOGIN"), row("U_ID").ToString()))
                Next
            End If
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim Valid As Boolean = True
        Dim ErrorMessage As String = String.Empty
        If String.IsNullOrWhiteSpace(txtBoxLogin.Text) Then
            Valid = False
            ErrorMessage = "Login name is required"
        End If

        If String.IsNullOrWhiteSpace(txtBoxPassword.Text) Then
            If Not Valid Then
                ErrorMessage = "Login and password are required"
            Else
                Valid = False
                ErrorMessage = "Password is required"
            End If
        End If

        Dim Uid As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Guid.TryParse(Request.QueryString("uid"), Uid)
        End If


        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If Valid Then
            parameters.Add(New SqlParameter("@U_LOGIN", txtBoxLogin.Text))
            If Uid <> Guid.Empty Then
                parameters.Add(New SqlParameter("@UID", Uid))
            End If
            Dim userWithSameLoginDt As DataTable = ClsDataAccessHelper.FillDataTable("[Administration].[CheckLoginName]", parameters)
            If userWithSameLoginDt.Rows.Count > 0 Then
                Valid = False
                ErrorMessage = "Login alerady taken"
            End If
        End If

        If Not Valid Then
            lblInfo.Text = ErrorMessage
            lblInfo.ForeColor = Color.Red
            Exit Sub
        Else
            parameters = New List(Of SqlParameter)()
            If Uid <> Guid.Empty Then
                parameters.Add(New SqlParameter("@UID", Uid))
            End If
            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
            parameters.Add(New SqlParameter("@U_LOGIN", txtBoxLogin.Text))
            parameters.Add(New SqlParameter("@U_PASSWORD", txtBoxPassword.Text))
            parameters.Add(New SqlParameter("@U_FIRSTNAME", txtBoxFirstName.Text))
            parameters.Add(New SqlParameter("@U_LASTNAME", txtBoxLastName.Text))
            parameters.Add(New SqlParameter("@U_NICKNAME", txtBoxNickName.Text))
            parameters.Add(New SqlParameter("@U_ISACTIVE", chkBoxActive.Checked))
            parameters.Add(New SqlParameter("@U_EMAIL", txtBoxEmail.Text))
            parameters.Add(New SqlParameter("@EXPIRE_DATE_PASSWORD", radDateTimeExpirationDate.SelectedDate))
            parameters.Add(New SqlParameter("@AccessProductionEnvironment", chkBoxProductionAccess.Checked))
            parameters.Add(New SqlParameter("@AccessStagingEnvironment", chkBoxStagingAccess.Checked))
            Dim checkedGroups As List(Of String) = New List(Of String)()
            For Each item As ListItem In userGroupBoxList.Items
                If item.Selected Then
                    checkedGroups.Add(item.Value.ToString())
                End If
            Next
            parameters.Add(New SqlParameter("@GroupIDs", String.Join(",", checkedGroups)))
            parameters.Add(New SqlParameter("@SOPIDs", String.Join(",", treeCountries.CheckedNodes.Select(Function(fc) fc.Value))))
            If (ClsDataAccessHelper.FillDataTable("Administration.AddOrUpdateUser", parameters).Rows.Count = 1) Then
                lblInfo.ForeColor = Color.Green
                lblInfo.Text = "Submitted with success"
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "FinsihUserSubmit();", True)
                ClsHelper.Log(IIf(Uid <> Guid.Empty, "Update", "Create") & " User", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                lblInfo.ForeColor = Color.Red
                lblInfo.Text = "An unexpected error has occurred. please try again later."
                ClsHelper.Log(IIf(Uid <> Guid.Empty, "Update", "Create") & " User", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
            End If

        End If
    End Sub
End Class
