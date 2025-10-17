
Imports System.Diagnostics
Imports SAPRequestsLib

Partial Class UserControls_MessageRequesterControl
    Inherits System.Web.UI.UserControl

    Private _Request As ClsAutomatedTestsHelper.TestRequest
    Private _IsTestRequest As Boolean

    Public Property TestRequest As ClsAutomatedTestsHelper.TestRequest
        Get
            Return _Request
        End Get
        Set(value As ClsAutomatedTestsHelper.TestRequest)
            _Request = value
        End Set
    End Property

    Public Property IsTestRequest As Boolean
        Get
            Return _IsTestRequest
        End Get
        Set(value As Boolean)
            _IsTestRequest = value
        End Set
    End Property

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If Not IsPostBack Then
            RenderControls()
        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            If Request("__EVENTTARGET") IsNot Nothing Then
                If Request("__EVENTTARGET").Contains("BtnSendRequestToSap") Then
                    Return
                End If
            End If
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "CreateXmlFormat", "CreateXmlFormat();", True)
        Else
            If TestRequest IsNot Nothing Then
                MessageXML.InnerText = ClsHelper.PrettyXml(TestRequest.MessageXML)
                ddlMessageType.SelectedValue = TestRequest.ActionID
                ddlEnvironment.SelectedValue = TestRequest.EnvironmentID
                ddlCountry.SelectedValue = TestRequest.Sop
                txtBoxHybrisMethodName.Text = TestRequest.WcfMethodName
                wcfB2BWebServiceUrl.Text = TestRequest.WcfB2BWebServiceURL
                txtBoxHybrisPassword.Text = TestRequest.WcfPassword
                txtboxHybrisUserName.Text = TestRequest.WcfUserName
            End If
            'Populate controls only when TestRequest is not supplied 
            ChangeControlsDisplay(TestRequest Is Nothing)
        End If
    End Sub

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim selectedApplication As ClsHelper.Application = logonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, ClsSessionHelper.LogonUser.DefaultEnvironmentID.ToString())
        Dim messageTypes As List(Of ClsMessageRequesterHelper.MessageType) = GetMessageTypes()
        ClsMessageRequesterHelper.RenderDropDownList(ddlMessageType, messageTypes)
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, logonUser.DefaultEbusinessSopID, False, String.IsNullOrEmpty(logonUser.DefaultEbusinessSopID))
    End Sub

    Public Sub ChangeControlsDisplay(Optional populateControls As Boolean = True)
        Dim selectedMessageType As ClsMessageRequesterHelper.MessageType = GetSelectedMessageType()
        txtBoxHybrisPassword.Attributes.Add("type", "password")
        If selectedMessageType IsNot Nothing Then
            If populateControls Then
                Dim messageInformation As ClsMessageRequesterHelper.MessageInformation = GetMessageInforamtionByMessageType(selectedMessageType)
                If Not String.IsNullOrEmpty(selectedMessageType.WebserviceSpecificKeyName) Then
                    wcfB2BWebServiceUrl.Text = messageInformation.SAPWCFMETHOD
                Else
                    wcfB2BWebServiceUrl.Text = messageInformation.SAPWCFURL
                End If
                If selectedMessageType.HasHybrisInterface Then
                    txtBoxHybrisMethodName.Text = messageInformation.SAPWCFMETHOD
                    txtBoxHybrisPassword.Text = messageInformation.SAPWCFPSSWORD
                    txtboxHybrisUserName.Text = messageInformation.SAPWCFUSERNAME
                Else
                    txtBoxHybrisMethodName.Text = ""
                    txtBoxHybrisPassword.Text = ""
                    txtboxHybrisUserName.Text = ""
                End If
            End If
            lblCountrySOP.Visible = String.IsNullOrEmpty(selectedMessageType.WebserviceSpecificKeyName)
            ddlCountry.Visible = Not String.IsNullOrEmpty(selectedMessageType.WebserviceSpecificKeyName)
            trViaHybrisInterface.Visible = String.IsNullOrEmpty(selectedMessageType.WebserviceSpecificKeyName)
            trHybrisUserdetails.Visible = String.IsNullOrEmpty(selectedMessageType.WebserviceSpecificKeyName) And selectedMessageType.HasHybrisInterface
            lblHybrisMethodName.Visible = selectedMessageType.HasHybrisInterface
            txtBoxHybrisMethodName.Visible = selectedMessageType.HasHybrisInterface
            If Not selectedMessageType.HasHybrisInterface Then
                chkBoxViaHybrisInerface.Attributes.Add("disabled", "disabled")
                chkBoxViaHybrisInerface.Checked = False
            Else
                chkBoxViaHybrisInerface.Attributes.Remove("disabled")
                chkBoxViaHybrisInerface.Checked = True
            End If
        End If
        UpdatePanel1.Update()
    End Sub

    Private Function GetSelectedMessageType() As ClsMessageRequesterHelper.MessageType
        If ddlMessageType.SelectedIndex <> -1 Then
            For Each item As ClsMessageRequesterHelper.MessageType In GetMessageTypes()
                If item.ID = ddlMessageType.SelectedValue Then
                    Return item
                End If
            Next
        End If
        Return Nothing
    End Function

    Private Function GetMessageInforamtionByMessageType(MessageType As ClsMessageRequesterHelper.MessageType) As ClsMessageRequesterHelper.MessageInformation
        Dim messageInformation As ClsMessageRequesterHelper.MessageInformation = New ClsMessageRequesterHelper.MessageInformation()
        If ddlEnvironment.SelectedIndex <> -1 Then
            Dim messageInformationList As List(Of ClsMessageRequesterHelper.MessageInformation) = GetMessageInformationByEnvironmentID(ddlEnvironment.SelectedValue)
            If messageInformationList IsNot Nothing Then
                For Each messageInformationItem As ClsMessageRequesterHelper.MessageInformation In messageInformationList
                    If MessageType.MethodSpecificKeyName = messageInformationItem.SAPWCFMETHODNAME Or MessageType.WebserviceSpecificKeyName = messageInformationItem.SAPWCFMETHODNAME Then
                        Return messageInformationItem
                    Else
                        If String.IsNullOrEmpty(messageInformation.SAPWCFURL) Then
                            messageInformation.SAPWCFURL = messageInformationItem.SAPWCFURL
                        End If
                    End If
                Next
            End If
        End If
        Return messageInformation
    End Function


    Private Function GetMessageInformationByEnvironmentID(EnvironmentID As Integer) As List(Of ClsMessageRequesterHelper.MessageInformation)
        Dim cachedMessageInformation As List(Of ClsMessageRequesterHelper.MessageInformation) = Cache("MessageInformation_" + EnvironmentID.ToString())
        If cachedMessageInformation Is Nothing Then
            cachedMessageInformation = ClsMessageRequesterHelper.GetMessageInformationByEnvironmentID(EnvironmentID)
            If cachedMessageInformation IsNot Nothing Then
                Cache("MessageInformation_" + EnvironmentID.ToString()) = cachedMessageInformation
            End If
        End If
        Return cachedMessageInformation
    End Function
    Private Function GetMessageTypes() As List(Of ClsMessageRequesterHelper.MessageType)
        Dim cachedMessageTypes As List(Of ClsMessageRequesterHelper.MessageType) = Cache("MessageTypes")
        If cachedMessageTypes Is Nothing Then
            cachedMessageTypes = ClsMessageRequesterHelper.GetMessageTypes()
            If cachedMessageTypes IsNot Nothing Then
                Cache("MessageTypes") = cachedMessageTypes
            End If
        End If
        Return cachedMessageTypes
    End Function
    Protected Sub ddlMessageType_SelectedIndexChanged(sender As Object, e As EventArgs)
        ChangeControlsDisplay()
    End Sub
    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        ChangeControlsDisplay()
    End Sub

    Public Sub SendRequestToSap(sender As Object, e As EventArgs) Handles BtnSendRequestToSap.Click
        Dim EnvironmentID As Integer = ddlEnvironment.SelectedValue
        Dim CorrelID As String = ""
        Dim SopID As String = IIf(ddlCountry.Visible, ddlCountry.SelectedValue, "")
        Dim selectedMessageType As ClsMessageRequesterHelper.MessageType = GetSelectedMessageType()
        Dim ActionName As String = selectedMessageType.Name
        Dim TableName As String = selectedMessageType.CorrespondentTableName
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim wcfUrl As String = wcfB2BWebServiceUrl.Text
        Dim wcfUserName As String = txtboxHybrisUserName.Text
        Dim wcfPassword As String = txtBoxHybrisPassword.Text
        Dim wcfMethodName As String = txtBoxHybrisMethodName.Text
        Dim xmlRequest As String = MessageXML.InnerText
        Dim errorMessage As String = Nothing
        Dim request As XMLRequest = SAPRequester.ValidateRequest(MessageXML.InnerText, SopID, EnvironmentID, ConfigurationManager.ConnectionStrings("LogDb").ConnectionString)

        If request.IsValid Then
            If SAPRequester.GetMessageTypeByMessage(ActionName) <> request.sAPMessageType Then
                request.IsValid = False
                request.error = New Exception("Selected message type does not correspond to your " + request.sAPMessageType.ToString() + " request")
            End If
        End If

        btnViewRequestReplyXML.Visible = False
        If request.IsValid Then
            SopID = request.SOPID
            If request.SessionID = Nothing Or Guid.Empty.Equals(request.SessionID) Then
                request.SessionID = Guid.Parse(ConfigurationManager.AppSettings("SessionID"))
            End If
            If request.GlobalID = Nothing Or Guid.Empty.Equals(request.GlobalID) Then
                request.GlobalID = ClsSessionHelper.LogonUser.GlobalID
            End If
            Dim SAPReplyResult As SAPReplyResult = SAPRequester.SendSAPMessage(xmlRequest, wcfUrl, wcfMethodName, wcfUserName, wcfPassword, request.GlobalID.ToString(), request.SessionID.ToString(), SopID)
            If SAPReplyResult.HasError Then
                divSendToSapInfo.InnerHtml = "<h3 style='color:red'>Message not sent please review the error message</h3>"
                If SAPReplyResult.error IsNot Nothing Then
                    divXMLReplySAP.InnerHtml = SAPReplyResult.error.Message
                    errorMessage = SAPReplyResult.error.Message
                End If
                divXMLReplySAP.Visible = False
                btnViewRequestReplyXML.Visible = False
            Else
                If String.IsNullOrEmpty(SAPReplyResult.Reply) Then
                    divSendToSapInfo.InnerHtml = "<h3 style='color:green'>Message sent with success</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                    btnViewRequestReplyXML.Visible = True
                    divXMLReplySAP.Visible = False
                    btnViewRequestReplyXML.Attributes.Add("onclick", String.Format("OpenViewXMLFilesWindow('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", 1, ActionName, EnvironmentID, SAPReplyResult.CorrelID, TableName, SopID, request.GlobalID, 0))
                Else
                    If Not SAPReplyResult.Reply.Contains("<Error Reply>") Then
                        divSendToSapInfo.InnerHtml = "<h3 style='color:green'>Message sent with success</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                        divXMLReplySAP.InnerHtml = ClsHelper.FormatXMLinHTML(ClsHelper.PrettyXml(SAPReplyResult.Reply))
                        divXMLReplySAP.Visible = True
                    Else
                        divSendToSapInfo.InnerHtml = "<h3 style='color:orange'>Message sent with Error</h3><h4>Correl ID: <b>" + SAPReplyResult.CorrelID + "</b></h4>"
                        divXMLReplySAP.InnerHtml = ClsHelper.FormatXMLinHTML(ClsHelper.PrettyXml(SAPReplyResult.Reply))
                        divXMLReplySAP.Visible = True
                    End If
                    btnViewRequestReplyXML.Visible = False
                End If
            End If
            ClsMessageRequesterHelper.LogXMLReply(SAPReplyResult.CorrelID, CorrelID, ClsHelper.PrettyXml(SAPReplyResult.Reply), xmlRequest, EnvironmentID, ActionName, SAPReplyResult.RequestedDate, SAPReplyResult.ReceivedDate, wcfUrl, SAPReplyResult.HasError, errorMessage)
            watch.Stop()
            ClsHelper.Log("Send Request To SAP", ClsSessionHelper.LogonUser.GlobalID.ToString(), String.Format("<b>ApplicationID:</b> {0}</br><b>Action Name:</b> {1}</br><b>CorrelID: </b> {2}</br><b>SAP Reply CorrelID:</b> {3}", 1, ActionName, CorrelID, SAPReplyResult.CorrelID), watch.ElapsedMilliseconds, SAPReplyResult.HasError, errorMessage)
        Else
            divSendToSapInfo.InnerHtml = "<h3 style='color:red'>Message XML is not valid, please review the error message</h3>"
            If request.error IsNot Nothing Then
                divXMLReplySAP.InnerHtml = request.error.Message
                errorMessage = request.error.Message
            End If
            divXMLReplySAP.Visible = True
        End If
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close dialog", "SendToSapFinish();", True)
    End Sub
End Class
