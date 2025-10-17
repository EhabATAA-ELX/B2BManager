
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class AutomatedTestsStepManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim ElementID As Integer = 0
            Dim IsDeleteOperation As Boolean = False
            BindCommandsDropDownList()

            If Not String.IsNullOrEmpty(Request.QueryString("Op")) Then
                If Request.QueryString("op").ToLower() = "delete" Then
                    IsDeleteOperation = True
                End If
            End If

            Integer.TryParse(Request.QueryString("id"), ElementID)

            Dim ParentElementID As Integer = 0

            If ElementID > 0 Then
                Form.DefaultButton = btnSaveOrUpdateTestStep.UniqueID
                Dim testStep As ClsAutomatedTestsHelper.TestStep = ClsAutomatedTestsHelper.GetTestStepByID(ElementID)
                If testStep IsNot Nothing Then
                    If IsDeleteOperation Then
                        btnSaveOrUpdateTestStep.Text = "Delete"
                        btnSaveOrUpdateTestStep.OnClientClick = "ProcessButton(this,'Deleting...')"
                        btnSaveOrUpdateTestStep.CssClass = "btn red"
                        EditTable.Visible = False
                        lblDelete.Text = "Are you sure you want to delete this step?"
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Delete Test Step Confirmation"
                    Else
                        DeleteTable.Visible = False
                        If ddlCommandName.Items.Where(Function(fc) fc.Value = testStep.CommandID.ToString()).ToList().Count = 1 Then
                            ddlCommandName.SelectedValue = testStep.CommandID
                        End If
                        txtBoxDescription.Text = testStep.Description
                        txtBoxTarget.Text = testStep.Target
                        txtBoxValue.Text = testStep.Value
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Test Step " + testStep.Command
                    End If
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Test Case Not Found"
                    lblErrorMessageInfo.ForeColor = System.Drawing.Color.Red
                    lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
                    btnSaveOrUpdateTestStep.Visible = False
                    DeleteTable.Visible = False
                    EditTable.Visible = False
                End If
            Else
                Integer.TryParse(Request.QueryString("pcid"), ParentElementID)
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Test Step"
                DeleteTable.Visible = False
            End If
        End If
    End Sub

    Private Sub BindCommandsDropDownList()
        ddlCommandName.DataSource = GetStepCommands()
        ddlCommandName.DataValueField = "CommandID"
        ddlCommandName.DataTextField = "Command"
        ddlCommandName.DataBind()
    End Sub

    Private Function GetStepCommands() As List(Of ClsAutomatedTestsHelper.TestStepCommand)
        Dim listOfStepCommands As List(Of ClsAutomatedTestsHelper.TestStepCommand) = Nothing
        If Cache("AutomatedTests_Step_Commands") Is Nothing Then
            listOfStepCommands = ClsAutomatedTestsHelper.GetTestStepCommands()
            Cache.Insert("AutomatedTests_Step_Commands", listOfStepCommands)
        Else
            listOfStepCommands = DirectCast(Cache("AutomatedTests_Step_Commands"), List(Of ClsAutomatedTestsHelper.TestStepCommand))
        End If
        Return listOfStepCommands
    End Function

    Protected Sub btnSaveOrUpdateTestStep_Click(sender As Object, e As EventArgs)
        If ClsSessionHelper.LogonUser Is Nothing Then
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "RedirectToLoginPage", "RedirectToLoginPage();", True)
            Return
        End If
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Dim IsDeleteOperation As Boolean = False

        If Not String.IsNullOrEmpty(Request.QueryString("op")) Then
            If Request.QueryString("op").ToLower() = "delete" Then
                IsDeleteOperation = True
            End If
        End If
        Dim ElementID As Integer = 0, TestCaseID As Integer = 0
        Integer.TryParse(Request.QueryString("id"), ElementID)
        Integer.TryParse(Request.QueryString("tcid"), TestCaseID)
        Try
            lblErrorMessageInfo.Text = " "
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", ElementID))
            If Not IsDeleteOperation Then
                parameters.Add(New SqlParameter("@TestCaseID", TestCaseID))
                parameters.Add(New SqlParameter("@CommandID", Integer.Parse(ddlCommandName.SelectedValue)))
                parameters.Add(New SqlParameter("@Target", txtBoxTarget.Text))
                parameters.Add(New SqlParameter("@Value", txtBoxValue.Text))
                parameters.Add(New SqlParameter("@Description", txtBoxDescription.Text))
                Dim Position As Integer = 1, LinkedStepID As Integer = 0
                If Integer.TryParse(Request.QueryString("lnkStepID"), LinkedStepID) Then
                    parameters.Add(New SqlParameter("@LinkedStepID", LinkedStepID))
                    Integer.TryParse(Request.QueryString("pos"), Position)
                    If Position <= 1 And Position >= -1 Then
                        parameters.Add(New SqlParameter("@Position", Position))
                    End If
                End If
            End If
            parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
            If IsDeleteOperation Then
                ClsDataAccessHelper.ExecuteNonQuery("[AutomatedTests].DeleteTestStep", parameters)
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadTestCases", "CloseTestStepWindows();window.parent.LoadTestCases();", True)
                ClsHelper.Log("Delete Test Step", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step ID: " & ElementID, watch.ElapsedMilliseconds, False, Nothing)
            Else
                Dim resultedElementID As Integer = ClsDataAccessHelper.ExecuteScalar("[AutomatedTests].SaveOrUpdateTestStep", parameters)
                If (resultedElementID > 0) Then
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadTestCases", "CloseTestStepWindows();window.parent.LoadTestCases('" + resultedElementID.ToString() + "');", True)
                    ClsHelper.Log(IIf(ElementID > 0, "Edit Test Step", "Create Test Step"), ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step name: " & txtBoxDescription.Text + "</br>Test Step ID: " & resultedElementID, watch.ElapsedMilliseconds, False, Nothing)
                Else
                    lblErrorMessageInfo.ForeColor = System.Drawing.Color.Red
                    lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
                End If
            End If
        Catch ex As Exception
            lblErrorMessageInfo.ForeColor = System.Drawing.Color.Red
            lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            If IsDeleteOperation Then
                Dim errorMsg As String = String.Format("Unable to Delete Test Step</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
                ClsHelper.Log("Delete Test Step", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step ID: " & ElementID, watch.ElapsedMilliseconds, True, errorMsg)
            Else
                Dim errorMsg As String = String.Format("Unable to Save " + IIf(ElementID > 0, "Edit Test Step", "Create Test Step") + "</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
                ClsHelper.Log(IIf(ElementID > 0, "Edit Test Step", "Create Test Step"), ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step name: " & txtBoxDescription.Text + "</br>Test Step ID: " & ElementID, watch.ElapsedMilliseconds, True, errorMsg)
            End If

        End Try
        watch.Stop()
    End Sub

End Class
