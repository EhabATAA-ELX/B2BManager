Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Web.Script.Services
Imports DataSource
Imports InsightsDataService
Imports Newtonsoft.Json

<ServiceContract(Namespace:="")>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class B2BManagerService

    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    Public Function UpdateB2BTranslations(EnvironmentID As String, TN_GlobalID As String, LangIsocode As String, Mode As Integer, DefaultValue As String, Comment As String, CountryValue As String, SopID As String) As Boolean
        'Dim response As DataSource(Of B2BTranslation()) = New DataSource(Of B2BTranslation())
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If (EnvironmentID > 0) And user IsNot Nothing Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@TN_GlobalID", TN_GlobalID))
            parameters.Add(New SqlParameter("@LangIsocode", LangIsocode))
            parameters.Add(New SqlParameter("@Mode", Mode))
            parameters.Add(New SqlParameter("@DefaultValue", DefaultValue))
            parameters.Add(New SqlParameter("@Comment", Comment))
            parameters.Add(New SqlParameter("@CountryValue", CountryValue))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[Translations_UpdateTranslation]", parameters) Then
                ClsHelper.Log("Update B2B Translation", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                ClsHelper.LogEbusinessAction(EnvironmentID, SopID, "Update Translation", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                Return True
            Else
                ClsHelper.Log("Update B2B Translatio", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                ClsHelper.LogEbusinessAction(EnvironmentID, SopID, "Update Translation", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                Return False
            End If
        End If
        Return False
    End Function
    ' To use HTTP GET, add <WebGet()> attribute. (Default ResponseFormat is WebMessageFormat.Json)
    ' To create an operation that returns XML,
    '     add <WebGet(ResponseFormat:=WebMessageFormat.Xml)>,
    '     and include the following line in the operation body:
    '         
    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetB2BTranslations(EnvironmentID As Integer, LangIsocode As String, AreaName As String) As DataSource(Of B2BTranslation())
        Dim response As DataSource(Of B2BTranslation()) = New DataSource(Of B2BTranslation())
        If (EnvironmentID > 0) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@LangIsocode", LangIsocode))
            If Not String.IsNullOrEmpty(AreaName) AndAlso AreaName <> "0" Then
                parameters.Add(New SqlParameter("@AreaName", AreaName))
            End If
            Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_GetTranslations", parameters)
            response = ClsHelper.GetDataTablesSource(Of B2BTranslation)(dt)
        End If
        Return response
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSAPOrderConfigurations(EnvironmentID As Integer) As DataSource(Of SAPOrderConfiguration())
        Dim response As DataSource(Of SAPOrderConfiguration()) = New DataSource(Of SAPOrderConfiguration())
        If (EnvironmentID > 0) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("B2BOrders.GetSAPOrderConfigurations", parameters)
            response = ClsHelper.GetDataTablesSource(Of SAPOrderConfiguration)(dt)
        End If
        Return response
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetFilesInfo(envid As Integer, entryid As Integer) As DataSource(Of FileDetail())
        Dim response As DataSource(Of FileDetail()) = New DataSource(Of FileDetail())
        Dim result As IEnumerable(Of FileDetail) = Nothing
        If (envid > 0 AndAlso entryid > 0) Then
            Dim entry As ClsFilesViewerHelper.FileEntry = Nothing
            Dim envrionment = ClsFilesViewerHelper.GetEnvironment(envid, entryid, entry)
            If entry IsNot Nothing And envrionment IsNot Nothing Then
                Try
                    Dim di As IO.DirectoryInfo = New DirectoryInfo(envrionment.RootPath)
                    Dim filesInfo As FileInfo() = di.GetFiles(entry.ExtensionFilter, IIf(entry.AllDirectories, SearchOption.AllDirectories, SearchOption.TopDirectoryOnly))
                    result = From r In filesInfo.AsEnumerable()
                             Select New FileDetail With {
                                .FileName = r.FullName.Replace(envrionment.RootPath + "\", ""),
                                .Size = Math.Round(r.Length / 1024),
                                .CreationDate = r.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                .ModificationDate = r.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    .Extension = r.Extension
                            }
                    response.data = JsonConvert.DeserializeObject(Of FileDetail())(JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented))
                Catch ex As Exception
                    Throw ex
                End Try
            End If
        End If
        Return response
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetContacts(EnvironmentID As Integer, SOPID As String) As DataSource(Of BasicContact())
        Dim response As DataSource(Of BasicContact()) = New DataSource(Of BasicContact())
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If (EnvironmentID > 0 And user IsNot Nothing And Not String.IsNullOrEmpty(SOPID)) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@UserID", user.GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SOPID", SOPID))
            Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.CtcMgmt_GetContacts", parameters)
            response = ClsHelper.GetDataTablesSource(Of BasicContact)(dt)
        End If
        Return response
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteUserByGlobalID(GlobalID As Guid) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            If ClsSessionHelper.LogonUser.Tools.Where(Function(fc) fc.ToolID = 36).Count() = 1 Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", user.ID))
                parameters.Add(New SqlParameter("@U_GLOBALID", GlobalID))
                runStatus = ClsDataAccessHelper.ExecuteScalar("Administration.DeleteUserByGlobalID", parameters)
                ClsHelper.Log("Delete user", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                runStatus = "You haven't the right to perform deletion"
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteTP2CountryEmail(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@U_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            parameters.Add(New SqlParameter("@ListType", 1))
            If ClsDataAccessHelper.ExecuteNonQuery("[TP2MailingList].[DeleteMailSettingByID]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete TP2 Country Email", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete TP2 Country Email", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteTP2CustomerEmail(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@U_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[TP2MailingList].[DeleteMailSettingByID]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete TP2 Customer Email", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete TP2 Customer Email", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteTP2ScheduleSetting(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[TP2StockPush].[DeleteSchedule]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete TP2 Stock Push Schedule", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete TP2 Stock Push Schedule", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteTP2PriceSetting(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[TP2PriceSettings].[DeleteTP2PriceSetting]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete TP2 Price Setting", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete TP2 Price Setting", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteSAPOrderType(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[SAPOrderTypes].[DeleteSAPOrderType]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete SAP Order Type", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete SAP Order Type", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function


    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetTradeplaceID(term As String, Envid As Integer) As IEnumerable(Of AutoCompleteInfo)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim data As DataTable = New DataTable
        Dim result As IEnumerable(Of AutoCompleteInfo) = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@UserID", user.ID))
        parameters.Add(New SqlParameter("@EnvironmentID", Envid))
        parameters.Add(New SqlParameter("@Term", term))
        data = ClsDataAccessHelper.FillDataTable("TP2MailingList.GetTradeplaceIDs", parameters)
        If data IsNot Nothing Then
            result = From r In data.AsEnumerable()
                     Select New AutoCompleteInfo With {
                        .id = r.Field(Of String)("TPC_Name"),
                        .label = r.Field(Of String)("TPID"),
                        .value = r.Field(Of String)("TPID")
                    }
        End If
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSalesOrgs(term As String, Envid As Integer) As IEnumerable(Of AutoCompleteInfo)
        Dim data As DataTable = New DataTable
        Dim result As IEnumerable(Of AutoCompleteInfo) = Nothing
        If HttpContext.Current.Cache("SalesOrgs_" & Envid.ToString()) Is Nothing Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            data = ClsDataAccessHelper.FillDataTable("TP2PriceSettings.GetSalesOrgs", parameters)
            HttpContext.Current.Cache("SalesOrgs_" & Envid.ToString()) = data
        Else
            data = HttpContext.Current.Cache("SalesOrgs_" & Envid.ToString())
        End If

        If data IsNot Nothing Then
            result = From r In data.AsEnumerable().Where(Function(fc) fc.Field(Of String)("SALES_ORG_LOWERCASE").Contains(term.ToLower()))
                     Select New AutoCompleteInfo With {
                        .id = r.Field(Of String)("SALES_ORG"),
                        .label = r.Field(Of String)("SALES_ORG"),
                        .value = r.Field(Of String)("SALES_ORG")
                    }
        End If
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetSAPFields(term As String, Envid As Integer) As IEnumerable(Of AutoCompleteInfo)
        Dim data As DataTable = New DataTable
        Dim result As IEnumerable(Of AutoCompleteInfo) = Nothing
        If HttpContext.Current.Cache("SAPFields_" & Envid.ToString()) Is Nothing Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            data = ClsDataAccessHelper.FillDataTable("TP2PriceSettings.GetSAPFields", parameters)
            HttpContext.Current.Cache("SAPFields_" & Envid.ToString()) = data
        Else
            data = HttpContext.Current.Cache("SAPFields_" & Envid.ToString())
        End If
        If data IsNot Nothing Then
            result = From r In data.AsEnumerable().Where(Function(fc) fc.Field(Of String)("SAP_FIELD_LOWERCASE").Contains(term.ToLower()))
                     Select New AutoCompleteInfo With {
                        .id = r.Field(Of String)("SAP_FIELD"),
                        .label = r.Field(Of String)("SAP_FIELD"),
                        .value = r.Field(Of String)("SAP_FIELD")
                    }
        End If
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetTP2Fields(term As String, Envid As Integer) As IEnumerable(Of AutoCompleteInfo)
        Dim data As DataTable = New DataTable
        Dim result As IEnumerable(Of AutoCompleteInfo) = Nothing
        If HttpContext.Current.Cache("TP2Fields_" & Envid.ToString()) Is Nothing Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            data = ClsDataAccessHelper.FillDataTable("TP2PriceSettings.GetTP2Fields", parameters)
            HttpContext.Current.Cache("TP2Fields_" & Envid.ToString()) = data
        Else
            data = HttpContext.Current.Cache("TP2Fields_" & Envid.ToString())
        End If
        If data IsNot Nothing Then
            result = From r In data.AsEnumerable().Where(Function(fc) fc.Field(Of String)("TP2_FIELD_LOWERCASE").Contains(term.ToLower()))
                     Select New AutoCompleteInfo With {
                        .id = r.Field(Of String)("TP2_FIELD"),
                        .label = r.Field(Of String)("TP2_FIELD"),
                        .value = r.Field(Of String)("TP2_FIELD")
                    }
        End If
        Return result
    End Function

    Public Class AutoCompleteInfo
        Public Property id As String
        Public Property label As String

        Public Property value As String
    End Class

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteGroupByID(GroupID As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            If ClsSessionHelper.LogonUser.Tools.Where(Function(fc) fc.ToolID = 36).Count() = 1 Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", user.ID))
                parameters.Add(New SqlParameter("@U_ID", GroupID))
                runStatus = ClsDataAccessHelper.ExecuteScalar("Administration.DeleteGroupByID", parameters)
                ClsHelper.Log("Delete group", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                runStatus = "You haven't the right to perform deletion"
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteB2BUserByGlobalID(GlobalID As Guid, Envid As Integer, Sopid As String, IsSuperUser As Boolean) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()

            Dim params As List(Of SqlParameter) = New List(Of SqlParameter)()
            params.Add(New SqlParameter("@EnvironmentID", Envid))
            params.Add(New SqlParameter("@U_Global_ID", GlobalID))
            Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetUserEmailLoginByGlobalId]", params)

            If dataSet.Tables(0).Rows.Count > 0 Then
                Dim userRow As DataRow = dataSet.Tables(0).Rows(0)
                If (Not userRow Is Nothing) Then
                    Dim emailLogin As String = ClsDataAccessHelper.GetText(userRow, "U_EMAIL_LOGIN")
                    If (Not String.IsNullOrEmpty(emailLogin)) Then
                        Dim isChiron As Boolean = False
                        If userRow("U_ISCHIRON") IsNot DBNull.Value Then
                            Try
                                isChiron = Convert.ToBoolean(ClsDataAccessHelper.GetText(userRow, "U_ISCHIRON"))
                            Catch
                            End Try
                        End If
                        Dim setup_type = ClsUsersManagementHelper.GetSetupType(isChiron)

                        'set Active = false on SCI
                        Dim statusCode As HttpStatusCode = Nothing
                        Dim returnMessage As String = String.Empty
                        Dim userUri As String = ClsSciUtil.SPUserIDRetrival(Envid.ToString(), Sopid, setup_type, emailLogin)
                        If (Not String.IsNullOrEmpty(userUri)) Then
                            ClsSciUtil.UpdateRemoteUser(Envid.ToString(),
                                                                "", "",
                                                                Nothing, isChiron, Sopid, setup_type,
                                                                userUri,
                                                                statusCode,
                                                                returnMessage,
                                                                True)
                        End If
                    End If

                End If
            End If

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@U_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_DeleteUser]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Delete " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Delete " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function ReactivateUserByGlobalID(GlobalID As Guid, Envid As Integer, Sopid As String, IsSuperUser As Boolean) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@U_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_ReactivateUser]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Reactivate " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Reactivate " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Reactivate " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Reactivate " + IIf(IsSuperUser, "Super ", "") + "User", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function


    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteCustomLink(LinkID As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@LinkID", LinkID))
            parameters.Add(New SqlParameter("@UserID", user.ID))
            If ClsDataAccessHelper.ExecuteNonQuery("[Administration].[DeleteLink]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete Link", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete Link", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteB2BCustomerByGlobalID(GlobalID As Guid, Envid As Integer, Sopid As String) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@C_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[UsrMgmt_DeleteCustomer]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete Customer", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Delete Customer", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete Customer", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
                ClsHelper.LogEbusinessAction(Envid, Sopid, "Delete Customer", user.GlobalID, ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteContactByGlobalID(GlobalID As Guid, Envid As Integer) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@C_Global_ID", GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[Ebusiness].[CtcMgmt_DeleteCustomer]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete Contact", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete Contact", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function ManageSuperUserSelection(Type As Integer, Values As String(), envid As Integer, uid As Guid) As String
        Return ClsEbusinessHelper.MaintainSuperUserList(Type, String.Join(",", Values), envid, uid, Nothing)
    End Function



    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetCustomerInsightsValues(envid As Integer, cid As Guid) As List(Of String)
        Dim UID = "Customer_Ordering_Insights_" + envid.ToString() + "_" & cid.ToString()
        Dim Result As List(Of String) = Nothing
        Dim data As List(Of ClsInsightLineChartData) = Nothing
        If Not String.IsNullOrEmpty(UID) Then
            Using client As InsightsDataServiceClient = New InsightsDataServiceClient()
                client.Open()
                If client.CheckItemInCache(UID) Then
                    Dim objectData As DataTable = New DataTable()
                    Try
                        objectData = client.GetData(UID, "")
                    Catch ex As Exception
                        Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
                        If Not ex.Message Is Nothing Then
                            exceptionMessage = ex.Message
                        End If
                        If Not ex.StackTrace Is Nothing Then
                            exceptionStackTrace = ex.StackTrace
                        End If
                        Dim errorMsg As String = String.Format("<b>Methode Name:</b>GetCustomerInsightsValues</br><b>Excepetion Message:</b></br>{0}</br>" _
                                    + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                                    , exceptionStackTrace)
                        ClsSendEmailHelper.SendErrorEmail(errorMsg)
                    End Try
                    If objectData.Rows.Count = 2 Then
                        Result = New List(Of String)()
                        If objectData.Rows(0)("Value") IsNot DBNull.Value Then
                            Result.Add(objectData.Rows(0)("Value"))
                        Else
                            Result.Add("--")
                        End If
                        If objectData.Rows(1)("Value") IsNot DBNull.Value Then
                            Result.Add(objectData.Rows(1)("Value"))
                        Else
                            Result.Add("0")
                        End If
                    End If
                End If
            End Using
        End If
        Return Result
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetCustomerUserCounts(envid As Integer, cid As Guid) As List(Of String)
        Dim result As List(Of String) = New List(Of String)
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", envid))
        parameters.Add(New SqlParameter("@CID", cid))
        Dim dataSet As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_GetCustomerUserCounts]", parameters)
        If dataSet.Tables.Count >= 2 Then
            If dataSet.Tables(0).Rows.Count = 1 Then
                result.Add(dataSet.Tables(0).Rows(0)(0))
            Else
                result.Add("--")
            End If

            If dataSet.Tables(1).Rows.Count = 1 Then
                result.Add(dataSet.Tables(1).Rows(0)(0))
            Else
                result.Add("--")
            End If
        End If
        Return result
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function LockOrUnlockUserByGlobalID(GlobalID As Guid) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            If ClsSessionHelper.LogonUser.Tools.Where(Function(fc) fc.ToolID = 36).Count() = 1 Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", user.ID))
                parameters.Add(New SqlParameter("@U_GlobalID", GlobalID))
                runStatus = IIf(ClsDataAccessHelper.ExecuteNonQuery("Administration.LockOrUnlockUserByGlobalID", parameters), "Success", "")
                ClsHelper.Log("Lock/Unlock user", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
            Else
                runStatus = "You haven't the right to perform deletion"
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function

    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function EmailUnique(ByVal email As String, ByVal EnvironmentID As String) As Boolean
        Dim parametersEmail As List(Of SqlParameter) = New List(Of SqlParameter)()
        parametersEmail.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parametersEmail.Add(New SqlParameter("@Email", email))
        Dim dataSetEMAIL As DataSet = ClsDataAccessHelper.FillDataSet("[Ebusiness].[UsrMgmt_PS_GET USER_WithSameEmail]", parametersEmail)
        If dataSetEMAIL.Tables(0).Rows.Count > 0 Then
            Return False
        Else
            Return True
        End If
    End Function


    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetMaintenanceInstance(ByVal sopid As String, ByVal EnvironmentID As String, mode As String) As String
        Dim dataSet As DataSet = ClsMaintenanceHelper.GetInstancesByEnvironmentID(EnvironmentID, sopid, HttpContext.Current.Cache)
        Dim LogonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim HasB2BInstances As Boolean = False
        Dim HasBiliInstances As Boolean = False
        Dim HasAEGInstances As Boolean = False
        Dim HasElectroluxInstances As Boolean = False
        Dim CountryName As String = String.Empty
        If LogonUser IsNot Nothing Then
            If dataSet.Tables.Count = 2 Then
                If dataSet.Tables(0).Select("SOP_ID='" + sopid + "'").Count > 0 Then
                    CountryName = dataSet.Tables(0).Select("SOP_ID='" + sopid + "'")(0)("Name")
                Else
                    Return "<li class=""storeapp available"" data-sopid='" + sopid + "'>An error has occurred</li>"
                End If
            End If
        Else
            Return "<li class=""storeapp available"" data-sopid='" + sopid + "'>Session expired, please connect again</li>"
        End If

        Return ClsMaintenanceHelper.GetInstancesHtml(dataSet, sopid, mode, CountryName, LogonUser.GlobalID.ToString(), EnvironmentID, HasB2BInstances, HasBiliInstances, HasAEGInstances, HasElectroluxInstances)
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function ChangeInstancesStatus(ByVal sopid As String, ByVal EnvironmentID As String, mode As String, type As String, startInstances As Boolean, serverName As String) As String
        Dim dataSet As DataSet = ClsMaintenanceHelper.GetInstancesByEnvironmentID(EnvironmentID, sopid, HttpContext.Current.Cache)
        Dim LogonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim HasB2BInstances As Boolean = False
        Dim HasBiliInstances As Boolean = False
        Dim HasAEGInstances As Boolean = False
        Dim HasElectroluxInstances As Boolean = False
        Dim B2BMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read
        Dim BiliMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read
        Dim AEGMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read
        Dim ElectroluxMode As ClsMaintenanceHelper.Mode = ClsMaintenanceHelper.Mode.Read
        Dim CountryName As String = String.Empty
        If LogonUser IsNot Nothing Then
            If dataSet.Tables.Count = 2 Then
                If dataSet.Tables(0).Select("SOP_ID='" + sopid + "'").Count > 0 Then
                    CountryName = dataSet.Tables(0).Select("SOP_ID='" + sopid + "'")(0)("Name")
                Else
                    Return "<li class=""storeapp available"" data-sopid='" + sopid + "'>An error has occurred</li>"
                End If
            End If
        Else
            Return "<li class=""storeapp available"" data-sopid='" + sopid + "'>Session expired, please connect again</li>"
        End If
        Select Case type
            Case "B2B"
                B2BMode = IIf(startInstances, ClsMaintenanceHelper.Mode.StartInstance, ClsMaintenanceHelper.Mode.StopInstance)
            Case "Bili"
                BiliMode = IIf(startInstances, ClsMaintenanceHelper.Mode.StartInstance, ClsMaintenanceHelper.Mode.StopInstance)
            Case "AEG"
                AEGMode = IIf(startInstances, ClsMaintenanceHelper.Mode.StartInstance, ClsMaintenanceHelper.Mode.StopInstance)
            Case "Electrolux"
                ElectroluxMode = IIf(startInstances, ClsMaintenanceHelper.Mode.StartInstance, ClsMaintenanceHelper.Mode.StopInstance)
        End Select
        Return ClsMaintenanceHelper.GetInstancesHtml(dataSet, sopid, mode, CountryName, LogonUser.GlobalID.ToString(), EnvironmentID, HasB2BInstances, HasBiliInstances, HasAEGInstances, HasElectroluxInstances, B2BMode, BiliMode, AEGMode, ElectroluxMode, serverName)
    End Function

    '' ReMake of SCI credentials setup
    '''''<OperationContract()>
    '''''<WebGet()>
    '''''<ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    '''''Public Function SciUserInformation(ByVal envID As String, ByVal userLocation As String) As SciUserInformation
    '''''    Return ClsSciUtil.SPUserInformation(envID, userLocation)
    '''''End Function
    ''''''Public Function SciUserInformation(ByVal userLocation As String) As SciUserInformation


    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function IdDS_SCIM_UserInformation(ByVal envID As String, SOPID As String, IsChiron As Boolean, ByVal email As String) As IdDS_SCIM_Response
        Dim SetupType As String = ClsUsersManagementHelper.GetSetupType(IsChiron)
        Return ClsSciUtil.IdDS_SCIM_EmailLookup(envID, SOPID, SetupType, email)
    End Function


    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function GetRightsForChironRoleId(ByVal ChironRoleID As Integer) As List(Of String)
        Return ClsUsersManagementHelper.GetRightsForChironRoleId(ChironRoleID)
    End Function


    <OperationContract()>
    <WebGet()>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function DeleteFocusRange(Envid As Integer, FocusRangeID As Guid, FocusRangeTitle As String) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = String.Empty
        If user IsNot Nothing Then
            Dim watcher As Stopwatch = Stopwatch.StartNew()
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@FocusRangeId", FocusRangeID))
            parameters.Add(New SqlParameter("@FocusRangeTitle", FocusRangeTitle))
            parameters.Add(New SqlParameter("@Author", ClsSessionHelper.LogonUser.GlobalID))
            parameters.Add(New SqlParameter("@EnvironmentID", Envid))
            If ClsDataAccessHelper.ExecuteNonQuery("[FocusRange_DeleteRecord]", parameters) Then
                runStatus = "success"
                ClsHelper.Log("Delete FocusRange", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, False, Nothing)
            Else
                ClsHelper.Log("Delete FocusRange", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watcher.ElapsedMilliseconds, True, "An unexpected error has occurred")
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later [DeleteFocusRange SProc]"
        End If
        Return runStatus
    End Function


    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function ManageFocusRangeCustomersList(Type As Integer, Values As String(), envid As Integer, sopid As String, focusRangeId As Guid?) As String
        Return ClsEbusinessHelper.ManageFocusRangeCustomersList(Type, String.Join(",", Values), envid, sopid, focusRangeId)
    End Function

    <OperationContract()>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.WrappedRequest)>
    <ScriptMethod(UseHttpGet:=True, ResponseFormat:=ResponseFormat.Json)>
    Public Function AssignmentLogs(AssignmentTypeId As Integer, EnvironmentId As Integer, CountryId As String, ObjectTypeId As Integer, ObjectIds As String, CustomerIds As String) As Boolean
        If CustomerIds Is Nothing Then CustomerIds = String.Empty
        Dim userID As Guid = ClsSessionHelper.LogonUser.GlobalID
        Dim parameters As New List(Of SqlParameter) From {
            New SqlParameter("@AssignmentTypeId", AssignmentTypeId),
            New SqlParameter("@EnvironmentId", EnvironmentId),
            New SqlParameter("@CountryId", Guid.Parse(CountryId)),
            New SqlParameter("@ObjectTypeId", ObjectTypeId),
            New SqlParameter("@ObjectIds", ObjectIds),
            New SqlParameter("@CompanyIds", CustomerIds),
            New SqlParameter("@UserId", userID)
        }
        Dim res = ClsDataAccessHelper.ExecuteNonQuery("Monitoring.InsertAssignmentLogs", parameters)
        Return res
    End Function

End Class