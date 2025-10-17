// data attributes:
// AssignmentTypeId : integer       0 : unassigned, 1 : assigned
// EnvironmentId : string           can be transformed in a Guid
// CountryId : string               can be transformed in a Guid
// ObjectTypeId : integer           1 : Document (table T_DOCUMENTS from database B2BCMS_V2)
// ObjectIds : string               ids separated with ';'
// CustomerIds : string             ids separated with ';'
function AssignmentLogs(data) {
    $.ajax({
        type: 'POST',
        url: 'B2BManagerService.svc/AssignmentLogs',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(data),
        async: true,
        success: function (res) {
        },
        error: function (e) {
            console.log("Error  : " + e.statusText);
        }
    });
}