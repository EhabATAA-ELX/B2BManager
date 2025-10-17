<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link href="CSS/Login/bootstrap.min.css" rel="stylesheet" />
    <link href="CSS/Login/main.css?v=1.1" rel="stylesheet" />
    <link href="CSS/Login/util.css" rel="stylesheet" />
    <style type="text/css">
        #logoImg {
            height: 46px !important;
        }

        #tdLogo {
            padding: 10px !important;
        }


        .container-login100 {            
            background-image: url('<%= GetBackgroundImageUrl %>');
            opacity: <%= GetOpacityAttribute() %>;
        }
    </style>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="limiter">
        <div class="container-login100">
            <div class="wrap-login100 p-l-55 p-r-55 p-t-65 p-b-50 card">
                <form class="login100-form validate-form card-body login-card-body">
                    <span class="login100-form-title p-b-33">Sign in to start your session
                    </span>
                    
                    <div class="wrap-input100 input-group mb-3">
                        <asp:TextBox class="input100 form-control" type="text" runat="server" ID="UserName" name="login" placeholder="Login"></asp:TextBox>
                        <div class="input-group-append">
                            <div class="input-group-text">
                                <span class="fas fa-envelope"></span>
                            </div>
                        </div>
                    </div>

                    <div class="wrap-input100 rs1 input-group mb-3">
                        <asp:TextBox class="input100 form-control" runat="server" ID="Password" type="password" name="pass" placeholder="Password"></asp:TextBox>
                        <div class="input-group-append">
                            <div class="input-group-text">
                                <span class="fas fa-lock"></span>
                            </div>
                        </div>
                    </div>
                    <div>
                        <asp:Label ID="InvalidCredentialsMessage" runat="server" ForeColor="Red" Text="Login failed, please check your credentials and try again."
                            Visible="False"></asp:Label>
                    </div>
                    <div class="container-login100-form-btn m-t-20">
                        <asp:Button ID="LoginButton" runat="server" CssClass="login100-form-btn btn bleu btn-primary btn-block" Text="Login" OnClick="LoginButton_Click" />
                    </div>

                </form>
            </div>
        </div>
    </div>

</asp:Content>

