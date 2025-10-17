<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" EnableViewState="true" EnableEventValidation="false" CodeFile="CustomersManager.aspx.vb" Inherits="CustomersManager" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/EbusinessCustomersGrid.ascx" TagPrefix="uc1" TagName="EbusinessCustomersGrid" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/nested.tables.min.js"></script>
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css?v=2.1" rel="stylesheet" />
    <link href="Scripts/DataTables/SearchHighlight/css/dataTables.searchHighlight.css?v=2.1" rel="stylesheet" />
    <script src="Scripts/DataTables/SearchHighlight/js/jquery.highlight.js?v=2.1"></script>
    <script src="Scripts/CustomerManager.js?v=2.1"></script>
    <link href="CSS/CustomerManager.css?v=2.1" rel="stylesheet" />
    <link href="CSS/jquery-ui.css?v=2.1" rel="stylesheet" /> 
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=2.1"></script>
    <style type="text/css">
        #gridViewContainertab_0_filter {
            width: 310px;
        }
    </style>
    <script type="text/javascript">        
        sortingColumn = '<%= GetSortingColumn()%>';
        isAscendingSotring = <%= IsAscendingSotring()%>;
        var companyTemplate = '<%= ClsEbusinessHelper.GetCompanyTemplate() %>';
        var userTemplate = '<%= ClsEbusinessHelper.GetUserTemplate() %>';
        var superUserTemplate = '<%= ClsEbusinessHelper.GetSuperUserTemplate() %>';
        
        function ShowDetails(url,Mode) {
            if ($("#ContentPlaceHolder1_chkBoxDisplayMode")[0].checked || Mode == '1' ) {
                popup(url, true);
            }
            else {
                $("#iframeContainer").removeClass().addClass("hidden");
                $("#iframeDetails").removeClass("hidden").addClass("load").attr('src', url + "&HideHeader=true");
            }
        }         

        function ShowAndRefreshGridUpdate(Action) {
            if (Action == 'SubmitCreate') {
                var oWnd = $find("<%= WindowNewCustomerAccount.ClientID %>");
                oWnd.close();
            } else {
                var oWnd = $find("<%= WindowEntityProfile.ClientID %>");
                oWnd.close();
            }
            if (Action.includes('Submit')) {
                $("#<%= imageBtnRefresh.ClientID%>").click();
            }
            else {
                var oWnd = $find("<%= WindowNewCustomerAccount.ClientID %>");
                oWnd.close();
            }
        }
        function ShowAndRefreshGridCreateUser(Action) {
            var oWnd = $find("<%= WindowNewUserAccount.ClientID %>");
            oWnd.close();
            if (Action == 'Submit') {
                $("#<%= imageBtnRefresh.ClientID%>").click();
            }
        }
        function ManagePreferences() {
            var oWnd = $find("<%= WindowPreferences.ClientID %>");
            oWnd.setUrl("EbusinessChangePreferences.aspx?HideHeader=true");
            oWnd.set_title('Loading...');
            oWnd.show();
        }

        function CloseChangePreferencesWindow() {
            var oWnd = $find("<%= WindowPreferences.ClientID %>");
            oWnd.close();
        }

        function EditCustomer(cid, envid,el) {
            ShowDetails("EbusinessCompanyProfile.aspx?cid=" + cid + "&envid=" + envid);            
            $(".selected-row").removeClass("selected-row");
            if (!$("#ContentPlaceHolder1_chkBoxDisplayMode")[0].checked) {
                $($(el)[0].parentElement.parentElement.parentElement).addClass("selected-row");
            }
            return false;
        }

        function DisplayCustomerList(uid, envid) {
            var url = 'EbusinessSuperUserCustomerList.aspx?iscustomerdisplay=true&envid=' + envid + "&uid=" + uid;
            if ($(window).height() > 750 && $(window).width() > 900) {
                var oWnd = $find("<%= WindowSuperUserCustomerList.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.set_title('Loading...');
                oWnd.show();
            }
            else {
                popup(url, true);
            }
        }

        function EditUser(uid, envid,el,Mode) {
            ShowDetails("EbusinessUserProfile.aspx?uid=" + uid + "&envid=" + envid,Mode);
            $(".selected-row").removeClass("selected-row");
            if (!$("#ContentPlaceHolder1_chkBoxDisplayMode")[0].checked) {
                $($(el)[0].parentElement.parentElement.parentElement).addClass("selected-row");
            }
            return false;
        }

        function CloseDeleteConfirmationWindow() {
            $("#dialog-error-info").text("");
            $('.ui-dialog-content:visible').dialog('close');
            $("#dialog-confirm-delete").dialog('close');
        }

        $(document).ready(function () {
            LoadData();
            if (typeof Forcehighlight !== 'undefined') {
               setInterval(highlightSearch, 1000);            
            }
        });
        
        function LoadData() {
            var combo = $find("<%= ddlCountry.ClientID %>");
            SopID = combo.get_selectedItem().get_value();            
            environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
            RenderGrid();
        }
    </script>
    <asp:PlaceHolder runat="server" ID="CreateCustomerScript" Visible="false">
        <script type="text/javascript">
            function NewCustomer() {
                var combo = $find("<%= ddlCountry.ClientID %>");
                var SopID = combo.get_selectedItem().get_value();
                environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
                var url = 'EbusinessNewCustomer.aspx?envid=' + environmentID + "&sopid=" + SopID;
                if ($(window).height() > 680 && $(window).width() > 850) {
                    var oWnd = $find("<%= WindowNewCustomerAccount.ClientID %>");
                    oWnd.setUrl(url + "&HideHeader=true");
                    oWnd.set_title('Loading...');
                    oWnd.show();
                }
                else {
                    popup(url, true);
                }
            }
            function ReloadDeletedCompany(cid,envid) {
                var url = 'EbusinessReactivateCustomer.aspx?cid=' + cid + '&envid=' + envid;
                var oWnd = $find("<%= WindowNewCustomerAccount.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.set_title('Loading...');
                oWnd.show();
            }
        </script>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="CreateUserOrSuperUserScript" Visible="false">
        <script type="text/javascript">
            function NewUser(cid) {
                var combo = $find("<%= ddlCountry.ClientID %>");
                var SopID = combo.get_selectedItem().get_value();
                environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
                var url = 'EbusinessNewUser.aspx?envid=' + environmentID + "&sopid=" + SopID;
                if (cid != undefined) {
                    url += "&cid=" + cid;
                }
                if ($(window).height() > 800 && $(window).width() > 850) {
                    var oWnd = $find("<%= WindowNewUserAccount.ClientID %>");
                    oWnd.setUrl(url + "&HideHeader=true");
                    oWnd.set_title('Loading...');
                    oWnd.show();
                }
                else {
                    popup(url, true);
                }
            }      
        </script>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="DeleteCustomerScript" Visible="false">
        <script type="text/javascript">
            function DeleteCustomer(C_Global_ID, envid, entity,sopid) {
                var customerCode = "";
                try {
                    customerCode = $(entity.parentElement.parentElement.parentElement.cells[3]).text();
                }
                catch (e) { }
                $("#dialog-delete-text").html("Are you sure you want to delete the customer <b>" + customerCode + "</b>?");
                $("#dialog-error-info").text("");
                $("#<%= deleteObjectID.ClientID%>").val(C_Global_ID);
                $("#<%= deleteEnvID.ClientID%>").val(envid);

                $("#btnConfirmDelete").on("click", function (event) {
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteB2BCustomerByGlobalID',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { GlobalID: $("#<%= deleteObjectID.ClientID%>").val(), Envid: $("#<%= deleteEnvID.ClientID%>").val(),Sopid: sopid },
                        async: true,
                        success: function (response) {
                            if (response.toLowerCase() == "success") {
                                CloseDeleteConfirmationWindow();
                                $("#<%= imageBtnRefresh.ClientID%>").click();
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
                    title: "Delete Customer Confirmation"
                });
            }
        </script>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="DeleteUserScript" Visible="false">
        <script type="text/javascript">
            function DeleteUser(U_Global_ID, envid, isSuperUser, entity,sopid) {
                var username = "";
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
                                $("#<%= imageBtnRefresh.ClientID%>").click();
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

         <script type="text/javascript">
             function ReactivateUser(U_Global_ID, envid, isSuperUser, entity, sopid) {
                 var username = "";
                 try {
                     username = $(entity.parentElement.parentElement.parentElement.cells[(isSuperUser ? 4 : 3)]).text();
                 }
                 catch { }
                 $("#dialog-delete-text").html("Are you sure you want to reactivate the " + (isSuperUser ? "super" : "") + " user <b>" + username + "</b>?")
                 $("#dialog-error-info").text("");
                 $("#<%= deleteObjectID.ClientID%>").val(U_Global_ID);
                $("#<%= deleteEnvID.ClientID%>").val(envid);

                $("#btnConfirmDelete").on("click", function (event) {
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/ReactivateUserByGlobalID',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { GlobalID: $("#<%= deleteObjectID.ClientID%>").val(), Envid: $("#<%= deleteEnvID.ClientID%>").val(),Sopid: sopid , IsSuperUser: isSuperUser },
                        async: true,
                        success: function (response) {
                            if (response.toLowerCase() == "success") {
                                CloseDeleteConfirmationWindow();
                                $("#<%= imageBtnRefresh.ClientID%>").click();
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
                     title: "Reactivate " + (isSuperUser ? "Super" : "") + " User confirmation"
                 });
             }
         </script>

    </asp:PlaceHolder>    
    <asp:PlaceHolder runat="server" ID="ReactivateUserScript" Visible="false">
       
    </asp:PlaceHolder>    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="notification-container hidden" id="NotificationMsg">
        <div class="notification notification-info outputmsg outputmsg_info outputmsg_has_text">
            <span class="btn-icon close icon-cross cursor-pointer">
                <img src="Images/Close.png" onclick="HideMsg()" /></span>
            <span class="outputmsg_text Electrolux_light_bold ng-binding">Execution time to retrive your data: </span><span id="ExcutionTime"></span>
        </div>
    </div>
    <table class="Filters" style="width: 100%;overflow-y:auto">
        <tr>
            <td style="width:100px">
                <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
            </td>
            <td class="width180px">
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" DataTextField="Name" AppendDataBoundItems="true" AutoPostBack="true" DataValueField="ID">
                </asp:DropDownList>
            </td>
            <td style="width:80px">
                <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
            </td>
            <td class="width180px">
                <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px"  OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true" ID="ddlCountry">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td style="width:80px">
                <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Display/Manage:</asp:Label>
            </td>
            <td class="width180px">
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlManagementType" OnSelectedIndexChanged="ddlManagementType_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                    <asp:ListItem Selected="True" Text="Customer & users" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Super users" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width:24px">
                <asp:ImageButton runat="server" ClientIDMode="Static" ID="imageBtnRefresh" CommandName="Refresh" ImageUrl="Images/Reload.png" Width="24" Height="24" ToolTip="Refresh" OnClick="imageBtnRefresh_Click" />
            </td>
            <td style="width:142px">                
                <span runat="server" Visible="false" id="newCustomerPnl"><a class="btn bleu" id="btnNewCustomer" onclick="NewCustomer()" ><i class="far fa-building"></i> New Customer</a></span>         
            </td>
            <td style="width:150px">
                <span runat="server" Visible="false" id="newSuperUserPnl"><a class="btn bleu" id="btnNewSuperUser" onclick="NewUser(undefined)" ><i class="fas fa-user-ninja"></i> New Super User</a></span>
            </td>
            <td style="float: right; padding-right: 30px">
                <table style="padding-top: 5px">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Expand rows on search</asp:Label>
                        </td>
                        <td>
                            <label class="switch">
                                <input runat="server" id="chkBoxExpandOnSearch" type="checkbox" />
                                <span class="slider round"></span>
                            </label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblDisplayMode" CssClass="Electrolux_light_bold Electrolux_Color">Window Mode</asp:Label>
                        </td>
                        <td>
                            <label class="switch">
                                <input runat="server" id="chkBoxDisplayMode" onchange="chkBoxDisplayModeChange()" type="checkbox" />
                                <span class="slider round"></span>
                            </label>
                        </td>
                        <td>
                            <img src="Images/Insights/preferences.png" title="Change Preferences" class="MoreInfoImg vertical-align-bottom" onclick="ManagePreferences()">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
        <ContentTemplate>
            <uc1:EbusinessCustomersGrid runat="server" ID="EbusinessCustomersGrid" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="ddlManagementType" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowNewUserAccount" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="None" Width="850px" Height="920px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowNewCustomerAccount" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="None" Width="850px" Height="680px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowEntityProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="None" Width="1120px" Height="1000px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowPreferences" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="650px" Height="535px" runat="server">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowSuperUserCustomerList" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="900px" Height="750px" runat="server">
    </telerik:RadWindow>

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

