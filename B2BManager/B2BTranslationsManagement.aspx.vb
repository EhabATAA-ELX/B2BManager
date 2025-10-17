
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports ClsHelper

Partial Class B2BTranslationsManagement
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim watch As Stopwatch = Stopwatch.StartNew()
            RenderControls()
            ClsHelper.Log("Load B2B Translations", ClsSessionHelper.LogonUser.GlobalID.ToString(), Nothing, watch.ElapsedMilliseconds, False, Nothing)
        End If
    End Sub

    Protected Function GetEditTemplate() As String
        Dim EditTemplate As String = ""
        Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
        Dim CanEdit As Boolean = False
        If logonUser.Actions.Count > 0 Then
            If logonUser.Actions.Contains(ActionDesignation.EDIT_LOCAL_TRANSLATION_VALUE) Then
                CanEdit = True
                If Not IsPostBack Then
                    HD_EditLOCALVALUE.Value = "1"
                End If
            End If

            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.EDIT_COMMENTS_AND_DEFAULT_TRANSLATION_VALUE) Then
                CanEdit = True
                If Not IsPostBack Then
                    HD_EDITALL.Value = "1"
                End If
            End If
        End If

        If CanEdit Then
            EditTemplate = "<img id=""#data#""  src=""Images/Edit.png"" title=""Edit Translation"" class=""MoreInfoImg"" onclick=""GetEditRow(this)""  width=""20"" height=""20"">"
        Else
            If logonUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_UNASSIGN_TRANSLATION_AREA) Then
                TableHeaderCell0.Text = "Manage Area(s)"
            Else
                TableHeaderCell0.Text = "Display Area(s)"
            End If
        End If
        Return EditTemplate
    End Function

    Private Sub RenderControls(Optional ByVal selectedValue As String = Nothing)
        Dim applications As List(Of ClsHelper.Application) = Nothing
        Dim clsUser As ClsUser = ClsSessionHelper.LogonUser
        If clsUser Is Nothing Then
            Return
        End If
        Dim selectedApplication As ClsHelper.Application = ClsSessionHelper.LogonUser.Applications.Where(Function(fn) (fn.ID = 1)).SingleOrDefault()
        ClsHelper.RenderDropDownList(ddlEnvironment, selectedApplication.Environments, True, False, IIf(ClsSessionHelper.EbusinessEnvironmentID IsNot Nothing, ClsSessionHelper.EbusinessEnvironmentID, ClsSessionHelper.LogonUser.DefaultEbusinessEnvironmentID))
        ClsHelper.RenderCountryDropDown(ddlCountry, selectedApplication.Countries, IIf(ClsSessionHelper.EbusinessSopID IsNot Nothing, ClsSessionHelper.EbusinessSopID, ClsSessionHelper.LogonUser.DefaultEbusinessSopID), False)
        GetAreaByEnv()
        GetLanguagesBycountry()
    End Sub


    Private Sub LoadGrid()
        ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "LoadGrid", "LoadGrid();", True)
    End Sub
    Public Sub GetAreaByEnv()
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", ddlEnvironment.SelectedValue))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_GetAreasByEnv", parameters)
        ddlArea.Items.Insert(0, New ListItem("All", "0"))
        For Each row As DataRow In dataTable.Rows
            ddlArea.Items.Add(New ListItem(row("TA_Name").ToString(), row("TA_AreaID").ToString()))
        Next
        ddlArea.SelectedIndex = 0
    End Sub

    Public Sub GetLanguagesBycountry()
        Dim dt = ClsEbusinessHelper.GetLanguages(ddlEnvironment.SelectedValue, Page.Cache)
        If dt IsNot Nothing Then
            Dim dr As DataRow() = dt.Select("S_SOP_ID='" + ddlCountry.SelectedValue + "'")
            If dr.Length = 1 Then
                HdLanguage.Value = dr(0)("LANG_ISOCODE")
                ddlLanguage.Visible = False
                lblLanguage.Visible = False
            Else
                ddlLanguage.Items.Clear()
                ddlLanguage.Visible = True
                lblLanguage.Visible = True
                For Each row As DataRow In dr
                    ddlLanguage.Items.Add(New ListItem(row("LANG_NAME").ToString(), row("LANG_ISOCODE").ToString()))
                Next
            End If
        End If
    End Sub

    Protected Sub ddlEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadGrid()
    End Sub
    Protected Sub ddlCountry_SelectedIndexChanged(o As Object, e As Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs)
        GetLanguagesBycountry()
        LoadGrid()
    End Sub
    Protected Sub ddlArea_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadGrid()
    End Sub
    Protected Sub ddlLanguage_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadGrid()
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As EventArgs) Handles Me.SaveStateComplete
        Try
            If IsPostBack Then
                ClsSessionHelper.EbusinessEnvironmentID = ddlEnvironment.SelectedValue
                ClsSessionHelper.EbusinessSopID = ddlCountry.SelectedValue
            End If
        Catch ex As Exception
            Dim exceptionMessage As String = "", exceptionStackTrace As String = ""
            If Not ex.Message Is Nothing Then
                exceptionMessage = ex.Message
            End If
            If Not ex.StackTrace Is Nothing Then
                exceptionStackTrace = ex.StackTrace
            End If
            Dim errorMsg As String = String.Format("<b>Excepetion Message:</b></br>{0}</br>" _
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage _
                        , exceptionStackTrace)
            ClsSendEmailHelper.SendErrorEmail(errorMsg)
        End Try
    End Sub

End Class
