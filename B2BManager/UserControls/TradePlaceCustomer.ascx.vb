
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class UserControls_TradePlaceCustomer
    Inherits System.Web.UI.UserControl
    Private Const MaxLine As Integer = 30
    Public Sub LoadContext()
        RenderCountryDropDown()
        RenderEnvironnementDropDown()
        PanelContext.Visible = True
        Refresh(ContextID:=ddlCountry.SelectedValue)
    End Sub

    Public Sub LoadWithoutCountry()
        RenderEnvironnementDropDown()
        PanelContext.Visible = False
        Refresh()
    End Sub

    Private Sub LoadTPCustomers(Optional ByVal TPCustomerId As String = "", Optional ByVal ContextID As String = "", Optional ByVal TPCName As String = "")

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMERLISTFILTERED", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        If ContextID <> "" Then
            cmd.Parameters.AddWithValue("@CY_ISOCODE", ContextID)
        End If
        If TPCName <> "" Then
            cmd.Parameters.AddWithValue("@TPCName", TPCName)
        End If

        MyDataReader = cmd.ExecuteReader()

        dtlTPCustomer.DataSource = MyDataReader
        dtlTPCustomer.DataBind()

        Dim dt As DataTable = New DataTable()
        dt.Load(MyDataReader)
        ViewState("TPCustomersList") = dt

        If (TPCustomerId <> "") Then
            ' this foreach loop is for select the right line after a save.
            ' the line will be in bold.
            For Each dtlItem In dtlTPCustomer.Items
                If CType(dtlItem.Controls(1), System.Web.UI.HtmlControls.HtmlInputHidden).Value = TPCustomerId Then
                    dtlTPCustomer.SelectedIndex = dtlItem.ItemIndex
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

    Public Sub Refresh(Optional ByVal TPCustomerId As String = "", Optional ByVal ContextID As String = "")
        LoadTPCustomers(TPCustomerId, ContextID)
        TradePlaceCustomerPanel.Update()
    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(sender As Object, e As EventArgs)
        SearchCustomer.Text = ""
        searchPanel.Update()
        Refresh(ContextID:=ddlCountry.SelectedValue)
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        SearchCustomer.Text = ""
        searchPanel.Update()
        Refresh(ContextID:=ddlCountry.SelectedValue)
    End Sub

    Public Function GetddlCountrySelectedValue() As String
        Return ddlCountry.SelectedValue
    End Function

    Public Function GetddlEnvironmentSelectedValue() As Integer
        Return ddlEnvironment.SelectedValue
    End Function

    Protected Sub SearchCustomer_TextChanged(sender As Object, e As EventArgs)
        Dim selectedValue = ""
        If (ddlCountry.SelectedValue <> "") Then
            selectedValue = ddlCountry.SelectedValue
        End If
        LoadTPCustomers(ContextID:=selectedValue, TPCName:=SearchCustomer.Text)
        TradePlaceCustomerPanel.Update()
    End Sub

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
            If item.Checked AndAlso item.Is_EManager Then
                ddlEnvironment.Items.Add(New ListItem(item.Name, item.ID.ToString()))
            End If
        Next

        ddlEnvironment.SelectedIndex = 0
    End Sub


End Class
