
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Drawing

Partial Class ProfileCustomLink
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim linkID As Integer = 0
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
            Integer.TryParse(Request.QueryString("linkid"), linkID)
            If linkID > 0 Then
                Dim listOfLinks As List(Of ClsHelper.Link) = ClsHelper.GetLinks(logonUser.ID, linkID)
                If listOfLinks.Count = 1 Then
                    txtBoxLinkName.Text = listOfLinks(0).LinkName
                    txtBoxLinkUrl.Text = listOfLinks(0).LinkUrl
                    RadColorPickerGroup.SelectedColor = System.Drawing.ColorTranslator.FromHtml(listOfLinks(0).LinkIconColor)
                End If
                CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Link " + listOfLinks(0).LinkName
            Else
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Link"
            End If
        End If

        lblInfoMessage.Text = ""

    End Sub
    Protected Sub btnSaveOrUpdateLink_Click(sender As Object, e As EventArgs)
        Dim linkID As Integer = 0
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Integer.TryParse(Request.QueryString("linkid"), linkID)
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Try
            parameters.Add(New SqlParameter("@LinkID", linkID))
            parameters.Add(New SqlParameter("@Name", txtBoxLinkName.Text))
            parameters.Add(New SqlParameter("@Url", txtBoxLinkUrl.Text))
            parameters.Add(New SqlParameter("@LinkIconColor", ColorTranslator.ToHtml(RadColorPickerGroup.SelectedColor)))
            parameters.Add(New SqlParameter("@UserID", logonUser.ID))
            Dim resultedLinkID As Integer = ClsDataAccessHelper.ExecuteScalar("[Administration].SaveOrUpdateLink", parameters)
            If (resultedLinkID > 0) Then
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadLinks", "window.parent.LoadLinks(" & resultedLinkID & ");", True)
                logonUser.Links = ClsHelper.GetLinks(logonUser.ID)
                ClsHelper.Log(IIf(linkID > 0, "Edit Custom Link", "Add New Link"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                lblInfoMessage.ForeColor = System.Drawing.Color.Red
                lblInfoMessage.Text = "An unexpected error has occurred. please try again later."
                ClsHelper.Log(IIf(linkID > 0, "Edit Custom Link", "Add New Link"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, lblInfoMessage.Text)
            End If
        Catch ex As Exception
            lblInfoMessage.ForeColor = System.Drawing.Color.Red
            lblInfoMessage.Text = "An unexpected error has occurred. please try again later."
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("Unable to Save Dashboard</br><b>Excepetion Message:</b></br>{0}</br>" _
                + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                , exceptionStackTrace
                )
            ClsHelper.Log(IIf(linkID > 0, "Edit Custom Link", "Add New Link"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, errorMsg)
        End Try
    End Sub
End Class
