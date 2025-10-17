
Partial Class AutomatedTestsRequestManager
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

            Integer.TryParse(Request.QueryString("id"), ElementID)

            Dim ParentElementID As Integer = 0

            If ElementID > 0 Then
                Form.DefaultButton = btnSaveOrUpdateTestStep.UniqueID
                Dim testRequest As ClsAutomatedTestsHelper.TestRequest = ClsAutomatedTestsHelper.GetRequestByID(ElementID)
                If testRequest IsNot Nothing Then
                    If IsDeleteOperation Then
                        btnSaveOrUpdateTestStep.Text = "Delete"
                        btnSaveOrUpdateTestStep.OnClientClick = "ProcessButton(this,'Deleting...')"
                        btnSaveOrUpdateTestStep.CssClass = "btn red"
                        lblDelete.Text = "Are you sure you want to delete this request?"
                        tableDescription.Visible = False
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Delete Request Confirmation"
                    Else
                        CType(Master.FindControl("title"), HtmlTitle).Text = "Edit Request"
                        LoadMessageRequesterControl(testRequest)
                        txtBoxRequestDescription.Text = testRequest.Description
                    End If
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Request Not Found"
                    lblErrorMessageInfo.ForeColor = System.Drawing.Color.Red
                    lblErrorMessageInfo.Text = "An unexpected error has occurred. please try again later."
                    btnSaveOrUpdateTestStep.Visible = False
                    tableDescription.Visible = False
                    DeleteTable.Visible = False
                End If
            Else
                Integer.TryParse(Request.QueryString("pcid"), ParentElementID)
                CType(Master.FindControl("title"), HtmlTitle).Text = "New Request"
                LoadMessageRequesterControl()
            End If
        End If

    End Sub
    Protected Sub LoadMessageRequesterControl(Optional testRequest As ClsAutomatedTestsHelper.TestRequest = Nothing)
        DeleteTable.Visible = False
        MessageRequesterControl.TestRequest = testRequest
        MessageRequesterControl.IsTestRequest = True
        MessageRequesterControl.Visible = True
    End Sub

End Class
