<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" MasterPageFile="~/MasterPage.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/InsightsChart.ascx" TagPrefix="uc1" TagName="InsightsChart" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="notification-container hidden" id="NotificationMsg">
        <div class="notification notification-info outputmsg outputmsg_info outputmsg_has_text">
            <button onclick="HideMsg()" class="btn-icon close icon-cross">
                <img src="Images/Close.png" /></button>
            <span class="outputmsg_text Electrolux_light_bold ng-binding">Your query took more than excepected, please refine your search. Execution time to retrive your data: </span><span id="ExcutionTime"></span>
        </div>
    </div>
    <uc1:InsightsChart runat="server" ID="InsightsChart" />
    <asp:HiddenField ID="uniqueGeneratedKey" runat="server" />
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <asp:Label runat="server" ID="infoLabel" CssClass="padding20px" Visible="false" ForeColor="Red">It seems that you are not authorized to access the logs of any application, please contact your administrator to check your access.</asp:Label>
            <table class="Filters" runat="server" id="filtersTable" style="width: 100%">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblApplicationName" CssClass="Electrolux_light_bold Electrolux_Color width130px">Application name:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlApplicationName_SelectedIndexChanged" AppendDataBoundItems="true" AutoPostBack="true" ID="ddlApplicationName">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlEnvironment">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="false" ID="ddlCountry">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblAction" CssClass="Electrolux_light_bold Electrolux_Color">Action:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlAction">
                            <asp:ListItem Text="All" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblError" CssClass="Electrolux_light_bold Electrolux_Color">Actions display:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlActionToDisplay">
                            <asp:ListItem Text="All" Value="0"></asp:ListItem>
                            <asp:ListItem Text="With errors only" Value="1"></asp:ListItem>
                            <asp:ListItem Text="With no errors" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblRowsCount" CssClass="Electrolux_light_bold Electrolux_Color">Max Rows Count:</asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox runat="server" MinValue="1" Value="20" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="txtRowsCount" CssClass="Electrolux_light_bold Electrolux_Color">
                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
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
                        <asp:Label runat="server" ID="lblActionDetails" CssClass="Electrolux_light_bold Electrolux_Color">Free text search:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxSearchInDetails" CssClass="Electrolux_light_bold Electrolux_Color width180px"></asp:TextBox>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="BeginSearch()" OnClick="btnSearch_Click" ><i class="fas fa-search"></i> Search</asp:LinkButton></td>
                                <td>
                                    <asp:LinkButton runat="server" ID="btnExport" CssClass="btn green" OnClick="btnExport_Click" ><i class="far fa-file-excel"></i> Export</asp:LinkButton></td>
                                <td>
                                    <a class="btn lightblue" runat="server" id="btnCreateChart" onclick="CreateChart()"><i class="fas fa-chart-line"></i> New Insights Chart</a>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="float: right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblAutoRefresh" CssClass="Electrolux_light_bold Electrolux_Color"><%= GetDefaultAutoSearchLabel() %></asp:Label>
                                </td>
                                <td>
                                    <label class="switch">
                                        <input runat="server" id="chkBoxAutoRefresh" type="checkbox" />
                                        <span class="slider round"></span>
                                    </label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblAutoSearch" CssClass="Electrolux_light_bold Electrolux_Color">Auto Search:</asp:Label>
                                </td>
                                <td>
                                    <label class="switch">
                                        <asp:CheckBox runat="server" OnCheckedChanged="chkBoxAutoSearch_CheckedChanged" AutoPostBack="true" ID="chkBoxAutoSearch" />
                                        <span class="slider round"></span>
                                    </label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                <ClientEvents OnRequestStart="RequestStart" />
                <ClientEvents OnResponseEnd="ResponseEnd" />
            </telerik:RadAjaxManager>
            <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" ClientIDMode="Static" IsSticky="true" Transparency="10" runat="server" Style="position: absolute;z-index: 999;">
                <asp:Image ID="Image1" runat="server" AlternateText="Loading..." ImageUrl="Images/Loading.gif" />
            </telerik:RadAjaxLoadingPanel>
            <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
                runat="server" RelativeTo="Element" Position="MiddleRight">
            </telerik:RadToolTipManager>
            <div style="position:relative">
            <span style="position:absolute;right:15px;top:5px"><span class="information-label" runat="server" id="lblInformation"></span></span>
            <telerik:RadGrid runat="server" ID="gridSearch" ShowGroupPanel="true" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true"  ClientSettings-DataBinding-EnableCaching="true"
                OnNeedDataSource="gridSearch_NeedDataSource" OnItemDataBound="gridSearch_ItemDataBound" PageSize="20" GroupingEnabled="true">
                <ClientSettings AllowDragToGroup="true" />
                <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                     <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced"/>                                  
                    <Columns>
                        <telerik:GridBoundColumn DataField="ActionStatus" UniqueName="ActionStatus" HeaderText="Status" Visible="false"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="HasError" Groupable="true" Reorderable="true" GroupByExpression="ActionStatus Status Group By ActionStatus" AllowFiltering="false" UniqueName="HasError" HeaderText="" HeaderStyle-Width="40">
                            <ItemTemplate>
                                <img src='Images/<%# Eval("ActionStatus").ToString()%>.png' class="<%# IIf(Eval("ActionStatus").ToString().Equals("Error"), "MoreInfoImg", "")%>" id="StatusImgTooltip_<%# Eval("TootlipID").ToString()%>" width="18" height="18" title="<%# Eval("ActionStatus") %>" />
                                <div class="hidden" id="StatusTooltipContent_<%# Eval("TootlipID").ToString()%>">
                                    <b>Error details:</b><br />
                                    Message: <%# Eval("ErrorMessage").ToString()%>
                                    <%# IIf(Eval("ErrorStackTrace").ToString().Length > 1, "<br />Stack Trace: " + Eval("ErrorStackTrace").ToString(), "")%>
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ViewXMLWithCorrelID" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                            <ItemTemplate>
                                <img src='Images/XML.png' width="20" class="<%# IIf(Eval("HasJson").ToString().ToLower() = "0", "MoreInfoImg", "hidden")%>" height="20" title="View XML Files"
                                    onclick="OpenViewXMLFilesWindow(<%# "'" + Eval("ApplicationID").ToString() + "','" + Eval("ActionName").ToString().Replace("<yellow>", "").Replace("</yellow>", "") + "','" + Eval("EnvironmentID").ToString() + "','" + Eval("CORREL_ID").ToString().Replace("<yellow>", "").Replace("</yellow>", "") + "','" + Eval("TABLE_NAME") + "','" + Eval("SOP_ID") + "','" + Eval("UserID").ToString() + "','" + Eval("ID").ToString() + "'" %>)" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ViewJsonWithCorrelID" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                            <ItemTemplate>
                                <img src='Images/Json.png' width="20" class="<%# IIf(Eval("HasJson").ToString().ToLower() = "true", "MoreInfoImg", "hidden")%>"  height="20" title="View XML Files"
                                    onclick="OpenViewJsonFileWindow(<%# "'" + Eval("ApplicationID").ToString() + "','" + Eval("ActionName").ToString().Replace("<yellow>", "").Replace("</yellow>", "") + "','" + Eval("EnvironmentID").ToString() + "','" + Eval("CORREL_ID").ToString().Replace("<yellow>", "").Replace("</yellow>", "") + "','" + Eval("TABLE_NAME") + "','" + Eval("SOP_ID") + "','" + Eval("UserID").ToString() + "','" + Eval("ID").ToString() + "'" %>)" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ViewXML" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                            <ItemTemplate>
                                <img src='Images/XML.png' width="20" class="<%# IIf(Eval("HasXML").ToString().ToLower() = "true", "MoreInfoImg", "hidden")%>" height="20" title="View XML File"
                                    onclick="OpenViewXMLFileWindow(<%# Eval("ID").ToString()  %>)" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="ViewJson" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                            <ItemTemplate>
                                <img src='Images/Json.png' width="20" class="<%# IIf(Eval("HasXML").ToString().ToLower() = "False", "MoreInfoImg", "hidden")%>" height="20" title="View JSON File"
                                    onclick="OpenViewJsonFileWindow(<%# Eval("ID").ToString() %>)" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" Groupable="false" Reorderable="false" AllowFiltering="true" Aggregate="None" DataField="ID" HeaderText="ID" HeaderStyle-Width="60">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="HubspanId" Groupable="false" Reorderable="false" UniqueName="HubspanId" HeaderText="Hubspan ID" Visible="false"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="SOP_ID" GroupByExpression="SOP_ID Group By SOP_ID" AllowFiltering="true" Groupable="true" UniqueName="SOP_ID" HeaderText="SOP" HeaderStyle-Width="60">
                            <ItemTemplate>
                                <img src='Images/Flags/<%# Eval("CY_NAME_ISOCODE").ToString() %>.png' width="20" height="16" title="<%# Eval("CY_NAME") %>" />
                                <span class="verticalAlignTop"><%# Eval("SOP_ID") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Aggregate="None" UniqueName="CORREL_ID" DataField="CORREL_ID" Groupable="false" Reorderable="false" HeaderText="CORREL ID" FilterControlWidth="280" AllowFiltering="true">
                            <ItemTemplate>
                                <span class="<%# IIf(Eval("HasError").ToString().ToLower() = "false", IIf(Eval("ActionStatus").ToString().Equals("PENDING"), "fontbleu", IIf(Eval("ActionStatus").ToString().Equals("WARNING"), "fontyellow", "fontgreen")), IIf(Eval("HasError").ToString().ToLower() = "true", "fontred", ""))%>"><%# Eval("CORREL_ID")%></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" Groupable="true" Reorderable="true" DataField="CustomerCode" HeaderText="Customer Code"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="U_ID" UniqueName="U_ID" HeaderText="User ID" Visible="false"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn DataField="U_Login" UniqueName="U_Login" Groupable="true" GroupByExpression="U_Login Group By U_Login" HeaderText="Login">
                            <ItemTemplate>
                                <table border="0" style="border: none;line-height: 18px;" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td style="border: none; vertical-align: top;">
                                            <img src='Images/Info.png' class="<%# IIf(Eval("UserID").ToString().Length > 1 And Not Eval("UserID").ToString().StartsWith("00000000-0000-0000-0000-000000000000"), "MoreInfoImg", "hidden")%>" id="UserImgTooltip_<%# Eval("TootlipID").ToString()%>" width="18" height="18" alt="More details" title="More details" />
                                        </td>
                                        <td style="border: none; vertical-align: top;line-height: 18px;">
                                            <span <%# IIf(Eval("DisplayUserProfile") > 0 And Eval("UserID").ToString().Length > 1 And Not Eval("UserID").ToString().StartsWith("00000000-0000-0000-0000-000000000000"),
                                                                                                                                                                                                                                    "class=""verticalAlignTop defaultLink linkColor"" " + IIf(Eval("DisplayUserProfile") = 1,
                                                                                                                                                                                                                                    "onclick=""popup('EbusinessUserProfile.aspx?uid=" + Eval("UserID").ToString() + "&envid=" + Eval("MatchingEnvironmentID").ToString() + "')""",
                                                                                                                                                                                                                                    "onclick=""popup('UserProfile.aspx?Uid=" + Eval("UserID").ToString() + "')"""), "class=""verticalAlignTop""") %>><%# Eval("U_Login") %></span>
                                        </td>
                                        <td style="border: none; vertical-align: top">
                                            <div class="hidden" id="UserTooltipContent_<%# Eval("TootlipID").ToString()%>">
                                                <b>User details:</b><br />
                                                ID: <%# Eval("U_ID").ToString()%><br />
                                                Login: <%# Eval("U_LOGIN").ToString()%><br />
                                                First name: <%# Eval("U_FIRSTNAME").ToString()%><br />
                                                Last name: <%# Eval("U_LASTNAME").ToString()%><br />
                                                Full name: <%# Eval("U_FULLNAME").ToString()%><br />
                                                Email: <%# Eval("U_EMAIL").ToString()%><br />
                                                U_GlobalID: <%# Eval("UserID").ToString()%><br />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="MachineName" AllowFiltering="true"  Groupable="true" Reorderable="true" GroupByExpression="MachineName Group By MachineName" UniqueName="MachineName" HeaderText="Originator">
                            <ItemTemplate>
                                <table border="0" style="border: none;line-height: 18px;" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td style="border: none; vertical-align: top">
                                            <img src='Images/Info.png' class="<%# IIf(Eval("MachineDetails").ToString().Length > 1, "MoreInfoImg", "hidden")%>" id="MachineImgTooltip_<%# Eval("TootlipID").ToString()%>" width="18" height="18" alt="More details" title="More details" />
                                        </td>
                                        <td style="border: none; vertical-align: top;line-height: 18px;">
                                            <span class="verticalAlignTop"><%# Eval("MachineName")%></span>
                                        </td>
                                        <td style="border: none; vertical-align: top">
                                            <div class="hidden" id="MachineTooltipContent_<%# Eval("TootlipID").ToString()%>">
                                                <b>Machine details:</b><br />
                                                <%# Eval("MachineDetails").ToString()%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="MachineDetails" Groupable="false" Reorderable="false" UniqueName="MachineDetails" HeaderText="MachineDetails" ItemStyle-Wrap="false" Visible="false"></telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn AllowSorting="true" AllowFiltering="false" Aggregate="None" DataField="Environment" ItemStyle-Wrap="false" HeaderText="Environment"></telerik:GridBoundColumn>--%>
                        <telerik:GridTemplateColumn DataField="ActionName" Groupable="true" GroupByExpression="ActionName Group By ActionName" AllowFiltering="true" UniqueName="ActionName" FilterControlWidth="110" HeaderText="Action Name">
                            <ItemTemplate>
                                <table border="0" style="border: none;line-height: 18px;" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td style="border: none; vertical-align: top;line-height: 18px;">
                                            <img src='Images/Info.png' class="<%# IIf(Eval("ActionDetails").ToString().Length > 1, "MoreInfoImg", "hidden")%>" id="ActionImgTooltip_<%# Eval("TootlipID").ToString()%>" width="18" height="18" alt="More details" title="More details" />
                                        </td>
                                        <td style="border: none; vertical-align: top;line-height: 18px;">
                                            <span class="verticalAlignTop"><%# Eval("ActionName")%></span>
                                        </td>
                                        <td style="border: none; vertical-align: top">
                                            <div class="hidden" id="ActionTooltipContent_<%# Eval("TootlipID").ToString() %>">
                                                <b>Actions details:</b><br />
                                                <%# Eval("ActionDetails").ToString()%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ActionDetailsForExport" Groupable="false" Reorderable="false" UniqueName="ActionDetails" HeaderText="ActionDetails" Visible="false"></telerik:GridBoundColumn>
                        <%--<telerik:GridBoundColumn AllowSorting="true" AllowFiltering="false" Aggregate="None" DataField="ApplicationName" HeaderText="Application Name" ItemStyle-Wrap="false"></telerik:GridBoundColumn>--%>
                        <telerik:GridBoundColumn DataField="POID" UniqueName="POID" Groupable="false" Reorderable="false" HeaderText="POID" Visible="false" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="SALESORDERID" Groupable="false" Reorderable="false" UniqueName="SALESORDERID" HeaderText="Sales Order ID" Visible="false" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" Groupable="false" Reorderable="false"  AllowFiltering="true" Aggregate="None" DataField="LoggedOn" DataType="System.DateTime" ItemStyle-Wrap="false" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderText="Logged On">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowSorting="true" Groupable="false" Reorderable="false" AllowFiltering="true" Visible="false" Aggregate="None" DataField="ReceivedDate" ItemStyle-Wrap="false" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderText="Received On">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn AllowFiltering="true" Groupable="false" Reorderable="false" Aggregate="None" UniqueName="ElapsedTime" DataField="ElapsedTime" SortExpression="ElapsedTime" HeaderText="Execution Time (ms)">
                            <ItemTemplate>
                                <span class="<%# ClsHelper.GetElapsedTimeFont(Eval("ElapsedTime"))%>"><%# Eval("ElapsedTime")%></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
                </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExport" />
            <asp:AsyncPostBackTrigger ControlID="ddlApplicationName" />
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
            <asp:AsyncPostBackTrigger ControlID="ddlAction" />
            <asp:AsyncPostBackTrigger ControlID="ddlActionToDisplay" />
            <asp:AsyncPostBackTrigger ControlID="txtRowsCount" />
            <asp:AsyncPostBackTrigger ControlID="RadDateTimePickerFrom" />
            <asp:AsyncPostBackTrigger ControlID="RadDateTimePickerTo" />
            <asp:AsyncPostBackTrigger ControlID="txtBoxSearchInDetails" />
            <asp:AsyncPostBackTrigger ControlID="chkBoxAutoSearch" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>






