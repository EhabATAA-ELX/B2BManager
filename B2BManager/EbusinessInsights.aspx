<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsights.aspx.vb" Inherits="EbusinessInsights" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/InsightsDashboard.ascx" TagPrefix="uc1" TagName="InsightsDashboard" %>
<%@ Register Src="~/UserControls/InsightsChart.ascx" TagPrefix="uc1" TagName="InsightsChart" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/gridStack/jquery.min.js"></script>
    <script src="Scripts/gridStack/jquery-ui.js"></script>
    <script src="Scripts/gridStack/lodash.min.js"></script>
    <script src="Scripts/gridStack/gridstack.js"></script>
    <script src="Scripts/gridStack/gridstack.jQueryUI.js"></script>
    <link href="CSS/gridStack/gridstack.css" rel="stylesheet" />
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <script src="Scripts/jspdf/html2canvas.min.js"></script>
    <script src="Scripts/jspdf/jspdf.min.js"></script>
    <link href="CSS/Insights.css" rel="stylesheet" />
    <script src="Scripts/Insights.js?v=2"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <uc1:InsightsChart runat="server" ID="InsightsChart" />
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ImgTooltipHelp_DesignMode" />
        </TargetControls>
    </telerik:RadToolTipManager>
    <telerik:RadWindow ID="WindowConfirmChanges" RenderMode="Lightweight" VisibleTitlebar="false"  Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Behaviors="None" Width="300px" Height="120px" runat="server">
        <ContentTemplate>
            <table align="center" style="margin-top: 10px">
                <tr class="Height30px">
                    <td colspan="3" align="center" id="windowInformationText" style="width: 100%; margin: 25px;">Hello</td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <input type="button" class="btn red" value="Close" onclick="ChangeWindowDisplay('ContentPlaceHolder1_WindowConfirmChanges',false)" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </telerik:RadWindow>
    <uc1:InsightsDashboard runat="server" ID="InsightsDashboard" />
    <table class="Filters no-print">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
            </td>
            <td>
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlEnvironment">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
            </td>
            <td>
                <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlCountry">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td>
                <asp:Label runat="server" ID="lblCountrySplit" CssClass="Electrolux_light_bold Electrolux_Color">Country Split:</asp:Label>
                <label class="switch">
                    <asp:CheckBox runat="server" ID="chkBoxCountrySplit" />
                    <span class="slider round"></span>
                </label>
            </td>
            <td>
                <asp:LinkButton runat="server" ID="btnUpdate" CssClass="btn bleu" OnClick="btnUpdate_Click" Text="Submit"></asp:LinkButton>
                <asp:Button runat="server" ID="btnExport" CssClass="btn green" OnClick="btnExport_Click" Text="Export to Excel" />
                <input type="button" value="Export to PDF" onclick="ExportToPDF()" id="PDF" class="btn lightblue" />
            </td>
        </tr>
    </table>

    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <div>
                <div style="position: absolute; right: 10px; padding-top: -5px;">
                    <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color vertical-align-bottom">Design Mode</asp:Label>
                    <img src='Images/Info.png' class='MoreInfoImg vertical-align-bottom' id='ImgTooltipHelp_DesignMode' width='18' height='18' alt='More details' />
                    <div class='hidden' style='margin: 25px;' id="TooltipContentHelp_DesignMode">Activate the design mode to move or resize chart panels.
                        <br />
                        Click on the save icon to save your changes.
                        <br />
                        You can revert back to initial positions by deactivating the mode again.</div>
                    <label runat="server" class="switch">
                        <asp:CheckBox runat="server" ID="chkBoxDesignMode" ClientIDMode="Static" AutoPostBack="true" Checked="false" />
                        <span class="slider round"></span>
                    </label>
                    <img src="Images/Insights/save.png" title="Save design changes" runat="server" id="imgBtnSave" class="MoreInfoImg vertical-align-bottom ImgDisabled" />
                    <img src="Images/Insights/edit.png" title="Edit Dashboard" runat="server" id="imgBtnEdit" class="MoreInfoImg vertical-align-bottom" />
                    <img src="Images/Insights/open.png" title="Open a saved dashboard" class="MoreInfoImg vertical-align-bottom" onclick="OpenDashboards()" />
                    <img src="Images/Insights/new.png" title="New Dashboard" class="MoreInfoImg vertical-align-bottom" onclick="CreateDashboard()" />
                    <img src="Images/Insights/preferences.png" title="Change Preferences" class="MoreInfoImg vertical-align-bottom" onclick="ManagePreferences()" />
                </div>
                <div id="dashboard">
                    <div runat="server" id="DashobardTitle" class="dashobard-title"></div>
                    <asp:Panel runat="server" ID="PnlDashobardCharts" CssClass="dashobard-container"></asp:Panel>
                </div>
            </div>
            <asp:HiddenField runat="server" ID="CurrentDashobardUID" />
            <asp:HiddenField runat="server" ClientIDMode="Static" ID="CurrentDashboardID" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnUpdate" />
            <asp:AsyncPostBackTrigger ControlID="CurrentDashboardID" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowDashboards" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="1200px" Height="750px" runat="server">
    </telerik:RadWindow>

    <telerik:RadWindow ID="PreferencesWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
        VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server">
    </telerik:RadWindow>

    <script type="text/javascript">

         function ShowOrClosePreferencesWindow(Show) {
            ChangeWindowDisplay("<%= PreferencesWindow.ClientID %>", Show);
        }
    
        function ManagePreferences() {
            var url = 'EbusinessInsightsPreferences.aspx';
            var oWnd = $find("<%= PreferencesWindow.ClientID %>");
            oWnd.setUrl(url + "?HideHeader=true");
            oWnd.set_title("Loading...");
            ShowOrClosePreferencesWindow(true);
        }    


        function ShowOrCloseWindow(windowIdentifier, Show) {
            var oWnd = null;
            switch (windowIdentifier) {
                case "Dashboards":
                    oWnd = $find("<%= WindowDashboards.ClientID %>");
                    break;
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

        function OpenDashboards() {
            var url = 'EbusinessInsightsDashboardsManager.aspx';
            if ($(window).height() > 750 && $(window).width() > 1200) {
                var oWnd = $find("<%= WindowDashboards.ClientID %>");
                oWnd.setUrl(url + "?HideHeader=true");
                ShowOrCloseWindow("Dashboards", true);
            }
            else {
                popup(url, true);
            }
        }

        function LoadDashboardByID(dashboardID) {
            __doPostBack('CurrentDashboardID', dashboardID);
        }

        function initChartContainer() {
            var options = {
                cellHeight: 85,
                verticalMargin: 10
            };
            $('.grid-stack').gridstack(options);
        }

        $(function () {
            initChartContainer();
        });

    </script>

</asp:Content>

