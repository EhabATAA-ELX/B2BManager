<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TP2MailingLists.aspx.vb" Inherits="TP2MailingLists" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }

        function ResponseEndSimple(sender, eventArgs) {
        }

        function Finish(Action) {
            CloseWindow();
            if (Action == 'Save') {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Email setting has been added with success, use the button to create a new one or just press on x to close this window and refresh the main list.</p>'
                            + '</br> <button type="button" class="btn blue btn-submit" onclick="DisplayMailingList()">New email setting</button></div> ',
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            __doPostBack("<%= btnSearch.UniqueID %>", "");
                            $.magnificPopup.close();
                        }
                    }
                });
            }
            if (Action == 'Update') {
                __doPostBack("<%= btnSearch.UniqueID %>", "");
            }
        }

        function ConfirmExistingMapping(id) {
            CloseWindow();
            $.magnificPopup.open({
                items: {
                    src: '<div class="white-popup"></br><p class="text-justify">This email has been alerady assigned, please use the button to access the existing setting</p>'
                        + '</br> <button type="button" class="btn blue btn-submit" onclick="$.magnificPopup.close();DisplayMailingList(\'' + id+'\')">Edit existing setting</button></div> ',
                    type: 'inline'
                },
                callbacks: {
                    close: function () {
                        $.magnificPopup.close();
                    }
                }
            });
        }


        function DisplayMailingList(id) {
            var oWnd = $find("<%= WindowMailingList.ClientID %>");
            oWnd.setUrl("TP2ManageMailing.aspx?HideHeader=true&envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&type=" + $('#<%= ddlListType.ClientID %>').val() + (id ? "&id=" + id : ""));
            oWnd.set_title('Loading...');
            oWnd.show();            
        }

        function CloseWindow() {
            var oWnd = $find("<%= WindowMailingList.ClientID %>");
            oWnd.close();
        }
    </script>

    <asp:PlaceHolder runat="server" ID="DeleteCustomerEmailScript">
        <script type="text/javascript">
            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }

            function DeleteCustomerEmail(id, email, tradeplaceid) {

                $("#dialog-delete-text").html("Are you sure you want to delete the setting for email <b>" + email + "</b> attached to tradeplace customer <b>" + tradeplaceid + "</b>?")
                $("#dialog-error-info").text("");
                $("#btnConfirmDelete").unbind("click");
                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                $("#btnConfirmDelete").on("click", function (event) {
                    $("#btnConfirmDelete").addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteTP2CustomerEmail',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { GlobalID: id, Envid: $('#<%= ddlEnvironment.ClientID %>').val() },
                    async: true,
                    success: function (response) {
                        if (response.toLowerCase() == "success") {
                            CloseDeleteConfirmationWindow();
                            __doPostBack("<%= btnSearch.UniqueID %>", "");
                        }
                        else {
                            $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                            $("#dialog-error-info").text(response);
                        }
                    },
                    error: function (e) {
                        $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                        $("#dialog-error-info").text(e.statusText);
                    }
                });
            });

                $("#dialog-confirm-delete").dialog({
                    resizable: false,
                    height: "auto",
                    width: 400,
                    modal: true,
                    title: "Delete customer email setting"
                });
            }
            
        </script>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="DeleteCountryEmailScript">
        <script type="text/javascript">
            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }

            function DeleteCountryEmail(id, email, country) {

                $("#dialog-delete-text").html("Are you sure you want to delete the setting for email <b>" + email + "</b> in <b><i>" + country + "</i></b>?")
                $("#dialog-error-info").text("");            
                $("#btnConfirmDelete").unbind("click");
                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                $("#btnConfirmDelete").on("click", function (event) {
                    $("#btnConfirmDelete").addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteTP2CountryEmail',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { GlobalID: id, Envid: $('#<%= ddlEnvironment.ClientID %>').val() },
                    async: true,
                    success: function (response) {
                        if (response.toLowerCase() == "success") {
                            CloseDeleteConfirmationWindow();
                            __doPostBack("<%= btnSearch.UniqueID %>", "");
                        }
                        else {
                            $("#dialog-error-info").text(response);
                        }
                    },
                    error: function (e) {
                        $("#dialog-error-info").text(e.statusText);
                    }
                });
            });

                $("#dialog-confirm-delete").dialog({
                    resizable: false,
                    height: "auto",
                    width: 400,
                    modal: true,
                    title: "Delete country email setting"
                });
            }            
        </script>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel runat="server" ID="dialogDeletePanel">
        <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
            <div id="dialog-delete-text" style="margin: 15px"></div>
            <table align="right">
                <tr style="text-align: left">
                    <td colspan="2">
                        <span id="dialog-error-info" style="color: red; height: 20px">&nbsp</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn bleu" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                    </td>
                    <td>
                        <button class="btn red" id="btnConfirmDelete">Confirm</button>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
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
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="handleRefreshEvent" AutoPostBack="true" AppendDataBoundItems="false" ID="ddlCountry">
                        </telerik:RadComboBox>
                    </td>
                    <td runat="server" id="tdLevelLabel">
                        <asp:Label runat="server" ID="lblLevel" CssClass="Electrolux_light_bold Electrolux_Color">Level:</asp:Label>
                    </td>
                    <td class="width120px" runat="server" id="tdLevelDropDown">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width120px" ID="ddlListType" OnSelectedIndexChanged="handleRefreshEvent" AutoPostBack="true" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Text="Customer" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Country" Value="1"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblActionDetails" CssClass="Electrolux_light_bold Electrolux_Color">Free text search:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxSearchInDetails" CssClass="Electrolux_light_bold Electrolux_Color width180px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="ProcessButton(this,'Searching...')" OnClick="handleRefreshEvent"><i class="fas fa-search"></i> Search</asp:LinkButton>
                        <td>
                        <td>
                            <span runat="server" Visible="false" id="newCustomerEmailSetting"><a class="btn lightblue" onclick="DisplayMailingList()"><i class="fas fa-envelope"></i> New customer email</a></span>  
                            <span runat="server" Visible="false" id="newCountryEmailSetting"><a class="btn lightgreen" onclick="DisplayMailingList()"><i class="fas fa-envelope"></i> New country email</a></span>  
                        </td>
                </tr>
            </table>
            <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                <ClientEvents OnRequestStart="RequestStartSimple" />
                <ClientEvents OnResponseEnd="ResponseEndSimple" />
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
                    <telerik:AjaxSetting AjaxControlID="gridSearchCustomer">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="gridSearchCountry">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="btnSearch">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridContainer" LoadingPanelID="RadAjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="ddlListType">
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
                <telerik:RadGrid runat="server" ID="gridSearchCustomer" ShowGroupPanel="true" AutoGenerateColumns="true" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" ClientSettings-DataBinding-EnableCaching="true"
                    PageSize="20" OnColumnCreated="searchGridColumn_created" OnItemDataBound="gridSearchCustomer_ItemDataBound" OnNeedDataSource="gridSearch_NeedDataSource" GroupingEnabled="true">
                    <ClientSettings AllowDragToGroup="true" />
                    <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced" />
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Actions" HeaderText="Actions" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <table border="0">
                                        <tr style="border: none">
                                            <td style="border: none" runat="server" id="tdEdit">
                                                <img src='Images/Edit.png' onclick="DisplayMailingList('<%# Eval("ID").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Edit customer email setting" /></td>
                                            <td style="border: none" runat="server" id="tdDelete">
                                                <img src='Images/Delete.png' onclick="DeleteCustomerEmail('<%# Eval("ID").ToString() %>','<%# Eval("email").ToString() %>','<%# Eval("tradeplace_id").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Delete customer email setting" /></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <telerik:RadGrid runat="server" ID="gridSearchCountry" ShowGroupPanel="true" AutoGenerateColumns="true" CssClass="LogGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" ClientSettings-DataBinding-EnableCaching="true"
                    PageSize="20" OnColumnCreated="searchGridColumn_created" OnItemDataBound="gridSearchCountry_ItemDataBound" OnNeedDataSource="gridSearch_NeedDataSource" GroupingEnabled="true">
                    <ClientSettings AllowDragToGroup="true" />
                    <MasterTableView AutoGenerateColumns="true" TableLayout="Auto">
                        <PagerStyle AlwaysVisible="false" Mode="NextPrevNumericAndAdvanced" />
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Actions" HeaderText="Actions" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <table border="0">
                                        <tr style="border: none">
                                            <td style="border: none" runat="server" id="tdEdit">
                                                <img src='Images/Edit.png' onclick="DisplayMailingList('<%# Eval("ID").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Edit country email setting" /></td>
                                            <td style="border: none" runat="server" id="tdDelete">
                                                <img src='Images/Delete.png' onclick="DeleteCountryEmail('<%# Eval("ID").ToString() %>','<%# Eval("email").ToString() %>','<%# Eval("country").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Delete  country email setting" /></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="CY_NAME_ISOCODE" GroupByExpression="CY_NAME_ISOCODE Group By CY_NAME_ISOCODE" AllowFiltering="true" Groupable="true" UniqueName="CountryName" HeaderText="Country" HeaderStyle-Width="60">
                                <ItemTemplate>
                                    <img src='Images/Flags/<%# Eval("CY_NAME_ISOCODE").ToString() %>.png' width="20" height="16" title="<%# Eval("Country") %>" />&nbsp;
                                <span class="verticalAlignTop"><%# Eval("Country") %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowMailingList" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="520px" Height="370px" runat="server">
    </telerik:RadWindow>
</asp:Content>

