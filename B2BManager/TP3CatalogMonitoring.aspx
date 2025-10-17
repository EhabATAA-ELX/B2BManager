<%@ Page Language="VB" AutoEventWireup="false"  MasterPageFile="~/MasterPage.master" CodeFile="TP3CatalogMonitoring.aspx.vb" Inherits="TP3CatalogMonitoring" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
      <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }

        function RadDatePicker_SetMaxDateToCurrentDate() {
            var datePicker = $find("<%= RadDateTimePickerFrom.ClientID %>");
            datePicker.set_maxDate(new Date());
        }; 
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
                        <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">Date:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" runat="server" OnSelectedDateChanged="handleRefreshEvent" AutoPostBack="true"  >
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today">
                                        <ItemStyle CssClass="rcToday" />
                                    </telerik:RadCalendarDay>
                                </SpecialDays>
                                <ClientEvents OnLoad="RadDatePicker_SetMaxDateToCurrentDate" /> 
                            </Calendar>
                        </telerik:RadDatePicker>
                    </td>                     
                </tr>
                <tr>
                    <td colspan="5"></td>
                     <td>
                                    <asp:Label runat="server" ID="lblErrorORWarning" CssClass="Electrolux_light_bold Electrolux_Color">See errors and warnings only</asp:Label>
                                </td>
                    <td>
                         <label class="switch">
                             <asp:CheckBox runat="server" OnCheckedChanged="handleRefreshEvent" AutoPostBack="true" ID="chkBoxErrorORWarning" />
                                        <span class="slider round"></span>
                                    </label>
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
                        <telerik:AjaxSetting AjaxControlID="RadDateTimePickerFrom">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                         <telerik:AjaxSetting AjaxControlID="chkBoxErrorORWarning">
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
                <telerik:RadGrid runat="server" ID="gridSearch" ShowGroupPanel="true" AutoGenerateColumns="true" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true"  ClientSettings-DataBinding-EnableCaching="true"
                    PageSize="20" OnNeedDataSource="gridSearch_NeedDataSource" >
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto" HeaderStyle-Height="30" >
                        <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced" />
                        <Columns>
                           <telerik:GridTemplateColumn DataField="CountryISOCode"  Groupable="true" UniqueName="CountryName" HeaderText="Country" >
                                <ItemTemplate>
                                    <img src='Images/Flags/<%# Eval("CountryISOCode").ToString().TrimEnd() %>.png' width="20" height="16" title="<%# Eval("CountryISOCode") %>" />&nbsp;
                                <span class="verticalAlignTop"><%# Eval("CountryISOCode") %></span>
                                </ItemTemplate>
                               <HeaderStyle Width="100px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn   Aggregate="None" DataField="XMLGenerationStartDate" HeaderText="Catalog Generation" HeaderTooltip="Product information is retrieved from EDEN and TP3 Catalog is built">
                                  <ItemTemplate>
                                      <img src='Images/<%# Eval("XMLGenerationStatus") %>.png' width="18" height="18" class="<%# IIf(Eval("XMLGenerationStatus").ToString().Length > 1, "FloatLeft", "DisplayNone")%>" style="margin-right:5px" title="<%# Eval("XMLGenerationStatus") %>" />
                                      <div>
                                          <%# Eval("XMLGenerationStartDate") %>
                                      </div>
                                           </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn   Aggregate="None" DataField="SentDate" HeaderText="Catalog Upload" HeaderTooltip="Catalog is uploaded to Prodanet Platform ">
                            <ItemTemplate>
                                <img src='Images/<%# Eval("SentStatus") %>.png' width="18" height="18"  class="<%# IIf(Eval("SentStatus").ToString().Length > 1, "FloatLeft", "DisplayNone")%>" style="margin-right:5px" title="<%# Eval("SentStatus") %>" />
                                      <div>
                                          <%# Eval("SentDate") %>
                                      </div>
                            </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn  Aggregate="None" DataField="ReceivedDate" HeaderText="Catalog Integration" HeaderTooltip="Integration report has been sent by Prodanet Platform ">
                                <ItemTemplate>
                                      <img src='Images/<%# Eval("ReceivedStatus") %>.png' width="18" height="18"  class="<%# IIf(Eval("ReceivedStatus").ToString().Length > 1, "FloatLeft", "DisplayNone")%>" style="margin-right:5px" title="<%# Eval("ReceivedStatus") %>"  />
                                      <div>
                                          <%# Eval("ReceivedDate") %>
                                      </div>
                                </ItemTemplate>             
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </asp:Content>
