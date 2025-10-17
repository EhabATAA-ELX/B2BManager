<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsManageSections.aspx.vb" Inherits="EbusinessInsightsManageSections" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/InsightsAreas.css" rel="stylesheet" />
    <script src="Scripts/InsightsAreas.js?v=1.0"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadWindow ID="AreaWindow" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false"
        VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="490px" Height="350px" runat="server">
    </telerik:RadWindow>
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="BottomCenter">
    </telerik:RadToolTipManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <asp:HiddenField ID="RefreshHiddenField" ClientIDMode="Static" runat="server" />
            <div class="drag-section-header Electrolux_light_bold Electrolux_Color" runat="server" id="DragAndDropHeader">Drag and drop sections to change positions</div>
            <ul id="areasListContainer" class="areas-list-container" runat="server">
            </ul>
            <table align="center">
                <tr>
                    <td>
                        <input type="button" class="btn red" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrCloseManageSectionsWindow(false)" />
                        <input type="button" class="btn green" id="btnSaveSectionPositions" runat="server" value="Save Positions" onclick="SaveSectionPositions(this)" />
                        <input type="button" class="btn bleu" id="btnAddSection" onclick="EditArea()" value="New Section" />
                        <input type="button" class="btn bleu" id="btnRefresh" onclick="ProcessButton(this, 'Refreshing'); LoadSections();" value="Refresh" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="RefreshHiddenField" />
        </Triggers>
    </asp:UpdatePanel>

    <telerik:RadWindow ID="WindowDeleteSection" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Section Confirmation" Behaviors="Close" Width="400px" Height="150px" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanel5">
                <ContentTemplate>
                    <asp:HiddenField ID="SelectedSectionIDToDelete" runat="server" ClientIDMode="Static" />
                    <table align="center" style="margin-top: 10px">
                        <tr class="Height30px">
                            <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete this section?</td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="lblDeleteSectionErrorMessage" ClientIDMode="Static" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <input type="button" class="btn red" id="BtnCancelDeleteSection" value="Cancel" onclick="ShowOrCloseSectionDeletetWindow(false)" />
                                <input type="button" class="btn green" id="BtnDeleteSection" value="Delete" onclick="ProcessDeleteSection()" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SelectedSectionIDToDelete" />
                </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow ID="WindowNoChanges" RenderMode="Lightweight" VisibleTitlebar="false"  Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Behaviors="None" Width="300px" Height="120px" runat="server">
        <ContentTemplate>
            <table align="center" style="margin-top: 10px">
                <tr class="Height30px">
                    <td colspan="3" align="center" style="width: 100%; margin: 25px;">Section positions are the same.<br />No changes were applied.</td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <input type="button" class="btn red" value="Cancel" onclick="CloseNoChangesWindow()" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </telerik:RadWindow>

    <script type="text/javascript">

        function ShowOrCloseSectiontWindow(Show) {
            ChangeWindowDisplay("<%= AreaWindow.ClientID %>", Show);
        }

        function ShowOrCloseSectionDeletetWindow(Show) {
            ChangeWindowDisplay("<%= WindowDeleteSection.ClientID %>", Show);
        }

        function ProcessDeleteSection() {
            $('#BtnDeleteSection').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
            __doPostBack("SelectedSectionIDToDelete", "SubmitDeleteSection")
            return false;
        }

        function DeleteArea(sectionID) {
            __doPostBack('SelectedSectionIDToDelete', sectionID);
            ShowOrCloseSectionDeletetWindow(true);
        }

        function CloseNoChangesWindow() {
            ChangeWindowDisplay("<%= WindowNoChanges.ClientID %>", false);
        }


        function SaveSectionPositions(sender) {
            var sectionIDs = [];
            $.each($(".areas-list-container li"), function (key, value) {
                sectionIDs.push($(value).attr("data-section-id"));
            });
            if (previousSectionPositions == sectionIDs.join(",")) {
                ChangeWindowDisplay("<%= WindowNoChanges.ClientID %>", true);
            }
            else {
                ProcessButton(sender, 'Saving...');
                __doPostBack('RefreshHiddenField', sectionIDs.join(","));
            }
        }

        function LoadSections() {
            __doPostBack('RefreshHiddenField', 'Refresh');
            ShowOrCloseSectiontWindow(false);
        }

        function EditArea(sectionID) {
            var url = 'EbusinessInsightsSectionProfile.aspx';
            var oWnd = $find("<%= AreaWindow.ClientID %>");
            oWnd.set_title('Loading...');
            oWnd.setUrl(url + "?HideHeader=true" + (sectionID != null ? "&sid=" + sectionID : ""));
            ShowOrCloseSectiontWindow(true);
        }

    </script>
</asp:Content>

