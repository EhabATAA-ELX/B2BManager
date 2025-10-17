<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="QueryBuilderStaticAssignment.aspx.vb" Inherits="QueryBuilderStaticAssignment" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnConditionID" ClientIDMode="Static" runat="server" />
    <asp:Panel ID="pnlStaticAssignment" runat="server" Visible="false">
        <style>
            .filtered-out {
                display: none !important;
            }

            .list-view-body tr {
                cursor: pointer;
            }

                .list-view-body tr.selected-row {
                    background-color: #fff8e1 !important;
                }

            .card-body {
                height: 500px;
                overflow-y: auto;
                border: 1px solid #ddd;
            }

            .action-buttons {
                width: 10%;
                vertical-align: middle;
                text-align: center;
            }

        </style>
        <script type="text/javascript">
            // This flag tracks if any changes have been made.
            var isDirty = false;

            // This is the main function that decides whether to close the window.
            function confirmAndClose() {
                if (isDirty) {
                    return confirm("You have unsaved changes. Are you sure you want to close and discard them?");
                }
                return true;
            }


            function closeActiveWindow() {
                if (typeof window.parent.closeActiveWindow === "function") {
                    window.parent.closeActiveWindow();
                }
            }

            //Start Static part

            function updateCounts() {
                $('#assignedCount').text('(' + $('#assignedTable tbody tr').length + ')');
                $('#availableCount').text('(' + $('#availableTable tbody tr').length + ')');
            }

            function ConfirmAssignment() {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer list updated with success.</p></div>',
                        type: 'inline'
                    }
                });
            }

            function searchInCustomerList() {
                var textSearch = $("#searchInCustomerListTextBox")[0].value.toString().toLowerCase();

                $("tr.list-row").removeClass("hidden").removeClass("filtred-search").addClass("display-inline");
                if (textSearch !== "") {
                    $("tr.list-row").addClass("hidden");
                    var matchingRows = $("tr.list-row[data-search*='" + textSearch + "']");
                    matchingRows.removeClass("hidden").addClass("filtred-search");
                    matchingRows.each(function () {
                        $(this).prependTo($(this).closest('tbody'));
                    });
                }
                else {
                    DisplaySelection($(".assignedDisplayDD").val(), 0);
                    DisplaySelection($(".unassignedDisplayDD").val(), 1);
                }
            }
            function SelectRows(shouldSelect, type) {
                var tableId = (type === 0) ? '#assignedTable' : '#availableTable';
                var rows = $(tableId + ' tbody tr:not(.filtered-out)'); // Only select visible rows

                if (shouldSelect) {
                    rows.addClass('selected-row');
                    rows.find('.checkedTickBox').removeClass('hidden');
                } else {
                    rows.removeClass('selected-row');
                    rows.find('.checkedTickBox').addClass('hidden');
                }
            }

            function DisplaySelection(selectionType, type) {
                var tableId = (type === 0) ? '#assignedTable' : '#availableTable';
                var rows = $(tableId + ' tbody tr');

                // Instead of hide()/show(), we toggle a class. 
                rows.removeClass('filtered-out');

                if (selectionType === "Selected") {
                    rows.not('.selected-row').addClass('filtered-out');
                } else if (selectionType === "Unselected") {
                    rows.filter('.selected-row').addClass('filtered-out');
                }
            }

            function removeHighlights() {
                $('#assignedTable, #availableTable').find('mark').each(function () {
                    // $(this).contents() gets the text node inside the <mark> tag
                    // .unwrap() removes the parent <mark> tag, leaving the text.
                    $(this).contents().unwrap();
                });
            }



            $(document).ready(function () {

                // Click handler for selecting/deselecting rows in EITHER list
                $(document).on('click', '#pnlStaticAssignment .list-view-body tr', function () {
                    $(this).toggleClass('selected-row');
                    $(this).find('.checkedTickBox').toggleClass('hidden');
                });

                // Click handler for the "Assign" button (move right to left)
                $('#btnAssign').on('click', function () {
                    isDirty = true;

                    // Find selected rows in the AVAILABLE table, detach them, and append to the ASSIGNED table
                    var selectedRows = $('#availableTable tbody tr.selected-row').detach();
                    $('#assignedTable tbody').prepend(selectedRows);
                    updateCounts();
                });

                // Click handler for the "Unassign" (Remove) button (move left to right)
                $('#btnUnAssign').on('click', function () {
                    isDirty = true;
                    var selectedRows = $('#assignedTable tbody tr.selected-row').detach();
                    $('#availableTable tbody').append(selectedRows);

                    selectedRows.removeClass('selected-row');
                    selectedRows.find('.checkedTickBox').addClass('hidden');
                    updateCounts();
                });



                // This ONE function handles selecting/deselecting rows in BOTH lists
                $(document).on('click', '.list-view-body tr', function () {
                    $(this).toggleClass('selected-row');
                    $(this).find('.checkedTickBox').toggleClass('hidden');

                    // Call the new function here
                    updateSelectedCounts();
                });

                // This handles the "Assign" button (move from right to left)
                $('#btnAssign').on('click', function () {
                    var selectedRows = $('#availableTable tbody tr.selected-row').detach();
                    $('#assignedTable tbody').append(selectedRows);
                    selectedRows.removeClass('selected-row').find('.checkedTickBox').addClass('hidden');
                    updateCounts();

                    updateSelectedCounts();
                });

                // This handles the "Unassign" button (move from left to right)
                $('#btnUnAssign').on('click', function () {
                    var selectedRows = $('#assignedTable tbody tr.selected-row').detach();
                    $('#availableTable tbody').append(selectedRows);
                    selectedRows.removeClass('selected-row').find('.checkedTickBox').addClass('hidden');
                    updateCounts();

                    updateSelectedCounts();
                });

                // This handles the final Save action
                $('#ContentPlaceHolder1_btnSaveChanges').on('click', function () {
                    var lastCritNumber = $(this).data('last-criteria-number');

                    var assignedCustomerCodes = [];
                    $('#assignedTable tbody tr').each(function () {
                        assignedCustomerCodes.push($(this).data('customercode'));
                    });

                    var dataToSend = {
                        conditionId: $('#hdnConditionID').val(),
                        customerCodes: assignedCustomerCodes.join(','),
                        lastCriteriaNumber: lastCritNumber
                    };

                    $.ajax({
                        type: 'POST',
                        url: 'InDirectAssignment.aspx/SaveChanges',
                        data: JSON.stringify(dataToSend),
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        success: function (response) {
                            if (response.d.toLowerCase() === "success") {
                                isDirty = false;
                                closeActiveWindow();
                                if (typeof window.parent.handleStaticSaveComplete === "function")
                                    window.parent.handleStaticSaveComplete();
                            } else {
                                alert('Error from server: ' + response.d);
                            }
                        },
                        error: function () { alert('A critical error occurred while saving.'); }
                    });
                });

                // Update counts when the page first loads
                updateCounts();
                updateSelectedCounts();

                function updateCounts() {
                    $('#ContentPlaceHolder1_assignedCountHeader').text('(' + $('#assignedTable tbody tr:not(.filtered)').length + ')');
                    $('#ContentPlaceHolder1_unassignedCountHeader').text('(' + $('#availableTable tbody tr:not(.filtered)').length + ')');
                }

                function updateSelectedCounts() {
                    // Count selected rows in the 'Assigned' table
                    var assignedSelectedCount = $('#assignedTable tbody tr.selected-row').length;
                    if (assignedSelectedCount > 0) {
                        $('#assignedSelected').text(assignedSelectedCount + ' selected');
                    } else {
                        $('#assignedSelected').text('');
                    }

                    // Count selected rows in the 'Available' table
                    var availableSelectedCount = $('#availableTable tbody tr.selected-row').length;
                    if (availableSelectedCount > 0) {
                        $('#unassignedSelected').text(availableSelectedCount + ' selected');
                    } else {
                        $('#unassignedSelected').text('');
                    }
                }
            });
        </script>

        <table style="width: 100%;">
            <tr>
                <%-- Assigned Customers List (Left) --%>
                <td style="width: 45%; vertical-align: top;">
                    <div class="card card-primary">
                        <div class="card-header">
                            Assigned customers <span runat="server" class="Electrolux_Color" id="assignedCountHeader"></span><span id="assignedSelected" style="float: right; color: red; padding-right: 5px;"></span>
                        </div>
                        <div class="card-body">
                            <table runat="server" style="width: 100%">
                                <tr>
                                    <th runat="server" id="Th1" colspan="2">
                                        <a class="defaultLink linkColor" onclick="SelectRows(true,0)">Select all</a> | <a class="defaultLink linkColor" onclick="SelectRows(false,0)">Unselect all </a>| Display
                                                <select class="assignedDisplayDD" onchange="DisplaySelection(this.value, 0)">
                                                    <option value="All">All</option>
                                                    <option value="Selected">Selected</option>
                                                    <option value="Unselected">Unselected</option>
                                                </select>
                                    </th>
                                </tr>
                            </table>
                            <table id="assignedTable" class="list-view" style="width: 100%">
                                <thead>
                                    <tr>
                                        <th runat="server" style="display: inline-block; width: 65% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Name</th>

                                        <th runat="server" style="display: inline-block; width: 32% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Code</th>

                                    </tr>
                                </thead>
                                <tbody id="itemPlaceholderContainer" runat="server" class="list-view-body">
                                    <asp:ListView runat="server" ID="ListView1">
                                        <LayoutTemplate>
                                            <div style="overflow-y: auto;">
                                                <tr runat="server" id="itemPlaceholder"></tr>
                                            </div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr runat="server" class="odd-row list-row list-assigned-row caro"
                                                data-search='<%#Eval("Name").ToString().ToLower() + "#" + Eval("CustomerCode").ToString().ToLower() + "#" + Eval("Description").ToString().ToLower() + "#" + Eval("CH_CUID_5").ToString().ToLower() + "#" + Eval("CH_NAME_5").ToString().ToLower() + "#" + Eval("CH_CUID_4").ToString().ToLower() + "#" + Eval("CH_NAME_4").ToString().ToLower() + "#" + Eval("CH_CUID_3").ToString().ToLower() + "#" + Eval("CH_NAME_3").ToString().ToLower() + "#" + Eval("PLC").ToString().ToLower() + "#" + Eval("PLC_Description").ToString().ToLower() + "#" + Eval("C_GRP4").ToString().ToLower() %>'
                                                data-cid='<%# Eval("ID") %>' data-name='<%# Eval("Name") %>' data-customercode='<%# Eval("CustomerCode") %>' data-description='<%# Eval("Description") %>'
                                                style="display: inline-block; width: 100%; padding-top: 8px; padding-bottom: 5px;">
                                                <td style="display: inline-block; width: 24px; vertical-align: top">
                                                    <span class="checkedTickBox hidden" style="padding: 5px;">
                                                        <i class="Electrolux_Color fas fa-check"></i>
                                                    </span>
                                                </td>
                                                <td style="display: inline-block; width: calc(65% - 24px);">
                                                    <b>
                                                        <asp:Label ID="Name1" CssClass="search-text" runat="server" Height="24" Text='<%#Eval("Name") %>' />
                                                    </b>
                                                </td>
                                                <td style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                    <b>
                                                        <asp:Label ID="CustomerCode1" CssClass="search-cuid" runat="server" Height="24" Text='<%#Eval("CustomerCode") %>' />
                                                    </b>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </td>
                <%-- Center Column for Action Buttons --%>
                <td style="width: 10%; padding-top: 70px; vertical-align: top">
                    <table style="margin-left: auto; margin-right: auto; border-collapse: separate; border-spacing: 10px;">
                        <tr>
                            <td style="text-align: center">Free text search </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" placeholder="Type in name or code or description" id="searchInCustomerListTextBox" class="width180px" style="height: 26px" onkeyup="searchInCustomerList()" />
                            </td>
                        </tr>
                        <tr>
                            <td class="action-buttons">
                                <a id="btnAssign" class="btn bleu width180px " data-button-text="Assign selected" data-button-icon="fas fa-arrow-alt-circle-left">
                                    <i class="far fa-arrow-alt-circle-left"></i>
                                    Assign selected
                                </a>
                            </td>
                        </tr>
                        <tr runat="server" id="managementTR5">
                            <td style="text-align: center">
                                <a id="btnUnAssign" class="btn bleu width180px " data-button-text="Remove selected" data-button-icon="fas fa-arrow-alt-circle-right">
                                    <i class="far fa-arrow-alt-circle-right"></i>
                                    Remove selected
                                </a>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center">
                                <input type="button" id="btnSaveChanges" class="btn green width180px" value="Save Changes" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center">
                                <input type="button" id="btnCancelChanges" onclick="closeActiveWindow(); return false;" class="btn red width180px" value="Cancel" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
                <%-- Available Customers List (Right) --%>
                <td style="width: 45%; vertical-align: top;">
                    <div class="card card-primary">
                        <div class="card-header">
                            Available customers <span runat="server" class="Electrolux_Color" id="unassignedCountHeader"></span><span id="unassignedSelected" style="float: right; color: red; padding-right: 5px;"></span>
                        </div>
                        <div class="card-body">
                            <table runat="server" style="width: 100%">
                                <tr>
                                    <th runat="server" id="ManageUnassignedTH" colspan="2">
                                        <a class="defaultLink linkColor" onclick="SelectRows(true,1)">Select all</a> | <a class="defaultLink linkColor" onclick="SelectRows(false,1)">Unselect all </a>| Display
                                                <select class="unassignedDisplayDD" onchange="DisplaySelection(this.value, 1)">
                                                    <option value="All">All</option>
                                                    <option value="Selected">Selected</option>
                                                    <option value="Unselected">Unselected</option>
                                                </select>
                                    </th>
                                </tr>
                            </table>
                            <table id="availableTable" class="list-view" style="width: 100%">
                                <thead>
                                    <tr>
                                        <th runat="server" style="display: inline-block; width: 65% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Name</th>

                                        <th runat="server" style="display: inline-block; width: 32% !important; padding-top: 10px; padding-bottom: 10px; margin: 0px">Customer Code</th>

                                    </tr>
                                </thead>

                                <tbody id="Tbody1" runat="server" class="list-view-body">
                                    <asp:ListView runat="server" ID="ListView2">
                                        <LayoutTemplate>
                                            <div style="overflow-y: auto;">
                                                <tr runat="server" id="itemPlaceholder"></tr>
                                            </div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr runat="server" class="odd-row list-row list-unassigned-row caro"
                                                data-search='<%#Eval("Name").ToString().ToLower() + "#" + Eval("CustomerCode").ToString().ToLower()  %>'
                                                data-cid='<%# Eval("ID") %>' data-name='<%# Eval("Name") %>' data-customercode='<%# Eval("CustomerCode") %>' data-description='<%# Eval("Description") %>'
                                                style="display: inline-block; width: 100%; padding-top: 8px; padding-bottom: 5px;">
                                                <td style="display: inline-block; width: 24px; vertical-align: top">
                                                    <span class="checkedTickBox hidden" style="padding: 5px;"><i class="Electrolux_Color fas fa-check"></i></span>
                                                </td>

                                                <td runat="server" style="display: inline-block; width: calc(65% - 24px);">
                                                    <b>

                                                        <asp:Label ID="Name3" CssClass="search-text" runat="server" Height="24" Text='<%#Eval("Name") %>' />
                                                    </b>
                                                </td>

                                                <td runat="server" style="display: inline-block; width: calc(100% - 65% - 24px);">
                                                    <b>
                                                        <asp:Label ID="CustomerCode3" CssClass="search-cuid" runat="server" Height="24" Text='<%#Eval("CustomerCode") %>' />
                                                    </b>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <hr />
    </asp:Panel>

</asp:Content>

