
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class SAPOrderTypeManagement
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim EnvironmentID As Integer = 0
            Dim id As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                Guid.TryParse(Request.QueryString("id"), id)
            End If

            If EnvironmentID > 0 Then
                If id <> Guid.Empty Then
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Edit SAP Order Type"
                    btnSubmit.Text = "Update"
                    btnSubmit.OnClientClick = "ProcessButton('Update')"
                    LoadPriceData(EnvironmentID, id)
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "New SAP Order Type"
                    btnSubmit.Text = "Save"
                    btnSubmit.OnClientClick = "ProcessButton('Add')"
                End If

            End If

        End If
    End Sub

    Private Sub LoadPriceData(environmentID As Integer, id As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@ID", id))
        Dim data As DataTable = ClsDataAccessHelper.FillDataTable("SAPOrderTypes.GetSAPOrderTypeByID", parameters)
        If data IsNot Nothing Then
            If data.Rows.Count = 1 Then
                txtBoxOrderType.Text = ClsDataAccessHelper.GetText(data.Rows(0), "B2B_Order_Type")
                txtBoxSAPSalesDocType.Text = ClsDataAccessHelper.GetText(data.Rows(0), "SAP_Sales_Doc_Type")
                txtBoxOrderReason.Text = ClsDataAccessHelper.GetText(data.Rows(0), "Order_Reason")
                txtBoxShippingCondition.Text = ClsDataAccessHelper.GetText(data.Rows(0), "Shipping_Condition")
            Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Close", "CloseWindow();", True)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        lblErrorInfo.Text = " "
        Dim EnvironmentID As Integer = 0
        Dim id As Guid = Guid.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
            Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
            Guid.TryParse(Request.QueryString("id"), id)
        End If
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        If id <> Guid.Empty Then
            parameters.Add(New SqlParameter("@ID", id))
        End If
        parameters.Add(New SqlParameter("@B2B_Order_Type", txtBoxOrderType.Text))
        parameters.Add(New SqlParameter("@SAP_Sales_Doc_Type", txtBoxSAPSalesDocType.Text))
        parameters.Add(New SqlParameter("@Order_Reason", txtBoxOrderReason.Text))
        parameters.Add(New SqlParameter("@Shipping_Condition", txtBoxShippingCondition.Text))
        Dim result As String = ClsDataAccessHelper.ExecuteScalar("SAPOrderTypes.SaveOrUpdateSAPOrderType", parameters)
        Select Case result
            Case "Success"
                ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " SAP Order Type", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.Finish(" + IIf(id <> Guid.Empty, "'Update'", "'Save'") + ");", True)
            Case Nothing
                ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " TSAP Order Type", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                lblErrorInfo.Text = "An unexpected error has occurred. Please try again later."
            Case Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.ConfirmExistingMapping('" + result + "');", True)
        End Select
    End Sub
End Class
