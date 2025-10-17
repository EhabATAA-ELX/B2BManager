Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Xml
Imports Microsoft.VisualBasic

Public Class ClsMaintenanceHelper

    Public Enum MaintenanceStatus
        Online = 1
        InMaintenance = 2
        Unkown = 3
        Denied = 4
        NotAligned = 5
        GeneratingError = 6
    End Enum

    Public Enum Environment
        PRODUCTION = 5
        STAGING = 12
        TEST = 17
        UAT = 27
        DEV = 53
    End Enum

    Public Enum Mode
        Read = 1
        StartInstance = 2
        StopInstance = 3
    End Enum

    Public Enum ApplicationType
        B2B = 1
        Bili = 2
        AEG = 3
        Electrolux = 4
    End Enum

    Public Const CHANGE_STATUS_TEMPLATE As String = "onclick='ChangeInstancesStatus(""{0}"",""{1}"",{2},this,{3})'"
    Public Shared B2B_TEMPLATE_FILE_PATH As String
    Public Shared Function GetImageByStatus(status As MaintenanceStatus, type As ApplicationType, SOP_ID As String, ServerName As String) As String
        Select Case status
            Case MaintenanceStatus.Online
                Return "<img width='30' src=""Images/Maintenance/checkmark.png"" " + String.Format(CHANGE_STATUS_TEMPLATE, SOP_ID, type.ToString(), "false", """" + ServerName + """") + " title='Running, click to Stop' style='cursor:pointer' />"
            Case MaintenanceStatus.InMaintenance
                Return "<img width='30' src=""Images/Maintenance/stopped.png"" " + String.Format(CHANGE_STATUS_TEMPLATE, SOP_ID, type.ToString(), "true", """" + ServerName + """") + "  title='In maintenance, click to Run' style='cursor:pointer' />"
            Case MaintenanceStatus.Unkown
                Return "<img width='30' src=""Images/Maintenance/warning.png"" title='Unkown, please check' style='cursor:help' />"
            Case MaintenanceStatus.Denied
                Return "<img width='30' src=""Images/Maintenance/denied.png"" title='Access to path denied' style='cursor:not-allowed'/>"
            Case MaintenanceStatus.GeneratingError
                Return "<img width='30' src=""Images/Maintenance/Error.png"" title='An error has occurred' style='cursor:not-allowed'/>"
        End Select
        Return "<img width='30' src=""Images/Maintenance/warning.png"" title='Unkown, please check' style='cursor:help' />"
    End Function

    Public Shared Function GetStatusHtml(status As MaintenanceStatus,
                                         type As ApplicationType,
                                         SOP_ID As String) As String
        Dim result As String = String.Empty
        Select Case status
            Case MaintenanceStatus.InMaintenance
                result += "<span class='" + status.ToString().ToLower() + "'>In maintenance</span><hr><a style='color:#31af91 !important' " + String.Format(CHANGE_STATUS_TEMPLATE, SOP_ID, type.ToString(), "true", "null") + ">&nbsp;<i class=""far fa-play-circle""></i> Start " + type.ToString() + "</a>"
            Case MaintenanceStatus.Online
                result += "<span class='" + status.ToString().ToLower() + "'>Online</span><hr><a style='color:#c93636 !important' " + String.Format(CHANGE_STATUS_TEMPLATE, SOP_ID, type.ToString(), "false", "null") + "'>&nbsp;<i class=""far fa-stop-circle""></i> Stop " + type.ToString() + "</a>"
            Case MaintenanceStatus.NotAligned
                result += "<hr>Not aligned"
            Case MaintenanceStatus.Denied
                result += "<hr><span style='color:red'>Access to file denied</span>"
            Case MaintenanceStatus.GeneratingError
                result += "<hr><span style='color:red'>An error occurred</span>"
            Case Else
                result += "<hr><span class='" + status.ToString().ToLower() + "'>Unkown status</span>"
        End Select
        Return result
    End Function

    Public Shared Function GetInstancesHtml(dataset As DataSet,
                                            SOP_ID As String,
                                            ApplicationType As String,
                                            CountryName As String,
                                            GlobalID As String,
                                            Environment As Environment,
                                            ByRef HasB2BInstances As Boolean,
                                            ByRef HasBiliInstances As Boolean,
                                            ByRef HasAEGInstances As Boolean,
                                            ByRef HasElectroluxInstances As Boolean,
                                            Optional B2BMode As Mode = Mode.Read,
                                            Optional BiliMode As Mode = Mode.Read,
                                            Optional AEGMode As Mode = Mode.Read,
                                            Optional ElectroluxMode As Mode = Mode.Read,
                                            Optional ServerInstance As String = Nothing,
                                            Optional B2bTemplateFilePath As String = Nothing) As String
        Dim html As String = String.Empty
        Dim watch As Stopwatch = Stopwatch.StartNew()
        If Not String.IsNullOrEmpty(B2bTemplateFilePath) Then
            B2B_TEMPLATE_FILE_PATH = B2bTemplateFilePath
        End If
        html += "<li class=""storeapp available"" data-sopid='" + SOP_ID + "' data-status='#{status}'>"
        html += "<div class=""folder""><table class=""width100percent""><tr><td>"
        html += "<img src=""Images/FlagsSop/" + SOP_ID + ".png"" width=""32"" height=""32"" style=""border-radius:3px""/>"
        html += "<td style=""padding:5px"">" + CountryName + "</td><td><a onclick='RefreshInstance(""" + SOP_ID + """,this)'>Refresh</a></td></tr>"
        Dim B2bInstances As DataRow() = dataset.Tables(1).Select("SOP_ID='" + SOP_ID + "' AND ApplicationTypeID = " + String.Format("{0:d}", ClsMaintenanceHelper.ApplicationType.B2B))
        Dim BiliInstances As DataRow() = dataset.Tables(1).Select("SOP_ID='" + SOP_ID + "' AND ApplicationTypeID =  " + String.Format("{0:d}", ClsMaintenanceHelper.ApplicationType.Bili))
        Dim AEGInstances As DataRow() = dataset.Tables(1).Select("SOP_ID='" + SOP_ID + "' AND ApplicationTypeID = " + String.Format("{0:d}", ClsMaintenanceHelper.ApplicationType.AEG))
        Dim ElectroluxInstances As DataRow() = dataset.Tables(1).Select("SOP_ID='" + SOP_ID + "' AND ApplicationTypeID = " + String.Format("{0:d}", ClsMaintenanceHelper.ApplicationType.Electrolux))

        Dim isOnLine As Boolean = True
        If (B2bInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "B2B")) Or
            (BiliInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Bili")) Or
            (AEGInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "AEG")) Or
            (ElectroluxInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Electrolux")) Then
            html += "<table cellpadding='5' cellspacing='5'  class=""width100percent""><tr>"
            Dim B2BStatus As MaintenanceStatus = Nothing
            Dim BiliStatus As MaintenanceStatus = Nothing
            Dim AEGStatus As MaintenanceStatus = Nothing
            Dim ElectroluxStatus As MaintenanceStatus = Nothing
            Dim B2BInstancesHtml As String = String.Empty
            Dim BiliInstancesHtml As String = String.Empty
            Dim AEGInstancesHtml As String = String.Empty
            Dim ElectroluxInstancesHtml As String = String.Empty

            If (B2bInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "B2B")) Then
                GenerateInstancesHtml(ClsMaintenanceHelper.ApplicationType.B2B, B2BStatus, B2BInstancesHtml, B2bInstances, SOP_ID, B2BMode, ServerInstance)
                html += "<td style='text-align: center;vertical-align:top'><span style='font-weight:bold;margin-right:5px'>B2B (TP1)</span>"
                html += GetStatusHtml(B2BStatus, ClsMaintenanceHelper.ApplicationType.B2B, SOP_ID)
                html += "</td>"
                HasB2BInstances = True
                isOnLine = isOnLine And B2BStatus = MaintenanceStatus.Online
            End If
            If (BiliInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Bili")) Then
                GenerateInstancesHtml(ClsMaintenanceHelper.ApplicationType.Bili, BiliStatus, BiliInstancesHtml, BiliInstances, SOP_ID, BiliMode, ServerInstance)
                html += "<td style='" + IIf(HasB2BInstances, "border-left: 1px solid #e5e5e5;", "") + "text-align: center;vertical-align:top'><span style='font-weight:bold;margin-right:5px'>Bili (TP2)</span>"
                html += GetStatusHtml(BiliStatus, ClsMaintenanceHelper.ApplicationType.Bili, SOP_ID)
                html += "</td>"
                HasBiliInstances = True
                isOnLine = isOnLine And BiliStatus = MaintenanceStatus.Online
            End If
            If (AEGInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "AEG")) Then
                GenerateInstancesHtml(ClsMaintenanceHelper.ApplicationType.AEG, AEGStatus, AEGInstancesHtml, AEGInstances, SOP_ID, AEGMode, ServerInstance)
                html += "<td style='text-align: center;vertical-align:top'><span style='font-weight:bold;margin-right:5px'>Chiron (AEG)</span>"
                html += GetStatusHtml(AEGStatus, ClsMaintenanceHelper.ApplicationType.AEG, SOP_ID)
                html += "</td>"
                HasAEGInstances = True
                isOnLine = isOnLine And AEGStatus = MaintenanceStatus.Online
            End If
            If (ElectroluxInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Electrolux")) Then
                GenerateInstancesHtml(ClsMaintenanceHelper.ApplicationType.Electrolux, ElectroluxStatus, ElectroluxInstancesHtml, ElectroluxInstances, SOP_ID, ElectroluxMode, ServerInstance)
                html += "<td style='text-align: center;vertical-align:top'><span style='font-weight:bold;margin-right:5px'>Chiron (Electrolux)</span>"
                html += GetStatusHtml(ElectroluxStatus, ClsMaintenanceHelper.ApplicationType.Electrolux, SOP_ID)
                html += "</td>"
                HasElectroluxInstances = True
                isOnLine = isOnLine And ElectroluxStatus = MaintenanceStatus.Online
            End If
            html += "</tr><tr>"
            If (B2bInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "B2B")) Then
                html += "<td>" + B2BInstancesHtml + "</td>"
            End If
            If (BiliInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Bili")) Then
                html += "<td " + IIf(HasB2BInstances, "style='border-left: 1px solid #e5e5e5;'", "") + ">" + BiliInstancesHtml + "</td>"
            End If
            If (AEGInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "AEG")) Then
                html += "<td>" + AEGInstancesHtml + "</td>"
            End If
            If (ElectroluxInstances.Count > 0 And (ApplicationType = "All" Or ApplicationType = "Electrolux")) Then
                html += "<td>" + ElectroluxInstancesHtml + "</td>"
            End If
            html += "</tr>"
        End If
        If (B2bInstances.Count > 0 And ApplicationType <> "Bili") Or (BiliInstances.Count > 0 And ApplicationType <> "B2B") Then
            html += "</table>"
        Else
            html += "<tr><td colspan='3' style='padding:15px;text-align:center'>No instances can be found</td></tr>"
        End If
        html += "</table></div></li>"
        If (BiliMode <> Mode.Read Or B2BMode <> Mode.Read) Then
            Dim mode As Mode = IIf(BiliMode <> Mode.Read, BiliMode, B2BMode)
            Dim impactedApplication As String = IIf(BiliMode <> Mode.Read, "Bili", "B2B")
            Dim actionName As String = String.Format("Put {0} {1}", impactedApplication, IIf(mode = Mode.StopInstance, "under mainteance", "online"))
            Dim actionDetails As String = String.Format("<b>Impacted SOP:</b> {0}</br><b>Environment:</b> {1}{2}", SOP_ID, Environment.ToString(), IIf(ServerInstance Is Nothing, "", "</br><b>Server name:</b> " + ServerInstance))
            ClsHelper.Log(actionName, GlobalID, actionDetails, watch.ElapsedMilliseconds, False, Nothing)
        End If
        Return html.Replace("#{status}", IIf(isOnLine, "on", "off"))
    End Function

    Private Shared Sub GenerateInstancesHtml(type As ApplicationType,
                                             ByRef status As MaintenanceStatus,
                                             ByRef instancesHtml As String,
                                             instancesRows As DataRow(),
                                             SOP_ID As String,
                                             Optional mode As Mode = Mode.Read,
                                             Optional serverInstance As String = Nothing)
        instancesHtml += "<table class='instance-table' >"
        For Each row As DataRow In instancesRows
            Dim nodeStatus As MaintenanceStatus = MaintenanceStatus.Unkown
            Dim rowMode As Mode = Mode.Read
            instancesHtml += "<tr>"
            If String.IsNullOrEmpty(serverInstance) Then
                rowMode = mode
            Else
                If row("ServerName") = serverInstance Then
                    rowMode = mode
                End If
            End If
            If type = ApplicationType.B2B Then
                instancesHtml += "<td style='padding-left:0px !important'>" + CheckServerState(row("MaintenanceFilePath"), row("MaintenanceFileName"), nodeStatus, SOP_ID, row("ServerName"), rowMode) + "</td>"
                instancesHtml += "<td class='" + nodeStatus.ToString().ToLower() + "'>" + row("ServerName") + "</td><td><a href='" + row("InstancePathUrl") + "' target='_blank'>Access</a></td>"
            ElseIf type = ApplicationType.Bili Then
                instancesHtml += "<td style='padding-left:0px !important'>" + CheckBiliServerState(row("MaintenanceFilePath"), row("MaintenanceFileName"), nodeStatus, row("COUNTRY_ISO_CODE"), SOP_ID, row("ServerName"), rowMode) + "</td>"
                instancesHtml += "<td class='" + nodeStatus.ToString().ToLower() + "'>" + row("ServerName") + "</td>"
            ElseIf type = ApplicationType.AEG Then
                instancesHtml += "<td style='padding-left:0px !important'>" + CheckServerState(row("MaintenanceFilePath"), row("MaintenanceFileName"), nodeStatus, SOP_ID, row("ServerName"), rowMode, "AEG") + "</td>"
                instancesHtml += "<td class='" + nodeStatus.ToString().ToLower() + "'>" + row("ServerName") + "</td><td><a href='" + row("InstancePathUrl") + "' target='_blank'>Access</a></td>"
            ElseIf type = ApplicationType.Electrolux Then
                instancesHtml += "<td style='padding-left:0px !important'>" + CheckServerState(row("MaintenanceFilePath"), row("MaintenanceFileName"), nodeStatus, SOP_ID, row("ServerName"), rowMode, "ELX") + "</td>"
                instancesHtml += "<td class='" + nodeStatus.ToString().ToLower() + "'>" + row("ServerName") + "</td><td><a href='" + row("InstancePathUrl") + "' target='_blank'>Access</a></td>"
            End If

            instancesHtml += "</tr>"
            If status = Nothing Then
                status = nodeStatus
            Else
                If status <> nodeStatus Then
                    status = MaintenanceStatus.NotAligned
                End If
            End If
        Next
        instancesHtml += "</table>"
    End Sub

    Public Shared Function CheckServerState(filePath As String,
                                            maintenanceFileName As String,
                                            ByRef status As MaintenanceStatus,
                                            SOP_ID As String,
                                            ServerName As String,
                                            Optional mode As Mode = Mode.Read,
                                            Optional chironBrand As String = "") As String
        Try
            If System.IO.Directory.GetAccessControl(filePath) IsNot Nothing Then
                If System.IO.File.Exists(filePath + maintenanceFileName) Then
                    status = MaintenanceStatus.InMaintenance
                    If mode = Mode.StartInstance Then
                        System.IO.File.Delete(filePath + maintenanceFileName)
                        status = MaintenanceStatus.Online
                    End If
                Else
                    status = MaintenanceStatus.Online
                    If mode = Mode.StopInstance Then
                        Dim TemplateFile As String = B2B_TEMPLATE_FILE_PATH
                        If String.IsNullOrEmpty(TemplateFile) Then
                            TemplateFile = HttpContext.Current.Server.MapPath("~/App_Data/Maintenance/app_offline{0}.htm")
                        End If
                        If Not String.IsNullOrEmpty(chironBrand) Then
                            TemplateFile = String.Format(TemplateFile, "_" + chironBrand)
                        Else
                            TemplateFile = String.Format(TemplateFile, "")
                        End If
                        System.IO.File.Copy(TemplateFile, filePath + maintenanceFileName, True)
                        status = MaintenanceStatus.InMaintenance
                    End If
                End If
            End If
        Catch e As UnauthorizedAccessException
            status = MaintenanceStatus.Denied
        Catch e As System.IO.DirectoryNotFoundException
            status = MaintenanceStatus.Unkown
        Catch e As Exception
            status = MaintenanceStatus.GeneratingError
        End Try
        Dim type As ApplicationType = ApplicationType.B2B
        If chironBrand = "" Then
            If chironBrand = "AEG" Then
                type = ApplicationType.AEG
            ElseIf chironBrand = "ELX" Then
                type = ApplicationType.Electrolux
            End If
        End If
        Return GetImageByStatus(status, type, SOP_ID, ServerName)
    End Function

    Public Shared Function CheckBiliServerState(filePath As String,
                                                maintenanceFileName As String,
                                                ByRef status As MaintenanceStatus,
                                                CountryIsoCode As String,
                                                SOP_ID As String,
                                                ServerName As String,
                                                Optional mode As Mode = Mode.Read) As String
        Try
            If System.IO.Directory.GetAccessControl(filePath) IsNot Nothing Then
                Dim docXml As XmlDocument = New XmlDocument()
                docXml.Load(filePath + maintenanceFileName)
                Dim xpath As String = "//country[iso[contains(text(),'" & CountryIsoCode & "')]]/maintenance"
                Dim xpathBili As String = "//country[iso[contains(text(),'Bili - " & CountryIsoCode & "')]]/maintenance"
                Dim root As XmlNode = docXml.DocumentElement
                Dim XmlNodeList As XmlNodeList = root.SelectNodes(xpath)
                Dim xmlNodeListBili As XmlNodeList = root.SelectNodes(xpathBili)
                Dim MaintenanceMode As String = Nothing
                Dim BiliMaintenanceMode As String = Nothing
                If XmlNodeList.Count > 0 Then
                    MaintenanceMode = XmlNodeList.Item(0).InnerText
                End If
                If xmlNodeListBili.Count > 0 Then
                    BiliMaintenanceMode = xmlNodeListBili.Item(0).InnerText
                End If

                If MaintenanceMode = "0" Or BiliMaintenanceMode = "0" Then
                    status = MaintenanceStatus.Online
                    If mode = Mode.StopInstance Then
                        If XmlNodeList.Count > 0 Then
                            XmlNodeList.Item(0).InnerText = "1"
                        End If
                        If xmlNodeListBili.Count > 0 Then
                            xmlNodeListBili.Item(0).InnerText = "1"
                        End If
                        docXml.Save(filePath + maintenanceFileName)
                        status = MaintenanceStatus.InMaintenance
                    End If
                ElseIf MaintenanceMode = "1" Or BiliMaintenanceMode = "1" Then
                    status = MaintenanceStatus.InMaintenance
                    If mode = Mode.StartInstance Then
                        If XmlNodeList.Count > 0 Then
                            XmlNodeList.Item(0).InnerText = "0"
                        End If
                        If xmlNodeListBili.Count > 0 Then
                            xmlNodeListBili.Item(0).InnerText = "0"
                        End If
                        docXml.Save(filePath + maintenanceFileName)
                        status = MaintenanceStatus.Online
                    End If
                Else
                    status = MaintenanceStatus.Unkown
                End If
            End If

        Catch e As UnauthorizedAccessException
            status = MaintenanceStatus.Denied
        Catch e As System.IO.DirectoryNotFoundException
            status = MaintenanceStatus.Unkown
        Catch e As Exception
            status = MaintenanceStatus.GeneratingError
        End Try
        Return GetImageByStatus(status, ApplicationType.Bili, SOP_ID, ServerName)
    End Function

    Public Shared Function GetInstancesByEnvironmentID(EnvironmentID As Integer, SOPIDs As String, Cache As Cache) As DataSet
        Dim instances As DataSet = Nothing
        Dim cacheKey As String = "Maintenance_Instances_" + EnvironmentID.ToString() + "_" + SOPIDs.Replace(",", "_")
        If (Cache(cacheKey) Is Nothing) Then
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
            parameters = New List(Of SqlParameter)()
            parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
            parameters.Add(New SqlParameter("@SOPIDs", SOPIDs))
            instances = ClsDataAccessHelper.FillDataSet("[Maintenance].[GetInstancesByEnvironmentID]", parameters)
            If instances IsNot Nothing Then
                Cache.Insert(cacheKey, instances)
            End If
            parameters = Nothing
        Else
            instances = DirectCast(Cache(cacheKey), DataSet)
        End If
        cacheKey = Nothing
        Return instances
    End Function

    Public Shared Function GetNBPendingOrders(EnvironmentID As Integer, SOPIDs As String) As Integer
        Dim NBPendingOrders As Integer = 0
        Dim dt As DataTable = GetPendingOrders(EnvironmentID, SOPIDs)

        If dt IsNot Nothing Then
            NBPendingOrders = dt.Rows.Count
        End If
        Return NBPendingOrders
    End Function


    Public Shared Function GetPendingOrders(EnvironmentID As Integer, SOPIDs As String) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@SOPIDs", SOPIDs))
        Dim dt As DataTable = ClsDataAccessHelper.FillDataTable("[Maintenance].[GetPendingOrders]", parameters)
        Return dt
    End Function


End Class
