
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class UserControls_CustomerDetails
    Inherits System.Web.UI.UserControl

    Public _Mode As String
    Public Property Mode As String
        Get
            Return _Mode
        End Get
        Set(ByVal value As String)
            _Mode = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        chkBoxOverrideSapImport.InputAttributes("class") = "override-sap-import"
    End Sub

    Protected Sub btnSubmit_ServerClick(sender As Object, e As EventArgs)
        If Me.Mode = "UpdateCustomer" Or Me.Mode = "ReactivateCustomer" Then
            UpdateCurrentCustomer()
        Else
            Dim DeletedCompany = IFDeletedCompany()
            If DeletedCompany <> "" Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ReloadCompanyDeleted", "ReloadCompanyDeleted('" + txtBoxCustomerCode.Text + "','" + DeletedCompany.ToString() + "');", True)
            Else
                SetNewCustomer()
            End If

        End If
    End Sub

    Private Sub UpdateCurrentCustomer()
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim runWithSuccess As Boolean = False
        If logonUser IsNot Nothing Then
            Try
                parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvID.Value))
                parameters.Add(New SqlParameter("@C_GLOBALID", HD_C_GlobalID.Value))
                parameters.Add(New SqlParameter("@C_CUID", txtBoxCustomerCode.Text))
                parameters.Add(New SqlParameter("@C_NAME", txtBoxCustomerName.Text))
                parameters.Add(New SqlParameter("@C_OVERRIDE_SAP_IMPORT", IIf(chkBoxOverrideSapImport.Checked, 1, 0)))
                parameters.Add(New SqlParameter("@C_MAX_LINE_QTY", radNumericSingleLineMaxQuantity.Text))
                parameters.Add(New SqlParameter("@C_MAX_LINES", radNumericShoppingBasketMaxLines.Text))
                parameters.Add(New SqlParameter("@C_MIN_QTY", radNumericSingleLineMinQuantity.Text))
                parameters.Add(New SqlParameter("@C_SB_MIN_QTY", radNumericShoppingBasketTotalQuantity.Text))
                parameters.Add(New SqlParameter("@C_DESCRIPTION", txtBoxDescription.Text))
                parameters.Add(New SqlParameter("@C_HARDSWITCH", IIf(GetValuefromList("C_HARDSWITCH"), 1, 0)))
                parameters.Add(New SqlParameter("@C_DEFAULT_MENU", ddlDefaultMenu.SelectedValue))
                parameters.Add(New SqlParameter("@C_FOCUS_RANGE", IIf(GetValuefromList("C_FOCUS_RANGE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_CMIR", IIf(GetValuefromList("C_CMIR"), 1, 0)))
                parameters.Add(New SqlParameter("@C_CUSTOMER_SOLDTOID", txtBoxLinkedSoldToID.Text))
                parameters.Add(New SqlParameter("@C_BROKEN_PROMISE", IIf(GetValuefromList("C_BROKEN_PROMISE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PRICE_SCALES", IIf(GetValuefromList("C_PRICE_SCALES"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PRICE_DISCOUNT", IIf(GetValuefromList("C_PRICE_DISCOUNT"), 1, 0)))
                parameters.Add(New SqlParameter("@C_EXPECTED_PRICE", IIf(GetValuefromList("C_EXPECTED_PRICE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_OtherPartners", IIf(GetValuefromList("C_OtherPartners"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PASSAvailability", IIf(GetValuefromList("C_PASSAvailability"), 1, 0)))
                parameters.Add(New SqlParameter("@C_DeliveryBlock", TxtBoxDeliveryBlock.Text))


                Dim outputResult As Integer = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_UpdateCustomer]", parameters)
                Select Case outputResult
                    Case 0
                        If Me.Mode = "UpdateCustomer" Then
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('SubmitUpdate');", True)
                        Else
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('SubmitCreate');", True)
                        End If
                        runWithSuccess = True
                    Case -1
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", String.Format("ErrorPopup('Customer code {0} is aleady taken by another customer');", txtBoxCustomerCode.Text), True)
                        errorMsg = String.Format("Customer code {0} is aleady taken by another customer", txtBoxCustomerCode.Text)
                    Case Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Customer update failed');", True)
                        errorMsg = "Customer update failed"
                End Select
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('An unexpected error has occurred, please try again later');", True)
                Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                If Not ex.Message Is Nothing Then
                    exceptionMessage = ex.Message
                End If
                If Not ex.StackTrace Is Nothing Then
                    exceptionStackTrace = ex.StackTrace
                End If
                errorMsg = String.Format("<b>Methode Name:</b>UpdateCurrentCustomer</br><b>Excepetion Message:</b></br>{0}</br>" _
                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                    , exceptionStackTrace)
            End Try

            ClsHelper.Log(IIf(Me.Mode = "UpdateCustomer", "Update Customer", "Create Customer"), logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
            ClsHelper.LogEbusinessAction(CInt(HD_EnvID.Value), HD_SopName.Value, IIf(Me.Mode = "UpdateCustomer", "Update Customer", "Create Customer"), logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Your session has expired please login again');", True)
        End If
    End Sub

    Private Sub SetNewCustomer()
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()
        Dim errorMsg As String = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim runWithSuccess As Boolean = False
        If logonUser IsNot Nothing Then
            Try
                parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvID.Value))
                parameters.Add(New SqlParameter("@SOPNAME", HD_SopName.Value))
                parameters.Add(New SqlParameter("@C_CUID", txtBoxCustomerCode.Text))
                parameters.Add(New SqlParameter("@C_NAME", txtBoxCustomerName.Text))
                parameters.Add(New SqlParameter("@C_OVERRIDE_SAP_IMPORT", IIf(chkBoxOverrideSapImport.Checked, 1, 0)))
                parameters.Add(New SqlParameter("@C_DESCRIPTION", txtBoxDescription.Text))
                parameters.Add(New SqlParameter("@C_CUSTOMER_SOLDTOID", txtBoxLinkedSoldToID.Text))
                parameters.Add(New SqlParameter("@C_DEFAULT_MENU", ddlDefaultMenu.SelectedValue))
                parameters.Add(New SqlParameter("@C_MAX_LINE_QTY", radNumericSingleLineMaxQuantity.Text))
                parameters.Add(New SqlParameter("@C_MAX_LINES", radNumericShoppingBasketMaxLines.Text))
                parameters.Add(New SqlParameter("@C_MIN_QTY", radNumericSingleLineMinQuantity.Text))
                parameters.Add(New SqlParameter("@C_SB_MIN_QTY", radNumericShoppingBasketTotalQuantity.Text))
                parameters.Add(New SqlParameter("@C_HARDSWITCH", IIf(GetValuefromList("C_HARDSWITCH"), 1, 0)))
                parameters.Add(New SqlParameter("@C_FOCUS_RANGE", IIf(GetValuefromList("C_FOCUS_RANGE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_CMIR", IIf(GetValuefromList("C_CMIR"), 1, 0)))
                parameters.Add(New SqlParameter("@C_BROKEN_PROMISE", IIf(GetValuefromList("C_BROKEN_PROMISE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PRICE_SCALES", IIf(GetValuefromList("C_PRICE_SCALES"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PRICE_DISCOUNT", IIf(GetValuefromList("C_PRICE_DISCOUNT"), 1, 0)))
                parameters.Add(New SqlParameter("@C_EXPECTED_PRICE", IIf(GetValuefromList("C_EXPECTED_PRICE"), 1, 0)))
                parameters.Add(New SqlParameter("@C_OtherPartners", IIf(GetValuefromList("C_OtherPartners"), 1, 0)))
                parameters.Add(New SqlParameter("@C_PASSAvailability", IIf(GetValuefromList("C_PASSAvailability"), 1, 0)))
                parameters.Add(New SqlParameter("@C_DeliveryBlock", TxtBoxDeliveryBlock.Text))

                Dim outputResult As Integer = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[UsrMgmt_AddCompany]", parameters)
                Select Case outputResult
                    Case 0
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "CloseWindow", "CloseWindow('SubmitCreate');", True)
                        runWithSuccess = True
                    Case -1
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", String.Format("ErrorPopup('Customer code {0} is aleady taken by another customer');", txtBoxCustomerCode.Text), True)
                        errorMsg = String.Format("Customer code {0} is aleady taken by another customer", txtBoxCustomerCode.Text)
                    Case Else
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Unable to create the customer');", True)
                        errorMsg = "Unable to create the customer"
                End Select
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('An unexpected error has occurred, please try again later');", True)
                Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                If Not ex.Message Is Nothing Then
                    exceptionMessage = ex.Message
                End If
                If Not ex.StackTrace Is Nothing Then
                    exceptionStackTrace = ex.StackTrace
                End If
                errorMsg = String.Format("<b>Methode Name:</b>SetNewCustomer</br><b>Excepetion Message:</b></br>{0}</br>" _
                + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                , exceptionStackTrace)
            End Try
            ClsHelper.Log(IIf(Me.Mode = "UpdateCustomer", "Update Customer", "Create Customer"), logonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
            ClsHelper.LogEbusinessAction(CInt(HD_EnvID.Value), HD_SopName.Value, IIf(Me.Mode = "UpdateCustomer", "Update Customer", "Create Customer"), logonUser.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), stopWatch.ElapsedMilliseconds, Not runWithSuccess, errorMsg)
        Else
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Alert", "ErrorPopup('Your session has expired please login again');", True)
        End If
    End Sub

    Private Function IFDeletedCompany() As String
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", HD_EnvID.Value))
        parameters.Add(New SqlParameter("@SOPNAME", HD_SopName.Value))
        parameters.Add(New SqlParameter("@C_CUID", txtBoxCustomerCode.Text))
        Dim ds As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GET_COMPANYDELETED]", parameters)
        If ds IsNot Nothing AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0).Rows(0).Item("C_GLOBALID").ToString()
        Else
            Return ""
        End If
    End Function

    Private Function GetValuefromList(ByVal Right As String) As Boolean
        Dim selectedValues As List(Of String) = customerRightsChbkoxList.Items.Cast(Of ListItem)().Where(Function(li) li.Selected).[Select](Function(li) li.Value).ToList()
        If selectedValues.Count > 0 AndAlso selectedValues.Contains(Right) Then
            Return True
        End If
        Return False
    End Function

End Class
