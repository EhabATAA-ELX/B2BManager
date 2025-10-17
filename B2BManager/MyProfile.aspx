<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="MyProfile.aspx.vb" Inherits="MyProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/EbusinessPreferences.ascx" TagPrefix="uc1" TagName="EbusinessPreferences" %>
<%@ Register Src="~/UserControls/InsightsPreferences.ascx" TagPrefix="uc1" TagName="InsightsPreferences" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/InsightsAreas.css" rel="stylesheet" />
    <script src="Scripts/InsightsAreas.js?v=1.0"></script>
    <link href="CSS/jquery-ui.css?v=2.1" rel="stylesheet" /> 
    <script type="text/javascript" src="Scripts/jquery-ui.js?v=2.1"></script>
    <style type="text/css">
        .profile-user-img {
            height: 100px !important;
        }
    </style>
    <script type="text/javascript">
        function UploadFile(obj) {
            if (obj.value != '') {
                var length = obj.files[0].size;
                var type = obj.files[0].type;
                if (type.indexOf("image") == -1) {
                    var oWnd = $find("<%= WindowInvalidExtension.ClientID%>");
                    oWnd.show();
                }
                else {
                    document.getElementById("<%=uploadButton.ClientID %>").click();
                }
            }
        }

         function CloseDeleteConfirmationWindow() {
            $("#dialog-error-info").text("");
            $('.ui-dialog-content:visible').dialog('close');
            $("#dialog-confirm-delete").dialog('close');
        }

        function DeleteLink(linkID) {
            $("#dialog-delete-text").html("Are you sure you want to delete this link?");
                $("#dialog-error-info").text("");

                $("#btnConfirmDelete").on("click", function (event) {
                    $.ajax({
                        type: 'Get',
                        url: 'B2BManagerService.svc/DeleteCustomLink',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: { LinkID : linkID },
                        async: true,
                        success: function (response) {
                            if (response.toLowerCase() == "success") {
                                CloseDeleteConfirmationWindow();
                                __doPostBack("<%= linksUpdatePanel.ClientID%>", "Delete");
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
                    title: "Delete Link Confirmation"
                });
        }

        function NewLink() {
            var oWnd = $find("<%= WindowLink.ClientID%>");
            oWnd.set_title('Loading...');
            oWnd.setUrl("ProfileCustomLink.aspx?HideHeader=true");
            oWnd.show();
        }

        function CloseLinktWindow() {
            var oWnd = $find("<%= WindowLink.ClientID%>");
            oWnd.close();
        }

        function EditLink(linkID) {
            var oWnd = $find("<%= WindowLink.ClientID%>");
            oWnd.set_title('Loading...');
            oWnd.setUrl("ProfileCustomLink.aspx?HideHeader=true&linkid=" + linkID);
            oWnd.show();
        }

        function LoadLinks() {
            CloseLinktWindow();
            __doPostBack("<%= linksUpdatePanel.ClientID%>", "");
        }

        function ChangePasswordWindow(show) {
            var oWnd = $find("<%= WindowChangePassword.ClientID%>");
            if (show) {
                oWnd.setUrl("ChangePassword.aspx?HideHeader=true");
                oWnd.show();
            }
            else {
                 oWnd.close();
            }
        }
        function CloseWindowInvalidExtension() {
            var oWnd = $find("<%= WindowInvalidExtension.ClientID%>");
            oWnd.close();
        }
        function deleteLogoClick() {
            document.getElementById("<%=btnDeleteLogo.ClientID %>").click();
        }
        function TryAgain() {
            CloseWindowInvalidExtension();
            $("#changeLogoLbl").trigger("click");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <section class="content">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-3">
                    <div class="card card-primary card-outline">
                        <div class="card-body box-profile">
                            <div class="text-center">
                                <asp:Image CssClass="profile-user-img img-fluid img-circle" ID="userImage" runat="server" />
                                <asp:FileUpload ID="FileUploadControl" ClientIDMode="Static" accept="image/*" AllowMultiple="false" CssClass="hidden" runat="server" onchange="UploadFile(this);" />
                                <asp:Button runat="server" ID="uploadButton" Text="" Style="display: none;" OnClick="uploadButton_Click" />
                                <br />
                                <label id="changeLogoLbl" for="FileUploadControl" class="defaultLink">Upload Photo</label>
                                <span id="deleteLogoLbl" class="defaultLink" runat="server" onclick="deleteLogoClick()" >| Reset</span>
                                <asp:Button runat="server" ID="btnDeleteLogo" Text="" OnClick="btnDeleteLogo_Click" Style="display: none;" />
                            </div>
                            <h3 class="profile-username text-center" id="userFullNameLbl" runat="server"></h3>
                            <p class="text-muted text-center" id="nickNameLbl" runat="server"></p>
                        </div>
                    </div>
                    <!-- /.card -->
                    <!-- About Me Box -->
                    <div class="card card-primary">
                        <div class="card-header">
                            <h3 class="card-title" style="font-size: 15px">About Me</h3>
                        </div>
                        <!-- /.card-header -->
                        <div class="card-body">
                            <p><strong>Login:</strong> <asp:Label runat="server" ID="loginLbl"></asp:Label></p>
                            <p><strong>First name:</strong> <asp:Label runat="server" ID="FirstNameLbl"></asp:Label></p>
                            <p><strong>Last name:</strong> <asp:Label runat="server" ID="lastNameLbl"></asp:Label></p>
                            <p><strong>Email:</strong> <a runat="server" id="emailHyperLink" class="defaultLink"></a></p>
                        </div>
                        <!-- /.card-body -->
                    </div>
                </div>
                <!-- /.col -->
                <div class="col-md-9">
                    <div class="card">
                        <div class="card-body">
                            <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
                                <Tabs>
                                    <telerik:RadTab Text="My details" Width="160px" PageViewID="RadPageViewSettings"></telerik:RadTab>
                                    <telerik:RadTab Text="My settings" Width="160px" PageViewID="RadPageViewPreferences" Visible="false" runat="server"></telerik:RadTab>
                                    <telerik:RadTab Text="My links" Width="160px" PageViewID="RadPageViewLinks"></telerik:RadTab>
                                    <telerik:RadTab Value="EbusinessPreferences" Text="E-buisness preferences" Width="160px" PageViewID="RadPageViewEbusinessSettings"></telerik:RadTab>
                                    <telerik:RadTab Value="InsightsPreferences" Text="Insights preferences" Width="160px" PageViewID="RadPageViewInsightsSettings"></telerik:RadTab>
                                </Tabs>
                            </telerik:RadTabStrip>
                            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage" EnableAjaxSkinRendering="true">
                                <telerik:RadPageView runat="server" ID="RadPageViewSettings">
                                    <asp:UpdatePanel runat="server" ID="up_UsersInformation">
                                        <ContentTemplate>
                                        <table style="height: 281px;">
                                            <tr>
                                                <td style="vertical-align: top; min-width: 450px">
                                                    <table cellpadding="5">
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Title:</asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList runat="server" ID="ddlTitle">
                                                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                                    <asp:ListItem Value="1" Text="Mr"></asp:ListItem>
                                                                    <asp:ListItem Value="2" Text="Mrs"></asp:ListItem>
                                                                    <asp:ListItem Value="3" Text="Miss"></asp:ListItem>
                                                                    <asp:ListItem Value="4" Text="Ms"></asp:ListItem>
                                                                    <asp:ListItem Value="5" Text="Dr"></asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 200px">
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">First Name:</asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBoxFirstName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Last Name:</asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBoxLastName" CssClass="Electrolux_light_bold width230px" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Password:</asp:Label>
                                                            </td>
                                                            <td style="height: 38px">
                                                                <div id="changePassword" onclick="ChangePasswordWindow(true)" runat="server" class="defaultLink">Change Passowrd</div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Nick Name:</asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBoxNickName" CssClass="Electrolux_light_bold width230px" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Email:</asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBoxEmail" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:Label runat="server" ID="lblInfo"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <asp:LinkButton runat="server" ID="btnSubmit" OnClientClick="ProcessButton(this,'Saving...')" OnClick="btnSubmit_Click" CssClass="btn lightblue" ><i class="fas fa-check"></i> Save Changes</asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnSubmit" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </telerik:RadPageView>
                                <telerik:RadPageView runat="server" ID="RadPageViewPreferences">
                                </telerik:RadPageView>                                
                                <telerik:RadPageView runat="server" ID="RadPageViewLinks">
                                    <asp:UpdatePanel runat="server" ID="linksUpdatePanel" UpdateMode="Conditional"> 
                                        <ContentTemplate>
                                            <div class="drag-section-header Electrolux_light_bold Electrolux_Color" runat="server" id="DragAndDropHeader">Drag and drop links to change positions</div>
                                            <ul id="linksListContainer" class="areas-list-container" style="height:245px !important" runat="server">
                                            </ul>
                                            <span style="margin-left:170px">
                                                <a class="btn lightblue" id="btnSaveLinksPositions" runat="server" onclick="SaveSectionPositions(this)"><i class="fas fa-check"></i> Save Positions</a>
                                                <a class="btn bleu" id="btnAddLink" onclick="NewLink()" ><i class="fas fa-link"></i> New Link</a>
                                            </span>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>                                    
                                </telerik:RadPageView>
                                <telerik:RadPageView runat="server" ID="RadPageViewEbusinessSettings">
                                    <div style="float: left">
                                        <uc1:EbusinessPreferences runat="server" ID="EbusinessPreferences" ShowCancelButton="false" />                                     
                                    </div>
                                </telerik:RadPageView>
                                <telerik:RadPageView runat="server" ID="RadPageViewInsightsSettings">
                                    <div style="float: left; height: 281px;">
                                        <uc1:InsightsPreferences runat="server" ID="InsightsPreferences" ShowCancelButton="false" />
                                    </div>
                                </telerik:RadPageView>
                            </telerik:RadMultiPage>
                        </div>
                        <!-- /.card-body -->
                    </div>
                    <!-- /.nav-tabs-custom -->
                </div>
                <!-- /.col -->
            </div>
            <!-- /.row -->
        </div>
        <!-- /.container-fluid -->
    </section>
        <telerik:RadWindow ID="WindowChangePassword" runat="server" RenderMode="Lightweight" Modal="true"  VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="500px" Height="270px">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowLink" runat="server" RenderMode="Lightweight" Modal="true"  VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="600px" Height="220px">
    </telerik:RadWindow>
       <telerik:RadWindow ID="WindowInvalidExtension" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Invalid Logo" Behaviors="Close" Width="450" Height="160">
        <ContentTemplate>
            <table style="width:100%;padding:15px" cellpadding="15px" cellspacing="5px"> 
                <tr>
                    <td style="text-align:justify" class="Electrolux_Color">
                        It seems that the file you are trying to upload is not a valid image or its extension is not supported. Please try again with a valid image
                    </td>
                </tr>
                <tr>
                    <td style="text-align:center">
                        <input type="button" value="Cancel" class="btn red" onclick="CloseWindowInvalidExtension()" />
                        <input type="button" value="Try again" class="btn bleu" onclick="TryAgain()" />
                    </td>
                </tr>
            </table>
           
        </ContentTemplate>
    </telerik:RadWindow>

    <div id="dialog-confirm-delete" title="Delete Confirmation" class="DisplayNone">
        <div id="dialog-delete-text" style="margin:15px"></div>
        <span id="dialog-error-info" style="color:red;height:20px"></span>
        <table align="right">
            <tr>
                <td>
                    <button class="btn bleu" id="btnCancel" onclick="CloseDeleteConfirmationWindow()">Cancel</button>
                    <asp:HiddenField ID="deleteObjectID" runat="server" />
                </td>
                <td>
                    <button class="btn red" id="btnConfirmDelete">Confirm</button>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

