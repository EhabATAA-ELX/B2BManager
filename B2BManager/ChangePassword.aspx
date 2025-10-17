<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="ChangePassword.aspx.vb" EnableViewState="true" Inherits="ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .password-form .indicator {
            height: 10px;
            margin: 10px 0;
            display: flex;
            align-items: center;
            justify-content: space-between;
            display: none;
        }

            .password-form .indicator span {
                position: relative;
                height: 100%;
                width: 100%;
                background: lightgrey;
                border-radius: 5px;
            }

                .password-form .indicator span:nth-child(2), .validator-control {
                    margin: 0 3px;
                }

                .password-form .indicator span.active:before {
                    position: absolute;
                    content: '';
                    top: 0;
                    left: 0;
                    height: 100%;
                    width: 100%;
                    border-radius: 5px;
                }

        .indicator span.weak:before {
            background-color: #ff4757;
        }

        .indicator span.medium:before {
            background-color: orange;
        }

        .indicator span.strong:before {
            background-color: #23ad5c;
        }

        .password-form .text {
            display: none;
        }

            .password-form .text.weak {
                color: red;
            }

            .password-form .text.medium {
                color: orange;
            }

            .password-form .text.strong {
                color: #23ad5c;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <table style="margin:15px">
                <tr>
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Confirm your current password:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxCurrentPassword" TextMode="Password" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="Electrolux_light validator-control" runat="server" ErrorMessage="Password is required" ForeColor="Red" ControlToValidate="txtBoxCurrentPassword" ValidationGroup="PasswordCheck"></asp:RequiredFieldValidator>
                    </td>
                </tr>               
                <tr class="container">
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Enter your new password:</asp:Label>
                    </td>
                    <td class="password-form">
                        <span class="field">
                            <asp:TextBox runat="server" ID="txtBoxNewPassword" CssClass="password-input Electrolux_light width230px" TextMode="Password"></asp:TextBox>
                        </span>
                    </td>
                </tr>
                <tr class="container">
                    <td></td>
                    <td class="password-form">
                        <div class="indicator">
                            <span class="weak"></span>
                            <span class="medium"></span>
                            <span class="strong"></span>
                        </div>
                        <div class="text">
                        </div>             
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="Electrolux_light validator-control" runat="server" ErrorMessage="Password is required" ForeColor="Red" ControlToValidate="txtBoxNewPassword" ValidationGroup="PasswordCheck"></asp:RequiredFieldValidator>
                    </td>
                </tr>               
                 <tr>
                    <td>
                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Repeat your new password:</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBoxRepeateNewPassword" TextMode="Password" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>                        
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>                        
                        <asp:CompareValidator id="Compare1" 
                               ControlToValidate="txtBoxNewPassword" 
                               ControlToCompare="txtBoxRepeateNewPassword"
                               EnableClientScript="true"
                               ValidationGroup="PasswordCheck"
                               CssClass="Electrolux_light validator-control"
                               ErrorMessage="Passwords don't match"                               
                               Type="String" 
                               ValidateEmptyText="True"
                               ForeColor="Red"
                               runat="server" />
                    </td>
                </tr>
                 <tr>
                    <td colspan="2" style="text-align:center">                        
                        <asp:CustomValidator ID="CurrentPasswordValidator" CssClass="Electrolux_light validator-control" runat="server"
                            OnServerValidate="CurrentPasswordValidator_ServerValidate"
                            ValidationGroup="PasswordCheck"
                            ForeColor="Red"
                            ControlToValidate="txtBoxCurrentPassword"
                            ErrorMessage="Your current password is invalid">
                        </asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <asp:LinkButton runat="server" ID="btnCancel" OnClientClick="window.parent.ChangePasswordWindow(false);" CausesValidation="false" CssClass="btn red">Cancel</asp:LinkButton>
                        <asp:LinkButton runat="server" CausesValidation="true" OnClientClick="ProcessButton(this,'Updating...')" ID="btnSubmit" ValidationGroup="PasswordCheck" OnClick="btnSubmit_Click" CssClass="btn blue">Submit</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger  ControlID="btnSubmit"/>
        </Triggers>
    </asp:UpdatePanel>
    <script>        
        function onKeyUpNewPassowrd() {
            var indicator = document.querySelector(".indicator");
            var input = document.querySelector(".password-input");
            var weak = document.querySelector(".weak");
            var medium = document.querySelector(".medium");
            var strong = document.querySelector(".strong");
            var text = document.querySelector(".text");
            var regExpWeak = /[a-z]/;
            var regExpMedium = /\d+/;
            $('#ContentPlaceHolder1_CurrentPasswordValidator').hide();
            var no = 0;
            let regExpStrong = /.[!,@,#,$,%,^,&,*,?,_,~,-,(,)]/;
            if (input.value != "") {
                indicator.style.display = "block";
                indicator.style.display = "flex";
                if (input.value.length <= 3 && (input.value.match(regExpWeak) || input.value.match(regExpMedium) || input.value.match(regExpStrong))) no = 1;
                if (input.value.length >= 6 && ((input.value.match(regExpWeak) && input.value.match(regExpMedium)) || (input.value.match(regExpMedium) && input.value.match(regExpStrong)) || (input.value.match(regExpWeak) && input.value.match(regExpStrong)))) no = 2;
                if (input.value.length >= 6 && input.value.match(regExpWeak) && input.value.match(regExpMedium) && input.value.match(regExpStrong)) no = 3;
                if (no == 1) {
                    weak.classList.add("active");
                    text.style.display = "block";
                    text.textContent = "Your password is too weak";
                    text.classList.add("weak");
                }
                if (no == 2) {
                    medium.classList.add("active");
                    text.textContent = "Your password strength is medium";
                    text.classList.add("medium");
                } else {
                    medium.classList.remove("active");
                    text.classList.remove("medium");
                }
                if (no == 3) {
                    weak.classList.add("active");
                    medium.classList.add("active");
                    strong.classList.add("active");
                    text.textContent = "Your password is strong";
                    text.classList.add("strong");
                } else {
                    strong.classList.remove("active");
                    text.classList.remove("strong");
                }
            } else {
                indicator.style.display = "none";
                text.style.display = "none";
            }
        }

        function CloseWindow() {
            window.parent.ChangePasswordWindow(false);
        }
    </script>
</asp:Content>


