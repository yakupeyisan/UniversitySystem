namespace Identity.Domain.Enums;

public enum PermissionType
{
    // User Management
    CreateUser = 0,
    ReadUser = 1,
    UpdateUser = 2,
    DeleteUser = 3,

    // Role Management
    CreateRole = 10,
    ReadRole = 11,
    UpdateRole = 12,
    DeleteRole = 13,

    // Permission Management
    CreatePermission = 20,
    ReadPermission = 21,
    UpdatePermission = 22,
    DeletePermission = 23,

    // Person Management
    ManagePerson = 30,
    ViewPerson = 31,

    // Academic
    ManageAcademic = 40,
    ViewAcademic = 41,

    // Virtual POS
    ManageVirtualPOS = 50,
    ViewVirtualPOS = 51,
    ProcessPayments = 52,

    // Wallet
    ManageWallet = 60,
    ViewWallet = 61,
    TransferFunds = 62,

    // Access Control
    ManageAccessControl = 70,
    ViewAccessLogs = 71,
    IssueCards = 72,

    // Payroll
    ManagePayroll = 80,
    ViewPayroll = 81,
    ProcessPayroll = 82,

    // General
    ViewDashboard = 100,
    ExportData = 101,
    ViewReports = 102,
    ManageSettings = 103
}