<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TPDefinition.aspx.vb" Inherits="TPDefinition" %>

<%@ Register Src="~/UserControls/TradePlaceCustomer.ascx" TagPrefix="uc1" TagName="TradePlaceCustomer" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
    <link href="CSS/Emanager.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/Jquery.amsify.suggestags/jquery.amsify.suggestags.js"></script>
    <link href="Scripts/Jquery.amsify.suggestags/amsify.suggestags.css" rel="stylesheet" />
    <script>

        function AddAmsifySuggest(disable,CustomerWhiteList) {

            var CountryList = JSON.parse(CountryListBinded);
            var SimpleListCountry = Object.keys(CountryList);
            $('[id$="countryList"]').amsifySuggestags({}, 'destroy');
             $('[id$="countryList"]').amsifySuggestags({
                 type: 'amsify',
                 suggestions: SimpleListCountry,
                 whiteList: true,
                 disabled:disable
            });

            var CustomerAffectedSuggestion = [];
            if (CustomerWhiteList != "") {
                CustomerAffectedSuggestion = CustomerWhiteList;
            }

            $('[id$="AddCustomerTextBoxe"]').amsifySuggestags({}, 'destroy');
             $('[id$="AddCustomerTextBoxe"]').amsifySuggestags({
                    type: 'amsify',
                    whiteList: true,
                    disabled: disable,
                    suggestions:CustomerAffectedSuggestion,
                    suggestionsAction : {
                        url: 'TPDefinition.aspx/GetCustomer',
                        AdditionnalParam: true,
                        AdditionnalParamList: { "Country":"[id$='countryList']","Environement":"[id$='ddlEnvironment']"}
	                }
                });

        }

        function BindAmsifyHiddenFields() {
            BindCountry();
            BindCustomer();
        }

        function BindCountry() {
            var CountryList = JSON.parse(CountryListBinded);
            var CountrySelectedIsoCode ="";

            if ($('[id$="countryList"]').val() != "") {            
                var CountrySelected = $('[id$="countryList"]').val().split(',');

                CountrySelected.forEach(function (element) {
                    CountrySelectedIsoCode += CountryList[element]+",";
                });
                $('[id$="countryListHidden"]').val(CountrySelectedIsoCode);
            }
        }

        function BindCustomer() {
            var TextBoxeValues = $('[id$="AddCustomerTextBoxe"]').val().split(",");
              var values = jQuery.map(TextBoxeValues, function (value) {
                var countValue = value.split("_").length;
                if (countValue > 1) {
                    return (value.split("_")[1]);
                } else {
                    return (value);
                }
            });
             $('[id$="AddCustomerTextBoxeHidden"]').val(values);
        }

         function DeleteConfirm(strObject) {
            if (window.confirm('Do you want to delete ' + strObject + '?')) {                
                $("#<%=BtnDeleteSubmit.ClientID %>").click();
            }
        }

        function CountryValidation(source, args) {
            if ($(".amsify-suggestags-input-area:first").children("span").length == 0) {                
                args.IsValid = false;
            } 
        }

        function TPIDValidation(source,args) {
            args.IsValid = false;
            
            if ($("[id$='txtTPIDHTTP']").val().length > 0) {
                 args.IsValid = true;
            }

            if ($("[id$='txtTPIDSMTP']").val().length > 0) {
                 args.IsValid = true;
            }
        }
    </script>    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">    
        <uc1:TradePlaceCustomer runat="server" ID="TradePlaceCustomer" />
    
        <asp:updatepanel runat="server" UpdateMode="Conditional" ID="DefinitionPanel" Class="DefinitionPanel" ChildrenAsTriggers="false">
            <ContentTemplate>      
                 <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="BottomCenter">
        <TargetControls>
                <telerik:ToolTipTargetControl TargetControlID="ImgTooltipAddCustomer" />
            </TargetControls>
        </telerik:RadToolTipManager>
                <asp:Button ID="BtnNew" runat="server" Text="New" OnClick="BtnNew_Click" Visible="True" UseSubmitBehavior="false" CssClass="btn bleu"></asp:Button>            
                <asp:Panel runat="server" ID="PanelDetailCustomer">
                <div id="message" runat="server"></div>
                <div style="width:100%;display:block">
                    <asp:Button ID="BtnEdit" runat="server" Text="Edit" OnClick="BtnEdit_Click" Visible="false" UseSubmitBehavior="false" CssClass="btn green"></asp:Button>
                    <asp:Button ID="BtnSave" runat="server" Text="Save" OnClientClick="BindAmsifyHiddenFields();" OnClick="BtnSave_Click" Visible="false" UseSubmitBehavior="false" ValidationGroup="CustomValidatorForCountryGroup" CssClass="btn bleu"></asp:Button>
                    <asp:Button ID="BtnCancel" runat="server" Text="Cancel" OnClick="BtnCancel_Click" Visible="false" UseSubmitBehavior="false" CssClass="btn red"></asp:Button>
                    <asp:Button ID="BtnAdd" runat="server" Text="Add" OnClientClick="BindAmsifyHiddenFields();" OnClick="BtnAdd_Click" Visible="false" UseSubmitBehavior="false" ValidationGroup="CustomValidatorForCountryGroup" CssClass="btn bleu"></asp:Button>
                    <asp:Button ID="BtnDelete" runat="server" Text="Delete" autopostback="false" OnClientClick="DeleteConfirm('TradePlace customer');return false;" Visible="false" CssClass="btn red" UseSubmitBehavior="false"/>
                    <asp:Button ID="BtnDeleteSubmit" runat="server" Text="" autopostback="false" OnClick="BtnDelete_Click" Style="display:none;" Enabled="false" UseSubmitBehavior="false"/>
                    <asp:HiddenField ID="TPCustomerID" runat="server" Value=""/>
                </div>
                <div style="display:inline-block;">
                    <table id="CustomerDetail" runat="server" visible="false">
                        <tr>
                            <td><span>Name</span></td>
                            <td><asp:TextBox ID="txtTPCName" runat="server" Enabled="false" style="width: 100%;"></asp:TextBox>                             
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><asp:RequiredFieldValidator ID="txtTPCNameValidator" runat="server" ErrorMessage="Name cannot be blank" ControlToValidate="txtTPCName" ForeColor="Red" ValidationGroup="CustomValidatorForCountryGroup"></asp:RequiredFieldValidator> </td>
                        </tr>
                        <tr>
                            <td><span>TPID HTTP</span></td>
                            <td> <asp:TextBox ID="txtTPIDHTTP" runat="server" Enabled="false" style="width: 100%;"></asp:TextBox>                                
                               
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"> 
                                <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="TPIDValidation" ErrorMessage="At least one TPID is required!" ValidationGroup="CustomValidatorForCountryGroup" ForeColor="Red"></asp:CustomValidator>

                            </td>
                        </tr>
                        <tr>
                            <td><span>TPID SMTP</span></td>
                            <td><asp:TextBox ID="txtTPIDSMTP" runat="server" Enabled="false" style="width: 100%;"></asp:TextBox>
                                
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtTPIDHTTP" ControlToValidate="txtTPIDSMTP" Operator="NotEqual" ErrorMessage="TPID HTTP and TPID SMTP can't be the same" ForeColor="Red" ValidationGroup="CustomValidatorForCountryGroup"></asp:CompareValidator> 
                            </td>
                        </tr>
                        <tr>
                            <td><span>Country</span></td>
                            <td>
                                <asp:TextBox type="text" class="form-control" name="country" id="countryList" runat="server"/>
                                <asp:HiddenField id="countryListHidden" runat="server"/>
                                <asp:CustomValidator ID="CustomValidatorForCountry" runat="server" ClientValidationFunction="CountryValidation" ValidationGroup="CustomValidatorForCountryGroup" ErrorMessage="At least one country is required!" ForeColor="Red"></asp:CustomValidator>
                            </td>
                            <td>                              

                            </td>
                        </tr>
                    </table>
                    </div>
                <div style="display:inline-block;vertical-align:top;" id="RightPanel" runat="server" visible="false">
                    <div style="display:inline-block;vertical-align:top;">                           
                        <span>Add Customer</span>
                        <img src='Images/Info.png' id="ImgTooltipAddCustomer" width="18" height="18" alt="More details" title="More details" runat="server"/>
                        <telerik:RadToolTip RenderMode="Lightweight" ID="TooltipContentAddCustomer" ClientIDMode="Static" runat="server" TargetControlID="ImgTooltipAddCustomer" RelativeTo="Element" Position="BottomCenter" RenderInPageRoot="true">             
                                    <b>Function details:</b><br />
                                    This tool allow you to add customer.
                        </telerik:RadToolTip >
                        <asp:TextBox ID="AddCustomerTextBoxe" runat="server" class="form-control"></asp:TextBox>     
                        <asp:HiddenField id="AddCustomerTextBoxeHidden" runat="server"/>

                    </div>
                    <div style="display:inline-block;vertical-align:top;margin-left: 10px;">
                        <span>Add PushStock</span>
                        <asp:TextBox ID="AddPushStock" runat="server" Enabled="False"></asp:TextBox>
                        <asp:Button  ID="AddPushStockButton" src="Images/EManager/But_ArrowRight.gif" runat="server" OnClick="ImageAddPushStock_Click" Enabled="false" CssClass="verticalAlignBottom AddButtonTPDefinition" UseSubmitBehavior="false"/>                        
                          <asp:DataList ID="dtlAffectedPushStock" runat="server" Width="100%" OnSelectedIndexChanged="dtlAffectedPushStock_SelectedIndexChanged" 
                                                                    CellPadding="0" Enabled="false">
                                <SelectedItemStyle CssClass="SelectedListItem"></SelectedItemStyle>
                                <HeaderTemplate>
                                    <table>
                                        <tr>
                                            <td class="width200px">Affected PushStock</td>              
                                            <td class="width200px TextAlignCenter">Is active</td>    
                                        </tr>
                                    </table>
                                                 
                                </HeaderTemplate>
                                <AlternatingItemStyle Wrap="False" CssClass="AlternatingItemStyle"></AlternatingItemStyle>
                                <ItemStyle Wrap="False" CssClass="ItemStyle"></ItemStyle>
                                <ItemTemplate>  
                                    <table>
                                        <tr>
                                            <td class="width200px">
                                                <input id="HdnPushStock" type="hidden" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "C_GLOBALID").ToString%>' name="AffectedC_ID"/>
                                                <asp:LinkButton ID="PushStockLine" runat="server" CssClass="ListItem1" CommandName="select" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "C_GlobalID").ToString%>' ><%# DataBinder.Eval(Container.DataItem, "C_CUID")%></asp:LinkButton>
                                            </td>
                                            <td class="width200px TextAlignCenter">
                                                <asp:CheckBox ID="chk_Actif" CssClass="ListItem_Check" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "IS_ACTIF")%>'/>
                                            </td>
                                        </tr>
                                    </table>                                                                        
                                </ItemTemplate>
                            </asp:DataList>
                    </div>
                </div>
                    </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="TradePlaceCustomer" />
            </Triggers>
        </asp:updatepanel>         

</asp:Content>
