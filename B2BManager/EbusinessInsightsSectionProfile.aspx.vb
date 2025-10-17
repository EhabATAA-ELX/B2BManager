
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class EbusinessInsightsSectionProfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim sectionID As Integer = 0
            Integer.TryParse(Request.QueryString("sid"), sectionID)
            If sectionID > 0 Then
                Dim area As ClsInsightsHelper.Area = ClsInsightsHelper.GetAreaByID(sectionID, ClsSessionHelper.LogonUser.GlobalID)
                Dim editable As Boolean = False
                If area IsNot Nothing Then
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Section " & area.Name
                    editable = area.Editable
                    txtBoxTooltipText.Text = area.TooltipText
                    txtBoxSectionName.Text = area.Name
                    ddlShowTooltip.SelectedValue = area.ShowHelpTooltip.ToString()
                End If

                If Not editable Then
                    btnSaveOrUpdateSection.Visible = False
                    txtBoxSectionName.Enabled = False
                    txtBoxTooltipText.Enabled = False
                    ddlShowTooltip.Enabled = False
                End If
            Else
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Section"
            End If
        End If

        lblInfoMessage.Text = ""

    End Sub
    Protected Sub btnSaveOrUpdateSection_Click(sender As Object, e As EventArgs)
        Dim sectionID As Integer = 0
        Integer.TryParse(Request.QueryString("sid"), sectionID)
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Try
            parameters.Add(New SqlParameter("@AreaID", sectionID))
            parameters.Add(New SqlParameter("@Name", txtBoxSectionName.Text))
            parameters.Add(New SqlParameter("@TooltipText", txtBoxTooltipText.Text))
            parameters.Add(New SqlParameter("@ShowToolTip", CBool(ddlShowTooltip.SelectedValue)))
            parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
            Dim resultedSectionID As Integer = ClsDataAccessHelper.ExecuteScalar("Insights.SaveOrUpdateSection", parameters)
            If (resultedSectionID > 0) Then
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadDashboard", "window.parent.LoadSections(" & resultedSectionID & ");", True)
                ClsHelper.Log(IIf(sectionID > 0, "Edit Section", "Create Section"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                lblInfoMessage.ForeColor = System.Drawing.Color.Red
                lblInfoMessage.Text = "An unexpected error has occurred. please try again later."
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
            ClsHelper.Log(IIf(sectionID > 0, "Edit Section", "Create Section"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, errorMsg)
        End Try
    End Sub
End Class
