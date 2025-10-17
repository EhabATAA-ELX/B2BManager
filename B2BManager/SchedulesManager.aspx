<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SchedulesManager.aspx.vb" Inherits="SchedulesManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">

    <link href="CSS/jquery-ui.css" rel="stylesheet" />
    <link href="Scripts/DataTables/datatables.css" rel="stylesheet" />

    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Buttons-1.5.6/js/dataTables.buttons.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Select-1.3.0/js/dataTables.select.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script>
        var table;
        var datatableSchedule;
        var scheduleTable;
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
            BindDataTable();
        });

        function LoadGrid() {
            $('#ViewContainer').append("<div id='temporaryLoadingElement' style='width:100%;height:800px;margin-top:-105px' class='loadingBackgroundDefault' />");
            var combo = $find("<%= ddlCountry.ClientID %>");
            LangIsocode = ($('#ContentPlaceHolder1_ddlLanguage').is(":visible")) ? $('#ContentPlaceHolder1_ddlLanguage').val() : $('#ContentPlaceHolder1_HdLanguage').val();
            table = $('#B2BTranslations').DataTable({
                "ajax": "B2BManagerService.svc/GetB2BTranslations?EnvironmentID=" + $('#<%= ddlEnvironment.ClientID %>').val() + "&LangIsocode=" + LangIsocode + "&AreaName=" + $('#<%= ddlArea.ClientID %>').val(),
                "bSort": true,
                "bPaginate": false,
                "oSearch": { "sSearch": "Warninginfo" },
                "columns": [
                    { width: 70 },
                    { "data": "TN_Name", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "DefaultValue", width: 140, className: "TextAlignCenter" },
                    { "data": "TN_Comment", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "CountryValue", width: 509 },
                    { "data": "Areas", width: 104, className: "TextAlignCenter" }
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
                            return EditTemplate.replaceAll("#data#", data)
                        }
                    }
                ],
                "order": [[1, "asc"]],
                "rowId": "TN_GlobalID"

            });
        }

        function BindDataTable() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BSchedules']").removeClass("DisplayNone");
            datatableSchedule = $("[id$='B2BSchedules']").DataTable({
                "lengthMenu": [5, 10],
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "GlobalID", visible: false },
                    { "data": "StartSchedule", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "EndSchedule", "autoWidth": true, className: "TextAlignCenter" }
                ],
                "columnDefs": [
                    {//action
                        className: "TextAlignCenter",
                    }
                ],
                "order": [[1, "asc"]],
                "dom": '<"floatLeft Width316px MarginTop25px DataTableCustom"f><t><"DatatableBottom"<i><pl>><"clear">'
            });
        }

     <%= GetActionRights() %> 
        //GetActionRights() add variable
        //EDIT_SCHEDULE_B2B
        //ADD_SCHEDULE_B2B
        //DELETE_SCHEDULE_B2B

        function EditRow(idRow) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                var content = this.innerHTML;
                var cssClass = "";
                //if (userRestricted) {
                //    cssClass = "InputDisabled";
                //}
                switch (index) {
                    case 0:
                        this.innerHTML = getEditButtons(idRow, index);
                        break;
                    case 1:
                        cssClass = "InputDisabled";
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 2:
                        //EDIT_SCHEDULE_B2B
                        if (!EDIT_SCHEDULE_B2B) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                }

            });
        }

        function ViewRow(idRow, Cancel) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                if (index >= 1) {
                    if (Cancel) {
                        this.innerHTML = $(this).attr("data");
                    } else {
                        this.innerHTML = $("#" + idRow + "_" + index).val();
                    }
                    //this.innerHTML = $(this).attr("data");
                } else if (index == 0) {
                    this.innerHTML = getUpdateButton(idRow, Cancel);
                }

            });
        }

        function getInput(id, index, type, text, cssClass, editable) {
            var readOnlyValue = "";
            if (!editable) {
                readOnlyValue = "readonly";
                disableSelect = " disabled=\"true\" ";
            }
            if (type == "TextBox") {
                return "<input id=\"" + id + "_" + index + "\" class=\"" + cssClass + "\" type=\"text\" " + readOnlyValue + " value=\"" + text + "\">";
            }
        }


        function getEditButtons(idRow) {

            var result = "";
            if (EDIT_SCHEDULE_B2B) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateSchedule('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getUpdateButton(idRow, Cancel) {
            var result = "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRow('" + idRow + "');return false;\">";
            var globalID = idRow.split("_")[1];

            var HasCountryValue;
            if (Cancel) {
                HasCountryValue = $("#" + globalID + "_CountryValue").attr("data") != "";
            } else {
                HasCountryValue = $("#" + idRow + "_4").val() != "";
            }

            if (DELETE_SCHEDULE_B2B && HasCountryValue) {
                var CountryBox = $find("<%= ddlCountry.ClientID %>");
                var EnvId = $("[id$='ddlEnvironment']").val();
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/delete.png\" title=\"Delete schedule\" onclick=\"Delete('" + globalID + "','" + CountryBox.get_selectedItem().get_value() + "','" + EnvId + "');\">";
            } else {
                if (DELETE_SCHEDULE_B2B) {
                    result += "<input type=\"image\"   class=\"width20px ImgDisabled\" src=\"./Images/delete.png\" title=\"Delete schedule\"\">";
                }
            }

            return result;
        }

        function updateSchedule(idRow) {
            ShowLoadingPanel();
            var url = "SchedulesManager.aspx/UpdateLine";
            var UserData = getDataRow(idRow);
            $.ajax({
                method: "POST",
                url: url,
                data: JSON.stringify(UserData),
                contentType: "Application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    HideLoadingPanel();
                    var globalID = idRow.split("_")[1];
                    $("#" + globalID + "_CountryValue").attr("data", $("#" + idRow + "_4").val());
                    ViewRow(idRow, false);
                    RefreshTable();
                },
                failure: function (msg) {
                    HideLoadingPanel();
                    alert(msg);
                },
                error: function (xhr, err) {
                    HideLoadingPanel();
                    alert(xhr.responseJSON.Message);
                }
            });
        }

        function Delete(GlobalID, SopId, EnvironmentID) {

            $("#dialog-confirm").dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                buttons: [
                    {
                        text: "Cancel",
                        class: "btn red btn-margin",
                        click: function () {
                            $(this).dialog("close");
                        }
                    },
                    {
                        text: "Confirm",
                        class: "btn green btn-margin",
                        click: function () {
                            $(this).dialog("close");
                            var url = "SchedulesManager.aspx/DeleteLine";
                            var data = {
                                EnvironmentID: EnvironmentID,
                                SopId: SopId,
                                ID: GlobalID
                            };
                            $.ajax({
                                method: "POST",
                                url: url,
                                data: JSON.stringify(data),
                                contentType: "Application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    HideLoadingPanel();
                                    var idColumn = GlobalID + "_CountryValue";
                                    $("#" + idColumn).html("");
                                    $("#" + idColumn).attr("data", "");
                                    RefreshTable();
                                },
                                failure: function (msg) {
                                    HideLoadingPanel();
                                    alert(msg);
                                },
                                error: function (xhr, err) {
                                    HideLoadingPanel();
                                    alert(xhr.responseJSON.Message);
                                }
                            });
                        }
                    }]
            });
        }


        //Convert the string receive by the textbox to convert it into a date format
        function ConvertToDateFormat(toFormat) {
            var dd = toFormat.split('/')[0];
            var mm = toFormat.split('/')[1];
            var yyyyANDhhmm = toFormat.split('/')[2];
            var yyyy = yyyyANDhhmm.split(' ')[0];
            var hhmm = yyyyANDhhmm.split(' ')[1];
            var dateFormat = yyyy + '-' + mm + '-' + dd + ' ' + hhmm;
            return dateFormat;
        }

        function getDataRow(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                ID: idRow.split('_')[1],
                StartSchedule: ConvertToDateFormat($("#" + idRow + "_1").val()),
                EndSchedule: ConvertToDateFormat($("#" + idRow + "_2").val()),
            };
            return data;
        }

        function ShowLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").removeClass("hidden");
        }

        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

        function AddSchedulePopup() {
            var oWnd = $find("<%= WindowActionProfile.ClientID %>");
            //oWnd.setUrl(url + "&HideHeader=true");
            oWnd.show();

        }

        function CloseWindow() {
            var oWnd = $find("<%= WindowActionProfile.ClientID %>");
            if (oWnd !== null) {
                oWnd.close();
            }
        }

        function ClearPopupAdd() {
            $("[id$='ScheduleStartTxt']").val("");
            $("[id$='ScheduleEndTxt']").val("");
        }

        function RefreshTable() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=UpdatePanel1.ClientID%>';
            if (UpdatePanel != null) {
                __doPostBack(UpdatePanel, '');
            }
            //__doPostBack('RefreshJqueryDatable', '');
        }

        // This function verify is the beginning date of the schedule is before the ending date
        // If not it will replace the ending date by the beginning date
        function VerifyDate() {
            var startDate = $("#<%=ScheduleStartTxt.ClientID%>").val();
            var endDate = $("#<%=ScheduleEndTxt.ClientID %>").val();
            var startDateSchedule = new Date(startDate);
            var endDateSchedule = new Date(endDate);
            if (startDateSchedule.getTime() > endDateSchedule.getTime()) {
                $("#<%=ScheduleEndTxt.ClientID%>").val(startDate);
            }
        }


    </script>

    <asp:PlaceHolder runat="server" ID="EditScript">
        <script type="text/javascript">
            //Script for the datable translations
            function GetEditRow(obj) {
                var idRow = $(obj).attr('id');
                var $row = $(obj).closest("tr");
                $row.find("td").first().html("<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Update\" onclick=\"update(this,'" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"CancelUpdate(this,'" + idRow + "');return false;\">");
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

            function CancelUpdate(obj, idRow) {
                var $row = $(obj).closest("tr");
                if ($('#ContentPlaceHolder1_HD_EDITALL').val() == "1") {
                    var $tds = $row.find("td").not(':first').not(":nth-child(2)").not(":last");
                } else {
                    if ($('#ContentPlaceHolder1_HD_EditLOCALVALUE').val() == "1") var $tds = $row.find("td:nth-child(5)");
                }
                $.each($tds, function (i, el) {
                    var txt = $(this).text();
                    $(this).html("").append(txt);
                });
                $row.find("td").first().html('<img id="' + idRow + '"  src="Images/Edit.png" title="Edit Translation" class="MoreInfoImg" onclick="GetEditRow(this)"  width="20" height="20">');
            }

            //Prevent bug with \r or \n with the translation, could be use to prevent special caractere by replacing them with '?'
            function VerifChar(stringToVerif) {
                return stringToVerif.replace(/[\n\r]/g, "");
                //var regEx = /[\x00-\x7F]/g;
                //var stockString = stringToVerif.split("");
                //for (var i = 0; i < stringToVerif.length; i++) {
                //    if (stockString[i] != stringToVerif[i].match(regEx)) {
                //       stockString[i] = '?';
                //    }
                //}
                //return stockString.join("");
            }

            function update(obj, idRow) {
                var $row = $(obj).closest("tr");
                var Edit = 0;
                var DefaultValue = '';
                var Comment = '';
                var CountryValue = '';
                if ($('#ContentPlaceHolder1_HD_EDITALL').val() == "1") {
                    var $tds = $row.find("td").not(':first').not(":nth-child(2)").not(":last");
                    Edit = 2;
                    DefaultValue = $tds.eq(0).find("textarea").val()
                    Comment = $tds.eq(1).find("textarea").val()
                    CountryValue = $tds.eq(2).find("textarea").val()
                    CountryValue = VerifChar(CountryValue);
                } else {
                    if ($('#ContentPlaceHolder1_HD_EditLOCALVALUE').val() == "1") {
                        var $tds = $row.find("td:nth-child(5)");
                        Edit = 1;
                        CountryValue = $tds.eq(0).find("textarea").val();
                        CountryValue = VerifChar(CountryValue);
                    }
                }
                var combo = $find("<%= ddlCountry.ClientID %>");
                var SopID = combo.get_selectedItem().get_value();

                var datatosubmit = {
                    EnvironmentID: $("[id$='ddlEnvironment']").val(),
                    TN_GlobalID: idRow,
                    LangIsocode: LangIsocode,
                    Mode: Edit,
                    DefaultValue: DefaultValue,
                    Comment: Comment,
                    CountryValue: CountryValue,
                    SopID: SopID
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
            display: none;
        }

        .dataTables_info {
            display: none;
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
            cursor: pointer;
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


                <td class="width180px" style="display: none">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlArea">
                    </asp:DropDownList>
                </td>
                <td>
                    <input type="button" id="btnAddSchedules" class="btn bleu" onclick="AddSchedulePopup()" value="New Schedule" runat="server" />
                </td>
            </tr>
        </table>
        <div>
            <asp:HiddenField runat="server" ID="HD_EditLOCALVALUE" />
            <asp:HiddenField runat="server" ID="HD_EDITALL" />

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">

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
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell5" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">Field Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Default Value</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Comment</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Translation</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">Area</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLanguage" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                </Triggers>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                    <asp:Table ID="B2BSchedules" ClientIDMode="Static" CssClass="DisplayNone" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell9" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">Start Schedule</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">End Schedule</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>

                    <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New SMS Key" ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server" OnClientBeforeClose="ClearPopupAdd">
                        <ContentTemplate>
                            <div class="padding20px">
                                <table style="width: 100%">
                                    <tbody>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblScheduleStart" class="Electrolux_light_bold Electrolux_Color">Start Schedule:</span>
                                            </td>
                                            <td>
                                                <input name="ScheduleStart" id="ScheduleStartTxt" onchange="VerifyDate()" type="datetime-local" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblScheduleEnd" class="Electrolux_light_bold Electrolux_Color">End Schedule:</span>
                                            </td>
                                            <td>
                                                <input name="ScheduleEnd" id="ScheduleEndTxt" onchange="VerifyDate()" type="datetime-local" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:RequiredFieldValidator ID="ScheduleStartTxtValidator" runat="server" ErrorMessage="Schedule Start cannot be blank" ControlToValidate="ScheduleStartTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="ScheduleEndTxtValidator" runat="server" ErrorMessage="Schedule End cannot be blank" ControlToValidate="ScheduleEndTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height: 45px">
                                            <td colspan="3" align="center">
                                                <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                                                <asp:LinkButton runat="server" CssClass="btn green" ID="btnAddSchedule" OnClick="btnAdd_ClickSchedule" ValidationGroup="CustomValidatorForAddKey">Submit</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </tbody>

                                </table>
                            </div>
                        </ContentTemplate>
                    </telerik:RadWindow>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLanguage" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>

    <div id="dialog-confirm" title="Delete schedule confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>The schedule will be permanently deleted. Are you sure?</p>
    </div>

</asp:Content>

