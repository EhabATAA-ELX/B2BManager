
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports ClsInsightsHelper

Partial Class EbusinessInsightsManageSections
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not IsPostBack Then
            CType(Master.FindControl("title"), HtmlTitle).Text = "Manage Sections"
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ClsSessionHelper.LogonUser Is Nothing Then
            Return
        End If

        If IsPostBack Then
            Dim __EVENTTARGET As String = Request("__EVENTTARGET")
            Dim __EVENTARGUMENT As String = Request("__EVENTARGUMENT")
            Dim watch As Stopwatch = Stopwatch.StartNew()

            If Not String.IsNullOrEmpty(__EVENTTARGET) Then
                If __EVENTTARGET.Contains("SelectedSectionIDToDelete") Then
                    If "SubmitDeleteSection".Equals(__EVENTARGUMENT) Then
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@SectionID", CInt(SelectedSectionIDToDelete.Value)))
                        parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
                        If (ClsDataAccessHelper.ExecuteNonQuery("Insights.DeleteSection", parameters)) Then
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Delete dialog", "ShowOrCloseSectionDeletetWindow(false);", True)
                            RenderSections()
                            ClsHelper.Log("Insights Delete Section", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Section ID : " & SelectedSectionIDToDelete.Value, watch.ElapsedMilliseconds, False, Nothing)
                        Else
                            lblDeleteSectionErrorMessage.ForeColor = System.Drawing.Color.Red
                            lblDeleteSectionErrorMessage.Text = "An unexpected error has occurred. please try again later."
                            RenderSections()
                            ClsHelper.Log("Insights Delete Section", ClsSessionHelper.LogonUser.GlobalID.ToString(), "Section ID : " & SelectedSectionIDToDelete.Value, watch.ElapsedMilliseconds, True, "Failed to delete section")
                        End If
                    Else
                        SelectedSectionIDToDelete.Value = __EVENTARGUMENT
                        RenderSections()
                    End If
                End If

                If __EVENTTARGET.Contains("RefreshHiddenField") Then
                    If "Refresh".Equals(__EVENTARGUMENT) Then
                        RenderSections()
                    Else
                        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
                        parameters.Add(New SqlParameter("@SectionIDs", __EVENTARGUMENT))
                        parameters.Add(New SqlParameter("@UserGlobalID", ClsSessionHelper.LogonUser.GlobalID))
                        If (ClsDataAccessHelper.ExecuteNonQuery("Insights.UpdateSectionPositions", parameters)) Then
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Close Section dialog", "window.parent.ShowOrCloseManageSectionsWindow(false);", True)
                            RenderSections()
                            ClsHelper.Log("Change Section Positions", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, False, Nothing)
                        Else
                            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Alert Error", "alert('An unexpected error has occurred. please try again later.');", True)
                            RenderSections()
                            ClsHelper.Log("Change Section Positions", ClsSessionHelper.LogonUser.GlobalID.ToString(), ClsHelper.ConvertSQLParametersToFriendlyText(parameters), watch.ElapsedMilliseconds, True, "Failed to save section new positions")
                        End If
                    End If
                End If
            End If
        Else
            RenderSections()
        End If


    End Sub

    Private Sub RenderSections()
        Dim areas As List(Of Area) = GetAreas(ClsSessionHelper.LogonUser.GlobalID)
        Dim hasAtLeastOnAreaEditable As Boolean = False
        For Each area As Area In areas
            Dim liContainer As HtmlGenericControl = New HtmlGenericControl("li")
            liContainer.Attributes.Add("data-section-id", area.ID)
            liContainer.Attributes.Add("class", "column" & IIf(area.Editable, " column-draggable", ""))
            Dim htmlAreaTable As HtmlTable = New HtmlTable()
            htmlAreaTable.Attributes.Add("class", "table-container")
            Dim htmlTr As HtmlTableRow = New HtmlTableRow()
            Dim editTitle As String = "Edit Area"
            Dim deleteTitle As String = "Delete Area"
            If Not area.Editable Then
                editTitle = "You don't have the right to edit this section"
                deleteTitle = "You don't have the right to delete this section"
            Else
                If Not area.Deletable Then
                    deleteTitle = "This section contains at least one chart and cannot be deleted"
                End If
            End If
            Dim htmlEditBtnCell As HtmlTableCell = New HtmlTableCell()
            htmlEditBtnCell.Attributes.Add("class", "action-image")
            htmlEditBtnCell.InnerHtml = "<img src=""Images/edit.png"" title=""" & editTitle & """ draggable=""false"" " & IIf(area.Editable, "onclick=""EditArea('" & area.ID & "')"" class=""MoreInfoImg"" ", " class=""ImgDisabled"" ") & " />"
            Dim htmlDeleteBtnCell As HtmlTableCell = New HtmlTableCell()
            htmlDeleteBtnCell.Attributes.Add("class", "action-image")
            htmlDeleteBtnCell.InnerHtml = "<img src=""Images/delete.png"" title=""" & deleteTitle & """ draggable=""false"" " & IIf(area.Deletable, "onclick=""DeleteArea('" & area.ID & "')"" class=""MoreInfoImg"" ", " class=""ImgDisabled"" ") & " />"
            Dim htmlAreaTittleInfo As HtmlTableCell = New HtmlTableCell()
            Dim areaTitleContainer As HtmlTable = New HtmlTable
            Dim areaTitleRow As HtmlTableRow = New HtmlTableRow()
            Dim areaTitle As HtmlTableCell = New HtmlTableCell()
            areaTitle.Attributes.Add("class", "content")
            areaTitle.InnerHtml = area.Name
            areaTitleRow.Cells.Add(areaTitle)
            If area.ShowHelpTooltip And Not String.IsNullOrWhiteSpace(area.TooltipText) Then
                Dim areaHelpTooltip As HtmlTableCell = New HtmlTableCell
                areaHelpTooltip.InnerHtml = String.Format("<img src='Images/Info.png' class='MoreInfoImg' id='ImgTooltipHelp_{0}' width='18' height='18' alt='More details' /><div class='hidden' style='margin:25px;' id=""TooltipContentHelp_{0}"" >{1}</div>", area.ID, area.TooltipText)
                Me.RadToolTipManager1.TargetControls.Add(String.Format("ImgTooltipHelp_{0}", area.ID), True)
                areaTitleRow.Cells.Add(areaHelpTooltip)
            End If
            areaTitleContainer.Rows.Add(areaTitleRow)
            htmlAreaTittleInfo.Controls.Add(areaTitleContainer)
            'Add all cells to area table
            htmlTr.Cells.Add(htmlEditBtnCell)
            htmlTr.Cells.Add(htmlDeleteBtnCell)
            htmlTr.Cells.Add(htmlAreaTittleInfo)
            'Add html table row to area table
            htmlAreaTable.Rows.Add(htmlTr)
            'Add html table to LI container
            liContainer.Controls.Add(htmlAreaTable)
            If area.Editable Then
                liContainer.Attributes.Add("draggable", "true")
            End If
            'Add LI to LU areas
            areasListContainer.Controls.Add(liContainer)
            If area.Editable Then
                hasAtLeastOnAreaEditable = True
            End If
        Next

        btnSaveSectionPositions.Visible = hasAtLeastOnAreaEditable
        If hasAtLeastOnAreaEditable Then
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "InitCells", "InitCells();", True)
        Else
            DragAndDropHeader.Visible = False
        End If
    End Sub

End Class
