
Imports System.Data
Imports System.Data.SqlClient

Partial Class TPTroisMaster
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If "TradePlaceCustomer".Equals(__EVENTTARGET) Then
                    InitMode(ClsEManagerHelper.InitMode.View, __EVENTARGUMENT)
                    TPCustomerID.Value = __EVENTARGUMENT
                    TradePlaceMasterCustomerPanel.Update()
                ElseIf (__EVENTTARGET.EndsWith("SearchCustomer") Or __EVENTTARGET.EndsWith("ddlEnvironment") Or __EVENTTARGET.EndsWith("ddlCountry")) Then
                    PanelDetailTPMaster.Visible = False
                    TblAddCustomer.Visible = False
                End If
            End If
        Else
            TradePlaceMasterCustomer.LoadContext()
        End If

    End Sub

    Protected Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        InitMode(ClsEManagerHelper.InitMode.Create)
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        InitMode(ClsEManagerHelper.InitMode.Create, TPCustomerID.Value)
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub BtnAdd_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim ddlCustomers As DropDownList = CType(CustomersAttached.FindControl("ddlCustomers"), DropDownList)
        InsertMasterCustomercode(ddlCustomersAdd.SelectedValue, ddlMastersAdd.SelectedValue, isDefaultAdd.Checked)
        If (ddlMastersAdd.SelectedValue = "") Then
            TPCustomerID.Value = ddlCustomersAdd.SelectedValue
        Else
            TPCustomerID.Value = ddlMastersAdd.SelectedValue
        End If

        InitMode(ClsEManagerHelper.InitMode.View, TPCustomerID.Value)
        TradePlaceMasterCustomer.Refresh(TPCustomerID.Value.ToString())
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Private Sub InitMode(ByVal Mode As ClsEManagerHelper.InitMode, Optional ByVal MasterCustomerSelectedItem As String = "", Optional ByVal CustomerDeletedCode As String = "")
        Dim ddlCustomers As DropDownList = CType(CustomersAttached.FindControl("ddlCustomers"), DropDownList)
        Select Case Mode
            Case ClsEManagerHelper.InitMode.Create
                BindDDLCustomers()
                BindDDLMasterCode()
                BtnNew.Visible = True
                BtnAdd.Visible = True
                BtnCancel.Visible = True

                ddlCustomersAdd.Visible = True
                ddlMastersAdd.Visible = True
                'ddlCustomersAdd.SelectedIndex = 0
                'ddlMastersAdd.SelectedIndex = 0

                CustomerLabel.Visible = False
                CustomerCodeLabel.Visible = False
                TblAddCustomer.Visible = True
                PanelDetailTPMaster.Visible = False
            Case ClsEManagerHelper.InitMode.View
                LoadTpCustomer(MasterCustomerSelectedItem)
                PanelDetailTPMaster.Visible = True
                If (ddlCustomers IsNot Nothing) Then
                    ddlCustomers.Visible = False
                End If
                ddlMastersAdd.Visible = False
                BtnNew.Visible = True
                BtnAdd.Visible = False
                BtnCancel.Visible = False
                CustomerLabel.Visible = True
                CustomerCodeLabel.Visible = True
                TblAddCustomer.Visible = False
            Case ClsEManagerHelper.InitMode.CustomerDeleted
                PanelDetailTPMaster.Visible = False
                TPCustomerID.Value = ""
                ScriptManager.RegisterStartupScript(TradePlaceMasterCustomerPanel, TradePlaceMasterCustomerPanel.GetType(), "Alert", "alert('Customer " + CustomerDeletedCode + " deleted!');", True)
            Case ClsEManagerHelper.InitMode.MasterDeleted
                PanelDetailTPMaster.Visible = True
                TPCustomerID.Value = MasterCustomerSelectedItem
                ScriptManager.RegisterStartupScript(TradePlaceMasterCustomerPanel, TradePlaceMasterCustomerPanel.GetType(), "Alert", "alert('Customer" + CustomerDeletedCode + " deleted!');", True)
        End Select
    End Sub

    Public Sub BindDDLCustomers()
        ddlCustomersAdd.DataSource = getCustomersCode()
        ddlCustomersAdd.DataBind()
    End Sub
    Private Sub BindDDLMasterCode()
        ddlMastersAdd.DataSource = getMasterCodes()
        ddlMastersAdd.DataBind()
    End Sub

    Private Sub LoadTpCustomer(ByVal MasterCode As String)
        Dim ds As New DataSet
        Dim dv As New DataView
        Dim cnx As SqlConnection

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))

        Try
            cnx.Open()

            Dim cmd As New SqlCommand("PS_TP3_MasterCustomerCodes_V2", cnx)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@contextid", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())
            cmd.Parameters.AddWithValue("@MasterCode", MasterCode)

            ds = New DataSet()
            Dim da As SqlDataAdapter = New SqlDataAdapter(cmd)
            da.Fill(ds)
            CustomersAttached.DataSource = ds
            CustomersAttached.DataBind()
        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try

    End Sub

    Protected Sub CustomersAttached_RowEditing(ByVal sender As System.Object, ByVal e As GridViewEditEventArgs)

        CustomersAttached.EditIndex = e.NewEditIndex
        Dim row As GridViewRow = CustomersAttached.Rows.Item(e.NewEditIndex)
        Dim SelectedValue As String = CType(row.FindControl("ddlCustomerMasterSelectedValue"), HiddenField).Value
        LoadTpCustomer(SelectedValue)


        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub CustomersAttached_RowUpdating(ByVal sender As System.Object, ByVal e As GridViewUpdateEventArgs)
                     
        Dim row As GridViewRow = CustomersAttached.Rows.Item(e.RowIndex)

        Dim ddlMasters2 As DropDownList = CType(row.FindControl("ddlMasters"), DropDownList)
        Dim ckIsDefault As CheckBox = CType(row.FindControl("ckIsDefault"), CheckBox)
        Dim CustomerCode As HiddenField = CType(row.FindControl("CustomerCode"), HiddenField)
        Dim ddlCustomerMasterSelectedValue As HiddenField = CType(row.FindControl("ddlCustomerMasterSelectedValue"), HiddenField)

        UpdateMasterCustomercode(CustomerCode.Value, ddlMasters2.SelectedValue, ckIsDefault.Checked)

        CustomersAttached.EditIndex = -1

        If (ddlMasters2.SelectedValue = "") Then
            LoadTpCustomer(CustomerCode.Value)
            TradePlaceMasterCustomer.Refresh(CustomerCode.Value)
        Else
            LoadTpCustomer(ddlMasters2.SelectedValue)
            TradePlaceMasterCustomer.Refresh(ddlMasters2.SelectedValue)
        End If

        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub CustomersAttached_RowCanceling(ByVal sender As System.Object, ByVal e As GridViewCancelEditEventArgs)

        CustomersAttached.EditIndex = -1

        Dim SelectedValue As String = CType(CustomersAttached.Rows.Item(e.RowIndex).FindControl("ddlCustomerMasterSelectedValue"), HiddenField).Value
        LoadTpCustomer(SelectedValue)
        TradePlaceMasterCustomerPanel.Update()
    End Sub

    Protected Sub CustomersAttached_RowDeleting(ByVal sender As System.Object, ByVal e As GridViewDeleteEventArgs)

        Try
            Dim row As GridViewRow = CustomersAttached.Rows.Item(e.RowIndex)

            Dim CustomerCode As HiddenField = CType(row.FindControl("CustomerCode"), HiddenField)
            Dim ddlCustomerMasterSelectedValue As HiddenField = CType(row.FindControl("ddlCustomerMasterSelectedValue"), HiddenField)

            DeleteMasterCustomercode(CustomerCode.Value)

            If (CustomersAttached.Rows.Count > 1) Then
                InitMode(ClsEManagerHelper.InitMode.MasterDeleted, MasterCustomerSelectedItem:=ddlCustomerMasterSelectedValue.Value, CustomerDeletedCode:=CustomerCode.Value)
                LoadTpCustomer(ddlCustomerMasterSelectedValue.Value)
                TradePlaceMasterCustomer.Refresh(ddlCustomerMasterSelectedValue.Value)

            Else
                InitMode(ClsEManagerHelper.InitMode.CustomerDeleted, CustomerDeletedCode:=CustomerCode.Value)
                TradePlaceMasterCustomer.Refresh()
            End If

            TradePlaceMasterCustomerPanel.Update()
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(TradePlaceMasterCustomerPanel, TradePlaceMasterCustomerPanel.GetType(), "Alert", "alert('" + ex.Message + "');", True)
            TradePlaceMasterCustomerPanel.Update()
        End Try
    End Sub

    Protected Sub CustomersAttached_RowDataBound(ByVal sender As System.Object, ByVal e As GridViewRowEventArgs)
        If ((e.Row.RowState And DataControlRowState.Edit) > 0) Then

            Dim ddlMasters As DropDownList = CType(e.Row.FindControl("ddlMasters"), DropDownList)
            ddlMasters.DataSource = getMasterCodes()
            ddlMasters.SelectedValue = CType(e.Row.FindControl("ddlCustomerMasterSelectedValue"), HiddenField).Value
            ddlMasters.DataBind()

        End If
    End Sub


    Private Function getCustomersCode() As DataTable
        Dim ds As New DataSet
        Dim dv As New DataView
        Dim cnx As SqlConnection
        Dim newRow As DataRow
        Dim table As DataTable = New DataTable("Customers")
        Dim colKeys(1) As DataColumn

        table.Columns.Add("CustomerName", GetType(System.String))
        table.Columns.Add("CustomerCode", GetType(System.String))
        colKeys(0) = table.Columns("CustomerName")
        table.PrimaryKey = colKeys

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))

        Try
            Dim MyDataReader As SqlDataReader
            cnx.Open()

            Dim cmd As New SqlCommand("PS_TP3_Customers_V2", cnx)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())
            cmd.Parameters.AddWithValue("@exceptAssociated", True)

            MyDataReader = cmd.ExecuteReader()

            While MyDataReader.Read()
                newRow = table.NewRow
                newRow.Item("CustomerName") = MyDataReader("CustomerName").ToString
                newRow.Item("CustomerCode") = MyDataReader("CustomerCode").ToString
                table.Rows.Add(newRow)
            End While

        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try
        Return table
    End Function

    Private Function getMasterCodes() As DataTable
        Dim ds As New DataSet
        Dim dv As New DataView
        Dim cnx As SqlConnection
        Dim newRow As DataRow
        Dim table As DataTable = New DataTable("MasterCode")
        Dim colKeys(1) As DataColumn

        table.Columns.Add("Mastername", GetType(System.String))
        table.Columns.Add("MasterCode", GetType(System.String))
        table.PrimaryKey = colKeys

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))

        Try
            Dim MyDataReader As SqlDataReader
            cnx.Open()

            Dim cmd As New SqlCommand("PS_TP3_MasterCustomerCodes_V2", cnx)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@contextid", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())
            cmd.Parameters.AddWithValue("@MasterCode", "")

            MyDataReader = cmd.ExecuteReader()

            newRow = table.NewRow
            newRow.Item("Mastername") = "[New Master]"
            newRow.Item("MasterCode") = ""
            table.Rows.Add(newRow)

            While MyDataReader.Read()
                newRow = table.NewRow
                newRow.Item("Mastername") = MyDataReader("Mastername").ToString
                newRow.Item("MasterCode") = MyDataReader("MasterCode").ToString
                table.Rows.Add(newRow)
            End While

        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try
        Return table.DefaultView.ToTable(True, "Mastername", "MasterCode")

    End Function

    Public Sub UpdateMasterCustomercode(ByVal CustomerCode As String, ByVal MasterCode As String, ByVal isDefault As Boolean)
        'Dim cnx As SqlConnection = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        Dim cnx As SqlConnection = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))
        Dim dt As New DataTable
        Try
            Dim sqlcmd As New SqlCommand("PU_TP3_MasterCustomerCode_V2", cnx)
            sqlcmd.CommandType = CommandType.StoredProcedure
            sqlcmd.Parameters.AddWithValue("@CustomerCode", CustomerCode)
            sqlcmd.Parameters.AddWithValue("@MasterCode", MasterCode)
            sqlcmd.Parameters.AddWithValue("@isDefault", isDefault)
            sqlcmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())

            cnx.Open()

            sqlcmd.ExecuteNonQuery()
        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try
    End Sub

    Public Sub InsertMasterCustomercode(ByVal CustomerCode As String, ByVal MasterCode As String, ByVal isDefault As Boolean)
        'Dim cnx As SqlConnection = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        Dim cnx As SqlConnection = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))
        Dim dt As New DataTable
        Try
            Dim sqlcmd As New SqlCommand("PI_TP3_MasterCustomerCode_V2", cnx)
            sqlcmd.CommandType = CommandType.StoredProcedure
            sqlcmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())
            sqlcmd.Parameters.AddWithValue("@CustomerCode", CustomerCode)
            sqlcmd.Parameters.AddWithValue("@MasterCode", MasterCode)
            sqlcmd.Parameters.AddWithValue("@isDefault", isDefault)
            cnx.Open()

            sqlcmd.ExecuteNonQuery()
        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try
    End Sub

    Public Sub DeleteMasterCustomercode(ByVal CustomerCode As String)
        'Dim cnx As SqlConnection = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        Dim cnx As SqlConnection = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceMasterCustomer.GetddlEnvironmentSelectedValue()))
        Dim dt As New DataTable

        Try
            Dim sqlcmd As New SqlCommand("PD_TP3_MasterCustomerCode_V2", cnx)
            sqlcmd.CommandType = CommandType.StoredProcedure
            sqlcmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceMasterCustomer.GetLstActiveProfileSelectedIndex())
            sqlcmd.Parameters.AddWithValue("@CustomerCode", CustomerCode)
            cnx.Open()

            sqlcmd.ExecuteNonQuery()
        Catch ex As Exception
            Throw ex
        Finally
            cnx.Close()
        End Try
    End Sub


End Class
