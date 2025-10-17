<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SurveyAnswers.aspx.vb" Inherits="SurveyAnswers" %>

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
        var datatableAnswers;

        $(document).ready(function () {
            BindDataTable();
        });

        function BindDataTable() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BAnswers']").removeClass("DisplayNone");
            datatableAnswers = $("[id$='B2BAnswers']").DataTable({
                "pageLength": 5,
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "QuestionID", visible: false },
                    { "data": "Rank", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "Answer", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_ID", visible: false },
                    { "data": "TRANSLATION_QUESTIONID", visible: false },
                    { "data": "TRANSLATION_ANSWERID", visible: false },
                    { "data": "TRANSLATION_COUNTRYID", visible: false },
                    { "data": "TRANSLATION_LANGID", visible: false },
                    { "data": "TRANSLATION_TEXT", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "LANG_ISOCODE", visible: false },
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

        function EditRowAnswer(idRow) {
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
                }

            });
        }

        function EditRowAnswerTranslation(idRow) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                var content = this.innerHTML;
                var cssClass = "";
                //if (userRestricted) {
                //    cssClass = "InputDisabled";
                //}
                switch (index) {
                    case 0:
                        this.innerHTML = getEditButtonsTranslationAnswer(idRow, index);
                        break;
                    case 3:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || !TRANSLATE_SURVEY) {
                           //// cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInputQuestion(idRow, index, "TextBox", content, cssClass, true);
                        break;
                }
            });
        }

        function ViewRowAnswer(idRow, Cancel) {
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
                    this.innerHTML = getUpdateButtonAnswer(idRow, Cancel);
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

        function getEditButtonsTranslationAnswer(idRow) {

            var result = "";
            if (EDIT_SURVEY ||TRANSLATE_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateAnswerTranslation('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowAnswer('" + idRow + "',true);return false;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowAnswer('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getEditButtons(idRow) {

            var result = "";
            if (EDIT_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateAnswer('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowAnswer('" + idRow + "',true);return false;\">";
                    
            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowAnswer('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getUpdateButtonAnswer(idRow, Cancel) {
            var result = "";
            if (EDIT_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRowAnswer('" + idRow + "');return false;\">";
            }
            
            var globalID = idRow.split("_")[1];


            if (DELETE_SURVEY) {
                var CountryBox = $find("<%= ddlCountry.ClientID %>");
                var EnvId = $("[id$='ddlEnvironment']").val();
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/delete.png\" title=\"Delete answer\" onclick=\"Delete('" + globalID + "','" + CountryBox.get_selectedItem().get_value() + "','" + EnvId + "');\">";
            } 

            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"Images/CircularTools/B2BTranslations.png\" title=\"Edit translation\" onclick=\"EditRowAnswerTranslation('" + idRow + "');return false;\">";
            }

            return result;
        }

        function updateAnswer(idRow) {
            ShowLoadingPanel();
            var url = "SurveyAnswers.aspx/UpdateLineAnswer";
            var UserData = getDataRowAnswer(idRow);
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
                    ViewRowAnswer(idRow, false);
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

        function updateAnswerTranslation(idRow) {
            ShowLoadingPanel();
            var url = "SurveyAnswers.aspx/UpdateLineAnswerTranslation";
            var UserData = getDataRowAnswerTranslation(idRow);
            $.ajax({
                method: "POST",
                url: url,
                data: JSON.stringify(UserData),
                contentType: "Application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    HideLoadingPanel();
                    var globalID = idRow.split("_")[1];
                    $("#" + globalID + "_CountryValue").attr("data", $("#" + idRow + "_3").val());
                    ViewRowAnswer(idRow, false);
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

        function Delete(AnswerID, SopId, EnvironmentID) {

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
                            var url = "SurveyAnswers.aspx/DeleteLine";
                            var data = {
                                EnvironmentID: EnvironmentID,
                                SopId: SopId,
                                ID: AnswerID
                            };
                            $.ajax({
                                method: "POST",
                                url: url,
                                data: JSON.stringify(data),
                                contentType: "Application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    HideLoadingPanel();
                                    var idColumn = AnswerID + "_CountryValue";
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

        function getDataRowAnswer(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                AnswerID: idRow.split('_')[1],
                QuestionID: full_url.split('=')[1],
                Answer: $("#" + idRow + "_2").val(),
                Rank: $("#" + idRow + "_1").val()
            };
            return data;
        }

        function getDataRowAnswerTranslation(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                QuestionID: full_url.split(/\=|\&/)[1],
                AnswerID: idRow.split('_')[1],
                LANG_ISOCODE: $("#ContentPlaceHolder1_ddlLanguage").val(),
                Translation: $("#" + idRow + "_3").val()
            };
            return data;
        }

        function ShowLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").removeClass("hidden");
        }

        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

        function AddAnswerPopup() {
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
            $("[id$='AnswerTxt']").val("");
            $("[id$='RankTxt']").val("");
        }

        function RefreshTable() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=UpdatePanel1.ClientID%>';
            if (UpdatePanel != null) {
                __doPostBack(UpdatePanel, '');
            }
            //__doPostBack('RefreshJqueryDatable', '');
        }



    </script>

    <%--SCRIPT DATATABLE FOR THE QUESTIONS OF THE SURVEY--%>

    <script>
        var datatableQuestion;
        var full_url = document.URL;

        $(document).ready(function () {
            BindDataTableQuestions();
        });

        function BindDataTableQuestions() {
            HideLoadingPanel();
            $("[id$='B2BQuestionTranslation']").removeClass("DisplayNone");
            datatableQuestion = $("[id$='B2BQuestionTranslation']").DataTable({
                "bPaginate": false,
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "SurveyID", visible: false },
                    { "data": "Question", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "Rank", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "Mandatory", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_ID", visible: false },
                    { "data": "TRANSLATION_QUESTIONID", visible: false },
                    { "data": "TRANSLATION_COUNTRYID", visible: false },
                    { "data": "TRANSLATION_LANGID", visible: false },
                    { "data": "TRANSLATION_TEXT", "autoWidth": true, className: "TextAlignCenter" },
                    { "data": "LANG_ISOCODE", visible: false },
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

        function EditRowQuestion(idRow) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                var content = this.innerHTML;
                var cssClass = "";
                //if (userRestricted) {
                //    cssClass = "InputDisabled";
                //}
                switch (index) {
                    case 0:
                        this.innerHTML = getEditButtonsQuestion(idRow, index);
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
                        this.innerHTML = getInputQuestion(idRow, index, "TextBox", content, cssClass, true);
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
                        this.innerHTML = getInputQuestion(idRow, index, "TextBox", content, cssClass, true);
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
                        this.innerHTML = getInputQuestion(idRow, index, "TextBox", content, cssClass, true);
                        break;
                }
            });
        }

        function EditTranslation(idRow) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                var content = this.innerHTML;
                var cssClass = "";
                //if (userRestricted) {
                //    cssClass = "InputDisabled";
                //}
                switch (index) {
                    case 0:
                        this.innerHTML = getEditButtonsTranslation(idRow, index);
                        break;
                    case 4:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || TRANSLATE_SURVEY) {
                           //// cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInputQuestion(idRow, index, "TextBox", content, cssClass, true);
                        break;
                }
            });
        }

        function ViewRowQuestion(idRow, Cancel) {
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
                    this.innerHTML = getUpdateButtonQuestion(idRow, Cancel);
                }

            });
        }

        function getInputQuestion(id, index, type, text, cssClass, editable) {
            var readOnlyValue = "";
            if (!editable) {
                readOnlyValue = "readonly";
                disableSelect = " disabled=\"true\" ";
            }
            if (type == "TextBox") {
                return "<input id=\"" + id + "_" + index + "\" class=\"" + cssClass + "\" type=\"text\" " + readOnlyValue + " value=\"" + text + "\">";
            }
        }


        function getEditButtonsTranslation(idRow) {

            var result = "";
            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateTranslation('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return false;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getEditButtonsQuestion(idRow) {

            var result = "";
            if (EDIT_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateQuestion('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return false;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getUpdateButtonQuestion(idRow) {
            var result = "";
            if (EDIT_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRowQuestion('" + idRow + "');return false;\">";
            }
            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"Images/CircularTools/B2BTranslations.png\" title=\"Edit\" onclick=\"EditTranslation('" + idRow + "');return false;\">";
            }

            return result;
        }

        function updateTranslation(idRow) {
            ShowLoadingPanel();
            var url = "SurveyAnswers.aspx/UpdateLineQuestionTranslation";
            var UserData = getDataRowTranslation(idRow);
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
                    ViewRowQuestion(idRow, false);
                    RefreshTableQuestion();
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

        function updateQuestion(idRow) {
            ShowLoadingPanel();
            var url = "SurveyAnswers.aspx/UpdateLineQuestion";
            var UserData = getDataRowQuestion(idRow);
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
                    ViewRowQuestion(idRow, false);
                    RefreshTableQuestion();
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

        
        function getDataRowTranslation(idRow) {
             var CountryBox = $find("<%= ddlCountry.ClientID %>");
             var CountryValue = CountryBox.get_selectedItem().get_value();
             var data = {
                 EnvironmentID: $("[id$='ddlEnvironment']").val(),
                 SopId: CountryValue,
                 SurveyID: full_url.split('=')[1],
                 QuestionID: idRow.split('_')[1],
                 LANG_ISOCODE: $("#ContentPlaceHolder1_ddlLanguage").val(),
                 Translation: $("#" + idRow + "_4").val()
             };
             return data;
         }

        function getDataRowQuestion(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                ID: idRow.split('_')[1],
                SurveyID: full_url.split('=')[2],
                Question: $("#" + idRow + "_1").val(),
                Rank: $("#" + idRow + "_2").val(),
                Mandatory: $("#" + idRow + "_3").val()
            };
            return data;
        }

        function ShowLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").removeClass("hidden");
        }

        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

        function RefreshTableQuestion() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=UpdatePanel2.ClientID%>';
            if (UpdatePanel != null) {
                __doPostBack(UpdatePanel, '');
            }
            //__doPostBack('RefreshJqueryDatable', '');
        }

        function returnToSurveys() {
            window.location = "SurveyManager.aspx";
        }

        function returnToSurveyQuestions() {
            window.location="SurveyQuestions.aspx?SurveyID=" + full_url.split('=')[2];
        }


    </script>


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

                
            </tr>
        </table>
        <div>
            <asp:HiddenField runat="server" ID="HD_EditLOCALVALUE" />
            <asp:HiddenField runat="server" ID="HD_EDITALL" />

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">

                <ContentTemplate>
                    <table class="Filters">
                        <td class="width100px">
                            <asp:Label runat="server" ID="lblReturnTo" CssClass="Electrolux_light_bold Electrolux_Color">Return to:</asp:Label>
                        </td>
                        <td>
                            <input type="button" id="btnReturnToSurveys" class="btn bleu" onclick="returnToSurveys()" value="Surveys" runat="server" />
                        </td>
                        <td>
                            <input type="button" id="btnReturnToSurveyQuestions" class="btn bleu" onclick="returnToSurveyQuestions()" value="Survey&Questions" runat="server" />
                        </td>
                        <td class="width120px">
                            <asp:Label runat="server" ID="lblLanguage" CssClass="Electrolux_light_bold Electrolux_Color">Language:</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" AutoPostBack="true" ID="ddlLanguage">
                            </asp:DropDownList>
                            <asp:HiddenField runat="server" ID="HdLanguage" />
                        </td>
                    </table>

                    <asp:Table ID="B2BQuestionTranslation" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow3" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell5" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">SurveyID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell8" runat="server">Question</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell9" runat="server">Rank</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">Mandatory</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell11" runat="server">TRANSLATION_ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell12" runat="server">TRANSLATION_QUESTIONID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell13" runat="server">TRANSLATION_COUNTRYID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell14" runat="server">TRANSLATION_LANGID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell27" runat="server">Translation</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell28" runat="server">LANG_ISOCODE</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLanguage" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                </Triggers>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <td>
                        <input type="button" id="btnAddAnswers" class="btn bleu" onclick="AddAnswerPopup()" value="New answer" runat="server" />
                    </td>
                    <asp:Table ID="B2BAnswers" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">QuestionID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Rank</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Answer</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell15" runat="server">TRANSLATION_ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell16" runat="server">TRANSLATION_QUESTIONID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell17" runat="server">TRANSLATION_ANSWERID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell18" runat="server">TRANSLATION_COUNTRYID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell19" runat="server">TRANSLATION_LANGID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell20" runat="server">TRANSLATION_TEXT</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell21" runat="server">LANG_ISOCODE</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>

                    <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New Answer" ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server" OnClientBeforeClose="ClearPopupAdd">
                        <ContentTemplate>
                            <div class="padding20px">
                                <table style="width: 100%">
                                    <tbody>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblAnswer" class="Electrolux_light_bold Electrolux_Color">Answer :</span>
                                            </td>
                                            <td>
                                                <input name="Answer" id="AnswerTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblRank" class="Electrolux_light_bold Electrolux_Color">Rank :</span>
                                            </td>
                                            <td>
                                                <textarea name="Rank" id="RankTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:RequiredFieldValidator ID="AnswerTxtValidator" runat="server" ErrorMessage="Answer cannot be blank" ControlToValidate="AnswerTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddQuestion"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="RankTxtValidator" runat="server" ErrorMessage="Rank cannot be blank" ControlToValidate="RankTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddQuestion"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height: 45px">
                                            <td colspan="3" align="center">
                                                <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                                                <asp:LinkButton runat="server" CssClass="btn green" ID="btnAddAnswer" OnClick="btnAdd_ClickAnswer" ValidationGroup="CustomValidatorForAddQuestion">Submit</asp:LinkButton>
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
        <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>The question will be permanently deleted. Are you sure?</p>
    </div>

</asp:Content>