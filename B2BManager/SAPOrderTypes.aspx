<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SAPOrderTypes.aspx.vb" Inherits="SAPOrderTypes" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }
    </script>
    <asp:PlaceHolder runat="server" ID="placeHolderSAPOrderManagement">
        <script type="text/javascript">
            function Finish(Action) {
                CloseWindow();
                if (Action == 'Save') {
                    $.magnificPopup.open({
                        items: {
                            src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> SAP order type has been added with success, use the button to create a new one or just press on x to close this window and refresh the main list.</p>'
                                + '</br> <button type="button" class="btn blue btn-submit" onclick="DisplaySAPOrderType()">New SAP order type</button></div> ',
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
                        src: '<div class="white-popup"></br><p class="text-justify">This SAP order type has been alerady assigned, please use the button to access the existing record</p>'
                            + '</br> <button type="button" class="btn blue btn-submit" onclick="$.magnificPopup.close();DisplaySAPOrderType(\'' + id + '\')">Edit existing order type</button></div> ',
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            $.magnificPopup.close();
                        }
                    }
                });
            }


            function DisplaySAPOrderType(id) {
                var oWnd = $find("<%= WindowSAPOrderType.ClientID %>");
            oWnd.setUrl("SAPOrderTypeManagement.aspx?HideHeader=true&envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + (id ? "&id=" + id : ""));
                oWnd.set_title('Loading...');
                oWnd.show();
            }

            function CloseWindow() {
                var oWnd = $find("<%= WindowSAPOrderType.ClientID %>");
                oWnd.close();
            }

            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }

            function DeleteSAPOrderType(id, ordertype, salesdoctype) {

                $("#dialog-delete-text").html("Are you sure you want to delete the order type <b>" + ordertype + "</b> with sales doc type <b><i>" + salesdoctype + "</i></b>?")
                $("#dialog-error-info").text("");
                $("#btnConfirmDelete").unbind("click");
                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                $("#btnConfirmDelete").on("click", function (event) {
                    $("#btnConfirmDelete").addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteSAPOrderType',
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
                    title: "Delete SAP order type"
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
                        <asp:Label runat="server" ID="lblActionDetails" CssClass="Electrolux_light_bold Electrolux_Color">Free text search:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtBoxSearchInDetails" CssClass="Electrolux_light_bold Electrolux_Color width180px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="ProcessButton(this,'Searching...')" OnClick="handleRefreshEvent"><i class="fas fa-search"></i> Search</asp:LinkButton>
                        <td>
                            <td>
                                <span runat="server" id="newSAPOrderType"><a class="btn lightblue" onclick="DisplaySAPOrderType()">New SAP order type</a></span>
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
                            <telerik:GridTemplateColumn UniqueName="Actions" HeaderText="Actions" Groupable="false" Reorderable="false" AllowFiltering="false" HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <table border="0">
                                        <tr style="border: none">
                                            <td style="border: none" runat="server" id="tdEdit">
                                                <img src='Images/Edit.png' onclick="DisplaySAPOrderType('<%# Eval("ID").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Edit SAP Order Type" /></td>
                                            <td style="border: none" runat="server" id="tdDelete">
                                                <img src='Images/Delete.png' onclick="DeleteSAPOrderType('<%# Eval("ID").ToString() %>','<%# Eval("B2B_Order_Type").ToString() %>','<%# Eval("SAP_Sales_Doc_Type").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Delete SAP Order Type" /></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="B2B_Order_Type" HeaderText="B2B Order Type">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="SAP_Sales_Doc_Type" HeaderText="SAP Sales Doc Type">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="Order_Reason" HeaderText="Order Reason">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="Shipping_Condition" HeaderText="Shipping Condition">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowSAPOrderType" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="540px" Height="250px" runat="server">
    </telerik:RadWindow>
</asp:Content>

