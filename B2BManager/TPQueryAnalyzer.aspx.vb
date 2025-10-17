
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports Telerik.Web.UI

Partial Class TPQueryAnalyzer
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not IsPostBack Then
            RenderEnvironnementDropDown()
            LoadListBusinessMessage()
            LoadListBusinessStatus()
            RenderCountryDropDown()
        End If
    End Sub

    Protected Sub BtnSearch_Click(ByVal sender As System.Object, ByVal e As EventArgs)
        ResultPanel.Visible = True
        Dim query As String = CType(SearchCriteriaPanel.FindControl("GeneratedQuery"), HtmlTextArea).InnerText
        Dim Histo As Boolean = CType(SearchCriteriaPanel.FindControl("chkDatabaseHisto"), CheckBox).Checked
        Search(query, Histo)
        ScriptManager.RegisterStartupScript(LogViewerUpdatePanel, LogViewerUpdatePanel.GetType(), "BindDataTable", "BindDataTable();", True)
        LogViewerUpdatePanel.Update()
    End Sub

    Private Sub LoadListBusinessMessage()

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(ddlEnvironment.SelectedValue))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("SELECT * FROM V_BUSINESSMSGViewer", cnx)
        cmd.CommandType = CommandType.Text

        MyDataReader = cmd.ExecuteReader()

        ddBusiMess.DataSource = MyDataReader
        ddBusiMess.DataValueField = "BUSINESSMSG"
        ddBusiMess.DataTextField = "BUSINESSMSGDefinition"
        ddBusiMess.DataBind()


        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        If Not cnx Is Nothing Then
            cnx.Close()
        End If

    End Sub

    Private Sub LoadListBusinessStatus()

        Dim cnx As SqlConnection
        Dim cmd As SqlCommand
        Dim MyDataReader As SqlDataReader
        'cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SQLCnx_Test").ConnectionString)
        cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetConectionString(ddlEnvironment.SelectedValue))
        cnx.Open()
        cmd = New SqlClient.SqlCommand("SELECT * FROM V_BUSINESSSTATUS", cnx)
        cmd.CommandType = CommandType.Text

        MyDataReader = cmd.ExecuteReader()

        ddBusiStatus.DataSource = MyDataReader
        ddBusiStatus.DataValueField = "BUSINESSSTATUS"
        ddBusiStatus.DataTextField = "BUSINESSSTATUSDefinition"
        ddBusiStatus.DataBind()


        If Not MyDataReader Is Nothing Then
            MyDataReader.Close()
        End If

        If Not cnx Is Nothing Then
            cnx.Close()
        End If
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

        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, ddlCountry.SelectedValue, selectedApplication.SelectAllCountriesByDefault, False, True)

        If selectedApplication.Countries.Where(Function(fc) fc.Checked).Count > 1 Then
            If selectedApplication.Countries.Where(Function(fc) fc.Checked).Count < 30 Then
                countryFilter.Value = " Country IN ('" + String.Join("','", selectedApplication.Countries.Where(Function(fc) fc.Checked).Select(Function(fc) fc.CY_NAME_ISOCODE)) + "')"
            End If
        End If
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

        ddlEnvironment.SelectedIndex = 0
    End Sub

    Private Sub Search(ByVal query As String, ByVal Histo As Boolean)

        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim MyDataReader As SqlDataReader
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet

            If (Histo) Then
                cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetTPHistoConectionString(ddlEnvironment.SelectedValue))
            Else
                cnx = New SqlClient.SqlConnection(ClsEManagerHelper.GetTPConectionString(ddlEnvironment.SelectedValue))
            End If
            cnx.Open()

            cmd = New SqlClient.SqlCommand(query, cnx)
            cmd.CommandType = CommandType.Text

            'MyDataReader = cmd.ExecuteReader()

            'dgResult.DataSource = MyDataReader
            'dgResult.DataBind()
            'RowCount.InnerText = dgResult.Items.Count

            'If Not MyDataReader Is Nothing Then
            '    MyDataReader.Close()
            'End If

            'If Not cnx Is Nothing Then
            '    cnx.Close()
            'End If

            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")
            Dim i As Integer = 0
            For Each dr In ds.Tables(0).Rows
                Dim trow As New TableRow
                For Each dc In ds.Tables(0).Columns
                    If (dc.ColumnName.ToString <> "Status") Then
                        Dim tcell As New TableCell
                        If (i = 0) Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/XML.png"
                            Dim onclick As String = "OpenViewXMLFilesWindow('10','" + dr("BUSIMESSAGE").ToString() + "','" + ddlEnvironment.SelectedValue.ToString() + "','" + dr("Corid").ToString() + "','','','','" + dr("id").ToString() + "')"
                            img.Attributes.Add("onclick", onclick)
                            img.Attributes.Add("class", "MoreInfoImg")

                            tcell.Controls.Add(img)
                            trow.Cells.Add(tcell)
                        End If
                        tcell = New TableCell()
                        If (dc.ColumnName = "country") Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/Flags/" + dr(dc.ColumnName).ToString.Trim + ".png"
                            img.Width = 20
                            img.Height = 16
                            tcell.Controls.Add(img)
                        ElseIf (dc.ColumnName = "Busistatus") Then
                            Dim img As New HtmlImage()
                            img.Src = "Images/" + dr(dc.ColumnName).ToString.Trim + ".gif"
                            img.Width = 16
                            img.Height = 16
                            tcell.Controls.Add(img)
                        Else
                            tcell.Controls.Add(New LiteralControl(dr(dc.ColumnName).ToString))
                        End If
                        If (dr("BUSIMESSAGE") = "ORDERPLACEMENT") Then
                            If (dr("BusiStatus").ToString = "OK") Then
                                trow.BackColor = Color.LightGreen
                            Else
                                trow.BorderStyle = BorderStyle.Solid
                                trow.BorderColor = Color.Red
                                trow.BorderWidth = 2
                            End If
                        End If
                        trow.Cells.Add(tcell)
                        i += 1
                    End If
                Next
                i = 0
                PocJQueryDataTable.Rows.Add(trow)
            Next
        Catch ex As Exception

        End Try

    End Sub
    'Protected Sub dgResult_ItemDataBound(sender As Object, e As GridItemEventArgs)

    '    If TypeOf e.Item Is GridDataItem Then
    '        Dim cellValue As HtmlGenericControl = CType(e.Item.FindControl("BusimessageLabel"), HtmlGenericControl)
    '        If (cellValue.InnerText = "ORDERPLACEMENT") Then
    '            Dim BusiStatus As HiddenField = CType(e.Item.FindControl("BusiStatus"), HiddenField)
    '            If (BusiStatus.Value = "OK") Then
    '                e.Item.BackColor = Color.LightGreen
    '            Else
    '                e.Item.BorderStyle = BorderStyle.Solid
    '                e.Item.BorderColor = Color.Red
    '                e.Item.BorderWidth = 2
    '            End If
    '        End If
    '        End If
    'End Sub
End Class
