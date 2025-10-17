<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessSuperUserCustomerList.aspx.vb" Inherits="EbusinessSuperUserCustomerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <asp:PlaceHolder runat="server" ID="ManagementScript" Visible="false">
        <script type="text/javascript">
            function changeSelectionStatus(row, type) {
                if ($(row).hasClass("selected-row")) {
                    $(row).removeClass("selected-row");
                    $(row).find(".checkedTickBox").addClass("hidden");
                }
                else {
                    $(row).addClass("selected-row");
                    $(row).find(".checkedTickBox").removeClass("hidden");
                }

                CalculateSelected(type);
            }

            function CalculateSelected(type) {
                var selectedCount = $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.selected-row").length;
                if (selectedCount > 0) {
                    $("#" + ((type == 0) ? "" : "un") + "assignedSelected").html(selectedCount.toString() + " selected");
                    $("#btn" + ((type == 1) ? "" : "Un") + "AssignSelection").removeClass("btn-disabled");
                    $("#btn" + ((type == 1) ? "" : "Un") + "AssignSelection").attr("onclick", "ProcessButton(this,'Executing...');ManageSelection(" + type.toString() + ",this)");
                }
                else {
                    $("#" + ((type == 0) ? "" : "un") + "assignedSelected").html("");
                    $("#btn" + ((type == 1) ? "" : "Un") + "AssignSelection").addClass("btn-disabled");
                    $("#btn" + ((type == 1) ? "" : "Un") + "AssignSelection").removeAttr("onclick");
                    $("." + ((type == 0) ? "" : "un") + "assignedDisplayDD").val("All");
                }
                DisplaySelection($("." + ((type == 0) ? "" : "un") + "assignedDisplayDD").val(), type);
            }

            function SelectRows(select, type) {
                if (select) {
                    $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.hidden)").addClass("selected-row");
                    $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.hidden)").find(".checkedTickBox").removeClass("hidden");
                }
                else {
                    $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.hidden)").removeClass("selected-row");
                    $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.hidden)").find(".checkedTickBox").addClass("hidden");
                }
                CalculateSelected(type);
            }
            $(document).ready(function () {
                $('#searchInCustomerListTextBox').on('paste', function () {
                    searchInCustomerList();
                });

                $('.assignedDisplayDD').on('change', function () {
                    DisplaySelection(this.value, 0);
                });

                $('.unassignedDisplayDD').on('change', function () {
                    DisplaySelection(this.value, 1);
                });
            });

            function DisplaySelection(selectionType, type) {
                switch (selectionType) {
                    case "Selected":
                        if ($(".filtred-search").length > 0) {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.filtred-search:not(.selected-row)").addClass("hidden").hide();
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.filtred-search.selected-row").removeClass("hidden").show();
                        }
                        else {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.selected-row)").addClass("hidden").hide();
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.selected-row").removeClass("hidden").show();
                        }
                        break;
                    case "Unselected":
                        if ($(".filtred-search").length > 0) {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.filtred-search.selected-row").addClass("hidden").hide();
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.filtred-search:not(.selected-row)").removeClass("hidden").show();
                        }
                        else {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.selected-row").addClass("hidden").hide();
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row:not(.selected-row)").removeClass("hidden").show();
                        }
                        break;
                    default:
                        if ($(".filtred-search").length > 0) {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row.filtred-search").removeClass("hidden").show();
                        }
                        else {
                            $(".list-" + ((type == 0) ? "" : "un") + "assigned-row").removeClass("hidden").show();
                        }
                        break;
                }
            }

            function ConfirmAssignment() {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer list updated with success.</p></div>',
                        type: 'inline'
                    }
                });
            }

            function ManageSelection(type, btn) {
                var values = $.map($(".list-" + ((type == 0) ? "" : "un") + "assigned-row.selected-row"), function (el) {
                    return $(el).attr("data-cid");
                });
                var data = { Type: type, Values: values <%= GetSuperUserInfo() %>};

                $.ajax({
                    type: 'POST',
                    url: 'B2BManagerService.svc/ManageSuperUserSelection',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify(data),
                    proccessData: false,
                    async: true,
                    success: function (response) {
                        if (response.toLowerCase() == "success") {
                            $(btn).html('Refreshing lists...');
                            __doPostBack('<%= RefreshHD.ClientID %>', 'ConfirmAssignment');
                        }
                        else {
                            $(btn).html('<i class="' + $(btn).attr("data-button-icon") + '"></i> ' + $(btn).attr("data-button-text"));
                            ErrorPopup(response);
                        }
                    },
                    error: function (e) {
                        $(btn).html('<i class="' + $(btn).attr("data-button-icon") + '"></i> ' + $(btn).attr("data-button-text"));
                        ErrorPopup("Error  : " + e.statusText);
                    }
                });
            }
        </script>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="BasicAccessScript" Visible="false">
        <script type="text/javascript">
            function changeSelectionStatus(row, type) {
                return false;
            }

            function SelectRows(select, type) {
                return false;
            }
            $(document).ready(function () {
                $('#searchInCustomerListTextBox').on('paste', function () {
                    searchInCustomerList();
                });
            });

            function DisplaySelection(selectionType, type) {
                return false;
            }
        </script>
    </asp:PlaceHolder>
    <script type="text/javascript">

        function searchInCustomerList() {
            var textSearch = $("#searchInCustomerListTextBox")[0].value.toString().toLowerCase();

            $("tr.list-row").removeClass("hidden").removeClass("filtred-search").addClass("display-inline");
            if (textSearch != "") {
                $("tr.list-row").addClass("hidden").removeClass("display-inline");
                $("tr.list-row[data-search*='" + textSearch + "']").removeClass("hidden").addClass("filtred-search");
            }

            if (textSearch.length == 0) {
                DisplaySelection($(".assignedDisplayDD").val(), 0);
                DisplaySelection($(".unassignedDisplayDD").val(), 1);
            }
        }
    </script>
    <style type="text/css">
        .odd-row, .even-row {
            cursor: pointer;
        }

        .display-inline {
            display: inline-block;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel runat="server" ID="superUserInformationPanel" Visible="false">
        <table style="margin: 5px" cellpadding="5" cellspacing="5">
            <tr valign="top">
                <td style="width: 130px;">
                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login name:</asp:Label>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblLoginName" CssClass="Electrolux_Color textHighlithed"></asp:Label>
                </td>
            </tr>
            <tr valign="top" runat="server" id="displayNameTR">
                <td>
                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Display Name:</asp:Label></td>
                <td>
                    <asp:Label runat="server" ID="lblDisplayName" CssClass="Electrolux_Color"></asp:Label></td>
            </tr>
            <tr valign="top" runat="server" id="emailTR">
                <td>
                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblEmail" CssClass="Electrolux_Color"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="RefreshHD" />
            <table style="width: 99%; margin-right: 15px; margin-left: 15px; padding-right: 5px; padding-left: 5px">
                <tr>
                    <td valign="top" style="width: 45%">
                        <div class="card card-primary">
                            <div class="card-header">
                                Assigned customers <span runat="server" class="Electrolux_Color" id="assignedCount"></span><span id="assignedSelected" style="float: right; color: red; padding-right: 5px;"></span>
                            </div>
                            <div class="card-body">
                                <table runat="server" style="width: 100%">
                                    <tr>
                                        <th runat="server" id="ManageAssignedTH" colspan="2">
                                            <a class="defaultLink linkColor" onclick="SelectRows(true,0)">Select all</a> | <a class="defaultLink linkColor" onclick="SelectRows(false,0)">Unselect all </a>| Display
                                            <select class="assignedDisplayDD">
                                                <option value="All">All</option>
                                                <option value="Selected">Selected</option>
                                                <option value="Unselected">Unselected</option>
                                            </select>
                                        </th>
                                    </tr>
                                    <tr runat="server">
                                        <th runat="server" style="display: inline-block; width: 65% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Name</th>

                                        <th runat="server" style="display: inline-block; width: 32% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Code</th>
                                    </tr>
                                </table>
                                <asp:ListView runat="server" ID="ListView1">
                                    <LayoutTemplate>
                                        <div style="height: 460px !important; overflow-y: auto;">
                                            <table runat="server" id="table1" class="list-view" style="width: 100%">
                                                <tr runat="server" id="itemPlaceholder" style="width: 100%; display: inline-block;"></tr>
                                            </table>
                                        </div>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr runat="server" class="odd-row list-row list-assigned-row display-inline caro"
                                            data-search='<%#Eval("C_Name").ToString().ToLower() + "#" + Eval("C_CUID").ToString().ToLower() + "#" + Eval("C_DESCRIPTION").ToString().ToLower() + "#" + Eval("CH_CUID_5").ToString().ToLower() + "#" + Eval("CH_NAME_5").ToString().ToLower() + "#" + Eval("CH_CUID_4").ToString().ToLower() + "#" + Eval("CH_NAME_4").ToString().ToLower() + "#" + Eval("CH_CUID_3").ToString().ToLower() + "#" + Eval("CH_NAME_3").ToString().ToLower() + "#" + Eval("PLC").ToString().ToLower() + "#" + Eval("PLC_Description").ToString().ToLower() + "#" + Eval("C_GRP4").ToString().ToLower() %>'
                                            data-cid='<%# Eval("C_GLOBALID") %>' onclick="changeSelectionStatus(this,0)"
                                            style="width: 100%; padding-top:8px; padding-bottom:5px;">
                                            <td style="display: inline-block; width: 24px; vertical-align: top">
                                                <span class="checkedTickBox hidden" style="padding: 5px;"><i class="Electrolux_Color fas fa-check"></i></span>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(65% - 24px);">
                                                <b>
                                                    <asp:Label ID="NameLabel1" CssClass="search-text" runat="server" Height="12"
                                                        Text='<%#Eval("C_Name") %>' />
                                                </b>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                <b>
                                                    <asp:Label ID="Label1" CssClass="search-cuid" runat="server" Height="12"
                                                        Text='<%#Eval("C_CUID") %>' /></b>
                                            </td>

                                            <td runat="server" visible='<%#Eval("PLC").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                PLC: 
                                                <asp:Label ID="Label9" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("PLC") %>' />
                                                --
                                                <asp:Label ID="Label10" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("PLC_description") %>' />
                                            </td>

                                            <td runat="server"  visible='<%#Eval("CH_CUID_3").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label7" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_3") %>' />
                                                --
                                                <asp:Label ID="Label8" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_3") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label5" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_4") %>' />
                                                --
                                                <asp:Label ID="Label6" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_4") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_5").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label2" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_5") %>' />
                                                --
                                                <asp:Label ID="Label4" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_5") %>' />
                                            </td>
                                            <td runat="server" visible='<%#Eval("C_GRP4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%; font-size: 10px;">
                                                <span class="" style="width: 24px; display: inline-block;"></span>
                                                Cust.Group4: 
                                                <asp:Label ID="Label11" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("C_GRP4") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr runat="server" class="even-row list-row list-assigned-row display-inline caro"
                                            data-search='<%#Eval("C_Name").ToString().ToLower() + "#" + Eval("C_CUID").ToString().ToLower() + "#" + Eval("C_DESCRIPTION").ToString().ToLower() + "#" + Eval("CH_CUID_5").ToString().ToLower() + "#" + Eval("CH_NAME_5").ToString().ToLower() + "#" + Eval("CH_CUID_4").ToString().ToLower() + "#" + Eval("CH_NAME_4").ToString().ToLower() + "#" + Eval("CH_CUID_3").ToString().ToLower() + "#" + Eval("CH_NAME_3").ToString().ToLower() + "#" + Eval("PLC").ToString().ToLower() + "#" + Eval("PLC_Description").ToString().ToLower() + "#" + Eval("C_GRP4").ToString().ToLower() %>'
                                            data-cid='<%# Eval("C_GLOBALID") %>' onclick="changeSelectionStatus(this,0)"
                                            style="width: 100%; padding-top:8px; padding-bottom:5px;">
                                            <td style="display: inline-block; width: 24px; vertical-align: top">
                                                <span class="checkedTickBox hidden" style="padding: 5px;"><i class="Electrolux_Color fas fa-check"></i></span>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(65% - 24px);">
                                                <b>
                                                    <asp:Label ID="NameLabel1" CssClass="search-text" runat="server" Height="12"
                                                        Text='<%#Eval("C_Name") %>' /></b>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                <b>
                                                    <asp:Label ID="Label1" CssClass="search-cuid" runat="server" Height="12"
                                                        Text='<%#Eval("C_CUID") %>' /></b>
                                            </td>

                                            <td runat="server" visible='<%#Eval("PLC").ToString().Length > 0  %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                PLC: 
                                                <asp:Label ID="Label9" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("PLC") %>' />
                                                --
                                                <asp:Label ID="Label10" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("PLC_description") %>' />
                                            </td>

                                            <td runat="server"  visible='<%#Eval("CH_CUID_3").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label7" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_3") %>' />
                                                --
                                                <asp:Label ID="Label8" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_3") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label5" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_4") %>' />
                                                --
                                                <asp:Label ID="Label6" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_4") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_5").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label2" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_5") %>' />
                                                --
                                                <asp:Label ID="Label4" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_5") %>' />
                                            </td>
                                            
                                            <td runat="server" visible='<%#Eval("C_GRP4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%; font-size: 10px;">
                                                <span class="" style="width: 24px; display: inline-block;"></span>
                                                Cust.Group4: 
                                                <asp:Label ID="Label11" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("C_GRP4") %>' />
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </td>
                    <td style="width: 10%; padding-top: 70px; vertical-align: top">
                        <table align="center" cellspacing="10px">
                            <tr>
                                <td style="text-align: center">Free text search 
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input type="text" placeholder="Type in name or code or description" id="searchInCustomerListTextBox" class="width180px" style="height: 26px" onkeyup="searchInCustomerList()" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR1">
                                <td style="text-align: center">Customer assignement
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR2">
                                <td>
                                    <a id="btnAssignSelection" class="btn bleu width180px btn-disabled" data-button-text="Assign selected" data-button-icon="fas fa-arrow-alt-circle-left"><i class="far fa-arrow-alt-circle-left"></i>Assign selected</a>
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR3">
                                <td>
                                    <asp:LinkButton Visible="false" runat="server" ID="AssignAllBtn" OnClientClick="ProcessButton(this,'Executing...')" OnClick="AssignAllBtn_Click" CssClass="btn lightblue width180px"><i class="fas fa-plus-circle"></i> Assign all</asp:LinkButton>
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR4">
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR5">
                                <td style="text-align: center">Customer unassignment 
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR6">
                                <td>
                                    <a id="btnUnAssignSelection" class="btn bleu width180px btn-disabled" data-button-text="Remove selected" data-button-icon="fas fa-arrow-alt-circle-right"><i class="far fa-arrow-alt-circle-right"></i>Remove selected</a>
                                </td>
                            </tr>
                            <tr runat="server" id="managementTR7">
                                <td>
                                    <asp:LinkButton Visible="false" runat="server" ID="RemoveAllBtn" OnClientClick="ProcessButton(this,'Executing...')" OnClick="RemoveAllBtn_Click" CssClass="btn red width180px"><i class="fas fa-minus-circle""></i> Remove all</asp:LinkButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" style="width: 45%">
                        <div class="card card-primary">
                            <div class="card-header">
                                Available customers <span runat="server" class="Electrolux_Color" id="unassignedCount"></span><span id="unassignedSelected" style="float: right; color: red; padding-right: 5px;"></span>
                            </div>
                            <div class="card-body">
                                <table runat="server" style="width: 100%">
                                    <tr>
                                        <th runat="server" id="ManageUnassignedTH" colspan="2">
                                            <a class="defaultLink linkColor" onclick="SelectRows(true,1)">Select all</a> | <a class="defaultLink linkColor" onclick="SelectRows(false,1)">Unselect all </a>| Display
                                            <select class="unassignedDisplayDD">
                                                <option value="All">All</option>
                                                <option value="Selected">Selected</option>
                                                <option value="Unselected">Unselected</option>
                                            </select>
                                        </th>
                                    </tr>
                                    <tr runat="server">
                                        <th runat="server" style="display: inline-block; width: 65% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Name</th>

                                        <th runat="server" style="display: inline-block; width: 32% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Code</th>
                                    </tr>
                                </table>
                                <asp:ListView runat="server" ID="ListView2">
                                    <LayoutTemplate>
                                        <div style="height: 460px; overflow-y: auto;">
                                            <table runat="server" id="table2" class="list-view" style="width: 100%">
                                                <tr runat="server" id="itemPlaceholder" style="width: 100%; display: inline-block;"></tr>
                                            </table>
                                        </div>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr runat="server" class="odd-row list-row list-unassigned-row display-inline caro"
                                            data-search='<%#Eval("C_Name").ToString().ToLower() + "#" + Eval("C_CUID").ToString().ToLower() + "#" + Eval("C_DESCRIPTION").ToString().ToLower() + "#" + Eval("CH_CUID_5").ToString().ToLower() + "#" + Eval("CH_NAME_5").ToString().ToLower() + "#" + Eval("CH_CUID_4").ToString().ToLower() + "#" + Eval("CH_NAME_4").ToString().ToLower() + "#" + Eval("CH_CUID_3").ToString().ToLower() + "#" + Eval("CH_NAME_3").ToString().ToLower() + "#" + Eval("PLC").ToString().ToLower() + "#" + Eval("PLC_description").ToString().ToLower() + "#" + Eval("C_GRP4").ToString().ToLower() %>'
                                            data-cid='<%# Eval("C_GLOBALID") %>' onclick="changeSelectionStatus(this,1)" 
                                            style="width: 100%; padding-top:8px; padding-bottom:5px;">
                                            <td style="display: inline-block; width: 24px; vertical-align: top">
                                                <span class="checkedTickBox hidden" style="padding: 5px;"><i class="Electrolux_Color fas fa-check"></i></span>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(65% - 24px);">
                                                <b><asp:Label ID="NameLabel2" CssClass="search-text" runat="server" Height="12"
                                                    Text='<%#Eval("C_Name") %>' /></b>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                <b><asp:Label ID="Label3" CssClass="search-cuid" runat="server" Height="12"
                                                    Text='<%#Eval("C_CUID") %>' /></b>
                                            </td>



                                            <td runat="server" visible='<%#Eval("PLC").ToString().Length > 0  %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                PLC: 
                                                <asp:Label ID="Label9" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("PLC") %>' />
                                                --
                                                <asp:Label ID="Label10" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("PLC_description") %>' />
                                            </td>

                                            <td runat="server"  visible='<%#Eval("CH_CUID_3").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label7" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_3") %>' />
                                                --
                                                <asp:Label ID="Label8" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_3") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label5" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_4") %>' />
                                                --
                                                <asp:Label ID="Label6" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_4") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_5").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label2" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_5") %>' />
                                                --
                                                <asp:Label ID="Label4" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_5") %>' />
                                            </td>
                                            <td runat="server" visible='<%#Eval("C_GRP4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%; font-size: 10px;">
                                                <span class="" style="width: 24px; display: inline-block;"></span>
                                                Cust.Group4: 
                                                <asp:Label ID="Label11" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("C_GRP4") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr runat="server" class="even-row list-row list-unassigned-row display-inline caro"
                                            data-search='<%#Eval("C_Name").ToString().ToLower() + "#" + Eval("C_CUID").ToString().ToLower() + "#" + Eval("C_DESCRIPTION").ToString().ToLower() + "#" + Eval("CH_CUID_5").ToString().ToLower() + "#" + Eval("CH_NAME_5").ToString().ToLower() + "#" + Eval("CH_CUID_4").ToString().ToLower() + "#" + Eval("CH_NAME_4").ToString().ToLower() + "#" + Eval("CH_CUID_3").ToString().ToLower() + "#" + Eval("CH_NAME_3").ToString().ToLower() + "#" + Eval("PLC").ToString().ToLower() + "#" + Eval("PLC_Description").ToString().ToLower() + "#" + Eval("C_GRP4").ToString().ToLower() %>'
                                            data-cid='<%# Eval("C_GLOBALID") %>' onclick="changeSelectionStatus(this,1)" 
                                            style="width: 100%; padding-top:8px; padding-bottom:5px;">
                                            <td style="display: inline-block; width: 24px; vertical-align: top">
                                                <span class="checkedTickBox hidden" style="padding: 5px;"><i class="Electrolux_Color fas fa-check"></i></span>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(65% - 24px);">
                                                <b><asp:Label ID="NameLabel2" CssClass="search-text" runat="server" Height="12"
                                                    Text='<%#Eval("C_Name") %>' /></b>
                                            </td>
                                            <td runat="server" style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                <b><asp:Label ID="Label3" CssClass="search-cuid" runat="server" Height="12"
                                                    Text='<%#Eval("C_CUID") %>' /></b>
                                            </td>
    
                                            <td runat="server" visible='<%#Eval("PLC").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                PLC: 
                                                <asp:Label ID="Label9" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("PLC") %>' />
                                                --
                                                <asp:Label ID="Label10" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("PLC_description") %>' />
                                            </td>

                                            <td runat="server"  visible='<%#Eval("CH_CUID_3").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label7" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_3") %>' />
                                                --
                                                <asp:Label ID="Label8" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_3") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label5" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_4") %>' />
                                                --
                                                <asp:Label ID="Label6" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_4") %>' />
                                            </td>
                                            <td runat="server"  visible='<%#Eval("CH_CUID_5").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%;font-size:10px;">
                                                <span class="" style="width: 24px; display:inline-block;"></span>
                                                <asp:Label ID="Label2" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("CH_CUID_5") %>' />
                                                --
                                                <asp:Label ID="Label4" CssClass="search-text" runat="server" Height="10"
                                                    Text='<%#Eval("CH_NAME_5") %>' />
                                            </td>
                                            <td runat="server" visible='<%#Eval("C_GRP4").ToString().Length > 0 %>'
                                                style="display: inline-block; width: 100%; font-size: 10px;">
                                                <span class="" style="width: 24px; display: inline-block;"></span>
                                                Cust.Group4: 
                                                <asp:Label ID="Label11" CssClass="search-cuid" runat="server" Height="10"
                                                    Text='<%#Eval("C_GRP4") %>' />
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AssignAllBtn" />
            <asp:AsyncPostBackTrigger ControlID="RemoveAllBtn" />
            <asp:AsyncPostBackTrigger ControlID="RefreshHD" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

