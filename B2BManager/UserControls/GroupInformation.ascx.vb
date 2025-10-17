
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Drawing

Partial Class UserControls_GroupInformation
    Inherits System.Web.UI.UserControl

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim Valid As Boolean = True
        Dim ErrorMessage As String = String.Empty
        If String.IsNullOrWhiteSpace(txtBoxGroupName.Text) Then
            Valid = False
            ErrorMessage = "Group name is required"
        End If

        Dim Uid As Integer = 0
        If Not String.IsNullOrEmpty(Request.QueryString("uid")) Then
            Integer.TryParse(Request.QueryString("uid"), Uid)
        End If


        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If Valid Then
            parameters.Add(New SqlParameter("@U_LOGIN", txtBoxGroupName.Text))
            If Uid > 0 Then
                parameters.Add(New SqlParameter("@UID", Uid))
            End If
            Dim userWithSameLoginDt As DataTable = ClsDataAccessHelper.FillDataTable("[Administration].[CheckGroupName]", parameters)
            If userWithSameLoginDt.Rows.Count > 0 Then
                Valid = False
                ErrorMessage = "Group name alerady taken"
            End If
        End If

        If Not Valid Then
            lblInfo.Text = ErrorMessage
            lblInfo.ForeColor = Color.Red
            Exit Sub
        Else
            parameters = New List(Of SqlParameter)()
            If Uid > 0 Then
                parameters.Add(New SqlParameter("@UID", Uid))
            End If
            parameters.Add(New SqlParameter("@UserID", ClsSessionHelper.LogonUser.ID))
            parameters.Add(New SqlParameter("@U_LOGIN", txtBoxGroupName.Text))
            parameters.Add(New SqlParameter("@U_FIRSTNAME", txtBoxDescription.Text))
            parameters.Add(New SqlParameter("@COLOR", ColorTranslator.ToHtml(RadColorPickerGroup.SelectedColor)))
            Dim checkedGroups As List(Of String) = New List(Of String)()
            parameters.Add(New SqlParameter("@ToolsAndActions", String.Join(",", treeToolsAndActions.CheckedNodes.Select(Function(fc) fc.Value))))
            If (ClsDataAccessHelper.FillDataTable("Administration.AddOrUpdateGroup", parameters).Rows.Count = 1) Then
                lblInfo.ForeColor = Color.Green
                lblInfo.Text = "Submitted with success"
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "FinsihGroupSubmit();", True)
                ClsHelper.Log(IIf(Uid > 0, "Update", "Create") & " Group", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                lblInfo.ForeColor = Color.Red
                lblInfo.Text = "An unexpected error has occurred. please try again later."
                ClsHelper.Log(IIf(Uid > 0, "Update", "Create") & " Group", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred. please try again later.")
            End If

        End If
    End Sub
End Class
