
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TPCusCode
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If "TradePlaceCustomer".Equals(__EVENTTARGET) Then
                    PanelDetailCustomer.Visible = True
                    LoadTPCustomerCusCodes(Integer.Parse(__EVENTARGUMENT))
                    PanelAddCUSOEM.Visible = False
                    BtnNew.Visible = True
                    BtnSave.Visible = False
                    BtnCancel.Visible = False
                    CusCodePanel.Update()
                    watch.Stop()
                    ClsHelper.Log("View TPCuscode detail customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

                ElseIf (__EVENTTARGET.EndsWith("SearchCustomer") Or __EVENTTARGET.EndsWith("ddlEnvironment") Or __EVENTTARGET.EndsWith("ddlCountry")) Then
                    PanelDetailCustomer.Visible = False
                    PanelAddCUSOEM.Visible = False
                    CusCodePanel.Update()
                    watch.Stop()
                    ClsHelper.Log("Search TPCuscode", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

                End If
            End If
        Else
            TradePlaceCustomer.LoadContext()
            watch.Stop()
            ClsHelper.Log("View TPCuscode Page", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

        End If
    End Sub

    Private Sub LoadTPCustomerCusCodes(ByVal CustomerID As Integer)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim newRow As DataRow
        Dim colKeys(1) As DataColumn
        Dim tblTPCustomerEANCode As New DataTable("TPCustomerCusCode")

        tblTPCustomerEANCode.Columns.Add("C_CUS", GetType(System.String))
        tblTPCustomerEANCode.Columns.Add("C_CUID", GetType(System.String))
        colKeys(0) = tblTPCustomerEANCode.Columns("C_CUS")
        tblTPCustomerEANCode.PrimaryKey = colKeys
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMERCODES", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)
        cmd.Parameters.AddWithValue("@CountryIsoCode", TradePlaceCustomer.GetddlCountrySelectedValue())
        cmd.Parameters.AddWithValue("@TypeCode", "CUS")

        MyDataReader = cmd.ExecuteReader()

        While MyDataReader.Read()
            newRow = tblTPCustomerEANCode.NewRow
            newRow.Item("C_CUS") = MyDataReader("C_CUS").ToString
            newRow.Item("C_CUID") = MyDataReader("C_CUID").ToString
            tblTPCustomerEANCode.Rows.Add(newRow)
        End While

        dtlTPCustomerCusCode.DataSource = tblTPCustomerEANCode
        dtlTPCustomerCusCode.DataBind()

        tpcid.Value = CustomerID

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
    End Sub

    Protected Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        PanelAddCUSOEM.Visible = True
        BtnNew.Visible = False
        BtnSave.Visible = True
        BtnCancel.Visible = True
        CusCodePanel.Update()
    End Sub

    Protected Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim result As String = ""
        Dim Success As Boolean = AddCusCode(tpcid.Value, txtOEM.Text, txtCUS.Text, result)
        If (Success) Then
            PanelAddCUSOEM.Visible = False
            BtnNew.Visible = True
            BtnSave.Visible = False
            BtnCancel.Visible = False
            txtOEM.Text = ""
            txtCUS.Text = ""
            'ScriptManager.RegisterStartupScript(CusCodePanel, CusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
            LoadTPCustomerCusCodes(tpcid.Value)
            CusCodePanel.Update()
        Else
            ScriptManager.RegisterStartupScript(CusCodePanel, CusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
            CusCodePanel.Update()
        End If
        watch.Stop()
        ClsHelper.Log("TPCuscode Save changes", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        PanelAddCUSOEM.Visible = False
        BtnNew.Visible = True
        BtnSave.Visible = False
        BtnCancel.Visible = False
        txtOEM.Text = ""
        txtCUS.Text = ""
        CusCodePanel.Update()
    End Sub

    Protected Sub BtnDelete_Click(ByVal sender As System.Object, e As CommandEventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim commandArgs() As String = e.CommandArgument.ToString().Split(",")
        Dim result As String = DeleteCusCode(tpcid.Value, commandArgs(1), commandArgs(0))
        If (result = "") Then
            result = "Deleted"
        End If
        ScriptManager.RegisterStartupScript(CusCodePanel, CusCodePanel.GetType(), "Alert", "alert('" + result + "');", True)
        LoadTPCustomerCusCodes(tpcid.Value)
        CusCodePanel.Update()
        watch.Stop()
        ClsHelper.Log("TPCuscode Delete code", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Private Function AddCusCode(ByVal TPC_ID As Integer, ByVal OEMCode As String, ByVal CusCode As String, ByRef StrReturn As String) As Boolean

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim Result As Boolean
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PI_B2BCustomerCUS_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@C_CUID", OEMCode)
        cmd.Parameters.AddWithValue("@C_CUS", CusCode)
        cmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceCustomer.GetddlCountrySelectedValue())

        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        Try
            cmd.ExecuteNonQuery()
            Select Case CType(cmd.Parameters("@Return_Value").Value, Integer)
                Case 0
                    ' Tout c'est bien déroulé
                    StrReturn = ""
                    Result = True
                Case 1
                    ' Translation CUS-OEM déjà définie
                    StrReturn = "CUS-OEM translation already exists in the system. Creation not carried out."
                    Result = False
                Case 2
                    ' Translation CUS-OEM déjà définie pour un autre customer
                    StrReturn = "CUS-OEM translation already exists in the system for another customer. Creation not carried out."
                    Result = False
                Case 3
                    ' Customer code déjà utilisé pour un code CUS
                    StrReturn = "The Customer code is already used for another CUS code. The new translation has been created in more of the preceding one. Please, check the data."
                    Result = True
                Case 4
                    ' L'utilisateur n'existe pas dans le B2B
                    StrReturn = "The Customer code doesn\'t exist in the B2B. Creation not carried out."
                    Result = False
                Case 5
                    ' Customer code déjà lié à un autre customer TradePlace
                    StrReturn = "The Customer code is already bound to another TradePlace customer!"
                    Result = False
                Case 10
                    ' Pas de pays correspondant au context ???
                    Throw New ApplicationException("unknow country for the context id !!!")
                Case 11, 12, 13
                    ' DB Error during insert in database
                    Throw New ApplicationException(cmd.Parameters("@Error").Value.ToString)
                Case Else
                    ' Erreur non prévue
                    Throw New ApplicationException("Unknow error.")
            End Select
        Catch Except As Exception
            ' Une exception est levée lors de l'exécution de la PS
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                ' Erreur identifiée dans la PS
                StrReturn = cmd.Parameters("@Error").Value.ToString
            Else
                ' Erreur non identifiée dans la PS
                ' On remonte l'erreur
                StrReturn = Except.Message
                Throw Except
            End If
        End Try
        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        Return Result
    End Function

    Private Function DeleteCusCode(ByVal TPC_ID As Integer, ByVal OEMCode As String, ByVal CusCode As String) As String
        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PD_B2BCustomerCUS_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@C_CUS", CusCode)
        cmd.Parameters.AddWithValue("@C_CUID", OEMCode)
        cmd.Parameters.AddWithValue("@CY_ISOCODE", TradePlaceCustomer.GetddlCountrySelectedValue())

        cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 255).Direction = ParameterDirection.Output
        cmd.Parameters.Add("@Return_Value", SqlDbType.NVarChar).Direction = ParameterDirection.ReturnValue

        Try
            cmd.ExecuteNonQuery()
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                ' Une erreur est survenue
                strReturn = cmd.Parameters("@Error").Value.ToString
            End If
        Catch Except As Exception
            ' Une exception est levée lors de l'exécution de la PS
            If CType(cmd.Parameters("@Return_Value").Value, Integer) <> 0 Then
                ' Erreur identifiée dans la PS
                strReturn = cmd.Parameters("@Error").Value.ToString
            Else
                ' Erreur non identifiée dans la PS
                ' On remonte l'erreur
                DeleteCusCode = Except.Message
                Throw Except
            End If
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        DeleteCusCode = strReturn
    End Function

End Class
