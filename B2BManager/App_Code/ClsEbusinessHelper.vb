Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports Microsoft.VisualBasic

Public Class ClsEbusinessHelper
    Public Shared Function Get_SOP_OrderTypes(EnvironmentID As Integer, SopID As String, Cache As Cache) As DataTable
        Dim orderTypes As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing

        If (Cache("SopOrderTypes_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            'parameters.Add(New SqlParameter("@SOPID", SopID))  get all values and cache them
            orderTypes = ClsDataAccessHelper.FillDataTable("[Ebusiness].[OrderType_GetSopOrderTypes]", parameters)
            If orderTypes IsNot Nothing Then
                Cache.Insert("SopOrderTypes_" + EnvironmentID.ToString(), orderTypes)
            End If
        Else
            orderTypes = DirectCast(Cache("SopOrderTypes_" + EnvironmentID.ToString()), DataTable)
        End If
        Return orderTypes
    End Function

    Public Shared Function Get_USER_OrderTypes(EnvironmentID As Integer, U_GLOBALID As String) As DataTable
        Dim userOrderTypes As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing

        parameters = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@U_GLOBALID", U_GLOBALID))
        userOrderTypes = ClsDataAccessHelper.FillDataTable("[Ebusiness].[OrderType_GetUserOrderTypes]", parameters)

        Return userOrderTypes
    End Function

    Public Shared Function GetGWSGroupType(EnvironmentID As Integer, Cache As Cache) As DataTable
        Dim gwsGroupTypes As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing
        If (Cache("GWSGroupType_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            gwsGroupTypes = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetGWSGroups]", parameters)
            If gwsGroupTypes IsNot Nothing Then
                Cache.Insert("GWSGroupType_" + EnvironmentID.ToString(), gwsGroupTypes)
            End If
        Else
            gwsGroupTypes = DirectCast(Cache("GWSGroupType_" + EnvironmentID.ToString()), DataTable)
        End If
        Return gwsGroupTypes
    End Function

    Public Shared Function GetCountrySettings(EnvironmentID As Integer, Cache As Cache) As DataTable
        Dim countrySettings As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        If (Cache("Ebusiness_CountrySettings_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            countrySettings = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetCountrySettingsByEnvironmentID]", parameters)
            If countrySettings IsNot Nothing Then
                Cache.Insert("Ebusiness_CountrySettings_" + EnvironmentID.ToString(), countrySettings)
            End If
        Else
            countrySettings = DirectCast(Cache("Ebusiness_CountrySettings_" + EnvironmentID.ToString()), DataTable)
        End If
        Return countrySettings
    End Function

    Public Shared Function GetUserRights(EnvironmentID As Integer, sopid As String, Cache As Cache) As DataTable
        Dim userRights As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing
        If (Cache("UserRights_" + sopid + "_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SopID", sopid))
            userRights = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetUserRights]", parameters)
            If userRights IsNot Nothing Then
                Cache.Insert("UserRights_" + sopid + "_" + EnvironmentID.ToString(), userRights)
            End If
        Else
            userRights = DirectCast(Cache("UserRights_" + sopid + "_" + EnvironmentID.ToString()), DataTable)
        End If
        Return userRights
    End Function

    Public Shared Function FormatTag(TagName As String, BackgroundColor As String, Optional title As String = "") As String
        Return String.Format("<span style='background-color:{0};' {2} class='userTag'>{1}</span>", BackgroundColor, TagName, IIf(String.IsNullOrEmpty(title), "", "title='" & title & "'"))
    End Function

    Public Shared Function GetSuperUserCategories(EnvironmentID As Integer, Cache As Cache) As DataTable
        Dim superUserCategories As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing
        If (Cache("SuperUserCategories_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            superUserCategories = ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetSuperUserCategories]", parameters)
            If superUserCategories IsNot Nothing Then
                Cache.Insert("SuperUserCategories_" + EnvironmentID.ToString(), superUserCategories)
            End If
        Else
            superUserCategories = DirectCast(Cache("SuperUserCategories_" + EnvironmentID.ToString()), DataTable)
        End If
        Return superUserCategories
    End Function

    Public Shared Sub SetValueCheckBoxList(ByRef cbl As CheckBoxList, ByVal sValues As String)
        If Not String.IsNullOrEmpty(sValues) Then
            Dim values As ArrayList
            If (sValues.Contains("|")) Then
                values = ClsHelper.StringToArrayList(sValues.Split("|")(1))
            Else
                values = ClsHelper.StringToArrayList(sValues)
            End If

            For Each li As ListItem In cbl.Items

                If values.ToArray().Intersect(li.Value.Split(",").ToArray()).Any Then
                    li.Selected = True
                    li.Attributes.Add("style", "color: #041d4f; font-weight: bold")
                Else
                    li.Selected = False
                    li.Attributes.Add("style", "color: #bbb;")
                End If
            Next
        End If
    End Sub

    Public Shared Sub SetValueCheckBoxList(ByRef cbl As CheckBoxList, ByVal sValues As String())
        If sValues IsNot Nothing Then
            For Each li As ListItem In cbl.Items
                If sValues.Contains(li.Value) Then
                    li.Selected = True
                Else
                    li.Selected = False
                End If
            Next
        End If
    End Sub

    Public Shared Function GetCompanyTemplate() As String
        Dim template = "<span class=\'buttonsContainer\' style=\'text-align:center;width:100% !important;min-width:80px !important;display:block\'>"
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim canEditCustomerDetails As Boolean = False
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.CREATE_NEW_USER)) Then
                template += "<img src=""Images/addUser.png"" title=""Add User"" class=""MoreInfoImg"" onclick=""NewUser(\'#id#\')"" width=""20"" height=""20""> "
            End If
            canEditCustomerDetails = clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_CUSTOMER_DETAILS)
        End If
        template += "<img src=""Images/Edit.png"" title=""" + IIf(canEditCustomerDetails, "Edit", "Display") + " Customer Details"" Class=""MoreInfoImg"" onclick=""EditCustomer(\'#id#\',\'#environmentid#\',this)"" width=""20"" height=""20""> "
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_CUSTOMER)) Then
                template += "<img src= ""Images/trash.png"" class=""MoreInfoImg""  title=""Delete Customer"" onclick=""DeleteCustomer(\'#id#\',\'#environmentid#\',this,\'#sopid#\')"" width=""20"" height=""22"" >"
            End If
        End If
        Return template
    End Function

    Public Shared Function GetUserTemplate() As String
        Dim template = "<span class=\'buttonsContainer\' style=\'text-align:center;width:100% !important;min-width:80px !important;display:block\'>"
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim canEditUserDetails As Boolean = False
        If clsUser IsNot Nothing Then
            canEditUserDetails = clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_USER_SUPER_USER_DETAILS)
        End If
        template += "<img src=""Images/Edit.png"" title=""" + IIf(canEditUserDetails, "Edit", "Display") + " User Details"" class=""MoreInfoImg"" onclick=""EditUser(\'#id#\',\'#environmentid#\',this)"" width=""20"" height=""22"">"
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_USER)) Then
                template += "<img src= ""Images/deleteUser.png"" class=""MoreInfoImg"" title=""Delete User"" onclick=""DeleteUser(\'#id#\',\'#environmentid#\',false,this,\'#sopid#\')"" width=""20"" height=""20"" >"
            End If
        End If
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.REACTIVATE_USER)) Then
                template += "<img src= ""Images/Reload.png"" class=""MoreInfoImg  reactivate-user"" title=""Reactivate User"" onclick=""ReactivateUser(\'#id#\',\'#environmentid#\',false,this,\'#sopid#\')"" width=""20"" height=""20"" style=""#display#"">"
            End If
        End If
        Return template
    End Function

    Public Shared Function GetSuperUserTemplate() As String
        Dim template = "<span class=\'buttonsContainer\' style=\'text-align:center;width:100% !important;min-width:90px !important;display:block\'>"
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        Dim canEditUserDetails As Boolean = False
        If clsUser IsNot Nothing Then
            canEditUserDetails = clsUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_USER_SUPER_USER_DETAILS)
        End If
        template += "<img src=""Images/Edit.png"" title=""" + IIf(canEditUserDetails, "Edit", "Display") + " Super User Details"" class=""MoreInfoImg"" onclick=""EditUser(\'#id#\',\'#environmentid#\',this)"" width=""20"" height=""22""> "
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DISPLAY_CUSTOMER_LIST_TAB_IN_SUPER_USER_PROFILE)) Then
                template += "<img src= ""Images/magnifyingglass.png"" class=""MoreInfoImg"" title=""Display Customer List"" onclick=""DisplayCustomerList(\'#id#\',\'#environmentid#\')"" width=""20"" height=""20"" > "
            End If
        End If
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.DELETE_SUPER_USER)) Then
                template += "<img src= ""Images/deleteUser.png"" class=""MoreInfoImg"" title=""Delete Super User"" onclick=""DeleteUser(\'#id#\',\'#environmentid#\',true,this,\'#sopid#\')"" width=""20"" height=""20"" >"
            End If
        End If
        If clsUser IsNot Nothing Then
            If (clsUser.Actions.Contains(ClsHelper.ActionDesignation.REACTIVATE_USER)) Then
                template += "<img src= ""Images/Reload.png"" class=""MoreInfoImg  reactivate-user"" title=""Reactivate User"" onclick=""ReactivateUser(\'#id#\',\'#environmentid#\',false,this,\'#sopid#\')"" width=""20"" height=""20"" style=""#display#"">"
            End If
        End If
        Return template
    End Function


    Public Shared Function GetLanguages(EnvironmentID As Integer, Cache As Cache) As DataTable
        Dim GetLanguagesDT As DataTable = Nothing
        Dim parameters As List(Of SqlParameter) = Nothing
        If (Cache("GetLanguages_" + EnvironmentID.ToString()) Is Nothing) Then
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            GetLanguagesDT = ClsDataAccessHelper.FillDataTable("[Ebusiness].[Translations_GetLanguagesByEnv]", parameters)
            If GetLanguagesDT IsNot Nothing Then
                Cache.Insert("GetLanguages_" + EnvironmentID.ToString(), GetLanguagesDT)
            End If
        Else
            GetLanguagesDT = DirectCast(Cache("GetLanguages_" + EnvironmentID.ToString()), DataTable)
        End If
        Return GetLanguagesDT
    End Function

    Public Shared Function GetCustomersForFocusRange(EnvironmentID As Integer, SopID As String, FocusRangeId As Guid) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@SopID", SopID))
        parameters.Add(New SqlParameter("@FR_ID", FocusRangeId))
        Return ClsDataAccessHelper.FillDataTable("[dbo].[FocusRange_GetCustomers]", parameters)
    End Function


    Public Shared Function ManageFocusRangeCustomersList(Type As Integer, Values As String, envid As Integer, sopId As String, focusRangeId As Guid?) As String
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = "Your session has ended, please sign in and try again"
        If user IsNot Nothing Then
            If user.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_FOCUS_RANGE) Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@ActingUserID", user.GlobalID))
                parameters.Add(New SqlParameter("@SOPID", sopId))
                parameters.Add(New SqlParameter("@FocusRangeId", focusRangeId))
                parameters.Add(New SqlParameter("@CustomerGlobalIDs", Values))
                parameters.Add(New SqlParameter("@EnvironmentID", envid))
                parameters.Add(New SqlParameter("@OperationType", Type))
                parameters.Add(New SqlParameter("@ExecuteOperation", True))
                Dim result As Integer = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[FocusRange_ManageCustomersList]", parameters)
                If result > 0 Then
                    runStatus = "Success"
                    ClsHelper.Log("FocusRange_Assign", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                Else
                    runStatus = "An unexpected error has occurred, please try later"
                    ClsHelper.Log("FocusRange_Assign", user.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "An unexpected error has occurred")
                End If
            Else
                runStatus = "You are not authorized to perform this operation"
            End If

        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function


    Public Shared Function GetCustomersForSuperUser(EnvironmentID As Integer, Uid As Guid) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@UID", Uid))
        Return ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetCustomersForSuperUser]", parameters)
    End Function

    Public Shared Function GetSuperUserInformation(EnvironmentID As Integer, Uid As Guid) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@UID", Uid))
        Return ClsDataAccessHelper.FillDataTable("[Ebusiness].[UsrMgmt_GetSuperUserBasicInformation]", parameters)
    End Function

    Public Shared Function GetFileServerUrl(EnvironmentId As String) As String
        Dim parameters As New List(Of SqlParameter) From {
            New SqlParameter("@param", EnvironmentId)
        }
        Dim UrlWS = ClsDataAccessHelper.FillDataTable("[Ebusiness].[FileServerUrl]", parameters)
        Return UrlWS.Rows(0)("url")
    End Function

    Public Shared Function MaintainSuperUserList(Type As Integer, Values As String, envid As Integer, uid As Guid?, cid As Guid?) As String
        Dim user As ClsUser = ClsSessionHelper.LogonUser
        Dim runStatus As String = "Your session has ended, please sign in and try again"
        If user IsNot Nothing Then
            If user.Actions.Contains(ClsHelper.ActionDesignation.MAINTAIN_SUPER_USER_CUSTOMER_LIST) Then
                Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                parameters.Add(New SqlParameter("@UserID", user.GlobalID))
                If Type < 5 Then
                    parameters.Add(New SqlParameter("@U_GLOBALID", uid))
                    parameters.Add(New SqlParameter("@CustomerGlobalIDs", Values))
                Else
                    parameters.Add(New SqlParameter("@C_GLOBALID", cid))
                    parameters.Add(New SqlParameter("@UserGlobalIDs", Values))
                End If
                parameters.Add(New SqlParameter("@EnvironmentID", envid))
                parameters.Add(New SqlParameter("@OperationType", Type))
                'STRING_SPLIT has a good performance otherwise the operation ID can be used to perform a bulk on table [Ebusiness].[SuperUser_List_Assignment_Customers]
                parameters.Add(New SqlParameter("@ExecuteOperation", True))
                Dim operationID As Integer = ClsDataAccessHelper.ExecuteScalar("[Ebusiness].[SuperUsr_SaveSuperUserListAssignmentOperation]", parameters)
                If operationID > 0 Then
                    runStatus = "Success"
                Else
                    runStatus = "An unexpected error has occurred, please try later"
                End If
            Else
                runStatus = "You are not authorized to perform this operation"
            End If
        End If
        If String.IsNullOrEmpty(runStatus) Then
            runStatus = "An unexpected error has occurred, please try later"
        End If
        Return runStatus
    End Function
    Public Class CustomerLogo
        Private _fileName As String
        Private _contentType As String
        Private _fileLength As Long
        Private _data As Byte()

        Public Property FileName As String
            Get
                Return _fileName
            End Get
            Set(value As String)
                _fileName = value
            End Set
        End Property

        Public Property ContentType As String
            Get
                Return _contentType
            End Get
            Set(value As String)
                _contentType = value
            End Set
        End Property

        Public Property FileLength As Long
            Get
                Return _fileLength
            End Get
            Set(value As Long)
                _fileLength = value
            End Set
        End Property

        Public Property Data As Byte()
            Get
                Return _data
            End Get
            Set(value As Byte())
                _data = value
            End Set
        End Property

        Public Sub New(fileName As String, contentType As String, fileLength As Long, data() As Byte)
            Me.FileName = fileName
            Me.ContentType = contentType
            Me.FileLength = fileLength
            Me.Data = data
        End Sub
    End Class
End Class
