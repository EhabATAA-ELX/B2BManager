
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class UserControls_AutomatedLoadingTestWaveControl
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

                    If __EVENTARGUMENT.StartsWith("MoveRequest") Then
                        Dim arguments As String() = __EVENTARGUMENT.Split("|")
                        If arguments.Length > 2 Then
                            selectedStepID = arguments(1)
                            MoveRequest(TestCaseID, selectedStepID, arguments(2))
                        End If
                    End If
                End If
            End If
        End If

        If TestCaseID <> 0 Then
            Dim testCaseRequests As DataTable = GetRequestsDataTable(TestCaseID)
            InitStepControls(testCaseRequests.Rows.Count > 0)
            Dim index As Integer = 0
            For Each dataRow As DataRow In testCaseRequests.Rows
                index = index + 1
                Dim stepType As StepType = StepType.MIDDLE
                If index = 1 Then
                    If index = testCaseRequests.Rows.Count Then
                        stepType = StepType.ONEITEM
                    Else
                        stepType = StepType.TOP
                    End If
                Else
                    If index = testCaseRequests.Rows.Count Then
                        stepType = StepType.BOTTOM
                    End If
                End If
                Dim testRequest As ClsAutomatedTestsHelper.TestRequest = Nothing
                ClsAutomatedTestsHelper.FillTestRequestFromRow(testRequest, dataRow)
                AddRequestStep(testRequest.TestCaseID,
                               testRequest.ID,
                               testRequest.ExecutionOrder,
                               (selectedStepID = 0 And testRequest.ExecutionOrder = 1) Or (selectedStepID > 0 And testRequest.ID = selectedStepID),
                               stepType,
                               testRequest.Description,
                               testRequest.CustomerCode,
                               testRequest.Sop,
                               testRequest.MessageType,
                               testRequest.Environment,
                               testRequest.TotalItems)
            Next
            btnNewRequest.Attributes.Add("onclick", "NewTestStep(" + TestCaseID.ToString() + ",'Request')")
        Else
            btnNewRequest.Visible = False
            btnRequestsGenerator.Visible = False
        End If
    End Sub

    Private Sub MoveRequest(testCaseID As Integer, selectedStepID As Integer, direction As String)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", selectedStepID))
            parameters.Add(New SqlParameter("@MoveDirection", direction))
            parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
            ClsDataAccessHelper.ExecuteNonQuery("[AutomatedTests].MoveRequest", parameters)
            ClsHelper.Log("Move Request", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Request ID: " & selectedStepID, watch.ElapsedMilliseconds, False, Nothing)
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("Unable to move request</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
            ClsHelper.Log("Move Request Step", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Test Request ID: " & selectedStepID, watch.ElapsedMilliseconds, True, errorMsg)
        End Try
    End Sub
    Protected Function GetRequestsDataTable(_TestCaseID As Integer) As DataTable
        Return ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetRequestsByTestCaseID]", New List(Of SqlParameter)(New SqlParameter() {New SqlParameter("@TestCaseID", _TestCaseID)}))
    End Function

    Protected Enum StepType
        TOP = 0
        MIDDLE = 1
        BOTTOM = 2
        ONEITEM = 4
    End Enum

    Protected Sub InitStepControls(shown As Boolean)
        Dim displayStyle As String = IIf(shown, "visibility:visible;", "visibility:visible;display:none")
        btnEditRequest.Attributes.Add("style", displayStyle)
        btnDeleteRequest.Attributes.Add("style", displayStyle)
        btnMoveDownRequest.Attributes.Add("style", displayStyle)
        btnMoveUpRequest.Attributes.Add("style", displayStyle)
        btnRunInsequence.Attributes.Add("style", displayStyle)
        btnRunInParallel.Attributes.Add("style", displayStyle)
    End Sub

    Private Sub AddRequestStep(testCaseID As Integer, stepID As Integer, rowIndex As Integer, selected As Boolean, stepType As StepType, Optional description As String = "&nbsp;", Optional customercode As String = "&nbsp;", Optional sop As String = "&nbsp;", Optional messagetype As String = "&nbsp;", Optional environment As String = "&nbsp;", Optional totalItems As Integer = 0)
        Dim dataRow As TableRow = New TableRow()
        dataRow.VerticalAlign = VerticalAlign.Middle
        dataRow.ID = "Step_" + stepID.ToString()
        dataRow.Attributes.Add("onclick", "SelectStep(this)")
        dataRow.Attributes.Add("data-test-case-id", testCaseID.ToString())
        dataRow.Attributes.Add("data-step-id", stepID.ToString())
        dataRow.Attributes.Add("data-step-type", stepType.ToString())
        dataRow.Attributes.Add("data-type", "Request")
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
        Dim ViewXMLCell As TableCell = New TableCell()
        Dim viewXMLImg As Image = New Image()
        viewXMLImg.ImageUrl = "~/Images/XML.png"
        viewXMLImg.Height = 22
        viewXMLImg.Attributes.Add("onclick", "OpenViewXMLFileWindow(" + stepID.ToString() + ",'1')")
        viewXMLImg.Attributes.Add("title", "View XML Message")
        ViewXMLCell.Controls.Add(viewXMLImg)
        ViewXMLCell.Attributes.Add("style", "text-align:center;cursor:pointer")
        Dim CountryCell As TableCell = New TableCell()
        CountryCell.Text = sop
        Dim EnvironmentCell As TableCell = New TableCell()
        EnvironmentCell.Text = environment
        Dim MsgTypeCell As TableCell = New TableCell()
        MsgTypeCell.Text = messagetype
        Dim CustomerCodeCell As TableCell = New TableCell()
        CustomerCodeCell.Text = customercode
        Dim TotalItemsCell As TableCell = New TableCell()
        TotalItemsCell.Text = totalItems
        Dim DescriptionCell As TableCell = New TableCell()
        DescriptionCell.Text = description
        dataRow.Cells.Add(selectionCell)
        dataRow.Cells.Add(IndexCell)
        dataRow.Cells.Add(ViewXMLCell)
        dataRow.Cells.Add(CountryCell)
        dataRow.Cells.Add(EnvironmentCell)
        dataRow.Cells.Add(MsgTypeCell)
        dataRow.Cells.Add(CustomerCodeCell)
        dataRow.Cells.Add(TotalItemsCell)
        dataRow.Cells.Add(DescriptionCell)
        tableSteps.Rows.Add(dataRow)
        If selected Then
            btnMoveUpRequest.Attributes.Add("style", IIf(stepType = StepType.MIDDLE Or stepType = StepType.BOTTOM, "visibility:visible;", "visibility:visible;display:none"))
            btnMoveDownRequest.Attributes.Add("style", IIf(stepType = StepType.MIDDLE Or stepType = StepType.TOP, "visibility:visible;", "visibility:visible;display:none"))
            btnEditRequest.Attributes.Add("onclick", "EditTestStep(" + testCaseID.ToString() + "," + stepID.ToString() + ",'Request')")
            btnDeleteRequest.Attributes.Add("onclick", "DeleteTestStep(" + stepID.ToString() + ",'Request')")
            btnMoveUpRequest.Attributes.Add("onclick", "MoveTestStep(" + stepID.ToString() + ",'UP','Request')")
            btnMoveDownRequest.Attributes.Add("onclick", "MoveTestStep(" + stepID.ToString() + ",'Down','Request')")
        End If

    End Sub

End Class
