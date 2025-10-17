
Imports System.Data

Partial Class B2BCacheManager
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
        If Not IsPostBack Or "RenderControls".Equals(__EVENTARGUMENT) Then
            RenderControls()
            LoadInstances()
        Else
            LoadInstances()
            LoadAllCacheKeys()
        End If
    End Sub

    Private Sub RenderControls()
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = clsUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, clsUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, clsUser.DefaultEbusinessSopID), True)
    End Sub

    Private Sub LoadInstances()
        Dim dataset As DataSet = ClsDataAccessHelper.FillDataSet("[CacheManager].[GetInstancesByEnvironmentID]", New List(Of SqlClient.SqlParameter)(New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@EnvironmentID", CInt(ddlEnvironment.SelectedValue)), New SqlClient.SqlParameter("@SOPIDs", ddlCountry.SelectedValue)}))
        Dim html As String = ""
        If dataset.Tables.Count = 2 Then
            For Each row In dataset.Tables(0).Rows
                html += "<div class=""store-apps-title"" ><img onclick='expandOrCollapse(this)' data-server-id='" + row("ID").ToString() + "' id='expand-" + row("ID").ToString() + "' src=""Images/down-arrow.png"" width=""20"" />" & ClsDataAccessHelper.GetText(row, "ServerName") & "&nbsp;" & GenerateServerMassActionDropDown(row("ID").ToString()) & "<span id='summary-" + row("ID").ToString() + "'></span></div>"
                html += "<div class=""progress-bar"" id='progress-bar-server-" + row("ID").ToString() + "'></div>"
                html += "<ul class=""storeapp-list"" data-server-id='" + row("ID").ToString() + "'>"
                For Each row1 In dataset.Tables(1).Rows
                    If row1("ServerID") = row("ID") Then
                        html += GetInstance(row1)
                    End If
                Next
                html += "</ul></br>"
            Next
        End If
        htmlInfo.InnerHtml = html
    End Sub

    Private Function GenerateServerMassActionDropDown(serverId As String) As String
        Return "<select class='MassActionSelect hidden' id='Select-" + serverId + "' data-server-id='" + serverId + "' onchange='MassActionChange(this," + serverId + ")' ><option value=""massActions"">--Select a mass action--</option><option value=""Translations"">Set Clear to translation keys</option>" _
                + "<option value=""Specifications"">Set Clear to specification keys</option>" _
                + "<option value=""ProductLists"">Set Clear to product list keys</option>" _
                + "<option value=""Reset"">Reset action dropdown components</option>" _
                + "</select >"
    End Function

    Private Function GetInstance(row As DataRow) As String
        Dim html As String = "<li class=""storeapp available"">"
        html += "<div class=""folder""><table class=""width100percent""><tr><td>"
        html += "<img src=""Images/FlagsSop/" + ClsDataAccessHelper.GetText(row, "SOP_ID") + ".png"" width=""32"" height=""32"" style=""border-radius:3px""/>"
        html += "<td style=""padding:5px"">" + ClsDataAccessHelper.GetText(row, "Name") + "</td><td><a onclick='RefreshCacheValues(""" + row("ID").ToString() + """,""" + ClsDataAccessHelper.GetText(row, "InstancePathUrl") + """)'>Refresh</a></td></tr>"
        html += "<tr><td colspan=""3""><div style=""line-height:24px"" class=""loading"" data-country-name=""" + ClsDataAccessHelper.GetText(row, "Name") + """ data-sop-id=""" + ClsDataAccessHelper.GetText(row, "SOP_ID") + """ data-server-id=""" + row("ServerID").ToString() + """ data-instance-path=""" + ClsDataAccessHelper.GetText(row, "InstancePathUrl") + """ data-instance-id=""" + row("ID").ToString() + """ id=""cacheKeys-" + row("ID").ToString() + """><img src=""Images/Loading.gif"" width=""24"" height=""24"" style=""vertical-align: top;margin-right:5px"" />Loading cache values...</div></td></tr></table>"
        Return html
    End Function

    Private Sub LoadAllCacheKeys()
        ScriptManager.RegisterStartupScript(updatePanel1, updatePanel1.GetType(), "LoadAllCacheKeys", "LoadAllCacheKeys();", True)
    End Sub

End Class
