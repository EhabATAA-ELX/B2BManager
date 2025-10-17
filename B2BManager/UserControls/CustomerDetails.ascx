<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CustomerDetails.ascx.vb" Inherits="UserControls_CustomerDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="row" style="padding:5px;">
    <div class="col-md-8">
        <div class="card card-primary card-widget">
            <div class="card-header" >
                 <span class="rtsTxt" style="font-size: 13px">Main information</span>
                <div class="card-tools">
                  <button type="button" class="btn-admin btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i>
                  </button>
                </div>
            </div>
            
            <div class="card-body">
                <table cellpadding="5">
                    <tr>
                        <td style="width: 180px">
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Customer name:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBoxCustomerName"  CssClass="Electrolux_light width230px customer-name" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxCustomerName" ControlToValidate="txtBoxCustomerName" ForeColor="Red" ErrorMessage="* mandatory" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td colspan="2">
                            <asp:CheckBox runat="server" AutoPostBack="false" ID="chkBoxOverrideSapImport"
                                Text=" Override customer name" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Customer Code:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBoxCustomerCode" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxCustomerCode" ControlToValidate="txtBoxCustomerCode" ForeColor="Red" ErrorMessage="* mandatory" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Description:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light width230px" TextMode="MultiLine" Height="60" ID="txtBoxDescription"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">TP2 Delivery Block :</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TxtBoxDeliveryBlock" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Linked Soldto ID:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBoxLinkedSoldToID" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Default Menu:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlDefaultMenu" AppendDataBoundItems="true">
                                <asp:ListItem Text="Home" Value="Home"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                

            </div>
        </div>        
    </div>
    <div class="col-md-4">
        <div class="card card-primary">
            <div class="card-header">
                <span class="rtsTxt" style="font-size: 13px">Customer permissions</span>
                <div class="card-tools">
                  <button type="button" class="btn-admin btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i>
                  </button>
                </div>
            </div>
            <div class="card-body">
                <asp:CheckBoxList runat="server" CssClass="checkboxlist" ID="customerRightsChbkoxList" Height="219">
                    <asp:ListItem Text="Broken Promise" Value="C_BROKEN_PROMISE"></asp:ListItem>
                    <asp:ListItem Text="CMIR" Value="C_CMIR"></asp:ListItem>
                    <asp:ListItem Text="Display Focus Range" Value="C_FOCUS_RANGE"></asp:ListItem>
                    <asp:ListItem Text="Expected Price" Value="C_EXPECTED_PRICE"></asp:ListItem>
                    <asp:ListItem Text="Hard Switch to Tradeplace" Value="C_HARDSWITCH"></asp:ListItem>
                    <asp:ListItem Text="PASS Customer" Value="C_PASSAvailability"></asp:ListItem>
                    <asp:ListItem Text="Price Discount" Value="C_PRICE_DISCOUNT"></asp:ListItem>
                    <asp:ListItem Text="Price Scales" Value="C_PRICE_SCALES"></asp:ListItem>
                    <asp:ListItem Text="Allow Other Partners" Value="C_OtherPartners"></asp:ListItem>
                </asp:CheckBoxList>
            </div>
        </div>
    </div>
</div>
<div class="card card-primary">
            <div class="card-header">
                <span class="rtsTxt" style="font-size: 13px">Shopping basket</span>
                <div class="card-tools">
                  <button type="button" class="btn-admin btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i>
                  </button>
                </div>
            </div>
            <div class="card-body">
                <table cellpadding="5">
                    <tr>
                        <td style="width: 180px">
                            <asp:Label runat="server" ID="lblRowsCount" CssClass="Electrolux_light_bold Electrolux_Color">Max lines:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="1" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="radNumericShoppingBasketMaxLines" CssClass="Electrolux_light_bold Electrolux_Color">
                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Total min quantity:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="0" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="radNumericShoppingBasketTotalQuantity" CssClass="Electrolux_light_bold Electrolux_Color">
                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Single line min quantity:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="0" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="radNumericSingleLineMinQuantity" CssClass="Electrolux_light_bold Electrolux_Color">
                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color">Single line max quantity:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" MinValue="1" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="radNumericSingleLineMaxQuantity" CssClass="Electrolux_light_bold Electrolux_Color">
                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                            </telerik:RadNumericTextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

<table width="100%" runat="server" id="actionButtonsTable">
    <tr>
        <td colspan="2" align="center">
            <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" CausesValidation="false" OnClientClick="CloseWindow('Cancel')" ><i class="fas fa-ban"></i> Cancel changes</asp:LinkButton>
            <asp:LinkButton runat="server" CssClass="btn bleu" ID="btnSubmit" CausesValidation="true" OnClick="btnSubmit_ServerClick">Submit changes</asp:LinkButton>
        </td>
    </tr>
</table>
<asp:HiddenField runat="server" ID="HD_EnvID" />
<asp:HiddenField runat="server" ID="HD_C_GlobalID" />
<asp:HiddenField runat="server" ID="HD_SopName" />