Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Threading
Imports System.Xml
Imports SAPRequestsLib.Models
Imports Telerik.Web.UI
Imports Newtonsoft.Json
Imports DataSource
Imports System.Security.Cryptography
Imports System.IO

Public Class ClsHelper


    Public Shared Function GetTime(ByVal Hour As String, ByVal Minute As String) As String
        Dim result As String = ""

        If Not String.IsNullOrEmpty(Hour) And Not String.IsNullOrEmpty(Minute) Then
            result = IIf(Hour.Length = 1, "0" & Hour, Hour) & ":" & IIf(Minute.Length = 1, "0" & Minute, Minute)
        End If

        Return result
    End Function

    Private Const HEADER_XML_IN_HTML As String = "<span style=""color:brown""><span style=""color:mediumblue"">&lt;</span>?xml<span style=""color:red""> version<span style=""color:mediumblue"">=""1.0""</span> encoding<span style=""color:mediumblue"">=""UTF-8<strong>""</strong></span>?</span><span style=""color:mediumblue"">&gt;</span></span>"
    Private Const HEADER_XML As String = "<?xml version=""1.0"" encoding=""UTF-8""?>"

    Public Shared Function FormatXMLinHTML(ByVal xml As String) As String
        Dim result As String = "&nbsp;&nbsp;Nothing to show"
        If Not String.IsNullOrEmpty(xml) Then
            result = HEADER_XML_IN_HTML + xml.Replace(HEADER_XML, "").Replace(" ", "&nbsp;") _
                        .Replace("<", "<span style='color:brown'&lta;&lt;") _
                        .Replace(">", "<span style='color:mediumblue'>&gt;</span></span>") _
                        .Replace("&lta;", ">") _
                        .Replace("&lt;", "<span style='color:mediumblue'>&lt;</span>") _
                        .Replace(vbNewLine, "<br/>")
        End If
        Return result
    End Function

    Public Shared Function FormatJsoninHTML(ByVal Json As String) As String
        Dim result As String = "&nbsp;&nbsp;Nothing to show"
        If Not String.IsNullOrEmpty(Json) Then
            result = Json.Replace(" ", "&nbsp;") _
                        .Replace("<", "<span style='color:brown'&lta;&lt;") _
                        .Replace(">", "<span style='color:mediumblue'>&gt;</span></span>") _
                        .Replace("&lta;", ">") _
                        .Replace("&lt;", "<span style='color:mediumblue'>&lt;</span>") _
                        .Replace(vbNewLine, "<br/>")
        End If
        Return result
    End Function



    Public Shared Function GetInstanceName(Optional isBasicMasterPage As Boolean = False) As String
        Dim instanceName As String = "B2B Manager"
        Dim instanceSubName As String = Nothing
        Dim isLiteVersion As Boolean = False
        Boolean.TryParse(ConfigurationManager.AppSettings("IsLiteVersion"), isLiteVersion)
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("InstanceName")) Then
            instanceName = ConfigurationManager.AppSettings("InstanceName")
        End If

        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("InstanceSubName")) Then
            instanceSubName = ConfigurationManager.AppSettings("InstanceSubName")
        Else
            If isLiteVersion Then
                instanceSubName = "Lite"
            End If
        End If

        If Not String.IsNullOrEmpty("InstanceSubName") Then
            If isBasicMasterPage Then
                Return "<div class=""custom-info-container"" title=""" & instanceName & """ style=""display: inline-block;margin-top: -10px;""> " &
                                                                                                "<div title=""" & instanceName & """ class=""name"">" & instanceName & "</div>" &
                                                                                                "<span class=""custom-info-instance-name-basic""><span>" & instanceSubName & "</span></span></div>"
            Else
                Return instanceName & "<span class=""custom-info-instance-name""><span>" & instanceSubName & "</span>"
            End If
        Else
            Return instanceName
        End If
    End Function

    Public Shared Function GetJson(ByVal dt As DataTable, Optional format As Newtonsoft.Json.Formatting = Newtonsoft.Json.Formatting.Indented) As String
        Return JsonConvert.SerializeObject(dt, format)
        'Return New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
    End Function

    Public Shared Function GetDataTablesSource(Of T)(ByVal dt As DataTable, Optional format As Newtonsoft.Json.Formatting = Newtonsoft.Json.Formatting.Indented) As DataSource(Of T())
        Dim dataTablesSource As DataSource(Of T()) = New DataSource(Of T())
        dataTablesSource.data = JsonConvert.DeserializeObject(Of T())(JsonConvert.SerializeObject(dt, format))
        Return dataTablesSource
    End Function

    Public Shared Function RemoveOddCharachters(expression As String) As String
        Dim objRegex As Object = CreateObject("VBScript.RegExp")
        With objRegex
            .Pattern = "(\\|/|<|>|\|\|\?|:|\.|\*|\$)"
            .Global = True
            .IgnoreCase = True
        End With
        Return objRegex.Replace(expression, "_")
    End Function

    Public Shared Function PrettyXml(ByVal xml As String) As String
        If String.IsNullOrEmpty(xml) Then
            Return ""
        Else
            Try
                Dim stringBuilder = New StringBuilder()
                Dim element = XElement.Parse(xml)
                Dim settings = New XmlWriterSettings()
                settings.Indent = True
                settings.Encoding = ASCIIEncoding.UTF8
                settings.OmitXmlDeclaration = False
                settings.NewLineOnAttributes = False
                Using MyXmlWriter As XmlWriter = XmlWriter.Create(stringBuilder, settings)
                    element.Save(MyXmlWriter)
                End Using

                Return stringBuilder.ToString().Replace("encoding=""utf-16""?>", "encoding=""UTF-8""?>")
            Catch ex As Exception
                Return xml
            End Try

        End If
    End Function

    Public Shared Function PrettyJsonInHtml(ByVal json As String) As String
        If String.IsNullOrEmpty(json) Then
            Return ""
        Else
            Return json.Replace(" ", "&ensp;") _
                       .Replace(vbCrLf, "</br>") _
                       .Replace("{", "<span style='color:#B4362D'>{</span>") _
                       .Replace("}", "<span style='color:#B4362D'>}</span>") _
                       .Replace("""", "<span style='color:#11328c;'>""</span>")
        End If
    End Function

    Public Shared Function GetMachineInformation() As String
        Return String.Format("Computer OSFullName: {0} OSPlatform: {1} OSVersion: {2} ",
                             IIf(Not My.Computer.Info.OSFullName Is Nothing, My.Computer.Info.OSFullName, ""),
                             IIf(Not My.Computer.Info.OSPlatform Is Nothing, My.Computer.Info.OSPlatform, ""),
                             IIf(Not My.Computer.Info.OSVersion Is Nothing, My.Computer.Info.OSVersion, ""))
    End Function

    Public Shared Function GetMonitoringEnvironments() As List(Of BasicModel)
        Dim enivronments As List(Of BasicModel) = New List(Of BasicModel)
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Monitoring.GetEnvironments")
        For Each row As DataRow In dataTable.Rows
            enivronments.Add(New BasicModel(row("EnvironmentID"), row("Name").ToString(), True, True))
        Next
        Return enivronments
    End Function

    Public Shared Function AddMonitoringWorkflow(UserID As Integer, EnvironmentID As Integer, Name As String) As Integer
        Dim workflowID As Integer
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@UserID", UserID))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@Name", IIf(String.IsNullOrWhiteSpace(Name), String.Format("Workflow created on {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm")), Name)))
        Integer.TryParse(ClsDataAccessHelper.ExecuteScalar("Monitoring.AddMonitoringWorkflow", parameters), workflowID)
        Return workflowID
    End Function

    Public Shared Function AddMonitoringAction(parameters As List(Of SqlParameter)) As Integer
        Dim actionID As Integer
        Integer.TryParse(ClsDataAccessHelper.ExecuteScalar("Monitoring.SaveOrUpdateAction", parameters), actionID)
        Return actionID
    End Function

    Public Shared Function DeleteMonitoringWorkflow(UserID As Integer, WorkflowID As Integer) As Boolean
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@UserID", UserID))
        parameters.Add(New SqlParameter("@WorkflowID", WorkflowID))
        Return ClsDataAccessHelper.ExecuteNonQuery("Monitoring.DeleteWorkflow", parameters)
    End Function

    Public Shared Function ConvertSQLParametersToFriendlyText(ByVal parameters As List(Of SqlParameter)) As String
        Dim resultStr As String = ""
        If parameters IsNot Nothing Then
            For Each parameter As SqlParameter In parameters
                Dim parameterValue As String = String.Empty
                If DBNull.Value.Equals(parameter.Value) Or parameter.Value Is Nothing Then
                    parameterValue = ""
                Else
                    parameterValue = parameter.Value.ToString()
                End If
                resultStr = IIf(resultStr.Length = 0, resultStr, resultStr & "</br>") & "<b>" & parameter.ParameterName.Replace("@", "") & ":</b> " & parameterValue
            Next
        End If
        Return resultStr
    End Function

    Public Shared Sub RenderCountryDropDown(ddlCountry As RadComboBox, countries As List(Of Country), selectedValue As String, appendAllCountries As Boolean, Optional selectFirstItem As Boolean = False, Optional useCountryISOCode As Boolean = False)
        ddlCountry.Items.Clear()
        If appendAllCountries AndAlso countries.Count > 0 Then
            If countries.Where(Function(fc) fc.Checked).Count > 1 Then
                Dim allCountriesSOPs As String = "All"
                If countries.Where(Function(fc) fc.Checked).Count < 30 Then
                    If useCountryISOCode Then
                        allCountriesSOPs = String.Join(",", countries.Where(Function(fc) fc.Checked).Select(Function(fc) fc.CY_NAME_ISOCODE))
                    Else
                        allCountriesSOPs = String.Join(",", countries.Where(Function(fc) fc.Checked).Select(Function(fc) fc.SOP_ID))
                    End If
                End If
                ddlCountry.Items.Insert(0, New RadComboBoxItem("All", allCountriesSOPs))
            End If
        End If
        Dim index As Integer = 0
        Dim ValueExists As Boolean = False
        For Each country As Country In countries
            If country.Checked Then
                Dim item As RadComboBoxItem
                If useCountryISOCode Then
                    item = New RadComboBoxItem(country.Name, country.CY_NAME_ISOCODE)
                Else
                    item = New RadComboBoxItem(country.Name, country.SOP_ID)
                End If
                item.ImageUrl = country.ImageURL
                ddlCountry.Items.Add(item)
                If selectFirstItem AndAlso index = 0 Then
                    ddlCountry.SelectedValue = item.Value
                Else
                    If country.SOP_ID = selectedValue Or country.Name = selectedValue Then
                        ValueExists = True
                        ddlCountry.SelectedValue = item.Value
                    End If
                End If
                index += 1
            End If
        Next
        If ddlCountry.Items.Count > 1 And Not ValueExists Then
            ddlCountry.SelectedValue = selectedValue
        End If

    End Sub


    Public Shared Sub RenderDropDownList(ByVal DropDownList As DropDownList, ByVal basicModels As List(Of BasicModel), Optional clearItems As Boolean = False, Optional appendDataBoundItem As Boolean = False, Optional SelectedValue As String = "All")
        If DropDownList.SelectedItem IsNot Nothing Then
            SelectedValue = DropDownList.SelectedItem.Text
        End If

        If (clearItems) Then
            DropDownList.Items.Clear()
        End If
        Dim index As Integer = 0
        If (appendDataBoundItem) And basicModels.Count > 1 Then
            DropDownList.Items.Insert(index, New ListItem("All", "0"))
        End If
        If DropDownList.ID = "ddlEnvironment" Then
            For Each item As BasicModel In basicModels
                If item.Checked Then
                    DropDownList.Items.Add(New ListItem(item.Name, item.ID.ToString()))
                End If
                If item.Name = SelectedValue Or item.ID.ToString() = SelectedValue Then
                    DropDownList.SelectedValue = item.ID.ToString()
                End If
            Next
        Else
            For Each item As BasicModel In basicModels
                DropDownList.Items.Add(New ListItem(item.Name, item.ID.ToString()))
                DropDownList.SelectedIndex = 0
            Next
        End If

    End Sub

    Public Shared Function ValidateUser(ByVal UserName As String, ByVal Password As String, Optional UserGlobalID As String = Nothing) As ClsUser
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        If UserGlobalID IsNot Nothing Then
            parameters.Add(New SqlParameter("@UserglobalID", UserGlobalID))
        Else
            parameters.Add(New SqlParameter("UserName", UserName))
            parameters.Add(New SqlParameter("Password", Password))
        End If
        Dim userDataTable As DataTable = ClsDataAccessHelper.FillDataTable(IIf(String.IsNullOrEmpty(UserGlobalID), "Logger.ValidateUser", "Logger.AuthenticateUser"), parameters)
        Dim user As ClsUser = Nothing
        If userDataTable IsNot Nothing Then
            If userDataTable.Rows.Count = 1 Then
                user = New ClsUser()
                Dim userRow As DataRow = userDataTable.Rows(0)
                user.ID = CInt(userRow("U_ID"))
                user.GlobalID = New Guid(userRow("U_GLOBALID").ToString())
                If userRow("U_Title") IsNot DBNull.Value Then
                    user.Title = CInt(userRow("U_TITLE"))
                End If
                user.Login = userRow("U_LOGIN").ToString()
                user.Password = userRow("U_PASSWORD").ToString()
                user.FirstName = userRow("U_FIRSTNAME").ToString()
                user.LastName = userRow("U_LASTNAME").ToString()
                user.FullName = userRow("U_FULLNAME").ToString()
                user.Email = userRow("U_Email").ToString()
                user.NickName = userRow("U_NickName").ToString()
                user.DefaultSOPIDs = ClsDataAccessHelper.GetText(userRow, "DefaultSOPIDs")

                If userRow("HomePageToolID") IsNot DBNull.Value Then
                    user.HomePageToolID = CInt(userRow("HomePageToolID"))
                End If
                If userRow("DefaultDashboardID") IsNot DBNull.Value Then
                    user.DefaultDashboardID = CInt(userRow("DefaultDashboardID"))
                End If
                If userRow("HomePageChartID") IsNot DBNull.Value Then
                    user.HomePageChartID = CInt(userRow("HomePageChartID"))
                End If
                If userRow("DefaultEnvironmentID") IsNot DBNull.Value Then
                    user.DefaultEnvironmentID = CInt(userRow("DefaultEnvironmentID"))
                End If

                If userRow("DefaultEbusinessManagementType") IsNot DBNull.Value Then
                    user.DefaultEbusinessManagementType = CInt(userRow("DefaultEbusinessManagementType"))
                End If

                If userRow("DefaultCountrtySplitStatus") IsNot DBNull.Value Then
                    user.DefaultCountrtySplitStatus = CBool(userRow("DefaultCountrtySplitStatus"))
                End If

                If userRow("IsAscendingSotring") IsNot DBNull.Value Then
                    user.IsAscendingSotring = CBool(userRow("IsAscendingSotring"))
                Else
                    user.IsAscendingSotring = True
                End If

                user.DefaultSortingFieldAlias = ClsDataAccessHelper.GetText(userRow, "DefaultSortingFieldAlias", "Customer Name")
                user.DefaultEbusinessSopID = ClsDataAccessHelper.GetText(userRow, "DefaultEbusinessSopID")

                If userRow("DefaultEbusinessEnvironmentID") IsNot DBNull.Value Then
                    user.DefaultEbusinessEnvironmentID = CInt(userRow("DefaultEbusinessEnvironmentID"))
                End If

                If userRow("ActivateWindowModeByDefault") IsNot DBNull.Value Then
                    user.ActivateWindowModeByDefault = CBool(userRow("ActivateWindowModeByDefault"))
                End If

                If userRow("ExpandRowsOnSearchByDefault") IsNot DBNull.Value Then
                    user.ExpandRowsOnSearchByDefault = CBool(userRow("ExpandRowsOnSearchByDefault"))
                End If
            End If
        End If
        Return user
    End Function

    Public Shared Function GetMonitoringActionTypes() As List(Of MonitoringActionTypeInfo)
        Return SAPRequestsLib.MonitoringWorkflowManager.GetMonitoringActionTypes(ConfigurationManager.ConnectionStrings("LogDb").ConnectionString,
                                                                             ConfigurationManager.AppSettings("ErrorEmailFrom").ToString(),
                                                                             ConfigurationManager.AppSettings("ErrorEmailTo").ToString(),
                                                                             ConfigurationManager.AppSettings("SMTPServer").ToString())
    End Function

    Public Shared Function ToReadableString(ByVal span As TimeSpan) As String
        Dim formatted As String
        If span.Duration().Days > 1 Then
            formatted = String.Format("about {0:0} days ago.", span.Days)
        ElseIf span.Duration().Hours > 1 Then
            formatted = String.Format("about {0:0} hours ago.", span.Hours)
        ElseIf span.Duration().Hours = 1 Then
            formatted = "about one hour ago."
        Else
            formatted = "few minutes ago."
        End If

        If String.IsNullOrEmpty(formatted) Then
            formatted = "few seconds ago."
        End If

        Return formatted
    End Function

    Public Shared Function GetUserInformation(ByVal UserID As Integer, ByRef Tools As List(Of Tool), ByRef Actions As List(Of ActionDesignation), ByRef Links As List(Of Link)) As List(Of Application)
        Dim applications As List(Of Application) = New List(Of Application)()
        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@UserID", UserID))
            Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Logger].[GetUserInformation]", parameters)
            If dataSet.Tables.Count >= 4 Then
                For Each applicationRow As DataRow In dataSet.Tables(0).Rows
                    Dim applicationID As Integer = CInt(applicationRow("ID"))
                    Dim application As New Application(applicationID, applicationRow("ApplicationName").ToString(), CBool(applicationRow("AppendAllItemInActions")), CBool(applicationRow("SelectAllCountriesByDefault")), CBool(applicationRow("ShowCorrelIDColumn")), CBool(applicationRow("ShowPOIDColumn")), CBool(applicationRow("ShowSalesOrderIDColumn")), CBool(applicationRow("Checked")), CBool(applicationRow("Enabled")), CBool(applicationRow("ShowCustomerCodeColumn")), CBool(applicationRow("ShowViewMessageXMLColumn")), CBool(applicationRow("IsMessageXML")), CBool(applicationRow("ShowHubspanIdColumn")), CBool(applicationRow("ShowDateReceivedColumn")))
                    If applicationRow.Table.Columns.Contains("UseEnvironmentConnectionString") Then
                        application.UseEnvironmentConnectionString = CBool(applicationRow("UseEnvironmentConnectionString"))
                    Else
                        application.UseEnvironmentConnectionString = False
                    End If
                    application.Countries = New List(Of Country)()
                    application.Environments = New List(Of BasicModel)
                    application.Actions = New List(Of Action)
                    For Each actionRow As DataRow In dataSet.Tables(1).Rows
                        If CInt(actionRow("ApplicationID")) = applicationID Then
                            application.Actions.Add(New Action(CInt(actionRow("ID")), actionRow("ActionName").ToString(), CBool(actionRow("Checked")), CBool(actionRow("Enabled")), CInt(actionRow("CorrespondentActionID"))))
                        End If
                    Next
                    For Each environmentRow As DataRow In dataSet.Tables(2).Rows
                        If CInt(environmentRow("ApplicationID")) = applicationID Then
                            Dim Checked As Boolean = environmentRow("Checked")
                            Dim applicable As Boolean = True
                            If ConfigurationManager.AppSettings("deniedEnvironments") IsNot Nothing Then
                                If (ConfigurationManager.AppSettings("deniedEnvironments").Split(",").Contains(environmentRow("ID"))) Then
                                    applicable = False
                                End If
                            End If
                            If applicable Then
                                Dim environment As BasicModel = New BasicModel(CInt(environmentRow("ID")), environmentRow("Environment").ToString(), Checked, CBool(environmentRow("Enabled")), CBool(environmentRow("Is_EManager")))
                                environment.ConnectionString = ClsDataAccessHelper.GetText(environmentRow, "ConnectionString")
                                application.Environments.Add(environment)
                            End If
                        End If
                    Next
                    For Each countryRow As DataRow In dataSet.Tables(3).Rows
                        If CInt(countryRow("ApplicationID")) = applicationID Then
                            application.Countries.Add(New Country(CInt(countryRow("ID")), countryRow("Name").ToString(), New Guid(countryRow("CY_GLOBALID").ToString()), countryRow("CY_NAME"), countryRow("CY_NAME_ISOCODE").ToString(), countryRow("SOP_ID").ToString(), countryRow("ImageUrl").ToString(), CBool(countryRow("Checked")), CBool(countryRow("Enabled"))))
                        End If
                    Next
                    applications.Add(application)
                Next
            End If
            Dim toolsDt As DataTable = Nothing
            Dim actionsDt As DataTable = Nothing
            Dim linksDt As DataTable = Nothing
            If dataSet.Tables.Count >= 5 Then
                toolsDt = dataSet.Tables(4)
            End If
            If dataSet.Tables.Count >= 6 Then
                actionsDt = dataSet.Tables(5)
            End If

            If dataSet.Tables.Count >= 7 Then
                linksDt = dataSet.Tables(6)
            End If

            If toolsDt IsNot Nothing Then
                Tools = New List(Of Tool)
                For Each toolRow As DataRow In toolsDt.Rows
                    Dim parentToolID As Integer? = Nothing
                    If toolRow("ParentToolID") IsNot DBNull.Value Then
                        parentToolID = CInt(toolRow("ParentToolID"))
                    End If
                    Dim typeID As Integer = toolRow("TypeID")
                    If IsDebugMode() And typeID = 2 Then
                        typeID = 1
                    End If
                    Dim tool As New Tool(toolRow("ToolID"), parentToolID, toolRow("IconImagePath"), toolRow("Name"), toolRow("Url"), typeID, toolRow("MenuIconImagePath"), ClsDataAccessHelper.GetText(toolRow, "BadgeText", ""), ClsDataAccessHelper.GetText(toolRow, "BadgeColor", ""), toolRow("isFavorite"))
                    If actionsDt IsNot Nothing Then
                        For Each actionRow As DataRow In actionsDt.Select(String.Format("ToolID={0}", toolRow("ToolID")))
                            Dim actionDesinagtion As ActionDesignation = Nothing
                            If [Enum].TryParse(actionRow("ActionID"), actionDesinagtion) Then
                                If tool.Actions Is Nothing Then
                                    tool.Actions = New List(Of ActionDesignation)
                                End If
                                tool.Actions.Add(actionDesinagtion)
                            End If
                        Next
                    End If
                    Tools.Add(tool)
                Next
            End If

            If actionsDt IsNot Nothing Then
                For Each actionRow As DataRow In actionsDt.Rows
                    Dim actionDesinagtion As ActionDesignation = Nothing
                    If [Enum].TryParse(actionRow("ActionID"), actionDesinagtion) Then
                        If Actions Is Nothing Then
                            Actions = New List(Of ActionDesignation)
                        End If
                        Actions.Add(actionDesinagtion)
                    End If
                Next
            End If

            If linksDt IsNot Nothing Then
                For Each linkRow As DataRow In linksDt.Rows
                    If Links Is Nothing Then
                        Links = New List(Of Link)
                    End If
                    Dim displayOrder As Integer = 1
                    If linkRow("DisplayOrder") IsNot DBNull.Value Then
                        displayOrder = CInt(linkRow("DisplayOrder"))
                    End If
                    Links.Add(New Link(linkRow("LinkID"), ClsDataAccessHelper.GetText(linkRow, "LinkName"), ClsDataAccessHelper.GetText(linkRow, "LinkUrl"), ClsDataAccessHelper.GetText(linkRow, "LinkIconColor"), displayOrder))
                Next
            End If

        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetUserInformation</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return applications
    End Function

    Public Shared Function GetLinks(userID As Integer, Optional linkID As Integer? = Nothing) As List(Of Link)
        Dim Links As List(Of Link) = New List(Of Link)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserID", userID))
        parameters.Add(New SqlParameter("@LinkID", linkID))
        Dim linksDt As DataTable = ClsDataAccessHelper.FillDataTable("[Administration].GetLinks", parameters)
        For Each linkRow As DataRow In linksDt.Rows
            Dim displayOrder As Integer = 1
            If linkRow("DisplayOrder") IsNot DBNull.Value Then
                displayOrder = CInt(linkRow("DisplayOrder"))
            End If
            Links.Add(New Link(linkRow("LinkID"), ClsDataAccessHelper.GetText(linkRow, "LinkName"), ClsDataAccessHelper.GetText(linkRow, "LinkUrl"), ClsDataAccessHelper.GetText(linkRow, "LinkIconColor"), displayOrder))
        Next
        Return Links
    End Function

    Public Shared Function GetChatbotsInformation(ByVal UserID As Integer) As List(Of Chatbot)
        Dim chatbots As List(Of Chatbot) = New List(Of Chatbot)()
        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@UserID", UserID))
            Dim dataSetInformation As DataSet = ClsDataAccessHelper.FillDataSet("[ChatBot].[GetInformation]", parameters)
            If dataSetInformation.Tables.Count = 3 Then
                For Each chatBotRow As DataRow In dataSetInformation.Tables(0).Rows
                    Dim chatBotID As Integer = CInt(chatBotRow("ID"))
                    Dim chatbot As New Chatbot(chatBotID, chatBotRow("Name").ToString(), chatBotRow("ChannelID").ToString(), chatBotRow("Token").ToString(), chatBotRow("DeveloperToken").ToString(), CBool(chatBotRow("Checked")), CBool(chatBotRow("Enabled")))
                    chatbot.Countries = New List(Of Country)()
                    chatbot.Environments = New List(Of BasicModel)
                    For Each environmentRow As DataRow In dataSetInformation.Tables(1).Rows
                        If CInt(environmentRow("ChatBotID")) = chatBotID Then
                            chatbot.Environments.Add(New BasicModel(CInt(environmentRow("EnvironmentID")), environmentRow("Name").ToString(), CBool(environmentRow("Checked")), CBool(environmentRow("Enabled"))))
                        End If
                    Next
                    For Each countryRow As DataRow In dataSetInformation.Tables(2).Rows
                        If CInt(countryRow("ChatBotID")) = chatBotID Then
                            chatbot.Countries.Add(New Country(CInt(countryRow("ID")), countryRow("Name").ToString(), New Guid(countryRow("CY_GLOBALID").ToString()), countryRow("CY_NAME"), countryRow("CY_NAME_ISOCODE").ToString(), countryRow("SOP_ID").ToString(), countryRow("ImageUrl").ToString(), CBool(countryRow("Checked")), CBool(countryRow("Enabled"))))
                        End If
                    Next
                    chatbots.Add(chatbot)
                Next
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetChatbotsInformation</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
        Return chatbots
    End Function

    Public Shared Function GetElapsedTimeFont(elapsedTime As Object, Optional relaxSeconds As Integer = 0) As String
        Dim font As String = ""
        If Not elapsedTime Is DBNull.Value Then
            font = IIf(CInt(elapsedTime) > 3000 + (relaxSeconds * 1000) And CInt(elapsedTime) <= 10000 + (relaxSeconds * 1000), "orangefont", IIf(CInt(elapsedTime) > 10000 + (relaxSeconds * 1000), "fontred", ""))
        End If
        Return font
    End Function

    Public Shared Function IsDebugMode() As Boolean
        Dim debugMode As Boolean = False
        If ConfigurationManager.AppSettings("debugMode") IsNot Nothing Then
            If ConfigurationManager.AppSettings("debugMode").ToLower().Equals("on") Then
                debugMode = True
            End If
        End If
        Return debugMode
    End Function

    Public Shared Sub Log(ActionName As String, UserID As String, ActionDetails As String, ElapsedTime As Integer, HasError As Boolean, ErrorMessage As String)
        Try
            Dim logger As Logger = New Logger()
            Dim EnvironmentName As String = "NOT APPLICABLE"
            If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Environment")) Then
                EnvironmentName = ConfigurationManager.AppSettings("Environment")
            End If
            Dim thread As Thread = New Thread(Sub() logger.Log("Log Viewer", EnvironmentName, ActionName, My.Computer.Name, UserID, Guid.Empty.ToString(), GetMachineInformation(), ActionDetails, ElapsedTime, Nothing, HasError, ErrorMessage, Nothing))
            thread.Start()
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>Log (basic)</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

    Public Shared Sub LogEbusinessAction(EnvironmentID As Integer, SOPName As String, ActionName As String, UserID As Guid, ActionDetails As String, ElapsedTime As Integer, HasError As Boolean, ErrorMessage As String)
        Try
            Dim logger As Logger = New Logger()
            Dim thread As Thread = New Thread(Sub() LogEbusinessAction(EnvironmentID, ActionName, My.Computer.Name, UserID, SOPName, GetMachineInformation(), ActionDetails, ElapsedTime, HasError, ErrorMessage, Nothing))
            thread.Start()
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Methode Name:</b>Log (EbusinessAction)</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

    Public Shared Sub LogEbusinessAction(EnvironmentID As String, ActionName As String, MachineName As String, UserID As Guid, SOPName As String, MachineDetails As String, ActionDetails As String, ElapsedTime As Integer, HasError As Boolean, ErrorMessage As String, ErrorStackTrace As String)
        Try
            Using Cnx As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("LogDB").ConnectionString)
                Cnx.Open()
                Dim Cmd As SqlCommand = New SqlCommand("[Logger].[LogEbusinessAction]", Cnx)
                Cmd.CommandType = Data.CommandType.StoredProcedure
                Cmd.CommandTimeout = 6000
                ClsDataAccessHelper.AddParameter(Cmd, "MachineName", MachineName)
                ClsDataAccessHelper.AddParameter(Cmd, "MachineDetails", MachineDetails)
                ClsDataAccessHelper.AddParameter(Cmd, "UserID", UserID, SqlDbType.UniqueIdentifier)
                ClsDataAccessHelper.AddParameter(Cmd, "SOPName", SOPName)
                ClsDataAccessHelper.AddParameter(Cmd, "EnvironmentID", EnvironmentID)
                ClsDataAccessHelper.AddParameter(Cmd, "ActionName", ActionName)
                ClsDataAccessHelper.AddParameter(Cmd, "ActionDetails", ActionDetails)
                ClsDataAccessHelper.AddParameter(Cmd, "ElapsedTime", ElapsedTime, Data.SqlDbType.Int)
                ClsDataAccessHelper.AddParameter(Cmd, "HasError", HasError, Data.SqlDbType.Bit)
                ClsDataAccessHelper.AddParameter(Cmd, "ErrorMessage", ErrorMessage)
                ClsDataAccessHelper.AddParameter(Cmd, "ErrorStackTrace", ErrorStackTrace)
                Cmd.ExecuteNonQuery()
                Cnx.Close()
            End Using
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Method name:</b>Log</br><b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>Application Name: {2}</br>Environment: {3}</br>Action Name: {4}", exceptionMessage _
                        , exceptionStackTrace _
                        , "E-business Actions" _
                        , EnvironmentID _
                        , ActionName
                        )
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try

    End Sub

    Public Shared Function FindApplicationByID(ByRef applications As List(Of Application), id As Integer) As Application
        Dim result As Application = Nothing
        If Not applications Is Nothing Then
            For Each application As Application In applications
                If application.ID = id Then
                    result = application
                    Exit For
                End If
            Next
        End If
        Return result
    End Function

    Public Shared Function FindChatbotByID(ByRef chatbots As List(Of Chatbot), id As Integer) As Chatbot
        Dim result As Chatbot = Nothing
        If Not chatbots Is Nothing Then
            For Each application As Chatbot In chatbots
                If application.ID = id Then
                    result = application
                    Exit For
                End If
            Next
        End If
        Return result
    End Function

    Public Shared Function FindToolByID(ByRef tools As List(Of Tool), id As Integer) As Tool
        Dim result As Tool = Nothing
        If Not tools Is Nothing Then
            For Each tool As Tool In tools
                If tool.ToolID = id Then
                    result = tool
                    Exit For
                End If
            Next
        End If
        Return result
    End Function

    Public Shared Function FindToolByUrl(ByRef tools As List(Of Tool), url As String) As Tool
        Dim result As Tool = Nothing
        If Not tools Is Nothing Then
            For Each tool As Tool In tools
                If url.ToLower().StartsWith(tool.Url.ToLower()) Then
                    result = tool
                    Exit For
                End If
            Next
        End If
        Return result
    End Function

    Public Class TreeItem(Of T)
        Public Property Item As T
        Public Property Children As IEnumerable(Of TreeItem(Of T))
    End Class

    Public Shared Iterator Function GenerateTree(Of T, K)(ByVal collection As IEnumerable(Of T), ByVal id_selector As Func(Of T, K), ByVal parent_id_selector As Func(Of T, K), ByVal Optional root_id As K = Nothing) As IEnumerable(Of TreeItem(Of T))
        For Each c In collection.Where(Function(fc) parent_id_selector(fc).Equals(root_id))
            Yield New TreeItem(Of T) With {
            .Item = c,
            .Children = GenerateTree(collection, id_selector, parent_id_selector, id_selector(c))
        }
        Next
    End Function

    Public Shared Function Truncate(ByVal value As String, ByVal maxLength As Integer) As String
        If String.IsNullOrEmpty(value) Then
            Return value
        End If

        Return value.Substring(0, Math.Min(value.Length, maxLength))
    End Function

    Public Shared Function GetExportTemplates(UserID As Guid) As List(Of Template)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim listOfTemplates As List(Of Template) = New List(Of Template)()
        parameters.Add(New SqlParameter("@UserID", UserID))
        Dim templatesDataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.ExportBuilderGetTemplates", parameters)
        For Each templateRow As DataRow In templatesDataTable.Rows
            listOfTemplates.Add(New Template(templateRow("ID"), templateRow("Name").ToString(), templateRow("IsShared"), templateRow("DataSourceID"), templateRow("Checked"), templateRow("Enabled")))
        Next
        Return listOfTemplates
    End Function

    Public Shared Function GetCustomerAndUserFields(UserID As Guid) As List(Of Field)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim fields As List(Of Field) = New List(Of Field)()
        Dim templateInfo As TemplateInfo = Nothing
        parameters.Add(New SqlParameter("@UserID", UserID))
        Dim templatesDataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.UsrMgmt_GetCustomerAndUserFields", parameters)
        If templatesDataTable IsNot Nothing Then
            For Each fieldDataRow As DataRow In templatesDataTable.Rows
                fields.Add(New Field(fieldDataRow("ID"), fieldDataRow("FieldAlias").ToString(), fieldDataRow("Checked"), fieldDataRow("Enabled"), IIf(fieldDataRow("ParentID") Is DBNull.Value, Nothing, fieldDataRow("ParentID")), fieldDataRow("IconImageUrl")))
            Next
        End If
        Return fields
    End Function

    Public Shared Function GetExportData(UserID As Guid, templateID As Integer?, Optional dataSourceID As Integer = 1) As TemplateInfo
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim templateInfo As TemplateInfo = Nothing
        parameters.Add(New SqlParameter("@UserID", UserID))
        parameters.Add(New SqlParameter("@TemplateID", templateID))
        parameters.Add(New SqlParameter("@DataSourceID", dataSourceID))
        Dim templatesDataSet As DataSet = ClsDataAccessHelper.FillDataSet("Ebusiness.ExportBuilderGetData", parameters)
        If templatesDataSet.Tables.Count >= 2 Then
            Dim dataSources As List(Of BasicModel) = New List(Of BasicModel)()
            Dim fields As List(Of Field) = New List(Of Field)()
            For Each dataSourceRow As DataRow In templatesDataSet.Tables(0).Rows
                dataSources.Add(New BasicModel(dataSourceRow("ID"), dataSourceRow("Name").ToString(), dataSourceRow("Checked"), True))
            Next
            For Each fieldDataRow As DataRow In templatesDataSet.Tables(1).Rows
                fields.Add(New Field(fieldDataRow("ID"), fieldDataRow("FieldAlias").ToString(), fieldDataRow("Checked"), fieldDataRow("Enabled"), IIf(fieldDataRow("ParentID") Is DBNull.Value, Nothing, fieldDataRow("ParentID")), fieldDataRow("IconImageUrl")))
            Next
            templateInfo = New TemplateInfo()
            templateInfo.DataSources = dataSources
            templateInfo.Fields = fields

            If templatesDataSet.Tables.Count = 3 Then
                If templatesDataSet.Tables(2).Rows.Count > 0 Then
                    Dim templateDataRow As DataRow = templatesDataSet.Tables(2).Rows(0)
                    templateInfo.TemplateID = templateDataRow("ID")
                    templateInfo.TemplateName = templateDataRow("Name")
                    templateInfo.Enabled = templateDataRow("Enabled")
                    templateInfo.IsShared = templateDataRow("IsShared")
                    templateInfo.SelectedDataSourceID = templateDataRow("DataSourceID")
                End If
            End If
        End If
        Return templateInfo
    End Function

    Public Shared Function GenerateExportDataSource(UserID As Guid, ExportName As String, EnvironmentID As Integer, SOPIDs As String, TemplateName As String, IsShared As Boolean, DataSourceID As Integer, Fields As String, Optional TemplateID As Integer? = Nothing) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        Dim listOfTemplates As List(Of Template) = New List(Of Template)()
        parameters.Add(New SqlParameter("@UserID", UserID))
        parameters.Add(New SqlParameter("@ExportName", ExportName))
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@SOPIDs", SOPIDs))
        parameters.Add(New SqlParameter("@TemplateName", TemplateName))
        parameters.Add(New SqlParameter("@IsShared", IsShared))
        parameters.Add(New SqlParameter("@DataSourceID", DataSourceID))
        parameters.Add(New SqlParameter("@Fields", Fields))
        parameters.Add(New SqlParameter("@TemplateID", TemplateID))
        Dim resultDataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.ExportBuilderPrepareDataSource", parameters)
        Return resultDataTable
    End Function

    Public Shared Function StringToArrayList(ByVal value As String) As ArrayList
        Dim _al As ArrayList = New ArrayList()
        Dim _s As String() = value.Split(New Char() {","c})

        For Each item As String In _s
            _al.Add(item)
        Next

        Return _al
    End Function

    <Serializable>
    Public Class BasicModel
        Private _id As Integer
        Private _name As String
        Public Property ID As Integer
            Get
                Return _id
            End Get
            Set(value As Integer)
                _id = value
            End Set
        End Property
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property
        Public Checked As Boolean
        Public Enabled As Boolean
        Public ConnectionString As String
        Public Is_EManager As Boolean
        Public Sub New(id As Integer, name As String, checked As Boolean, enabled As Boolean)
            Me.ID = id
            Me.Name = name
            Me.Checked = checked
            Me.Enabled = enabled
        End Sub
        Public Sub New(checked As Boolean, enabled As Boolean)
            Me.Checked = checked
            Me.Enabled = enabled
        End Sub
        Public Sub New(id As Integer, name As String, checked As Boolean, enabled As Boolean, IsEmanager As Boolean)
            Me.ID = id
            Me.Name = name
            Me.Checked = checked
            Me.Enabled = enabled
            Me.Is_EManager = IsEmanager
        End Sub

    End Class

    <Serializable>
    Public Class TemplateInfo
        Public TemplateID As Integer?
        Public TemplateName As String
        Public IsShared As Boolean?
        Public Enabled As Boolean?
        Public SelectedDataSourceID As Integer?
        Public DataSources As List(Of BasicModel)
        Public Fields As List(Of Field)
    End Class

    <Serializable>
    Public Class Template
        Inherits BasicModel

        Public IsShared As Boolean
        Public DataSourceID As Integer
        Public ReadOnly Property ImageUrl As String
            Get
                If IsShared Then
                    Return "Images/Shared.png"
                Else
                    Return "Images/Private.png"
                End If
            End Get
        End Property

        Public Sub New(id As Integer, name As String, isShared As Boolean, dataSourceID As Integer, checked As Boolean, enabled As Boolean)
            MyBase.New(id, name, checked, enabled)
            Me.IsShared = isShared
            Me.DataSourceID = dataSourceID
        End Sub
    End Class

    <Serializable>
    Public Class Country
        Inherits BasicModel

        Public CY_GLOBALID As Guid
        Public CY_NAME As String
        Public CY_NAME_ISOCODE As String
        Public SOP_ID As String
        Public ImageURL As String

        Public Sub New(id As Integer, name As String, cy_gloablID As Guid, cy_name As String, cy_name_isocode As String, sop_id As String, imageUrl As String, checked As Boolean, enabled As Boolean)
            MyBase.New(id, name, checked, enabled)
            Me.CY_GLOBALID = cy_gloablID
            Me.CY_NAME = cy_name
            Me.CY_NAME_ISOCODE = cy_name_isocode
            Me.SOP_ID = sop_id
            Me.ImageURL = imageUrl
        End Sub
    End Class

    <Serializable>
    Public Class Action
        Inherits BasicModel

        Public CorrespondentActionID As Integer

        Public Sub New(id As Integer, name As String, checked As Boolean, enabled As Boolean, correspondentActionID As Integer)
            MyBase.New(id, name, checked, enabled)
            Me.CorrespondentActionID = correspondentActionID
        End Sub

    End Class


    <Serializable>
    Public Class Link
        Private _LinkID As Integer
        Private _LinkName As String
        Private _LinkUrl As String
        Private _LinkIconColor As String
        Private _DisplayOrder As Integer

        Public Sub New(LinkID As Integer, LinkName As String, LinkUrl As String, LinkIconColor As String, DisplayOrder As Integer)
            _LinkID = LinkID
            _LinkName = LinkName
            _LinkUrl = LinkUrl
            _LinkIconColor = LinkIconColor
            _DisplayOrder = DisplayOrder
        End Sub

        Public Property LinkID As Integer
            Get
                Return _LinkID
            End Get
            Set
                _LinkID = Value
            End Set
        End Property

        Public Property LinkName As String
            Get
                Return _LinkName
            End Get
            Set
                _LinkName = Value
            End Set
        End Property

        Public Property LinkUrl As String
            Get
                Return _LinkUrl
            End Get
            Set
                _LinkUrl = Value
            End Set
        End Property

        Public Property LinkIconColor As String
            Get
                Return _LinkIconColor
            End Get
            Set
                _LinkIconColor = Value
            End Set
        End Property

        Public Property DisplayOrder As Integer
            Get
                Return _DisplayOrder
            End Get
            Set
                _DisplayOrder = Value
            End Set
        End Property
    End Class
    <Serializable>
    Public Class Field
        Inherits BasicModel
        Private _parentID As Integer?
        Private _imageUrl As String

        Public Property ParentID As Integer?
            Get
                Return _parentID
            End Get
            Set(value As Integer?)
                _parentID = value
            End Set
        End Property

        Public Property imageUrl As String
            Get
                Return _imageUrl
            End Get
            Set(value As String)
                _imageUrl = value
            End Set
        End Property

        Public Sub New(id As Integer, name As String, checked As Boolean, enabled As Boolean, parentID As Integer?, imageUrl As String)
            MyBase.New(id, name, checked, enabled)
            Me.ParentID = parentID
            Me.imageUrl = imageUrl
        End Sub

    End Class

    <Serializable>
    Public Class CountryTreeItem
        Inherits Field

        Private _value As String


        Public Property Value As String
            Get
                Return _value
            End Get
            Set(value As String)
                _value = value
            End Set
        End Property

        Public Sub New(id As Integer, name As String, checked As Boolean, enabled As Boolean, parentID As Integer?, imageUrl As String, value As String)
            MyBase.New(id, name, checked, enabled, parentID, imageUrl)
            Me.Value = value
        End Sub

    End Class

    <Serializable>
    Public Class Tool
        Public ToolID As Integer
        Public ParentToolID As Integer?
        Public Property IconImagePath As String
            Get
                Return _IconImagePath
            End Get
            Set
                _IconImagePath = Value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _Name
            End Get
            Set
                _Name = Value
            End Set
        End Property

        Public Property Url As String
            Get
                Return _Url
            End Get
            Set
                _Url = Value
            End Set
        End Property

        Public Property TypeID As Integer
            Get
                Return _TypeID
            End Get
            Set
                _TypeID = Value
            End Set
        End Property

        Public Property MenuIconImagePath As String
            Get
                Return _MenuIconImagePath
            End Get
            Set
                _MenuIconImagePath = Value
            End Set
        End Property

        Public Property BadgeColor As String
            Get
                Return _BadgeColor
            End Get
            Set
                _BadgeColor = Value
            End Set
        End Property

        Public Property BadgeText As String
            Get
                Return _BadgeText
            End Get
            Set
                _BadgeText = Value
            End Set
        End Property

        Public Property IsFavorite As Boolean
            Get
                Return _IsFavorite
            End Get
            Set
                _IsFavorite = Value
            End Set
        End Property

        Public Actions As List(Of ActionDesignation)
        Private _IconImagePath As String
        Private _Name As String
        Private _Url As String
        Private _TypeID As Integer
        Private _MenuIconImagePath As String
        Private _BadgeText As String
        Private _BadgeColor As String
        Private _IsFavorite As Boolean

        Public Sub New(toolID As Integer, parentToolID As Integer?, iconImagePath As String, name As String, url As String, typeID As Integer, menuIconImagePath As String, badgeText As String, badgeColor As String, isFavorite As Boolean)
            Me.ToolID = toolID
            Me.ParentToolID = parentToolID
            _IconImagePath = iconImagePath
            _Name = name
            _Url = url
            _MenuIconImagePath = menuIconImagePath
            _TypeID = typeID
            _BadgeColor = badgeColor
            _BadgeText = badgeText
            _IsFavorite = isFavorite
        End Sub
    End Class

    Public Enum ActionDesignation
        DISPLAY_B2B_LOGS = 1
        DISPLAY_EBM_LOGS = 2
        DISPLAY_B2B_REQUESTS_LOGS = 3
        VIEW_XML_FILES = 4
        DISPLAY_CROSSDOCK_LOGS = 5
        DISPLAY_B2B_MANAGER_LOGS = 6
        EXPORT_LOGS_IN_EXCEL = 7
        SEND_REQUESTS_TO_SAP = 8
        CREATE_A_MONITORING_MESSAGE = 9
        MANAGE_PAUSE_INTERVALS = 10
        ENABLE_DISABLE_A_MONITORING_MESSAGE = 11
        DISPLAY_COUNTRY_RANGE_INTEGRATION_LOGS = 12
        RESET_FILES_CACHE = 13
        DOWNLOAD_COUNTRY_RANGE_FILES = 14
        COMPARE_COUNTRY_RANGE_FILES_WITH_EDEN_DATA = 15
        CHECK_CONFIGURATIONS = 16
        DISPLAY_B2B_CHATBOT_LOGS = 17
        DISPLAY_B2B_REPLIES_LOGS = 18
        DISPLAY_B2B_ERRORS_LOGS = 19
        SETUP_SURVEY_CHARTS = 20
        SETUP_FREE_QUERIES_CHARTS = 21
        EDIT_ALL_SHARED_CHARTS = 22
        DELETE_ALL_SHARED_CHARTS = 23
        DISPLAY_TP2_LOGS = 24
        DISPLAY_TP2_HYBRIS_LOGS = 25
        DISPLAY_TP2_SHIPPING_NOTES_LOGS = 26
        EDIT_SMS_LOCAL_VALUE = 27
        EDIT_SMS_DEFAULT_AND_COMMENT_VALUES = 28
        CHANGE_SMS_TYPE = 29
        DELETE_SMS_KEY = 30
        ADD_SMS_KEY = 31
        DISPLAY_ALL_SMS_TYPES = 32
        DISPLAY_B2B_PERFORMANCES_LOGS = 33
        EDIT_LOCAL_TRANSLATION_VALUE = 37
        EDIT_COMMENTS_AND_DEFAULT_TRANSLATION_VALUE = 38
        DELETE_TRANSLATION_KEY = 39
        ADD_TRANSLATION_KEY = 40
        DISPLAY_ALL_COUNTRIES_VALUES = 41
        EDIT_CUSTOMER_DETAILS = 42
        EDIT_USER_SUPER_USER_DETAILS = 43
        CREATE_NEW_CUSTOMER = 44
        CREATE_NEW_USER = 45
        CREATE_NEW_SUPER_USER = 46
        DELETE_CUSTOMER = 47
        DELETE_USER = 48
        DELETE_SUPER_USER = 49
        DISPLAY_USER_SUPER_USER_ACTIVITY_HISTORY_TAB = 50
        DISPLAY_CUSTOMER_ACTIVITY_HISTORY_TAB = 51
        DISPLAY_MANAGE_CONTACTS_TAB_IN_CUSTOMER_AND_USER_PROFILES = 52
        DISPLAY_INSIGHTS_TAB_IN_CUSTOMER_PROFILE = 53
        DISPLAY_ADDRESS_LIST_TAB_IN_CUSTOMER_PROFILE = 54
        RESTRICT_ADDRESS_IN_ADDRESS_LIST_TAB = 55
        DELETE_CUSTOMER_RANGE = 56
        DELETE_CUSTOMER_PRICE_CACHE = 57
        UPLOAD_CUSTOMER_LOGO = 58
        CREATE_DUPLICATE_CONTACT = 59
        EDIT_CONTACT_DETAILS = 60
        DELETE_CONTACT = 61
        BASIC_ACCESS_TO_REALTIME_MONITOR = 62
        BASIC_ACCESS_TO_COUNTRY_RANGE_EXPLORER = 63
        BASIC_ACCESS_TO_CHATBOT_MANAGER = 64
        BASIC_ACCESS_TO_B2B_ACCOUNTS = 65
        BASIC_ACCESS_TO_INSIGHTS = 66
        BASIC_ACCESS_TO_B2B_TRANSLATIONS = 67
        BASIC_ACCESS_TO_B2B_SPECIFICATIONS = 68
        BASIC_ACCESS_TO_B2B_CONTACTS = 69
        DISPLAY_CUSTOMER_LIST_TAB_IN_SUPER_USER_PROFILE = 70
        MAINTAIN_SUPER_USER_CUSTOMER_LIST = 71
        ASSIGN_UNASSIGN_TRANSLATION_AREA = 72
        DISPLAY_EBUSINESS_ACTIONS_LOGS = 73
        DISPLAY_FILE_TRANSFER_GATEWAY_LOGS = 74
        EDIT_TP2_SMS_LOCAL_VALUE = 75
        EDIT_TP2_SMS_DEFAULT_AND_COMMENT_VALUES = 76
        CHANGE_TP2_SMS_TYPE = 77
        DELETE_TP2_SMS_KEY = 78
        ADD_TP2_SMS_KEY = 79
        DISPLAY_ALL_TP2_SMS_TYPES = 80
        BASIC_ACCESS_TO_TP2_SPECIFICATIONS = 81
        BASIC_ACCESS_TO_TP2_MAILING_LISTS = 82
        ACCESS_COUNTRY_LEVEL = 83
        ADD_EDIT_CUSTOMER_EMAIL_SETTINGS = 84
        ADD_EDIT_COUNTRY_EMAIL_SETTINGS = 85
        DELETE_CUSTOMER_EMAIL_SETTINGS = 86
        DELETE_COUNTRY_EMAIL_SETTINGS = 87
        BASIC_ACCESS_TO_SAP_ORDER_TYPES_DISPLAY_ONLY = 91
        MAINTAIN_SAP_ORDER_TYPE_ADD_CHANGE_DELETE = 92
        BASIC_ACCESS_TO_TP_PRICE_SETTINGS_DISPLAY_ONLY = 93
        MAINTAIN_TP_PRICE_SETTINGS_ADD_CHANGE_DELETE = 95
        DOWNLOAD_FILES = 96
        BASIC_ACCESS_TO_TP2_STOCK_PUSH_SCHEDULES = 97
        MAINTAIN_TP2_STOCK_PUSH_SCHEDULES_ADD_CHANGE_DELETE = 98
        IMPERSONATE_USER = 102
        ADD_SCHEDULE_B2B = 99
        DELETE_SCHEDULE_B2B = 100
        EDIT_SCHEDULE_B2B = 101
        ADD_SURVEY = 103
        EDIT_SURVEY = 104
        DELETE_SURVEY = 105
        TRANSLATE_SURVEY = 106
        UPDATE_COMNORM_USER_INFO = 110
        REACTIVATE_USER = 111
        ADD_FOCUS_RANGE = 112
        DELETE_FOCUS_RANGE = 113
        ASSIGN_FOCUS_RANGE = 114
        ADD_NOTIFICATION_GROUP = 115
        EDIT_NOTIFICATION_GROUP = 116
        DEACTIVATE_NOTIFICATION_GROUP = 117
        DELETE_NOTIFICATION_GROUP = 118
    End Enum

    Public Class MonitoringWorkflowInfo
        Public workflowRootTreeNode As RadTreeNode
        Public workflowHTML As String
    End Class

    Public Class ExportTemplate
        Public ColumnFieldName As String
        Public ColumnName As String

        Public Sub New(columnFieldName As String, columnName As String)
            Me.ColumnFieldName = columnFieldName
            Me.ColumnName = columnName
        End Sub
    End Class

    Public Class ApplicationExtraColumn
        Inherits ExportTemplate
        Public ApplicationID As Integer
        Public ColumnIndex As Integer
        Public Sub New(applicationID As Integer, columnIndex As Integer, columnFieldName As String, columnName As String)
            MyBase.New(columnFieldName, columnName)
            Me.ApplicationID = applicationID
            Me.ColumnIndex = columnIndex
        End Sub
    End Class

    <Serializable>
    Public Class Application
        Inherits BasicModel

        Public AppendAllItemInActions As Boolean
        Public SelectAllCountriesByDefault As Boolean
        Public ShowCorrelIDColumn As Boolean
        Public ShowCustomerCodeColumn As Boolean
        Public ShowViewMessageXMLColumn As Boolean
        Public ShowPOIDColumn As Boolean
        Public ShowSalesOrderIDColumn As Boolean
        Public ShowHubspanIdColumn As Boolean
        Public ShowDateReceivedColumn As Boolean
        Public IsMessageXML As Boolean
        Public UseEnvironmentConnectionString As Boolean
        Public Actions As List(Of Action)
        Public Environments As List(Of BasicModel)
        Public Countries As List(Of Country)

        Public Sub New(id As Integer, name As String, appendAllItemInActions As Boolean, selectAllCountriesByDefault As Boolean, showCorrelIDColumn As Boolean, showPOIDColumn As Boolean, showSalesOrderIDColumn As Boolean, checked As Boolean, enabled As Boolean, showCustomerCodeColumn As Boolean, showViewMessageXMLColumn As Boolean, isMessageXML As Boolean, showHubspanIDColumn As Boolean, showDateReceivedColumn As Boolean)
            MyBase.New(id, name, checked, enabled)
            Me.AppendAllItemInActions = appendAllItemInActions
            Me.SelectAllCountriesByDefault = selectAllCountriesByDefault
            Me.ShowCorrelIDColumn = showCorrelIDColumn
            Me.ShowCustomerCodeColumn = showCustomerCodeColumn
            Me.ShowViewMessageXMLColumn = showViewMessageXMLColumn
            Me.IsMessageXML = isMessageXML
            Me.ShowPOIDColumn = showPOIDColumn
            Me.ShowSalesOrderIDColumn = showSalesOrderIDColumn
            Me.ShowHubspanIdColumn = showHubspanIDColumn
            Me.ShowDateReceivedColumn = showDateReceivedColumn
        End Sub
    End Class

    <Serializable>
    Public Class Chatbot
        Inherits BasicModel
        Public ChannelID As String
        Public Token As String
        Public DeveloperToken As String
        Public Environments As List(Of BasicModel)
        Public Countries As List(Of Country)

        Public Sub New(id As Integer, name As String, channelID As String, token As String, developerToken As String, checked As Boolean, enabled As Boolean)
            MyBase.New(id, name, checked, enabled)
            Me.ChannelID = channelID
            Me.Token = token
            Me.DeveloperToken = developerToken
        End Sub

    End Class

    Public Shared Function SizeSuffix(ByVal value As Long, ByVal Optional decimalPlaces As Integer = 1) As String
        Dim SizeSuffixes As String() = {"bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb"}

        If value < 0 Then
            Return "-" & SizeSuffix(-value)
        End If

        Dim i As Integer = 0
        Dim dValue As Decimal = value

        While Math.Round(dValue, decimalPlaces) >= 1000
            dValue /= 1024
            i += 1
        End While
        Return Math.Round(dValue, decimalPlaces).ToString() + " " + SizeSuffixes(i)
    End Function

    Public Shared Function GetExcelColumnName(ByVal columnNumber As Integer) As String
        Dim dividend As Integer = columnNumber
        Dim columnName As String = String.Empty
        Dim modulo As Integer

        While dividend > 0
            modulo = (dividend - 1) Mod 26
            columnName = Convert.ToChar(65 + modulo).ToString() & columnName
            dividend = CInt(((dividend - modulo) / 26))
        End While

        Return columnName
    End Function

    Private Shared Sub addValueWihUnitToList(Val As Integer, unit As String, ByRef parts As List(Of String))
        If Val > 0 Then parts.Add(Val.ToString() + " " + unit)
    End Sub


    Public Shared Function GetDurationInFriendlyText(milliseconds As Integer) As String
        Dim parts = New List(Of String)
        Dim t = TimeSpan.FromMilliseconds(milliseconds)
        addValueWihUnitToList(t.Days, "d", parts)
        addValueWihUnitToList(t.Hours, "h", parts)
        addValueWihUnitToList(t.Minutes, "min", parts)
        addValueWihUnitToList(t.Seconds, "sec", parts)
        If milliseconds < 1000 Then
            addValueWihUnitToList(t.Milliseconds, "ms", parts)
        End If
        Return IIf((parts.Count > 0), String.Join(" ", parts), "--")
    End Function

    Public Shared Function Encrypt(clearText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim clearBytes As Byte() = Encoding.Unicode.GetBytes(clearText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(clearBytes, 0, clearBytes.Length)
                    cs.Close()
                End Using
                clearText = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
        Return clearText
    End Function

    Public Shared Function Decrypt(cipherText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(cipherBytes, 0, cipherBytes.Length)
                    cs.Close()
                End Using
                cipherText = Encoding.Unicode.GetString(ms.ToArray())
            End Using
        End Using
        Return cipherText
    End Function

#Region "Assignment"

    ' From table [B2B_V2].[dbo].[T_AssignmentTypes]
    Public Enum AssignmentType
        Unassigned = 0
        Assignment = 1
    End Enum

    ' From table [B2B_V2].[dbo].[T_ObjectTypes]
    Public Enum ObjectType
        File = 1
    End Enum

#End Region


End Class

<Serializable>
Public Class DataTablesSource
    Public data As Object()
End Class