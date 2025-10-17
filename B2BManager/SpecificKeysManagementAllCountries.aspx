<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="SpecificKeysManagementAllCountries.aspx.vb" Inherits="SpecificKeysManagementAllCountries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
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
        .flag{
            height: 18px;
            vertical-align: bottom;
        }
        .flagLabel{
            padding-left:15px;
        }
    </style>
    <script>

         $(document).ready(function () {
             BindDataTable();              
        });

        function BindDataTable() {
            $("[id$='JQueryDataTable']").removeClass("DisplayNone");
            $("[id$='JQueryDataTable']").DataTable({
                "stateSave": false,
                "dom": '<"floatLeft DataTableCustom"f><t><"DatatableBottom"<"PopupDatatableBottomLeft"p><"PopupDatatableBottomRight"i>><"clear">'               
            });
        }

         function Delete(KeyName, SopId, EnvironmentID,IdRow) {

            $( "#dialog-confirm" ).dialog({
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
                              var url = "SpecificKeysManagementAllCountries.aspx/DeleteLine";
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
                                      var table = $("[id$='JQueryDataTable']").DataTable();
                                      table.row($("#" + IdRow)).remove().draw();
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

          function DeleteAll(KeyName,  EnvironmentID) {

              $("#dialog-confirmAll").dialog({
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
                              var url = "SpecificKeysManagementAllCountries.aspx/DeleteAllLine";
                              var data = {
                                  EnvironmentID: EnvironmentID,
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
                                      var table = $("[id$='JQueryDataTable']").DataTable();
                                      table.clear().draw();
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


        function HideLoadingPanel(sender, eventArgs) {
            $("#LoadingPanel").addClass("hidden");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>

             <asp:Table ID="JQueryDataTable" runat="server" CssClass="DisplayNone" Style="border-collapse: collapse !important; font-size: 12px;">
                <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" TableSection="TableHeader">
                    <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Action</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell0" runat="server">KeyName</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">CountryName</asp:TableHeaderCell>
                    <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">CountryValue</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </ContentTemplate>
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID="ddlType" />--%>
        </Triggers>
    </asp:UpdatePanel>

     <div id="dialog-confirm" title="Delete local key value confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span>The local value set for this key will be permanently deleted. Are you sure?</p>
    </div>

     <div id="dialog-confirmAll" title="Delete key confirmation" class="DisplayNone">
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:12px 12px 20px 0;"></span>This key will be permanently deleted with all related local values. Are you sure?</p>
    </div>

</asp:Content>

