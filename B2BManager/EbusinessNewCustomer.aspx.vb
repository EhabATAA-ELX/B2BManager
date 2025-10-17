
Imports Telerik.Web.UI

Partial Class EbusinessNewCustomer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CType(Master.FindControl("title"), HtmlTitle).Text = "New customer"
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim sopID As String = String.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("sopid")) Then
                sopID = Request.QueryString("sopid")
            End If

            If EnvironmentID > 0 Then
                RenderControls(EnvironmentID, sopID)
            End If
        End If
    End Sub

    Private Sub RenderControls(environmentID As Integer, sopID As String)
        CType(CustomerDetails.FindControl("HD_SopName"), HiddenField).Value = sopID
        CType(CustomerDetails.FindControl("HD_EnvID"), HiddenField).Value = environmentID
        CType(CustomerDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Value = 999
        CType(CustomerDetails.FindControl("radNumericShoppingBasketTotalQuantity"), RadNumericTextBox).Value = 0
        CType(CustomerDetails.FindControl("radNumericSingleLineMaxQuantity"), RadNumericTextBox).Value = 999
        CType(CustomerDetails.FindControl("radNumericSingleLineMinQuantity"), RadNumericTextBox).Value = 0
        CType(CustomerDetails.FindControl("btnCancel"), LinkButton).Text = "<i class=""fas fa-ban""></i> Cancel"
        CType(CustomerDetails.FindControl("btnSubmit"), LinkButton).Text = "Submit"
        CustomerDetails.Mode = "CreateCustomer"
    End Sub
End Class
