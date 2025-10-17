
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TPAdressCus
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If "TradePlaceCustomer".Equals(__EVENTTARGET) Then
                    PanelDetailCustomer.Visible = True
                    LoadTPCustomerAddressCusCodes(Integer.Parse(__EVENTARGUMENT))
                    PanelAddAddressCusOEM.Visible = False
                    BtnNew.Visible = True
                    BtnSave.Visible = False
                    BtnCancel.Visible = False
                    AddressCusCodePanel.Update()
                    watch.Stop()
                    ClsHelper.Log("View TPAdressCus detail customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

                ElseIf (__EVENTTARGET.EndsWith("SearchCustomer") Or __EVENTTARGET.EndsWith("ddlEnvironment") Or __EVENTTARGET.EndsWith("ddlCountry")) Then
                    PanelDetailCustomer.Visible = False
                    PanelAddAddressCusOEM.Visible = False
                    AddressCusCodePanel.Update()
                    watch.Stop()
                    ClsHelper.Log("Search TPAdressCus", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

                End If
            End If
        Else
            TradePlaceCustomer.LoadContext()
            watch.Stop()
            ClsHelper.Log("View TPAdressCus Page", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

        End If
    End Sub

    Protected Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        PanelAddAddressCusOEM.Visible = True
        BtnNew.Visible = False
        BtnSave.Visible = True
        BtnCancel.Visible = True
        AddressCusCodePanel.Update()
    End Sub

    Protected Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim result As String = ""
        Dim Success As Boolean = AddAddressCusCode(tpcid.Value, txtOEM.Text, txtCus.Text, result)
        If (Success) Then
            PanelAddAddressCusOEM.Visible = False
            BtnNew.Visible = True
            BtnSave.Visible = False
            BtnCancel.Visible = False
            txtOEM.Text = ""
            txtCus.Text = ""
            'ScriptManager.RegisterStartupScript(AddressCusCodePanel, AddressCusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
            LoadTPCustomerAddressCusCodes(tpcid.Value)
            AddressCusCodePanel.Update()
        Else
            ScriptManager.RegisterStartupScript(AddressCusCodePanel, AddressCusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
            AddressCusCodePanel.Update()
        End If
        watch.Stop()
        ClsHelper.Log("TPAdressCus Save changes", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        PanelAddAddressCusOEM.Visible = False
        BtnNew.Visible = True
        BtnSave.Visible = False
        BtnCancel.Visible = False
        txtOEM.Text = ""
        txtCus.Text = ""
        AddressCusCodePanel.Update()
    End Sub

    Protected Sub BtnDelete_Click(ByVal sender As System.Object, e As CommandEventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim commandArgs As String = e.CommandArgument.ToString()
        Dim result As String = DeleteAddressCusCode(tpcid.Value, commandArgs)
        If (result = "") Then
            result = "Deleted"
        End If
        ScriptManager.RegisterStartupScript(AddressCusCodePanel, AddressCusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
        LoadTPCustomerAddressCusCodes(tpcid.Value)
        AddressCusCodePanel.Update()
        watch.Stop()
        ClsHelper.Log("TPAdressCus Delete code", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Private Sub LoadTPCustomerAddressCusCodes(ByVal CustomerID As Integer)
        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim newRow As DataRow
        Dim colKeys(1) As DataColumn
        Dim tblTPCustomerEANCode As New DataTable("TPCustomerAddressCusCode")

        tblTPCustomerEANCode.Columns.Add("CUS", GetType(System.String))
        tblTPCustomerEANCode.Columns.Add("ADDRESSKEY", GetType(System.String))
        colKeys(0) = tblTPCustomerEANCode.Columns("CUS")
        tblTPCustomerEANCode.PrimaryKey = colKeys

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMERCODES", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)
        cmd.Parameters.AddWithValue("@CountryIsoCode", TradePlaceCustomer.GetddlCountrySelectedValue())
        cmd.Parameters.AddWithValue("@TypeCode", "ACUS")
        MyDataReader = cmd.ExecuteReader()

        While MyDataReader.Read()
            newRow = tblTPCustomerEANCode.NewRow
            newRow.Item("CUS") = MyDataReader("CUS").ToString
            newRow.Item("ADDRESSKEY") = MyDataReader("ADDRESSKEY").ToString
            tblTPCustomerEANCode.Rows.Add(newRow)
        End While

        dtlTPCustomerAddressCusCode.DataSource = tblTPCustomerEANCode
        dtlTPCustomerAddressCusCode.DataBind()

        tpcid.Value = CustomerID

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
    End Sub

    Private Function AddAddressCusCode(ByVal TPC_ID As Integer, ByVal OEMCode As String, ByVal CusCode As String, ByRef StrReturn As String) As Boolean

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim Result As Boolean
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PI_ADDRESSCUSOEM_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@OEMCode", OEMCode)
        cmd.Parameters.AddWithValue("@CUSCode", CusCode)
        cmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceCustomer.GetddlCountrySelectedValue())

        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        Try
            cmd.ExecuteNonQuery()
            Select Case CType(cmd.Parameters("@Return_Value").Value, Integer)
                Case 0
                    StrReturn = ""
                    Result = True

                Case 1
                    StrReturn = "Unabled to create new CUS-OEM translation!"
                    Result = False

                Case 2
                    StrReturn = "EAN-OEM translation already exists in the system for another customer. Creation not carried out."
                    Result = False

                Case Else
                    Throw New ApplicationException("Unknow error.")

            End Select

            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                StrReturn = cmd.Parameters("@Error").Value.ToString
                Result = False
            End If
        Catch Except As Exception
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                StrReturn = cmd.Parameters("@Error").Value.ToString
                Result = False
            Else
                StrReturn = Except.Message
                Throw Except
            End If
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        Return Result
    End Function

    Private Function DeleteAddressCusCode(ByVal TPC_ID As Integer, ByVal CusCode As String) As String

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PD_ADDRESSCUSOEM_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@CUSCode", CusCode)
        cmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceCustomer.GetddlCountrySelectedValue())

        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        Try
            cmd.ExecuteNonQuery()
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                strReturn = cmd.Parameters("@Error").Value.ToString
            End If
        Catch Except As Exception
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                strReturn = cmd.Parameters("@Error").Value.ToString
            Else
                DeleteAddressCusCode = Except.Message
                Throw Except
            End If
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        DeleteAddressCusCode = strReturn
    End Function

End Class
