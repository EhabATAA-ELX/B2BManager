<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="Maintenance.aspx.vb" Inherits="Maintenance" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .largeTiles .store-apps-container {
            margin-left: 12px;
            margin-right: 14px;
        }

        hr {
            border-color: #e5e5e5 !important;
        }

        .instance-table td {
            padding-left: 10px;
        }

        .inmaintenance {
            font-weight: bold;
        }

        .unkown {
            color: #ff6a00;
        }

        .online {
            font-weight: bold;
        }

        .green-color, .online {
            color: #31af91;
        }

        .red-color, .inmaintenance {
            color: #c93636;
        }

        .storeapp-list a, .blueLink {
            color: #278efc !important;
        }

        .ui-widget-header, .ui-corner-all, .ui-widget.ui-widget-content {
            border: none;
        }

        .ui-widget-header {
            background-color: #e5f3f2;
        }

        .largeTiles .store-apps-title {
            font-size: 20px;
            display: block;
            margin-left: 12px;
            margin-top: 25px;
            padding: 15px;
            padding-bottom: 0px;
            background-color: #e5f3f2;
        }

            .largeTiles .store-apps-title span {
                font-size: 16px;
            }

        .largeTiles .storeapp-list {
            margin: 16px 0;
        }

        .largeTiles .storeapp-list {
            padding: 0;
            position: relative;
        }

        .storeapp {
            display: inline-block;
            min-height: 50px;
            min-width: 220px;
            height: auto;
            margin: 10px 0 15px 15px;
            padding: 0 5px 0 0;
            border-right: 1px solid #031d4e;
            -moz-box-sizing: border-box;
            box-sizing: border-box;
        }

        .storeapp {
            overflow: hidden;
            vertical-align: top;
        }

        .storeapp, .folder {
            position: relative;
        }

        .redFont {
            color: red;
        }

        .progress-bar {
            display: block;
            margin-left: 12px;
            height: 15px;
        }

        .MassActionSelect {
            font-size: 10pt;
        }
    </style>
    <script type="text/javascript">
        function OpenSAPWindow() {
            var oWnd = $find("<%= WindowSendToSAP.ClientID %>");
            var environmentID = $("#<%= ddlEnvironment.ClientID %>").val();
            var combo = $find("<%= ddlCountry.ClientID %>");
            var SopID = combo.get_selectedItem().get_value();
            var NbOrders = parseInt($("#ContentPlaceHolder1_LblNBOrdersToResend").text());
            oWnd.setUrl("TP2PendingOrdersResend.aspx?HideHeader=true&EnvironmentID=" + environmentID + "&SOPID=" + SopID + "&NBORDERS=" + NbOrders);
            oWnd.show();

        }
        function ShowAndRefreshGrid() {
            CloseWindow();
            ShowLoading();
            $("#<%= imageBtnRefresh.ClientID%>").click();
        }
        function CloseWindow() {
            var oWnd = $find("<%= WindowSendToSAP.ClientID %>");
            oWnd.close();
        }
        function ShowLoading() {
            $('#<%= htmlInfo.ClientID %>').html("<img src='Images/Loading.gif' /> Fetching realtime data...");
            $('#actionsTable').hide();
        }
        function imageBtnRefreshClick() {
            $.each($(".image-btn-refresh"), function (index, value) {
                $(value)[0].src = "Images/Loader.gif";
            });
        }

        function HideOnlineInstances() {
            $("li[data-status='on']").hide();
        }

        function ShowAll() {
            $("li[data-status='on']").show()
        }

        $(document).ready(function () {
            ShowLoading();
            $("#<%= imageBtnRefresh.ClientID%>").click();
        });

        function RefreshInstance(SOP_ID, el) {
            $(el).replaceWith("<img width='20px' height='20px' src='Images/Loading.gif' /> Loading")
            var data = {
                sopid: SOP_ID,
                EnvironmentID: $("#<%= ddlEnvironment.ClientID%>").val(),
                mode: $("#<%= ddlMaintenanceMode.ClientID%>").val()
            };

            $.ajax({
                type: 'POST',
                url: 'B2BManagerService.svc/GetMaintenanceInstance',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(data),
                async: true,
                success: function (response) {
                    $("li[data-sopid='" + SOP_ID + "']").replaceWith(response);
                },
                error: function (e) {
                    if (e.statusText == "Unauthorized") {
                        window.location = "Login.aspx";
                    }
                    console.log("Error  : " + e.statusText);
                }
            });
        }

        function ChangeInstancesStatus(SOP_ID, type, doStart, el, serverName) {
            if (serverName == null) {
                $(el).replaceWith("<img width='20px' height='20px' src='Images/Loading.gif' /> " + (doStart ? '<span class="gree-color">Starting...</span>' : '<span class="red-color">Stoping...</span>'));
            }
            else {
                $(el).replaceWith("<img width='30px' height='30px' src='Images/Loading.gif' />");
            };
            var data = {
                sopid: SOP_ID,
                EnvironmentID: $("#<%= ddlEnvironment.ClientID%>").val(),
                mode: $("#<%= ddlMaintenanceMode.ClientID%>").val(),
                type: type,
                startInstances: doStart,
                serverName: serverName
            };

            $.ajax({
                type: 'POST',
                url: 'B2BManagerService.svc/ChangeInstancesStatus',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify(data),
                async: true,
                success: function (response) {
                    $("li[data-sopid='" + SOP_ID + "']").replaceWith(response);
                },
                error: function (e) {
                    if (e.statusText == "Unauthorized") {
                        window.location = "Login.aspx";
                    }
                    console.log("Error  : " + e.statusText);
                }
            });

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <ClientEvents OnRequestStart="ShowLoading" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ddlEnvironment"></telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ddlCountry"></telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ddlMaintenanceMode"></telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleRight">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ImgTooltipHelp_Actions" />
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ImgTooltipHelp_Environments" />
        </TargetControls>
    </telerik:RadToolTipManager>
    <asp:UpdatePanel runat="server" ID="updatePanel1" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="Filters">
                <tr>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                        <img src='Images/Info.png' class='MoreInfoImg vertical-align-bottom' id='ImgTooltipHelp_Environments' width='24' height='24' alt='More details' />
                        <div class='hidden' style='margin: 25px; padding: 25px' id="TooltipContentHelp_Environments">
                            <ul style="list-style: none; margin: 5px; padding: 5px; line-height: 1.5;">
                                <li style="margin: 5px 0px"><b>Production:</b> corresponds to <i>Production</i> environment for <i style="color: #278efc">B2B (TP1)</i> and <i style="color: #ff6a00">Bili (TP2)</i> connected to <b>P</b> environment in SAP</li>
                                <li style="margin: 5px 0px"><b>Staging:</b> used as <i>Non Regression</i> environment for <i style="color: #278efc">B2B (TP1)</i> connected to <b>Q</b> environment in SAP</li>
                                <li style="margin: 5px 0px"><b>UAT:</b> used for <i>UAT</i> for <i style="color: #278efc">B2B (TP1)</i> and test environment for <i style="color: #ff6a00">Bili (TP2)</i>. Both connected to <b>R</b> environment in SAP</li>
                                <li style="margin: 5px 0px"><b>Test:</b> used for <i>SIT</i> for <i style="color: #278efc">B2B (TP1)</i>. It is also connected to <b>R</b> environment in SAP</li>
                                <li style="margin: 5px 0px"><b>Dev:</b> not applicable.</li>
                            </ul>
                        </div>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" DataTextField="Name" AutoPostBack="true" DataValueField="ID">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Maintenance for:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlMaintenanceMode" AutoPostBack="true">
                            <asp:ListItem Value="All" Selected="True" Text="All"></asp:ListItem>
                            <asp:ListItem Value="B2B" Text="B2B (TP1)"></asp:ListItem>
                            <asp:ListItem Value="Bili" Text="Bili (TP2)"></asp:ListItem>
                            <asp:ListItem Value="AEG" Text="Chiron (AEG)"></asp:ListItem>
                            <asp:ListItem Value="Electrolux" Text="Chiron (ELX)"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td style="width: 24px">
                        <asp:ImageButton runat="server" ClientIDMode="Static" ID="imageBtnRefresh" CssClass="image-btn-refresh" OnClientClick="imageBtnRefreshClick()" ImageUrl="Images/Reload.png" Width="24" Height="24" ToolTip="Refresh" />
                    </td>
                </tr>
            </table>
            <table class="Filters" id="actionsTable">
                <tr>
                    <td class="width120px">
                        <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Actions:</asp:Label>
                        <img src='Images/Info.png' class='MoreInfoImg vertical-align-bottom' id='ImgTooltipHelp_Actions' width='24' height='24' alt='More details' />
                        <div class='hidden' style='margin: 25px; padding: 25px' id="TooltipContentHelp_Actions">
                            <div style="margin: 5px; padding: 5px;">
                                <i><b>Maintenance statues legend:</b></i>
                                </br>
                            </br>
                            <img width="24" src="Images/Maintenance/checkmark.png">
                                Instance is running, click on the icon to put it maintenance.
                            <br />
                                <img width="24" src="Images/Maintenance/stopped.png">
                                Instance is under maintenance, click on the icon to put it online.
                            <br />
                                <img width="24" src="Images/Maintenance/warning.png">
                                The maintenance status is unkown, you need to manually check.
                            <br />
                                <img width="24" src="Images/Maintenance/Error.png">
                                An error occurred while capturing the maintenance status.
                            <br />
                                <br />
                                <i><b>Main actions summary:</b></i>
                                <br />
                                <br />
                                <b>Put B2B under maintenance</b>: put all available online B2B (TP1) instances in maintenance mode
                            <br />
                                <b>Put Bili under maintenance</b>: put all available online Bili (TP2) instances in maintenance mode
                                <br />
                                <b>Put AEG under maintenance</b>: put all available online Chiron (AEG) instances in maintenance mode
                                <br />
                                <b>Put Electrolux under maintenance</b>: put all available online Chiron (Electrolux) instances in maintenance mode
                            <br />
                                <b>Put B2B online</b>: put out B2B (TP1) instances from maintenance mode and bring them online
                            <br />
                                <b>Put Bili online</b>: put out Bili (TP2) instances from maintenance mode and bring them online
                            <br />
                                <b>Put AEG online</b>: put out Chiron (AEG) instances from maintenance mode and bring them online
                            <br />
                                <b>Put Electrolux online</b>: put out Chiron (Electrolux) instances from maintenance mode and bring them online
                            <br />
                                <b style="color: #31af91 !important"><i class="far fa-play-circle"></i>Start B2B/Bili</b>: put out B2B/Bili/AEG/Electrolux instances in the selected country from maintenance mode and bring them online
                            <br />
                                <b style="color: #c93636 !important"><i class="far fa-stop-circle"></i>Stop B2B/Bili</b>: put online B2B/Bili/AEG/Electrolux instances in the selected country in maintenance mode
                            </div>
                    </td>
                    <td runat="server" visible="false" id="tdB2BInMaintenance">
                        <asp:LinkButton CssClass="btn red" Text="Put B2B under maintenance" OnClick="TriggerChange_Click" CommandName="Stop" CommandArgument="B2B" runat="server" ID="btnPutB2BUnderMaintenance"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdB2BUp">
                        <asp:LinkButton CssClass="btn metrogreen" Text="Put B2B online" OnClick="TriggerChange_Click" CommandName="Start" CommandArgument="B2B" runat="server" ID="btnPutB2BOnline"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdBiliInMaintenance">
                        <asp:LinkButton CssClass="btn red" Text="Put Bili under maintenance" OnClick="TriggerChange_Click" CommandName="Stop" CommandArgument="Bili" runat="server" ID="btnPutBiliUnderMaintenance"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdBiliUp">
                        <asp:LinkButton CssClass="btn metrogreen" Text="Put Bili online" OnClick="TriggerChange_Click" CommandName="Start" CommandArgument="Bili" runat="server" ID="btnPutBiliOnline"></asp:LinkButton>
                    </td>
                    

                    <td>
                        <a class="blueLink" onclick="HideOnlineInstances()">Hide online instances</a>
                        | <a class="blueLink" onclick="ShowAll()">Show all</a>
                    </td>
                    <td runat="server" visible="false" id="tdBiliResendOrderMaintenance">
                        <asp:LinkButton CssClass="btn metrogreen" Text="Release Pending Orders" OnClientClick="OpenSAPWindow()" runat="server" ID="btnReleasePending"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdNBPendingOrdersToResend">
                        <asp:Label runat="server" ID="LblNBOrdersToResend"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="width120px"></td>
                    <td runat="server" visible="false" id="tdAEGInMaintenance">
                        <asp:LinkButton CssClass="btn red" Text="Put AEG under maintenance" OnClick="TriggerChange_Click" CommandName="Stop" CommandArgument="AEG" runat="server" ID="btnPutAEGUnderMaintenance"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdAEGUp">
                        <asp:LinkButton CssClass="btn metrogreen" Text="Put AEG online" OnClick="TriggerChange_Click" CommandName="Start" CommandArgument="AEG" runat="server" ID="btnPutAEGOnline"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdElectroluxInMaintenance">
                        <asp:LinkButton CssClass="btn red" Text="Put Electrolux under maintenance" OnClick="TriggerChange_Click" CommandName="Stop" CommandArgument="Electrolux" runat="server" ID="btnPutElectroluxUnderMaintenance"></asp:LinkButton>
                    </td>
                    <td runat="server" visible="false" id="tdElectroluxUp">
                        <asp:LinkButton CssClass="btn metrogreen" Text="Put Electrolux online" OnClick="TriggerChange_Click" CommandName="Start" CommandArgument="Electrolux" runat="server" ID="btnPutElectroluxOnline"></asp:LinkButton>
                    </td>
                </tr>
            </table>
            <div class="largeTiles store-view">
                <div class="store-apps-container" id="htmlInfo" runat="server">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
            <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
            <asp:AsyncPostBackTrigger ControlID="btnPutB2BUnderMaintenance" />
            <asp:AsyncPostBackTrigger ControlID="btnPutB2BOnline" />
            <asp:AsyncPostBackTrigger ControlID="btnPutBiliUnderMaintenance" />
            <asp:AsyncPostBackTrigger ControlID="btnPutBiliOnline" />
        </Triggers>
    </asp:UpdatePanel>
    <telerik:RadWindow ID="WindowSendToSAP" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Send Request to SAP" Behaviors="Close" Width="400" Height="150px">
    </telerik:RadWindow>
</asp:Content>

