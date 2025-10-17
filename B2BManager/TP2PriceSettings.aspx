<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TP2PriceSettings.aspx.vb" Inherits="TP2PriceSettings" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        function RequestStartSimple(sender, eventArgs) {
            centerElementOnScreen($get("RadAjaxLoadingPanel1"));
        }
    </script>
    <asp:PlaceHolder runat="server" ID="placeHolderManageScript">
        <script type="text/javascript">
            function Finish(Action) {
                CloseWindow();
                if (Action == 'Save') {
                    $.magnificPopup.open({
                        items: {
                            src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Price setting has been added with success, use the button to create a new one or just press on x to close this window and refresh the main list.</p>'
                                + '</br> <button type="button" class="btn blue btn-submit" onclick="DisplayPriceSetting()">New price setting</button></div> ',
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
                        src: '<div class="white-popup"></br><p class="text-justify">This price setting has been alerady assigned, please use the button to access the existing mapping</p>'
                            + '</br> <button type="button" class="btn blue btn-submit" onclick="$.magnificPopup.close();DisplayPriceSetting(\'' + id + '\')">Edit existing setting</button></div> ',
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            $.magnificPopup.close();
                        }
                    }
                });
            }


            function DisplayPriceSetting(id) {
                var oWnd = $find("<%= WindowPriceSetting.ClientID %>");
                  oWnd.setUrl("TP2ManagePriceSetting.aspx?HideHeader=true&envid=" + $('#<%= ddlEnvironment.ClientID %>').val() + (id ? "&id=" + id : ""));
                oWnd.set_title('Loading...');
                oWnd.show();
            }

            function CloseWindow() {
                var oWnd = $find("<%= WindowPriceSetting.ClientID %>");
                oWnd.close();
            }

            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }

            function DeletePriceSetting(id, salesorg, sapfield) {

                $("#dialog-delete-text").html("Are you sure you want to delete the price setting <b>" + sapfield + "</b> in sales org <b><i>" + salesorg + "</i></b>?")
                $("#dialog-error-info").text("");
                $("#btnConfirmDelete").unbind("click");
                $("#btnConfirmDelete").removeClass("loadingBackground").html("Delete").removeAttr('disabled');
                $("#btnConfirmDelete").on("click", function (event) {
                    $("#btnConfirmDelete").addClass("loadingBackground").html("Deleting..").prop('disabled', true);
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteTP2PriceSetting',
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
                    title: "Delete price setting"
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
                                <span runat="server" id="newPriceMapping"><a class="btn lightblue" onclick="DisplayPriceSetting()"><i class="fas fa-tag"></i>New price setting</a></span>
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
                                                <img src='Images/Edit.png' onclick="DisplayPriceSetting('<%# Eval("ID").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Edit price setting" /></td>
                                            <td style="border: none" runat="server" id="tdDelete">
                                                <img src='Images/Delete.png' onclick="DeletePriceSetting('<%# Eval("ID").ToString() %>','<%# Eval("SALES_ORG").ToString() %>','<%# Eval("SAP_FIELD").ToString() %>')" width="20" class="MoreInfoImg" height="20" title="Delete price setting" /></td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="SALES_ORG" HeaderText="Sales Org">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="SAP_FIELD" HeaderText="SAP Field">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="TP2_FIELD" HeaderText="TP2 Field">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" Aggregate="None" DataField="STEPNUMBER" HeaderText="Step Number">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn AllowSorting="true" Reorderable="false" AllowFiltering="true" Aggregate="None" DataField="CODE" HeaderText="Code">
                            </telerik:GridBoundColumn>
                            <telerik:GridCheckBoxColumn AllowSorting="true" Reorderable="false" AllowFiltering="true" DataField="MONETARY_AMOUNT" HeaderText="Monetary Amount">
                            </telerik:GridCheckBoxColumn>
                            <telerik:GridCheckBoxColumn AllowSorting="true" Reorderable="false" AllowFiltering="true" DataField="ALLOWANCES_CHARGES" HeaderText="Allowances & Charges">
                            </telerik:GridCheckBoxColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowPriceSetting" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="540px" Height="300px" runat="server">
    </telerik:RadWindow>
</asp:Content>

