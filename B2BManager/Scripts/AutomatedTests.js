var selectedStep;
var testStepContainerUrl = 'AutomatedTestsStepManager.aspx?HideHeader=true';
var testRequestContainerUrl = 'AutomatedTestsRequestManager.aspx?HideHeader=true';
var containerUrl = 'AutomatedTestsElementManager.aspx?HideHeader=true&type=';
var generaterequestsUrl = 'AutomatedTestsRequestGenerator.aspx?HideHeader=true';

function ClientContextMenuItemClicked(sender, eventArgs) {
    var node = eventArgs.get_node();
    var item = eventArgs.get_menuItem();
    var menu = item.get_menu();
    
    switch (item.get_value()) {
        case "NewSubContainer": {
            NewElement('container', node.get_value());
            break;
        }
        case "EditContainer": {
            EditElement('container', node.get_value());
            break;
        }
        case "NewTestCase": {
            NewElement('testcase', node.get_value());
            break;
        }
        case "NewLoadingTestWave": {
            NewElement('loadtestwave', node.get_value());
            break;
        }
        case "EditTestCase": {
            EditElement('testcase', node.get_value());
            break;
        }

        case "EditLoadTestWave": {
            EditElement('loadtestwave', node.get_value());
            break;
        }

        case "DeleteContainer": {
            DeleteElement('container', node.get_value());
            break;
        }

        case "DeleteTestCase": {
            DeleteElement('testcase', node.get_value());
            break;
        }

        case "DeleteLoadTestWave": {
            DeleteElement('loadtestwave', node.get_value());
            break;
        }

        case "AllSteps":
        case "AllRequests": {            
            node.select();
            node.set_text("Loading...");
            break;
        }
    }
}

function ClientNodeClicking(sender, EventArgs) {
    var node = EventArgs.get_node();
    if ($(node)[0].get_attributes()._data.nodeType == "Folder") {
        EventArgs.set_cancel(true);
        return false;
    }
    node.set_text("Loading...");
}

function SelectStep(step) {
    var stepID = $(step).attr("data-step-id");
    var testCaseID = $(step).attr("data-test-case-id");
    var type = $(step).attr("data-type");
    $("#tableSteps").find(".selected-row").removeClass();
    $("#tableSteps").find("[data-step-id='" + stepID + "']").addClass("selected-row");
    switch ($(step).attr("data-step-type")) {
        case "TOP": {
            $(".MoveUp").hide();
            $(".MoveDown").show();
            break;
        }
        case "BOTTOM": {
            $(".MoveUp").show();
            $(".MoveDown").hide();
            break;
        }
        case "ONEITEM": {
            $(".MoveUpStep").hide();
            $(".MoveDown").hide();
            break;
        }
        default: {
            $(".MoveUp").show();
            $(".MoveDown").show();
            break;
        }
    }
    $(".EditStep").attr("onclick", "EditTestStep(" + testCaseID + "," + stepID + ",'" + type+"')");
    $(".DeleteStep").attr("onclick", "DeleteTestStep(" + stepID + ",'" + type +"')");
    $(".MoveUp").attr("onclick", "MoveTestStep(" + stepID + ",'UP','" + type +"')");
    $(".MoveDown").attr("onclick", "MoveTestStep(" + stepID + ",'Down','" + type +"')");
}
function OnClientStepItemClicked(sender, args) {
    selectedStep.Action = args.get_item().get_value();

    switch (selectedStep.Action) {
        case "Edit": {
            EditTestStep(selectedStep.TestCaseID, selectedStep.StepID, selectedStep.Type);
            break;
        }
        case "Delete": {
            DeleteTestStep(selectedStep.StepID, selectedStep.Type);
            break;
        }
        case "Before": {
            NewTestStep(selectedStep.TestCaseID, selectedStep.Type, selectedStep.StepID, -1);
            break;
        }
        case "After": {
            NewTestStep(selectedStep.TestCaseID, selectedStep.Type, selectedStep.StepID, 1);
            break;
        }
        case "Up": {
            MoveTestStep(selectedStep.StepID, 'Up', selectedStep.Type);
            break;
        }
        case "Down": {
            MoveTestStep(selectedStep.StepID, 'Down', selectedStep.Type);
            break;
        }
    }
}

function MoveTestStep(stepID, direction,type) {
    __doPostBack('OperationHdField', 'Move' + type+'|' + stepID.toString() + '|' + direction);
}

function OnClientItemClickingHandler(sender, eventArgs) {
    if (eventArgs.get_item().get_value() == "Move" || eventArgs.get_item().get_value() == "Add") {
        eventArgs.set_cancel(true);
        var menu = $find("ContentPlaceHolder1_ctl02_contextMenuSteps");
        menu.findItemByText(eventArgs.get_item().get_text()).show();
    }
}


function showContextMenu(menu, args) {
    var target = args.get_targetElement();
    var stepType = $(target).closest('tr').attr("data-step-type").toString();

    selectedStep = {
        StepID: parseInt($(target).closest('tr').attr("data-step-id").toString()),
        TestCaseID: parseInt($(target).closest('tr').attr("data-test-case-id").toString()),
        Type: $(target).closest('tr').attr("data-type").toString(),
        Action: ""
    }

    switch (stepType) {
        case "TOP": {
            menu.get_items().getItem(1).get_items().getItem(0).disable();
            break;
        }
        case "BOTTOM": {
            menu.get_items().getItem(1).get_items().getItem(1).disable();
            break;
        }
        case "ONEITEM": {
            menu.get_items().getItem(1).disable();
            break;
        }
        default: {
            menu.get_items().getItem(1).enable();
            menu.get_items().getItem(1).get_items().getItem(1).enable();
            menu.get_items().getItem(1).get_items().getItem(0).enable();
            break;
        }
    }
}

function ShowOrClosetestStepWindow(windowIdentifier, Show) {
    var oWnd = null;
    switch (windowIdentifier) {
        case "ManageTestStep":
            oWnd = $find("ContentPlaceHolder1_WindowTestSteps");
            break;
        case "DeleteTestStep":
            oWnd = $find("ContentPlaceHolder1_WindowDeleteTestStep");
            break;
    }
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



function OpenTestStepWindow(url) {
    var oWnd = $find("ContentPlaceHolder1_WindowTestSteps");
    oWnd.set_title('Loading...');
    oWnd.setUrl(url);
    ShowOrClosetestStepWindow("ManageTestStep", true);
}

function OpenDeleteTestStepWindow(url) {
    var oWnd = $find("ContentPlaceHolder1_WindowDeleteTestStep");
    oWnd.set_title('Loading...');
    oWnd.setUrl(url);
    ShowOrClosetestStepWindow("DeleteTestStep", true);
}

function NewTestStep(testCaseID, type, lnkStepID, position) {
    var url;
    if (type == "Step") {
        url = testStepContainerUrl;
    }
    else {
        url = testRequestContainerUrl;
    }
    url = url + '&tcid=' + testCaseID;
    if (position) {
        url = url + "&pos=" + position;
    }
    if (lnkStepID) {
        url = url + "&lnkStepID=" + lnkStepID;
    }
    OpenTestStepWindow(url);
}

function EditTestStep(testCaseID, elementID,type) {
    var url;
    if (type == "Step") {
        url = testStepContainerUrl;
    }
    else {
        url = testRequestContainerUrl;
    }
    url = url + '&tcid=' + testCaseID + '&id=' + elementID;
    OpenTestStepWindow(url)
}

function DeleteTestStep(elementID, type) {
    var url;
    if (type == "Step") {
        url = testStepContainerUrl;
    }
    else {
        url = testRequestContainerUrl;
    }
    url = url + '&id=' + elementID + '&op=delete';
    OpenDeleteTestStepWindow(url)
}

function CloseTestStepWindows() {
    ShowOrClosetestStepWindow("ManageTestStep", false);
    ShowOrClosetestStepWindow("DeleteTestStep", false);
}

function ShowOrCloseWindow(windowIdentifier, Show) {
    var oWnd = null;
    switch (windowIdentifier) {
        case "ManageTestElement":
            oWnd = $find("WindowContainers");
            break;
        case "DeleteElement":
            oWnd = $find("WindowDeleteElement");
            break;
        case "GenerateRequests":
            oWnd = $find("GenerateRequestsWindow");
            break;
    }
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

function OpenGenerateRequestsWindow() {
    var oWnd = $find("GenerateRequestsWindow");
    oWnd.set_title('Loading...');
    oWnd.setUrl(generaterequestsUrl);
    ShowOrCloseWindow("GenerateRequests", true);
}


function OpenContainerWindow(url) {
    var oWnd = $find("WindowContainers");
    oWnd.set_title('Loading...');
    oWnd.setUrl(url);
    ShowOrCloseWindow("ManageTestElement", true);
}

function OpenDeleteWindow(url) {
    var oWnd = $find("WindowDeleteElement");
    oWnd.set_title('Loading...');
    oWnd.setUrl(url);
    ShowOrCloseWindow("DeleteElement", true);
}

function NewElement(type, parentID) {
    var url = containerUrl + type;
    if (parentID) {
        url = url + "&pcid=" + parentID;
    }
    OpenContainerWindow(url);
}

function EditElement(type, elementID) {
    var url = containerUrl + type + '&cid=' + elementID;
    OpenContainerWindow(url)
}

function DeleteElement(type, elementID) {
    var url = containerUrl + type + '&cid=' + elementID + '&op=delete';
    OpenDeleteWindow(url)
}

function LoadTestCases(stepID) {
    ShowOrCloseWindow("ManageTestElement", false);
    ShowOrCloseWindow("DeleteElement", false);
    if (stepID) {
        __doPostBack("OperationHdField", 'Refresh|' + stepID);
    }
    else {
        __doPostBack("ContentPlaceHolder1_tvTestCases", 'Refresh');
    }
}