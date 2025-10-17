Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports B2BExtentions
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq




Public Class ClsSciUtil

    Public Const DisplayTextForGuidLoginName As String = "NotDefined"

    Public Shared Function GetSapCloudIdentityConnectionCredentials(EnvironmentID As String, SOPID As String, SetupType As String) As SciCredentials
        Dim creds As SciCredentials = New SciCredentials()
        If (EnvironmentID > 0) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SOPID", SOPID))
            parameters.Add(New SqlParameter("@SetupType", SetupType))
            Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("[Ebusiness].[PS_SCI_CLIENT_SECRETS_V2]", parameters)
            If dt.Rows IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                creds.ClientId = dt.Rows(0)("SCI_CLIENT_ID")
                creds.ClientSecret = dt.Rows(0)("SCI_CLIENT_SECRET")
                creds.SCI_API_BASEURL = dt.Rows(0)("SCI_API_BASEURL")
                creds.SCI_TARGETURL = dt.Rows(0)("SCI_TARGETURL")

                creds.IdDS_SCIM_USER = dt.Rows(0)("IdDS_SCIM_USER")
                creds.IdDS_SCIM_PASSWORD = dt.Rows(0)("IdDS_SCIM_PASSWORD")
                creds.IdDS_SCIM_BASEURL = dt.Rows(0)("IdDS_SCIM_BASEURL")
                creds.IdDS_SCIM_B2BTradeplaceGroupId = dt.Rows(0)("IdDS_SCIM_B2BTradeplaceGroupId")

                Return creds
            End If
        End If
        Throw New Exception("SCI credentials not found!")
    End Function

    Public Shared Function IdDS_SCIM_EmailLookup(EnvironmentID As String, SOPID As String, SetupType As String, email As String) As IdDS_SCIM_Response
        Dim filterQuery As String = "Users?filter=emails.value eq "
        Dim filterKey As String = HttpUtility.UrlEncode(email)
        Return IdDS_SCIM_Lookup(EnvironmentID, SOPID, SetupType, filterQuery, filterKey)
    End Function

    Public Shared Function IdDS_SCIM_UsernameLookup(EnvironmentID As String, SOPID As String, SetupType As String, username As String) As IdDS_SCIM_Response
        Dim filterQuery As String = "Users?filter=userName eq "
        Dim filterKey As String = HttpUtility.UrlEncode(username)
        Return IdDS_SCIM_Lookup(EnvironmentID, SOPID, SetupType, filterQuery, filterKey)
    End Function

    Protected Shared Function IdDS_SCIM_Lookup(EnvironmentID As String, SOPID As String, SetupType As String, filterQuery As String, filterKey As String) As IdDS_SCIM_Response
        Dim _idDS_SCIM_Response As IdDS_SCIM_Response = Nothing

        Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, SetupType)
        Dim serviceUsername As String = sciCredentials.IdDS_SCIM_USER
        Dim servicePassword As String = sciCredentials.IdDS_SCIM_PASSWORD


        Dim serviceEndpoint As String = sciCredentials.IdDS_SCIM_BASEURL & filterQuery & Chr(34) & filterKey & Chr(34)

        Dim req As HttpWebRequest = PrepareRequest(EnvironmentID, serviceEndpoint, serviceUsername, servicePassword)
        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

        Dim receivedContent As String
        Dim ResponseStream As Stream = response.GetResponseStream()
        Using streamReader = New StreamReader(ResponseStream)
            receivedContent = streamReader.ReadToEnd()
        End Using

        Dim statusCode As HttpStatusCode = response.StatusCode
        If (statusCode = HttpStatusCode.OK) Then
            _idDS_SCIM_Response = Deserialize_IdDS_SCIM_Response(receivedContent)
        End If
        Return _idDS_SCIM_Response

    End Function

    Public Shared Function IdDS_SCIM_Group_Operation(EnvironmentID As String, SOPID As String, SetupType As String,
                                                        UserId As String, Operation As String) As Boolean

        Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, SetupType)
        Dim serviceUsername As String = sciCredentials.IdDS_SCIM_USER
        Dim servicePassword As String = sciCredentials.IdDS_SCIM_PASSWORD

        Dim SCI_B2BTradeplace_Group_Id As String = sciCredentials.IdDS_SCIM_B2BTradeplaceGroupId      ' "afc760fc-0ffd-4084-9893-04960a12507c"

        Dim serviceEndpoint As String = sciCredentials.IdDS_SCIM_BASEURL & "Groups/" & SCI_B2BTradeplace_Group_Id & "/"

        Dim req As HttpWebRequest = PrepareRequest(EnvironmentID, serviceEndpoint, serviceUsername, servicePassword)


        Dim oOperations As List(Of Operation) = New List(Of Operation)
        Dim oOperation As Operation = New Operation()
        Dim oValues As List(Of Value) = New List(Of Value)
        Dim oValue As Value = New Value()
        oValue.value = UserId '"f6cf4e47-b1ca-4945-969f-99b93cce74b6"  'User Id
        oValues.Add(oValue)
        oOperation.op = Operation

        If Operation.Contains("add") Then
            oOperation.value = oValues
            oOperation.path = "members"
        End If
        If Operation.Contains("remove") Then
            'oOperation.path = "members[value eq \"8E8b6568-7b42-428b-875a-6F7bf3982bb8\"]"
            '            oOperation.path = "members[value eq \" & Chr(34) & UserId & "\" & Chr(34) & "]"
            oOperation.path = "members[value eq " & Chr(34) & UserId & Chr(34) & "]"
        End If
        oOperations.Add(oOperation)

        Dim Schemas() As String = New String() {"urn:ietf:params:scim:api:messages:2.0:PatchOp"}

        Dim payload As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
        payload.Add("Operations", oOperations)
        payload.Add("schemas", Schemas)
        Dim json As String = JsonConvert.SerializeObject(payload)

        req.Method = "PATCH"
        req.ContentType = "application/scim+json"
        Using writer As New StreamWriter(req.GetRequestStream())
            writer.Write(json)
        End Using

        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)
        Dim statusCode As HttpStatusCode = response.StatusCode
        Console.WriteLine(response.StatusCode)
        Using reader As New StreamReader(response.GetResponseStream())
            Dim body As String = reader.ReadToEnd()
            'Dim updatedObject As JObject = JObject.Parse(body)
            'Console.WriteLine(String.Format("Updated Folder {0}: ", updatedObject.GetValue("Id")))
        End Using

        If statusCode = HttpStatusCode.NoContent Then
            Return True
        Else
            Return False
        End If



        'Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

        'Dim receivedContent As String
        'Dim ResponseStream As Stream = response.GetResponseStream()
        'Using streamReader = New StreamReader(ResponseStream)
        '    receivedContent = streamReader.ReadToEnd()
        'End Using

        'Dim statusCode As HttpStatusCode = response.StatusCode
        'If (statusCode = HttpStatusCode.OK) Then
        '    _idDS_SCIM_Response = Deserialize_IdDS_SCIM_Response(receivedContent)
        'End If
        'Return _idDS_SCIM_Response

    End Function


    Public Shared Function SPUserIDRetrival(EnvironmentID As String, SOPID As String, SetupType As String, email As String, Optional name_id As String = "uid") As String

        Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, SetupType)
        Dim serviceUsername As String = sciCredentials.ClientId
        Dim servicePassword As String = sciCredentials.ClientSecret
        Dim serviceEndpoint As String = sciCredentials.SCI_API_BASEURL + "users?mail=" + email

        Dim req As HttpWebRequest = PrepareRequest(EnvironmentID, serviceEndpoint, serviceUsername, servicePassword)
        '//For currently unkwon reasons the request comes back with code 401 Unathorized but with the location in the response uri
        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)
        If (response.ResponseUri IsNot Nothing) Then
            Dim index As Integer = response.ResponseUri.AbsoluteUri.LastIndexOf("users/")
            Dim SciApp_SPId As String = response.ResponseUri.AbsoluteUri.Substring(index + 6)
            Dim tempGuid As Guid
            Dim isValid As Boolean = Guid.TryParse(SciApp_SPId, tempGuid)
            If (isValid) Then
                Return response.ResponseUri.AbsoluteUri
            Else
                Return Nothing
            End If
        End If
        Return Nothing
    End Function

    Public Shared Function SPUserInformation(EnvironmentID As String, SOPID As String, SetupType As String, userLocation As String) As SciUserInformation
        Dim userInformations As SciUserInformation = Nothing

        Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, SetupType)
        Dim serviceUsername As String = sciCredentials.ClientId
        Dim servicePassword As String = sciCredentials.ClientSecret

        Dim req As HttpWebRequest = PrepareRequest(EnvironmentID, userLocation, serviceUsername, servicePassword)

        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

        Dim receivedContent As String
        Dim ResponseStream As Stream = response.GetResponseStream()
        Using streamReader = New StreamReader(ResponseStream)
            receivedContent = streamReader.ReadToEnd()
        End Using

        Dim statusCode As HttpStatusCode = response.StatusCode
        If (statusCode = HttpStatusCode.OK) Then
            userInformations = Fill_User_info(receivedContent, response.Headers("Location"))
        End If
        Return userInformations

    End Function

    Private Shared Function PrepareRequest(EnvironmentId As String,
                                           serviceEndpoint As String,
                                           serviceUsername As String,
                                           servicePassword As String) As HttpWebRequest

        Dim req As HttpWebRequest
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls _
                                                   Or SecurityProtocolType.Tls11 _
                                                   Or SecurityProtocolType.Tls12 _
                                                   Or SecurityProtocolType.Ssl3

        req = HttpWebRequest.Create(serviceEndpoint)
        req.AddBasicAuthentication(serviceUsername, servicePassword)

        Return req
    End Function


    Private Shared Function HttpGetHandler(EnvironmentId As String, SOPID As String, SetupType As String, serviceEndpoint As String) As SciUserInformation
        Dim userInformations As SciUserInformation = Nothing

        Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentId, SOPID, SetupType)
        Dim serviceUsername As String = sciCredentials.ClientId
        Dim servicePassword As String = sciCredentials.ClientSecret

        Dim req As HttpWebRequest
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls _
                                                   Or SecurityProtocolType.Tls11 _
                                                   Or SecurityProtocolType.Tls12 _
                                                   Or SecurityProtocolType.Ssl3

        req = HttpWebRequest.Create(serviceEndpoint)
        req.AddBasicAuthentication(serviceUsername, servicePassword)

        Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

        Dim receivedContent As String
        Dim ResponseStream As Stream = response.GetResponseStream()
        Using streamReader = New StreamReader(ResponseStream)
            receivedContent = streamReader.ReadToEnd()
        End Using

        Dim statusCode As HttpStatusCode = response.StatusCode
        If (statusCode = HttpStatusCode.OK) Then
            userInformations = Fill_User_info(receivedContent, response.Headers("Location"))
        End If
        Return userInformations
    End Function

    Private Shared Function Fill_User_info(receivedContent As String, userLocationHeader As String) As SciUserInformation
        Dim userInformations As SciUserInformation = New SciUserInformation()
        userInformations.userLocationHeader = userLocationHeader

        Dim doc As XmlDocument = New XmlDocument()
        doc.LoadXml(receivedContent)


        If (doc.SelectSingleNode("/user/user_uuid") IsNot Nothing) Then
            userInformations.uuid = doc.SelectSingleNode("/user/user_uuid").InnerText
        End If
        If (doc.SelectSingleNode("/user/user_profile_id") IsNot Nothing) Then
            userInformations.user_profile_id = doc.SelectSingleNode("/user/user_profile_id").InnerText
        End If
        If (doc.SelectSingleNode("/user/profile_status") IsNot Nothing) Then
            userInformations.profile_status = doc.SelectSingleNode("/user/profile_status").InnerText
        End If
        If (doc.SelectSingleNode("/user/status") IsNot Nothing) Then
            userInformations.status = doc.SelectSingleNode("/user/status").InnerText
        End If
        If (doc.SelectSingleNode("/user/language") IsNot Nothing) Then
            userInformations.language = doc.SelectSingleNode("/user/language").InnerText
        End If

        If (doc.SelectSingleNode("/user/spCustomAttribute1") IsNot Nothing) Then
            userInformations.customField1 = doc.SelectSingleNode("/user/spCustomAttribute1").InnerText
        End If
        If (doc.SelectSingleNode("/user/spCustomAttribute2") IsNot Nothing) Then
            userInformations.customField2 = doc.SelectSingleNode("/user/spCustomAttribute2").InnerText
        End If
        If (doc.SelectSingleNode("/user/spCustomAttribute3") IsNot Nothing) Then
            userInformations.customField3 = doc.SelectSingleNode("/user/spCustomAttribute3").InnerText
        End If
        If (doc.SelectSingleNode("/user/spCustomAttribute4") IsNot Nothing) Then
            userInformations.customField4 = doc.SelectSingleNode("/user/spCustomAttribute4").InnerText
        End If
        If (doc.SelectSingleNode("/user/spCustomAttribute5") IsNot Nothing) Then
            userInformations.customField5 = doc.SelectSingleNode("/user/spCustomAttribute5").InnerText
        End If

        Return userInformations
    End Function

    Private Shared Function Deserialize_IdDS_SCIM_Response(receivedContent As String) As IdDS_SCIM_Response
        Dim results = JsonConvert.DeserializeObject(Of IdDS_SCIM_Response)(receivedContent)
        Return results
    End Function

    Public Shared Function CreateRemoteUser(EnvironmentID As String,
                                      LoginEmail As String,
                                      Login As String,
                                      FirstName As String,
                                      LastName As String,
                                      Country As String,
                                      Language As String,
                                      Is_Marketplace As Boolean,
                                      Is_Chiron As Boolean,
                                      SOPID As String,
                                      Setup_type As String,
                                      SendEmail As Boolean,
                                      ByRef userLocation As String,
                                      ByRef statusCode As HttpStatusCode,
                                      ByRef returnMessage As String) As Boolean

        Dim returnValue As Boolean = False

        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If (EnvironmentID > 0) And user IsNot Nothing Then
            'be sure of email login unicity before this
            Dim request As HttpWebRequest
            Dim enc As UTF8Encoding
            Dim postdata As String
            Dim postdatabytes As Byte()


            Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, Setup_type)
            Dim serviceUsername As String = sciCredentials.ClientId
            Dim servicePassword As String = sciCredentials.ClientSecret
            Dim targetUrl As String = sciCredentials.SCI_TARGETURL
            Dim serviceEndpoint As String = sciCredentials.SCI_API_BASEURL + "users/"

            If Is_Chiron Then
                'targetUrl = GetTargetUrl(EnvironmentID, Country, "CHIRON_AEG_Url")
            End If

            request = PrepareRequest(EnvironmentID, serviceEndpoint, serviceUsername, servicePassword)
            enc = New System.Text.UTF8Encoding()

            LoginEmail = HttpUtility.UrlEncode(LoginEmail)
            Login = HttpUtility.UrlEncode(Login)


            postdata = "email=" + LoginEmail +
                        "&spCustomAttribute1=" + Country +
                        "&spCustomAttribute2=" + "" +
                        "&spCustomAttribute3=" + "" +
                        "&spCustomAttribute4=" + IIf(Is_Marketplace = True, "B2B Tradeplace", "B2B PORTAL") +
                        "&spCustomAttribute5=" + Login +
                        "&send_email=" + SendEmail.ToString() +
                        "&target_url=" + targetUrl
            If Not String.IsNullOrEmpty(FirstName) Then
                postdata = postdata + "&first_name=" + FirstName
            End If
            If Not String.IsNullOrEmpty(LastName) Then
                postdata = postdata + "&last_name=" + LastName
            End If
            If Not String.IsNullOrEmpty(Language) Then
                postdata = postdata + "&language=" + Language
            End If


            If Not String.IsNullOrEmpty(Login) Then
                postdata = postdata + "&login_name=" + Login
            End If

            postdatabytes = enc.GetBytes(postdata)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = postdatabytes.Length

            Using stream = request.GetRequestStream()
                stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using

            Dim Response As HttpWebResponse = CType(request.GetResponseWithoutException(), HttpWebResponse)

            Dim receivedContent As String
            Dim ResponseStream As Stream = Response.GetResponseStream()
            Using streamReader = New StreamReader(ResponseStream)
                receivedContent = streamReader.ReadToEnd()
            End Using

            statusCode = Response.StatusCode

            Select Case Response.StatusCode
                Case HttpStatusCode.Created
                    userLocation = Response.Headers("Location")
                    returnValue = True

                Case HttpStatusCode.BadRequest
                    returnMessage = "SCI: POST Bad request; " + receivedContent
                    returnValue = False

                Case HttpStatusCode.Conflict
                    returnMessage = "SCI: POST conflict; " + receivedContent
                    returnValue = False

                Case Else
                    returnMessage = "SCI: POST error; " + receivedContent
                    returnValue = False
            End Select

            'Should log the call to the external system ...
        End If
        Return returnValue
    End Function

    Public Shared Function UpdateRemoteUser(EnvironmentID As String,
                                      Login As String,
                                      Language As String,
                                      Is_Marketplace As Boolean?,
                                      Is_Chiron As Boolean,
                                      SOPID As String,
                                      Setup_type As String,
                                      ByRef userLocation As String,
                                      ByRef statusCode As HttpStatusCode,
                                      ByRef returnMessage As String,
                                      Optional ByVal setAsInactive As Boolean = False) As Boolean

        If String.IsNullOrEmpty(userLocation) Then
            Throw New Exception("SCI cannot update user with missing user location")
        End If

        Dim returnValue As Boolean = False

        Dim user As ClsUser = ClsSessionHelper.LogonUser
        If (EnvironmentID > 0) And user IsNot Nothing Then
            'be sure of email login unicity before this
            Dim request As HttpWebRequest
            Dim enc As UTF8Encoding
            Dim postdata As String
            Dim postdatabytes As Byte()

            Dim serviceEndpoint As String = userLocation


            Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, Setup_type)
            Dim serviceUsername As String = sciCredentials.ClientId
            Dim servicePassword As String = sciCredentials.ClientSecret


            request = PrepareRequest(EnvironmentID, serviceEndpoint, serviceUsername, servicePassword)
            enc = New System.Text.UTF8Encoding()

            Login = HttpUtility.UrlEncode(Login)

            postdata = "<user>"
            If Not String.IsNullOrEmpty(Language) Then
                postdata = postdata + "<language>" + Language + "</language>"
            End If
            If Not String.IsNullOrEmpty(Login) Then
                postdata = postdata + "<login_name>" + Login + "</login_name>"
            End If
            If Is_Marketplace.HasValue Then
                postdata = postdata + "<spCustomAttribute4>" + IIf(Is_Marketplace = True, "B2B Tradeplace", "B2B PORTAL") + "</spCustomAttribute4>"
            End If
            If setAsInactive Then
                postdata = postdata + "<status>inactive</status>"
            Else
                postdata = postdata + "<status>active</status>"
            End If
            postdata = postdata + "</user>"



            postdatabytes = enc.GetBytes(postdata)
            request.Method = "PUT"
            request.ContentType = "application/vnd.sap-id-service.sp-user-id+xml"
            request.ContentLength = postdatabytes.Length

            Using stream = request.GetRequestStream()
                stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using

            Dim Response As HttpWebResponse = CType(request.GetResponseWithoutException(), HttpWebResponse)

            Dim receivedContent As String
            Dim ResponseStream As Stream = Response.GetResponseStream()
            Using streamReader = New StreamReader(ResponseStream)
                receivedContent = streamReader.ReadToEnd()
            End Using

            statusCode = Response.StatusCode

            Select Case Response.StatusCode
                Case HttpStatusCode.OK
                    userLocation = Response.Headers("Location")
                    returnValue = True
                Case HttpStatusCode.Conflict
                    returnMessage = "SCI: Conflict on PUT"
                    returnValue = False
                Case Else
                    returnMessage = "SCI: PUT error; " + receivedContent
                    returnValue = False
            End Select

            'Should log the call to the external system ...
        End If
        Return returnValue
    End Function

    Private Shared Function IsValidEmailFormat(ByVal s As String) As Boolean
        Try
            Dim a As New System.Net.Mail.MailAddress(s)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Shared Function SendPasswordResetEmail(EnvironmentID As String, SOPID As String, SetupType As String, email As String, ByRef returnMessage As String) As Boolean
        returnMessage = "An email was sent to the LoginEmail address"

        If (Not IsValidEmailFormat(email)) Then
            returnMessage = "Invalid email format"
            Return False
        End If

        If (Not String.IsNullOrEmpty(email) AndAlso Not String.IsNullOrEmpty(EnvironmentID)) Then

            Dim sciCredentials As SciCredentials = GetSapCloudIdentityConnectionCredentials(EnvironmentID, SOPID, SetupType)
            Dim serviceUsername As String = sciCredentials.ClientId
            Dim servicePassword As String = sciCredentials.ClientSecret

            Dim serviceEndpoint As String = sciCredentials.SCI_API_BASEURL + "users/"

            Dim enc As UTF8Encoding
            Dim req As HttpWebRequest

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls _
                                                       Or SecurityProtocolType.Tls11 _
                                                       Or SecurityProtocolType.Tls12 _
                                                       Or SecurityProtocolType.Ssl3
            Dim postdata As String
            Dim postdatabytes As Byte()

            req = HttpWebRequest.Create(serviceEndpoint + "forgotPassword")
            enc = New System.Text.UTF8Encoding()
            postdata = "{""identifier"":  """ + email + """}"

            postdatabytes = enc.GetBytes(postdata)
            req.Method = "POST"
            req.ContentType = "application/json"
            req.AddBasicAuthentication(serviceUsername, servicePassword)
            req.ContentLength = postdatabytes.Length


            Using stream = req.GetRequestStream()
                stream.Write(postdatabytes, 0, postdatabytes.Length)
            End Using

            Dim response As HttpWebResponse = CType(req.GetResponseWithoutException(), HttpWebResponse)

            Dim receivedContent As String
            Dim ResponseStream As Stream = response.GetResponseStream()
            Using streamReader = New StreamReader(ResponseStream)
                receivedContent = streamReader.ReadToEnd()
            End Using

            Dim statusCode As HttpStatusCode = response.StatusCode
            If (statusCode = HttpStatusCode.OK) Then
                Return True
            End If
            If (statusCode = HttpStatusCode.BadRequest) Then
                Dim errorCode As String = response.Headers("X-message-code")
                returnMessage = "Error: " + errorCode
                Return False
            End If
        Else
            returnMessage = "Email or EnvID is empty"
            Return False
        End If



    End Function



    Public Shared Function GuidLoginNameHandler(inputString As String) As String
        Dim defaultValue As String = ClsSciUtil.DisplayTextForGuidLoginName
        Dim tempGuid As Guid
        Dim isValid As Boolean = Guid.TryParse(inputString, tempGuid)
        If (isValid) Then
            Return defaultValue
        Else
            Return inputString
        End If

    End Function
End Class


Public Class SciUserInformation
    Public Property userLocationHeader As String

    Public Property uuid As String
    Public Property user_profile_id As String
    Public Property organization_user_type As String
    Public Property status As String
    Public Property profile_status As String
    Public Property language As String


    Public Property customField1 As String
    Public Property customField2 As String
    Public Property customField3 As String
    Public Property customField4 As String
    Public Property customField5 As String


End Class

Public Class SciCredentials
    Public Property ClientId As String

    Public Property ClientSecret As String
    Public Property SCI_API_BASEURL As String
    Public Property SCI_TARGETURL As String


    Public Property IdDS_SCIM_USER As String
    Public Property IdDS_SCIM_PASSWORD As String
    Public Property IdDS_SCIM_BASEURL As String
    Public Property IdDS_SCIM_B2BTradeplaceGroupId As String

End Class




Public Class IdDS_SCIM_Response
    <JsonProperty("totalResults")>
    Public Property totalResults As Integer

    <JsonProperty("Resources")>
    Public Property resources As New List(Of IdDS_SCIM_Resource)


End Class

Public Class IdDS_SCIM_Resource

    <JsonProperty("id")>
    Public Property id As String

    <JsonProperty("meta")>
    Public Property meta As IdDS_SCIM_meta

    <JsonProperty("userName")>
    Public Property userName As String
    <JsonProperty("displayName")>
    Public Property displayName As String
    <JsonProperty("familyName")>
    Public Property familyName As String
    <JsonProperty("locale")>
    Public Property locale As String
    <JsonProperty("active")>
    Public Property active As String

    <JsonProperty("urn:ietf:params:scim:schemas:extension:sap:2.0:User")>
    Public Property ieft_params_extension_sap_User As IdDS_SCIM_ieft_params_extension_sap_User
End Class

Public Class IdDS_SCIM_meta
    <JsonProperty("created")>
    Public Property created As String
    <JsonProperty("lastModified")>
    Public Property lastModified As String
    <JsonProperty("location")>
    Public Property location As String
    <JsonProperty("resourceType")>
    Public Property resourceType As String

End Class

Public Class IdDS_SCIM_ieft_params_extension_sap_User

    <JsonProperty("emails")>
    Public Property emails As New List(Of IdDS_SCIM_ieft_params_User_Email)
    <JsonProperty("loginTime")>
    Public Property loginTime As String
    <JsonProperty("sourceSystem")>
    Public Property sourceSystem As String
    <JsonProperty("userUuid")>
    Public Property userUuid As String
    <JsonProperty("mailVerified")>
    Public Property mailVerified As Boolean
    <JsonProperty("userId")>
    Public Property userId As String
    <JsonProperty("passwordDetails")>
    Public Property passwordDetails As IdDS_SCIM_ieft_params_User_Password_Details

End Class

Public Class IdDS_SCIM_ieft_params_User_Email
    <JsonProperty("verified")>
    Public Property verified As Boolean
    <JsonProperty("value")>
    Public Property value As String
    <JsonProperty("verifiedTime")>
    Public Property verifiedTime As String
    <JsonProperty("primary")>
    Public Property primary As Boolean

End Class

Public Class IdDS_SCIM_ieft_params_User_Password_Details
    <JsonProperty("loginTime")>
    Public Property loginTime As String
    <JsonProperty("failedLOginAttempts")>
    Public Property failedLOginAttempts As String
    <JsonProperty("setTime")>
    Public Property setTime As String
    <JsonProperty("status")>
    Public Property status As String

End Class



Public Class ACIPatchRootobject
    Public Property Operations As List(Of Operation)
    Public Property schemas As List(Of String)
End Class

Public Class Operation
    Public Property op As String
    Public Property path As String
    Public Property value As List(Of Value)
End Class

Public Class Value
    Public Property value As String
End Class

