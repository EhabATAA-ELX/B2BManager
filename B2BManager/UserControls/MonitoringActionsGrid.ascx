<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MonitoringActionsGrid.ascx.vb" Inherits="UserControls_MonitoringActionsGrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
    function ShowActionProfile(actionID) {
        var url = 'MonitoringActionProfile.aspx?v=1';
        if (actionID && actionID != null) {
            url += "&ActionID=" + actionID.toString();
        }
        if ($(window).height() > 410 && $(window).width() > 700) {
            var oWnd = $find("<%= WindowActionProfile.ClientID %>");
            oWnd.setUrl(url + "&HideHeader=true");
            oWnd.show();
        }
        else {
            popup(url);
        }
    }    
    function RunSearch() {
        __doPostBack('CurrentEnvironmentID', '');
    }
    function CloseActionProfileWindow() {
        var oWnd = $find("<%= WindowActionProfile.ClientID %>");
        oWnd.close();
    }

    function DeleteAction(actionID) {
        __doPostBack('SelectedActionIDToDelete', actionID);
        ShowOrCloseWindow('DeleteAction', true);
    }

    function ShowOrCloseWindow(windowIdentifier, Show) {
        var oWnd = null;
        switch (windowIdentifier) {
            case "DeleteAction":
                $("#lblDeleteActionErrorMessage").text("");
                oWnd = $find("<%= WindowDeleteAction.ClientID%>");
        }
        if (oWnd != null) {
            if (Show) {
                oWnd.show();
            }
            else {
                oWnd.close();
            }
        }
        return false;
    }

    function ProcessDeleteAction() {

        $('#BtnDeleteAction').addClass("loadingBackground").html("Deleting..").prop('disabled', true);
        __doPostBack("SelectedActionIDToDelete", "SubmitDeleteAction")
        return false;
    }
</script>
<asp:UpdatePanel runat="server" ID="UpdatePanel1">
    <ContentTemplate>
        <asp:HiddenField runat="server" ID="CurrentEnvironmentID" Value="" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="CurrentLinkedWorkflowID" Value="" ClientIDMode="Static" />
        <table class="Filters">
            <tr>
                <td class="width120px">
                    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                </td>
                <td class="width180px">
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlEnvironment" DataTextField="Name" DataValueField="ID">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblType" CssClass="Electrolux_light_bold Electrolux_Color">Type:</asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" ID="ddlType" DataTextField="ID" DataValueField="Name">
                    </telerik:RadComboBox>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblActionDetails" CssClass="Electrolux_light_bold Electrolux_Color">Free text search:</asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtBoxSearchInDetails" CssClass="Electrolux_light_bold Electrolux_Color width120px"></asp:TextBox>
                </td>
                <td>
                    <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="BeginSearch()" OnClick="btnSearch_Click" Text="Search" />
                    <input type="button" id="btnNewAction" class="btn bleu" onclick="ShowActionProfile()" value="Add New Action" />
                </td>
            </tr>
        </table>


        <telerik:RadGrid runat="server" MasterTableView-ShowHeadersWhenNoRecords="false" OnNeedDataSource="monitoringActionsGrid_NeedDataSource" ID="monitoringActionsGrid" CssClass="MonitoringGridSearch" AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="true" PageSize="15" GroupingEnabled="true">
            <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="Select" AllowFiltering="false" HeaderText="Select" Visible="false">
                        <ItemTemplate>
                           <img src="Images/select.png" width="20" title="Select Action" onclick="SelectActionWithParameters('<%# Eval("DefaultImageName").ToString() %>','<%# Eval("Comments").ToString() %>','<%# Eval("ActionID").ToString() %>','<%# (CInt(Eval("ActionTypeID")) = 0).ToString() %>')" class="MoreInfoImg" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Actions">
                        <ItemTemplate>
                            <table border="0" style="border:none" >
                                <tr style="border:none" cellpadding="0">
                                    <td style="border:none">
                                        <img src='Images/Edit.png' width="20" height="20" title="Display/edit details" class="<%# IIf(Eval("Editable"), "MoreInfoImg", "ImgDisabled")  %>"
                                           onclick="<%# IIf(Eval("Editable"), "ShowActionProfile('" & Eval("ActionID").ToString() & "')", "return false;")  %>" />
                                    </td>
                                    <td style="border:none">
                                        <img src='Images/delete.png' width="20"  class="<%# IIf(Eval("Editable"), "MoreInfoImg", "ImgDisabled")  %>" height="20" title="Delete Action" onclick="<%# IIF(Eval("Editable"), "DeleteAction('" & Eval("ActionID").ToString() & "')", "return false;")  %>"  />
                                    </td>
                                </tr>
                            </table>                
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Type" AllowFiltering="false" HeaderText="Action Type">
                        <ItemTemplate>
                            <table title="<%# Eval("Description").ToString() %>" style="border:none !important">
                                <tr>
                                    <td style="border:none !important">
                                        <img src='Images/MonitoringWorkflow/<%# Eval("DefaultImageName").ToString() %>' width="20" height="20" />
                                    </td>
                                    <td style="border:none !important">
                                        <span><%# Eval("Name").ToString() %></span>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="Comments" HeaderText="Description">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="ServerName" HeaderText="Server Name / Title">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="InputParameter" AllowFiltering="true" DataField="InputParameter" HeaderText="Input Parameter">
                        <ItemTemplate>
                            <img src='<%# Eval("CustomImageUrl").ToString() %>' class="<%# IIf(CBool(Eval("HasCustomImage")), "", "hidden") %>" width="15" height="15" />
                            <span><%# Eval("InputParameter") %></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="CreateByUserName" HeaderText="Created By">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="WarningRatio" HeaderText="Warning Ratio">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="AlertRatio" HeaderText="Altert Ratio">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowSorting="true" AllowFiltering="true" DataField="TotalWorkflows" HeaderText="Linked Workflows">
                    </telerik:GridBoundColumn>
                </Columns>
                <NoRecordsTemplate>
                    No action available.
                </NoRecordsTemplate>
            </MasterTableView>
        </telerik:RadGrid>

        <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" OnClientShow="setPopupTitle" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Loading..." ShowContentDuringLoad="false" Behaviors="Close" Width="700px" Height="410px" runat="server">
        </telerik:RadWindow>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        <asp:AsyncPostBackTrigger ControlID="CurrentEnvironmentID" />
    </Triggers>
</asp:UpdatePanel>

<telerik:RadWindow ID="WindowDeleteAction" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Delete Monitoring Action" Behaviors="Close" Width="400px" Height="150px" runat="server">
    <ContentTemplate>
        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
            <ContentTemplate>
                <asp:HiddenField ID="SelectedActionIDToDelete" runat="server" ClientIDMode="Static" />
                 <table align="center" style="margin-top: 10px">
                        <tr class="Height30px">
                            <td colspan="3" align="center" style="width: 100%; margin: 25px;">Are you sure you want to delete this action?</td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <asp:Label runat="server" ID="lblDeleteActionErrorMessage" ClientIDMode="Static" ForeColor="Red" Text=" "></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <input type="button" class="btn red" id="BtnCancelDeleteAction" value="Cancel" onclick="ShowOrCloseWindow('DeleteAction', false)" />
                                <input type="button" class="btn green" id="BtnDeleteAction" value="Delete" onclick="ProcessDeleteAction()" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SelectedActionIDToDelete" />
            </Triggers>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>