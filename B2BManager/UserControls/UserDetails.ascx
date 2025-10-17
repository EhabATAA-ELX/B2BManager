<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UserDetails.ascx.vb" Inherits="UserControls_UserDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.3.0/font/bootstrap-icons.css" />
<style>
    input.aspNetDisabled {
        background-color: #d3d3d3;
    }

    input.mkt-switch-selector[type=checkbox] {
        height: 0;
        width: 0;
        visibility: hidden;
    }

    label.mkt-switch-selector {
        cursor: pointer;
        text-indent: -9999px;
        width: 30px;
        height: 20px;
        background: grey;
        display: block;
        border-radius: 100px;
        position: relative;
    }

        label.mkt-switch-selector:after {
            content: '';
            position: absolute;
            top: 5px;
            left: 5px;
            width: 9px;
            height: 9px;
            background: #fff;
            border-radius: 9px;
            transition: 0.3s;
        }

    input.mkt-switch-selector:checked + label.mkt-switch-selector {
        background: #bada55;
    }

        input.mkt-switch-selector:checked + label.mkt-switch-selector:after {
            left: calc(100% - 5px);
            transform: translateX(-100%);
        }

    label.mkt-switch-selector:active:after {
        width: 130px;
    }

    .RBL td {
        padding-right: 5px;
    }

    /* Prevents clicks & makes readonly behave like disabled */
    .readonly-disabled {
        pointer-events: none; /* Blocks clicks */
        background-color: #e9ecef; /* Greyed-out background like disabled */
        cursor: not-allowed; /* Shows disabled cursor */
    }
</style>

<script type="text/javascript">
    // Declare global variables
    var electroluxBrand;
    var aegBrand;
    var isElectroluxEmpty;
    var isAegEmpty;

    $(document).ready(function () {
        $('#ContentPlaceHolder1_UserDetails_txtBoxEmailLogin').on('change keyup paste', function () {
            $('#ContentPlaceHolder1_UserDetails_txtBoxWebUserID').val(this.value);
        });

        $("#btnExtUserInformation").on("click", function (event) {
            $(".loading").show();
            CallExternalUserDetailsWebService();
        });

        var $ddl = $('#<%= drpdownChironRole.ClientID %>');
        $ddl.on('change', function () {
            var val = this.value;
            //console.log("role chosen:", val);

            ClearUserRights();
            SetUserRightsByChironRole(val);

            ValidatorEnable(
                document.getElementById('<%= RequiredChironRole.ClientID %>'),
                true
            );
        });

        //set initial state.
        var initialSwitchState = $('.IsMarketplace_TextBox').val().toLowerCase()
        if (initialSwitchState === 'true') {
            $('#switch').prop("checked", true);
            ShowLoginNameControls();
        }
        else {
            $('#switch').prop("checked", false);
            HideLoginNameControls();
        }
        $('.IsMarketplace_TextBox').hide();



        $('#switch').change(function () {

            console.log("Marketplace switch change");

            var returnVal = this.checked;

            $('.IsMarketplace_TextBox').val(returnVal);
            $('.ckhSendActivationEmail input').prop("checked", !returnVal);
            if (returnVal == true) {
                $('.sendNotificationEmailRowSelector').hide();
                ShowLoginNameControls();
            }
            else {
                $('.sendNotificationEmailRowSelector').show();
                HideLoginNameControls();
            }

        });

        var initialSwitchStateChiron = $('.IsChiron_TextBox').val().toLowerCase()
        if (initialSwitchStateChiron === 'true') {
            $('#switchChiron').prop("checked", true);
            EnableChironRolesDrp();
            DisableB2BRightsTable();
        }
        else {
            $('#switchChiron').prop("checked", false);
            ClearChironRolesDrp();
            DisableChironRolesDrp();
            EnableB2BRightsTable();
        }
        $('.IsChiron_TextBox').hide();
        $('#switchChiron').change(function () {
            var returnVal = this.checked;

            $('.IsChiron_TextBox').val(returnVal);
            if (returnVal == true) {
                EnableChironRolesDrp();
                DisableB2BRightsTable();
                console.log("SelectAllChironBrands();");
                SelectAllChironBrands();
            }
            else {
                ClearChironRolesDrp();
                DisableChironRolesDrp();
                EnableB2BRightsTable();
                ClearChironBrands();
            }

        });

        //START Chiron Brand

        //You can show this two line of code to debug , or to see directly what is the value of toggle brand
        //Must be hide when deploying a version 
        $('.ChironBrandAEG').hide();
        $('.ChironBrandElectrolux').hide();

        if ($("#<%= HD_ChironBrandELX.ClientID%>").val() == null || $("#<%= HD_ChironBrandELX.ClientID%>").val().trim() === "") {
            electroluxBrand = $('.ChironBrandElectrolux').val();
            $("#<%= HD_ChironBrandELX.ClientID%>").val(electroluxBrand);
        }
        else {
            electroluxBrand = $("#<%= HD_ChironBrandELX.ClientID%>").val();
        }

        if ($("#<%= HD_ChironBrandAEG.ClientID%>").val() == null || $("#<%= HD_ChironBrandAEG.ClientID%>").val().trim() === "") {
            aegBrand = $('.ChironBrandAEG').val();
            $("#<%= HD_ChironBrandAEG.ClientID%>").val(aegBrand);
        }
        else {
            aegBrand = $("#<%= HD_ChironBrandAEG.ClientID%>").val();
        }

        // Track if one of the brands is missing
        isElectroluxEmpty = (electroluxBrand == null || electroluxBrand.trim() === "");
        isAegEmpty = (aegBrand == null || aegBrand.trim() === "");

        // Handle Electrolux Brand
        if (electroluxBrand === 'ELX') {
            $('#switchChironBrandElectrolux').prop("checked", true);
        } else if (electroluxBrand === 'NOTSELECTED') {
            $('#switchChironBrandElectrolux').prop("checked", false);
        } else if (isElectroluxEmpty) {
            $('.td-brand-electrolux').hide();
        }

        // Handle AEG Brand
        if (aegBrand === 'AEG') {
            $('#switchChironBrandAEG').prop("checked", true);
        } else if (aegBrand === 'NOTSELECTED') {
            $('#switchChironBrandAEG').prop("checked", false);
        } else if (isAegEmpty) {
            $('.td-brand-aeg').hide();
        }

        // Checkbox change events to update values
        $('#switchChironBrandElectrolux').change(function () {
            var returnVal = $(this).is(":checked");
            $('.ChironBrandElectrolux').val(returnVal ? "ELX" : "NOTSELECTED");
            $("#<%= HD_ChironBrandELX.ClientID%>").val($('.ChironBrandElectrolux').val());
        });

        $('#switchChironBrandAEG').change(function () {
            var returnVal = $(this).is(":checked");
            $('.ChironBrandAEG').val(returnVal ? "AEG" : "NOTSELECTED");
            $("#<%= HD_ChironBrandAEG.ClientID%>").val($('.ChironBrandAEG').val());
        });

        DisableOneBrandAvailable();

        //END Chiron Brand

        var initialSwitchStateComn = $('.IsComNorm_TextBox').val().toLowerCase()
        if (initialSwitchStateComn === 'true') {
            $('#switchComNorm').prop("checked", true);
        }
        else {
            $('#switchComNorm').prop("checked", false);
        }
        $('.IsComNorm_TextBox').hide();
        $('#switchComNorm').change(function () {
            var returnVal = this.checked;
            var rfvLoginComNorm = document.getElementById("<%= RequiredFieldValidatorLogintext_ComNorm.ClientID %>");
            var rfvPasswordComNorm = document.getElementById("<%= RequiredFieldValidatorPassword_ComNorm.ClientID %>");
            $('.IsComNorm_TextBox').val(returnVal);
            if (returnVal == true) {
                rfvLoginComNorm.enabled = true;
                rfvPasswordComNorm.enabled = true;
            }
            else {
                rfvLoginComNorm.enabled = false;
                rfvPasswordComNorm.enabled = false;
            }

        });

        var togglePassword = document.getElementById("togglePassword");
        var password = document.getElementById("<%= Password_ComNorm.ClientID %>");

        togglePassword.addEventListener("click", function () {
            // toggle the type attribute
            var type = password.getAttribute("type") === "password" ? "text" : "password";
            password.setAttribute("type", type);

            // toggle the icon
            this.classList.toggle("bi-eye");
        });

    });


    function CallExternalUserDetailsWebService() {
        $.ajax({
            type: 'Get',
            url: '<%=ConfigurationManager.AppSettings("B2BManagerSvcRelativeUrl").ToString()%>' + '/IdDS_SCIM_UserInformation',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: {
                envID: $("#<%= HD_EnvironmentID.ClientID%>").val(),
                SOPID: $("#<%= HD_SOPNAME.ClientID%>").val(),
                IsChiron: $("#<%= IsChiron_TextBox.ClientID%>").val(),
                email: $("#<%= txtBoxEmailLogin.ClientID%>").val()
            },
            async: true,
            success: function (response) {

                $(".loading").hide();

                if (response.totalResults > 0) {
                    var userID = "";
                    var userUUID = "";
                    var mail_verified = "";
                    var failedLOginAttempts = "";
                    var loginTime = "";
                    var setTime = "";
                    var status = "";
                    if (response.resources[0].ieft_params_extension_sap_User) {
                        userID = response.resources[0].ieft_params_extension_sap_User.userId;
                        userUUID = response.resources[0].ieft_params_extension_sap_User.userUuid;
                        mail_verified = response.resources[0].ieft_params_extension_sap_User.mailVerified;
                        if (response.resources[0].ieft_params_extension_sap_User.passwordDetails) {
                            failedLOginAttempts = response.resources[0].ieft_params_extension_sap_User.passwordDetails.failedLOginAttempts,
                                loginTime = response.resources[0].ieft_params_extension_sap_User.passwordDetails.loginTime,
                                setTime = response.resources[0].ieft_params_extension_sap_User.passwordDetails.setTime,
                                status = response.resources[0].ieft_params_extension_sap_User.passwordDetails.status
                        }
                    }
                    ShowExternalUserDetails(
                        response.resources[0].active,
                        response.resources[0].locale,
                        userUUID,
                        userID,
                        mail_verified,
                        failedLOginAttempts,
                        loginTime,
                        setTime,
                        status
                    );
                }

            },
            error: function (e) {
                $(".loading").hide();
                console.log(response);
                alert(e.statusText);
            }
        });
    }

    function ClearChironRolesDrp() {
        var ddl = document.getElementById('<%= drpdownChironRole.ClientID %>');
        ddl.selectedIndex = -1;

        ValidatorEnable(
            document.getElementById('<%= RequiredChironRole.ClientID %>'),
            false
        );
    }

    function DisableChironRolesDrp() {
        $('#<%= drpdownChironRole.ClientID %>').prop('disabled', true);
        DisableAllChironBrands();
    }
    function EnableChironRolesDrp() {
        $('#<%= drpdownChironRole.ClientID %>').prop('disabled', false);
        EnabledAllChironBrands();
    }


    function ClearChironBrands() {
        $('#switchChironBrandElectrolux').prop("checked", false);
        $('#switchChironBrandAEG').prop("checked", false);

        if ($('.ChironBrandElectrolux').val().trim() !== "") {
            $('.ChironBrandElectrolux').val("NOTSELECTED");
            $("#<%= HD_ChironBrandELX.ClientID%>").val("NOTSELECTED");
        }
        if ($('.ChironBrandAEG').val().trim() !== "") {
            $('.ChironBrandAEG').val("NOTSELECTED");
            $("#<%= HD_ChironBrandAEG.ClientID%>").val("NOTSELECTED");
        }
    }

    //Select all chiron brands when user select Is Chiron user
    function SelectAllChironBrands() {

        //Set by default ExtraBasic role when user check Chiron user
        var ddl = document.getElementById('<%= drpdownChironRole.ClientID %>');
        ddl.selectedIndex = 0;
        ClearUserRights();
        SetUserRightsByChironRole(5);

        if (!isElectroluxEmpty) {
            $('#switchChironBrandElectrolux').prop("checked", true);
            $('.ChironBrandElectrolux').val("ELX");
            $("#<%= HD_ChironBrandELX.ClientID%>").val("ELX");
        }

        if (!isAegEmpty) {
            $('#switchChironBrandAEG').prop("checked", true);
            $('.ChironBrandAEG').val("AEG");
            $("#<%= HD_ChironBrandAEG.ClientID%>").val("AEG");
        }

        if (isAegEmpty || isElectroluxEmpty) {
            DisableOneBrandAvailable();
        }
        else if (!isAegEmpty && !isElectroluxEmpty) {
            EnabledAllChironBrands();
        }
    }
    function DisableOneBrandAvailable() {
        if ($('.td-brand-electrolux').is(':hidden')) {
            //If Electrolux is hidden, disable AEG
            DisableChironBrand('.td-brand-aeg');
        }
        if ($('.td-brand-aeg').is(':hidden')) {
            //If AEG is hidden, disable Electrolux
            DisableChironBrand('.td-brand-electrolux');
        }
    }
    function SetUserRightsByChironRole(selectedChironRoleId) {
        $.ajax({
            type: 'Get',
            url: '<%=ConfigurationManager.AppSettings("B2BManagerSvcRelativeUrl").ToString()%>' + '/GetRightsForChironRoleId',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: { ChironRoleID: selectedChironRoleId },
            success: function (response) {
                //console.log(response);

                if (response.length > 0) {
                    EnableB2BRightsTable();
                    $.each(response, function (index, val) {
                        //console.log(index, val)

                        var cbxList = document.getElementById("<%= uerRightsChbkoxList.ClientID %>");
                        var cbx = cbxList.getElementsByTagName("input");
                        var len = cbx.length;
                        for (var i = 0; i < len; i++) {
                            //console.log(cbx[i])
                            if (cbx[i].value == val) {
                                cbx[i].checked = true;
                            }
                        }

                    });

                    DisableB2BRightsTable()

                }

            },
            error: function (e) {
                $(".loading").hide();
                console.log(response);
                alert(e.statusText);
            }
        });
    }

    function DisableB2BRightsTable() {
        $("table[id$=<%= uerRightsChbkoxList.ClientID %>] input").attr("disabled", "disabled");
    }
    function EnableB2BRightsTable() {
        $("table[id$=<%= uerRightsChbkoxList.ClientID %>] input").removeAttr("disabled");
    }

    function DisableAllChironBrands() {
        $(".tbl-chironBrand input, .tbl-chironBrand label, .tbl-chironBrand textarea, .tbl-chironBrand select").prop("readonly", true);

        $(".tbl-chironBrand").css(
            {
                "opacity": "0.5",
                "pointer-events": "none",  // Blocks interaction
                "cursor": "not-allowed"  // Shows disabled cursor
            }
        );
        //Need to check every time to handle case of check/uncheck chiron user%
        CheckOneBrandAvailable();
    }

    function EnabledAllChironBrands() {
        $(".tbl-chironBrand input, .tbl-chironBrand label, .tbl-chironBrand textarea, .tbl-chironBrand select").prop("readonly", false);

        $(".tbl-chironBrand").css(
            {
                "opacity": "1",
                "pointer-events": "auto",  // allow interaction
                "cursor": "default"  // Shows default cursor
            }
        );

        //Need to check every time to handle case of check/uncheck chiron user%
        CheckOneBrandAvailable();
    }

    function DisableChironBrand(selector) {
        $(selector + " input[type='text'], " + selector + " textarea").prop("readonly", true);

        // Reduce opacity for a visually disabled effect
        //$(selector).css("opacity", "0.5");
        $(selector).css(
            {
                "opacity": "0.5",
                "pointer-events": "none",  // Blocks interaction
                "cursor": "not-allowed"  // Shows disabled cursor
            }
        );
    }

    function CheckOneBrandAvailable() {
        if (isElectroluxEmpty && !isAegEmpty) {
            $('#switchChironBrandAEG').prop("checked", true);
            DisableChironBrand('.td-brand-aeg');
        } else if (isAegEmpty && !isElectroluxEmpty) {
            $('#switchChironBrandElectrolux').prop("checked", true);
            DisableChironBrand('.td-brand-electrolux');
        }
    }



    function ClearUserRights() {
        var cbxList = document.getElementById("<%= uerRightsChbkoxList.ClientID %>");
        var cbx = cbxList.getElementsByTagName("input");
        for (var i = 0; i < cbx.length; i++) {
            if (cbx[i].checked) {
                cbx[i].checked = false;
            }
        }
    }


    function HideLoginNameControls() {

        console.log("HideLoginNameControls entered");

        var rfvLoginName = document.getElementById("<%= ReqtxtBoxLoginName.ClientID %>");
        var regexfvLoginName = document.getElementById("<%= rev_txtBoxLoginName.ClientID %>");
        var cvldLoginName = document.getElementById("<%= cvldLoginName.ClientID %>");

        $('.loginNameFieldsSelector').attr('style', 'display: none;');
        rfvLoginName.enabled = false;
        regexfvLoginName.enabled = false;
        cvldLoginName.enabled = false;
    }

    function ShowLoginNameControls() {

        console.log("ShowLoginNameControls entered");

        var LoginNameRequiredForTPInCurrentCountry =  <%=LoginNameRequiredForTPInCurrentCountry%>.toString().toLowerCase();
        console.log(LoginNameRequiredForTPInCurrentCountry);
        if (LoginNameRequiredForTPInCurrentCountry === 'true') {

            console.log("Inside the if");

            var rfvLoginName = document.getElementById("<%= ReqtxtBoxLoginName.ClientID %>");
            var regexfvLoginName = document.getElementById("<%= rev_txtBoxLoginName.ClientID %>");
            var cvldLoginName = document.getElementById("<%= cvldLoginName.ClientID %>");

            $('.loginNameFieldsSelector').attr('style', 'display: inline;');
            rfvLoginName.enabled = true;
            //regexfvLoginName.enabled = true;
            cvldLoginName.enabled = true;

        }
        else {
            HideLoginNameControls();
        }
    }

    function ShowExternalUserDetails(user_is_active, locale, userUUID, userID, mail_verified, failedLOginAttempts, loginTime, setTime, status) {
        $.magnificPopup.open({
            items: {
                src: '<div class="white-popup "></br><p class="text-justify"> '
                    + '<label for="user_is_active">User is active : </label>'
                    + '<span id="user_is_active">' + user_is_active + '</span><br />'

                    + '<label for="locale">Locale : </label>'
                    + '<span id="locale">' + locale + '</span><br />'

                    + '<label for="userUUID">User UUID : </label>'
                    + '<span id="userUUID">' + userUUID + '</span><br />'

                    + '<label for="userID">User ID : </label>'
                    + '<span id="userID">' + userID + '</span><br />'

                    + '<label for="mail_verified">Email verified : </label>'
                    + '<span id="mail_verified">' + mail_verified + '</span><br />'

                    + '<label for="failedLOginAttempts">Password details - Failed Login Attempts : </label>'
                    + '<span id="failedLOginAttempts">' + failedLOginAttempts + '</span><br />'
                    + '<label for="loginTime">Password details - Last Login Time : </label>'
                    + '<span id="loginTime">' + loginTime + '</span><br />'
                    + '<label for="setTime">Password details - Password set time : </label>'
                    + '<span id="setTime">' + setTime + '</span><br />'
                    + '<label for="status">Password details - Password status : </label>'
                    + '<span id="status">' + status + '</span><br />'



                    + '</p>'
                    + '</br> </div> ',
                type: 'inline'
            },
            callbacks: {
                close: function () {
                    window.close();
                }
            }
        });
    }

</script>
<table style="width: 100%;" cellpadding="2">
    <tr>
        <td style="vertical-align: top; width: 65%; min-width: 450px">
            <div class="card card-primary">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">Main information</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-minus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <table cellpadding="2">
                        <tr valign="center" runat="server" id="Tr1">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Is tradeplace user:</asp:Label>
                            </td>
                            <td>
                                <input type="checkbox" id="switch" class="mkt-switch-selector" />
                                <label for="switch" class="mkt-switch-selector">Toggle</label>
                                <asp:TextBox ID="IsMarketplace_TextBox" runat="server" CssClass="IsMarketplace_TextBox" />
                            </td>
                            <td>
                                <div>
                                    <asp:Button ID="btnImpersonate" runat="server" Text="Impersonate" CausesValidation="False" UseSubmitBehavior="false" Visible="false" />
                                </div>
                            </td>
                        </tr>
                        <tr valign="top" runat="server" id="displayNameTR">
                            <td style="width: 200px">
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Display name (*):</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxDisplayName" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxDisplayName" ControlToValidate="txtBoxDisplayName" ForeColor="Red" ErrorMessage="* mandatory" />
                            </td>
                        </tr>
                        <tr valign="top" runat="server" id="loginTR">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color loginNameFieldsSelector">Login name :</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxLoginName" CssClass="Electrolux_light_bold width230px loginNameFieldsSelector" runat="server" MaxLength="64"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="ReqtxtBoxLoginName"
                                    Enabled="false" ControlToValidate="txtBoxLoginName"
                                    ForeColor="Red" ErrorMessage="* mandatory" />
                                <asp:RegularExpressionValidator ID="rev_txtBoxLoginName" runat="server" Enabled="false"
                                    ControlToValidate="txtBoxLoginName"
                                    ForeColor="Red"
                                    ValidationExpression="^[a-zA-Z0-9_\+\-;\.\$\^\@]{1,64}$"
                                    Display="Dynamic" ErrorMessage="Invalid Login name. You have used characters that are not allowed. Max 64 characters." />
                                <asp:CustomValidator runat="server" ID="cvldLoginName"
                                    Display="Dynamic" ControlToValidate="txtBoxLoginName"
                                    ForeColor="Red" />
                            </td>
                        </tr>
                        <tr valign="top" runat="server" id="emailLoginTR">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Login email (*):</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxEmailLogin" CssClass="Electrolux_light_bold width230px" runat="server" autocomplete="off" MaxLength="255"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnSetMissingLoginEmail" runat="server" Text="Add" CausesValidation="False" UseSubmitBehavior="false" />
                                <asp:Button ID="btnResetPassword" runat="server" Text="Generate password reset email" CausesValidation="False" Visible="false" UseSubmitBehavior="false" />
                                <asp:RequiredFieldValidator runat="server" ID="rfv_txtBoxEmailLogin" ControlToValidate="txtBoxEmailLogin" ForeColor="Red" ErrorMessage="* mandatory" />
                                <asp:RegularExpressionValidator ID="rev_txtBoxEmailLogin" runat="server" ControlToValidate="txtBoxEmailLogin"
                                    ForeColor="Red"
                                    ValidationExpression="^[a-zA-Z0-9]{1,}[a-zA-Z0-9-._\+]{0,}@([a-zA-Z0-9]{1,}([a-zA-Z0-9-._])+\.)+[a-zA-Z0-9]{2,8}$"
                                    Display="Dynamic" ErrorMessage="Invalid Email" />

                            </td>
                        </tr>

                        <tr valign="top" runat="server" id="confirmEmailLoginTR">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Confirm login email (*):</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxConfirmLoginEmail" CssClass="Electrolux_light_bold width230px" runat="server" autocomplete="off"></asp:TextBox>
                            </td>
                            <td>
                                <asp:CustomValidator runat="server" Display="Dynamic" ControlToValidate="txtBoxConfirmLoginEmail" ID="vldEmail" ForeColor="Red"></asp:CustomValidator>
                            </td>
                        </tr>

                        <tr valign="top" runat="server" id="sendActivationEmailTR" class="sendNotificationEmailRowSelector">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Send activation link email:</asp:Label>
                            </td>
                            <td style="height: 38px">
                                <asp:CheckBox ID="ckhSendActivationEmail" runat="server" CssClass="ckhSendActivationEmail" />
                                <div style="padding: 4px 0 4px 0;">
                                    <i class="fas fa-exclamation-triangle"></i>
                                    If the checkbox is left unticked you will have to notify the user so that she/he can set a password for this account.
                                </div>
                            </td>
                            <td></td>
                        </tr>

                        <tr valign="top" runat="server" id="externalAccountTR">
                            <td></td>
                            <td>
                                <div id="btnExtUserInformation" class="get_Details defaultLink">get SapCloudIdentity user details</div>
                                <img class="loading" src='Images/Loading.gif' height='22px' width='22px' style="display: none;" />
                            </td>
                            <td></td>
                        </tr>

                        <tr valign="top" runat="server" id="emailTR">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Notifications Email:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxEmail" CssClass="Electrolux_light width230px" runat="server" autocomplete="off"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtBoxEmail"
                                    ForeColor="Red"
                                    ValidationExpression="^[a-zA-Z0-9]{1,}[a-zA-Z0-9-._\+]{0,}@([a-zA-Z0-9]{1,}([a-zA-Z0-9-._])+\.)+[a-zA-Z0-9]{2,8}$"
                                    Display="Dynamic" ErrorMessage="Invalid Email" />
                                <div runat="server" visible="false" id="Div_uniqueEmail">
                                    <a id="BtnDislayUserswithsameLogin" onclick="LoadContent()" style="cursor: pointer;"><i class="fas fa-exclamation-triangle" style="color: #0094ff; margin-top: 2px;"></i></a>
                                </div>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Web User ID (*):</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtBoxWebUserID" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator runat="server" ID="ReqTxtBoxWebUserID" ControlToValidate="txtBoxWebUserID" ForeColor="Red" ErrorMessage="* mandatory" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Phone No:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPhoneNo" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">User Type:</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlManagementType" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Real User" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Test User" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trSuperUserCategory">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Category (*):</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlSuperUserCategory" AppendDataBoundItems="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="expirationDateTR" visible="false">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Expiration Date:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadDateTimePicker ID="radDateTimeExpirationDate" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server" CssClass="width230px">
                                    <Calendar runat="server">
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today">
                                                <ItemStyle CssClass="rcToday" />
                                            </telerik:RadCalendarDay>
                                        </SpecialDays>
                                    </Calendar>
                                </telerik:RadDateTimePicker>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Administrator:</asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="chkboxAdministrator" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Internal:</asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="chkboxInternal" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Default Menu:</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlDefaultMenu" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Home" Value="Home"></asp:ListItem>
                                    <asp:ListItem Text="Agreements" Value="AGREEMENTS"></asp:ListItem>
                                    <asp:ListItem Text="Contacts" Value="CONTACTS"></asp:ListItem>
                                    <asp:ListItem Text="Download" Value="DOWNLOAD"></asp:ListItem>
                                    <asp:ListItem Text="Media Download Center" Value="MEDIA_DOWNLOAD_CENTER"></asp:ListItem>
                                    <asp:ListItem Text="Fast Order" Value="MULTILINE"></asp:ListItem>
                                    <asp:ListItem Text="Product Catalog" Value="Nav_ProductAndOrder"></asp:ListItem>
                                    <asp:ListItem Text="News" Value="NEWS"></asp:ListItem>
                                    <asp:ListItem Text="Order Search" Value="ORDERSEARCH"></asp:ListItem>
                                    <asp:ListItem Text="Order Status" Value="ORDERSTATUS"></asp:ListItem>
                                    <asp:ListItem Text="User Preferences" Value="USER_PREFERENCE"></asp:ListItem>
                                    <asp:ListItem Text="Energy Label Search" Value="FICHE_LABEL"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr runat="server" id="trSalesRep" visible="false">
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Sales rep:</asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="cbSalesRep" AutoPostBack="true" />
                                <asp:TextBox runat="server" ID="txtSalesRep" />
                            </td>
                        </tr>
                    </table>
                </div>
                <!-- /.card-body -->
            </div>


            <div class="card card-primary " id="DivChiron" runat="server" visible="true">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">Chiron</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <table cellpadding="2">
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Is Chiron user:</asp:Label>
                            </td>
                            <td>
                                <input type="checkbox" id="switchChiron" class="mkt-switch-selector" />
                                <label for="switchChiron" class="mkt-switch-selector">Toggle</label>
                                <asp:TextBox ID="IsChiron_TextBox" runat="server" CssClass="IsChiron_TextBox" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 200px">
                                <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color">Chiron role:</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="drpdownChironRole" AppendDataBoundItems="true">
                                    <asp:ListItem Text="ExtraBasic" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="Basic" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="Medium" Value="20"></asp:ListItem>
                                    <asp:ListItem Text="MediumPlus" Value="24"></asp:ListItem>
                                    <asp:ListItem Text="AdvancedLight" Value="27"></asp:ListItem>
                                    <asp:ListItem Text="Advanced" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="Admin" Value="40"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredChironRole" runat="server" ControlToValidate="drpdownChironRole"
                                    InitialValue=""
                                    ErrorMessage="* mandatory"
                                    ForeColor="Red"
                                    Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Chiron brand(s):</asp:Label>
                            </td>
                            <td>
                                <table class="tbl-chironBrand">
                                    <tr>
                                        <td class="td-brand-electrolux">
                                            <input type="checkbox" id="switchChironBrandElectrolux" class="mkt-switch-selector" />
                                            <label for="switchChironBrandElectrolux" class="mkt-switch-selector" style="display: inline-block">Toggle</label>
                                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Electrolux</asp:Label>
                                            <asp:TextBox ID="txtChironBrandElectrolux" runat="server" CssClass="ChironBrandElectrolux" />
                                        </td>
                                        <td class="td-brand-aeg">
                                            <input type="checkbox" id="switchChironBrandAEG" class="mkt-switch-selector" />
                                            <label for="switchChironBrandAEG" class="mkt-switch-selector" style="display: inline-block">Toggle</label>
                                            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">AEG</asp:Label>
                                            <asp:TextBox ID="txtChironBrandAEG" runat="server" CssClass="ChironBrandAEG" />
                                        </td>
                                    </tr>
                                </table>
                            </td>

                        </tr>
                    </table>
                </div>
                <!-- /.card-body -->
            </div>
            <div class="card card-primary collapsed-card" id="DivComNorm" runat="server" visible="false">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">ComNorm</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <table cellpadding="2">
                        <tr>
                            <td>
                                <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Is ComNorm user:</asp:Label>
                            </td>
                            <td>
                                <input type="checkbox" id="switchComNorm" class="mkt-switch-selector" />
                                <label for="switchComNorm" class="mkt-switch-selector">Toggle</label>
                                <asp:TextBox ID="Is_ComNorm_user" runat="server" CssClass="IsComNorm_TextBox" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 200px">
                                <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">ComNorm Login:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Logintext_ComNorm" CssClass="Electrolux_light width230px" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorLogintext_ComNorm" Enabled="false" ControlToValidate="Logintext_ComNorm" ForeColor="Red" ErrorMessage="* mandatory" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color">ComNorm Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Password_ComNorm" CssClass="Electrolux_light width230px" runat="server" TextMode="Password"></asp:TextBox>
                                <i class="bi bi-eye-slash" id="togglePassword"></i>
                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorPassword_ComNorm" Enabled="false" ControlToValidate="Password_ComNorm" ForeColor="Red" ErrorMessage="* mandatory" />

                            </td>
                        </tr>
                    </table>
                </div>
                <!-- /.card-body -->
            </div>
            <div class="card card-primary">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">Shopping basket</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-minus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <table cellpadding="2">
                        <tr>
                            <td style="width: 200px">
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
                                <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Signle line max quantity:</asp:Label>
                            </td>
                            <td>
                                <telerik:RadNumericTextBox runat="server" MinValue="0" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="radNumericShoppingBasketLineQty" CssClass="Electrolux_light_bold Electrolux_Color">
                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                    </table>
                </div>
                <!-- /.card-body -->
            </div>
        </td>
        <td style="vertical-align: top; width: 35%;">
            <div class="card card-primary">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">Access rights</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-minus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <asp:CheckBoxList runat="server" CssClass="checkboxlist" ID="uerRightsChbkoxList" Height="373">
                    </asp:CheckBoxList>
                </div>
                <!-- /.card-body -->
            </div>
        </td>
    </tr>
    <tr>
        <td style="vertical-align: top;" colspan="2">
            <div class="card card-primary">
                <div class="card-header">
                    <span class="rtsTxt" style="font-size: 13px">Enabled order types</span>
                    <div class="card-tools">
                        <button type="button" class="btn-admin btn-tool" data-card-widget="collapse">
                            <i class="fas fa-minus"></i>
                        </button>
                    </div>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <asp:UpdatePanel runat="server" ID="updatePanelListGWSGroup">
                        <ContentTemplate>
                            <table>
                                <tr>
                                    <td style="width: 200px">
                                        <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">Order type definition group (*):</asp:Label>
                                    </td>
                                    <td>
<div style="display:none">
                                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlGWSGroup" OnSelectedIndexChanged="ddlGWSGroup_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                        </asp:DropDownList>
                                    </td>
</div>
                                </tr>
                                <tr>
                                    <td colspan="2">
<div style="display:none">
                                        <asp:CheckBoxList ID="chkboxListGWSGroup" CssClass="checkboxlist" CellPadding="2" Enabled="false" CellSpacing="2" RepeatColumns="6" RepeatDirection="Horizontal" TextAlign="Right" runat="server" Height="100">
                                            <asp:ListItem Text="Trade order" Value="44,45,46,47,77,88"></asp:ListItem>
                                            <asp:ListItem Text="SC04 Home Delivery " Value="49,45,46,48"></asp:ListItem>
                                            <asp:ListItem Text="Site order" Value="50,46,47,48"></asp:ListItem>
                                            <asp:ListItem Text="Alternative address" Value="43"></asp:ListItem>
                                            <asp:ListItem Text="Consignment" Value="76,77"></asp:ListItem>
                                            <asp:ListItem Text="Trade display" Value="78"></asp:ListItem>
                                            <asp:ListItem Text="Employee order" Value="75"></asp:ListItem>
                                            <asp:ListItem Text="Employee alternative address" Value="80"></asp:ListItem>
                                            <asp:ListItem Text="Pick-up" Value="87,88"></asp:ListItem>
                                            <asp:ListItem Text="Employee alternative pick-up" Value="89"></asp:ListItem>
                                            <asp:ListItem Text="Next day delivery" Value="91"></asp:ListItem>
                                            <asp:ListItem Text="Spares Warranty Order" Value="92"></asp:ListItem>
                                            <asp:ListItem Text="SC07 Home Delivery" Value="93"></asp:ListItem>
                                        </asp:CheckBoxList>
</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBoxList ID="chkboxListOrderTypes" CssClass="checkboxlist" CellPadding="2" CellSpacing="2" RepeatColumns="6" RepeatDirection="Horizontal" TextAlign="Right" runat="server" Height="100">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlGWSGroup" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <!-- /.card-body -->
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="left" style="padding-left: 25px; font-size: 8pt !important">
            <asp:Label runat="server" CssClass="Electrolux_light_bold Electrolux_Color">(*) this field is mondatory</asp:Label>
        </td>
    </tr>
    <tr runat="server" id="ButtonsTR">
        <td colspan="2" align="center">
            <asp:LinkButton runat="server" CssClass="btn red" ID="btnCancel" CausesValidation="false" OnClientClick="CloseWindow('Cancel')"><i class="fas fa-ban"></i> Cancel changes</asp:LinkButton>
            <asp:Button runat="server" ID="btnSubmit" class="btn bleu" Text="Submit changes" OnClick="btnSubmitForm_Click" />
            <%--<i class="fa fa-spinner fa-spin" style="font-size:24px"></i>--%>
        </td>
    </tr>
    <tr>
        <asp:HiddenField runat="server" ID="HD_C_GlobalID" />
        <asp:HiddenField runat="server" ID="HD_CustomerCode" />
        <asp:HiddenField runat="server" ID="HD_EnvironmentID" />
        <asp:HiddenField runat="server" ID="HD_SOPNAME" />
        <asp:HiddenField runat="server" ID="HD_Setup_Type" />
        <asp:HiddenField runat="server" ID="HD_U_GlobalID" />
        <asp:HiddenField runat="server" ID="HD_Mode" />
        <asp:HiddenField runat="server" ID="HD_InitialLoginEmail" />
        <asp:HiddenField runat="server" ID="HD_InitialState_Is_Marketplace" />
        <asp:HiddenField runat="server" ID="HD_DatabaseLoginName" />
        <asp:HiddenField runat="server" ID="HD_ExtUserLocation" />
        <asp:HiddenField runat="server" ID="HD_ExtUserSendActivationEmail" />
        <asp:HiddenField runat="server" ID="HD_txtLoginName_Editable" Value="False" />
        <asp:HiddenField runat="server" ID="HD_txtEmailLogin_Editable" Value="False" />
        <asp:HiddenField runat="server" ID="HD_U_GWSID" />
        <asp:HiddenField runat="server" ID="HD_MAIN_URL" />
        <asp:HiddenField runat="server" ID="HD_ChironBrandAEG" />
        <asp:HiddenField runat="server" ID="HD_ChironBrandELX" />
    </tr>
</table>



