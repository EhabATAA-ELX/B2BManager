<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="TP2SpecificKeysManagement.aspx.vb" Inherits="TP2SpecificKeysManagement" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link href="Scripts/DataTables/datatables.min.css" rel="stylesheet" />    
    <link href="CSS/jquery-ui.css" rel="stylesheet" />
    <link href="Scripts/DataTables/datatables.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/DataTables/datatables.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Buttons-1.5.6/js/dataTables.buttons.min.js"></script>
    <script type="text/javascript" src="Scripts/DataTables/Select-1.3.0/js/dataTables.select.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <style type="text/css">
        .dataTables_filter label {
            display: block;
            float: left;
        }
        .dataTables_filter label input{
            font-weight:normal;
            width:230px;
            margin-left:37px !important;
        }

        th{
            text-align:left;
        }
    </style>
    <script> 


         $(document).ready(function () {
             BindDataTable(); 
             
        });

        var datatable;
        function BindDataTable() {
            //if (datatable != undefined) {                
            //    datatable.destroy();
            //}
            HideLoadingPanel();
            $("[id$='JQueryDataTable']").removeClass("DisplayNone");
            datatable = $("[id$='JQueryDataTable']").DataTable({
                "stateSave": true,
                "pageLength": 15,
                "lengthChange": false,
                  "columns": [
                        { "width": "1px", orderable: false },
                        { "width": "250px" },
                        { "width": "400px" },
                        { "width": "350px" },
                        { "width": "350px" },
                        { "width": "75px" }
                ],
                "order": [[1, "asc"]],
                "dom": '<"floatLeft Width316px DataTableCustom"f><t><"DatatableBottom"<i><pl>><"clear">'                
                });
        }

     <%= GetActionRights() %> 
        //GetActionRights() add variable 
        //EDIT_SMS_LOCAL_VALUE
        //EDIT_SMS_DEFAULT_AND_COMMENT_VALUES
        //CHANGE_SMS_TYPE
        //DELETE_SMS_KEY
        //ADD_SMS_KEY
        //DISPLAY_ALL_SMS_TYPES

    function EditRow(idRow){
        var tr = $("[id='" + idRow + "']");
        var DefaultType;        
       

        tr.find('td').each(function (index) {
            var content = this.innerHTML;
            var cssClass = "";
            //if (userRestricted) {
            //    cssClass = "InputDisabled";
            //}
            switch (index) {
                case 0:
                    this.innerHTML = getEditButtons(idRow,index);
                    break;
                case 1:
                    cssClass = "InputDisabled";                   
                    if (content.length < 10) {
                        cssClass += " width80px";
                    } else {
                         cssClass += " width100percent";
                    }                          
                    this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass,false);
                    break; 
                case 2:
                    //EDIT_SMS_DEFAULT_AND_COMMENT_VALUES
                    if (!EDIT_SMS_DEFAULT_AND_COMMENT_VALUES) {
                        cssClass = "InputDisabled";
                    }                    
                    if (content.length < 10) {
                        cssClass += " width80px";
                    } else {
                        cssClass += " width100percent";
                    }                           
                    this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass,EDIT_SMS_DEFAULT_AND_COMMENT_VALUES);
                    break;                
                case 3:
                    //EDIT_SMS_DEFAULT_AND_COMMENT_VALUES
                    if (!EDIT_SMS_DEFAULT_AND_COMMENT_VALUES) {
                        cssClass = "InputDisabled";
                    }  
                    if (content.toLowerCase() == "true" || content.toLowerCase() == "false") {
                        this.innerHTML = getInput(idRow, index, "BooleanDropDownlist", content,cssClass,EDIT_SMS_DEFAULT_AND_COMMENT_VALUES);
                        DefaultType = "BooleanDropDownlist";
                    } else if (content.toLowerCase() == "f" || content.toLowerCase() == "t") {
                        this.innerHTML = getInput(idRow, index, "TypeDropDownlist", content,cssClass,EDIT_SMS_DEFAULT_AND_COMMENT_VALUES);
                        DefaultType = "TypeDropDownlist";
                    } else {
                        this.innerHTML = getInput(idRow, index, "TextBox", content, cssClass,EDIT_SMS_DEFAULT_AND_COMMENT_VALUES);
                        DefaultType = "TextBox";
                    }
                    break;
                case 4:         
                    //EDIT_SMS_LOCAL_VALUE
                    if (!EDIT_SMS_LOCAL_VALUE) {
                        cssClass = "InputDisabled";
                    } 
                    this.innerHTML = getInput(idRow, index, DefaultType, content, cssClass,EDIT_SMS_LOCAL_VALUE);                  
                    break;
                case 5:     
                    //CHANGE_SMS_TYPE
                    if (!CHANGE_SMS_TYPE) {
                        cssClass = "InputDisabled";
                    } 
                    this.innerHTML = getInput(idRow, index, "TypeDropDownlist", content,cssClass,CHANGE_SMS_TYPE);
                    break;
            }

        });
        DefaultType = "";
        }

        function ViewRow(idRow,Cancel){
        var tr = $("[id='" + idRow + "']");
            tr.find('td').each(function (index) {
                if (index >= 1) {     
                    if (Cancel) {                        
                        this.innerHTML = $(this).attr("data");
                    } else {                        
                        this.innerHTML = $("#" + idRow + "_" + index).val();
                    }
                //this.innerHTML = $(this).attr("data");
            } else if(index == 0) {
                this.innerHTML = getUpdateButton(idRow,Cancel);
            }

        });
        }

        function ViewRowAfterDelete(KeyName){
        var tr = $("[id='Line_" + KeyName + "']");
            tr.find('td').each(function (index) {
                if (index >= 1) {                        
                        this.innerHTML = $(this).html();
                } else if(index == 0) {
                    this.innerHTML = getUpdateButton("Line_" + KeyName,true);
                }
            });

        }

        function ViewAllCountry(idRow){
            var url = 'SpecificKeysManagementAllCountries.aspx?v=1';
            if (idRow && idRow != null) {
                url += "&KeyName=" + idRow.toString();
            }

            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            url += "&ddlCountry="+CountryBox.get_selectedItem().get_value();
            url += "&ddlEnvironment="+$("[id$='ddlEnvironment']").val();
            if ($(window).height() > 570 && $(window).width() > 700) {
                var oWnd = $find("<%= AllCountryGrid.ClientID %>");
                oWnd.set_title('Loading...'); 
                oWnd.setUrl(url + "&HideHeader=true");
                oWnd.show();
            }
            else {
                popup(url);
            }
        }

        function getInput(id, index, type, text, cssClass, editable) {
            var readOnlyValue = "";
            var disableSelect = "";
            if (!editable) {
                readOnlyValue = "readonly";
                disableSelect = " disabled=\"true\" ";
            }
            if (type == "TextBox") {
                return "<input id=\"" + id + "_" + index + "\" class=\"" + cssClass + "\" type=\"text\" "+readOnlyValue+" value=\"" + text + "\">";
            }
            else if (type == "BooleanDropDownlist") {
                var option = "";
                if (text.toLowerCase() == "true") {
                    option = "<option selected=\"selected\" value=\"True\">True</option>" +
                        "<option value=\"False\">False</option>";
                } else {
                    option = "<option value=\"True\">True</option>" +
                        "<option selected=\"selected\" value=\"False\">False</option>";
                }

                return "<select id=\"" + id + "_" + index + "\" name=\"\"  "+readOnlyValue+""+disableSelect+" class=\"width80px "+cssClass+"\">" +
                    option
                "</select > ";
            } else if (type == "TypeDropDownlist") {
                  var option = "";
                if (text.toLowerCase() == "f") {
                    option = "<option selected=\"selected\" value=\"F\">F</option>" +
                        "<option value=\"T\">T</option>";
                } else {
                    option = "<option value=\"F\">F</option>" +
                        "<option selected=\"selected\" value=\"T\">T</option>";
                }

                return "<select id=\"" + id + "_" + index + "\" name=\"\"  "+readOnlyValue+""+disableSelect+" class=\"width80px "+cssClass+"\">" +
                    option
                "</select > ";
            }
        }

        function getEditButtons(idRow) {

            var result = "";
            if (EDIT_SMS_DEFAULT_AND_COMMENT_VALUES || EDIT_SMS_LOCAL_VALUE) {
                result = "<input type=\"image\" class=\"width20px\" src=\"./Images/save.png\" title=\"Save update\" onclick=\"update('" + idRow + "');return false;\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
            } else {
                 result = "<input type=\"image\" class=\"width20px ImgDisabled\" src=\"./Images/save.png\" title=\"Save update\">" +
                    "<input type=\"image\" class=\"width20px\" src=\"./Images/cancel.png\" title=\"Cancel\" onclick=\"ViewRow('" + idRow + "',true);return false;\">";
            }

           return result;
        }

        function getUpdateButton(idRow,Cancel) {
            var result = "<input type=\"image\" class=\"width20px\" src=\"./Images/edit.png\" title=\"Edit\" onclick=\"EditRow('" + idRow + "');return false;\">";
            var keyname = idRow.split("_")[1];

            var HasCountryValue;
            if (Cancel) {                
                HasCountryValue = $("#" + keyname + "_CountryValue").attr("data") != "";
            } else {
                  HasCountryValue = $("#" + idRow + "_4").val() != "";
            }

            if (DISPLAY_ALL_SMS_TYPES) {
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/magnifyingglass.png\" title=\"View values in all countries\" onclick=\"ViewAllCountry('"+keyname+"');\">";
            }

            if (DELETE_SMS_KEY && HasCountryValue) {
                var CountryBox = $find("<%= ddlCountry.ClientID %>");
                var EnvId = $("[id$='ddlEnvironment']").val();
                result += "<input type=\"image\"   class=\"width20px\" src=\"./Images/delete.png\" title=\"Delete Country value\" onclick=\"Delete('" + keyname + "','" + CountryBox.get_selectedItem().get_value() + "','" + EnvId + "');\">";
            } else {
                if (DELETE_SMS_KEY) {
                    result += "<input type=\"image\"   class=\"width20px ImgDisabled\" src=\"./Images/delete.png\" title=\"Delete country value\"\">";
                }
            }            
          
           return result;
        }
        function update(idRow) {
            ShowLoadingPanel();
            var url = "SpecificKeysManagement.aspx/UpdateLine";           
            var UserData = getDataRow(idRow);
             $.ajax({
                method: "POST",
                url: url,
                data: JSON.stringify(UserData),
                contentType: "Application/json; charset=utf-8",
                dataType: "json",
                 success: function (data) {
                     HideLoadingPanel();
                     var keyname = idRow.split("_")[1];
                     $("#" + keyname+"_CountryValue").attr("data",$("#" + idRow + "_4").val());
                    ViewRow(idRow,false);
                },
                 failure: function (msg) {
                    HideLoadingPanel();
                    alert(msg);
                },
                 error: function (xhr, err) {
                    HideLoadingPanel();
                    alert(xhr.responseJSON.Message);
                }
            });                    
        }

        function Delete(KeyName, SopId, EnvironmentID) {

            $("#dialog-confirm").dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                buttons: [
                    {
                        text: "Cancel",
                        class: "btn red btn-margin",
                        click: function () {
                            $(this).dialog("close");
                        }
                    },
                    {
                        text: "Confirm",
                        class: "btn green btn-margin",
                        click: function () {
                            $(this).dialog("close");
                            var url = "SpecificKeysManagement.aspx/DeleteLine";
                            var data = {
                                EnvironmentID: EnvironmentID,
                                SopId: SopId,
                                KeyName: KeyName
                            };
                            $.ajax({
                                method: "POST",
                                url: url,
                                data: JSON.stringify(data),
                                contentType: "Application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data) {
                                    HideLoadingPanel();
                                    var idColumn = KeyName + "_CountryValue";
                                    $("#" + idColumn).html("");
                                    $("#" + idColumn).attr("data", "");
                                    ViewRowAfterDelete(KeyName);
                                },
                                failure: function (msg) {
                                    HideLoadingPanel();
                                    alert(msg);
                                },
                                error: function (xhr, err) {
                                    HideLoadingPanel();
                                    alert(xhr.responseJSON.Message);
                                }
                            });
                        }
                    }]
            });               
        }

        function getDataRow(idRow) {
            var CountryBox = $find("<%= ddlCountry.ClientID %>");
            var CountryValue = CountryBox.get_selectedItem().get_value();
            var data = {
                EnvironmentID: $("[id$='ddlEnvironment']").val(),
                SopId:CountryValue,
                KeyName: $("#" + idRow + "_1").val(),
                Comment: $("#" + idRow + "_2").val(),
                DefaultValue: $("#" + idRow + "_3").val(),
                CountryValue: $("#" + idRow + "_4").val(),
                Type:$("#" + idRow + "_5").val(),
            };
            return data;
        }

        function ShowLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").removeClass("hidden");
        }

        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

        function AddSmsKeyPopup() {         
              var oWnd = $find("<%= WindowActionProfile.ClientID %>");
            //oWnd.setUrl(url + "&HideHeader=true");
            oWnd.show();
            
        }

          function CloseWindow() {
              var oWnd = $find("<%= WindowActionProfile.ClientID %>");
              if (oWnd !== null) {                  
                oWnd.close();
              }
        }

        function ClearPopupAdd() {            
              $("[id$='KeyNameTxt']").val("");
              $("[id$='CommentTxt']").val("");
              $("[id$='DefautValueTxt']").val("");
              $("[id$='type']").val("F");          
        }

        function RefreshTable() {
            ShowLoadingPanel();
            var UpdatePanel = '<%=SmsKeyUpdatePanel.ClientID%>';
            if (UpdatePanel != null) 
               {
                   __doPostBack(UpdatePanel, '');
               }
            //__doPostBack('RefreshJqueryDatable', '');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlEnvironment" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true" OnChange="ShowLoadingPanel();">
    </asp:DropDownList>

    <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
    <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlCountry" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true" OnClientSelectedIndexChanged="ShowLoadingPanel" >
        <Items>
            <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
        </Items>
    </telerik:RadComboBox>

      <asp:Label runat="server" ID="lblTypeSearch" CssClass="Electrolux_light_bold Electrolux_Color">Type:</asp:Label>
    <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" ID="ddlType" OnSelectedIndexChanged="ddl_SelectedIndexChanged" AutoPostBack="true" OnChange="ShowLoadingPanel();">
    </asp:DropDownList>

    <input type="button" id="btnAddSmsKey" class="btn bleu" onclick="AddSmsKeyPopup()" value="New SMS Key" runat="server" />
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="SmsKeyUpdatePanel" ChildrenAsTriggers="false">
        <ContentTemplate>
             <div id="LoadingPanel" style="position: absolute; top: 80px; width: 100%; height: 100%; z-index: 1000;  vertical-align: middle;" class="">
                <asp:Image ID="LoadingImage" runat="server" AlternateText="Loading..." ImageUrl="Images/Loading.gif" style="position: absolute;top: 50%;left: 50%;margin-left: -32px;margin-top: -20px;z-index: 1001;" />
            </div>
            <asp:Table ID="JQueryDataTable" runat="server" CssClass="DisplayNone" Style="border-collapse: collapse !important; font-size: 12px;">
                <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                    <asp:TableHeaderCell CssClass="TextAlignCenter" ID="TableHeaderCell0" runat="server">Actions</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">SMS Key</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Comment</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Default Value (All countries)</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Country Value</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">Type</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
             <telerik:RadWindow ID="WindowActionProfile" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" VisibleOnPageLoad="false" VisibleStatusbar="false" Title="Add New SMS Key" ShowContentDuringLoad="false" Behaviors="Close" Width="600px" Height="300px" runat="server" OnClientBeforeClose="ClearPopupAdd">
                 <ContentTemplate>
                     <div class="padding20px">                     
                     <table style="width:100%">
                    <tbody>                      
                    <tr style="height: 24px">
                        <td>
                            <span id="lblKeyName" class="Electrolux_light_bold Electrolux_Color">Key name:</span>
                        </td>
                        <td>
                            <input name="KeyName" type="text" id="KeyNameTxt" class="Electrolux_Color" style="width:400px;" runat="server"/>
                        </td>
                    </tr>
                     <tr style="height: 24px">
                        <td>
                            <span id="lblComment" class="Electrolux_light_bold Electrolux_Color">Comment:</span>
                        </td>
                        <td>
                            <input name="KeyName" type="text" id="CommentTxt" class="Electrolux_Color" style="width:400px;" runat="server"/>
                        </td>
                    </tr> 
                    <tr style="height: 24px">
                        <td>
                            <span id="lblDefaultValue" class="Electrolux_light_bold Electrolux_Color">All Value:</span>
                        </td>
                        <td>
                            <input name="KeyName" type="text" id="DefautValueTxt" class="Electrolux_Color" style="width:400px;" runat="server" />
                        </td>
                    </tr>                     
                    <tr style="height: 24px">
                        <td>
                            <span id="lblType" class="Electrolux_light_bold Electrolux_Color">Type :</span>
                        </td>
                        <td>
                            <select id="type" name="" class="" runat="server">
                                <option value="F">Functional (F)</option>
                                <option value="T">Technical (T)</option>
                            </select > 
                        </td>
                    </tr>   
                        <tr>
                            <td colspan="3" >
                                <asp:RequiredFieldValidator ID="KeyNameTxtValidator" runat="server" ErrorMessage="KeyName cannot be blank" ControlToValidate="KeyNameTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                <br /><asp:RequiredFieldValidator ID="CommentTxtValidator" runat="server" ErrorMessage="Comment cannot be blank" ControlToValidate="CommentTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator>
                                <br /><asp:RequiredFieldValidator ID="DefautValueTxtValidator" runat="server" ErrorMessage="Default Value cannot be blank" ControlToValidate="DefautValueTxt" ForeColor="Red" ValidationGroup="CustomValidatorForAddKey"></asp:RequiredFieldValidator> 
                            </td>
                        </tr>
                    <tr style="height: 45px">
                        <td colspan="3" align="center">
                            <input type="button" class="btn red" id="btnCancelDispalyActionProfile" value="Cancel" onclick="CloseWindow()">
                            <asp:LinkButton runat="server" CssClass="btn green" ID="btnAdd" OnClick="btnAdd_Click" ValidationGroup="CustomValidatorForAddKey" >Submit</asp:LinkButton>
                        </td>
                    </tr>
                </tbody></table>
                    </div>
                </ContentTemplate>
             </telerik:RadWindow>
             <telerik:RadWindow ID="AllCountryGrid" OnClientBeforeClose="RefreshTable" RenderMode="Lightweight" Modal="true" DestroyOnClose="false" Title="Loading..." VisibleOnPageLoad="false" VisibleStatusbar="false" ShowContentDuringLoad="false" Behaviors="Close" Width="700px" Height="570px" runat="server">
        </telerik:RadWindow>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlEnvironment" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlCountry" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlType" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="WindowActionProfile" />
            <asp:AsyncPostBackTrigger ControlID="AllCountryGrid"/>

        </Triggers>
    </asp:UpdatePanel>


    <div id="dialog-confirm" title="Delete local value confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span>The local value set for this key will be permanently deleted. Are you sure?</p>
    </div>

</asp:Content>


