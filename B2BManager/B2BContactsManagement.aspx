<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="B2BContactsManagement.aspx.vb" Inherits="B2BContactsManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <link href="CSS/jquery-ui.css?v=1.1" rel="stylesheet" /> 
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=1.1"></script>
    <script type="text/javascript">
        var table;
        var actionsTemplate = '<%= GetActionsTemplate() %>';
        $(document).ready(function () {
            LoadData();
        });

        function LoadData() {
            var combo = $find("<%= ddlCountry.ClientID %>");
            var SopID = combo.get_selectedItem().get_value();
            var environmentID = $('#<%= ddlEnvironment.ClientID %>').val();
            table = $('#example').DataTable({
                "ajax": "B2BManagerService.svc/GetContacts?EnvironmentID=" + environmentID + "&SOPID=" + SopID,
                "bPaginate": true,
                "bInfo": true,
                "autoWidth": true,
                "iDisplayLength": 15,
                "bLengthChange": false,
                "bFilter": true,
                "columns": [
                    {
                        "data": "GlobalID", render: function (data) {
                            return actionsTemplate.replaceAll("#data#",data).replaceAll("#environmentid#",environmentID).replaceAll("#sopid#",SopID);
                        }, width: 90
                    },
                    { "data": "Name" },
                    { "data": "Type" }
                ],
                "columnDefs": [{ orderable: false, targets: [0] , className : 'actions-column'}],
                "order": [[1, "asc"]],
            });
        }

        function ShowDetails(url) {
             $("#displayInfo").removeClass().addClass("hidden");
             $("#iframeDetails").removeClass("hidden").addClass("load").attr('src', url + "&HideHeader=true");
        }

        function EditContact(cid,evnid,SopID,el) {
            ShowDetails("EbusinessContactProfile.aspx?cid=" + cid + "&envid="+ evnid + "&sopid=" + SopID);
            $(".selected-row").removeClass("selected-row");
            $($(el)[0].parentElement.parentElement.parentElement).addClass("selected-row");
            return false;
        }

        function RefreshGrid(Action) {
            switch (Action) {
                case "SubmitCreate": {
                    var oWnd = $find("<%= WindowNewContact.ClientID %>");
                    oWnd.close();
                    table.ajax.reload();
                    break;
                }
                case "Cancel": {
                    var oWnd = $find("<%= WindowNewContact.ClientID %>");
                    oWnd.close();
                }
                default: {
                    table.ajax.reload();
                    break;
                }
            }
        }        

    </script>
    <style type="text/css">
        #example_filter {
            float: left;
            margin-left: 10px;
        }

            #example_filter input {
                margin-left: 49px;
            }
        .actions-column{
            text-align:center;
        }
    </style>

<asp:PlaceHolder runat="server" ID="CreateOrDuplicateScript">
    <script type="text/javascript">
        function NewContact() {
            var combo = $find("<%= ddlCountry.ClientID %>");
            var SopID = combo.get_selectedItem().get_value();
            var environmentID = $("#ContentPlaceHolder1_ddlEnvironment").val();
            var url = 'EbusinessNewContact.aspx?envid=' + environmentID + "&sopid=" + SopID;            
            if ($(window).height() > 480 && $(window).width() > 850) {
                var oWnd = $find("<%= WindowNewContact.ClientID %>");
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.set_title('Loading...');
                oWnd.show();
            }
            else {
                popup(url, true);
            }
        }

        function DuplicateContact(cid,evnid,SopID) {
            var url = 'EbusinessNewContact.aspx?envid=' + evnid + "&sopid=" + SopID + "&cid=" + cid;            
            if ($(window).height() > 650 && $(window).width() > 850) {
                var oWnd = $find("<%= WindowNewContact.ClientID %>");
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

<asp:PlaceHolder runat="server" ID="DeleteScript">
    <script type="text/javascript">
        function CloseDeleteConfirmationWindow() {
            $("#dialog-error-info").text("");
            $('.ui-dialog-content:visible').dialog('close');
            $("#dialog-confirm-delete").dialog('close');
        }

        function DeleteContact(entity,cid, envid) {
            var contactName;
            try {
                contactName = $(entity.parentElement.parentElement.parentElement.cells[1]).text();
            }
            catch (e) {}
            $("#dialog-delete-text").html("Are you sure you want to delete the contact <b>"+ contactName +"</b>?")
            $("#dialog-error-info").text("");
            $("#<%= deleteObjectID.ClientID%>").val(cid);
            $("#<%= deleteEnvID.ClientID%>").val(envid);

            $("#btnConfirmDelete").on("click", function (event) {
                $.ajax({
                    type: 'Get',
                    url: 'B2BManagerService.svc/DeleteContactByGlobalID',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: { GlobalID: $("#<%= deleteObjectID.ClientID%>").val() , Envid: $("#<%= deleteEnvID.ClientID%>").val() },
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
                        $("#dialog-error-info").text(e.statusText);
                    }
                });
            });

            $("#dialog-confirm-delete").dialog({
                resizable: false,
                height: "auto",
                width: 350,
                modal: true,
                title: "Delete Contact Confirmation"
            });
        }
    </script>
</asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table class="Filters">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
            </td>
            <td class="width180px">
                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged" DataTextField="Name" AutoPostBack="true" DataValueField="ID">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
            </td>
            <td class="width180px">
                <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="false" ID="ddlCountry">
                </telerik:RadComboBox>
            </td>
            <td style="width: 24px">
                <asp:ImageButton runat="server" ClientIDMode="Static" ID="imageBtnRefresh" ImageUrl="Images/Reload.png" Width="24" Height="24" ToolTip="Refresh" OnClick="imageBtnRefresh_Click" />
            </td>
            <td runat="server" id="tdNewContact">
                <a class="btn bleu" onclick="NewContact()"><i class="fas fa-id-card"></i> New Contact</a>
            </td>
        </tr>
    </table>

    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="col-sm-12 col-md-5 col-lg-4">
                    <table id="example" class="display" style="width: 100%;">
                        <thead>
                            <tr>
                                <th runat="server" id="thActions">Display</th>
                                <th>Name</th>
                                <th>Type</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div class="col-sm-12 col-md-7 col-lg-8">
                    <span id="displayInfo">
                        <h2 style="color: #BDC0C4; margin-left: 10%; margin-top: 150px;">Search for the entity you want to manage and then press
                    <img src="Images/Edit.png" width="20">
                            icon to display details here</h2>
                    </span>
                    <iframe style="width: 100%; border: none; height: 800px; overflow-x: hidden; overflow-y: auto" id="iframeDetails" class="hidden"></iframe>                    
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="imageBtnRefresh" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" />
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowNewContact" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="850px" Height="480px" runat="server">
    </telerik:RadWindow>
    
    <asp:Panel runat="server" ID="dialogDeletePanel">
         <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
            <div id="dialog-delete-text" style="margin:15px"></div>        
            <table align="right">
                <tr style="text-align:left"> 
                    <td colspan="2">
                        <span id="dialog-error-info" style="color:red;height:20px">&nbsp</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <button class="btn bleu" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                        <asp:HiddenField ID="deleteObjectID" runat="server" />
                        <asp:HiddenField ID="deleteEnvID" runat="server" />
                    </td>
                    <td>
                        <button class="btn red" id="btnConfirmDelete">Confirm</button>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>

