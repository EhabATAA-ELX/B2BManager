<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsDashboardsManager.aspx.vb" Inherits="EbusinessInsightsDashboardsManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/InsightsDashboard.ascx" TagPrefix="uc1" TagName="InsightsDashboard" %>
<%@ Register Src="~/UserControls/InsightsChart.ascx" TagPrefix="uc1" TagName="InsightsChart" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script type="text/javascript">
        function DeleteDashboard(dashboardID) {
            __doPostBack('SelectedDashboardIDToDelete', dashboardID);
            ShowOrCloseWindow('DeleteDashboard', true);
        }

        function ShowOrCloseWindow(windowIdentifier, Show) {
            var oWnd = null;
            switch (windowIdentifier) {
                case "DeleteDashboard":
                    $("#lblDeleteActionErrorMessage").text("");
                    oWnd = $find("<%= WindowDeleteDashboard.ClientID%>");
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

        function OpenDashboard(dashboardID) {

            if (typeof window.parent.LoadDashboardByID == "function") {
                window.parent.LoadDashboardByID(dashboardID);
                window.parent.ShowOrCloseWindow("Dashboards", false);
            }

            if (typeof window.opener.LoadDashboardByID == "function") {
                window.opener.LoadDashboardByID(dashboardID);
                window.close();
            }
        }

        function ProcessDeleteDashboard() {

            $('#BtnDeleteDashboard').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
            __doPostBack("SelectedDashboardIDToDelete", "SubmitDeleteDashboard")
            return false;
        }
    </script>
    <style type="text/css">
        .RadGrid .rgRow {
            height: 45px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table align="center">
        <tr>
            <td>
                <input type="button" value="New Dashboard" id="btnCreateDashboard" onclick="CreateDashboard()" class="btn blue" />
                <input type="button" class="btn bleu" id="btnManageSection" value="Manage Sections" onclick="ManageSections()" />
                <asp:PlaceHolder runat="server" ID="btnNewChartPlaceHolder"><input type="button" value="New Chart" id="btnCreateChart" onclick="CreateChart()" class="btn blue" /></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <uc1:InsightsDashboard runat="server" ID="InsightsDashboard" />
    <uc1:InsightsChart runat="server" ID="InsightsChart" />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div style="height:600px;max-height:600px;">
                <telerik:RadGrid runat="server" MasterTableView-ShowHeadersWhenNoRecords="false" ID="dashboardsGrid" CssClass="MonitoringGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="15" GroupingEnabled="true">
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Open" AllowFiltering="false" HeaderText="Open">
                                <ItemTemplate>
                                    <img src="Images/Insights/open.png" width="20" title="Load Dashboard" onclick="OpenDashboard('<%# Eval("ID").ToString() %>')" class="MoreInfoImg" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Actions">
                                <ItemTemplate>
                                    <table style="padding:0;margin:0;border:none">
                                        <tr style="padding:0;margin:0;border:none">
                                            <td style="padding:0;margin:0;border:none">
                                                <img src='Images/Edit.png' width="20" height="20" title="Edit Dashboard" class="<%# IIf(Eval("Editable"), "MoreInfoImg", "ImgDisabled")  %>"
                                        onclick="<%# IIf(Eval("Editable"), "EditDashboard('" & Eval("ID").ToString() & "')", "return false;")  %>" />
                                            </td>
                                            <td style="padding:0;margin:0;border:none">
                                                <img src='Images/delete.png' width="20" class="<%# IIf(Eval("Editable"), "MoreInfoImg", "ImgDisabled")  %>" height="20" title="Delete Dashboard" onclick="<%# IIF(Eval("Editable"), "DeleteDashboard('" & Eval("ID").ToString() & "')", "return false;")  %>" />
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="Name" HeaderText="Name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="IsShared" AllowFiltering="true" UniqueName="IsShared" HeaderText="Accessibility Level">
                                <ItemTemplate>
                                    <img src='Images/<%# Eval("SharingStatus").ToString()%>.png' height="18" title="<%# Eval("SharingStatus") %>" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="CreatedByName" HeaderText="Created By">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="CreatedOn" HeaderText="Created On">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="LastModifiedByName" HeaderText="Last Modified By">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="LastModifiedOn" HeaderText="Last Modified On">
                            </telerik:GridBoundColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            No dashboards available.
                        </NoRecordsTemplate>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
            <div style="width:100%;text-align:center">
                <input type="button" class="btn red" id="btnCloseWindow" value="Cancel" onclick="window.parent.ShowOrCloseWindow('Dashboards', false)" />                                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowDeleteDashboard" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Dashboard Confirmation" Behaviors="Close" Width="400px" Height="150px" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel5">
                <ContentTemplate>
                    <asp:HiddenField ID="SelectedDashboardIDToDelete" runat="server" ClientIDMode="Static" />
                    <table align="center" style="margin-top: 10px">
                        <tr class="Height30px">
                            <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete this dashboard?</td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="lblDeleteActionErrorMessage" ClientIDMode="Static" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <input type="button" class="btn red" id="BtnCancelDeleteDashboard" value="Cancel" onclick="ShowOrCloseWindow('DeleteDashboard', false)" />
                                <input type="button" class="btn green" id="BtnDeleteDashboard" value="Delete" onclick="ProcessDeleteDashboard()" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SelectedDashboardIDToDelete" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

