
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Telerik.Web.UI

Partial Class AutomatedTestsElementManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            Dim ElementID As Integer = 0
            Dim IsDeleteOperation As Boolean = False

            If Not String.IsNullOrEmpty(Request.QueryString("Op")) Then
                If Request.QueryString("op").ToLower() = "delete" Then
                    IsDeleteOperation = True
                End If
            End If

            Integer.TryParse(Request.QueryString("cid"), ElementID)
            Dim Type As String = "Container"
            If Not String.IsNullOrEmpty(Request.QueryString("type")) Then
                If Request.QueryString("type").ToLower() = "testcase" Then
                    Type = "Test Case"
                End If
                If Request.QueryString("type").ToLower() = "loadtestwave" Then
                    Type = "Load Testing Wave"
                End If
            End If

            lblName.Text = Type & " Name:"

            Dim ParentElementID As Integer = 0

            If ElementID > 0 Then
                Form.DefaultButton = btnSaveOrUpdateContainer.UniqueID
                Dim testCase As ClsAutomatedTestsHelper.TestCase = ClsAutomatedTestsHelper.GetTestCaseByID(ElementID)
                If testCase IsNot Nothing Then
                    If IsDeleteOperation Then
                        btnSaveOrUpdateContainer.Text = "Delete"
                        btnSaveOrUpdateContainer.OnClientClick = "ProcessbtnSaveOrUpdateContainer(this,'Deleting...')"
                        btnSaveOrUpdateContainer.CssClass = "btn red"
                        EditTable.Visible = False
                        lblDelete.Text = "Are you sure you want to delete the " + Type.ToLower() + " '" + testCase.Name + "'" + (IIf(Type.ToLower() = "container", " and its sub-containers and child test cases", "")) + "?"
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Delete " & Type & " Confirmation"
                    Else
                        txtBoxName.Text = testCase.Name
                        txtBoxDescription.Text = testCase.Tooltip
                        If testCase.ParentID IsNot Nothing Then
                            ParentElementID = testCase.ParentID
                        End If
                        DeleteTable.Visible = False
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Edit " & Type & " " & testCase.Name
                    End If
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Test Case Not Found"
                    lblErrorMessageInfo.ForeColor = System.Drawing.Color.Red
                    lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
                    btnSaveOrUpdateContainer.Visible = False
                    DeleteTable.Visible = False
                    EditTable.Visible = False
                End If
            Else
                Integer.TryParse(Request.QueryString("pcid"), ParentElementID)
                CType(Master.FindControl("title"), HtmlTitle).Text = "New " & Type
                DeleteTable.Visible = False
            End If
            RenderParentContainerDropDown(ParentElementID)
        End If
    End Sub

    Private Sub RenderParentContainerDropDown(Optional selectedContainer As Integer = 0)
        Dim nodeItems = ClsAutomatedTestsHelper.GetTestCases().Where(Function(fc) fc.IsFolder).ToList()
        If nodeItems.Count > 0 Then
            For Each nodeItem As ClsAutomatedTestsHelper.TestCase In nodeItems
                Dim radComboItem As RadComboBoxItem = New RadComboBoxItem(nodeItem.Name, nodeItem.ID)
                If nodeItem.ID = selectedContainer Then
                    radComboItem.Selected = True
                End If
                ddlParentContainer.Items.Add(radComboItem)
            Next
        End If
    End Sub

    Protected Sub btnSaveOrUpdateContainer_Click(sender As Object, e As EventArgs)
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
        Dim ElementID As Integer = 0
        Dim itemType As Integer = 1
        Integer.TryParse(Request.QueryString("cid"), ElementID)
        Dim isFolder As Boolean = True
        Dim Type As String = "Container"
        If Not String.IsNullOrEmpty(Request.QueryString("type")) Then
            If Request.QueryString("type").ToLower() = "testcase" Then
                isFolder = False
                Type = "Test Case"
                itemType = 2
            End If
            If Request.QueryString("type").ToLower() = "loadtestwave" Then
                isFolder = False
                Type = "Load Testing Wave"
                itemType = 3
            End If
        End If
        Try
            lblErrorMessageInfo.Text = " "
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", ElementID))
            If Not IsDeleteOperation Then
                parameters.Add(New SqlParameter("@ParentID", ddlParentContainer.SelectedValue))
                parameters.Add(New SqlParameter("@ItemType", itemType))
                parameters.Add(New SqlParameter("@IsFolder", isFolder))
                parameters.Add(New SqlParameter("@Name", txtBoxName.Text))
                parameters.Add(New SqlParameter("@Description", txtBoxDescription.Text))
            End If
            parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
            If IsDeleteOperation Then
                If ClsDataAccessHelper.ExecuteNonQuery("[AutomatedTests].Delete" + Type.Replace(" ", ""), parameters) Then
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadTestCases", "window.parent.LoadTestCases();", True)
                    ClsHelper.Log("Delete " + Type, ClsSessionHelper.LogonUser.GlobalID.ToString(), Type + " ID: " & ElementID, watch.ElapsedMilliseconds, False, Nothing)
                Else
                    Throw New Exception("Unable to perform deletion")
                End If
            Else
                Dim resultedElementID As Integer = ClsDataAccessHelper.ExecuteScalar("[AutomatedTests].SaveOrUpdateTestCase", parameters)
                If (resultedElementID > 0) Then
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadTestCases", "window.parent.LoadTestCases();", True)
                    ClsHelper.Log(IIf(ElementID > 0, "Edit " + Type, "Create " + Type), ClsSessionHelper.LogonUser.GlobalID.ToString(), Type + " name: " & txtBoxDescription.Text + "</br>" + Type + " ID: " & resultedElementID, watch.ElapsedMilliseconds, False, Nothing)
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
                Dim errorMsg As String = String.Format("Unable to Delete " + Type + "</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
                ClsHelper.Log("Delete " + Type, ClsSessionHelper.LogonUser.GlobalID.ToString(), Type + " ID: " & ElementID, watch.ElapsedMilliseconds, True, errorMsg)
            Else
                Dim errorMsg As String = String.Format("Unable to Save " + IIf(ElementID > 0, "Edit " + Type, "Create " + Type) + "</br><b>Excepetion Message:</b></br>{0}</br>" _
                                        + "<b>Exception Stack Trace:</b></br>{1}", exceptionMessage _
                                        , exceptionStackTrace
                                        )
                ClsHelper.Log(IIf(ElementID > 0, "Edit " + Type, "Create " + Type), ClsSessionHelper.LogonUser.GlobalID.ToString(), Type + " name: " & txtBoxDescription.Text + "</br>" + Type + " ID: " & ElementID, watch.ElapsedMilliseconds, True, errorMsg)
            End If

        End Try
        watch.Stop()
    End Sub
End Class
