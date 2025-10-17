<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPage.master" CodeFile="TP2PendingOrders.aspx.vb" Inherits="TP2PendingOrders" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
      <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }
    </script>
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table class="Filters">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" OnSelectedIndexChanged="handleRefreshEvent" DataTextField="Name" AutoPostBack="true" DataValueField="ID">
                        </asp:DropDownList>
                    </td>
                   <td>
                <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
            </td>
            <td class="width180px">
                <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="handleRefreshEvent" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                    </Items>
                </telerik:RadComboBox>
            </td>
                    
                       <td>
                        <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">From:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblTo" CssClass="Electrolux_light_bold Electrolux_Color">To:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadDateTimePicker ID="RadDateTimePickerTo" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDateTimePicker>
                    </td>   
                    <td>
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="ProcessButton(this,'Searching...')" OnClick="handleRefreshEvent"><i class="fas fa-search"></i> Search</asp:LinkButton>
                   </td>
                </tr>
            </table>
            <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                <ClientEvents OnRequestStart="RequestStartSimple" />
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="ddlEnvironment">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="btnSearch">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="ddlCountry">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="gridSearch">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" ClientIDMode="Static" IsSticky="true" Transparency="10" runat="server" Style="position: absolute; z-index: 999;">
                <asp:Image ID="Image1" runat="server" AlternateText="Loading..." ImageUrl="Images/Loading.gif" />
            </telerik:RadAjaxLoadingPanel>
            <div id="gridContainer" runat="server" style="position: relative; min-height: 500px">
                <span id="lblInformationContainer" enableviewstate="false" runat="server" style="position: absolute; top: 5px"><span class="information-label" runat="server" id="lblInformation"></span></span>
                <telerik:RadGrid runat="server" ID="gridSearch" ShowGroupPanel="true" AutoGenerateColumns="true" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" ClientSettings-DataBinding-EnableCaching="true"
                    PageSize="20" OnNeedDataSource="gridSearch_NeedDataSource" GroupingEnabled="true">
                    <ClientSettings AllowDragToGroup="true" />
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                        <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced" />
                        <Columns>
                           <telerik:GridTemplateColumn DataField="CY_NAME_ISOCODE" GroupByExpression="CY_NAME_ISOCODE Group By CY_NAME_ISOCODE" AllowFiltering="true" Groupable="true" UniqueName="CountryName" HeaderText="Country" >
                                <ItemTemplate>
                                    <img src='Images/Flags/<%# Eval("Country").ToString() %>.png' width="20" height="16" title="<%# Eval("Country") %>" />&nbsp;
                                <span class="verticalAlignTop"><%# Eval("Country") %></span>
                                </ItemTemplate>
                               <HeaderStyle Width="100px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="DateCreat" HeaderText="Date / Time of re-process">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="CustomerName" HeaderText="Customer name">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="CustomerCode" HeaderText="Customer code">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="SalesOrderID" HeaderText="SAP’s order Number">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="PONumber" HeaderText="Customer’s PO Number">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>