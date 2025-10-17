<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="UserGroupList.aspx.vb" Inherits="UserGroupList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <link href="CSS/UsersManagement.css" rel="stylesheet" />
    <script type="text/javascript">
        var table;
        $(document).ready(function () {

            table = $('#example').DataTable({
                "ajax": getAjaxUrl(),
                "lengthChange": false,
                "pageLength": 5,
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
                            return '<span style="text-align:center;vertical-align:top"><img src="Images/edit.png" class="cursor-pointer" title="View profile" onclick="window.parent.parent.openUserProfile(\'' + userGlobalid + '\')" width="22px" height="22px">'
                                + '<img src="Images/connect.png" ' + (((userType != 1 && userType != 2) || $("#userglobalid").val().toLowerCase() == userGlobalid.toLowerCase()) ? 'class="ImgDisabled"' : ' title="Connect as this user" class="cursor-pointer" onclick="window.parent.parent.connectAsUser(\'' + userGlobalid + '\')"') + ' width="22px" height="22px">';
                        }, width: 110
                    },
                    { "data": "U_ID", width: 90 },
                    { "data": "U_FULLNAME" },
                    { "data": "U_EMAIL" },
                    { "data": "U_LOGIN" },
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
        });

        function getAjaxUrl() {
            var url = "UsersNestedTablesDs.ashx?utype=1&groupid=" + $("#<%= hdfSelectedGroup.ClientID%>").val() + "&countries=ALL&showallselected=false"
            return url;
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ClientIDMode="Static" runat="server" ID="userglobalid" />
    <asp:HiddenField ID="hdfSelectedGroup" runat="server" />
    <asp:Panel runat="server" ID="panelVisualizeInfo">
        <table id="example" class="display" style="width: 100%">
            <thead>
                <tr>
                    <th></th>
                    <th>Actions</th>
                    <th>ID</th>
                    <th>Full Name</th>
                    <th>Email</th>
                    <th>Login</th>
                    <th>Last connected on</th>
                </tr>
            </thead>
        </table>
    </asp:Panel>
</asp:Content>

