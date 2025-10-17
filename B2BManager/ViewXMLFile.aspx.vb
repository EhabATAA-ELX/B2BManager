
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics

Partial Class ViewXMLFile
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ID As Integer = 0
        Dim type As Integer = 0
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If Not Request.QueryString("ID") Is Nothing Then
            Integer.TryParse(Request.QueryString("ID"), ID)
        End If

        If Not Request.QueryString("Type") Is Nothing Then
            Integer.TryParse(Request.QueryString("Type"), type)
        End If

        Dim XMLFileSessionName = "XML_" + ID.ToString()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()

        If Session(XMLFileSessionName) Is Nothing And ID > 0 Then

            parameters.Add(New SqlParameter("ID", ID))
            parameters.Add(New SqlParameter("Type", type))
            Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Logger.GetXMLFileByID", parameters)
            If (dataTable.Rows.Count > 0) Then
                Session(XMLFileSessionName) = ClsHelper.PrettyXml(dataTable.Rows(0)("MSG_XML").ToString())
            End If

        End If
        If String.IsNullOrEmpty(Session(XMLFileSessionName)) Or ID = 0 Then
            divXMLFile.InnerHtml = "No data found"
            BtnCopyXML.Visible = False
            BtnViewXMLInBrowser.Visible = False
            BtnDownloadXML.Visible = False
        Else
            divXMLFile.InnerHtml = ClsHelper.FormatXMLinHTML(Session(XMLFileSessionName))
            BtnViewXMLInBrowser.Attributes.Add("onclick", "popup('GetXMLFile.ashx?file=" + XMLFileSessionName + "')")
            BtnDownloadXML.Attributes.Add("onclick", "donwloadXML('" + XMLFileSessionName + "')")
        End If

        watch.Stop()
        If ClsSessionHelper.LogonUser Is Nothing Then
            Response.Redirect("Login.aspx", True)
        Else
            If Not IsPostBack Then
                ClsHelper.Log(IIf(type = 0, "View Single XML File", "View Request XML"), ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            End If
        End If


    End Sub

End Class
