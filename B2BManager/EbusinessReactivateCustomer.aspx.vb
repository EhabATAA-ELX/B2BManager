
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class EbusinessReactivateCustomer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CustomerDetails.Mode = "ReactivateCustomer"
        CType(Master.FindControl("title"), HtmlTitle).Text = "Reactivate customer"
        Dim target As String = Request("__EVENTTARGET")
        If Not IsPostBack Or "Refresh".Equals(target) Then
            Dim EnvironmentID As Integer = 0
            Dim cid As Guid = Guid.Empty
            If Not String.IsNullOrEmpty(Request.QueryString("envid")) Then
                Integer.TryParse(Request.QueryString("envid"), EnvironmentID)
            End If

            If Not String.IsNullOrEmpty(Request.QueryString("cid")) Then
                Guid.TryParse(Request.QueryString("cid"), cid)
            End If

            If EnvironmentID > 0 AndAlso cid <> Guid.Empty Then
                BindCustomerInformation(EnvironmentID, cid)
            End If

        End If
    End Sub

    Private Sub BindCustomerInformation(environmentID As Integer, cid As Guid)
        CType(CustomerDetails.FindControl("HD_EnvID"), HiddenField).Value = environmentID
        CType(CustomerDetails.FindControl("HD_C_GlobalID"), HiddenField).Value = cid.ToString()
        CType(CustomerDetails.FindControl("btnCancel"), LinkButton).Text = "<i class=""fas fa-ban""></i> Cancel"
        CType(CustomerDetails.FindControl("btnSubmit"), LinkButton).Text = "Reactivate"
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim customerActive As Boolean = False
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@CID", cid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetCustomerByID]", parameters)
        If dataSet.Tables.Count = 2 Then
            Dim customerDetailsDT As DataTable = dataSet.Tables(0)
            If customerDetailsDT.Rows.Count > 0 Then
                Dim companyRow As DataRow = customerDetailsDT.Rows(0)
                CType(CustomerDetails.FindControl("HD_SopName"), HiddenField).Value = ClsDataAccessHelper.GetText(companyRow, "S_SOP_ID")
                Dim C_MAX_LINES As Double = 999
                Dim C_SB_MIN_QTY As Double = 0
                Dim C_MAX_LINE_QTY As Double = 999
                Dim C_MIN_QTY As Double = 1
                Dim C_OVERRIDE_SAP_IMPORT As Boolean
                Double.TryParse(companyRow("C_MAX_LINES"), C_MAX_LINES)
                Double.TryParse(companyRow("C_SB_MIN_QTY"), C_SB_MIN_QTY)
                Double.TryParse(companyRow("C_MAX_LINE_QTY"), C_MAX_LINE_QTY)
                Double.TryParse(companyRow("C_MIN_QTY"), C_MIN_QTY)
                Boolean.TryParse(companyRow("C_OVERRIDE_SAP_IMPORT"), C_OVERRIDE_SAP_IMPORT)
                CType(CustomerDetails.FindControl("radNumericShoppingBasketMaxLines"), RadNumericTextBox).Value = C_MAX_LINES
                CType(CustomerDetails.FindControl("radNumericShoppingBasketTotalQuantity"), RadNumericTextBox).Value = C_SB_MIN_QTY
                CType(CustomerDetails.FindControl("radNumericSingleLineMaxQuantity"), RadNumericTextBox).Value = C_MAX_LINE_QTY
                CType(CustomerDetails.FindControl("radNumericSingleLineMinQuantity"), RadNumericTextBox).Value = C_MIN_QTY
                CType(CustomerDetails.FindControl("txtBoxCustomerName"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_NAME")
                CType(CustomerDetails.FindControl("chkBoxOverrideSapImport"), CheckBox).Checked = C_OVERRIDE_SAP_IMPORT
                CType(CustomerDetails.FindControl("txtBoxCustomerCode"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_CUID")
                CType(CustomerDetails.FindControl("txtBoxDescription"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_Description")
                CType(CustomerDetails.FindControl("txtBoxLinkedSoldToID"), TextBox).Text = ClsDataAccessHelper.GetText(companyRow, "C_CUSTOMER_SOLDTOID")
                Dim checkBoxList As CheckBoxList = CType(CustomerDetails.FindControl("customerRightsChbkoxList"), CheckBoxList)
                For Each item As ListItem In checkBoxList.Items
                    If companyRow(item.Value) IsNot DBNull.Value Then
                        checkBoxList.Items.FindByValue(item.Value).Selected = companyRow(item.Value)
                    End If
                Next
            End If
        End If
    End Sub


End Class
