<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessUsersList.aspx.vb" Inherits="EbusinessUsersList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/EbusinessCustomersGrid.ascx" TagPrefix="uc1" TagName="EbusinessCustomersGrid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/nested.tables.min.js"></script>
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css?v=2.1" rel="stylesheet" />
    <link href="Scripts/DataTables/SearchHighlight/css/dataTables.searchHighlight.css" rel="stylesheet" />
    <script src="Scripts/DataTables/SearchHighlight/js/jquery.highlight.js"></script>
    <script src="Scripts/CustomerManager.js?v=2.1"></script>
    <link href="CSS/CustomerManager.css?v=2.1" rel="stylesheet" />
    <link href="CSS/jquery-ui.css?v=2.1" rel="stylesheet" /> 
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=2.1"></script>
    <script type="text/javascript">
        var Forcehighlight = true;
        var companyTemplate = '<%= ClsEbusinessHelper.GetCompanyTemplate() %>';
        var userTemplate = '<%= ClsEbusinessHelper.GetUserTemplate() %>';
        var superUserTemplate = '<%= ClsEbusinessHelper.GetSuperUserTemplate() %>';
        $(document).ready(function () {
            LoadData();
            setInterval(highlightSearch, 1000);
        });

        function LoadData() {
            environmentID = <%= Request.QueryString("envid")%>;
            superUserDisplayType = 0;
            var SopID = '<%= Request.QueryString("SopID") %>';        
            var CID = '<%= Request.QueryString("CID") %>';        
            RenderGrid(CID);
        }
        function EditUser(uid, envid, el) {
            if (typeof window.parent.parent.EditUser  === "function") {
                window.parent.parent.EditUser(uid, envid, el, '1');
            }
            else {
                popup('EbusinessUserProfile.aspx?uid=' + uid + '&envid=' + envid);
            }
        }

        function DeleteUser(uid, envid, isSuperUser, entity) {
            var SopID = '<%= Request.QueryString("SopID") %>'; 
            if (typeof window.parent.parent.DeleteUser === "function") {
                window.parent.parent.DeleteUser(uid, envid, isSuperUser, entity, SopID);
            }
            else {
                DeleteUserAction(uid, envid, isSuperUser, entity,SopID);
            }
        }
    </script>
    <asp:PlaceHolder runat="server" ID="DeleteUserScript" Visible="false">
        <script type="text/javascript">
            
            function CloseDeleteConfirmationWindow() {
                $("#dialog-error-info").text("");
                $('.ui-dialog-content:visible').dialog('close');
                $("#dialog-confirm-delete").dialog('close');
            }


            function DeleteUserAction(U_Global_ID, envid, isSuperUser, entity,sopid) {
                var username = "";
                var CID = '<%= Request.QueryString("CID") %>'; 
                try {
                    username = $(entity.parentElement.parentElement.parentElement.cells[(isSuperUser ? 4 : 3)]).text();
                }
                catch { }
                $("#dialog-delete-text").html("Are you sure you want to delete the " + (isSuperUser ? "super" : "") + " user <b>" + username + "</b>?")
                $("#dialog-error-info").text("");
                $("#<%= deleteObjectID.ClientID%>").val(U_Global_ID);
                $("#<%= deleteEnvID.ClientID%>").val(envid);

                $("#btnConfirmDelete").on("click", function (event) {
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteB2BUserByGlobalID',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: {
                            GlobalID: $("#<%= deleteObjectID.ClientID%>").val(),
                            Envid: $("#<%= deleteEnvID.ClientID%>").val(),
                            Sopid: sopid,
                            IsSuperUser: isSuperUser
                        },
                        async: true,
                        success: function (response) {
                            if (response.toLowerCase() == "success") {
                                CloseDeleteConfirmationWindow();
                                RenderGrid(CID);
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

                $("#dialog-confirm-delete").dialog({
                    resizable: false,
                    height: "auto",
                    width: 350,
                    modal: true,
                    title: "Delete " + (isSuperUser ? "Super" : "") + " User confirmation"
                });
            }        
        </script>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="margin:15px">
        <uc1:EbusinessCustomersGrid runat="server" ID="EbusinessCustomersGrid" />
    </div>    
    <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
        <div id="dialog-delete-text" style="margin:15px"></div>
        <span id="dialog-error-info" style="color:red;height:20px"></span>
        <table align="right">
            <tr>
                <td>
                    <button class="btn bleu" id="btnCancel" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                    <asp:HiddenField ID="deleteObjectID" runat="server" />
                    <asp:HiddenField ID="deleteEnvID" runat="server" />
                </td>
                <td>
                    <button class="btn red" id="btnConfirmDelete">Confirm</button>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

