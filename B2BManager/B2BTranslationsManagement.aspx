<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="B2BTranslationsManagement.aspx.vb" Inherits="B2BTranslationsManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">

    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Buttons-1.5.6/js/dataTables.buttons.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Select-1.3.0/js/dataTables.select.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script>
        var table;
        var LangIsocode;
        var EditTemplate = '<%= GetEditTemplate() %>';

        function ShowAndRefreshGrid() {
             $('#ViewContainer').append("<div id='temporaryLoadingElement' style='width:100%;height:800px;margin-top:-105px' class='loadingBackgroundDefault' />");
              CloseWindow();
              table.ajax.reload(function () {
                        $("#temporaryLoadingElement").remove();;
                    }, false);
        }
        $(document).ready(function () {
            LoadGrid();
        });
        function LoadGrid() {
            $('#ViewContainer').append("<div id='temporaryLoadingElement' style='width:100%;height:800px;margin-top:-105px' class='loadingBackgroundDefault' />");
            var combo = $find("<%= ddlCountry.ClientID %>");
            LangIsocode = ($('#ContentPlaceHolder1_ddlLanguage').is(":visible")) ? $('#ContentPlaceHolder1_ddlLanguage').val() : $('#ContentPlaceHolder1_HdLanguage').val();
            table = $('#B2BTranslations').DataTable({
                "ajax": "B2BManagerService.svc/GetB2BTranslations?EnvironmentID=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&LangIsocode=" + LangIsocode + "&AreaName="+$('#<%= ddlArea.ClientID %>').val(),
                "pageLength": 15,
                "bLengthChange": false,
                "bSort": true,
                "columns": [
                    { width: 70 },
                    { "data": "TN_Name", width: 250 },
                    { "data": "DefaultValue", width: 350 },
                    { "data": "TN_Comment", width: 200 },
                    { "data": "CountryValue" },
                    { "data": "Areas" }
                ],
                "initComplete": function () {
                    $("#temporaryLoadingElement").remove();
                },
                "columnDefs": [
                    {//action
                        visible: true,
                        targets: 0,
                        orderable: false,
                        className: "TextAlignCenter",
                        data: "TN_GlobalID",
                        render: function (data, type, row) {
                            return EditTemplate.replaceAll("#data#", data) + '<i class="fas fa-layer-group area-icon" title="Manage translation areas" onclick="OpenAreasWindow(this)"></i>'
                        }
                    }
                ],
                "order": [[1, "asc"]],
                "rowId": "TN_GlobalID"

            });
             <%--$('#B2BTranslations').on('click', 'td', function (evt) {
                //var id = table.row(this).id();
                 var id = $(this).closest('tr').prop('id')
                 if ($(this).find("textarea").length == 0) {
                    var oWnd = $find("<%= WindowAreas.ClientID %>");
                    var environmentID = $("#<%= ddlEnvironment.ClientID %>").val();
                    oWnd.setUrl("B2BTranslationsAreas.aspx?HideHeader=true&Tn_GlobalID=" + id.toString() + "&EnvironmentID=" + environmentID);
                    oWnd.show();
                 }
              
            
             });--%>
        }

        function OpenAreasWindow(element) {
            var id = $(element).closest('tr').prop('id')
            if ($(element).find("textarea").length == 0) {
                var oWnd = $find("<%= WindowAreas.ClientID %>");
                var environmentID = $("#<%= ddlEnvironment.ClientID %>").val();
                oWnd.setUrl("B2BTranslationsAreas.aspx?HideHeader=true&Tn_GlobalID=" + id.toString() + "&EnvironmentID=" + environmentID);
                oWnd.show();
            }
        }

        
        function CloseWindow() {

            var oWnd = $find("<%= WindowAreas.ClientID %>");
            oWnd.close();
        }
    </script>
    <asp:PlaceHolder runat="server" ID="EditScript">
        <script type="text/javascript">
            function GetEditRow(obj) {
            var idRow = $(obj).attr('id');
            var $row = $(obj).closest("tr");
            $row.find("td").first().html("<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Update\" onclick=\"update(this,'" + idRow + "');return false;\">" +
                "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"CancelUpdate(this,'"+idRow+"');return false;\">");
            if ($('#ContentPlaceHolder1_HD_EDITALL').val() == "1") {
                var $tds = $row.find("td").not(':first').not(":nth-child(2)").not(":last");
            } else {
                if ($('#ContentPlaceHolder1_HD_EditLOCALVALUE').val() == "1") {
                var $tds = $row.find("td:nth-child(5)");
                }       
            }            
            $.each($tds, function (i, el) {
                var txt = $(this).text();
                $(this).html("").append("<textarea class='width100percent' >" + txt + "</textarea >");
            });
        }

        function CancelUpdate(obj,idRow) {
            var $row = $(obj).closest("tr");
            if ($('#ContentPlaceHolder1_HD_EDITALL').val() == "1") {
                var $tds = $row.find("td").not(':first').not(":nth-child(2)").not(":last");
            } else {
                if ($('#ContentPlaceHolder1_HD_EditLOCALVALUE').val() == "1")  var $tds = $row.find("td:nth-child(5)");
            }
            $.each($tds, function (i, el) {
                var txt = $(this).text();
                $(this).html("").append(txt);
            });
            $row.find("td").first().html('<img id="'+idRow+'"  src="Images/Edit.png" title="Edit Translation" class="MoreInfoImg" onclick="GetEditRow(this)"  width="20" height="20"><i class="fas fa-layer-group area-icon" title="Manage translation areas" onclick="OpenAreasWindow(this)"></i>');
        }

        function update(obj, idRow) {
            var $row = $(obj).closest("tr");
            var Edit =0;
            var DefaultValue='';
            var Comment = '';
            var CountryValue = '';
            if ($('#ContentPlaceHolder1_HD_EDITALL').val() == "1") {
                var $tds = $row.find("td").not(':first').not(":nth-child(2)").not(":last");
                Edit = 2;
                DefaultValue = $tds.eq(0).find("textarea").val()
                Comment = $tds.eq(1).find("textarea").val()
                CountryValue = $tds.eq(2).find("textarea").val()
            } else {
                if ($('#ContentPlaceHolder1_HD_EditLOCALVALUE').val() == "1") {
                    var $tds = $row.find("td:nth-child(5)");
                    Edit = 1;
                    CountryValue = $tds.eq(0).find("textarea").val()
                }
            }
            var combo = $find("<%= ddlCountry.ClientID %>");
            var SopID = combo.get_selectedItem().get_value();

            var datatosubmit = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                TN_GlobalID: idRow,
                LangIsocode: LangIsocode,
                Mode:Edit,
                DefaultValue: DefaultValue,
                Comment:Comment,                
                CountryValue: CountryValue,
                SopID:SopID
            };
            $('#ViewContainer').prepend("<div id='temporaryLoadingElement' style='width:100%;height:100%;margin-top:-105px' class='loadingBackgroundDefault' />");
            $.ajax({
                method: "POST",
                url: "B2BManagerService.svc/UpdateB2BTranslations",
                data: JSON.stringify(datatosubmit),
                contentType: "Application/json; charset=utf-8",
                dataType: "json",
                success: function (d) {
                    table.ajax.reload(function () {
                        $("#temporaryLoadingElement").remove();;
                    }, false);
                },
                failure: function (msg) {
                    alert(msg);
                },
                error: function (xhr, err) {
                    alert(xhr.responseJSON.Message);
                }
            }); 
             
        }
        </script>
    </asp:PlaceHolder>

    <style>
        .dataTables_filter {
            float: left !important;
            margin-left: 13px;
            margin-bottom: 10px;
            text-align: left;
            font-family: Electrolux_light;
            font-weight: bold;
            color: #041d4f;
        }

            .dataTables_filter input {
                float: right;
                width: 180px;
                margin-left: 82px  !important;
            }

        table.dataTable th {
           border: 1px solid #ddd !important;
        }
        table.dataTable td {
            border: 1px solid #ddd;
        }
        table.dataTable.no-footer {
            border-bottom: none !important;
            }

        .area-icon {
            font-size: 20px;
            vertical-align: top;
            color: #518cc9;
            cursor:pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="ViewContainer">
        <table class="Filters">
            <tr>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                </td>
                <td class="width180px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" DataTextField="Name" DataValueField="ID">
                    </asp:DropDownList>
                </td>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                </td>
                <td class="width180px">
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblArea" CssClass="Electrolux_light_bold Electrolux_Color">Area:</asp:Label>
                </td>
                <td class="width180px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlArea">
                    </asp:DropDownList>
                </td>

            </tr>
        </table>
        <div>
            <asp:HiddenField runat="server" ID="HD_EditLOCALVALUE" />
            <asp:HiddenField runat="server" ID="HD_EDITALL" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">

                <ContentTemplate>
                    <table class="Filters">
                        <td class="width120px">
                            <asp:Label runat="server" ID="lblLanguage" CssClass="Electrolux_light_bold Electrolux_Color">Language:</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" AutoPostBack="true" ID="ddlLanguage">
                            </asp:DropDownList>
                            <asp:HiddenField runat="server" ID="HdLanguage" />
                        </td>
                    </table>
                    <asp:Table ID="B2BTranslations" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                    <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                        <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">Field Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Default Value</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Comment</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Translation</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">Area(s)</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLanguage" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                    <asp:AsyncPostBackTrigger ControlID="ddlArea" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>
      <telerik:RadWindow ID="WindowAreas" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false"  Behaviors="Close" Width="500" Height="600px">
    </telerik:RadWindow>
</asp:Content>

