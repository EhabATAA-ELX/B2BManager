<%@ Page Title="" Language="VB" MasterPageFile="~/BasicMasterPage.master" AutoEventWireup="false" CodeFile="EbusinessInsightsChartProfile.aspx.vb" Inherits="EbusinessInsightsChartProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .RadComboBoxDropDown .rcbImage {
            width: auto !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadToolTipManager RegisterWithScriptManager="true" rendermode="Lightweight" ID="RadToolTipManager1" OnClientBeforeShow="createTooltipContent" HideEvent="ManualClose" ShowEvent="OnMouseOver"
        runat="server" RelativeTo="Element" Position="MiddleLeft">
        <TargetControls>
            <telerik:ToolTipTargetControl IsClientID="true" TargetControlID="ImgTooltipHelp_ExcelSheetName" />
        </TargetControls>
    </telerik:RadToolTipManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
            <fieldset style="width: 95%; margin: 15px">
                <legend class="Electrolux_light_bold Electrolux_Color">Chart Details</legend>
                <table style="width: 100%;">
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label4" CssClass="Electrolux_light_bold Electrolux_Color width130px">Title:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxChartTitle"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtBoxChartTitle" runat="server" ID="rqFieldValidatorChartTitle" Text="* Required" ForeColor="Red" ValidationGroup="ChartDetails"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label5" CssClass="Electrolux_light_bold Electrolux_Color width130px">Subtitle:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxChartSubTitle" TextMode="MultiLine" Height="40"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label9" CssClass="Electrolux_light_bold Electrolux_Color width130px">Section:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlSections" CssClass="Electrolux_light width230px" AppendDataBoundItems="true"></asp:DropDownList>&nbsp;<asp:ImageButton runat="server" ID="BtnRefreshSections" ClientIDMode="Static" ImageUrl="Images/Reload.png" CssClass="verticalAlignBottom" Width="20" Height="20" ToolTip="Refresh" OnClientClick="refreshSections()" />
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label6" CssClass="Electrolux_light_bold Electrolux_Color width130px">Type:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox runat="server" CssClass="Electrolux_light width230px" AutoPostBack="true" OnSelectedIndexChanged="ddlChartType_SelectedIndexChanged" ID="ddlChartType">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Pie Chart" Value="1" ImageUrl="Images/Insights/Pie.png" />
                                    <telerik:RadComboBoxItem Text="Line Chart" Value="2" ImageUrl="Images/Insights/Line.png" />
                                    <telerik:RadComboBoxItem Text="Horizontal Bar Chart" Value="3" ImageUrl="Images/Insights/Bar.png" />
                                    <telerik:RadComboBoxItem Text="Vertical Bar chart" Value="5" ImageUrl="Images/Insights/VerticalBar.png" />
                                    <telerik:RadComboBoxItem Text="Custom Data Table" Enabled="false" Value="4" ImageUrl="Images/Insights/datatable.png" />                                    
                                </Items>
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label10" CssClass="Electrolux_light_bold Electrolux_Color">Accessibility Level:</asp:Label>
                        </td>
                        <td class="width230px" align="left">
                            <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width230px" AppendDataBoundItems="true" ID="ddlSharingType">
                                <Items>
                                    <telerik:RadComboBoxItem ImageUrl="Images/Private.png" Text="Private" Value="0" Selected="true" />
                                    <telerik:RadComboBoxItem ImageUrl="Images/Shared.png" Text="Shared" Value="1" />
                                </Items>
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label8" CssClass="Electrolux_light_bold Electrolux_Color width130px">Excel Sheet Name:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light width230px" ID="txtBoxExcelSheetName" MaxLength="31"></asp:TextBox>
                            <img src='Images/Info.png' class='MoreInfoImg vertical-align-bottom' id='ImgTooltipHelp_ExcelSheetName' width='18' height='18' alt='More details' />
                            <div class='hidden' style='margin: 25px;' id="TooltipContentHelp_ExcelSheetName">
                                It defines the excel sheet name
                                <br />
                                in a generated dashboard excel export.<br />
                                Maximum length is 31 charachters
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label7" CssClass="Electrolux_light_bold Electrolux_Color width130px">Comments:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light width230px" TextMode="MultiLine" Height="40" ID="txtBoxComments"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset style="width: 95%; margin: 15px;">
                <legend class="Electrolux_light_bold Electrolux_Color">Data Source</legend>
                <table>
                    <tr runat="server" id="trDataFrom">
                        <td>
                            <asp:Label runat="server" ID="Label1" CssClass="Electrolux_light_bold Electrolux_Color">Data From:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" ID="ddlSource" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlSource_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trFreeQuery" visible="false">
                        <td class="width130px">
                            <asp:Label runat="server" ID="Label13" CssClass="Electrolux_light_bold Electrolux_Color width130px">Query Text:</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" CssClass="Electrolux_light" Width="320" ID="txtBoxQueryText" TextMode="MultiLine" Height="65"></asp:TextBox>
                        </td>
                    </tr>
                    <tr runat="server" id="trFreeQueryHelp" visible="false">
                        <td colspan="2">
                            <span style="font-size:9pt" class="Electrolux_Color"><span style="color:#f2c5c8;">[INFO]</span> you can make your query running using the different parameters (Country split, countries and environment) by adding them as follows: [SOPIDs] for Countries, [CountrySplit] for country split and [EnvironmentID] for environment.</br>For example: EXEC [YOUR_STORED_PROCEDURE_NAME] @SOPIDs='[SOPIDs]',@CountrySplit=[CountrySplit],@EnvironmentID=[EnvironmentID]</span>
                        </td>
                    </tr>
                    <tr runat="server" id="trSurveyEnvironment" visible="false">
                        <td>
                            <asp:Label runat="server" ID="lblSurvey" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" AppendDataBoundItems="true"  AutoPostBack="true"  ID="ddlSurveyEnvironment" OnSelectedIndexChanged="ddlSurveyEnvironment_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>

                    <tr runat="server" id="trSurveySelection" visible="false">
                        <td>
                            <asp:Label runat="server" ID="Label11" CssClass="Electrolux_light_bold Electrolux_Color">Survey:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" AppendDataBoundItems="true"  AutoPostBack="true"  ID="ddlSurvey" OnSelectedIndexChanged="ddlSurvey_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trSurveyQuestion" visible="false">
                        <td>
                            <asp:Label runat="server" ID="Label12" CssClass="Electrolux_light_bold Electrolux_Color">Question:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" AppendDataBoundItems="true" ID="ddlSurveyQuestion">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trLogViewerApplicationName">
                        <td class="width130px">
                            <asp:Label runat="server" ID="lblApplicationName" CssClass="Electrolux_light_bold Electrolux_Color width130px">Application name:</asp:Label>
                        </td>
                        <td class="width180px">
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" OnSelectedIndexChanged="ddlApplicationName_SelectedIndexChanged" AppendDataBoundItems="true" AutoPostBack="true" ID="ddlApplicationName">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trLogViewerActions">
                        <td>
                            <asp:Label runat="server" ID="lblAction" CssClass="Electrolux_light_bold Electrolux_Color">Action:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" AppendDataBoundItems="true" ID="ddlAction">
                                <asp:ListItem Text="All" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trLogViewerActionStatus">
                        <td>
                            <asp:Label runat="server" ID="lblError" CssClass="Electrolux_light_bold Electrolux_Color">Actions status:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" ID="ddlActionToDisplay">
                                <asp:ListItem Text="All" Value="0"></asp:ListItem>
                                <asp:ListItem Text="With errors only" Value="1"></asp:ListItem>
                                <asp:ListItem Text="With no errors" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trLogViewerGroupBy">
                        <td>
                            <asp:Label runat="server" ID="lblGroupBy" CssClass="Electrolux_light_bold Electrolux_Color">Group by:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" ID="ddlGroupBy">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trDateIntervalType">
                        <td>
                            <asp:Label runat="server" ID="Label2" CssClass="Electrolux_light_bold Electrolux_Color">Date Interval Type:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width230px" OnSelectedIndexChanged="ddlDateInterval_SelectedIndexChanged" AutoPostBack="true" ID="ddlDateInterval">
                                <asp:ListItem Text="Static" Selected="True" Value="Static"></asp:ListItem>
                                <asp:ListItem Text="Dynamic" Value="Dynamic"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="trDateIntervalDynamic" visible="false">
                        <td>
                            <asp:Label runat="server" ID="Label3" CssClass="Electrolux_light_bold Electrolux_Color">Date Interval:</asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" Width="320" OnSelectedIndexChanged="ddlDynamicDateInterval_SelectedIndexChanged" AutoPostBack="true" ID="ddlDynamicDateInterval">
                                <asp:ListItem Text="Today" Selected="True" Value="Today"></asp:ListItem>
                                <asp:ListItem Text="In last (to specify)" Value="InLast"></asp:ListItem>
                                <asp:ListItem Text="Ever (since data available)" Value="Ever"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Panel runat="server" ID="pnlIntervalToSpecify" Visible="false">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadNumericTextBox runat="server" MinValue="1" MaxValue="90" Value="1" ShowSpinButtons="true" Width="50" DataType="Integer" ID="txtDateUnit" CssClass="Electrolux_light_bold Electrolux_Color">
                                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color" ID="ddlDateUnits">
                                                <asp:ListItem Text="Day" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Week" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Month" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Year" Value="4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr runat="server" id="trDateIntervalStaticFrom">
                        <td>
                            <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">From:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadDateTimePicker ID="RadDateTimePickerFrom" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
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
                    <tr runat="server" id="trDateIntervalStaticTo">
                        <td>
                            <asp:Label runat="server" ID="lblTo" CssClass="Electrolux_light_bold Electrolux_Color">To:</asp:Label>
                        </td>
                        <td>
                            <telerik:RadDateTimePicker ID="RadDateTimePickerTo" DateInput-DateFormat="dd/MM/yyyy" TimeView-TimeFormat="HH:mm" runat="server">
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
                </table>
            </fieldset>

            <table align="center">
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblErrorMessageInfo" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="button" class="btn red" id="btnCancelDashboardProfile" value="Cancel" onclick="window.parent.ShowOrCloseChartWindow(false)" />
                        <asp:LinkButton runat="server" CssClass="btn green" ID="btnSaveOrUpdateChart" ClientIDMode="Static" Text="Save" OnClientClick="ProcessButton(this,'Saving...')" OnClick="btnSaveOrUpdateChart_Click" CausesValidation="true" ValidationGroup="ChartDetails"></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn lightblue" ID="btnPreview" ClientIDMode="Static" Text="Preview" OnClick="btnPreview_Click" CausesValidation="false" ValidationGroup="ChartDetails"></asp:LinkButton>
                        <input type="button" class="btn bleu" id="btnManageSection" value="Manage Sections" onclick="window.parent.ManageSections()" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlDateInterval" />
            <asp:AsyncPostBackTrigger ControlID="ddlDynamicDateInterval" />
            <asp:AsyncPostBackTrigger ControlID="BtnRefreshSections" />
            <asp:AsyncPostBackTrigger ControlID="ddlSource" />
            <asp:AsyncPostBackTrigger ControlID="ddlSurveyEnvironment" />
            <asp:AsyncPostBackTrigger ControlID="ddlSurvey" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function refreshSections() {
            $("#BtnRefreshSections")[0].src = "Images/Loader.gif";
            __doPostBack('BtnRefreshSections', '')
        }
    </script>
</asp:Content>

