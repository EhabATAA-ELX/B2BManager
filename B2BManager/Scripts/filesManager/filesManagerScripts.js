(function () {

    document.addEventListener("DOMContentLoaded", function () {
        var input = document.getElementById("FileUploadSW");
        if (input) {
            input.addEventListener("change", function () {
                UploadFile(input);
            });
        }
    });


    window.onClientContextMenuShowing = function (sender, args) {
        var treeNode = args.get_node();

        treeNode.set_selected(true);
        // enable/disable menu items
        setMenuItemsState(args.get_menu().get_items(), treeNode);
    };

    window.onClientContextMenuItemClicking = function (sender, args) {
        var menuItem = args.get_menuItem();
        var treeNode = args.get_node();

        menuItem.get_menu().hide();

        switch (menuItem.get_value()) {
            case 'Rename':
                treeNode.startEdit();
                break;
            case 'NewFolder':
                break;
            case 'Delete':
                var result = confirm('Are you sure you want to delete the folder: ' + treeNode.get_text());

                args.set_cancel(!result);
                break;
        }
    };

    // this method disables the appropriate context menu items
    function setMenuItemsState(menuItems, treeNode) {
        for (var i = 0; i < menuItems.get_count(); i++) {
            var menuItem = menuItems.getItem(i);

            switch (menuItem.get_value()) {
                case 'Rename':
                    formatMenuItem(menuItem, treeNode, 'Rename "{0}"');
                    break;
                case 'Delete':
                    formatMenuItem(menuItem, treeNode, 'Delete "{0}"');
                    break;
                case 'NewFolder':
                    if (treeNode.get_parent() === treeNode.get_treeView()) {
                        menuItem.set_enabled(false);
                    } else {
                        menuItem.set_enabled(true);
                    }

                    break;
            }
        }
    }

    // formats the Text of the menu item
    function formatMenuItem(menuItem, treeNode, formatString) {
        var nodeValue = treeNode.get_value();

        if (nodeValue && nodeValue.indexOf('_Private_') === 0) {
            menuItem.set_enabled(false);
        } else {
            menuItem.set_enabled(true);
        }

        var newText = String.format(formatString, extractTitleWithoutMails(treeNode));

        menuItem.set_text(newText);
    }

    // removes the brackets with the numbers,e.g. Inbox (30)
    function extractTitleWithoutMails(treeNode) {
        return treeNode.get_text().replace(/\s*\([\d]+\)\s*/ig, '');
    }

})();


function ReNameFolder() {
    $telerik.$("[id$='FileManagementRadTreeView']")[0].control.get_selectedNode().startEdit();
}

function DeleteFolder() {
    var treeNode = $telerik.$("[id$='FileManagementRadTreeView']")[0].control.get_selectedNode();
    return confirm('Do you want to delete the folder: ' + treeNode.get_text());
}

function DeleteFile(fileName) {
    return confirm('Do you want to delete the file ' + fileName + ' ?');
}


function UploadFile(obj) {
    if (obj.value !== '') {
        document.getElementById("ContentPlaceHolder1_FileUploadbtn").click(); 
    }
}

function FileUploadShow() {
    disableDocumentBody();
    $('#FileUploadSW').click();
}

function OnPopUpShowing(sender, args) {
    args.get_popUp().className += " popUpEditForm";
}

function PublishDateCheckBoxClick() {
    var managePublicationChecked = $('#PublishDateCheckBox')[0].checked;
    $telerik.findControl(document, 'FromDatePicker').set_enabled(managePublicationChecked);
    $telerik.findControl(document, 'ToDatePicker').set_enabled(managePublicationChecked);
    if (!managePublicationChecked) {
        $telerik.findControl(document, 'FromDatePicker').clear();
        $telerik.findControl(document, 'ToDatePicker').clear();
    }
}

function ManagePublicationBtnOkClientClick() {
    var dateFrom = $telerik.findControl(document, 'FromDatePicker').get_selectedDate();
    var dateTo = $telerik.findControl(document, 'ToDatePicker').get_selectedDate();

    if ($('#PublishDateCheckBox')[0].checked && (dateFrom === null || dateTo === null)) {
        $('#ManagePublicationErrorLabel').text("Please enter the dates.");
        $('#ManagePublicationErrorLabel').css("color", "red")
        return false;
    }
    else if ($('#PublishDateCheckBox')[0].checked && dateFrom >= dateTo) {
        $('#ManagePublicationErrorLabel').text("''Date from'' must be smaller than ''Date to''");
        $('#ManagePublicationErrorLabel').css("color", "red")
        return false;
    }
    else {
        return true;
    }
}

function ThumbnailUploadShow() {
    $('#ThumbnailUpload').click();
}

function ThumbnailUploadChange(obj) {
    if (obj.value !== '') {
        $('#ThumbnailUploadBtn').click();
    }
}

function FMRequestStart(sender, args) {
    if (args.EventTarget.includes("OKbtn")) {
        $('*[id$="FileManagerPanel"]').hide();
        $('*[id$="ManageSecurityPanel"]').hide();
    }
    else if (args.EventTarget.includes("UnsecuredToSecuredBtn")) {
        $('*[id$="UnsecuredToSecuredBtn"]').addClass("loadingBackground").prop('disabled', true);
        $('*[id$="SecuredRadTreeView"]').hide();
    }
    if (args.get_eventTarget().indexOf("DownloadFileLinkBtn") >= 0) {
        args.set_enableAjax(false);
    }

    disableDocumentBody();
}
function FMResponseEnd(sender, eventArgs) {
    enableDocumentBody();
}

function disableDocumentBody() {
    $("body").append("<div id='block'></div>");
    document.documentElement.style.cursor = 'wait';
}
function enableDocumentBody() {
    $("#block").remove();
    document.documentElement.style.cursor = '';
}

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
    var selectedCount = $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.selected-row").length;
    if (selectedCount > 0) {
        $("#" + ((type === 0) ? "" : "un") + "assignedSelected").html(selectedCount.toString() + " selected");
        $("#btn" + ((type === 1) ? "" : "Un") + "AssignSelection").removeClass("btn-disabled");
        $("#btn" + ((type === 1) ? "" : "Un") + "AssignSelection").attr("onclick", "ProcessButton(this,'Executing...');ManageSelection(" + type.toString() + ",this)");
    }
    else {
        $("#" + ((type === 0) ? "" : "un") + "assignedSelected").html("");
        $("#btn" + ((type === 1) ? "" : "Un") + "AssignSelection").addClass("btn-disabled");
        $("#btn" + ((type === 1) ? "" : "Un") + "AssignSelection").removeAttr("onclick");
        $("." + ((type === 0) ? "" : "un") + "assignedDisplayDD").val("All");
    }
    DisplaySelection($("." + ((type === 0) ? "" : "un") + "assignedDisplayDD").val(), type);
}

function DisplaySelection(selectionType, type) {
    switch (selectionType) {
        case "Selected":
            if ($(".filtred-search").length > 0) {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.filtred-search:not(.selected-row)").addClass("hidden").hide();
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.filtred-search.selected-row").removeClass("hidden").show();
            }
            else {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.selected-row)").addClass("hidden").hide();
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.selected-row").removeClass("hidden").show();
            }
            break;
        case "Unselected":
            if ($(".filtred-search").length > 0) {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.filtred-search.selected-row").addClass("hidden").hide();
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.filtred-search:not(.selected-row)").removeClass("hidden").show();
            }
            else {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.selected-row").addClass("hidden").hide();
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.selected-row)").removeClass("hidden").show();
            }
            break;
        default:
            if ($(".filtred-search").length > 0) {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.filtred-search").removeClass("hidden").show();
            }
            else {
                $(".list-" + ((type === 0) ? "" : "un") + "assigned-row").removeClass("hidden").show();
            }
            break;
    }
}

function SelectRows(select, type) {
    if (select) {
        $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.hidden)").addClass("selected-row");
        $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.hidden)").find(".checkedTickBox").removeClass("hidden");
    }
    else {
        $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.hidden)").removeClass("selected-row");
        $(".list-" + ((type === 0) ? "" : "un") + "assigned-row:not(.hidden)").find(".checkedTickBox").addClass("hidden");
    }
    CalculateSelected(type);
}

function ConfirmAssignment() {
    $.magnificPopup.open({
        items: {
            src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer list updated with success.</p></div>',
            type: 'inline'
        }
    });
}

// TODO méthode à supprimer ?
function ManageSelection(type, btn) {
    disableDocumentBody();

    var companyIds = [];
    var mapGrid = $(".list-" + ((type === 0) ? "" : "un") + "assigned-row.selected-row");
    mapGrid.each(function (i, el) {
        companyIds.push($(el).attr("data-cid"));
    });
    var data = {
        Type: type,
        CompanyIds: companyIds.join(';'),
        CountryId: $("#CountryGuidLabel").text(),
        DocumentIds: $("#DocumentGuidLabel").text(),
        EnvironmentId: $("#EnvironmentIDLabel").text()
    };
    var dataLog = {
        AssignmentTypeId: data.Type,
        EnvironmentId: data.EnvironmentId,
        CountryId: data.CountryId,
        ObjectTypeId: 1,
        ObjectIds: data.DocumentIds,
        CustomerIds: data.CompanyIds
    };

    $.ajax({
        type: 'POST',
        url: 'FilesManager.aspx/AssignedCompagnies',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(data),
        proccessData: false,
        async: true,
        success: function (response) {
            if (response.d.toLowerCase() === "success") {
                let sendNotification = $("#SendNotificationLabel").text();
                if (sendNotification === "True") {
                    AssignmentLogs(dataLog);
                }
                $(btn).html('Refreshing lists...');
                __doPostBack('<%= RefreshHD.ClientID %>', 'ConfirmAssignment');
            }
            else {
                $(btn).html('<i class="' + $(btn).attr("data-button-icon") + '"></i> ' + $(btn).attr("data-button-text"));
                ErrorPopup(response);
            }
            enableDocumentBody();
        },
        error: function (e) {
            $(btn).html('<i class="' + $(btn).attr("data-button-icon") + '"></i> ' + $(btn).attr("data-button-text"));
            ErrorPopup("Error  : " + e.statusText);
            enableDocumentBody();
        }
    });
}

function searchInCustomerList() {
    var textSearch = $("#searchInCustomerListTextBox")[0].value.toString().toLowerCase();

    $("tr.list-row").removeClass("hidden").removeClass("filtred-search").addClass("display-inline");
    if (textSearch !== "") {
        $("tr.list-row").addClass("hidden").removeClass("display-inline");
        $("tr.list-row[data-search*='" + textSearch + "']").removeClass("hidden").addClass("filtred-search");
    }

    if (textSearch.length === 0) {
        DisplaySelection($(".assignedDisplayDD").val(), 0);
        DisplaySelection($(".unassignedDisplayDD").val(), 1);
    }
}

