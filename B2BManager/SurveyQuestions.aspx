<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="SurveyQuestions.aspx.vb" Inherits="SurveyQuestions" %>

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
        var datatableSurveyTranslation;

        $(document).ready(function () {
            BindDataTable();
            BindDataTableTranslation();
        });

        function BindDataTable() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BSurvey']").removeClass("DisplayNone");
            datatableSurvey = $("[id$='B2BSurvey']").DataTable({
                "bPaginate": false,
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "Title", width: 100, className: "TextAlignCenter" },
                    { "data": "Description", width: 350, className: "TextAlignCenter" },
                    { "data": "WelcomeMsg", width: 200, className: "TextAlignCenter" },
                    { "data": "EndMsg", width: 200, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_SurveyID", visible: false  },
                    { "data": "TRANSLATION_COUNTRYID", visible: false },
                    { "data": "TRANSLATION_LANGID", visible: false },
                    { "data": "TRANSLATION_TITLE", visible: false },
                    { "data": "TRANSLATION_DESCRIPTION", visible: false },
                    { "data": "TRANSLATION_WELCOMEMSG", visible: false },
                    { "data": "TRANSLATION_ENDMSG", visible: false },
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

        function BindDataTableTranslation() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='B2BSurveyTranslation']").removeClass("DisplayNone");
            datatableSurveyTranslation = $("[id$='B2BSurveyTranslation']").DataTable({
                "bPaginate": false,
                "bSort": false,
                "columns": [
                    { width: 1, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "Title", visible: false },
                    { "data": "Description", visible: false },
                    { "data": "WelcomeMsg", visible: false },
                    { "data": "EndMsg", visible: false },
                    { "data": "TRANSLATION_SurveyID", visible: false },
                    { "data": "TRANSLATION_COUNTRYID", visible: false },
                    { "data": "TRANSLATION_LANGID", visible: false },
                    { "data": "TRANSLATION_TITLE", width: 0, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_DESCRIPTION", width: 350, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_WELCOMEMSG", width: 200, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_ENDMSG", width: 200, className: "TextAlignCenter" },
                    { "data": "LANG_ISOCODE", visible: false },
                ],
                "columnDefs": [
                    {//action
                        className: "TextAlignCenter",
                    }
                ],
                "order": [[1, "asc"]],
                "dom": '<"floatLeft Width316px DataTableCustom"f><t><"DatatableBottom"<i><pl>><"clear">',
                fnDrawCallback: function () {
                    $("#B2BSurveyTranslation thead").visible = false;
                }
            });
        }

        <%= GetActionRights() %> 
        //GetActionRights() add variable
        //EDIT_SURVEY
        //ADD_SURVEY
        //DELETE_SURVEY
        //TRANSLATE_SURVEY

        function EditRowSurvey(idRow) {
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
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
                    
            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        

        function getUpdateButton(idRow, Cancel) {
            var result = "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRowSurvey('" + idRow + "');return false;\">";
            return result;
        }



        function updateSurvey(idRow) {
            ShowLoadingPanel();
            var url = "SurveyQuestions.aspx/UpdateLineSurvey";
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

        function AddQuestionPopup() {
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
            $("[id$='AuthorTxt']").val("");
        }

        function RefreshTable() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=UpdatePanel1.ClientID%>';
            if (UpdatePanel != null) {
                __doPostBack(UpdatePanel, '');
            }
            //__doPostBack('RefreshJqueryDatable', '');
        }


        function EditTranslationSurvey(idRow) {
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
                    case 1:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || !TRANSLATE_SURVEY) {
                           //// cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 2:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || !TRANSLATE_SURVEY) {
                            ////cssClass = "InputDisabled";
                        }
                        if (content.length < 10) {
                            cssClass += " width80px";
                        } else {
                            cssClass += " width100percent";
                        }
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
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
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass, true);
                        break;
                    case 4:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || !TRANSLATE_SURVEY) {
                           //// cssClass = "InputDisabled";
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

        function ViewRowTranslation(idRow, Cancel) {
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
                    this.innerHTML = getUpdateButtonTranslation(idRow, Cancel);
                }

            });
        }

        function getEditButtonsTranslation(idRow) {

            var result = "";
            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateSurveyTranslation('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowTranslation('" + idRow + "',true);return false;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowTranslation('" + idRow + "',true);return false;\">";
            }

            return result;
        }

        function getUpdateButtonTranslation(idRow, Cancel) {
            var result = "<input type=\"image\" class=\"width20px\" src=\"Images/CircularTools/B2BTranslations.png\" title=\"Edit Translation\" onclick=\"EditTranslationSurvey('" + idRow + "');return false;\">";
            return result;
        }

        function updateSurveyTranslation(idRow) {
            ShowLoadingPanel();
            var url = "SurveyQuestions.aspx/UpdateLineSurveyTranslation";
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

        function getDataRowTranslation(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
                   var CountryValue = CountryBox.get_selectedItem().get_value();
                   var data = {
                       EnvironmentID: $("[id$='ddlEnvironment']").val(),
                       SopId: CountryValue,
                       SurveyID: full_url.split('=')[1],
                       IsoCodeLanguage: $("#ContentPlaceHolder1_ddlLanguage").val(),
                       Title: $("#" + idRow + "_1").val(),
                       Description: $("#" + idRow + "_2").val(),
                       WelcomeMsg: $("#" + idRow + "_3").val(),
                       EndMsg: $("#" + idRow + "_4").val(),
                   };
                   return data;
               }

    </script>

    <%--SCRIPT DATATABLE FOR THE QUESTIONS OF THE SURVEY--%>

    <script>
        var datatableSurvey;
        var full_url = document.URL;

        $(document).ready(function () {
            BindDataTableQuestions();
        });

        function BindDataTableQuestions() {
            HideLoadingPanel();
            $("[id$='B2BQuestions']").removeClass("DisplayNone");
            datatableSurvey = $("[id$='B2BQuestions']").DataTable({
                "columns": [
                    { width: 70, orderable: false },
                    { "data": "ID", visible: false },
                    { "data": "SurveyID", visible: false },
                    { "data": "Question", width: 200, className: "TextAlignCenter" },
                    { "data": "Rank", width: 70, className: "TextAlignCenter" },
                    { "data": "Mandatory", width: 70, className: "TextAlignCenter" },
                    { "data": "TRANSLATION_ID", visible: false },
                    { "data": "TRANSLATION_QUESTIONID", visible: false },
                    { "data": "TRANSLATION_COUNTRYID", visible: false },
                    { "data": "TRANSLATION_LANGID", visible: false },
                    { "data": "TRANSLATION_TEXT", width: 200, className: "TextAlignCenter" },
                    { "data": "LANG_ISOCODE", visible: false }
                ],
                "columnDefs": [
                    {//action
                        className: "TextAlignCenter",
                    }
                ],
                "order": [[4, "asc"]],
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

        function EditRowQuestionTranslation(idRow) {
            var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                var content = this.innerHTML;
                var cssClass = "";
                //if (userRestricted) {
                //    cssClass = "InputDisabled";
                //}
                switch (index) {
                    case 0:
                        this.innerHTML = getEditButtonsQuestionTranslation(idRow, index);
                        break;
                    case 4:
                        //EDIT_SURVEY OR TRANSLATE_SURVEY
                        if (!EDIT_SURVEY || !TRANSLATE_SURVEY) {
                            ////cssClass = "InputDisabled";
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

        function getEditButtonsQuestion(idRow) {

            var result = "";
            if (EDIT_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateQuestion('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return true;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return true;\">";
            }

            return result;
        }

        function getEditButtonsQuestionTranslation(idRow) {

            var result = "";
            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"updateQuestionTranslation('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return true;\">";

            } else {
                result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRowQuestion('" + idRow + "',true);return true;\">";
            }

            return result;
        }

        function getUpdateButtonQuestion(idRow) {
            var result = "";
            if (EDIT_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRowQuestion('" + idRow + "');return false;\">";
            }
            var questionID = idRow.split("_")[1];
            

            if (DELETE_SURVEY) {
                var CountryBox = $find("<%= ddlCountry.ClientID %>");
                var EnvId = $("[id$='ddlEnvironment']").val();
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/delete.png\" title=\"Delete question\" onclick=\"Delete('" + questionID + "','" + CountryBox.get_selectedItem().get_value() + "','" + EnvId + "');\">";
            }

            if (EDIT_SURVEY || TRANSLATE_SURVEY) {
                result += "<input type=\"image\" class=\"width20px\" src=\"Images/CircularTools/B2BTranslations.png\" title=\"Edit\" onclick=\"EditRowQuestionTranslation('" + idRow + "');return false;\">" +
                          "<input type=\"image\" class=\"width20px\" src=\"./Images/magnifyingglass.png\" title=\"Edit\" onclick=\"ViewDetail('" + idRow + "');return false;\">";
                          
            }

            return result;
        }

        function updateQuestionTranslation(idRow) {
            ShowLoadingPanel();
            var url = "SurveyQuestions.aspx/UpdateLineQuestionTranslation";
            var UserData = getDataRowQuestionTranslation(idRow);
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
            var url = "SurveyQuestions.aspx/UpdateLineQuestion";
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
                            var url = "SurveyQuestions.aspx/DeleteLine";
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
                    }]
            });
        }

        function getDataRowQuestion(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                ID: idRow.split('_')[1],
                SurveyID: full_url.split('=')[1],
                Question: $("#" + idRow + "_1").val(),
                Rank: $("#" + idRow + "_2").val(),
                Mandatory: $("#" + idRow + "_3").val()
            };
            return data;
        }

        function getDataRowQuestionTranslation(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId: CountryValue,
                QuestionID: idRow.split('_')[1],
                IsoCodeLanguage: $("#ContentPlaceHolder1_ddlLanguage").val(),
                Translation: $("#" + idRow + "_4").val(),
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
            $("[id$='AuthorTxt']").val("");
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

        function ViewDetail(idRow) {
            window.location = "SurveyAnswers.aspx?QuestionID=" + idRow.split("_")[1] + "&SurveyID=" + full_url.split('=')[1];
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

            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">

                <ContentTemplate>
                    <table class="Filters">
                        <td class="width100px">
                            <asp:Label runat="server" ID="lblReturnTo" CssClass="Electrolux_light_bold Electrolux_Color">Return to:</asp:Label>
                        </td>
                        <td>
                            <input type="button" id="btnReturnToSurveys" class="btn bleu" onclick="returnToSurveys()" value="Surveys" runat="server" />
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
                    <asp:Table ID="B2BSurvey" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Title</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Description</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">WelcomeMsg</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">EndMsg</asp:TableHeaderCell>  
                        </asp:TableHeaderRow>
                    </asp:Table>

                    <asp:Table ID="B2BSurveyTranslation" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow3" runat="server"  TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell21" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell13" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell16" runat="server">Title</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell17" runat="server">Description</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell22" runat="server">WelcomeMsg</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell23" runat="server">EndMsg</asp:TableHeaderCell> 
                            <asp:TableHeaderCell ID="TableHeaderCell24" runat="server">TRANSLATION_SurveyID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell25" runat="server">TRANSLATION_COUNTRYID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell26" runat="server">TRANSLATION_LANGID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell27" runat="server">TRANSLATION_Title</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell28" runat="server">TRANSLATION_Description</asp:TableHeaderCell> 
                            <asp:TableHeaderCell ID="TableHeaderCell29" runat="server">TRANSLATION_WelcomeMsg</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell30" runat="server">TRANSLATION_EndMsg</asp:TableHeaderCell>               
                            <asp:TableHeaderCell ID="TableHeaderCell31" runat="server">LANG_ISOCODE</asp:TableHeaderCell>  
                        </asp:TableHeaderRow>
                    </asp:Table>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLanguage" />
                    <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
                </Triggers>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <td>
                        <input type="button" id="btnAddQuestions" class="btn bleu" onclick="AddQuestionPopup()" value="New question" runat="server" />
                    </td>
                    <asp:Table ID="B2BQuestions" ClientIDMode="Static" runat="server" Style="border-collapse: collapse !important; font-size: 12px;">
                        <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" TableSection="TableHeader">
                            <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell15" runat="server">Actions</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell11" runat="server">SurveyID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell18" runat="server">Question</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell19" runat="server">Rank</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell20" runat="server">Mandatory</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell14" runat="server">TRANSLATION_ID</asp:TableHeaderCell>  
                            <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">TRANSLATION_QUESTIONID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">TRANSLATION_COUNTRYID</asp:TableHeaderCell>  
                            <asp:TableHeaderCell ID="TableHeaderCell8" runat="server">TRANSLATION_LANGID</asp:TableHeaderCell>
                            <asp:TableHeaderCell ID="TableHeaderCell9" runat="server">TRANSLATION_TEXT</asp:TableHeaderCell>  
                            <asp:TableHeaderCell ID="TableHeaderCell12" runat="server">LANG_ISOCODE</asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                    </asp:Table>

                    <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New Question" ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server" OnClientBeforeClose="ClearPopupAdd">
                        <ContentTemplate>
                            <div class="padding20px">
                                <table style="width: 100%">
                                    <tbody>
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblQuestion" class="Electrolux_light_bold Electrolux_Color">Question :</span>
                                            </td>
                                            <td>
                                                <input name="Question" id="QuestionTxt" type="text" class="Electrolux_Color" style="width: 400px;" runat="server" />
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
                                        <tr style="height: 24px">
                                            <td>
                                                <span id="lblMandatory" class="Electrolux_light_bold Electrolux_Color">Mandatory :</span>
                                            </td>
                                            <td>
                                                <input name="Mandatory" id="DeployedBool" type="checkbox" class="Electrolux_Color" style="width: 400px;" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:RequiredFieldValidator ID="QuestionTxtValidator" runat="server" ErrorMessage="Question cannot be blank" ControlToValidate="QuestionTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddQuestion"></asp:RequiredFieldValidator>
                                                <br />
                                                <asp:RequiredFieldValidator ID="RankTxtValidator" runat="server" ErrorMessage="Rank cannot be blank" ControlToValidate="RankTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddQuestion"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr style="height: 45px">
                                            <td colspan="3" align="center">
                                                <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                                                <asp:LinkButton runat="server" CssClass="btn green" ID="btnAddQuestion" OnClick="btnAdd_ClickQuestion" ValidationGroup="CustomValidatorForAddQuestion">Submit</asp:LinkButton>
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

    <div id="dialog-confirm" title="Delete question confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>The question will be permanently deleted. Are you sure?</p>
    </div>

</asp:Content>