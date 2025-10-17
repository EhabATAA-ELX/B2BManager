<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TP2Customers.aspx.vb" Inherits="TP2Customers" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <script>
        $(document).ready(function () {
            // This function formats how each item in the dropdown list appears
            function formatCountry(country) {
                if (!country.id) {
                    return country.text; // For the search box
                }
                // Get the image URL from the data-attribute we added in the code-behind
                var imageUrl = $(country.element).attr('data-image-url');
                if (!imageUrl) {
                    return country.text;
                }

                // Create the HTML for the option
                var $country = $(
                    '<span><img src="' + imageUrl + '" class="country-flag" /> ' + country.text + '</span>'
                );
                return $country;
            };

            // Initialize Select2 on our ListBox
            $('.select2-dropdown').select2({
                placeholder: 'Select Countries...',
                templateResult: formatCountry, // Function to render items in the dropdown
                templateSelection: formatCountry // Function to render the selected items
            });

            var table = $('#<%= CustomerTable.ClientID %>').DataTable({
                "pageLength": 15,
                "lengthChange": false,
                "ordering": true,
                "pagingType": "simple",
                "order": [[1, "asc"]]
            });

            function formatRowDetails(customerId) {
                var tableId = "childTable_" + customerId;
                var html = '<div>Loading...</div>';

                $.ajax({
                    type: "POST",
                    url: "TP2Customers.aspx/GetCustomerDetails",
                    data: JSON.stringify({ tpcId: customerId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (response) {
                        var rows = response.d;

                        //  add the "Add Customer" below non-empty table
                        html = '<div style="margin-top:10px;">' +
                            '<button class="btn-add-customer" data-parent="' + customerId + '">' +
                            '+ Add Customer' +
                            '</button>' +
                            '</div>';

                        if (rows.length === 0) {
                            // No children → show message
                            html += '<div>No linked customers found.</div>';
                            return;
                        }


                        // build table
                        html += '<table id="' + tableId + '" class="child-table display">' +
                            '<thead>' +
                            '<tr>' +
                            '<th>Customer Code (C_CUID)</th>' +
                            '<th>Country (CY_ISOCODE)</th>' +
                            '</tr>' +
                            '</thead><tbody>';

                        for (var i = 0; i < rows.length; i++) {
                            var r = rows[i];
                            html += '<tr>' +
                                '<td>' + r.C_CUID + '</td>' +
                                '<td>' + r.CY_ISOCODE + '</td>' +
                                '</tr>';
                        }

                        html += '</tbody></table>';


                    }
                });

                return html;
            }

            // Expand/collapse on button click
            $('#<%= CustomerTable.ClientID %> tbody').on('click', 'button.btn-expand', function () {
                var tr = $(this).closest('tr');
                var row = table.row(tr);
                var customerId = $(this).data("id");

                if (row.child.isShown()) {
                    row.child.hide();
                    $(this).text('+');
                } else {
                    var childHtml = formatRowDetails(customerId);
                    row.child(childHtml).show();

                    // Initialize DataTable for the child table
                    var childTableId = "#childTable_" + customerId;
                    $(childTableId).DataTable({
                        "pageLength": 10,
                        "lengthChange": false,
                        "ordering": true,
                        "pagingType": "simple",
                        "order": [[1, "asc"]]
                    });

                    $(this).text('-');
                }
            });

            $(document).on('click', '.btn-add-customer', function () {
                var parentId = $(this).data("parent");
                var container = $(this).parent();

                // If form already exists, don’t add again
                if (container.find('.add-form').length > 0) return;

                var formHtml = `
                    <div class="add-form" style="margin-top:10px; padding:10px; border:1px solid #ccc; background:#f9f9f9; width:90%;">
                        <label>Customer Code:</label> <input type="text" id="txtCUID_${parentId}" style="width:150px;" />
                        <label>Name:</label> <input type="text" id="txtName_${parentId}" style="width:200px;" />
                        <label>Country:</label> <input type="text" id="txtCountry_${parentId}" style="width:80px;" />

                        <button class="btn-save-customer" data-parent="${parentId}">Save</button>
                        <button class="btn-cancel-customer" data-parent="${parentId}">Cancel</button>
                    </div>
                `;

                container.append(formHtml);
            });

            // Edit icon click
            $(document).on('click', '.btn-edit', function () {
                var tpcId = $(this).data("id");
                var name = $(this).data("name");
                var tpid = $(this).data("tpid");
                var tpType = $(this).data("tptype");

                $("#<%= txtEditCustomerCode.ClientID %>").val(tpcId);
                $("#<%= txtEditName.ClientID %>").val(name);
                $("#<%= txtEditTPID.ClientID %>").val(tpid);
                $("#<%= txtEditTPType.ClientID %>").val(tpType);

                //highlight the selected row in the grid
                $("#<%= CustomerTable.ClientID %> tr").removeClass("selected-row");
                $(this).closest("tr").addClass("selected-row");
            });


            $(document).on('click', '.btn-cancel-customer', function () {
                $(this).closest('.add-form').remove();
            });

            $(document).on('click', '.btn-save-customer', function () {
                var parentId = $(this).data("parent");

                var cuid = $("#txtCUID_" + parentId).val();
                var name = $("#txtName_" + parentId).val();
                var country = $("#txtCountry_" + parentId).val();

                if (!cuid || !name || !country) {
                    alert("All fields are required!");
                    return;
                }

                $.ajax({
                    type: "POST",
                    url: "TP2Customers.aspx/AddCustomerToTPC",
                    data: JSON.stringify({ tpcId: parentId, cCUID: cuid, cName: name, cyIso: country }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {
                        alert("Customer added successfully!");
                        // Reload the child table
                        var tr = $('button.btn-expand[data-id="' + parentId + '"]').closest('tr');
                        var row = $('#<%= CustomerTable.ClientID %>').DataTable().row(tr);
                        row.child(formatRowDetails(parentId)).show();
                    },
                    error: function () {
                        alert("Error saving customer!");
                    }
                });
            });

        });

    </script>
    <style type="text/css">
        /* Parent table styling */
        .parent-table {
            border-collapse: collapse;
            width: 100%;
            font-size: 13px;
        }

            .parent-table th, .parent-table td {
                border: 1px solid #ccc;
                padding: 6px;
            }

            .parent-table thead th {
                background-color: #f5f5f5;
                font-weight: bold;
                text-align: center;
            }

        /* Child tables styling */
        .child-table {
            border-collapse: collapse;
            width: 90%;
            margin-left: 50px;
            font-size: 12px;
            background-color: #fafafa;
        }

            .child-table th, .child-table td {
                border: 1px solid #ddd;
                padding: 5px;
            }

            .child-table thead th {
                background-color: #e9f2fb;
                font-weight: normal;
            }

        /*highlight the selected row in the grid*/
        .selected-row {
            background-color: #fef3c7; /* light yellow */
        }

        .badge {
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 12px;
            text-decoration: none;
        }

        .badge-info {
            background-color: #17a2b8;
            color: #fff;
        }

        .badgsecondary {
            background-color: #6c757d;
            color: #fff;
        }

        .badge:hover {
            opacity: 0.8;
        }

        /* Style for images in the dropdown list */
        .select2-results__option .country-flag {
            width: 20px;
            height: 15px;
            margin-right: 8px;
            vertical-align: middle;
        }

        /* --- Modern Look for Select2 Multi-Select --- */

        /* The main container holding the selected items */
        .select2-container--default .select2-selection--multiple {
            border: 1px solid #ced4da;
            border-radius: 0.375rem;
            padding: 2px; /* Add a little space for the pills */
        }

            /* Each selected item "pill" */
            .select2-container--default .select2-selection--multiple .select2-selection__choice {
                background-color: #e9ecef;
                border: 1px solid #dee2e6;
                color: #495057;
                border-radius: 1rem;
                padding: 3px 6px 3px 8px; /* Fine-tuned padding */
                font-size: 0.875em;
                margin: 2px; /* Uniform margin */
                display: inline-flex;
                align-items: center;
            }

                /* Style for flags inside the SELECTED pills */
                .select2-container--default .select2-selection--multiple .select2-selection__choice .country-flag {
                    width: 16px;
                    height: 12px;
                    margin-right: 6px; /* A bit more space next to text */
                }

            /* The 'x' remove button on each pill */
            .select2-container--default .select2-selection--multiple .select2-selection__choice__remove {
                color: #6c757d;
                font-weight: bold;
                font-size: 1.2em;
                margin-left: auto; /* THIS IS THE KEY: Pushes the 'x' to the far right */
                padding: 4px; /* Adds some clickable space around the 'x' */
                cursor: pointer;
            }

                /* Hover effect for the 'x' remove button to make it clear */
                .select2-container--default .select2-selection--multiple .select2-selection__choice__remove:hover {
                    color: #dc3545; /* A clear "danger" or "remove" color on hover */
                    background-color: #e2e6ea;
                }

            /* Style the placeholder text */
            .select2-container--default .select2-selection--multiple .select2-search__field::placeholder {
                color: #6c757d;
                opacity: 1;
            }

        .select2-selection__choice__display {
            padding-left: 15px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>

    <div style="width: 350px;">
        <asp:ListBox ID="lstCountry" runat="server" Width="350px" SelectionMode="Multiple" CssClass="select2-dropdown"></asp:ListBox>
    </div>

    <asp:UpdatePanel ID="UpdatePanelTP2Customers" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div id="LoadingPanel" class="hidden" style="position: absolute; top: 80px; width: 100%; height: 100%; z-index: 1000;">
                <asp:Image ID="imgLoading" runat="server" ImageUrl="Images/Loading.gif" />
            </div>

            <div style="display: flex; width: 100%;">
                <!-- Left: Table -->
                <div style="flex: 0 0 60%; padding-right: 10px;">
                    <asp:Table ID="CustomerTable" runat="server"
                        CssClass="display parent-table"
                        Style="border-collapse: collapse; font-size: 12px; width: 100%;">
                        <asp:TableHeaderRow TableSection="TableHeader" runat="server">
                            <asp:TableHeaderCell>Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Customer Code</asp:TableHeaderCell>
                            <asp:TableHeaderCell>TP Type</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell>TPID</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Countries</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Client Count</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>

                <!-- Right: Edit Panel -->
                <div style="flex: 1; border-left: 1px solid #ccc; padding-left: 15px;" id="editPanel">
                    <h3>Edit TP2 Customer</h3>
                    <table style="width: 100%; font-size: 12px;">
                        <tr>
                            <td>Customer Code:</td>
                            <td>
                                <asp:TextBox ID="txtEditCustomerCode" runat="server" Width="200px" /></td>
                        </tr>
                        <tr>
                            <td>Name:</td>
                            <td>
                                <asp:TextBox ID="txtEditName" runat="server" Width="200px" /></td>
                        </tr>
                        <tr>
                            <td>TPID:</td>
                            <td>
                                <asp:TextBox ID="txtEditTPID" runat="server" Width="200px" /></td>
                        </tr>
                        <tr>
                            <td>TP Type:</td>
                            <td>
                                <asp:TextBox ID="txtEditTPType" runat="server" Width="200px" /></td>
                        </tr>
                    </table>
                    <asp:Button ID="btnSaveEdit" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSaveEdit_Click" />

                </div>

            </div>

        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

