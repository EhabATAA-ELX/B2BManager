<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PauseIntervalsManager.ascx.vb" Inherits="UserControls_PauseIntervalsManager" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function ShowOrClosePauseIntervalWindow(windowIdentifier, Show) {
        var oWnd = null;
        switch (windowIdentifier) {
            case "ManagePauseInterval":
                oWnd = $find("<%= WindowAddPauseInterval.ClientID %>");
                break;
            case "DeletePauseInterval":
                oWnd = $find("<%= WindowDeletePauseInterval.ClientID %>");
        }
        if (oWnd != null) {
            if (Show) {
                oWnd.show();
            }
            else {
                oWnd.close();
            }
        }
        return false;
    }

    function BindPauseIntervals() {
        __doPostBack('CurrentUID', '');
        return false;
    }

    function PauseIntervalClick(pauseIntervalID) {
        var url = 'MonitoringPauseIntervalProfile.aspx?UID=' + $('#CurrentUID').val() + "&IsUIDWorkflowID=" + $('#CurrentIsUIDWorkflowID').val();
        if (pauseIntervalID && pauseIntervalID != null) {
            url += "&pauseIntervalID=" + pauseIntervalID.toString();
        }
        if ($(window).height() > 320 && $(window).width() > 500) {
            var oWnd = $find("<%= WindowAddPauseInterval.ClientID %>");
            oWnd.setUrl(url + "&HideHeader=true");
            oWnd.show();
        }
        else {
            popup(url);
        }
        return false;
    }

    function DeletePauseIntervalClick(pauseIntervalID) {
        __doPostBack('DeletePauseInterval', pauseIntervalID);
        return false;
    }

    function ProcessPauseIntervalButton(sender) {

        switch (sender) {
            case "Delete":
                $('#<%= BtnDeletePauseInterval.ClientID %>').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                break;
        }
        return false;
    }
</script>

<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <asp:HiddenField ID="DeletePauseInterval" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="CurrentUID" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="CurrentIsUIDWorkflowID" runat="server" ClientIDMode="Static" />
        <table align="center" style="width: 100%">
            <tr>
                <td align="center">
                    <input type="button" id="btnAddPauseIntervall" value="Add Pause Interval" onclick="PauseIntervalClick(null)" class="btn bleu" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid runat="server" MasterTableView-ShowHeadersWhenNoRecords="false" ID="monitoringPauseIntervalsGrid" CssClass="MonitoringGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="20" GroupingEnabled="true">
                        <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Actions">
                                    <ItemTemplate>
                                        <img src='Images/Edit.png' width="20" class="MoreInfoImg" height="20" title="Display/edit details"
                                            onclick="PauseIntervalClick('<%# Eval("PauseIntervalID").ToString() %>')" />
                                        <img src='Images/delete.png' width="20" class="MoreInfoImg" height="20" title="Delete Pause Interval"
                                            onclick="DeletePauseIntervalClick('<%# Eval("PauseIntervalID").ToString() %>')" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="PauseIntervalID" HeaderText="ID">
                                </telerik:GridBoundColumn>
                                <telerik:GridCheckBoxColumn AllowSorting="true" AllowFiltering="true" DataField="OccursEveryDay" HeaderText="Occurs Every Day?">
                                </telerik:GridCheckBoxColumn>
                                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="StartPauseTime" HeaderText="Start Pause Time">
                                    <ItemTemplate>
                                        <span class="verticalAlignTop"><%# ClsHelper.GetTime(Eval("StartPauseTimeHour"), Eval("StartPauseTimeMinute")) %></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="EndPauseTime" HeaderText="End Pause Time">
                                    <ItemTemplate>
                                        <span class="verticalAlignTop"><%# ClsHelper.GetTime(Eval("EndPauseTimeHour"), Eval("EndPauseTimeMinute")) %></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="false" DataField="StartDayOfOccurenceFormatted" HeaderText="Start Day Of Occurence">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="false" DataField="EndDayOfOccurenceFormatted" HeaderText="End Day Of Occurence">
                                </telerik:GridBoundColumn>
                            </Columns>
                            <NoRecordsTemplate>
                                No pause intervals were found
                            </NoRecordsTemplate>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
        <telerik:RadWindow ID="WindowAddPauseInterval" RenderMode="Lightweight" Modal="true" DestroyOnClose="true" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add Pause Interval" Behaviors="Close" Width="500" Height="320px" runat="server">
        </telerik:RadWindow>
        
        <telerik:RadWindow ID="WindowDeletePauseInterval" RenderMode="Lightweight" Modal="true" DestroyOnClose="true" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Pause Interval Confirmation" Behaviors="Close" Width="400" Height="150px" runat="server">
            <ContentTemplate>
                <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                    <ContentTemplate>
                        <table align="center" style="margin-top: 10px">
                            <tr class="Height30px">
                                <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete the pause interval ID: 
                            <asp:Label runat="server" ID="lblPauseIntervalIDToDelete" CssClass="Electrolux_light width230px"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <button class="btn red" id="BtnCancelDeletePauseInterval" onclick="ShowOrClosePauseIntervalWindow('DeletePauseInterval',false)">Cancel</button>
                                    <asp:LinkButton CssClass="btn green" ID="BtnDeletePauseInterval" runat="server" Text="Delete" OnClick="BtnDeletePauseInterval_Click" OnClientClick="ProcessPauseIntervalButton('Delete')"></asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="DeletePauseInterval" />
        <asp:AsyncPostBackTrigger  ControlID="CurrentUID" />
    </Triggers>
</asp:UpdatePanel>


