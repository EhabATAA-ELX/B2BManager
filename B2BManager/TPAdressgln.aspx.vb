
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class TPAdressgln
    Inherits System.Web.UI.Page
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If "TradePlaceCustomer".Equals(__EVENTTARGET) Then
                    PanelDetailCustomer.Visible = True
                    LoadTPCustomerGLNAddress(Integer.Parse(__EVENTARGUMENT))
                    PanelAddGLNOEM.Visible = False
                    BtnNew.Visible = True
                    BtnSave.Visible = False
                    BtnCancel.Visible = False
                    AdresseGlnOEMPanel.Update()
                    watch.Stop()
                    ClsHelper.Log("View TPAdressgln detail customer", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
                ElseIf (__EVENTTARGET.EndsWith("SearchCustomer") Or __EVENTTARGET.EndsWith("ddlEnvironment") Or __EVENTTARGET.EndsWith("ddlCountry")) Then
                    PanelDetailCustomer.Visible = False
                    PanelAddGLNOEM.Visible = False
                    AdresseGlnOEMPanel.Update()
                    watch.Stop()
                    ClsHelper.Log("Search TPAdressgln", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)
                End If
            End If
        Else
            TradePlaceCustomer.LoadContext()
        End If
    End Sub

    Protected Sub BtnNew_Click(ByVal sender As System.Object, ByVal e As EventArgs)

        PanelAddGLNOEM.Visible = True
        BtnNew.Visible = False
        BtnSave.Visible = True
        BtnCancel.Visible = True
        AdresseGlnOEMPanel.Update()

    End Sub

    Protected Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim result As String = ""
        Dim Success As Boolean = AddGLNCode(tpcid.Value, txtGLN.Text, txtOEM.Text, result)
        If (Success) Then
            PanelAddGLNOEM.Visible = False
            BtnNew.Visible = True
            BtnSave.Visible = False
            BtnCancel.Visible = False
            txtOEM.Text = ""
            txtGLN.Text = ""
            'ScriptManager.RegisterStartupScript(AdresseGlnOEMPanel, AdresseGlnOEMPanel.GetType(), "Alert", "alert('" + result + "');", True)
            LoadTPCustomerGLNAddress(tpcid.Value)
            AdresseGlnOEMPanel.Update()
        Else
            ScriptManager.RegisterStartupScript(AdresseGlnOEMPanel, AdresseGlnOEMPanel.GetType(), "Alert", "alert('" + result + "');", True)
            AdresseGlnOEMPanel.Update()
        End If
        watch.Stop()
        ClsHelper.Log("TPAdressgln Save changes", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Protected Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs)

        PanelAddGLNOEM.Visible = False
        BtnNew.Visible = True
        BtnSave.Visible = False
        BtnCancel.Visible = False
        txtOEM.Text = ""
        txtGLN.Text = ""
        AdresseGlnOEMPanel.Update()

    End Sub

    Protected Sub BtnDelete_Click(ByVal sender As System.Object, e As CommandEventArgs)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim commandArgs As String = e.CommandArgument.ToString()
        Dim result As String = DeleteGLNCode(tpcid.Value, commandArgs)
        If (result = "") Then
            result = "Deleted"
        End If
        ScriptManager.RegisterStartupScript(AdresseGlnOEMPanel, AdresseGlnOEMPanel.GetType(), "Alert", "alert('" + result + "');", True)
        LoadTPCustomerGLNAddress(tpcid.Value)
        AdresseGlnOEMPanel.Update()
        watch.Stop()
        ClsHelper.Log("TPAdressgln Delete code", ClsSessionHelper.LogonUser.GlobalID.ToString(), "", watch.ElapsedMilliseconds, False, Nothing)

    End Sub

    Private Sub LoadTPCustomerGLNAddress(ByVal CustomerID As Integer)

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        Dim newRow As DataRow
        Dim colKeys(1) As DataColumn

        Dim tblTPCustomerEANCode As New DataTable("TPCustomerGLNAddress")

        tblTPCustomerEANCode.Columns.Add("GLN", GetType(System.String))
        tblTPCustomerEANCode.Columns.Add("ADDRESSKEY", GetType(System.String))
        colKeys(0) = tblTPCustomerEANCode.Columns("C_EAN")
        tblTPCustomerEANCode.PrimaryKey = colKeys

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()

        cmd = New SqlClient.SqlCommand("PS_TPCUSTOMERCODES", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", CustomerID)
        cmd.Parameters.AddWithValue("@CountryIsoCode", TradePlaceCustomer.GetddlCountrySelectedValue())
        cmd.Parameters.AddWithValue("@TypeCode", "GLN")

        MyDataReader = cmd.ExecuteReader()

        While MyDataReader.Read()
            newRow = tblTPCustomerEANCode.NewRow
            newRow.Item("GLN") = MyDataReader("GLN").ToString
            newRow.Item("ADDRESSKEY") = MyDataReader("ADDRESSKEY").ToString
            tblTPCustomerEANCode.Rows.Add(newRow)
        End While

        dtlTPCustomerGLNOEM.DataSource = tblTPCustomerEANCode
        dtlTPCustomerGLNOEM.DataBind()

        tpcid.Value = CustomerID

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
    End Sub

    Private Function AddGLNCode(ByVal TPC_ID As Integer, ByVal AddressGLNCode As String, ByVal AddressOEMCode As String, ByRef StrReturn As String) As Boolean

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim Result As Boolean

        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PI_ADDRESSGLNOEM_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@GLNCode", AddressGLNCode)
        cmd.Parameters.AddWithValue("@OEMCode", AddressOEMCode)
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
                    StrReturn = "Unabled to create new GLN-OEM translation!"
                    Result = False

                Case 2
                    StrReturn = "GLN-OEM translation already exists in the system for another customer. Creation not carried out."
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
                ' Erreur non identifiée dans la PS
                ' On remonte l'erreur
                StrReturn = Except.Message
                Throw Except
            End If
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
        AddGLNCode = Result
    End Function

    Private Function DeleteGLNCode(ByVal TPC_ID As Integer, ByVal GLNCode As String) As String

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim strReturn As String = ""
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(TradePlaceCustomer.GetddlEnvironmentSelectedValue()))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("PD_ADDRESSGLNOEM_V2", cnx)
        cmd.CommandType = CommandType.StoredProcedure

        cmd.Parameters.AddWithValue("@TPC_ID", TPC_ID)
        cmd.Parameters.AddWithValue("@GLNCode", GLNCode)
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
                DeleteGLNCode = Except.Message
                Throw Except
            End If
        End Try

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

        DeleteGLNCode = strReturn
    End Function
End Class
