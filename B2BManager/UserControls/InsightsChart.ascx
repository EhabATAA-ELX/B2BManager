<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InsightsChart.ascx.vb" Inherits="UserControls_InsightsChart" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadWindow ID="ChartWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
    VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="560px" Height="640px" runat="server">
</telerik:RadWindow>

<telerik:RadWindow ID="ChartPreviewWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
    VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="1100px" Height="700px" runat="server">
</telerik:RadWindow>

<telerik:RadWindow ID="ManageSectionsWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
    VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="650px" Height="550px" runat="server">
</telerik:RadWindow>

<telerik:RadWindow ID="AreaWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
    VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="450px" Height="350px" runat="server">
</telerik:RadWindow>


<script type="text/javascript">

    function ShowOrCloseChartWindow(Show) {
        ChangeWindowDisplay("<%= ChartWindow.ClientID %>", Show);
    }

    function ShowOrCloseChartPreviewWindow(Show) {
        ChangeWindowDisplay("<%= ChartPreviewWindow.ClientID %>", Show);
    }

    function ShowOrCloseManageSectionsWindow(Show) {
        ChangeWindowDisplay("<%= ManageSectionsWindow.ClientID %>", Show);
    }

    function ManageSections() {
        var url = 'EbusinessInsightsManageSections.aspx';
        var oWnd = $find("<%= ManageSectionsWindow.ClientID %>");
        oWnd.setUrl(url + "?HideHeader=true");
        oWnd.set_title("Loading...");
        ShowOrCloseManageSectionsWindow(true);
    }

    function PreviewChart() {
        var url = 'EbusinessInsightsChartPreview.aspx';
        var oWnd = $find("<%= ChartPreviewWindow.ClientID %>");
        oWnd.setUrl(url + "?HideHeader=true");
        oWnd.set_title("Loading...");
        ShowOrCloseChartPreviewWindow(true);
    }

    function CreateChart() {
        var url = 'EbusinessInsightsChartProfile.aspx';
        var oWnd = $find("<%= ChartWindow.ClientID %>");
        oWnd.set_title("Loading...");
        oWnd.setUrl(url + "?HideHeader=true");
        ShowOrCloseChartWindow(true);
    }

    function EditChart(chartID) {
        var url = 'EbusinessInsightsChartProfile.aspx';
        var oWnd = $find("<%= ChartWindow.ClientID %>");
        oWnd.set_title("Loading...");
        oWnd.setUrl(url + "?HideHeader=true&cid=" + chartID);
        ShowOrCloseChartWindow(true);
    }

</script>
