<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsChartPreview.aspx.vb" Inherits="EbusinessInsightsChartPreview" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css" rel="stylesheet" />
    <script src="Scripts/Insights.js?v=2"></script>
    <style type="text/css">
        body {
            overflow: hidden;
        }

        .grid-stack-item-content {
            max-height: 600px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>

            <asp:HiddenField runat="server" ID="ChartUID" />
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
                        <input type="button" class="btn red" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrCloseChartPreviewWindow(false)" />
                        <asp:Button runat="server" ID="btnExport" CssClass="btn green" OnClick="btnExport_Click" Text="Export to Excel" />
                    </td>
                </tr>
            </table>
            <asp:Panel runat="server" Width="1120" ID="chartPnlContainer"></asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnUpdate" />
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

