
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Partial Class TPDefinition
    Inherits System.Web.UI.Page
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If "TradePlaceCustomer".Equals(__EVENTTARGET) Then
                    LoadTPCustomerDetails(Integer.Parse(__EVENTARGUMENT))
                    InitMode(ClsEManagerHelper.InitMode.View)
                    DefinitionPanel.Update()
                    watch.Stop()
                    ClsHelper.Log("View TPDefinition detail customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
                ElseIf (__EVENTTARGET.EndsWith("SearchCustomer") Or __EVENTTARGET.EndsWith("ddlEnvironment")) Then
                    InitMode(ClsEManagerHelper.InitMode.NewView)
                    DefinitionPanel.Update()
                    watch.Stop()
                    ClsHelper.Log("Search TPDefinition", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
                End If
            End If
        Else
            TradePlaceCustomer.LoadWithoutCountry()
            AddCountryListToJavascript()
            watch.Stop()
            ClsHelper.Log("View TPDefinition Page", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
        End If

    End Sub

    Private Sub AddCountryListToJavascript()
        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim result As String = ""

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMERCountriesAvailable", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        MyDataReader = cmd.ExecuteReader()

        result += "'{"

        While MyDataReader.Read()
            result += """" + MyDataReader("CY_Name").ToString + """:"
            result += """" + MyDataReader("CY_ISOCODE").ToString + ""","
        End While
        result = result.TrimEnd(",")
        result += "}'"
        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        Page.ClientScript.RegisterArrayDeclaration("CountryListBinded", result)

    End Sub

    Protected Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As EventArgs)

        InitMode(ClsEManagerHelper.InitMode.Create)
        DefinitionPanel.Update()

    End Sub

    Protected Sub BtnEdit_Click(ByVal sender As System.Object, ByVal e As EventArgs)

        InitMode(ClsEManagerHelper.InitMode.Update)
        DefinitionPanel.Update()

    End Sub

    Protected Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        UpdateTPCustomer(TPCustomerID.Value.ToString())
        InitMode(ClsEManagerHelper.InitMode.View)
        TradePlaceCustomer.Refresh(TPCustomerID.Value.ToString())
        DefinitionPanel.Update()
        watch.Stop()
        ClsHelper.Log("TPDefinition Save changes", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        If (TPCustomerID.Value <> "") Then
            LoadTPCustomerDetails(TPCustomerID.Value.ToString())
            InitMode(ClsEManagerHelper.InitMode.View)
        Else
            InitMode(ClsEManagerHelper.InitMode.NewView)
        End If
        DefinitionPanel.Update()
    End Sub

    Protected Sub BtnAdd_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        AddTPCustomer(TPCustomerID.Value.ToString())
        InitMode(ClsEManagerHelper.InitMode.View)
        TradePlaceCustomer.Refresh()
        DefinitionPanel.Update()
        watch.Stop()
        ClsHelper.Log("TPDefinition Add customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        DeleteTPCustomer(TPCustomerID.Value.ToString())
        InitMode(ClsEManagerHelper.InitMode.Deleted)
        TradePlaceCustomer.Refresh()
        DefinitionPanel.Update()
        watch.Stop()
        ClsHelper.Log("TPDefinition Delete customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
    End Sub

    Protected Sub ImageAddPushStock_Click(ByVal sender As System.Object, ByVal e As EventArgs)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_GetC_GLOBALIDbyC_CUID", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@C_CUID", AddPushStock.Text)

        MyDataReader = cmd.ExecuteReader()

        If MyDataReader.Read Then

            Dim AffectedPushStock As DataRow
            Dim AffectedPushStockExistingRow As DataRow

            AffectedPushStockExistingRow = CType(ViewState("AffectedPushStock"), DataTable).Rows.Find(MyDataReader("C_GLOBALID").ToString)

            If (AffectedPushStockExistingRow Is Nothing) Then
                AffectedPushStock = CType(ViewState("AffectedPushStock"), DataTable).NewRow
                AffectedPushStock.Item("C_GLOBALID") = MyDataReader("C_GLOBALID").ToString
                AffectedPushStock.Item("C_CUID") = MyDataReader("C_CUID").ToString
                AffectedPushStock.Item("IS_ACTIF") = MyDataReader("IS_ACTIF").ToString

                CType(ViewState("AffectedPushStock"), DataTable).Rows.Add(AffectedPushStock)
                dtlAffectedPushStock.DataSource = CType(ViewState("AffectedPushStock"), DataTable)
                dtlAffectedPushStock.DataBind()

                dtlAffectedPushStock.SelectedIndex = -1
            Else
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('Customer already attached!');", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('This customer doesn\'t exist!');", True)
        End If

        AddPushStock.Text = ""
        Dim Liste As String() = AddCustomerTextBoxe.Text.Split(",")
        ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "AddAmsifySuggest", "AddAmsifySuggest(false," + (New JavaScriptSerializer()).Serialize(Liste) + ");", True)
        DefinitionPanel.Update()
    End Sub

    Protected Sub dtlAffectedPushStock_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If dtlAffectedPushStock.SelectedIndex <> -1 Then
            Dim AffectedPushStock As DataRow

            AffectedPushStock = CType(ViewState("AffectedPushStock"), DataTable).Rows.Find(CType(dtlAffectedPushStock.SelectedItem.Controls(1), System.Web.UI.HtmlControls.HtmlInputHidden).Value)

            CType(ViewState("AffectedPushStock"), DataTable).Rows.Remove(AffectedPushStock)
            dtlAffectedPushStock.DataSource = CType(ViewState("AffectedPushStock"), DataTable)
            dtlAffectedPushStock.DataBind()

            dtlAffectedPushStock.SelectedIndex = -1
            Dim Liste As String() = AddCustomerTextBoxe.Text.Split(",")
            ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "AddAmsifySuggest", "AddAmsifySuggest(false," + (New JavaScriptSerializer()).Serialize(Liste) + ");", True)
            DefinitionPanel.Update()
        End If

    End Sub

    Private Sub LoadTPCustomerDetails(ByVal CustomerID As Integer)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim newRow As DataRow
        Dim colKeys(1) As DataColumn

        Dim tblAffectedCustomers As New DataTable("AffectedCustomers")
        Dim tblAffectedPushStock As New DataTable("AffectedPushStock")

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMER", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)

        MyDataReader = cmd.ExecuteReader()

        If MyDataReader.Read Then
            txtTPCName.Text = MyDataReader("TPC_Name").ToString
            txtTPIDHTTP.Text = MyDataReader("TPID_HTTP").ToString
            txtTPIDSMTP.Text = MyDataReader("TPID_SMTP").ToString
            TPCustomerID.Value = CustomerID
        Else
            txtTPCName.Text = ""
            txtTPIDHTTP.Text = ""
            txtTPIDSMTP.Text = ""
        End If

        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMER_COUNTRIES", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)

        MyDataReader = cmd.ExecuteReader()
        Dim result As String = ""
        While MyDataReader.Read()
            result += MyDataReader("CY_Name").ToString + ","
        End While

        countryList.Text = result.TrimEnd(",")

        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMER_AFFECTED_CUSTOMERS_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)

        MyDataReader = cmd.ExecuteReader()
        result = ""
        While MyDataReader.Read()
            result += MyDataReader("C_CUID").ToString + "_" + MyDataReader("C_GLOBALID").ToString + ","
        End While

        AddCustomerTextBoxe.Text = result.TrimEnd(",")

        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        tblAffectedPushStock.Columns.Add("C_GLOBALID", GetType(System.String))
        tblAffectedPushStock.Columns.Add("C_CUID", GetType(System.String))
        tblAffectedPushStock.Columns.Add("IS_ACTIF", GetType(System.String))
        colKeys(0) = tblAffectedPushStock.Columns("C_GLOBALID")
        tblAffectedPushStock.PrimaryKey = colKeys

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMER_AFFECTED_PUSHSTOCK", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)

        MyDataReader = cmd.ExecuteReader()

        While MyDataReader.Read()
            newRow = tblAffectedPushStock.NewRow
            newRow.Item("C_GLOBALID") = MyDataReader("C_GLOBALID").ToString
            newRow.Item("C_CUID") = MyDataReader("C_CUID").ToString
            newRow.Item("IS_ACTIF") = MyDataReader("IS_ACTIF").ToString
            tblAffectedPushStock.Rows.Add(newRow)
        End While
        ViewState("AffectedPushStock") = tblAffectedPushStock

        dtlAffectedPushStock.DataSource = tblAffectedPushStock
        dtlAffectedPushStock.DataBind()


        If Not cnx Is Nothing Then
            cnx.Close()
        End If
    End Sub

    Private Sub InitMode(ByVal Mode As ClsEManagerHelper.InitMode)

        Select Case Mode
            Case ClsEManagerHelper.InitMode.Create
                PanelDetailCustomer.Visible = True
                CustomerDetail.Visible = True
                BtnNew.Visible = False
                BtnEdit.Visible = False
                BtnSave.Visible = False
                BtnCancel.Visible = True
                BtnDelete.Visible = False
                BtnDeleteSubmit.Enabled = False
                BtnAdd.Visible = True
                RightPanel.Visible = True
                EnableTexboxes()
                txtTPCName.Text = ""
                txtTPIDHTTP.Text = ""
                txtTPIDSMTP.Text = ""
                AddCustomerTextBoxe.Text = ""
                AddPushStock.Text = ""
                Dim tblAffectedPushStock As New DataTable("AffectedPushStock")
                Dim colKeys(1) As DataColumn
                tblAffectedPushStock.Columns.Add("C_GLOBALID", GetType(System.String))
                tblAffectedPushStock.Columns.Add("C_CUID", GetType(System.String))
                tblAffectedPushStock.Columns.Add("IS_ACTIF", GetType(System.String))
                colKeys(0) = tblAffectedPushStock.Columns("C_GLOBALID")
                tblAffectedPushStock.PrimaryKey = colKeys
                ViewState("AffectedPushStock") = tblAffectedPushStock

                dtlAffectedPushStock.DataSource = CType(ViewState("AffectedPushStock"), DataTable)
                dtlAffectedPushStock.DataBind()
                dtlAffectedPushStock.Visible = True

                countryList.Enabled = True
                countryList.Text = ""

                Dim Liste As String() = AddCustomerTextBoxe.Text.Split(",")
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "AddAmsifySuggest", "AddAmsifySuggest(false," + (New JavaScriptSerializer()).Serialize(Liste) + ");", True)

            Case ClsEManagerHelper.InitMode.Update
                PanelDetailCustomer.Visible = True
                BtnNew.Visible = False
                BtnEdit.Visible = False
                BtnSave.Visible = True
                BtnCancel.Visible = True
                BtnDelete.Visible = False
                BtnDeleteSubmit.Enabled = False
                BtnAdd.Visible = False
                RightPanel.Visible = True
                EnableTexboxes()
                countryList.Enabled = True
                Dim Liste As String() = AddCustomerTextBoxe.Text.Split(",")
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "AddAmsifySuggest", "AddAmsifySuggest(false," + (New JavaScriptSerializer()).Serialize(Liste) + ");", True)

                dtlAffectedPushStock.Enabled = True

            Case ClsEManagerHelper.InitMode.View

                PanelDetailCustomer.Visible = True
                BtnSave.Visible = False
                BtnCancel.Visible = False
                BtnNew.Visible = True
                BtnEdit.Visible = True
                BtnSave.Visible = False
                BtnAdd.Visible = False
                BtnDelete.Visible = True
                BtnDeleteSubmit.Enabled = True
                RightPanel.Visible = True
                CustomerDetail.Visible = True
                AddPushStock.Text = ""
                DisableTexboxes()
                countryList.Enabled = False
                Dim Liste As String() = AddCustomerTextBoxe.Text.Split(",")
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "AddAmsifySuggest", "AddAmsifySuggest(true," + (New JavaScriptSerializer()).Serialize(Liste) + ");", True)
                dtlAffectedPushStock.Enabled = False
            Case ClsEManagerHelper.InitMode.Deleted
                PanelDetailCustomer.Visible = False
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('Tradeplace customer deleted!');", True)
            Case ClsEManagerHelper.InitMode.NewView
                PanelDetailCustomer.Visible = False
                BtnSave.Visible = False
                BtnCancel.Visible = False
                BtnNew.Visible = True
                BtnEdit.Visible = False
                BtnSave.Visible = False
                BtnAdd.Visible = False
                BtnDelete.Visible = False
                BtnDeleteSubmit.Enabled = False

        End Select
    End Sub

    Private Sub EnableTexboxes()
        txtTPCName.Enabled = True
        txtTPIDHTTP.Enabled = True
        txtTPIDSMTP.Enabled = True
        AddCustomerTextBoxe.Enabled = True
        AddPushStock.Enabled = True
        AddPushStockButton.Enabled = True
    End Sub

    Private Sub DisableTexboxes()
        txtTPCName.Enabled = False
        txtTPIDHTTP.Enabled = False
        txtTPIDSMTP.Enabled = False
        AddCustomerTextBoxe.Enabled = False
        AddPushStock.Enabled = False
        AddPushStockButton.Enabled = False
    End Sub

    Private Sub UpdateTPCustomer(ByVal TPCustomerID As String)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""
        Dim CountryList As String = ""
        Dim AffectedCustomerList As String = ""
        Dim AffectedPushStockList As String = ""
        Dim AffectedPushStockListActif As String = ""
        Dim ii As Integer

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PU_TPCUSTOMER", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@TPC_ID", TPCustomerID)
        If txtTPCName.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPC_Name", txtTPCName.Text)
        End If
        If txtTPIDHTTP.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPID_HTTP", txtTPIDHTTP.Text)
        End If
        If txtTPIDSMTP.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPID_SMTP", txtTPIDSMTP.Text)
        End If
        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue


        If countryListHidden.Value <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTEDCOUNTRIES", countryListHidden.Value)
        End If

        If AddCustomerTextBoxeHidden.Value <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTEDCUSTOMERS", AddCustomerTextBoxeHidden.Value)
        End If

        For ii = 0 To dtlAffectedPushStock.Items.Count - 1
            AffectedPushStockList += CType(dtlAffectedPushStock.Items(ii).Controls(1), System.Web.UI.HtmlControls.HtmlInputHidden).Value & ","
            AffectedPushStockListActif += IIf(CType(dtlAffectedPushStock.Items(ii).Controls(5), CheckBox).Checked, "1", "0") & ","
        Next

        If AffectedPushStockList <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTED_PUSHSTOCK", AffectedPushStockList)
            cmd.Parameters.AddWithValue("@AFFECTED_PUSHSTOCK_ACIFS", AffectedPushStockListActif)
        End If

        Try
            cmd.ExecuteNonQuery()
            If Integer.Parse(cmd.Parameters("@Return_Value").Value.ToString) <> 0 Then
                strReturn = cmd.Parameters("@Error").Value.ToString
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('" + strReturn.Replace("'", " ") + "');", True)
            End If
        Catch Except As Exception
            ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('" + Except.Message.Replace("'", " ") + "');", True)
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If


    End Sub

    Private Sub AddTPCustomer(ByVal TPCustomerID As String)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""
        Dim AffectedCustomerList As String = ""
        Dim AffectedPushStockList As String = ""
        Dim AffectedPushStockListActif As String = ""
        Dim ii As Integer

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PI_TPCUSTOMER", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        If txtTPCName.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPC_Name", txtTPCName.Text)
        End If
        If txtTPIDHTTP.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPID_HTTP", txtTPIDHTTP.Text)
        End If
        If txtTPIDSMTP.Text <> "" Then
            cmd.Parameters.AddWithValue("@TPID_SMTP", txtTPIDSMTP.Text)
        End If
        cmd.Parameters.Add("@TPC_ID", SqlDbType.Int, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        If countryListHidden.Value <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTEDCOUNTRIES", countryListHidden.Value)
        End If

        If AddCustomerTextBoxeHidden.Value <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTEDCUSTOMERS", AddCustomerTextBoxeHidden.Value)
        End If

        For ii = 0 To dtlAffectedPushStock.Items.Count - 1
            AffectedPushStockList += CType(dtlAffectedPushStock.Items(ii).Controls(1), System.Web.UI.HtmlControls.HtmlInputHidden).Value & ","
            AffectedPushStockListActif += IIf(CType(dtlAffectedPushStock.Items(ii).Controls(5), CheckBox).Checked, "1", "0") & ","
        Next

        If AffectedPushStockList <> "" Then
            cmd.Parameters.AddWithValue("@AFFECTED_PUSHSTOCK", AffectedPushStockList)
            cmd.Parameters.AddWithValue("@AFFECTED_PUSHSTOCK_ACIFS", AffectedPushStockListActif)
        End If

        Try
            cmd.ExecuteNonQuery()
            If Integer.Parse(cmd.Parameters("@Return_Value").Value.ToString) <> 0 Then
                ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('" + cmd.Parameters("@Error").Value.ToString.Replace("'", " ") + "');", True)
            End If
        Catch Except As Exception
            ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('" + Except.Message.Replace("'", " ") + "');", True)
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If


    End Sub

    Private Sub DeleteTPCustomer(ByVal CustomerID As Integer)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""

        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PD_TPCUSTOMER", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.Add("@TPC_ID", CustomerID).Direction = ParameterDirection.Input
        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        Try
            cmd.ExecuteNonQuery()
            If Integer.Parse(cmd.Parameters("@Return_Value").Value.ToString) <> 0 Then
                strReturn = cmd.Parameters("@Error").Value.ToString
            End If
        Catch Except As Exception
            ScriptManager.RegisterStartupScript(DefinitionPanel, DefinitionPanel.GetType(), "Alert", "alert('" + Except.Message.Replace("'", " ") + "');", True)
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

    End Sub

    <System.Web.Services.WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetCustomer(existing As String(), term As String, Country As String, Environement As Integer) As String()

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim myList As New List(Of String)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(Environement))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PS_GetC_GLOBALIDbyC_CUID_SuggestTag", cnx)
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@C_CUID", term)
        cmd.Parameters.AddWithValue("@Country", Country)

        MyDataReader = cmd.ExecuteReader()
        While MyDataReader.Read()
            myList.Add(MyDataReader("C_CUID").ToString() + "_" + MyDataReader("C_GLOBALID").ToString())
        End While

        Return myList.ToArray()

    End Function

End Class
