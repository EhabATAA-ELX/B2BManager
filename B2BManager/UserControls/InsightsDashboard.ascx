<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InsightsDashboard.ascx.vb" Inherits="UserControls_InsightsDashboard" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadWindow ID="DashboardWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
    VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="800px" Height="650px" runat="server">
</telerik:RadWindow>

<script type="text/javascript">

    function ShowOrCloseDashboardWindow(Show) {
        var oWnd = $find("<%= DashboardWindow.ClientID %>");
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

    function CreateDashboard() {
        var url = 'EbusinessInsightsDashboardProfile.aspx';
        var oWnd = $find("<%= DashboardWindow.ClientID %>");
        oWnd.set_title("Loading...");
        oWnd.setUrl(url + "?HideHeader=true");
        ShowOrCloseDashboardWindow(true);
    }

    function EditDashboard(dashboardID) {
        var url = 'EbusinessInsightsDashboardProfile.aspx';
        var oWnd = $find("<%= DashboardWindow.ClientID %>");
        oWnd.setUrl(url + "?HideHeader=true&did=" + dashboardID);
        oWnd.set_title("Loading...");
        ShowOrCloseDashboardWindow(true);
    }

    function LoadDashboard(dashboardID) {
        if (typeof LoadDashboardByID == "function") {
            LoadDashboardByID(dashboardID);
        }
        else {
            window.parent.LoadDashboardByID(dashboardID);
        }
        ShowOrCloseDashboardWindow(false);
        if (typeof window.parent.ShowOrCloseWindow == "function") {
            window.parent.ShowOrCloseWindow("Dashboards", false);
        }
        else {
            window.close();
        }
    }
</script>
