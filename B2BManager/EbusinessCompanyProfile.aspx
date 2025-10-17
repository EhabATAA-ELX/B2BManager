<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessCompanyProfile.aspx.vb" Inherits="EbusinessCompanyProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/UserControls/CustomerDetails.ascx" TagPrefix="uc1" TagName="CustomerDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <script src="Scripts/ECharts/echarts.common.min.js"></script>
    <link href="CSS/Insights.css" rel="stylesheet" />
    <script src="Scripts/Insights.js?v=1.5"></script>
    <script type="text/javascript">
        function CloseWindow(Action) {
            if (Action == 'SubmitUpdate') {
                $.magnificPopup.open({
                    items: {
                        src: '<div class="white-popup success-popup"></br><p class="text-justify"><i class="far fa-check-circle" style="font-size: 18pt;vertical-align: middle;"></i> Customer has been updated with success</p></div>', 
                        type: 'inline'
                    },
                    callbacks: {
                        close: function () {
                            if (window.parent) {
                                if (typeof window.parent.ShowAndRefreshGridUpdate == "function") {
                                    window.parent.ShowAndRefreshGridUpdate(Action);
                                }
                            }
                            if (window.opener) {
                                if (typeof window.opener.ShowAndRefreshGridUpdate == "function") {
                                    window.opener.ShowAndRefreshGridUpdate(Action);
                                }
                                window.close();
                            }
                        }
                    }
                });
            }
            else {
                __doPostBack("Refresh","");
            }
        }
            
        function CloseoWnd(Action) {
            if (Action == 'Price') {
                var oWnd = $find("<%= WindowDeletePriceCache.ClientID %>");
                oWnd.close();
            }
            if (Action == 'Avail') {
                var oWnd = $find("<%= WindowDeleteTP2AvailCache.ClientID %>");
                oWnd.close();
            }
            else {
                var oWnd = $find("<%= WindowDeleteTimeStampCusRange.ClientID %>");
                oWnd.close();
            }
        }

        $(document).ready(function () {
            $('.override-sap-import').click(function () {
                if ($(this).is(":checked")) {
                    $("input.customer-name").removeAttr('disabled');
                    $('span.userTag:contains("Name updated by SAP")').hide();
                }
                else if ($(this).is(":not(:checked)")) {
                    $("input.customer-name").attr('disabled', 'disabled');
                    if ($('span.userTag:contains("Name updated by SAP")').length) {
                        console.log('inside length');
                        $('span.userTag:contains("Name updated by SAP")').show();
                    }
                }
            });
        });

    </script>
    <style type="text/css">
        .minWidth600px{
            width:100% !important;
            min-width:600px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table style="width:100%" cellspacing="5" cellpadding="5">
                <tr>
                    <td style="width: 350px;">
                        <div class="card card-primary card-outline">                            
                                        <div class="text-center">
                                            <asp:Image runat="server" Height="80" ID="customerLogo" ImageUrl="~/Images/Ebusiness/CustomersManagement/company.png" />
                                            <asp:PlaceHolder runat="server" ID="uploadPlaceHolder">                                                
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
                                                <asp:UpdatePanel ChildrenAsTriggers="true" UpdateMode="Conditional" runat="server" ID="UploaderUpdatePanel">
                                                    <ContentTemplate>
                                                            <asp:FileUpload ID="FileUploadControl" ClientIDMode="Static" accept="image/*" AllowMultiple="false" CssClass="hidden" runat="server" onchange="UploadFile(this);" />
                                                            <asp:Button runat="server" ID="uploadButton" Text="" Style="display: none;" OnClick="uploadButton_Click" />
                                                            <br />
                                            
                                                            <label id="changeLogoLbl" for="FileUploadControl" class="defaultLink">Upload Logo</label>
                                                            <span id="deleteLogoLbl" class="defaultLink" runat="server" onclick="deleteLogoClick()" visible="false">| Reset</span>
                                                            <asp:Button runat="server" ID="btnDeleteLogo" Text="" OnClick="btnDeleteLogo_Click" Style="display: none;" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                            </asp:PlaceHolder>
                                        </div>
                                        <h3 class="profile-username text-center">
                                            <asp:Label runat="server" ID="lblCustomerCode" CssClass="Electrolux_Color textHighlithed"></asp:Label></h3>
                                        <p class="text-muted text-center">
                                            <asp:Label runat="server" ID="lblCustomerName" CssClass="Electrolux_Color"></asp:Label></p>
                                        <p class="text-muted text-center" style="height:16px">
                                            <asp:Label runat="server" ID="lblDescription" CssClass="Electrolux_Color"></asp:Label></p>                            
                        </div>
                        </div>
                    </td>
                    <td runat="server" id="deletePriceCacheAndRangeTD" style="width:150px"> 
                        <div class="card card-primary">
                            <div class="card-header">
                                <h3 runat="server" id="customerActionsHeader" class="rtsTxt" style="font-size: 13px">Customer Actions</h3>
                            </div>
                            <div class="card-body" style="min-height: 135px">
                                <asp:PlaceHolder runat="server" ID="deleteCustomerRangePlaceHolder">
                                    <script type="text/javascript">
                                        function DeleteCustomerRange() {
                                            var EnvID = $('#ContentPlaceHolder1_CustomerDetails_HD_EnvID').val()
                                            var CustomerName = $('#ContentPlaceHolder1_CustomerDetails_txtBoxCustomerName').val()
                                            var C_GlobalID = $('#ContentPlaceHolder1_CustomerDetails_HD_C_GlobalID').val()
                                            var SopName = $('#ContentPlaceHolder1_CustomerDetails_HD_SopName').val()
                                            var oWnd = $find("<%= WindowDeleteTimeStampCusRange.ClientID %>");
                                            oWnd.setUrl("EbusinessDeleteCustomerRange.aspx?HideHeader=true&EnvironmentID=" + EnvID + "&CustomerName=" + CustomerName + "&C_GlobalID=" + C_GlobalID + "&SOPNAME=" + SopName);
                                            oWnd.show();
                                        }
                                    </script>
                                    <input type="button" class="btn lightblue Width100" value="Reset Customer Range" id="btnDeleteCustomerRange" onclick="DeleteCustomerRange()" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="deletePriceCachePlaceHolder"> 
                                    <script type="text/javascript">
                                        function DeletePriceCache() {
                                            var EnvID = $('#ContentPlaceHolder1_CustomerDetails_HD_EnvID').val()
                                            var CustomerCode = $('#ContentPlaceHolder1_lblCustomerCode').text()
                                            var CustomerName = $('#ContentPlaceHolder1_CustomerDetails_txtBoxCustomerName').val()
                                            var SopName = $('#ContentPlaceHolder1_CustomerDetails_HD_SopName').val()
                                            var oWnd = $find("<%= WindowDeletePriceCache.ClientID %>");
                                            oWnd.setUrl("EbusinessDeleteCachePrice.aspx?HideHeader=true&CUSTOMERCODE=" + CustomerCode + "&EnvironmentID=" + EnvID + "&CustomerName=" + CustomerName + "&SOPNAME=" + SopName);
                                            oWnd.show();
                                        }
                                    </script>
                                    <input type="button" class="btn lightblue Width100" value="Delete Price Cache" id="btnDeletePriceCache" onclick="DeletePriceCache()" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="attachToSuperUserPlaceHolder">
                                    <script type="text/javascript">
                                        function AttachToSuperUser() {
                                            var EnvID = $('#ContentPlaceHolder1_CustomerDetails_HD_EnvID').val()
                                            var C_GlobalID = $('#ContentPlaceHolder1_CustomerDetails_HD_C_GlobalID').val()
                                            var SopName = $('#ContentPlaceHolder1_CustomerDetails_HD_SopName').val()
                                            var oWnd = $find("<%= WindowAttachToSuperUser.ClientID %>");
                                            oWnd.setUrl("EbusinessAttachCustomerToSuperUser.aspx?HideHeader=true&cid=" + C_GlobalID + "&envid=" + EnvID);
                                            oWnd.show();
                                        }
                                        function CloseAttachToSuperUserWindow() {
                                            var oWnd = $find("<%= WindowAttachToSuperUser.ClientID %>");
                                            oWnd.close();
                                        }
                                    </script>
                                    <input type="button" class="btn lightblue Width100" value="Attach to Super User" id="btnAttachToSuperUser" onclick="AttachToSuperUser()" />
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="PlaceHolder1">
                                    <script type="text/javascript">
                                        function DeleteCacheAvail() {
                                            var EnvID = $('#ContentPlaceHolder1_CustomerDetails_HD_EnvID').val()
                                            var CustomerCode = $('#ContentPlaceHolder1_lblCustomerCode').text()
                                            var CustomerName = $('#ContentPlaceHolder1_CustomerDetails_txtBoxCustomerName').val()
                                            var SopName = $('#ContentPlaceHolder1_CustomerDetails_HD_SopName').val()
                                            var oWnd = $find("<%= WindowDeleteTP2AvailCache.ClientID %>");
                                            oWnd.setUrl("EbusinessDeleteTP2CacheAvail.aspx?HideHeader=true&CUSTOMERCODE=" + CustomerCode + "&EnvironmentID=" + EnvID + "&CustomerName=" + CustomerName + "&SOPNAME=" + SopName);
                                            oWnd.show();
                                        }                                      
                                    </script>
                                    <input type="button" class="btn lightblue Width100" value="TP2 Delete Avail Cache" id="btnDeleteCacheAvail" onclick="DeleteCacheAvail()" />
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </td>
                   <%-- <td style="min-width: 150px">
                        <div class="card card-primary">
                            <div class="card-header" runat="server" id="CustomerHeaderTagsID">
                                <h3 class="rtsTxt" style="font-size: 13px">Customer Tags</h3>
                            </div>
                            <!-- /.card-header -->
                            
                            <!-- /.card-body -->
                        </div>
                    </td>--%>

                </tr>
            </table>
            <telerik:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
                <Tabs>
                    <telerik:RadTab Text="Customer Details" Width="120px" PageViewID="RadPageViewDetails"></telerik:RadTab>                
                    <telerik:RadTab Value="Insights" Text="Insights" Width="110px" PageViewID="RadPageViewInsights"></telerik:RadTab>
                    <telerik:RadTab Value="UserList" Text="User List" Width="110px" PageViewID="RadPageViewUserList"></telerik:RadTab>
                    <telerik:RadTab Text="SAP Master Data" Width="120px" PageViewID="RadPageViewCustomerTagsList"></telerik:RadTab>
                    <telerik:RadTab Value="AddressList" Text="Address List" Width="100px" PageViewID="RadPageViewAddressList"></telerik:RadTab>
                    <telerik:RadTab Value="ActivityHistory" Text="Activity History" Width="110px" PageViewID="RadPageViewActivityHistory"></telerik:RadTab>
                    <telerik:RadTab Value="Contacts" Text="Contacts" Width="100px" PageViewID="RadPageViewContacts"></telerik:RadTab>                    
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0" CssClass="outerMultiPage" EnableAjaxSkinRendering="true">
                <telerik:RadPageView runat="server" ID="RadPageViewDetails">
                    <uc1:CustomerDetails runat="server" ID="CustomerDetails" />
                </telerik:RadPageView> 
                <telerik:RadPageView runat="server" ID="RadPageViewInsights">
                    <iframe runat="server" class="card" style="width: 100%; min-height: 620px; border: none" loading="lazy" id="iframeInsights"></iframe>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageViewUserList">
                    <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeUserList"></iframe>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageViewCustomerTagsList">
                    <div  runat="server" id="CustomerTagsTD" style="display:inline-flex" >
                        <div runat="server" id="CustomerTagsGeneral" style="display: inline-block;">

                        </div>
                        <div runat="server" id="CustomerTagsCompanyCode" style="display: inline-block;">

                        </div>
                        <div runat="server" id="CustomerTagsSalesArea" style="display: inline-block;">

                        </div>
                     </div>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageViewAddressList">
                    <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeAddressList"></iframe>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageViewActivityHistory">
                    <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none;" id="iframeActivityLog"></iframe>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="RadPageViewContacts">
                    <iframe runat="server" class="card" style="width: 100%; min-height: 600px; border: none" id="iframeContacts"></iframe>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="uploadButton" />
            <asp:AsyncPostBackTrigger ControlID="btnDeleteLogo" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowInvalidExtension" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" VisibleStatusbar="false" Title="Invalid Logo" Behaviors="Close" Width="450" Height="160">
        <ContentTemplate>
            <table style="width: 100%; padding: 15px" cellpadding="15px" cellspacing="5px">
                <tr>
                    <td style="text-align: justify" class="Electrolux_Color">It seems that the file you are trying to upload is not a valid image or its extension is not supported. Please try again with a valid image
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <input type="button" value="Cancel" class="btn red" onclick="CloseWindowInvalidExtension()" />
                        <input type="button" value="Try again" class="btn bleu" onclick="TryAgain()" />
                    </td>
                </tr>
            </table>

        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowDeletePriceCache" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="450" Height="200px">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowDeleteTimeStampCusRange" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="450" Height="200px">
    </telerik:RadWindow>
    <telerik:RadWindow ID="WindowAttachToSuperUser" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="550" Height="550px">
    </telerik:RadWindow>
        <telerik:RadWindow ID="WindowDeleteTP2AvailCache" runat="server" RenderMode="Lightweight" Modal="true" VisibleOnPageLoad="false" DestroyOnClose="false" ShowContentDuringLoad="false" VisibleStatusbar="false" Title="Loading..." Behaviors="Close" Width="450" Height="200px">
    </telerik:RadWindow>
</asp:Content>


