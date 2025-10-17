<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SurveyManager.aspx.vb" Inherits="SurveyManager" %>

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
        var datatableSurvey;

        $(document).ready(function () {
            BindDataTable();
        });

        function BindDataTableDetail() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BSurveys']").removeClass("DisplayNone");
            datatableSurveyDetail = datatableSurvey
            
        }

        function BindDataTable() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BSurveys']").removeClass("DisplayNone");
            datatableSurvey = $("[id$='B2BSurveys']").DataTable({
                "lengthMenu": [20, 40],
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "Title", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "Description", width: 350, className: "TextAlignCenter" },
                    { "data": "WelcomeMsg", width: 200, className: "TextAlignCenter" },
                    { "data": "EndMsg", width: 200, className: "TextAlignCenter" },
                    { "data": "StartDate", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "EndDate", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "Deployed", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "CreationDate", visible: false },
                    { "data": "UpdateDate", visible: false },
                    { "data": "Author", visible: false },
                    { "data": "IsDeleted", visible: false },
                ],
                "columnDefs": [
                    {//action
                        className: "TextAlignCenter",
                    }
                ],
                "order": [[1, "asc"]],
                "dom": '<"floatLeft Width316px DataTableCustom"f><t><"DatatableBottom"<i><pl>><"clear">'
            });
        }

        <%= GetActionRights() %> 
        //GetActionRights() add variable
        //EDIT_SURVEY
        //ADD_SURVEY
        //DELETE_SURVEY
        //TRANSLATE_SURVEY

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
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 2:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 3:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 4:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 5:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 6:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
                            cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 7:
                        //EDIT_SURVEY
                        if (!EDIT_SURVEY) {
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


        function ViewDetail(idRow) {
            window.location = "SurveyQuestions.aspx?SurveyID="+idRow.split("_")[1];
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
            if (EDIT_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateSurvey('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return true;\">";
            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return true;\">";
            }

            return result;
        }

        function getUpdateButton(idRow) {
            var result = "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRow('" + idRow + "');return false;\">";
            var globalID = idRow.split("_")[1];

            if (DELETE_SURVEY) {
                var CountryBox = $find("<%= ddlCountry.ClientID %>");
                var EnvId = $("[id$='ddlEnvironment']").val();
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/delete.png\" title=\"Delete survey\" onclick=\"Delete('" + globalID + "','" + CountryBox.get_selectedItem().get_value() + "','" + EnvId + "');\">";
            } else {
                    result += "<input type=\"image\"   class=\"width20px ImgDisabled\" src=\"./Images/delete.png\" title=\"Delete survey\"\">";
            }

            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"./Images/magnifyingglass.png\" title=\"View Detail\" onclick=\"ViewDetail('" + idRow + "')\">";
            }

            return result;
        }

        function updateSurvey(idRow) {
            ShowLoadingPanel();
            var url = "SurveyManager.aspx/UpdateLine";
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
                            var url = "SurveyManager.aspx/DeleteLine";
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


        //Convert the string receive by the textbox to convert it into a correct date format
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
                Title: $("#" + idRow + "_1").val(),
                Description: $("#" + idRow + "_2").val(),
                WelcomeMsg: $("#" + idRow + "_3").val(),
                EndMsg: $("#" + idRow + "_4").val(),
                StartDate: ConvertToDateFormat($("#" + idRow + "_5").val()),
                EndDate: ConvertToDateFormat($("#" + idRow + "_6").val()),
                Deployed: $("#" + idRow + "_7").val()
            };
            return data;
        }

        function ShowLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").removeClass("hidden");
        }

        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

        function AddSurveyPopup() {
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
            $("[id$='TitleTxt']").val("");
            $("[id$='DescriptionTxt']").val("");
            $("[id$='WelcomeMsgTxt']").val("");
            $("[id$='EndMsgTxt']").val("");
            $("[id$='StartDateTxt']").val("");
            $("[id$='EndDateTxt']").val("");
            $("[id$='DeployedBool']").val("");
        }

        function RefreshTable() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=UpdatePanel1.ClientID%>';
            if (UpdatePanel != null) {
                __doPostBack(UpdatePanel, '');
            }
            //__doPostBack('RefreshJqueryDatable', '');
        }

        // This function verify is the beginning date of the survey is before the ending date
        // If not it will replace the ending date by the beginning date
        function VerifyDate() {
            var startDate = $("#<%=StartDateTxt.ClientID%>").val();
            var endDate = $("#<%=EndDateTxt.ClientID %>").val();
            var startDateSurvey = new Date(startDate);
            var endDateSurvey = new Date(endDate);
            if (startDateSurvey.getTime() > endDateSurvey.getTime()) {
                $("#<%=EndDateTxt.ClientID%>").val(startDate);
            }
        }

        $('input[type="checkbox"]').change(function VerifyCheck() {
            this.value = (Number(this.checked));
        });

    </script>

    <style>
        .dataTables_filter label {
            display: block;
            float: left;
        }

            .dataTables_filter label input {
                font-weight: normal;
                width: 230px;
                margin-left: 37px !important;
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
                <td>
                    <input type="button" id="btnAddSurveys" class="btn bleu" onclick="AddSurveyPopup()" value="New Survey" runat="server" />
                </td>
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
            </tr>
        </table>
        <div>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Table ID="B2BSurveys" ClientIDMode="Static" CssClass="DisplayNone" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Title</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Description</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">WelcomeMsg</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">EndMsg</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">StartDate</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">EndDate</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell8" runat="server">Deployed</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell9" runat="server">CreationDate</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">UpdateDate</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell11" runat="server">Author</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell12" runat="server">IsDeleted</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>

                    <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New SMS Key" ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server" OnClientBeforeClose="ClearPopupAdd">
                        <ContentTemplate>
                            <div class="padding20px">
                                <table style="width: 100%">
                                    <tbody>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblTitle" class="Electrolux_light_bold Electrolux_Color">Title :</span>
                                            </td>
                                            <td>
                                                <input name="Title" id="TitleTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblDescription" class="Electrolux_light_bold Electrolux_Color">Description :</span>
                                            </td>
                                            <td>
                                                <textarea name="Description" id="DescriptionTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblWelcomeMsg" class="Electrolux_light_bold Electrolux_Color">WelcomeMsg :</span>
                                            </td>
                                            <td>
                                                <textarea name="WelcomeMsg" id="WelcomeMsgTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblEndMsg" class="Electrolux_light_bold Electrolux_Color">EndMsg :</span>
                                            </td>
                                            <td>
                                                <textarea name="EndMsg" id="EndMsgTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblStartDate" class="Electrolux_light_bold Electrolux_Color">Start Date:</span>
                                            </td>
                                            <td>
                                                <input name="StartDate" id="StartDateTxt" onchange="VerifyDate()" type="datetime-local" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblEndDate" class="Electrolux_light_bold Electrolux_Color">End Date:</span>
                                            </td>
                                            <td>
                                                <input name="EndDate" id="EndDateTxt" onchange="VerifyDate()" type="datetime-local" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblDeployed" class="Electrolux_light_bold Electrolux_Color">Deployed :</span>
                                            </td>
                                            <td>
                                                <input name="Deployed" id="DeployedBool" type="checkbox" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td colspan="3">
                                                <asp:RequiredFieldValidator ID="TitleTxtValidator" runat="server" ErrorMessage="Title cannot be blank" ControlToValidate="TitleTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="DescriptionTxtValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="WelcomeMsgTxtValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="EndMsgTxtValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="StartDateTxtValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="EndDateTxtValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="DeployedBoolValidator" runat="server" ErrorMessage="Description cannot be blank" ControlToValidate="DescriptionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height: 45px">
                                            <td colspan="3" align="center">
                                                <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                                                <asp:LinkButton runat="server" CssClass="btn green" ID="btnAddSchedule" OnClick="btnAdd_ClickSurvey" ValidationGroup="CustomValidatorForAddKey">Submit</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </tbody>

                                </table>
                            </div>
                        </ContentTemplate>
                    </telerik:RadWindow>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                </Triggers>
            </asp:UpdatePanel>

        </div>
    </div>

    <div id="dialog-confirm" title="Delete survey confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>The survey will be permanently deleted. Are you sure?</p>
    </div>

    

</asp:Content>

