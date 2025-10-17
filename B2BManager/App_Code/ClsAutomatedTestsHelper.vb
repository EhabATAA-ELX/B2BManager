Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class ClsAutomatedTestsHelper

    Public Enum TestItemType
        NotSpecified = 0
        Folder = 1
        TestCase = 2
        LoadingTestWave = 3
    End Enum

    <Serializable>
    Public Class TestCase
        Public ID As Integer
        Public ParentID As Integer?
        Public Property ImagePath As String
            Get
                Return _ImagePath
            End Get
            Set
                _ImagePath = Value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _Name
            End Get
            Set
                _Name = Value
            End Set
        End Property

        Public Property Tooltip As String
            Get
                Return _Tooltip
            End Get
            Set
                _Tooltip = Value
            End Set
        End Property

        Public Property IsFolder As Boolean
            Get
                Return _IsFolder
            End Get
            Set
                _IsFolder = Value
            End Set
        End Property

        Public Property ItemType As TestItemType
            Get
                Return _itemType
            End Get
            Set(value As TestItemType)
                _itemType = value
            End Set
        End Property

        Private _ImagePath As String
        Private _Name As String
        Private _Tooltip As String
        Private _IsFolder As Boolean
        Private _itemType As TestItemType

        Public Sub New(id As Integer, parentId As Integer?, imagePath As String, name As String, tooltip As String, isFolder As Boolean, itemType As Integer)
            Me.ID = id
            Me.ParentID = parentId
            _ImagePath = imagePath
            _Name = name
            _IsFolder = isFolder
            _Tooltip = tooltip
            _itemType = itemType
        End Sub
    End Class

    <Serializable>
    Public Class TestStepCommand
        Private _CommandID As Integer
        Private _Command As String
        Private _ImageUrl As String
        Public IsEnabled As Boolean
        Public AvailableInKatalon As Boolean
        Public AvailableInSelenium As Boolean

        Public Property ImageUrl As String
            Get
                If String.IsNullOrEmpty(_ImageUrl) Then
                    If CommandID > 0 Then
                        If IsEnabled Then
                            _ImageUrl = "/Images/AutomatedTests/Available.png"
                        ElseIf AvailableInSelenium Then
                            _ImageUrl = "/Images/AutomatedTests/Selenium.png"
                        ElseIf AvailableInKatalon Then
                            _ImageUrl = "/Images/AutomatedTests/Katalon.png"
                        Else
                            _ImageUrl = "/Images/AutomatedTests/Unavailable.png"
                        End If
                    Else
                        _ImageUrl = "/Images/AutomatedTests/Unavailable.png"
                    End If
                End If
                Return _ImageUrl
            End Get
            Set(value As String)
                _ImageUrl = value
            End Set
        End Property
        Public Property CommandID As Integer
            Get
                Return _CommandID
            End Get
            Set(value As Integer)
                _CommandID = value
            End Set
        End Property

        Public Property Command As String
            Get
                Return _Command
            End Get
            Set
                _Command = Value
            End Set
        End Property


        Private _Description As String
        Public Sub New(Command As String, commandID As Integer, description As String)
            _CommandID = commandID
            _Command = Command
            _Description = description
        End Sub

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set
                _Description = Value
            End Set
        End Property


        Public Sub New(Command As String, commandID As Integer, description As String, enabled As Boolean, availableInKatalon As Boolean, availableInSelenium As Boolean)
            _CommandID = commandID
            _Command = Command
            Me.IsEnabled = enabled
            Me.AvailableInKatalon = availableInKatalon
            Me.AvailableInSelenium = availableInSelenium
        End Sub
    End Class

    <Serializable>
    Public Class TestStep
        Inherits TestStepCommand
        Public ID As Integer
        Public TestCaseID As Integer
        Public ExecutionOrder As Integer
        Public Property Target As String
            Get
                Return _Target
            End Get
            Set
                _Target = Value
            End Set
        End Property

        Public Property Value As String
            Get
                Return _Value
            End Get
            Set
                _Value = Value
            End Set
        End Property



        Private _Target As String
        Private _Value As String

        Public Sub New(id As Integer, testCaseID As Integer, Command As String, value As String, target As String, description As String, executionOrder As Integer, CommandID As Integer)
            MyBase.New(Command, CommandID, description)
            Me.ID = id
            Me.TestCaseID = testCaseID
            Me.ExecutionOrder = executionOrder
            _Value = value
            _Target = target
        End Sub
    End Class


    <Serializable>
    Public Class TestRequest
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set
                _Description = Value
            End Set
        End Property

        Public Property ID As Integer
            Get
                Return _iD
            End Get
            Set(value As Integer)
                _iD = value
            End Set
        End Property

        Public Property TestCaseID As Integer
            Get
                Return _testCaseID
            End Get
            Set(value As Integer)
                _testCaseID = value
            End Set
        End Property

        Public Property ExecutionOrder As Integer
            Get
                Return _executionOrder
            End Get
            Set(value As Integer)
                _executionOrder = value
            End Set
        End Property

        Public Property Environment As String
            Get
                Return _environment
            End Get
            Set(value As String)
                _environment = value
            End Set
        End Property

        Public Property Sop As String
            Get
                Return _sop
            End Get
            Set(value As String)
                _sop = value
            End Set
        End Property

        Public Property MessageType As String
            Get
                Return _messageType
            End Get
            Set(value As String)
                _messageType = value
            End Set
        End Property

        Public Property CustomerCode As String
            Get
                Return _customerCode
            End Get
            Set(value As String)
                _customerCode = value
            End Set
        End Property

        Public Property TotalItems As Integer
            Get
                Return _totalItems
            End Get
            Set(value As Integer)
                _totalItems = value
            End Set
        End Property

        Public Property MessageXML As String
            Get
                Return _MessageXML
            End Get
            Set
                _MessageXML = Value
            End Set
        End Property

        Public Property ActionID As Integer
            Get
                Return _actionID
            End Get
            Set(value As Integer)
                _actionID = value
            End Set
        End Property

        Public Property WcfB2BWebServiceURL As String
            Get
                Return _wcfB2BWebServiceURL
            End Get
            Set(value As String)
                _wcfB2BWebServiceURL = value
            End Set
        End Property

        Public Property WcfMethodName As String
            Get
                Return _wcfMethodName
            End Get
            Set(value As String)
                _wcfMethodName = value
            End Set
        End Property

        Public Property WcfUserName As String
            Get
                Return _wcfUserName
            End Get
            Set(value As String)
                _wcfUserName = value
            End Set
        End Property

        Public Property WcfPassword As String
            Get
                Return _wcfPassword
            End Get
            Set(value As String)
                _wcfPassword = value
            End Set
        End Property

        Public Property EnvironmentID As Integer
            Get
                Return _environmentID
            End Get
            Set(value As Integer)
                _environmentID = value
            End Set
        End Property

        Private _environmentID As Integer
        Private _actionID As Integer
        Private _Description As String
        Private _iD As Integer
        Private _testCaseID As Integer
        Private _executionOrder As Integer
        Private _environment As String
        Private _sop As String
        Private _messageType As String
        Private _customerCode As String
        Private _totalItems As Integer
        Private _MessageXML As String
        Private _wcfB2BWebServiceURL As String
        Private _wcfMethodName As String
        Private _wcfUserName As String
        Private _wcfPassword As String


        Public Sub New(iD As Integer, actionID As Integer, environmentID As Integer, testCaseID As Integer, Description As String, executionOrder As Integer, environment As String, sop As String, messageType As String, customerCode As String, totalItems As Integer, messageXML As String, wcfB2BWebServiceURL As String, wcfMethodName As String, wcfUserName As String, wcfPassword As String)
            _Description = Description
            _iD = iD
            _environmentID = environmentID
            _actionID = actionID
            _testCaseID = testCaseID
            _executionOrder = executionOrder
            _environment = environment
            _sop = sop
            _messageType = messageType
            _customerCode = customerCode
            _totalItems = totalItems
            _MessageXML = messageXML
            _wcfB2BWebServiceURL = wcfB2BWebServiceURL
            _wcfMethodName = wcfMethodName
            _wcfUserName = wcfUserName
            _wcfPassword = wcfPassword
        End Sub
    End Class

    Public Shared Function GetTestStepCommands() As List(Of TestStepCommand)
        Dim testStepCommands = New List(Of TestStepCommand)
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetTestStepCommands]")
        For Each dataRow As DataRow In dataTable.Rows
            testStepCommands.Add(New TestStepCommand(ClsDataAccessHelper.GetText(dataRow, "CommandName"),
                                       dataRow("ID"),
                                       ClsDataAccessHelper.GetText(dataRow, "Description"),
                                       dataRow("Enabled"),
                                       dataRow("AvailableInKatalon"),
                                       dataRow("AvailableInSelenium")))

        Next
        Return testStepCommands
    End Function

    Public Shared Function GetTestCases() As List(Of TestCase)
        Dim testCases = New List(Of TestCase)
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetTestCases]")
        For Each dataRow As DataRow In dataTable.Rows
            testCases.Add(New TestCase(dataRow("ID"),
                                       IIf(dataRow("ParentID") Is DBNull.Value, Nothing, dataRow("ParentID")),
                                       ClsDataAccessHelper.GetText(dataRow, "IconImageUrl"),
                                       ClsDataAccessHelper.GetText(dataRow, "Name"),
                                       ClsDataAccessHelper.GetText(dataRow, "Description"),
                                       dataRow("IsFolder"),
                                       dataRow("ItemType")))

        Next
        Return testCases
    End Function

    Public Shared Function GetTestCaseByID(ID As Integer) As TestCase
        Dim testCase As TestCase = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ID", ID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetTestCaseByID]", parameters)
        If dataTable.Rows.Count = 1 Then
            testCase = New TestCase(dataTable.Rows(0)("ID"),
                                       IIf(dataTable.Rows(0)("ParentID") Is DBNull.Value, Nothing, dataTable.Rows(0)("ParentID")),
                                       ClsDataAccessHelper.GetText(dataTable.Rows(0), "IconImageUrl"),
                                       ClsDataAccessHelper.GetText(dataTable.Rows(0), "Name"),
                                       ClsDataAccessHelper.GetText(dataTable.Rows(0), "Description"),
                                       dataTable.Rows(0)("IsFolder"),
                                       dataTable.Rows(0)("ItemType"))
        End If
        Return testCase
    End Function

    Public Shared Function GetTestStepByID(ID As Integer) As TestStep
        Dim testStep As TestStep = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ID", ID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetTestStepByID]", parameters)
        If dataTable.Rows.Count = 1 Then
            FillTestStepFromRow(testStep, dataTable.Rows(0))
        End If
        Return testStep
    End Function

    Public Shared Function GetRequestByID(ID As Integer) As TestRequest
        Dim testRequest As TestRequest = Nothing
        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)()
        parameters.Add(New SqlParameter("@ID", ID))
        Dim dataTable As DataTable = ClsDataAccessHelper.FillDataTable("[AutomatedTests].[GetRequestByID]", parameters)
        If dataTable.Rows.Count = 1 Then
            FillTestRequestFromRow(testRequest, dataTable.Rows(0))
        End If
        Return testRequest
    End Function

    Public Shared Sub FillTestStepFromRow(ByRef testStep As TestStep, row As DataRow)
        If row IsNot Nothing Then
            testStep = New TestStep(row("ID"),
                                    row("TestCaseID"),
                                    ClsDataAccessHelper.GetText(row, "Command"),
                                    ClsDataAccessHelper.GetText(row, "Value"),
                                    ClsDataAccessHelper.GetText(row, "Target"),
                                    ClsDataAccessHelper.GetText(row, "Description"),
                                    row("ExecutionOrder"),
                                    IIf(row("CommandID") Is DBNull.Value, 0, row("CommandID")))
            If row("CommandID") IsNot DBNull.Value Then
                testStep.IsEnabled = row("Enabled")
                testStep.AvailableInKatalon = row("AvailableInKatalon")
                testStep.AvailableInSelenium = row("AvailableInSelenium")
            End If
        End If
    End Sub

    Public Shared Sub FillTestRequestFromRow(ByRef testRequest As TestRequest, row As DataRow)
        If row IsNot Nothing Then
            testRequest = New TestRequest(row("ID"),
                                    row("ActionID"),
                                    row("EnvironmentID"),
                                    row("TestCaseID"),
                                    ClsDataAccessHelper.GetText(row, "Description"),
                                    row("ExecutionOrder"),
                                    ClsDataAccessHelper.GetText(row, "Environment"),
                                    ClsDataAccessHelper.GetText(row, "SOP_ID"),
                                    ClsDataAccessHelper.GetText(row, "MessageType"),
                                    ClsDataAccessHelper.GetText(row, "CustomerCode"),
                                    row("TotalItems"),
                                    ClsDataAccessHelper.GetText(row, "MSG_XML"),
                                    ClsDataAccessHelper.GetText(row, "WcfB2BWebServiceURL"),
                                    ClsDataAccessHelper.GetText(row, "WcfMethodName"),
                                    ClsDataAccessHelper.GetText(row, "WcfUserName"),
                                    ClsDataAccessHelper.GetText(row, "WcfPassword"))

        End If
    End Sub
End Class


