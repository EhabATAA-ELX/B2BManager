<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="InDirectAssignment.aspx.vb" Inherits="InDirectAssignment" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
     <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>

    <style type="text/css">
        /* Main style for the criteria table */
        .parent-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
            border: 1px solid #ddd;
            box-shadow: 0 2px 4px rgba(0,0,0,0.05);
        }


            .parent-table th {
                background-color: #f2f5f7;
                font-weight: 600;
                text-align: left;
                padding: 12px 15px;
                border-bottom: 2px solid #007bff;
            }


            .parent-table td {
                padding: 10px 15px;
                border-bottom: 1px solid #e9ecef;
                vertical-align: middle;
            }

            .parent-table tr:nth-child(even) {
                background-color: #f9fafb;
            }

            .parent-table tr:hover {
                background-color: #e9ecef;
            }


            .parent-table tr:last-child td {
                border-bottom: 0;
            }

        /* END Main style for the criteria table */

        .main-container {
            display: flex;
            width: 100%;
        }

        .left-panel {
            flex: 0 0 60%;
            padding-right: 20px;
        }

        .right-panel {
            flex: 1;
            border-left: 1px solid #ccc;
            padding-left: 20px;
            background-color: #f8f9fa;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <asp:Label ID="DocumentGuidLabel" ClientIDMode="Static" runat="server" Style="display: none;" />
    <asp:Label runat="server" ID="EnvironmentIDLabel" ClientIDMode="Static" class="hidden" />
    <asp:Label runat="server" ID="CountryGuidLabel" ClientIDMode="Static" class="hidden" />
    <asp:HiddenField ID="hdnConditionID" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdnSopId" runat="server" />

    <script type="text/javascript">

        function openCriterionWindow() {
            $find("<%= RadWindow_EditCriterion.ClientID %>").show();
        }

        function closeCriterionWindow() {
            $find("<%= RadWindow_EditCriterion.ClientID %>").close();
        }

        function clearCriterionModal() {
            $('#hdnCrit_EditingCriteriaNumber').val('');
            $('#<%= cmbField.ClientID %>').prop('selectedIndex', 0);
            $('#<%= ddlOperator.ClientID %>').prop('selectedIndex', 0);
            $('#<%= txtValue.ClientID %>').val('');
        }

        // --- Global SAVE function for the modal ---
        function validateAndSaveCriterion() {
            var conditionId = $('#hdnConditionID').val();
            var critNumber = $('#hdnCrit_EditingCriteriaNumber').val();
            var isEditMode = critNumber !== "";
            var cmbField = $find("<%= cmbField.ClientID %>");
            var selectedField = cmbField.get_selectedItem().get_value();
            var dataToSend = {
                conditionId: conditionId,
                criteriaNumber: isEditMode ? critNumber : 0,
                field: selectedField,
                op: $('#<%= ddlOperator.ClientID %>').val(),
                value: $('#<%= txtValue.ClientID %>').val()
            };
            var url = isEditMode ? "InDirectAssignment.aspx/UpdateCriterion" : "InDirectAssignment.aspx/AddCriterion";

            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(dataToSend),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d === true) {
                        location.reload();
                    } else {
                        alert("Error: Could not save criterion.");
                    }
                },
                error: function () {
                    alert("A server error occurred.");
                }
            });
        }

        function closeCriterionWindow() {
            var radWindow = $find("<%= RadWindow_EditCriterion.ClientID %>");

            if (radWindow) {
                radWindow.close();
            }
        }

        function openAddConditionWindow() {
            var radWindow = $find("<%= RadWindow_AddCondition.ClientID %>");
            radWindow.show();
        }

        function closeAddConditionWindow() {
            var radWindow = $find("<%= RadWindow_AddCondition.ClientID %>");
            radWindow.close();
        }

        function toggleDropdown(checkbox, dropdownId) {
            var dropdown = document.getElementById(dropdownId);
            dropdown.disabled = !checkbox.checked;
            if (!checkbox.checked) {
                dropdown.selectedIndex = 0;
            }
        }


        // Reusable function for refreshing the preview panel
        function refreshPreview(conditionId) {
            if (!conditionId) {
                // If there's no ID, clear the panel
                $('#previewTable tbody').empty();
                $('#customerCount').text('0');
                return;
            }

            // Show a loading message
            $('#previewTable tbody').html('<tr><td colspan="2"><em></em></td></tr>');
            $('#customerCount').text('...');
            var sopId = $('#<%= hdnSopId.ClientID %>').val();

            $.ajax({
                type: "POST",
                url: "InDirectAssignment.aspx/GetMatchingCustomers",
                data: JSON.stringify({ conditionId: conditionId, sopName: sopId }),
                contentType: "application/json; charset=utf-8",
                success: function (response) {
                    var customers = JSON.parse(response.d);
                    var tableBody = $('#previewTable tbody');

                    if ($.fn.DataTable.isDataTable('#previewTable')) {
                        $('#previewTable').DataTable().destroy();
                    }

                    tableBody.empty();
                    $('#customerCount').text(customers.length);

                    if (customers.length > 0) {
                        $.each(customers, function (index, customer) {
                            tableBody.append('<tr><td>' + customer.C_CUID + '</td><td>' + customer.C_NAME + '</td></tr>');
                        });
                    }

                    $('#previewTable').DataTable({
                        "pageLength": 20,
                        "searching": true,
                        "paging": true,
                        "destroy": true,
                        "info": true
                    });
                },
                error: function () {
                    /* $('#previewTable tbody').html('<tr><td colspan="2" style="color:red;">An error occurred.</td></tr>');*/
                }
            });
        }


        $(document).ready(function () {

            // --- Click handler for ADD new criterion ---
            $(document).on('click', '.btn-add-criterion', function () {
                clearCriterionModal();
                $find("<%= RadWindow_EditCriterion.ClientID %>").set_title("Add New Criterion");
                openCriterionWindow();
            });

            // --- Click handler for EDIT existing criterion ---
            $(document).on('click', '.btn-edit-criterion', function () {
                var conditionId = $(this).data("conditionid");
                var criteriaNumber = $(this).data("criteriaid");

                $.ajax({
                    type: "POST",
                    url: "InDirectAssignment.aspx/GetCriterionForEdit",
                    data: JSON.stringify({ conditionId: conditionId, criteriaNumber: criteriaNumber }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var crit = response.d;
                        clearCriterionModal();
                        var cmbField = $find("<%= cmbField.ClientID %>");
                        var targetValue = crit.ConditionField;

                        cmbField.clearSelection();
                        cmbField.set_text("");

                        var itemToSelect = cmbField.findItemByValue(targetValue);

                        if (itemToSelect) {
                            itemToSelect.select();
                        } else {
                            cmbField.set_text(targetValue);
                        }
                        $('#hdnCrit_EditingCriteriaNumber').val(crit.CriteriaNumber);
                        $('#<%= ddlOperator.ClientID %>').val(crit.ConditionOperator);
                        $('#<%= txtValue.ClientID %>').val(crit.ConditionValues);

                        $find("<%= RadWindow_EditCriterion.ClientID %>").set_title("Edit Criterion");
                        openCriterionWindow();
                    }
                });
            });

            // Event handler for deleting a criterion
            $(document).on('click', '.btn-delete-criterion', function () {
                var button = $(this);
                var conditionId = $(this).data("conditionid");
                var criteriaNumber = button.data("criteriaid");

                $.ajax({
                    type: "POST",
                    url: "InDirectAssignment.aspx/DeleteCriterion",
                    data: JSON.stringify({ conditionId: conditionId, criteriaNumber: criteriaNumber }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d === true) {
                            location.reload();
                        } else {
                            alert("Error: Could not delete criterion.");
                        }
                    }
                });
            });
        });
    </script>

    <style type="text/css">
        /* Style for the new header/filter bar */
        .filter-bar {
            display: flex;
            align-items: center;
            gap: 15px;
            padding-bottom: 15px;
            flex-wrap: wrap;
        }

        .filter-group {
            display: flex;
            align-items: center;
            gap: 5px;
        }
    </style>

    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" RenderMode="Lightweight" />

    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="false" />
    <hr />

    <%-- ===================================================================== --%>
    <%--                   2. THE RADWINDOW MODAL POPUP                        --%>
    <%-- ===================================================================== --%>

    <telerik:RadWindow ID="RadWindow_AddCondition" runat="server" Title="Add New Condition"
        Modal="true" VisibleOnPageLoad="false" Behaviors="Close"
        Width="550px" Height="300px" RenderMode="Lightweight">
        <ContentTemplate>
            <style type="text/css">
                /* Styles for the modal form */
                .modal-form-table {
                    width: 100%;
                    border-collapse: separate;
                    border-spacing: 0 15px;
                }

                    .modal-form-table td {
                        vertical-align: middle;
                    }

                    .modal-form-table .label-cell {
                        width: 130px;
                        text-align: right;
                        padding-right: 10px;
                        font-weight: bold;
                    }
            </style>

            <div style="padding: 20px;">
                <asp:UpdatePanel ID="updAddCondition" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ClientIDMode="Static" ID="hdnEditingConditionID" runat="server" />
                        <table class="modal-form-table">
                            <tr>
                                <td class="label-cell">Condition Name:</td>
                                <td>
                                    <asp:TextBox ID="txtConditionName" runat="server" Width="300px" />
                                    <span id="errMsgConditionName" style="color: red; display: none;">Name is required.</span>
                                </td>
                            </tr>
                            <tr>
                                <td class="label-cell">Is Static:</td>
                                <td>
                                    <asp:CheckBox ID="chkIsStatic" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label-cell">Is Focus Range:</td>
                                <td>
                                    <asp:CheckBox ID="chkIsFocusRange" runat="server" onclick="toggleDropdown(this, '<%= ddlFocusRanges.ClientID %>');" />
                                    <asp:DropDownList ID="ddlFocusRanges" runat="server" Enabled="false">
                                        <asp:ListItem Text="-- Select a Range --" Value="" />
                                        <asp:ListItem Text="Spring 2025 Promo" Value="GUID-FOR-RANGE-1" />
                                        <asp:ListItem Text="Core Products" Value="GUID-FOR-RANGE-2" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="label-cell">Is Super User:</td>
                                <td>
                                    <asp:CheckBox ID="chkIsSuperUser" runat="server" onclick="toggleDropdown(this, '<%= ddlSuperUsers.ClientID %>');" />
                                    <asp:DropDownList ID="ddlSuperUsers" runat="server" Enabled="false">
                                        <asp:ListItem Text="-- Select a User --" Value="" />
                                        <asp:ListItem Text="Admins" Value="GUID-FOR-USER-GROUP-1" />
                                        <asp:ListItem Text="Managers" Value="GUID-FOR-USER-GROUP-2" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="padding-top: 20px;">
                                    <%-- Client-side validation happens here --%>
                                    <input type="button" class="btn green" value="Save" onclick="validateAndSaveCondition()" />
                                    <input type="button" class="btn red" value="Cancel" onclick="closeAddConditionWindow(); return false;" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <%-- ===================================================================== --%>
    <%--                3. THE RADWINDOW FOR ADD/EDIT CRITERION                --%>
    <%-- ===================================================================== --%>
    <telerik:RadWindow ID="RadWindow_EditCriterion" runat="server" Title="Add New Criterion"
        Modal="true" VisibleOnPageLoad="false" Behaviors="Close" AutoSize="true"
        Width="600px" RenderMode="Lightweight" OnClientClose="clearCriterionModal">
        <ContentTemplate>
            <style>
                .criterion-form-table {
                    width: 100%;
                    border-collapse: separate;
                    border-spacing: 0 15px;
                }

                    .criterion-form-table td {
                        vertical-align: middle;
                    }

                .crit-label {
                    width: 100px;
                    text-align: right;
                    padding-right: 10px;
                    font-weight: bold;
                }
            </style>
            <div style="padding: 20px;">
                <asp:UpdatePanel ID="updEditCriterion" runat="server">
                    <ContentTemplate>
                        <%-- Hidden fields to manage state --%>
                        <input type="hidden" id="hdnCrit_EditingCriteriaNumber" />

                        <table class="criterion-form-table">
                            <tr>
                                <td class="crit-label">Field:</td>
                                <td>
                                    <telerik:RadComboBox ID="cmbField" runat="server" Width="350px"
                                        AllowCustomText="true" 
                                        Filter="Contains"
                                        MarkFirstMatch="true"
                                        EmptyMessage="Type to search...">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="crit-label">Operator:</td>
                                <td>
                                    <asp:DropDownList ID="ddlOperator" runat="server" Width="350px" /></td>
                            </tr>
                            <tr>
                                <td class="crit-label">Value:</td>
                                <td>
                                    <asp:TextBox ID="txtValue" runat="server" Width="344px" /></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="padding-top: 20px;">
                                    <input type="button" class="btn green" value="Save" onclick="validateAndSaveCriterion()" />
                                    <input type="button" class="btn red" value="Cancel" onclick="closeCriterionWindow(); return false;" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>

    <%-- This is the main container for the two-panel layout --%>
    <div class="main-container">

        <%-- LEFT PANEL --%>
        <div id="leftPanel" runat="server" class="left-panel">
            <h3>Criteria for Condition:
                <asp:Literal ID="litConditionName" runat="server" /></h3>
            <hr />

            <div class="filter-bar" runat="server" id="divActionBar">
                <%-- Main action buttons are now at the top --%>
                <input type="button" class="btn bleu btn-add-criterion" value="+ Add New Criterion" />
            </div>

            <asp:Table ID="CriteriaTable" runat="server" CssClass="display parent-table" Style="width: 100%;">
                <asp:TableHeaderRow TableSection="TableHeader" runat="server">
                    <asp:TableHeaderCell>Actions</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Field</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Operator</asp:TableHeaderCell>
                    <asp:TableHeaderCell>Value</asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <%-- Rows are added from code-behind --%>
            </asp:Table>
        </div>

        <%-- RIGHT PANEL - This is the dedicated preview area --%>
        <div class="right-panel" id="divrightPanel" runat="server">
            <h3>Preview: <span id="customerCount" style="color: #0056b3;">0</span> Matching Customers</h3>

            <table id="previewTable" class="display" style="width: 100%;">
                <thead>
                    <tr>
                        <th>Customer Code</th>
                        <th>Customer Name</th>
                    </tr>
                </thead>
                <tbody>
                    <%-- Data will be injected here by JavaScript --%>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>

