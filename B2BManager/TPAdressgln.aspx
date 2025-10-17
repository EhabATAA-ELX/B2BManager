<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TPAdressgln.aspx.vb" Inherits="TPAdressgln" %>

<%@ Register Src="~/UserControls/TradePlaceCustomer.ascx" TagPrefix="uc1" TagName="TradePlaceCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
     <link href="CSS/Emanager.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:TradePlaceCustomer runat="server" ID="TradePlaceCustomer" />
    <asp:updatepanel runat="server" UpdateMode="Conditional" ID="AdresseGlnOEMPanel" Class="EANPanel" ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:Panel runat="server" ID="PanelDetailCustomer" Visible="false" CssClass="PanelDetailCustomer">
                    <asp:HiddenField runat="server" ID="tpcid" Value=""/>
                <asp:Button ID="BtnNew" runat="server" Text="New" OnClick="BtnNew_Click" Visible="true" UseSubmitBehavior="false" CssClass="btn bleu"></asp:Button>
                <asp:Panel runat="server" ID="PanelAddGLNOEM" Visible="false">
                <asp:Button ID="BtnSave" runat="server" Text="Save" OnClick="BtnSave_Click" Visible="false" UseSubmitBehavior="false" ValidationGroup="NewValueValidation" CssClass="btn bleu"></asp:Button>
                <asp:Button ID="BtnCancel" runat="server" Text="Cancel" OnClick="BtnCancel_Click" Visible="false" UseSubmitBehavior="false" CssClass="btn red"></asp:Button>
                <table>
                    <tr>
                        <td>Address GLN</td>
                        <td>Address OEM</td>
                    </tr>
                    <tr>
                        <td><asp:TextBox runat="server" ID="txtGLN"></asp:TextBox></td>
                        <td><asp:TextBox runat="server" ID="txtOEM"></asp:TextBox></td>                        
                    </tr>
                    <tr>
                        <td><asp:RequiredFieldValidator ID="txtEANValidator" runat="server" ErrorMessage="GLN cannot be blank" ControlToValidate="txtGLN" ForeColor="Red" ValidationGroup="NewValueValidation"></asp:RequiredFieldValidator></td>
                        <td><asp:RequiredFieldValidator ID="txtOEMValidator" runat="server" ErrorMessage="OEM cannot be blank" ControlToValidate="txtOEM" ForeColor="Red" ValidationGroup="NewValueValidation"></asp:RequiredFieldValidator></td>
                                            
                    </tr>
                </table>
            </asp:Panel>
                  <asp:DataList id="dtlTPCustomerGLNOEM" runat="server" Width="100%" GridLines="None" ShowHeader="True"
											AutoGenerateColumns="False">
											<SelectedItemStyle CssClass="SelectedListItem"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="AlternatingRowStyle" />
                                            <ItemStyle CssClass="RowStyle" />
                                            <HeaderTemplate>
                                                        <td>                                                           
                                                            Address GLN
                                                        </td>
                                                        <td>
                                                            Address OEM
                                                        </td>
                                                        <td>
                                                            Action
                                                        </td>
                                            </HeaderTemplate>
											<ItemTemplate>
                                                        <td>
                                                            <%#Container.DataItem("GLN")%>
                                                        </td>
                                                        <td>
                                                            <%#Container.DataItem("ADDRESSKEY")%>
                                                        </td>
                                                        <td>
                                                            <asp:ImageButton ID="BtnDelete" runat="server" Text="Delete" ToolTip="Delete" ImageUrl="./Images/delete.png" OnCommand="BtnDelete_Click" Visible="true" UseSubmitBehavior="false" CommandArgument='<%#Container.DataItem("GLN") %>' Height="20" Width="20"/>
                                                        </td>											
                                            </ItemTemplate>
                        <FooterTemplate>
                            <td colspan="3" style="text-align: center;">                                
                                <asp:Label Visible='<%#Boolean.Parse((dtlTPCustomerGLNOEM.Items.Count = 0).ToString())%>' runat="server" ID="lblNoRecord" Text="No Record Found!"></asp:Label>
                            </td>
                        </FooterTemplate>
				    </asp:DataList>
            </asp:Panel>            
        </ContentTemplate>
         <Triggers>
                <asp:AsyncPostBackTrigger ControlID="TradePlaceCustomer" />
            </Triggers>
    </asp:updatepanel>
</asp:Content>

