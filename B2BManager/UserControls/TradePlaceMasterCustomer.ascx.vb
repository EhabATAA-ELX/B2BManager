
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class UserControls_TradePlaceMasterCustomer
    Inherits System.Web.UI.UserControl

    Public Sub LoadContext()
        RenderCountryDropDown()
        RenderEnvironnementDropDown()
        PanelContext.Visible = True
        Refresh()
    End Sub

    Private Sub LoadTPMasterCustomers(Optional ByVal TPCustomerId As String = "", Optional ByVal ContextID As String = "", Optional ByVal TPCName As String = "")

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TP3_MasterCustomersFiltered", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        If ContextID <> "" Then
            cmd.Parameters.AddWithValue("@CY_ISOCODE", ContextID)
        End If

        If TPCName <> "" Then
            cmd.Parameters.AddWithValue("@CustomerName", TPCName)
        End If

        MyDataReader = cmd.ExecuteReader()

        dtlTPMasterCustomer.DataSource = MyDataReader
        dtlTPMasterCustomer.DataBind()

        If (TPCustomerId <> "") Then
            ' this foreach loop is for select the right line after a save.
            ' the line will be in bold.
            For Each dtlItem In dtlTPMasterCustomer.Items
                If CType(dtlItem.Controls(1), System.Web.UI.HtmlControls.HtmlInputHidden).Value = TPCustomerId Then
                    dtlTPMasterCustomer.SelectedIndex = dtlItem.ItemIndex
                    Exit For
                End If
            Next
        End If

        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
    End Sub

    Public Sub Refresh(Optional ByVal TPCustomerId As String = "")
        LoadTPMasterCustomers(TPCustomerId, ContextID:=ddlCountry.SelectedValue)
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub lstActiveProfile_SelectedIndexChanged(sender As Object, e As EventArgs)
        Refresh()
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        SearchCustomer.Text = ""
        searchPanel.Update()
        Refresh()
    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(sender As Object, e As EventArgs)
        SearchCustomer.Text = ""
        searchPanel.Update()
        Refresh()
    End Sub

    Public Function GetLstActiveProfileSelectedIndex() As String
        Return ddlCountry.SelectedValue
    End Function

    Protected Sub SearchCustomer_TextChanged(sender As Object, e As EventArgs)
        Dim selectedValue = ""
        If (ddlCountry.SelectedValue <> "") Then
            selectedValue = ddlCountry.SelectedValue
        End If
        LoadTPMasterCustomers(ContextID:=selectedValue, TPCName:=SearchCustomer.Text)
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Public Function GetddlEnvironmentSelectedValue() As Integer
        Return ddlEnvironment.SelectedValue
    End Function

    Private Sub RenderCountryDropDown()
        Dim activeApplications As List(Of ClsHelper.BasicModel) = New List(Of ClsHelper.BasicModel)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If

        For Each application As ClsHelper.Application In applications
            If application.Checked = True Then
                activeApplications.Add(application)
            End If
        Next

        Dim selectedApplication = ClsHelper.FindApplicationByID(applications, 10)

        Dim SelectedValue As String = ddlCountry.SelectedValue
        ddlCountry.Items.Clear()
        Dim ValueExists As Boolean = False
        For Each country As ClsHelper.Country In selectedApplication.Countries
            If country.Checked Then
                Dim item As RadComboBoxItem = New RadComboBoxItem(country.Name, country.CY_NAME_ISOCODE)
                item.ImageUrl = country.ImageURL
                ddlCountry.Items.Add(item)
                If country.SOP_ID = SelectedValue Then
                    ValueExists = True
                    ddlCountry.SelectedValue = item.Value
                End If
            End If
        Next

    End Sub

    Private Sub RenderEnvironnementDropDown()
        Dim activeApplications As List(Of ClsHelper.BasicModel) = New List(Of ClsHelper.BasicModel)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            applications = ClsSessionHelper.LogonUser.Applications
        End If

        For Each application As ClsHelper.Application In applications
            If application.Checked = True Then
                activeApplications.Add(application)
            End If
        Next

        Dim selectedApplication = ClsHelper.FindApplicationByID(applications, 10)

        Dim SelectedValue As String = "All"
        If ddlEnvironment.SelectedItem IsNot Nothing Then
            SelectedValue = ddlEnvironment.SelectedItem.Text
        End If

        ddlEnvironment.Items.Clear()
        Dim index As Integer = 0

        For Each item As ClsHelper.BasicModel In selectedApplication.Environments
            If item.Checked Then
                ddlEnvironment.Items.Add(New ListItem(item.Name, item.ID.ToString()))
            End If
        Next
    End Sub


End Class
