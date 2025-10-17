<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPage.master" AutoEventWireup="false" CodeFile="ChatBotConversationsViewer.aspx.vb" Inherits="Chatbot_ChatBotConversationsViewer" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <link rel="stylesheet" href="https://b2bstag.electrolux.net/EluxBot/CSS/recast.min.css" />
    <link rel="stylesheet" href="https://b2bstag.electrolux.net/EluxBot/CSS/webchat.css" />
    <script type="text/javascript">
        function selectUser(chatbotID, userglobalID) {
            $(".chatbot-selected-item").removeClass("chatbot-selected-item");
            __doPostBack('selectedUser', chatbotID + '|' + userglobalID);
        }

        function SelectConversation(conversationID, chatbotID) {
            $(".chatbot-conversation-alternate").removeClass("chatbot-selected-item");
            $(".chatbot-conversation").removeClass("chatbot-selected-item");
            __doPostBack('selectedConversation', chatbotID + '|' + conversationID);
        }

        function selectConversationFinish(conversationID) {
            $("#conversation_" + conversationID).addClass("chatbot-selected-item");
            $("#conversation_" + conversationID)[0].scrollIntoView();
        }
        
        function selectUserFinish(userglobalID) {
            $("#user_" + userglobalID).addClass("chatbot-selected-item");
            $("#user_" + userglobalID)[0].scrollIntoView();
        }

        function searchUser() {
            var textSearch = $("#searchUsertxtBox")[0].value.toString().toLowerCase().replace("&", " ");
            if (textSearch.length > 0) {
                $("div[id^=user]:not([data-search*=" + textSearch + "])").addClass("hidden");
                $("div[id^=user][data-search*=" + textSearch + "]").removeClass("hidden");

                $.each($("div[id^=user][data-search*=" + textSearch + "]"), function (index, value) {
                    var id = $(value)[0].id;
                    $.each($("div[id=" + id + "] .chatbot-search-text"), function (index1, value1) {
                        if (textSearch != $("#searchUsertxtBox")[0].value.toString()) {
                            $(value1).html($(value1).text().replace($("#searchUsertxtBox")[0].value.toString(), "<yellow>" + $("#searchUsertxtBox")[0].value.toString() + "</yellow>"));
                        }
                        else {
                            $(value1).html($(value1).text().replace(textSearch, "<yellow>" + textSearch + "</yellow>"));
                        }
                    });
                });

            }
            else {
                $("div[id^=user]").removeClass("hidden");
                $.each($("div[id^=user]"), function (index, value) {
                    var id = $(value)[0].id;
                    $.each($("div[id=" + id + "] .chatbot-search-text"), function (index1, value1) {
                        $(value1).html($(value1).text());
                    });
                });
            }
        }

        function occurrences(string, subString, allowOverlapping) {

            string += "";
            subString += "";
            if (subString.length <= 0) return (string.length + 1);

            var n = 0,
                pos = 0,
                step = allowOverlapping ? 1 : subString.length;

            while (true) {
                pos = string.indexOf(subString, pos);
                if (pos >= 0) {
                    ++n;
                    pos += step;
                } else break;
            }
            return n;
        }

        function searchInConversation() {
            var occurences = 0;
            var textSearch = $("#searchInConversationTextBox")[0].value.toString().replace("&", " ");
            $.each($(".RecastAppText"), function (index, value) {
                $(value).html($(value).html().replace("<yellow>", "").replace("</yellow>", ""));
                $(value).html($(value).html().replace(textSearch, "<yellow>" + textSearch + "</yellow>"));
                occurences += occurrences($(value).text(), textSearch, true);
            });
            var occurencesValue = "";
            if (occurences > 0 && textSearch.length > 0) {
                occurencesValue = "Occurences of \"<span style='color:#2196F3;'>" + textSearch + "</span>\" : " + occurences.toString();
            }
            $("#spanOccurences").html(occurencesValue);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="Filters no-print">
                <tr>
                    <td class="width130px">
                        <asp:Label runat="server" ID="lblChatbotsName" CssClass="Electrolux_light_bold Electrolux_Color width130px">&ensp;&ensp;Chatbots:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" OnSelectedIndexChanged="ddlChatbots_SelectedIndexChanged" AppendDataBoundItems="true" AutoPostBack="true" ID="ddlChatbots">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblEnvironment" CssClass="Electrolux_light_bold Electrolux_Color">Environment:</asp:Label>
                    </td>
                    <td class="width180px">
                        <asp:DropDownList runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlEnvironment">
                        </asp:DropDownList>
                    </td>
                    <td class="width120px">
                        <asp:Label runat="server" ID="lblCountry" CssClass="Electrolux_light_bold Electrolux_Color">Country:</asp:Label>
                    </td>
                    <td class="width180px">
                        <telerik:RadComboBox runat="server" CssClass="ddl_Text Electrolux_Color width180px" AppendDataBoundItems="true" ID="ddlCountry">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="All" Value="0" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label runat="server" ID="lblFrom" CssClass="Electrolux_light_bold Electrolux_Color">&ensp;&ensp;Show users who interacted with the chatbot between:</asp:Label>
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
                    <td>
                        <asp:Label runat="server" ID="lblTo" CssClass="Electrolux_light_bold Electrolux_Color">And:</asp:Label>
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
                    <td class="width180px">
                        <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn bleu" OnClientClick="BeginSearch()" OnClick="btnSearch_Click" Text="Search" />
                    </td>
                </tr>
            </table>

            <div style="width: 100%; padding: 25px">
                <asp:HiddenField runat="server" ClientIDMode="Static" ID="selectedUser" />
                <asp:HiddenField runat="server" ClientIDMode="Static" ID="selectedConversation" />
                <div class="chatbotConversationPanel Panel1 no-print">
                    <input type="text" placeholder="Search for customer or user" class="chatbot-input" id="searchUsertxtBox" onkeyup="searchUser()" />
                    <div class="chatbotConversationContent">
                        <asp:Repeater runat="server" ID="usersRepeter" ClientIDMode="Static">
                            <ItemTemplate>
                                <div class="chatbot-user" data-search="<%# Eval("U_LOGIN").ToString().ToLower() %>&<%# Eval("U_FULLNAME").ToString().ToLower() %>&<%# Eval("CustomerCode").ToString().ToLower() %>" id="user_<%# Eval("U_GLOBALID") %>" onclick="selectUser('<%# Eval("ChatBotID")  %>','<%# Eval("U_GLOBALID") %>')">
                                    <div style="position: relative;">
                                        <div class="chatbot-navi">
                                            <img src="Images/userphoto.png" class="floatLeft" height='68' width='68' />
                                            <table class="floatLeft" style="margin-left: 10px">
                                                <tr>
                                                    <td class="chatbot-search-text"><%# Eval("U_LOGIN") %></td>
                                                </tr>
                                                <tr>
                                                    <td class="chatbot-search-text"><%# Eval("U_FULLNAME") %></td>
                                                </tr>
                                                <tr>
                                                    <td class="chatbot-search-text"><%# Eval("CustomerCode") %></td>
                                                </tr>
                                            </table>
                                            <table class="floatRight" style="margin-right: 10px">
                                                <tr>
                                                    <td><%# CDate(Eval("LatestAction")).ToString("dd/MM/yyyy HH:mm")  %></td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <img src='<%# Eval("ImageUrl") %>' title='<%# Eval("SopName")%>' width="20" height="18" /></td>
                                                </tr>
                                            </table>

                                        </div>
                                        <div class="chatbot-infoi">
                                            <span <%# "style='background-color:" & Eval("StatusColor") & "'" %>>&emsp;</span>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="chatbotConversationPanel Panel2 no-print">
                    <div class="chatbotConversationContent">
                        <asp:Repeater runat="server" ID="conversationsRepeter" ClientIDMode="Static">
                            <ItemTemplate>
                                <div class="chatbot-conversation" id="conversation_<%# Eval("ConversationID") %>" onclick="SelectConversation('<%# Eval("ConversationID") %>',<%# Eval("ChatBotID") %>)">
                                    <table class="floatLeft" style="margin-left: 10px">
                                        <tr>
                                            <td>Started on:
                                            </td>
                                            <td>
                                                <%# CDate(Eval("DateAdded")).ToString("dd/MM/yyyy HH:mm")  %>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Duration:
                                            </td>
                                            <td>
                                                <b><%# ClsHelper.GetDurationInFriendlyText(Eval("Duration"))  %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Total interactions:
                                            </td>
                                            <td>
                                                <b><%# Eval("TotalInteractionsLogged") %></b>
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="floatRight" style="margin-right: 10px">
                                        <tr>
                                            <td>Availability requests:
                                            </td>
                                            <td>
                                                <b><%# Eval("AvailabilityRequests") %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Price requests:
                                            </td>
                                            <td>
                                                <b><%# Eval("PriceRequests") %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Fallbacks logged:
                                            </td>
                                            <td>
                                                <span <%# "style='color:" & Eval("FallbackBackColor") & "'" %>><b><%# Eval("NumberOfFallbacks") %></b></span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <div class="chatbot-conversation-alternate" id="conversation_<%# Eval("ConversationID") %>" onclick="SelectConversation('<%# Eval("ConversationID") %>',<%# Eval("ChatBotID") %>)">
                                    <table class="floatLeft" style="margin-left: 10px">
                                        <tr>
                                            <td>Started on:
                                            </td>
                                            <td>
                                                <%# CDate(Eval("DateAdded")).ToString("dd/MM/yyyy HH:mm")  %>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Duration:
                                            </td>
                                            <td>
                                                <b><%# ClsHelper.GetDurationInFriendlyText(Eval("Duration"))  %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Total interactions:
                                            </td>
                                            <td>
                                                <b><%# Eval("TotalInteractionsLogged") %></b>
                                            </td>
                                        </tr>
                                    </table>
                                    <table class="floatRight" style="margin-right: 10px">
                                        <tr>
                                            <td>Availability requests:
                                            </td>
                                            <td>
                                                <b><%# Eval("AvailabilityRequests") %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Price requests:
                                            </td>
                                            <td>
                                                <b><%# Eval("PriceRequests") %></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Fallbacks logged:
                                            </td>
                                            <td>
                                                <span <%# "style='color:" & Eval("FallbackBackColor") & "'" %>><b><%# Eval("NumberOfFallbacks") %></b></span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </AlternatingItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="chatbotConversationPanel Panel3">
                    <div class="chatbot-input-container no-print">
                        <input type="text" placeholder="Search in conversation" class="chatbot-input-absolute chatbot-input" id="searchInConversationTextBox" onkeyup="searchInConversation()" />    
                        <img src="Images/print.png" class="print-img" onclick="window.print()" title="Print conversation" />
                    </div>
                    <div class="chatbotConversationContent">
                        <span id="spanOccurences" style="font-weight: bold; font-size: 8pt; padding-left: 5px;"></span>
                        <div id="conversationContent" runat="server"></div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlChatbots" />
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
            <asp:AsyncPostBackTrigger ControlID="selectedUser" />
            <asp:AsyncPostBackTrigger ControlID="selectedConversation" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

