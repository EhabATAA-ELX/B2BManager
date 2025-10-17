<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TPTroisMaster.aspx.vb" Inherits="TPTroisMaster" %>

<%@ Register Src="~/UserControls/TradePlaceMasterCustomer.ascx" TagPrefix="uc1" TagName="TradePlaceMasterCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="CSS/Emanager.css" rel="stylesheet" />
     <script>
        <%-- function DeleteConfirm(strObject) {
            if (window.confirm('Do you want to delete ' + strObject + '?')) {                
                $("#<%=BtnDeleteSubmit.ClientID %>").click();
                //__doPostBack('BtnDeleteSubmit','BtnDelete_Click');
            }
        }--%>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">        
        <uc1:TradePlaceMasterCustomer runat="server" ID="TradePlaceMasterCustomer" />
       
          <asp:updatepanel runat="server" UpdateMode="Conditional" ID="TradePlaceMasterCustomerPanel" Class="DefinitionPanel" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:Button ID="BtnNew" runat="server" Text="New" OnClick="BtnNew_Click" CssClass="btn bleu"></asp:Button>
                <asp:Panel runat="server" ID="PanelDetailTPMaster" Visible="false">   
                    <asp:HiddenField ID="TPCustomerID" runat="server" Value=""/>

                    <asp:GridView ID="CustomersAttached" runat="server" AutoGenerateColumns="false" OnRowEditing="CustomersAttached_RowEditing" OnRowDataBound="CustomersAttached_RowDataBound" OnRowUpdating="CustomersAttached_RowUpdating" OnRowCancelingEdit="CustomersAttached_RowCanceling" OnRowDeleting="CustomersAttached_RowDeleting">
                        <Columns>
                             <asp:TemplateField HeaderText="Customer Name">
                                <ItemTemplate>                                    
                                    <asp:HiddenField ID="CustomerCode" runat="server" Value='<%# Eval("CustomerCode") %>' />
                                    <%# Eval("CustomerName") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:HiddenField ID="CustomerCode" runat="server" Value='<%# Eval("CustomerCode") %>' />                                    
                                    <%# Eval("CustomerName") %>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Master Code">
                                <ItemTemplate>
                                     <asp:HiddenField ID="ddlCustomerMasterSelectedValue" runat="server" Value='<%# Eval("MasterCode") %>' />
                                    <%# Eval("Mastername") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:HiddenField ID="ddlCustomerMasterSelectedValue" runat="server" Value='<%# Eval("MasterCode") %>' />
                                    
                                    <asp:DropDownList ID="ddlMasters" Width="100%" AutoPostBack="false"
                                    class="CustomerCodeCell" runat="server" DataTextField="Mastername" DataValueField="MasterCode" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Is Default">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckIsDefault" runat="server" Checked='<%# Bind("isDefault") %>' Enabled="false"/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="ckIsDefault" runat="server" Checked='<%# Bind("isDefault") %>' />                                   
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ButtonType="Image" ShowEditButton="true" EditImageUrl="./Images/edit.png"  ShowDeleteButton="true" DeleteImageUrl="./Images/delete.png" ShowCancelButton="true" CancelImageUrl="./Images/cancel.png" UpdateImageUrl="./Images/save.png" headertext="Edit Controls" EditText="Edit"/>                          
                        </Columns>
                    </asp:GridView>                    
                </asp:Panel>
                    <asp:Panel runat="server" ID="TblAddCustomer" Visible="false">
                         <asp:Button ID="BtnAdd" runat="server" Text="Add" OnClick="BtnAdd_Click" Visible="false" UseSubmitBehavior="false" CssClass="btn bleu"></asp:Button>
                         <asp:Button ID="BtnCancel" runat="server" Text="Cancel" OnClick="BtnCancel_Click" Visible="false" UseSubmitBehavior="false" CssClass="btn red"></asp:Button>
                    <table>
                        <tr>
                            <td>
                                Customer Name
                            </td>
                            <td>
                                 <asp:DropDownList ID="ddlCustomersAdd" Width="100%" AutoPostBack="false"
                                    class="CustomerCodeNameCell" runat="server" DataTextField="CustomerName" DataValueField="CustomerCode"/>
                                <span ID="CustomerLabel" runat="server"></span>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                  Master Code
                            </td>
                            <td>
                                 <asp:DropDownList ID="ddlMastersAdd" Width="100%" AutoPostBack="false"
                                    class="CustomerCodeCell" runat="server" DataTextField="Mastername" DataValueField="MasterCode" />
                        <span ID="CustomerCodeLabel" runat="server"></span>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                 Is Default
                            </td>
                            <td>
                                <asp:CheckBox ID="isDefaultAdd" runat="server"/>
                            </td>
                        </tr>
                    </table>
                        </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="TradePlaceMasterCustomer" />
            </Triggers>
        </asp:updatepanel>

</asp:Content>

