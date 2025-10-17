String.prototype.toDate = function (format) {
    var normalized = this.replace(/[^a-zA-Z0-9]/g, '-');
    var normalizedFormat = format.toLowerCase().replace(/[^a-zA-Z0-9]/g, '-');
    var formatItems = normalizedFormat.split('-');
    var dateItems = normalized.split('-');

    var monthIndex = formatItems.indexOf("mm");
    var dayIndex = formatItems.indexOf("dd");
    var yearIndex = formatItems.indexOf("yyyy");
    var hourIndex = formatItems.indexOf("hh");
    var minutesIndex = formatItems.indexOf("ii");
    var secondsIndex = formatItems.indexOf("ss");

    var today = new Date();

    var year = yearIndex > -1 ? dateItems[yearIndex] : today.getFullYear();
    var month = monthIndex > -1 ? dateItems[monthIndex] - 1 : today.getMonth() - 1;
    var day = dayIndex > -1 ? dateItems[dayIndex] : today.getDate();

    var hour = hourIndex > -1 ? dateItems[hourIndex] : today.getHours();
    var minute = minutesIndex > -1 ? dateItems[minutesIndex] : today.getMinutes();
    var second = secondsIndex > -1 ? dateItems[secondsIndex] : today.getSeconds();

    return new Date(year, month, day, hour, minute, second);
};

function GoHomePage() {
    GoTo("Home.aspx");
}

function GoTo(url) {
    window.location.href = url;
}

function createTooltipContent(sender, args) {
    sender.set_content($("#" + sender.get_targetControlID().toString().replace("ImgTooltip", "TooltipContent")).html());
}
var t0;
var inProgress;
var msgShown;
function RequestStart(sender, eventArgs) {
    centerLoadingPanel();
    msgShown = false;
    t0 = performance.now()
    inProgress = true;
    count();
    if ($("#ContentPlaceHolder1_chkBoxAutoRefresh")[0].checked != true) {
        $('#ContentPlaceHolder1_lblAutoRefresh').html("Auto Refresh:");
    }
}

function ProcessButton(button, processText) {
    $(button).addClass("loadingBackground").html(processText).val(processText).prop('disabled', true);
}

function UndoProcessButton(button, processText) {
    $(button).removeClass("loadingBackground").html(processText).val(processText).prop('disabled', false);
}

function BeginSearch() {
    $('*[id$="btnSearch"]').addClass("loadingBackground").text("Searching...").val("Searching...").prop('disabled', true);
}

$("#ContentPlaceHolder1_txtBoxSearchInDetails, #ContentPlaceHolder1_RadDateTimePickerTo_dateInput_text, #ContentPlaceHolder1_RadDateTimePickerFrom_dateInput_text, select, #ContentPlaceHolder1_txtRowsCount_text, #ContentPlaceHolder1_ddlCountry").keydown(function (e) {
    var keyCode = (event.keyCode ? event.keyCode : event.which);
    if (keyCode == 13) {
        $("#ContentPlaceHolder1_btnSearch").focus();
        BeginSearch();
        __doPostBack('ctl00$ContentPlaceHolder1$btnSearch', '');
    }
});

$("#ContentPlaceHolder1_txtBoxSearchInDetails, #ContentPlaceHolder1_RadDateTimePickerTo_dateInput_text, #ContentPlaceHolder1_RadDateTimePickerFrom_dateInput_text, select, #ContentPlaceHolder1_txtRowsCount_text, #ContentPlaceHolder1_ddlCountry").keypress(function (e) {
    var keyCode = (event.keyCode ? event.keyCode : event.which);
    if (keyCode == 13) {
        $("#ContentPlaceHolder1_btnSearch").focus();
        BeginSearch();
        __doPostBack('ctl00$ContentPlaceHolder1$btnSearch', '');
    }
});


function count() {
    var t1 = performance.now();
    var elapsedTime = parseFloat((t1 - t0) / 1e3).toFixed(3);
    $("#ExcutionTime").text(elapsedTime.toString() + "s");

    if (!msgShown && elapsedTime > 7.5) {
        ShowMsg();
        msgShown = true;
    }

    if (inProgress) {
        setTimeout(count, 0);
    }
}
var Interval = null;
function AutoRefresh(time) {
    if (Interval != null) clearInterval(Interval);
    Interval = setInterval(function () {
        if ($("#ContentPlaceHolder1_chkBoxAutoRefresh")[0].checked != true) {
            $('#ContentPlaceHolder1_lblAutoRefresh').html("Auto Refresh:");
            if (Interval != null) {
                clearInterval(Interval);
            }
        }
        else {
            time--;
            $('#ContentPlaceHolder1_lblAutoRefresh').html("Auto Refresh after <b>" + time.toString() + "</b> seconds");
            if (time === 1) {
                __doPostBack('ctl00$ContentPlaceHolder1$btnSearch', '');
                $('#ContentPlaceHolder1_lblAutoRefresh').html("Refreshing...");
                clearInterval(Interval);
            }
        }
    }, 1000);
}

function centerLoadingPanel() {
    centerElementOnScreen($get("RadAjaxLoadingPanel1"));
}

function setPopupTitle(sender) {
    sender.set_title("Loading...");
}


function centerElementOnScreen(element) {
    var scrollTop = document.body.scrollTop;
    var scrollLeft = document.body.scrollLeft;
    var viewPortHeight = document.body.clientHeight;
    var viewPortWidth = document.body.clientWidth;
    if (document.compatMode == "CSS1Compat") {
        viewPortHeight = document.documentElement.clientHeight;
        viewPortWidth = document.documentElement.clientWidth;
        scrollTop = document.documentElement.scrollTop;
        scrollLeft = document.documentElement.scrollLeft;
    }
    var topOffset = Math.ceil(viewPortHeight / 2 - element.offsetHeight / 2);
    var leftOffset = Math.ceil(viewPortWidth / 2 - element.offsetWidth / 2);
    var top = scrollTop + topOffset - 40;
    var left = scrollLeft + leftOffset - 70;
    element.style.position = "absolute";
    element.style.top = top + "px";
    element.style.left = left + "px";

}
function ResponseEnd(sender, eventArgs) {
    inProgress = false;
    setTimeout(HideMsg, 2500);
    if ($("#ContentPlaceHolder1_chkBoxAutoRefresh")[0].checked != true) {
        $('#ContentPlaceHolder1_lblAutoRefresh').html("Auto Refresh:");
    }
    $("#ContentPlaceHolder1_btnSearch").removeClass("loadingBackground").val("Search").prop('disabled', false);
}
function HideMsg() {
    $("#NotificationMsg").removeClass("visible").addClass("hidden");
}

function ShowMsg() {
    $("#NotificationMsg").removeClass("hidden").addClass("visible");
}

var strWindowFeatures = "menubar=yes,location=yes,resizable=yes,scrollbars=yes,status=yes";

function popup(url,refreshonclose) {
    var w = 1800, h = 750; lefts = 50; tops = 50;
    if (window.screen) {
        w = (window.screen.availWidth < w) ? window.screen.availWidth : w;
        h = window.screen.availHeight * 90 / 100;
        left = Number((screen.availWidth / 2) - (w / 2));
        tops = Number((screen.availHeight / 2) - (h / 2));
    }
    params = 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes,top=0,left=0, copyhistory=no,modal = yes, width=' + w + ',height=' + h + ', top=' + tops + ', left=' + left;
    newwin = window.open(url, '_blank', params);
    if (window.focus) { newwin.focus() }
    if (refreshonclose == true) {
        newwin.onbeforeunload = function () {
            if (newwin.opener.Refresh && typeof newwin.opener.Refresh === "function") {
                newwin.opener.Refresh();
            }
        }
    }
    return false;
}

function donwloadXML(fileName) {
    window.location.href = "GetXMLFile.ashx?file=" + fileName + "&type=download";
}

function donwloadJSON(fileName) {
    window.location.href = "GetJSONFile.ashx?file=" + fileName + "&type=download";
}
function OpenViewXMLFilesWindow(ApplicationID, ActionName, EnvironmentID, CorrelID, TableName, SopID, GlobalID, ID) {
    popup("ViewXMLFiles.aspx?ApplicationID=" + ApplicationID + "&ActionName=" + ActionName + "&EnvironmentID=" + EnvironmentID + "&CorrelID=" + CorrelID + "&TableName=" + TableName + "&SopID=" + SopID + "&GlobalID=" + GlobalID + "&ID=" + ID);
}

function OpenViewXMLFileWindow(ID,Type) {
    popup("ViewXMLFile.aspx?ID=" + ID + "&Type=" + ((Type == undefined) ? "0" : Type ));
}

function OpenViewJsonFileWindow(ApplicationID, ActionName, EnvironmentID, CorrelID, TableName, SopID, GlobalID, ID) {
    popup("ViewJsonFile.aspx?ApplicationID=" + ApplicationID + "&ActionName=" + ActionName + "&EnvironmentID=" + EnvironmentID + "&CorrelID=" + CorrelID + "&TableName=" + TableName + "&SopID=" + SopID + "&GlobalID=" + GlobalID + "&ID=" + ID);
}

function ChangeWindowDisplay(WindowID, Show) {
    var oWnd = $find(WindowID);
    if (oWnd != null) {
        if (Show) {
            oWnd.show();
        }
        else {
            oWnd.close();
        }
    }
    return false;
}

function ErrorPopup(str) {
    $.magnificPopup.open({
        items: {
            src: '<div class="white-popup error-popup" ><p class="text-justify" style="width: 90%;"><i class="fas fa-exclamation-triangle" style="font-size: 18pt;vertical-align: middle;"></i> ' + str + '</p></div>', 
            type: 'inline'
        }
    });
}