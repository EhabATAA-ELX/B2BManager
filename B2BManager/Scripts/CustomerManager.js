var loaded = false;
var environmentID;
var SopID;
var sortingColumn = "User Name";
var isAscendingSotring = true;
var superUserDisplayType = "0";
var settings = {
    "iDisplayLength": 15,
    "bLengthChange": false,
    "bFilter": true,
    "bSort": true,
    "bInfo": true,
    "autoWidth": true,
    "columnDefs": [{
        "targets": 1,
        "render": function (url, type, full) {
            var type = "Company";
            var searchText = "";
            var isStatusColumn = false;
            var id = "";
            var expirationIndicator = "";
            if (full[0].toString().startsWith("<img")) {
                searchText = full[1].split('|')[2];
                id = full[1].split('|')[1];
                if (full[1].toString().startsWith("Ctc|")) {
                    type = "User";                   
                }
            }
            else {
                if (full[0].toString() == "") {
                    searchText = full[1].split('|')[2];
                    if (full[1].toString().startsWith("Ctc|")) {
                        type = "User";
                        id = full[1].split('|')[1];
                    }
                    else {
                        id = full[1].split('|')[1];
                    }
                }
                else {
                    if (full[0].toString().startsWith("Cpy|")) {
                        type = "Text";
                        id = full[1];
                    }
                    else {
                        type = "Text";
                        if (full[0].toString().startsWith("CpySU|")) {
                            id = full[1];
                        }
                        else {
                            expirationIndicator = parseInt(full[1]);
                            id = GetStatusColumnHTML(parseInt(full[1]))
                        }
                    }
                }
            }
            return GetActionsToolbar(id, type, searchText, expirationIndicator);
        },
        "searchable": true
    },
    {
        "targets": 0,
        "render": function (url, type, full) {
            var type = "Company";
            var searchText = "";
            var id = "";
            var expirationIndicator = full[1];
            if (full[0].toString().startsWith("Cpy|")) {
                id = full[0].split('|')[1];
            }
            else {
                if (full[0].toString().startsWith("<img")) {
                    type = "Text"
                    id = "<span class='expandIconContainer'>" + full[0] + "</span>";
                }
                else {
                    if (full[1].toString().startsWith("Cpy|") || full[1].toString().startsWith("Ctc|")) {
                        type = "Text";
                    }
                    else {
                        type = "User";
                        if (full[0].toString().startsWith("Ctc|")) {
                            type = "User";
                            id = full[0].split('|')[1];
                        }
                        else {
                            id = full[0];
                        }
                    }
                }
            }
            return GetActionsToolbar(id, type, searchText, expirationIndicator);
        },
        "searchable": true,
        "width": "90px"
    },
    {
        "targets": 2,
        "render": function (url, type, full) { 
            var type = "Text";
            var searchText = "";
            var id = full[2];
            var expirationIndicator = "";
            if (full[1].toString().startsWith("Ctc|")) {
                expirationIndicator = parseInt(full[2]);
                id = GetStatusColumnHTML(parseInt(full[2]));
            }
            return GetActionsToolbar(id, type, searchText, expirationIndicator);
        },
        "searchable": true
    }
    ]
};

function GetStatusColumnHTML(value) {
    var title = "";

    switch (value) {
        case 1: title = "Active user"; break;
        case 2: title = "Nearly expired user"; break;
        case 3: title = "Expired user"; break;
        case 4: title = "Active test user"; break;
        case 5: title = "Nearly expired test user"; break;
        case 6: title = "Expired test user"; break;
        case 7: title = "Active super user"; break;
        case 8: title = "Nearly expired super user"; break;
        case 9: title = "Expired super user"; break;
        case 10: title = "Active test super user"; break;
        case 11: title = "Nearly expired test super user"; break;
        case 12: title = "Expired test super user"; break;
        case 13: title = "Active admin user"; break;
        case 14: title = "Nearly expired admin user"; break;
        case 15: title = "Expired admin user"; break;
        case 16: title = "Active admin test user"; break;
        case 17: title = "Nearly expired admin test user"; break;
        case 18: title = "Expired admin test user"; break;
        case 19: title = "Active admin super user"; break;
        case 20: title = "Nearly expired admin super user"; break;
        case 21: title = "Expired admin super user"; break;
        case 22: title = "Active test admin super user"; break;
        case 23: title = "Nearly expired test admin super user"; break;
        case 24: title = "Expired test admin super user"; break;
    }
    return "<span style='width:65px;text-align:center;display:block'><img src=\"Images\/Ebusiness\/UserType\/" + value + ".png\" height=\"24px\" title='" + title + "' \/></span>";
}

function GetActionsToolbar(id, type, searchText, expirationIndicator) {
    var displayStyle = expirationIndicator % 3 == 0 ? "display : inline-block" : "display : none ";
    switch (type) {
        case "Text": {
            return id;
        }
        case "Company": {
            return companyTemplate.replaceAll("#id#", id).replaceAll("#environmentid#", environmentID).replaceAll("#sopid#", SopID) + ((searchText != "") ? "<span class='hidden'>" + searchText + "</span>" : "") + "</span>";
        }
        case "User": {
            return (superUserDisplayType == "0" ? userTemplate : superUserTemplate).replaceAll("#id#", id).replaceAll("#environmentid#", environmentID).replaceAll("#sopid#", SopID).replaceAll("#display#", displayStyle) + ((searchText != "") ? "<span class='hidden'>" + searchText + "</span>" : "") + "</span>";
        }
    }
}

function highlightSearch() {
    if (loaded) {
        if ($("#gridViewContainertab_0_filter input").val() != "") {
            $("#gridViewContainer").find(".table tbody td").highlight($("#gridViewContainertab_0_filter input").val());
        }
        else {
            $("#gridViewContainer").find(".table tbody td").unhighlight();
        }
        loaded = false;
    }
    else {
        if ($("#gridViewContainertab_0_filter input").val() == "") {
            $("#gridViewContainer").find(".table tbody td").unhighlight();
        }
    }
}

function SortData() {
    $("#gridViewContainertab_0 > thead > tr > th[aria-label^='" + sortingColumn + "']").trigger("click");
    if (!isAscendingSotring) {
        $("#gridViewContainertab_0 > thead > tr > th[aria-label^='" + sortingColumn + "']").trigger("click");
    }
}

function chkBoxDisplayModeChange() {
    if ($("#ContentPlaceHolder1_chkBoxDisplayMode").length > 0) {

        if ($("#ContentPlaceHolder1_chkBoxDisplayMode")[0].checked) {
            $("#gridTD").removeClass("compactModeContainer");
            $("#iframeTD").removeClass().addClass("hidden");
        }
        else {
            $("#gridTD").removeClass().addClass("compactModeContainer");
            $("#iframeTD").removeClass().addClass("compactModeContainer").addClass("minWidth830px");
            $("#iframeContainer").removeClass("hidden");
            $("#iframeDetails").addClass("hidden").attr('src', '');
        }
    }
    else {
        $("#gridTD").removeClass("compactModeContainer");
        $("#iframeTD").removeClass().addClass("hidden");
    }
}

function RenderGrid(cid) {
    msgShown = true;
    loaded = false;
    if ($("#ContentPlaceHolder1_ddlManagementType").length > 0) {
        superUserDisplayType = $("#ContentPlaceHolder1_ddlManagementType").val();
    }
    t0 = performance.now()
    inProgress = true;
    count();
    HideMsg();
    $('#gridViewContainer').html("");
    $("#iframeContainer").addClass("hidden");
    $("#iframeDetails").addClass("hidden").attr('src', '');
    $('#gridViewContainer').append("<div id='temporaryLoadingElement' style='width:100%;height:800px;margin-top:-50px' class='loadingBackgroundDefault' />");
    $.ajax({
        type: "GET",
        url: "UsersNestedTablesDs.ashx?EnvironmentID=" + environmentID + "&SopID=" + SopID + "&su=" + superUserDisplayType + ((cid != undefined) ? "&cid=" + cid : ""),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (json) {
            chkBoxDisplayModeChange();
            var table = new nestedTables.TableHierarchy("gridViewContainer", json, settings);
            table.initializeTableHierarchy();
            var tableEle = document.querySelector("#gridViewContainer .table");
            if (json.length == 0) {
                $(".dataTables_info").hide();
                $(".dataTables_paginate").hide();
                $("#gridViewContainer .table thead").hide();
                $("#gridViewContainer .table").attr("style","border-top: 1px solid #ddd;");
            }

            tableEle.addEventListener("onShowChildHierarchy", function (e) {
                loaded = true;
            });

            tableEle.addEventListener("onHideChildHierarchy", function (e) {
                loaded = true;
            });

            $(".dataTables_filter input").on('keyup', function () {
                if ($("#ContentPlaceHolder1_chkBoxExpandOnSearch").length > 0) {
                    if ($("#ContentPlaceHolder1_chkBoxExpandOnSearch")[0].checked) {
                        if ($("#gridViewContainertab_0_filter input").val() != undefined && $("#gridViewContainertab_0_filter input").val() != "") {
                            $($(document.querySelector("#gridViewContainer .table"))[0].rows).find('td:first-child span.expandIconContainer img:not(.rotate-down)').trigger('click');
                        }
                        else {
                            $($(document.querySelector("#gridViewContainer .table"))[0].rows).find('td:first-child span.expandIconContainer img.rotate-down').trigger('click');
                        }
                    }
                    else {
                        $($(document.querySelector("#gridViewContainer .table"))[0].rows).find('td:first-child span.expandIconContainer img.rotate-down').trigger('click');
                    }
                }
                if (typeof Forcehighlight !== 'undefined') {
                    $(this).closest("div").parent().find(".table tbody td").unhighlight();
                    $(this).closest("div").parent().find(".table tbody td").highlight($(this).val());
                }
            })
            SortData();
            $(".buttonsContainer").parent().on("click", function (event) {
                event.preventDefault();
                event.stopPropagation();
            });

            inProgress = false;
            var t1 = performance.now();
            var elapsedTime = parseFloat((t1 - t0) / 1e3).toFixed(3);
            if (elapsedTime > 10) {
                ShowMsg();
                setTimeout(HideMsg, 5000);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log(errorThrown);
            $("#gridViewContainer").removeClass("loadingBackgroundDefault");
            inProgress = false;
            var t1 = performance.now();
            var elapsedTime = parseFloat((t1 - t0) / 1e3).toFixed(3);
            if (elapsedTime > 10) {
                ShowMsg();
                setTimeout(HideMsg, 5000);
            }
        }
    });
}