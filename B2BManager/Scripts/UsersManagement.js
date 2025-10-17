var table;
$(document).ready(function () {

    BindGroupButtonsClickEvent();
    tree = $find("ContentPlaceHolder1_treeCountries");
    LoadGrid();
});

function LoadGrid() {
    table = $('#example').DataTable({
        "ajax": getAjaxUrl(),
        "lengthChange": false,
        "pageLength": 15,
        "columns": [
            {
                "data": "UserType", render: function (data, type, row) {
                    var imageUrl = "Images/UsersManager/activeuser.png";
                    var title = "Active user";
                    switch (data) {
                        case 0: imageUrl = "Images/UsersManager/disableduser.png"; title = "Locked user"; break;
                        case 2: imageUrl = "Images/UsersManager/nearlyexpired.png"; title = "Nearly epxired user"; break;
                        case 3: imageUrl = "Images/UsersManager/expireduser.png"; title = "Expired user"; break;
                    }
                    return '<img style="background-color: transparent;" src="' + imageUrl + '" title="' + title + '" width="28px" height="28px">';
                }, width: 30
            },
            {
                "data": "ActionsParameters", render: function (data, type, row) {
                    var params = data.split('|');
                    var userid = params[0];
                    var userGlobalid = params[1];
                    var userType = parseInt(params[2]);
                    return '<span style="text-align:center;vertical-align:top"><img src="Images/edit.png" class="cursor-pointer" title="View profile" onclick="openUserProfile(\'' + userGlobalid + '\')" width="22px" height="22px">'
                        + '<img src="Images/connect.png" ' + (((userType != 1 && userType != 2) || $("#userglobalid").val().toLowerCase() == userGlobalid.toLowerCase()) ? 'class="ImgDisabled"' : ' title="Connect as this user" class="cursor-pointer" onclick="connectAsUser(\'' + userGlobalid + '\')"') + ' width="22px" height="22px">'
                        + '<img src="Images/UsersManager/' + ((userType == 0) ? 'unlock.png' : 'lock.png') + '" title="' + ((userType == 0) ? 'Unlock user' : 'Lock user') + '" ' + (($("#userglobalid").val().toLowerCase() == userGlobalid.toLowerCase()) ? 'class="ImgDisabled"' : ' id="img_' + userGlobalid + '"  class="cursor-pointer" onclick="lockOrUnlockUser(\'' + userGlobalid + '\')"') + ' width="22px" height="22px">'
                        + '<img src="Images/delete.png" ' + (($("#userglobalid").val().toLowerCase() == userGlobalid.toLowerCase()) ? 'class="ImgDisabled"' : ' title="Delete user" class="cursor-pointer" onclick="deleteUser(\'' + userGlobalid + '\')"') + ' width="22px" height="22px"></span>';
                }, width: 110
            },
            { "data": "U_ID", width: 90 },
            { "data": "U_FULLNAME" },
            { "data": "U_EMAIL" },
            { "data": "U_LOGIN" },
            { "data": "UserStatus" },
            { "data": "U_LASTCONNECTEDON" },
        ],
        "columnDefs": [{ orderable: false, targets: [0, 1] }],
        "order": [[3, "asc"]],
        'processing': true,
        'language': {
            'loadingRecords': '&nbsp;',
            'processing': 'Loading...'
        }
    });
}

var selectedgroup = -1;
var showallselected = "false";
var tree;
function getAjaxUrl() {
    var tempTree = $find("ContentPlaceHolder1_treeCountries");

    if (tempTree != undefined) {
        tree = tempTree;
    }

    var nodes = tree.get_nodes();
    return "UsersNestedTablesDs.ashx?utype=1" + ((selectedgroup >= 0) ? "&groupid=" + selectedgroup.toString() : "") + "&countries=" + getCheckedSOPs(nodes).join('_') + "&showallselected=" + showallselected;
}

function lockOrUnlockUser(userid) {
    var img = $("#img_" + userid.toString())[0];
    img.src = "Images/ajax-loader.gif";
    $.ajax({
        type: 'Get',
        url: 'B2BManagerService.svc/LockOrUnlockUserByGlobalID',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: { GlobalID: userid },
        async: true,
        success: function (response) {
            if (response.toLowerCase() == "success") {
                Refresh();
            }
        },
        error: function (e) {
            console.log("Error  : " + e.statusText);
        }
    });
}

function connectAsUser(userid) {
    __doPostBack("ContentPlaceHolder1_UpdatePanel1", userid);
}

function EndUserCreation() {
    var oWnd = $find("UsersManagementWindow");
    oWnd.close();
    __doPostBack("ContentPlaceHolder1_UpdatePanel1", "");
}

function EndUserUpdate() {
    var oWnd = $find("UserProfileWindow");
    oWnd.close();
    __doPostBack("ContentPlaceHolder1_UpdatePanel1", "");
}

function EndGroupUpdate() {
    var oWnd = $find("UserGroupProfileWindow");
    oWnd.close();
    __doPostBack("ContentPlaceHolder1_UpdatePanel1", "");
}

function EndGroupCreation() {
    var oWnd = $find("NewGroupWindow");
    oWnd.close();
    __doPostBack("ContentPlaceHolder1_UpdatePanel1", "");
}

var selectedgroupid; var selecteduserid;
function BindGroupButtonsClickEvent() {
    $(".groupDelete").on("click", function (event) {
        selectedgroupid = $(this).attr("data-group-id");
        OpenDeleteWindow("uroup");
        $("#btnConfirmDelete").on("click", function (event) {
            $.ajax({
                type: 'Get',
                url: 'B2BManagerService.svc/DeleteGroupByID',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: { GroupID: selectedgroupid },
                async: true,
                success: function (response) {
                    console.log(response);
                    if (response.toLowerCase() == "success") {
                        CloseDeleteConfirmationWindow();
                        setTimeout(function () { __doPostBack("ContentPlaceHolder1_UpdatePanel1", ""); }, 1);
                    }
                    else {
                        $("#dialog-error-info").text(response);
                    }
                },
                error: function (e) {
                    console.log("Error  : " + e.statusText);
                }
            });
        });
        event.preventDefault();
        event.stopPropagation();
    });

    $(".groupEdit").on("click", function (event) {
        var url = 'UserGroupProfile.aspx';
        var oWnd = $find("UserGroupProfileWindow");
        oWnd.setUrl(url + "?HideHeader=true&Uid=" + $(this).attr("data-group-id").toString());
        oWnd.set_title('Loading...');
        oWnd.show();
        event.preventDefault();
        event.stopPropagation();
    });
}

function deleteUser(userid) {
    console.log("used to delete :" +  userid);
    selecteduserid = userid;
    $("#btnConfirmDelete").on("click", function (event) {
        $.ajax({
            type: 'Get',
            url: 'B2BManagerService.svc/DeleteUserByGlobalID',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: { GlobalID: selecteduserid },
            async: true,
            success: function (response) {
                console.log(response);
                if (response.toLowerCase() == "success") {
                    CloseDeleteConfirmationWindow();
                    setTimeout(function () { __doPostBack("ContentPlaceHolder1_UpdatePanel1", ""); }, 1);
                }
                else {
                    $("#dialog-error-info").text(response);
                }
            },
            error: function (e) {
                console.log("Error  : " + e.statusText);
            }
        });
    });
    OpenDeleteWindow("user");
}

function OpenDeleteWindow(type) {
    $("#dialog-delete-text").text("Are you sure you want to delete this " + type)
    $("#dialog-error-info").text("");
    $("#dialog-confirm-delete").dialog({
        resizable: false,
        height: "auto",
        width: 350,
        modal: true
    });
}

function CloseDeleteConfirmationWindow() {
    $("#dialog-error-info").text("");
    $('.ui-dialog-content:visible').dialog('close');
    $("#dialog-confirm-delete").dialog('close');
}

function Refresh() {
    BindGroupButtonsClickEvent();
    table.ajax.url(getAjaxUrl()).load();
}

function selectGroup(el, groupid) {
    selectedgroup = groupid;
    $(".groups .selected-row").removeClass("selected-row");
    $(el).addClass("selected-row");
    table.ajax.url(getAjaxUrl()).load();
}

function ApplyCountriesFilter(oneOfSelected) {

    $(".countries .selected-row").removeClass("selected-row");
    if (!oneOfSelected) {
        $("#spanOneOfSelectedCountries").addClass("selected-row");
        $("#lblInfo").text("(Showing users assigned to one of the below selected countries)");
    }
    else {
        $("#spanAllSelectedCountries").addClass("selected-row");
        $("#lblInfo").text("(Showing users assigned to all below selected countries)");
    }

    var tree = $find("ContentPlaceHolder1_treeCountries");
    var nodes = tree.get_nodes();
    switch (getCheckedSOPs(nodes).length) {
        case 0: $("#lblInfo").text("(Showing users not assigned to any country)"); break;
        case 1:
            if (!oneOfSelected) {
                $("#lblInfo").text("(Showing users assigned only to the below selected country)");
            }
            else {
                $("#lblInfo").text("(Showing users assigned to the below selected country)");
            }
            break;
    }
    showallselected = (oneOfSelected).toString();

    table.ajax.url(getAjaxUrl()).load();
}

function getCheckedSOPs(nodes) {
    var node, childCheckedNodes;
    var checkedNodes = []; // array of the checked nodes

    for (var i = 0; i < nodes.get_count(); i++) {
        node = nodes.getNode(i);
        if (node.get_checked() && !node.get_value().startsWith("C_")) {
            checkedNodes.push(node.get_value()); // add to array if checked
        }

        // to understand recursion, first you must understand recursion
        if (node.get_nodes().get_count() > 0) {
            // recursive function call
            childCheckedNodes = getCheckedSOPs(node.get_nodes());

            if (childCheckedNodes.length > 0) {
                // append array with results from recursive call
                checkedNodes = checkedNodes.concat(childCheckedNodes);
            }
        }
    }

    return checkedNodes;
}

function CreateUser() {
    var url = 'UserCreator.aspx';
    var oWnd = $find("UsersManagementWindow");
    oWnd.setUrl(url + "?HideHeader=true");
    oWnd.set_title('Loading...');
    oWnd.show();
}

function CreateGroup() {
    var url = 'UserGroupCreator.aspx';
    var oWnd = $find("NewGroupWindow");
    oWnd.setUrl(url + "?HideHeader=true");
    oWnd.set_title('Loading...');
    oWnd.show();
}

function openUserProfile(userID) {
    var url = 'UserProfile.aspx';
    var oWnd = $find("UserProfileWindow");
    oWnd.setUrl(url + "?HideHeader=true&Uid=" + userID.toString());
    oWnd.set_title('Loading...');
    oWnd.show();
}