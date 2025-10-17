<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ContactPreview.ascx.vb" Inherits="UserControls_ContactPreview" %>

<asp:Repeater runat="server" ID="rptContact">
    <ItemTemplate>
        <div class="Spacer Center">
            <div class="FloatLeft Width100 HeaderLine LineHeight16px">
                <div class="TextLeft MarginTop5 MarginBottom5 FontSize13px">
                    <%#  IIf(container.dataitem("CT_BEFORETEXT1").GetType is GetType(dbNull), "&nbsp;", IIf(container.dataitem("CT_BEFORETEXT1").tostring = "" , "&nbsp;", container.dataitem("CT_BEFORETEXT1")))%>
                </div>
                <div class="Title FloatLeft ">
                    <div class="FloatLeft" style="text-align: left !important;">
                        <%#IIf(Container.DataItem("CT_NAME").GetType Is GetType(DBNull), "&nbsp;", IIf(Container.DataItem("CT_NAME").ToString = "", "&nbsp;", Container.DataItem("CT_NAME")))%>
                    </div>
                    <div class="FloatLeft TitlePrinc">
                        &nbsp; <%#IIf(container.dataitem("CT_TYPE").GetType is GetType(dbNull), "&nbsp;", IIf(container.dataitem("CT_TYPE").tostring = "" , "&nbsp;", container.dataitem("CT_TYPE")))%>
                    </div>
                </div>
                <div class="TitleContent FloatLeft">
                    <div class="Left">
                        <table class="FieldValue">
                            <%#IIf(Container.DataItem("CT_STREET1").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_STREET1").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_STREET1") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_STREET2").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_STREET2").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_STREET2") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_STREET3").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_STREET3").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_STREET3") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_STREET4").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_STREET4").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_STREET4") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_STREET5").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_STREET5").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_STREET5") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_POSTCODE").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_POSTCODE").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Trim(Container.DataItem("CT_POSTCODE") & " " & Container.DataItem("CT_CITY")) & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_COUNTY").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_COUNTY").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & Container.DataItem("CT_COUNTY") & "</TD></TR>"))%>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <%# IIf(Container.DataItem("CT_OFFICE_PHONE").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_OFFICE_PHONE").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & "<span>Office phone:</span>" & " " & Container.DataItem("CT_OFFICE_PHONE") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_DIRECT_PHONE").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_DIRECT_PHONE").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & "<span>Direct phone:</span>" & " " & Container.DataItem("CT_DIRECT_PHONE") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_FAX").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_FAX").ToString = "", "", "<TR><TD Class=""TextNormalTitle"">" & "<span>Fax:</span>" & " " & Container.DataItem("CT_FAX") & "</TD></TR>"))%>
                            <%# IIf(Container.DataItem("CT_EMAIL").GetType Is GetType(DBNull), "", IIf(Container.DataItem("CT_EMAIL").ToString = "", "", "<TR><TD Class=""TextMailTitle"">Email: <a Class=""TextMail"" href='mailto:" & Container.DataItem("CT_EMAIL") & "'>" & Container.DataItem("CT_EMAIL") & "</a></TD></TR>"))%>
                        </table>
                    </div>
                </div>
                <div class="FooterText">
                    <%#  IIf(container.dataitem("CT_BEFORETEXT2").GetType is GetType(dbNull), "&nbsp;", IIf(container.dataitem("CT_BEFORETEXT2").tostring = "" , "&nbsp;", container.dataitem("CT_BEFORETEXT2")))%>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
