<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TPQueryAnalyzer.aspx.vb" Inherits="TPQueryAnalyzer" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="CSS/Emanager.css" rel="stylesheet" />
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <script>

        $(document).ready(function () {
            AddHandleEvents();
            BindQuery();
            //$('#poctotolamenace').DataTable({
            //    ordering: false,
            //    searching: true
            //});          
        });

        function BindDataTable() {
              var table = $("[id$='PocJQueryDataTable']").DataTable({
                ordering: false,
                searching: true,
                colReorder: true,
                autoWidth: false
            });
            table.colReorder.order([0,1,13,2,3,4,5,14,15,9,10,6,7,8,11,12]);
        }

        function AddHandleEvents() {

            AddTransacEvents();
            AddHubSpanEvents();
            //AddPendingEvents();
            AddShippingNotesEvents();
            AddFromToDateEvents();
            AddBusinessMessageEvents();
            AddBusinessStatusEvents();
            AddXmlEvents();
            AddNotNullEvents();
            AddCorIdParentEvents();
            AddCustCodeEvents();
            AddFromAtEvents();
            AddCountryEvents();
            AddDurationEvents();
            AddMQDurationEvents();
            AddTopSelectionEvents();
            AddDataBaseHisto();
        }



        function AddTransacEvents() {
            $("[id$='chkTpTransID']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='TPTransIdValue']").focus();
                } else {
                    $("[id$='TPTransIdValue']").val("");
                    BindQuery();
                }
            });

            $("[id$='OperatorTpTransID']").change(function () {
                BindQuery();
            });

            $("[id$='TPTransIdValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkTpTransID']").prop("checked", true);
                } else {
                    $("[id$='chkTpTransID']").prop("checked", false);
                }
                BindQuery();
            });
        }

        function AddHubSpanEvents() {
            $("[id$='chkHubSpanID']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='HubSpanIdValue']").focus();
                } else {
                    $("[id$='HubSpanIdValue']").val("");
                    BindQuery();
                }
            });

            $("[id$='OperatorHubSpanId']").change(function () {
                BindQuery();
            });

            $("[id$='HubSpanIdValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkHubSpanID']").prop("checked", true);
                } else {
                    $("[id$='chkHubSpanID']").prop("checked", false);
                }
                BindQuery();
            });
        }

        //function AddPendingEvents() {
        //    $("[id$='chkPending']").change(function () {
        //        BindQuery();
        //    });
        //}

        function AddShippingNotesEvents() {
            $("[id$='chkShippingNotes']").change(function () {
                if ($(this).is(':checked')) {
                    DisableShippingNoteFields();
                } else {
                    EnableShippingNoteFields();
                }
                BindQuery();
            });
        }

        function AddFromToDateEvents() {
            $("[id$='chkDate']").change(function () {
                if (!$(this).is(':checked')) {
                    $find("<%= RadDateTimePickerFrom.ClientID %>").clear();
                    $find("<%= RadDateTimePickerTo.ClientID %>").clear();
                    BindQuery();
                }
            });
            $("[id$='RadDateTimePickerFrom']").change(function () {
                BindQuery();
            });

            $("[id$='RadDateTimePickerTo']").change(function () {
                BindQuery();
            });
        }

        function AddBusinessMessageEvents() {
            $("[id$='chkBusiMess']").change(function () {
                BindQuery();
            });

            $("[id$='ddBusiMess']").change(function () {
                $("[id$='chkBusiMess']").prop("checked", true);
                BindQuery();
            });
        }

        function AddBusinessStatusEvents() {
            $("[id$='chkBusiStatus']").change(function () {
                BindQuery();
            });

            $("[id$='ddBusiStatus']").change(function () {
                $("[id$='chkBusiStatus']").prop("checked", true);
                BindQuery();
            });
        }

        function AddXmlEvents() {
            $("[id$='chkXMLChoice']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='tbXMLChoice']").focus();
                } else {
                    $("[id$='tbXMLChoice']").val("");
                    BindQuery();
                }
            });

            $("[id$='ddXmlChoice']").change(function () {
                $("[id$='chkXMLChoice']").prop("checked", true);
                BindQuery();
            });

            $("[id$='tbXMLChoice']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkXMLChoice']").prop("checked", true);
                } else {
                    $("[id$='chkXMLChoice']").prop("checked", false);
                }
                BindQuery();
            });
        }

        function AddNotNullEvents() {
            $("[id$='chkNotNull']").change(function () {
                BindQuery();
            });
        }

        function AddCorIdParentEvents() {
            $("[id$='chkCorIdParent']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='txtCorIdParentValue']").focus();
                } else {
                    $("[id$='txtCorIdParentValue']").val("");
                    BindQuery();
                }
            });

            $("[id$='txtCorIdParentValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkCorIdParent']").prop("checked", true);
                } else {
                    $("[id$='chkCorIdParent']").prop("checked", false);
                }
                BindQuery();
            });
        }

        function AddCustCodeEvents() {
            $("[id$='chkCustCode']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='txtCustCodeValue']").focus();
                } else {
                    $("[id$='txtCustCodeValue']").val("");
                    BindQuery();
                }
            });

            $("[id$='txtCustCodeValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkCustCode']").prop("checked", true);
                } else {
                    $("[id$='chkCustCode']").prop("checked", false);
                }
                BindQuery();
            });

        }

        function AddFromAtEvents() {
            $("[id$='chkFromAt']").change(function () {
                if ($(this).is(':checked')) {
                    $("[id$='FromValue']").focus();
                } else {
                    $("[id$='FromValue']").val("");
                    $("[id$='AtValue']").val("");
                    BindQuery();
                }
            });

            $("[id$='FromValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkFromAt']").prop("checked", true);
                } else {
                    $("[id$='chkFromAt']").prop("checked", false);
                }
                BindQuery();
            });
            $("[id$='AtValue']").keyup(function (event) {
                if ($(this).val() != "") {
                    $("[id$='chkFromAt']").prop("checked", true);
                } else {
                    $("[id$='chkFromAt']").prop("checked", false);
                }
                BindQuery();
            });

        }

        function AddCountryEvents() {
            $("[id$='chkCountry']").change(function () {
                BindQuery();
            });
        }

        //This event is for the dropdown country. We can't add .change() event due of Telerik RadComboBox use.
        //We call this method on onclientselectedindexchanged of RadComBox. We need AddCountryEvents for the checkbox coutnry event (see above).
        function AddDdlCountryEvent() {
            $("[id$='chkCountry']").prop("checked", true);
            BindQuery();
        }

        function AddDurationEvents() {
            $("[id$='chkDuration']").change(function () {
                BindQuery();
            });

            $("[id$='txtDuration']").change(function () {
                if ($(this).val() != "") {
                    $("[id$='chkDuration']").prop("checked", true);
                } else {
                    $("[id$='chkDuration']").prop("checked", false);
                }
                BindQuery();
            });

        }

        function AddMQDurationEvents() {
            $("[id$='chkMQDuration']").change(function () {
                BindQuery();
            });

            $("[id$='txtMQDuration']").change(function () {
                if ($(this).val() != "") {
                    $("[id$='chkMQDuration']").prop("checked", true);
                } else {
                    $("[id$='chkMQDuration']").prop("checked", false);
                }
                BindQuery();
            });
        }

        function AddTopSelectionEvents() {
            $("[id$='txtTopSelection']").change(function () {
                BindQuery();
            });
        }

        function AddDataBaseHisto() {
            $("[id$='chkDatabaseHisto']").change(function () {
                BindQuery();
            });
        }

        function BindQuery() {
            var shippingNote = $("[id$='chkShippingNotes']").is(':checked');
            var defaultQuery = !shippingNote;
            var HistoQuery = $("[id$='chkDatabaseHisto']").is(':checked');
            $("[id$='GeneratedQuery']").val(GetQuery(defaultQuery, shippingNote, HistoQuery));
        }

        function GetQuery(defaultQuery, shippingNote, HistoQuery) {
            var dataTable = "V_TransactionTP";
            var logDataTable = "TPTransactionLog";
            if (HistoQuery) {
                dataTable = "TPTransactionHisto";
                logDataTable = "TPTransactionLogHisto";
            }
            else {
                if ($("[id$='chkBusiMess']").is(':checked')) {
                    var ddBusiMess = $("[id$='ddBusiMess']").val();
                    dataTable = ((ddBusiMess == 'ORDERPLACEMENT' || ddBusiMess == 'PRICEREQUEST') ? "TP2." : "") + "TPTransaction"
                }
            }

            
            var topSelection = $("[id$='txtTopSelection']").val();
            var query = "";
            if (defaultQuery) {
                query = "SELECT top " + topSelection + " " + dataTable + ".id, " + dataTable + ".hubspanId,DateCreat DateBegin,DateReceive DateEnd, datediff(second, DateCreat, DateReceive) As TreatTime, dateCreat, Status, dateReceive, datediff(second, dateCreat, dateReceive) As MQReceiveTime, Busimessage, Busistatus, Corid, CorIdParent, country, CustCode, CustName " +
                    "FROM " + dataTable + " WITH(NOLOCK) " +
                    GetWhereClause(dataTable) +
                    " ORDER By " + dataTable + ".id desc";
            } else if (shippingNote) {
                query = "SELECT top " + topSelection + " TPShipNotes.TPShipNotesID as ID, '' as hubspanId, '' as DateBegin,'' as DateEnd, '' As TreatTime, dateCreate as DateCreat, Status, dateReceive, datediff(second, dateCreate, dateReceive) As MQReceiveTime, BusinessmessageType as Busimessage, statussend as Busistatus, '' as Corid, '' as CorIdParent, country, CustomerCode as CustCode ,'' as CustName " +
                    "FROM TPShipNotes WITH(NOLOCK)" +
                    GetWhereClause(dataTable) +
                    " ORDER By TPShipNotesid desc";
            }

            return query;
        }

        function GetWhereClause(dataTable) {
            var wherelabel = " WHERE ";
            var clause = "";
            var ddlCountry = $("[id$='ddlCountry']");
            var ddlCountryValue = ddlCountry[0].control._value;
            var ddlCountryText = ddlCountry[0].control._text;           
            

            if ($("[id$='chkCountry']").is(':checked')) {
                if (ddlCountryText.toLowerCase() != "all") {
                    clause = "Country = '" + ddlCountryValue + "'";
                }
                else {
                    if ((ddlCountryText.toLowerCase() == "all") && (ddlCountryValue.toLowerCase() != "all")) {
                        clause = " Country IN ('" + ddlCountryValue.split(",").join("','") + "') "
                    }
                }
            }
            else {
                if ($("[id$='countryFilter']").val() != "") {
                    clause = $("[id$='countryFilter']").val();
                }
            }

            if ($("[id$='chkTpTransID']").is(':checked')) {
                var operator = $("[id$='OperatorTpTransID']").val();
                var transacID = $("[id$='TPTransIdValue']").val();
                if (clause != "") {
                    clause += " And ";
                }
                if (transacID != "") {
                    clause += dataTable + ".Id " + operator + " " + transacID;
                }
            }

            if ($("[id$='chkHubSpanID']").is(':checked')) {
                var operator = $("[id$='OperatorHubSpanId']").val();
                var hubspanid = $("[id$='HubSpanIdValue']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += dataTable + ".hubspanId " + operator + " " + hubspanid;
            }

            //if ($("[id$='chkPending']").is(':checked')) {
            //    if (clause != "") {
            //        clause += " And ";
            //    }
            //    clause += "Pending = 1";
            //}

            if ($("[id$='chkBusiMess']").is(':checked')) {
                var ddBusiMess = $("[id$='ddBusiMess']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += "BUSIMESSAGE = '" + ddBusiMess + "'";
            }

            if ($("[id$='chkBusiStatus']").is(':checked')) {
                var ddBusiStatus = $("[id$='ddBusiStatus']").val();
                if (clause != "") {
                    clause += " And ";
                }
                if (ddBusiStatus == "null") {
                    clause += "BUSISTATUS is null";
                } else {
                    clause += "BUSISTATUS = '" + ddBusiStatus + "'";
                }
            }

            if ($("[id$='chkXMLChoice']").is(':checked')) {
                var ddXmlChoice = $("[id$='ddXmlChoice']").val();
                var tbXMLChoice = $("[id$='tbXMLChoice']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += dataTable + "." + ddXmlChoice + " like '%" + tbXMLChoice + "%'";
            }

            if ($("[id$='chkNotNull']").is(':checked')) {
                var ddXmlChoice = $("[id$='ddXmlChoice']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += dataTable + "." + ddXmlChoice + " is not null";
            }

            if ($("[id$='chkCorIdParent']").is(':checked')) {
                var txtCorIdParentValue = $("[id$='txtCorIdParentValue']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += "CorIdParent = '" + txtCorIdParentValue + "'";

            }

            if ($("[id$='chkCustCode']").is(':checked')) {
                var txtCustCodeValue = $("[id$='txtCustCodeValue']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += "CustCode = '" + txtCustCodeValue + "'";

            }

            if ($("[id$='chkFromAt']").is(':checked')) {
                var FromValue = $("[id$='FromValue']").val();
                var AtValue = $("[id$='AtValue']").val();
                if (clause != "") {
                    clause += " And ";
                }
                var FromClause = "";
                var AtClause = "";

                if (FromValue != "") {
                    FromClause = "CustName = '" + FromValue + "'";
                }

                if (AtValue != "") {
                    if (FromClause != "") {
                        AtClause = " And ";
                    }
                    AtClause += "ApplicationName='" + AtValue + "'";
                }
                clause += FromClause + AtClause;

            }            

            if ($("[id$='chkDate']").is(':checked')) {
                var RadDateTimePickerFrom = $("[id$='RadDateTimePickerFrom']").val();
                var RadDateTimePickerTo = $("[id$='RadDateTimePickerTo']").val();


                if (RadDateTimePickerFrom != "") {
                    if (clause != "") {
                        clause += " And ";
                    }
                    clause += "dateCreat >= '" + RadDateTimePickerFrom + "'";
                }

                if (RadDateTimePickerTo != "") {
                    if (clause != "") {
                        clause += " And ";
                    }
                    clause += "dateCreat <= '" + RadDateTimePickerTo + "'";
                }


            }

            if ($("[id$='chkDuration']").is(':checked')) {
                var txtDuration = $("[id$='txtDuration']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += "datediff(second, DateBegin, DateEnd) >= " + txtDuration;

            }

            if ($("[id$='chkMQDuration']").is(':checked')) {
                var txtMQDuration = $("[id$='txtMQDuration']").val();
                if (clause != "") {
                    clause += " And ";
                }
                clause += "datediff(second, dateCreat, dateReceive) >= " + txtMQDuration;

            }

            var result = "";
            if (clause != "") {
                result = wherelabel + clause;
            }

            return result;
        }

        function DisableShippingNoteFields() {

            $("[id$='chkTpTransID']").attr("disabled", true);
            $("[id$='chkTpTransID']").prop("checked", false);
            $("[id$='OperatorTpTransID']").attr("disabled", true);
            $("[id$='TPTransIdValue']").attr("disabled", true);
            $("[id$='TPTransIdValue']").val("");

            $("[id$='chkHubSpanID']").attr("disabled", true);
            $("[id$='chkHubSpanID']").prop("checked", false);
            $("[id$='OperatorHubSpanId']").attr("disabled", true);
            $("[id$='HubSpanIdValue']").attr("disabled", true);
            $("[id$='HubSpanIdValue']").val("");

            $("[id$='chkBusiMess']").attr("disabled", true);
            $("[id$='chkBusiMess']").prop("checked", false);
            $("[id$='ddBusiMess']").attr("disabled", true);

            $("[id$='chkPending']").attr("disabled", true);

            $("[id$='chkXMLChoice']").attr("disabled", true);
            $("[id$='chkXMLChoice']").prop("checked", false);
            $("[id$='ddXmlChoice']").attr("disabled", true);
            $("[id$='tbXMLChoice']").attr("disabled", true);
            $("[id$='chkNotNull']").attr("disabled", true);
            $("[id$='chkNotNull']").prop("checked", false);

            $("[id$='chkCorIdParent']").attr("disabled", true);
            $("[id$='chkCorIdParent']").prop("checked", false);
            $("[id$='txtCorIdParentValue']").attr("disabled", true);

            $("[id$='chkFromAt']").attr("disabled", true);
            $("[id$='chkFromAt']").prop("checked", false);
            $("[id$='FromValue']").attr("disabled", true);
            $("[id$='AtValue']").attr("disabled", true);

            $("[id$='chkDuration']").attr("disabled", true);
            $("[id$='chkDuration']").prop("checked", false);
            $find('<%= txtDuration.ClientID %>').disable();

            $("[id$='chkDatabaseHisto']").attr("disabled", true);
            $("[id$='chkDatabaseHisto']").prop("checked", false);
        }

        function EnableShippingNoteFields() {

            $("[id$='chkTpTransID']").removeAttr("disabled");
            $("[id$='OperatorTpTransID']").removeAttr("disabled");
            $("[id$='TPTransIdValue']").removeAttr("disabled");

            $("[id$='chkHubSpanID']").removeAttr("disabled");
            $("[id$='OperatorHubSpanId']").removeAttr("disabled");
            $("[id$='HubSpanIdValue']").removeAttr("disabled");

            $("[id$='chkBusiMess']").removeAttr("disabled");
            $("[id$='ddBusiMess']").removeAttr("disabled");

            $("[id$='chkPending']").removeAttr("disabled");

            $("[id$='chkXMLChoice']").removeAttr("disabled");
            $("[id$='ddXmlChoice']").removeAttr("disabled");
            $("[id$='tbXMLChoice']").removeAttr("disabled");
            $("[id$='chkNotNull']").removeAttr("disabled");

            $("[id$='chkCorIdParent']").removeAttr("disabled");
            $("[id$='txtCorIdParentValue']").removeAttr("disabled");

            $("[id$='chkFromAt']").removeAttr("disabled");
            $("[id$='FromValue']").removeAttr("disabled");
            $("[id$='AtValue']").removeAttr("disabled");

            $("[id$='chkDuration']").removeAttr("disabled");
            $find('<%= txtDuration.ClientID %>').enable();

            $("[id$='chkDatabaseHisto']").removeAttr("disabled");

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Panel runat="server" ID="SearchCriteriaPanel" Visible="true">
        <table class="Filters" style="width: 100%; display: block;">
            <tr>
                <td class="">
                    <asp:CheckBox ID="chkTpTransID" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblTpTransID" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkTpTransID">TPTrans Id</asp:Label>
                    <asp:DropDownList ID="OperatorTpTransID" runat="server" CssClass="ddl_Text Electrolux_Color">
                        <asp:ListItem Text="=" Value="="></asp:ListItem>
                        <asp:ListItem Text=">" Value=">"></asp:ListItem>
                        <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                        <asp:ListItem Text="<" Value="<"></asp:ListItem>
                        <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="TPTransIdValue" runat="server" CssClass="width160px floatRight Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                </td>
                <td class="">
                    <asp:CheckBox ID="chkBusiMess" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblBusiMess" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkBusiMess">BUSIMESSAGE</asp:Label>
                    <asp:DropDownList ID="ddBusiMess" runat="server" CssClass="ddl_Text Electrolux_Color floatRight width190px">
                    </asp:DropDownList>
                </td>
                <td class="">
                    <asp:CheckBox ID="chkXMLChoice" runat="server"></asp:CheckBox>
                    <asp:DropDownList ID="ddXmlChoice" runat="server" CssClass="ddl_Text Electrolux_Color" Style="width: 92%;">
                        <asp:ListItem Text="XMLTP" Value="XMLTP"></asp:ListItem>
                        <asp:ListItem Text="XMLB2B" Value="XMLB2B"></asp:ListItem>
                        <asp:ListItem Text="XMLSOP" Value="XMLSOP"></asp:ListItem>
                        <asp:ListItem Text="TP2XMLOUT" Value="TP2XMLOUT"></asp:ListItem>
                        <asp:ListItem Text="TP2XMLOUTMAIL" Value="TP2XMLOUTMAIL"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox ID="tbXMLChoice" runat="server" Style="width: 100%;" CssClass="Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblXMLChoice" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkNotNull">Not null</asp:Label>
                    <asp:CheckBox ID="chkNotNull" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td class="">
                    <asp:CheckBox ID="chkHubSpanID" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblHubspanID" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkHubSpanID">HubSpan id</asp:Label>
                    <asp:DropDownList ID="OperatorHubSpanId" runat="server" CssClass="ddl_Text Electrolux_Color">
                        <asp:ListItem Text="=" Value="="></asp:ListItem>
                        <asp:ListItem Text=">" Value=">"></asp:ListItem>
                        <asp:ListItem Text=">=" Value=">="></asp:ListItem>
                        <asp:ListItem Text="<" Value="<"></asp:ListItem>
                        <asp:ListItem Text="<=" Value="<="></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="HubSpanIdValue" runat="server" CssClass="width160px floatRight Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                </td>
                <td class="">
                    <asp:CheckBox ID="chkBusiStatus" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblBusiStatus" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkBusiStatus">BUSISTATUS</asp:Label>
                    <asp:DropDownList ID="ddBusiStatus" runat="server" CssClass="ddl_Text Electrolux_Color floatRight width190px">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:CheckBox ID="chkCorIdParent" runat="server"></asp:CheckBox>
                    <asp:Label ID="lblCorIdParent" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkCorIdParent">CorIdParent</asp:Label>
                    <asp:TextBox runat="server" ID="txtCorIdParentValue" CssClass="width160px floatRight Electrolux_Color Electrolux_light_bold" />
                </td>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td>
                    <%--<asp:CheckBox ID="chkPending" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblPending" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkPending">PENDING</asp:Label>--%>
                    <asp:CheckBox ID="chkShippingNotes" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblShippingNotes" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkShippingNotes">ShippingNotes</asp:Label>

                </td>
                <td>
                    <span class="Electrolux_light_bold Electrolux_Color" style="width: 100px; display: inline-block;">Environment:</span>
                    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color floatRight width190px" ID="ddlEnvironment"></asp:DropDownList>

                </td>
                <td>
                    <asp:CheckBox ID="chkCustCode" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblCustCode" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkCustCode">Cust. Code</asp:Label>
                    <asp:TextBox ID="txtCustCodeValue" runat="server" CssClass="width160px floatRight Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                </td>
                <td>
                    <asp:CheckBox ID="chkFromAt" runat="server"></asp:CheckBox>
                    <asp:Label ID="lblFrom" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkFromAt">From</asp:Label>
                    <asp:TextBox ID="FromValue" runat="server" CssClass="Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                    <asp:Label ID="lblAt" runat="server" CssClass="Electrolux_Color Electrolux_light_bold">@</asp:Label>
                    <asp:TextBox ID="AtValue" runat="server" CssClass="Electrolux_Color Electrolux_light_bold"></asp:TextBox>
                </td>
                <td>
                    <asp:CheckBox ID="chkCountry" runat="server"></asp:CheckBox>
                    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlCountry" OnClientSelectedIndexChanged="AddDdlCountryEvent">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:CheckBox ID="chkDate" runat="server"></asp:CheckBox>
                    <asp:Label runat="server" ID="lblDate" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkDate">From/To Date</asp:Label>
                    <telerik:RadDateTimePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm:ss" runat="server" Style="display: inline-block;">
                        <Calendar runat="server">
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today">
                                    <ItemStyle CssClass="rcToday" />
                                </telerik:RadCalendarDay>
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDateTimePicker>
                    -
                           <telerik:RadDateTimePicker ID="RadDateTimePickerTo" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm:ss" runat="server" Style="display: inline-block;">
                               <Calendar runat="server">
                                   <SpecialDays>
                                       <telerik:RadCalendarDay Repeatable="Today">
                                           <ItemStyle CssClass="rcToday" />
                                       </telerik:RadCalendarDay>
                                   </SpecialDays>
                               </Calendar>
                           </telerik:RadDateTimePicker>
                    <asp:Label ID="lblSelection" runat="server" CssClass="Electrolux_Color Electrolux_light_bold">Top Selection</asp:Label>
                    <telerik:RadNumericTextBox AutoPostBack="false" runat="server" MinValue="25" Value="25" ShowSpinButtons="true" DataType="Integer" MaxValue="10000" ID="txtTopSelection" CssClass="Electrolux_light_bold Electrolux_Color" IncrementSettings-Step="25" Style="display: inline-block;">
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>

                </td>
                <td>
                    <asp:CheckBox ID="chkDatabaseHisto" runat="server"></asp:CheckBox>
                    <asp:Label ID="lblDatabaseHisto" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkDatabaseHisto">DataBase Histo</asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkDuration" runat="server"></asp:CheckBox>
                    <asp:Label ID="lblDuration" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkDuration">duration >= </asp:Label>
                    <telerik:RadNumericTextBox runat="server" MinValue="25" Value="25" ShowSpinButtons="true" DataType="Integer" MaxValue="1000" ID="txtDuration" CssClass="Electrolux_light_bold Electrolux_Color" IncrementSettings-Step="25" Style="display: inline-block;">
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:CheckBox ID="chkMQDuration" runat="server"></asp:CheckBox>
                    <asp:Label ID="lblMQDuration" runat="server" CssClass="Electrolux_Color Electrolux_light_bold" AssociatedControlID="chkMQDuration">MQ duration >= </asp:Label>
                    <telerik:RadNumericTextBox runat="server" MinValue="25" Value="25" ShowSpinButtons="true" DataType="Integer" MaxValue="1000" ID="txtMQDuration" CssClass="Electrolux_light_bold Electrolux_Color" IncrementSettings-Step="25" Style="display: inline-block;">
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                </td>
            </tr>
        </table>
        <textarea id="GeneratedQuery" name="GeneratedQuery" style="width: 100%; display: block;" rows="5" runat="server" readonly="readonly"></textarea>
    </asp:Panel>
    <asp:HiddenField ID="countryFilter" runat="server" />

    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="LogViewerUpdatePanel">
        <ContentTemplate>
            <asp:Button runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="javascript:BeginSearch();" OnClick="BtnSearch_Click" Text="Search" UseSubmitBehavior="false" />
            <asp:Panel runat="server" ID="ResultPanel" Visible="false" Style="padding: 10px;">
                <%--<label id="CountLabel">Record Count :
                    <label id="RowCount" runat="server"></label>
                </label>--%>
               <%-- <telerik:RadGrid runat="server" ID="dgResult" CssClass="LogGridSearch" AllowPaging="False" AllowSorting="false" AllowFilteringByColumn="true" OnItemDataBound="dgResult_ItemDataBound" PageSize="20" GroupingEnabled="true">
                    <MasterTableView AutoGenerateColumns="false" TableLayout="Auto">
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="ViewXML" AllowFiltering="false" HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <img src='Images/XML.png' width="20" height="20" title="View XML File" class="MoreInfoImg"
                                        onclick="OpenViewXMLFilesWindow(<%# "'10','" + Eval("BUSIMESSAGE").ToString() + "','48','" + Eval("Corid").ToString() + "','','','','" + Eval("ID").ToString() + "'" %>)" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="ID" AllowFiltering="false" UniqueName="ID" HeaderText="ID">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "ID")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Country" AllowFiltering="false" UniqueName="Country" HeaderText="Country">
                                <ItemTemplate>
                                    <img src='Images/Flags/<%# DataBinder.Eval(Container.DataItem, "Country").ToString().Trim()%>.png' width="20" height="16" title="<%# DataBinder.Eval(Container.DataItem, "Country")%>" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="HubspanId" AllowFiltering="false" UniqueName="HubspanId" HeaderText="HubspanId">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "HubspanId")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="HubspanReceive" AllowFiltering="false" UniqueName="HubspanReceive" HeaderText="HubspanReceive">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "DateBegin")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="SendToHubSpan" AllowFiltering="false" UniqueName="SendToHubSpan" HeaderText="SendToHubSpan">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "DateEnd")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn DataField="Time" AllowFiltering="false" UniqueName="Time" HeaderText="Time">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "TreatTime")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>

                            <telerik:GridTemplateColumn DataField="CustCode" AllowFiltering="false" UniqueName="CustCode" HeaderText="CustCode">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "CustCode")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="CustName" AllowFiltering="false" UniqueName="CustName" HeaderText="CustName">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "CustName")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="BUSIMESSAGE" AllowFiltering="false" UniqueName="BUSIMESSAGE" HeaderText="BUSIMESSAGE">
                                <ItemTemplate>
                                    <label id="BusimessageLabel" runat="server"><%# DataBinder.Eval(Container.DataItem, "BUSIMESSAGE")%></label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="BUSISTATUS " AllowFiltering="false" UniqueName="BUSISTATUS " HeaderText="Res.">
                                <ItemTemplate>
                                    <asp:HiddenField ID="BusiStatus" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "BUSISTATUS")%>' />
                                    <img src='Images/<%# DataBinder.Eval(Container.DataItem, "BUSISTATUS").ToString().Trim()%>.gif' width="16" height="16" title="<%# DataBinder.Eval(Container.DataItem, "BUSISTATUS")%>" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="SQLDateCreat" AllowFiltering="false" UniqueName="SQLDateCreat" HeaderText="SQLDateCreat">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "dateCreat")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="MQReceive" AllowFiltering="false" UniqueName="MQReceive" HeaderText="MQReceive">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "dateReceive")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="MQReceiveTime" AllowFiltering="false" UniqueName="MQReceiveTime" HeaderText="MQReceiveTime">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "MQReceiveTime")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Pending" AllowFiltering="false" UniqueName="Pending" HeaderText="Pending">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "Pending")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Corid" AllowFiltering="false" UniqueName="Corid" HeaderText="Corid">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "Corid")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="CorIdParent" AllowFiltering="false" UniqueName="CorIdParent" HeaderText="CorIdParent">
                                <ItemTemplate>
                                    <%# DataBinder.Eval(Container.DataItem, "CorIdParent")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>--%>
                <asp:Table id="PocJQueryDataTable" runat="server" CssClass="display" style="border-collapse:collapse !important;font-size:12px;">
                    <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                        <asp:TableHeaderCell ID="TableHeaderCell0" runat="server"></asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">ID</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">HubspanId</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">HubspanReceive</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">SendToHubSpan</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">Time</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">SQLDateCreat</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">MQ Time</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell8" runat="server">Seconds</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">BUSIMESSAGE</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell11" runat="server">Res.</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell12" runat="server">Corid</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell13" runat="server">CorIdParent</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell14" runat="server">Country</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell15" runat="server">CustCode</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell16" runat="server">CustName</asp:TableHeaderCell>     
                    </asp:TableHeaderRow>
                </asp:Table>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
        </Triggers>
    </asp:UpdatePanel>   
</asp:Content>

