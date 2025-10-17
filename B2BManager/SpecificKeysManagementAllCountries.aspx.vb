
Imports System.Data
Imports System.Data.SqlClient

Partial Class SpecificKeysManagementAllCountries
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim KeyName As String
        KeyName = Request.QueryString("KeyName")
        Dim ddlCountry As String
        ddlCountry = Request.QueryString("ddlCountry")
        Dim ddlEnvironment As String
        ddlEnvironment = Request.QueryString("ddlEnvironment")

        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Show all countries values"

            If KeyName <> "" And ddlCountry <> "" And ddlEnvironment <> "" Then
                BindData(KeyName, ddlCountry, ddlEnvironment)
            End If
        End If

    End Sub

    Private Sub BindData(KeyName As String, ddlCountry As String, ddlEnvironment As String)
        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand
            Dim Adapter As SqlDataAdapter
            Dim ds As DataSet

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[Ebusiness].[SmsMgmt_GetCountriesValueByKeyName]", cnx)
            cmd.Parameters.AddWithValue("@SOPID", ddlCountry)
            cmd.Parameters.AddWithValue("@EnvironmentID", ddlEnvironment)
            cmd.Parameters.AddWithValue("@Key", KeyName)
            cmd.CommandType = CommandType.StoredProcedure
            Adapter = New SqlDataAdapter(cmd)
            ds = New DataSet
            Adapter.Fill(ds, "Result")

            Dim i As Integer = 0
            For Each dr In ds.Tables(0).Rows
                Dim trow As New TableRow
                trow.ClientIDMode = ClientIDMode.Static
                trow.ID = "Line_" + dr("KeyName").ToString() + "_" + dr("CountryName").ToString
                For Each dc In ds.Tables(0).Columns
                    Dim tcell As New TableCell
                    If (i = 0) Then
                        Dim img As New HtmlImage()
                        If (dr("CountryName").ToString.ToLower <> "all") Then
                            img = New HtmlImage()
                            img.Src = "Images/delete.png"
                            'img.Attributes.Add("onclick", "alert('Delete: Key :" + dr("KeyName").ToString() + " Country:" + ddlCountry.SelectedValue + " Env:" + ddlEnvironment.SelectedValue + "');")
                            'img.Attributes.Add("onclick", "Delete('" + dr("KeyName").ToString() + "','" + dr("CountryName").ToString() + "');")
                            If ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SMS_KEY) Then
                                img.Attributes.Add("onclick", "Delete('" + dr("KeyName").ToString() + "','" + dr("CountryName").ToString + "','" + ddlEnvironment + "','" + "Line_" + dr("KeyName").ToString() + "_" + dr("CountryName").ToString + "');")
                                img.Attributes.Add("class", "width20px LineChartImg")
                            Else
                                img.Attributes.Add("class", "width20px ImgDisabled")
                            End If
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            trow.Cells.Add(tcell)
                        Else

                            img = New HtmlImage()
                            img.Src = "Images/delete.png"
                            If ClsSessionHelper.LogonUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SMS_KEY) Then
                                img.Attributes.Add("onclick", "DeleteAll('" + dr("KeyName").ToString() + "','" + ddlEnvironment + "');")
                                img.Attributes.Add("class", "width20px LineChartImg")
                            Else
                                img.Attributes.Add("class", "width20px ImgDisabled")
                            End If
                            tcell.Attributes.Add("class", "TextAlignCenter")
                            tcell.Controls.Add(img)
                            trow.Cells.Add(tcell)
                        End If
                    End If
                    If (dc.ColumnName.ToString = "CountryName" And dr("CountryName").ToString.ToLower <> "all") Then
                        Dim img As New HtmlImage()
                        img = New HtmlImage()
                        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 10)).SingleOrDefault()
                        Dim sopID As String = ""
                        If selectedApplication.Countries.Where(Function(fn) (fn.CY_NAME_ISOCODE = dr(dc.ColumnName).ToString)).SingleOrDefault() IsNot Nothing Then
                            sopID = selectedApplication.Countries.Where(Function(fn) (fn.CY_NAME_ISOCODE = dr(dc.ColumnName).ToString)).SingleOrDefault().SOP_ID
                        Else
                            sopID = dr(dc.ColumnName).ToString()
                        End If
                        img.Src = "Images/FlagsSop/" + sopID + ".png"
                        img.Attributes.Add("class", "width20px LineChartImg flag")
                        tcell.Controls.Add(img)

                        Dim label As New Label()
                        label.Text = dr(dc.ColumnName).ToString
                        label.CssClass = "flagLabel"
                        tcell.Controls.Add(label)
                        trow.Cells.Add(tcell)
                    Else
                        tcell = New TableCell()
                        tcell.Controls.Add(New LiteralControl(dr(dc.ColumnName).ToString))
                        trow.Cells.Add(tcell)
                    End If
                    i += 1
                Next
                i = 0
                JQueryDataTable.Rows.Add(trow)
            Next

        Catch ex As Exception
            Dim toto As String = ex.Message
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteLine(EnvironmentID As String, SopId As String, KeyName As String)

        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[Ebusiness].[SmsMgmt_DeleteCountryValue]", cnx)
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@SOPID", SopId)
            cmd.Parameters.AddWithValue("@EnvironmentID", EnvironmentID)
            cmd.Parameters.AddWithValue("@Key", KeyName)
            cmd.ExecuteNonQuery()


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Sub DeleteAllLine(EnvironmentID As String, KeyName As String)

        Try
            Dim cnx As SqlConnection
            Dim cmd As SqlCommand

            cnx = New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)
            cnx.Open()
            cmd = New SqlClient.SqlCommand("[Ebusiness].[SmsMgmt_DeleteAllKeysValue]", cnx)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@EnvironmentID", EnvironmentID)
            cmd.Parameters.AddWithValue("@Key", KeyName)
            cmd.ExecuteNonQuery()


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

End Class
