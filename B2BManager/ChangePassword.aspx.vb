
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class ChangePassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, args As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            txtBoxNewPassword.Attributes.Add("onkeyup", "onKeyUpNewPassowrd()")
            CType(Master.FindControl("title"), HtmlTitle).Text = "Change Password"
        End If
    End Sub
    Protected Sub CurrentPasswordValidator_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = txtBoxCurrentPassword.Text = ClsSessionHelper.LogonUser.Password
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        If (Page.IsValid) Then
            parameters.Add(New SqlParameter("@UserID", logonUser.GlobalID))
            parameters.Add(New SqlParameter("@Password", txtBoxNewPassword.Text))
            If ClsDataAccessHelper.ExecuteNonQuery("[Administration].[ChangePassword]", parameters) Then
                logonUser.Password = txtBoxNewPassword.Text
                ClsHelper.Log("Change Password", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, False, Nothing)
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "CloseWindow", "CloseWindow();", True)
            Else
                ClsHelper.Log("Change Password", logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
    End Sub
End Class
