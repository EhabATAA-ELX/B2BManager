
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TP2ManagePriceSetting
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
                    CType(Master.FindControl("title"), HtmlTitle).Text = "Edit price setting"
                    btnSubmit.Text = "Update"
                    btnSubmit.OnClientClick = "ProcessButton('Update')"
                    LoadPriceData(EnvironmentID, id)
                Else
                    CType(Master.FindControl("title"), HtmlTitle).Text = "New price setting"
                    btnSubmit.Text = "Save"
                    btnSubmit.OnClientClick = "ProcessButton('Add')"
                    txtStepNumber.Value = 100
                End If

            End If

        End If
    End Sub

    Private Sub LoadPriceData(environmentID As Integer, id As Guid)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", environmentID))
        parameters.Add(New SqlParameter("@ID", id))
        Dim data As DataTable = ClsDataAccessHelper.FillDataTable("TP2PriceSettings.GetPriceSettingByID", parameters)
        If data IsNot Nothing Then
            If data.Rows.Count = 1 Then
                txtBoxSalesOrg.Text = data.Rows(0)("SALES_ORG")
                txtBoxSAPField.Text = data.Rows(0)("SAP_FIELD")
                txtBoxTP2Field.Text = data.Rows(0)("TP2_FIELD")
                If data.Rows(0)("STEPNUMBER") IsNot DBNull.Value Then
                    Dim stepNumber As Integer = -1
                    Integer.TryParse(data.Rows(0)("STEPNUMBER"), stepNumber)
                    If stepNumber > 0 Then
                        txtStepNumber.Value = stepNumber
                    End If
                End If
                txtBoxCode.Text = data.Rows(0)("CODE")
                ChkBoxAllowancesCharges.Checked = data.Rows(0)("ALLOWANCES_CHARGES")
                ChkBoxMonetoryAmount.Checked = data.Rows(0)("MONETARY_AMOUNT")
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
        parameters.Add(New SqlParameter("@SALES_ORG", txtBoxSalesOrg.Text))
        parameters.Add(New SqlParameter("@SAP_FIELD", txtBoxSAPField.Text))
        parameters.Add(New SqlParameter("@TP2_FIELD", txtBoxTP2Field.Text))
        parameters.Add(New SqlParameter("@STEPNUMBER", txtStepNumber.Value.ToString()))
        parameters.Add(New SqlParameter("@ALLOWANCES_CHARGES", ChkBoxAllowancesCharges.Checked))
        parameters.Add(New SqlParameter("@MONETARY_AMOUNT", ChkBoxMonetoryAmount.Checked))
        parameters.Add(New SqlParameter("@CODE", txtBoxCode.Text))
        Dim result As String = ClsDataAccessHelper.ExecuteScalar("TP2PriceSettings.SaveOrUpdatePriceSetting", parameters)
        Select Case result
            Case "Success"
                ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " TP2 Price Setting", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.Finish(" + IIf(id <> Guid.Empty, "'Update'", "'Save'") + ");", True)
            Case Nothing
                ClsHelper.Log(IIf(id <> Guid.Empty, "Update", "Save") + " TP2 Price Setting", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                lblErrorInfo.Text = "An unexpected error has occurred. Please try again later."
            Case Else
                ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "Confirm", "window.parent.ConfirmExistingMapping('" + result + "');", True)
        End Select
    End Sub
End Class
