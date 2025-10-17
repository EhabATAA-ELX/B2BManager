<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsDashboardProfile.aspx.vb" Inherits="EbusinessInsightsDashboardProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .RadComboBoxDropDown .rcbImage {
            width: auto !important;
        }
        .rtImg{
            height:20px;
         }
        
        .sharingTag {
            padding: 0px 5px;
            font-size: 10px;
            color: white;
            border-radius: 2px;
        }
    </style>
    <script type="text/javascript">
        function OnClientNodeClicking(sender, eventArgs) {
            eventArgs.set_cancel(true);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table style="width: 100%">
                <tr>
                    <td style="width: 100%; padding-left: 25px" valign="top">
                        <table cellpadding="5" cellspacing="5">
                            <tr class="Height30px">
                                <td class="width180px">
                                    <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color width180px">Dashboard Name:</asp:Label>
                                </td>
                                <td class="width230px" align="left">
                                    <asp:TextBox ID="txtBoxDashboardName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="width180px">
                                    <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Accessibility Level:</asp:Label>
                                </td>
                                <td class="width230px"  align="left">
                                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlSharingType">
                                        <Items>
                                            <telerik:RadComboBoxItem ImageUrl="Images/Private.png" Text="Private" Value="0" Selected="true" />
                                            <telerik:RadComboBoxItem ImageUrl="Images/Shared.png" Text="Shared" Value="1" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <fieldset>
                                        <legend class="Electrolux_light_bold Electrolux_Color">Available Charts <asp:ImageButton runat="server" ID="BtnRefreshChartsAndSections" ImageUrl="Images/Reload.png" ClientIDMode="Static" CssClass="verticalAlignBottom" Width="20" Height="20" ToolTip="Refresh" OnClientClick="refreshChartsAndSections()" /></legend>
                                         <telerik:RadTreeView runat="server" ID="tvCharts" CheckBoxes="true" ShowLineImages="false"  OnClientNodeClicking="OnClientNodeClicking" EnableDragAndDrop="false" AllowNodeEditing="false" CheckChildNodes="true" TriStateCheckBoxes="true" Width="670" Height="370">
                                        </telerik:RadTreeView>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:Label runat="server" ID="lblInfoMessage" Height="14" ForeColor="Red" Text=" "></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <input type="button" class="btn red" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrCloseDashboardWindow(false)" />
                                    <asp:LinkButton runat="server" CssClass="btn green" ID="btnExecute" ClientIDMode="Static" Text="Save and Load" OnClick="btnExecute_Click" CausesValidation="true" ValidationGroup="ActionSubmit"></asp:LinkButton>
                                    <input type="button" class="btn bleu" id="btnManageSection" value="Manage Sections" onclick="window.parent.ManageSections()" />
                                    <asp:PlaceHolder runat="server" ID="btnNewChartPlaceHolder"><input type="button" value="New Chart" id="btnCreateChart" class="btn blue" onclick="window.parent.CreateChart()" /></asp:PlaceHolder>
                                </td> 
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnExecute" />
            <asp:AsyncPostBackTrigger ControlID="BtnRefreshChartsAndSections" />
        </Triggers>
    </asp:UpdatePanel>    
     <script type="text/javascript">
         function refreshChartsAndSections() {
             $("#BtnRefreshChartsAndSections")[0].src = "Images/Loader.gif";
             __doPostBack('BtnRefreshChartsAndSections', '')
         }
    </script>
</asp:Content>

