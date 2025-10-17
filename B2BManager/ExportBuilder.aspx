<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ExportBuilder.aspx.vb" Inherits="ExportBuilder" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/ToolsRepeater.ascx" TagPrefix="uc1" TagName="ToolsRepeater" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .RadComboBoxDropDown .rcbImage {
            width: auto !important;
        }
        .Padding15px{
            padding:15px;
        }
        .RadTreeView .rtImg {
            height: 20px;
        }
    </style>
    <script type="text/javascript">
        function OnVisualizeClick() {
            $('#<%= btnVisualize.ClientID %>').addClass("loadingBackground").html("Processing..").prop('disabled', true);
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table style="width: 100%">
                <tr>
                    <td style="width: 450px; border-right: 1px solid #BDC0C4; padding-left: 25px" valign="top">
                        <table cellpadding="5" cellspacing="5">
                            <tr class="Height30px">
                                <td class="width180px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Export Title:</asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtBoxExportName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="width180px">
                                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                                </td>
                                <td class="width230px">
                                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlEnvironment">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="width180px">
                                    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                                </td>
                                <td class="width230px">
                                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlCountry">
                                        <Items>
                                            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trExistingTemplatesUsage">
                                <td colspan="3" align="center">
                                    <asp:Label runat="server" ID="lbltemplate" CssClass="Electrolux_light_bold Electrolux_Color">Use existing template:</asp:Label>
                                    <label class="switch">
                                        <asp:CheckBox runat="server" OnCheckedChanged="chkBoxUseTemplate_CheckedChanged" Checked="false" AutoPostBack="true" ID="chkBoxUseTemplate" />
                                        <span class="slider round"></span>
                                    </label>
                                </td>
                            </tr>
                            <tr runat="server" id="trExistingTemplates">
                                <td class="width180px">
                                    <asp:Label runat="server" ID="lblExistingTemplates" CssClass="Electrolux_light_bold Electrolux_Color">Existing Templates:</asp:Label>
                                </td>
                                <td class="width230px">
                                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AutoPostBack="true" OnSelectedIndexChanged="ddlExistingTemplates_SelectedIndexChanged" AppendDataBoundItems="true" ID="ddlExistingTemplates">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr class="Height30px" runat="server" id="trTemplateName">
                                <td class="width180px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Template Name:</asp:Label>
                                </td>
                                <td colspan="2">
                                    <asp:TextBox ID="txtBoxTemplateName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="width180px">
                                    <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Accessibility Level:</asp:Label>
                                </td>
                                <td class="width230px">
                                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlTemplateType">
                                        <Items>
                                            <telerik:RadComboBoxItem ImageUrl="Images/Private.png" Text="Private" Value="0" Selected="true" />
                                            <telerik:RadComboBoxItem ImageUrl="Images/Shared.png" Text="Shared" Value="1" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="width180px">
                                    <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color">Data Source:</asp:Label>
                                </td>
                                <td class="width230px">
                                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" AutoPostBack="true" OnSelectedIndexChanged="ddlDataSource_SelectedIndexChanged" AppendDataBoundItems="true" ID="ddlDataSource">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <fieldset>
                                        <legend class="Electrolux_light_bold Electrolux_Color">Available fields</legend>
                                        <telerik:RadTreeView runat="server" ID="tvFields" CheckBoxes="true" ShowLineImages="false" CheckChildNodes="true" TriStateCheckBoxes="true" Width="400" Height="350">
                                        </telerik:RadTreeView>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:Label runat="server" ID="lblInfoMessage" ForeColor="Red" Text=" "></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:LinkButton runat="server" ID="btnVisualize" CssClass="btn bleu" OnClientClick="OnVisualizeClick()" OnClick="btnVisualize_Click" Text="Visualize" />
                                    <asp:Button runat="server" ID="btnReset" CssClass="btn red" OnClick="btnRest_Click" Text="Reset" />
                                    <asp:Button runat="server" ID="btnExport" CssClass="btn green" OnClick="btnExport_Click" Text="Export to Excel" />
                                </td> 
                            </tr>
                        </table>
                    </td>
                    <td align="center" class="verticalAlignTop">
                        <asp:Panel runat="server" ID="panelVisualizeInfo">
                        <h2 style="color: #BDC0C4;margin-top:250px">Enter the title, select the data and fields you want to visualize/export and then press Visualize</h2>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="panelGridResult" Visible="false" CssClass="Padding15px">
                        <telerik:RadGrid runat="server" ID="gridResult" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" OnNeedDataSource="gridResult_NeedDataSource" PageSize="20" GroupingEnabled="true">
                            <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                            </MasterTableView>
                        </telerik:RadGrid>
                        </asp:Panel>
                    </td>
                </tr>
            </table>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="gridResult" />
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

