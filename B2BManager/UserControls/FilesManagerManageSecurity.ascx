<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FilesManagerManageSecurity.ascx.vb" Inherits="UserControls_FilesManagerManageSecurity" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:UpdatePanel runat="server" ID="ManageSecurityUpdatePanel">
    <ContentTemplate>
        <asp:Label runat="server" ID="CountryGuidLabel" ClientIDMode="Static" class="hidden" />
        <asp:Label runat="server" ID="FolderGuidLabel" Visible="false" />
        <asp:Label runat="server" ID="DocumentGuidLabel" ClientIDMode="Static" class="hidden" />
        <asp:Label runat="server" ID="EnvironmentIDLabel" ClientIDMode="Static" class="hidden" />
        <asp:Label runat="server" ID="SendNotificationLabel" ClientIDMode="Static" class="hidden" />

        <table class="filters">
            <tr>
                <td>
                    <asp:Button runat="server" ID="BackFileUploadBtn" class="btn bleu rounded" Text="Back to file management" OnClick="BackFileUploadBtn_Click" />
                </td>
            </tr>
        </table>

        <telerik:RadGrid runat="server" RenderMode="Lightweight" ID="FileSelectRadGrid" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" Width="50%">
            <MasterTableView DataKeyNames="ID">
                <Columns>
                    <telerik:GridBoundColumn SortExpression="ID" DataField="ID" HeaderText="ID" HeaderButtonType="TextButton" Display="false" />
                    <telerik:GridTemplateColumn HeaderText="Thumbnail" DataField="ThumbnailContent" UniqueName="ThumbnailContent"
                        AllowFiltering="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Image ID="ThumbnailImage" runat="server" Width="50px" Height="50px"
                                ImageUrl='<%# If(Eval("ThumbnailContent") IsNot Nothing AndAlso Eval("ThumbnailContent") IsNot DBNull.Value, "data:image/png;base64," & Convert.ToBase64String(CType(Eval("ThumbnailContent"), Byte())), String.Empty) %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn SortExpression="Name" DataField="Name" HeaderText="File name" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn SortExpression="Size" DataField="Size" HeaderText="Size" HeaderButtonType="TextButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn SortExpression="CreationDate" DataField="CreationDate" HeaderText="Last update" HeaderButtonType="TextButton" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn SortExpression="ThumbnailName" DataField="ThumbnailName" HeaderText="Thumb" Display="false" HeaderButtonType="TextButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn SortExpression="ThumbnailID" DataField="ThumbnailID" HeaderText="ThumbnailID" HeaderButtonType="TextButton" Visible="false" />
                    <telerik:GridBoundColumn SortExpression="UseDateRangeForPublishing" DataField="UseDateRangeForPublishing" HeaderText="UseDateRangeForPublishing" HeaderButtonType="TextButton" Visible="false" />
                    <telerik:GridTemplateColumn SortExpression="StartDate" DataField="StartDate" HeaderText="Published from" HeaderButtonType="TextButton" UniqueName="StartDateGridTemplate" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server">
                                <%# IIf(Eval("UseDateRangeForPublishing") = False, String.Empty, DateTime.Parse(Eval("StartDate")).ToString("dd/MM/yyyy"))  %>
                            </asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn SortExpression="EndDate" DataField="EndDate" HeaderText="Published to" HeaderButtonType="TextButton" UniqueName="EndDateGridTemplate" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server">
                                <%# IIf(Eval("UseDateRangeForPublishing") = False, String.Empty, DateTime.Parse(Eval("EndDate")).ToString("dd/MM/yyyy"))  %>
                            </asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Send notification" HeaderButtonType="TextButton" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="SendNotificationCb" Checked='<%# Bind("SendNotification") %>' Enabled="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>

        <br />

        <telerik:RadTabStrip runat="server" ID="SecuredUnsecuredRadTabStrip" MultiPageID="RadMultiPage1">
            <Tabs>
                <telerik:RadTab runat="server" Text="Manage Security"></telerik:RadTab>
                <telerik:RadTab runat="server" Text="Secure Selection"></telerik:RadTab>
                <telerik:RadTab runat="server" Text="Manage Publication"></telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage runat="server" ID="RadMultiPage1">
            <telerik:RadPageView runat="server" ID="RadPageView1">
                <table style="width: 40%;">
                    <tr>
                        <td>
                            <span>
                                <asp:Label runat="server" Text="Number of assigned customers : " /><asp:Label ID="AssignedCustomersNumberLb" runat="server" />
                                <asp:Button ID="btnPreviewCustomers" OnClientClick="callParentPreview(); return false;" runat="server" class="btn bleu rounded" Text="Preview Matching Customers" />

                            </span>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="Label" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadGrid ID="ConditionRg" runat="server" OnNeedDataSource="ConditionRg_NeedDataSource" OnItemDataBound="ConditionRg_ItemDataBound" RenderMode="Lightweight" AllowPaging="True" AllowSorting="true" AutoGenerateColumns="False" Width="100%">
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <MasterTableView Width="95%" DataKeyNames="ConditionID, ConditionIsStatic,SOPName">
                                    <Columns>
                                        <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="EditConditionBtn">
                                                    <img src="Images/edit.png" alt="Edit" width="20" height="20">
                                                </asp:LinkButton>
                                                <asp:LinkButton runat="server" ID="DeleteConditionBtn">
                                                    <img src="Images/Delete.png" alt="Delete" onclick="DeleteConditionFocusRange('<%# Eval("ConditionID").ToString() %>'); return false;" width="20" height="20">
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="ConditionName" Display="false" SortExpression="ConditionName" HeaderText="Name" HeaderStyle-Width="35%" />

                                        <telerik:GridCheckBoxColumn DataField="ConditionIsStatic" Display="false" HeaderText="Static Condition" UniqueName="Static" SortExpression="ConditionIsStatic"
                                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
                                        <telerik:GridBoundColumn DataField="ConditionMatchingNumber" SortExpression="ConditionMatchingNumber" HeaderText="Matching Customers"
                                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="20%" />
                                        <telerik:GridBoundColumn DataField="CriteriaCount" SortExpression="CriteriaCount" HeaderText="Criteria Count"
                                            ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
                                        <telerik:GridBoundColumn DataField="SOPName" Display="false" />
                                        <telerik:GridBoundColumn DataField="ConditionUpdateDate" HeaderStyle-HorizontalAlign="Center" DataFormatString="{0:dd/MM/yyyy}" SortExpression="ConditionUpdateDate" HeaderText="Last update" HeaderStyle-Width="15%" />
                                        <telerik:GridBoundColumn DataField="ConditionUpdatedBy" HeaderStyle-HorizontalAlign="Center" SortExpression="ConditionUpdatedBy" HeaderText="Updated by" HeaderStyle-Width="15%" />

                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>
                                <asp:Button ID="AddConditionBtn" runat="server" class="btn bleu rounded" Text="Add Condition" />
                                <asp:Button ID="AddStaticConditionBtn" ClientIDMode="Static" runat="server" class="btn bleu rounded" Text="Assign customers manually" />
                            </span>
                        </td>
                    </tr>
                </table>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="RadPageView2">
                <h4>Please, choose the folder to secure your file</h4>
                <telerik:RadTreeView ID="SecuredRadTreeView" runat="server" Font-Bold="true" OnNodeClick="SecuredRadTreeView_NodeClick">
                    <Nodes>
                        <telerik:RadTreeNode Value="_Private_PersonalFolders" Expanded="true" ExpandMode="ServerSide">
                        </telerik:RadTreeNode>
                    </Nodes>
                </telerik:RadTreeView>
                <asp:Button runat="server" ID="UnsecuredToSecuredBtn" Text="To secure file" class="btn bleu rounded" OnClick="UnsecuredToSecuredBtn_Click" />
                <asp:Label runat="server" ID="UnsecuredToSecuredLabel" Style="color: red" />
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="RadPageView3">
                <table runat="server" id="ThumbnailTable" class="Filters">
                    <tr>
                        <td>
                            <asp:CheckBox runat="server" ID="SetThumbnailDefault" Text=" Use default Thumbnail" />
                        </td>
                        <td>
                            <asp:Image runat="server" ID="ImageThumbnail" />
                            <asp:Label runat="server" ID="ThumbnailGuidLabel" Style="display: none" />
                            <asp:Label runat="server" ID="ThumbnailNameLabel" Style="display: none" />
                            <br />
                            <asp:Label runat="server" ID="ThumbnailInstructionLabel" Width="450px">When uploading a thumbnail, the image must be exactly 100px width and 80px height. No automatic resizing will be performed — the image will be displayed to users exactly as uploaded.</asp:Label>
                            <br />
                            <asp:FileUpload runat="server" ClientIDMode="Static" ID="ThumbnailUpload" CssClass="hidden" onchange="ThumbnailUploadChange(this)" accept="image/*" />
                            <asp:Button type="button" runat="server" ClientIDMode="Static" ID="ThumbnailUploadBtn" Style="display: none" OnClick="ThumbnailUploadBtn_Click" />
                            <asp:Button type="button" runat="server" ID="AddThumbnailUploadBtn" class="btn bleu rounded" Text="Upload Thumbnail" OnClientClick="ThumbnailUploadShow(); return false;" />
                            <br />
                            <asp:Label runat="server" ID="ThumbnailUploadLabel" />
                        </td>
                    </tr>
                </table>
                <table class="Filters">
                    <tr>
                        <td>
                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="PublishDateCheckBox" Text=" Publish file between:" OnClick="PublishDateCheckBoxClick();" />
                        </td>
                        <td>From date
                            <telerik:RadDatePicker runat="server" ID="FromDatePicker" Culture="en-US" DateInput-DisplayDateFormat="dd/MM/yyyy" />
                        </td>
                        <td>To Date
                            <telerik:RadDatePicker runat="server" ID="ToDatePicker" Culture="en-US" DateInput-DisplayDateFormat="dd/MM/yyyy" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="SendNotificationCB" Text=" Send notification email" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button runat="server" ID="ManagePublicationBtnOK" Text="OK" OnClick="ManagePublicationBtnOK_Click" OnClientClick="if(!ManagePublicationBtnOkClientClick()) return false;" class="btn bleu rounded" />
                        </td>
                        <td>
                            <asp:Label runat="server" ClientIDMode="Static" ID="ManagePublicationErrorLabel" />
                        </td>
                    </tr>
                </table>

            </telerik:RadPageView>

        </telerik:RadMultiPage>

    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="ThumbnailUploadBtn" />
    </Triggers>
</asp:UpdatePanel>

<style type="text/css">
    #AddStaticConditionBtn.aspNetDisabled {
        background-color: #c1c1c1 !important;
        cursor: not-allowed !important;
        font-weight: 400;
        border: none !important;
        color: #FFFFFF !important;
        padding: 6px 14px !important;
        text-align: center !important;
        font-family: 'Electrolux_light', sans-serif !important; /* Added a fallback font */
        border-radius: 0px !important;
        font-size: 14px !important;
        text-decoration: none !important;
    }
</style>
<script type="text/javascript">
    function callParentPreview() {
        var idInput = document.getElementById("DocumentGuidLabel").innerHTML;
        if (idInput) {
            loadAndShowPreview(idInput);
        }
        else {
            alert("Could not find the ID to preview.");
        }
    }

    function refreshGrid() {
        var grid = $find("<%= ConditionRg.ClientID %>");
        if (grid) {
            grid.get_masterTableView().rebind();
        }
    }

    function DeleteConditionFocusRange(conditionID) {
        $.ajax({
            type: "POST",
            url: "FocusRangeManagement.aspx/DeleteCondition",
            data: JSON.stringify({ conditionId: conditionID }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d === true) {
                    refreshGrid();
                    } else {
                        alert("Error: Could not delete condition.");
                    }
                }
            });
    }

</script>


