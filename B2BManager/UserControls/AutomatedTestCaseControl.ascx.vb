
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Public Class UserControls_AutomatedTestCaseControl
    Inherits ClsTestCaseUserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim __EVENTTARGET As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        Dim selectedStepID As Integer = 0

        If TestCaseID <> 0 Then
            UC_hdfTestCaseID.Value = TestCaseID
        Else
            Integer.TryParse(UC_hdfTestCaseID.Value, TestCaseID)
        End If

        If IsPostBack Then
            If __EVENTTARGET = "UC_hdfTestCaseID" Then
                If Not String.IsNullOrEmpty(__EVENTARGUMENT) Then
                    Dim arguments As String() = __EVENTARGUMENT.Split("|")
                    selectedStepID = arguments(1)
                    If TestCaseID = 0 Then
                        TestCaseID = arguments(0)
                    End If
                End If
            Else
                If __EVENTARGUMENT IsNot Nothing Then
                    If __EVENTARGUMENT.StartsWith("Refresh") Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        If arguments.Length > 1 Then
                            selectedStepID = arguments(1)
                        End If
                    End If

                    If __EVENTARGUMENT.StartsWith("MoveStep") Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        If arguments.Length > 2 Then
                            selectedStepID = arguments(1)
                            MoveTestStep(TestCaseID, selectedStepID, arguments(2))
                        End If
                    End If
                End If
            End If
        End If

        If TestCaseID <> 0 Then
            Dim testCaseSteps As DataTable = GetStepsDataTable(TestCaseID)
            InitStepControls(testCaseSteps.Rows.Count > 0)
            Dim index As Integer = 0
            For Each dataRow As DataRow In testCaseSteps.Rows
                index = index + 1
                Dim stepType As StepType = StepType.MIDDLE
                If index = 1 Then
                    If index = testCaseSteps.Rows.Count Then
                        stepType = StepType.ONEITEM
                    Else
                        stepType = StepType.TOP
                    End If
                Else
                    If index = testCaseSteps.Rows.Count Then
                        stepType = StepType.BOTTOM
                    End If
                End If
                Dim testStep As ClsAutomatedTestsHelper.TestStep = Nothing
                ClsAutomatedTestsHelper.FillTestStepFromRow(testStep, dataRow)
                AddCommandStep(testStep.TestCaseID,
                               testStep.ID,
                               testStep.ExecutionOrder,
                               (selectedStepID = 0 And testStep.ExecutionOrder = 1) Or (selectedStepID > 0 And testStep.ID = selectedStepID),
                               stepType,
                               testStep.Command,
                               testStep.Target,
                               testStep.Value,
                               testStep.Description)
            Next
            btnNewStep.Attributes.Add("onclick", "NewTestStep(" + TestCaseID.ToString() + ",'Step')")
        Else
            btnNewStep.Visible = False
            btnLoadStepsFromFile.Visible = False
        End If
    End Sub

    Private Sub MoveTestStep(testCaseID As Integer, selectedStepID As Integer, direction As String)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", selectedStepID))
            parameters.Add(New SqlParameter("@MoveDirection", direction))
            parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
            ClsDataAccessHelper.ExecuteNonQuery("[AutomatedTests].MoveTestStep", parameters)
            ClsHelper.Log("Move Test Step", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step ID: " & selectedStepID, watch.ElapsedMilliseconds, False, Nothing)
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("Unable to move Test Step</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
            ClsHelper.Log("Move Test Step", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Step ID: " & selectedStepID, watch.ElapsedMilliseconds, True, errorMsg)
        End Try
    End Sub
    Protected Function GetStepsDataTable(_TestCaseID As Integer) As DataTable
        Return ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetStepsByTestCaseID]", New List(Of SqlClient.SqlParameter)(New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@TestCaseID", _TestCaseID)}))
    End Function

    Protected Enum StepType
        TOP = 0
        MIDDLE = 1
        BOTTOM = 2
        ONEITEM = 4
    End Enum

    Protected Sub InitStepControls(shown As Boolean)
        Dim displayStyle As String = IIf(shown, "visibility:visible;", "visibility:visible;display:none")
        btnEditStep.Attributes.Add("style", displayStyle)
        btnDeleteStep.Attributes.Add("style", displayStyle)
        btnMoveDownStep.Attributes.Add("style", displayStyle)
        btnMoveUpStep.Attributes.Add("style", displayStyle)
        btnRunAllSteps.Attributes.Add("style", displayStyle)
    End Sub

    Private Sub AddCommandStep(testCaseID As Integer, stepID As Integer, rowIndex As Integer, selected As Boolean, stepType As StepType, Optional command As String = "&nbsp;", Optional target As String = "&nbsp;", Optional value As String = "&nbsp;", Optional description As String = "&nbsp;")
        Dim dataRow As TableRow = New TableRow()
        dataRow.VerticalAlign = VerticalAlign.Middle
        dataRow.ID = "Step_" + stepID.ToString()
        dataRow.Attributes.Add("onclick", "SelectStep(this)")
        dataRow.Attributes.Add("data-test-case-id", testCaseID.ToString())
        dataRow.Attributes.Add("data-step-id", stepID.ToString())
        dataRow.Attributes.Add("data-step-type", stepType.ToString())
        dataRow.Attributes.Add("data-type", "Step")
        Dim selectionCell As TableCell = New TableCell()
        Dim selectionImage As Image = New Image()
        selectionImage.ImageUrl = "~/Images/AutomatedTests/rightArrow.png"
        selectionImage.Height = 22
        selectionImage.CssClass = "selected-img"
        selectionCell.Controls.Add(selectionImage)
        If selected Then
            dataRow.Attributes.Add("class", "selected-row")
        End If
        Dim IndexCell As TableCell = New TableCell()
        IndexCell.Text = rowIndex
        IndexCell.CssClass = "index"
        Dim CommandCell As TableCell = New TableCell()
        CommandCell.Text = command
        Dim TargetCell As TableCell = New TableCell()
        TargetCell.Text = target
        Dim ValueCell As TableCell = New TableCell()
        ValueCell.Text = value
        Dim DescriptionCell As TableCell = New TableCell()
        DescriptionCell.Text = description
        dataRow.Cells.Add(selectionCell)
        dataRow.Cells.Add(IndexCell)
        dataRow.Cells.Add(CommandCell)
        dataRow.Cells.Add(TargetCell)
        dataRow.Cells.Add(ValueCell)
        dataRow.Cells.Add(DescriptionCell)
        tableSteps.Rows.Add(dataRow)
        If selected Then
            btnMoveUpStep.Attributes.Add("style", IIf(stepType = StepType.MIDDLE Or stepType = StepType.BOTTOM, "visibility:visible;", "visibility:visible;display:none"))
            btnMoveDownStep.Attributes.Add("style", IIf(stepType = StepType.MIDDLE Or stepType = StepType.TOP, "visibility:visible;", "visibility:visible;display:none"))
            btnEditStep.Attributes.Add("onclick", "EditTestStep(" + testCaseID.ToString() + "," + stepID.ToString() + ",'Step')")
            btnDeleteStep.Attributes.Add("onclick", "DeleteTestStep(" + stepID.ToString() + ",'Step')")
            btnMoveUpStep.Attributes.Add("onclick", "MoveTestStep(" + stepID.ToString() + ",'UP','Step')")
            btnMoveDownStep.Attributes.Add("onclick", "MoveTestStep(" + stepID.ToString() + ",'Down','Step')")
        End If

    End Sub

End Class
