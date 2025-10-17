
Imports System.Data
Imports System.Data.SqlClient
Imports Telerik.Web.UI

Partial Class B2BTranslationsAreas
    Inherits System.Web.UI.Page


    Dim dt As DataTable = Nothing

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Translation Key Areas"
        End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim logonUser As ClsUser = ClsSessionHelper.LogonUser
            If logonUser IsNot Nothing Then
                Dim EnvironmentID As Integer
                Dim Tn_GlobalID As String = ""
                If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
                    Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
                End If
                If Not String.IsNullOrEmpty(Context.Request.QueryString("Tn_GlobalID")) Then
                    Tn_GlobalID = Context.Request.QueryString("Tn_GlobalID")
                End If

                If Tn_GlobalID <> "" Then
                    dt = GetAreaByTranslationName(EnvironmentID, Tn_GlobalID)
                End If
                If dt.Rows.Count > 0 Then
                    Lbl_Translation_Name.Text = dt(0)("TN_Name")
                End If
                AreasID.DataSource = GetAreaByEnv(EnvironmentID)
                AreasID.DataBind()

                If Not logonUser.Actions.Contains(ClsHelper.ActionDesignation.ASSIGN_UNASSIGN_TRANSLATION_AREA) Then
                    AreasID.Enabled = False
                    BtnTranslationsAreas.Visible = False
                End If

            End If
        End If
    End Sub

    Public Function GetAreaByEnv(ByVal EnvironmentID As Integer) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_GetAreasByEnv", parameters)
        Return dataTable
    End Function

    Public Function GetAreaByTranslationName(ByVal EnvironmentID As Integer, ByVal Tn_GlobalID As String) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@TnGlobalID", Tn_GlobalID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_GetAreasByField", parameters)
        Return dataTable
    End Function
    Protected Sub AreasID_DataBinding(sender As Object, e As EventArgs)
        For Each item As RadListBoxItem In AreasID.Items
            If (dt.AsEnumerable.Where(Function(dr) dr("TA_AreaID").ToString = item.Value).Count > 0) Then
                item.Checked = True
            End If
        Next
    End Sub


    Protected Sub BtnTranslationsAreas_Click(sender As Object, e As EventArgs)
        Dim EnvironmentID As Integer
        Dim Tn_GlobalID As String = ""
        If Context.Request.QueryString("EnvironmentID") IsNot Nothing Then
            Integer.TryParse(Context.Request.QueryString("EnvironmentID"), EnvironmentID)
        End If
        If Not String.IsNullOrEmpty(Context.Request.QueryString("Tn_GlobalID")) Then
            Tn_GlobalID = Context.Request.QueryString("Tn_GlobalID")
        End If
        Dim DTA = GetAreaByTranslationName(EnvironmentID, Tn_GlobalID)
        If AreasID.CheckedItems.Count = 0 Then
            ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "Alert", "alert('you must at least choose one Area');", True)
        Else
            For Each item As RadListBoxItem In AreasID.CheckedItems
                If (DTA.AsEnumerable.Where(Function(dr) dr("TA_AreaID").ToString = item.Value).Count = 0) Then
                    AddNewAreaToTranslationName(EnvironmentID, Tn_GlobalID, item.Value)
                End If
            Next
            For Each row As DataRow In DTA.Rows
                If (AreasID.CheckedItems.AsEnumerable.Where(Function(dr) dr.Value.ToString = row("TA_AreaID").ToString()).Count = 0) Then
                    DeleteAreaFromTranslationName(EnvironmentID, Tn_GlobalID, row("TA_AreaID").ToString())
                End If
            Next

            ScriptManager.RegisterStartupScript(UpdatePanel2, UpdatePanel2.GetType(), "CloseWindow", "CloseWindow();", True)
        End If

    End Sub

    Public Function AddNewAreaToTranslationName(ByVal EnvironmentID As Integer, ByVal Tn_GlobalID As String, ByVal TA_AreaID As String) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@TnGlobalID", Tn_GlobalID))
        parameters.Add(New SqlParameter("@TA_ID", TA_AreaID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_InsertAreaByField", parameters)
        Return dataTable
    End Function

    Public Function DeleteAreaFromTranslationName(ByVal EnvironmentID As Integer, ByVal Tn_GlobalID As String, ByVal TA_AreaID As String) As DataTable
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        parameters.Add(New SqlParameter("@EnvironmentID", EnvironmentID))
        parameters.Add(New SqlParameter("@TnGlobalID", Tn_GlobalID))
        parameters.Add(New SqlParameter("@TA_ID", TA_AreaID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("Ebusiness.Translations_DeleteAreaByField", parameters)
        Return dataTable
    End Function
End Class
