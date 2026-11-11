# Leave Management Project Guide

A beginner-friendly guide to the Leave Management application: what it does, how the .NET MAUI app works, and how the mobile client communicates with the backend.

## 1. What this project does

This application helps an organization manage employee leave. Users sign in with an account and get screens based on their role:

- **Employee** requests leave, views their leave balance and tracks their own requests.
- **Manager** manages direct reports, reviews their requests and approves or rejects pending leave.
- **Admin** manages users and leave types, views dashboard statistics and views their own profile.

The solution contains two applications:

| Project | Technology | Purpose |
|---|---|---|
| `LeaveManagement.Mobile` | .NET MAUI, targeting .NET 10 | Cross-platform user interface for Android and other configured MAUI targets. |
| `LeaveManagement` | ASP.NET Core Web API, targeting .NET 8 | Authenticates users, applies access rules and reads/writes application data. |

The mobile app is the **client**: it displays screens and sends requests. The backend is the **server**: it checks permissions and performs the operations. Hiding a button in the app is not a security boundary; important access checks are also done by the API.

## 2. Important terms

- **API**: A set of HTTP routes that lets one application request data or actions from another.
- **Endpoint**: One API route, such as `GET api/employee/me`.
- **HTTP method**: The kind of operation: `GET` reads, `POST` creates/submits, `PUT` updates/acts, and `DELETE` removes or deactivates.
- **DTO**: A data-transfer object; a C# class used to send data between the app and API.
- **Entity**: A C# class that represents stored data, such as an employee or leave request.
- **JWT / bearer token**: A signed token issued at login. The mobile app sends it with protected API requests so the server can identify the user and role.
- **Direct report**: An employee whose `ManagerId` points to a particular manager's employee ID.

## 3. How the MAUI app starts and signs in

1. `MauiProgram.cs` configures the MAUI application, dependency injection, fonts, and HTTP clients.
2. `App.xaml.cs` opens the login page.
3. `LoginPage.xaml` displays the sign-in form. `LoginViewModel.cs` handles the command and busy/error state.
4. `AuthService.LoginAsync` sends the username and password to `api/login/login`.
5. The backend verifies the account and returns a JWT with the user's employee ID, username, and role.
6. `SessionService.cs` stores the token and basic user details using MAUI Secure Storage, with Preferences as a fallback.
7. `RoleShellFactory.cs` creates navigation tabs for the returned role.

For authenticated requests, `AuthHeaderHandler.cs` attaches the token as an `Authorization: Bearer ...` header. If the server responds with `401 Unauthorized`, it clears the local session and returns the user to login.

### Where the main MAUI code lives

- `MauiProgram.cs`: app startup and dependency registration.
- `App.xaml.cs`: switches between login and role navigation.
- `Views/LoginPage.xaml`: login screen layout.
- `ViewModels/LoginViewModel.cs`: login screen behavior.
- `Helpers/RoleShellFactory.cs`: tabs selected for each role.
- `Views/PlaceholderPage.cs`: current dynamic implementation for dashboard, profile, employee, leave, and admin screens.
- `Services/AuthService.cs`: login and API methods used by the app.
- `Services/SessionService.cs`: signed-in user's token and identity.
- `Models/ApiModels.cs`: client DTOs used to parse API responses and submit requests.
- `Helpers/Constants.cs`: API base URL and login route.

Most operational screens are currently assembled in code by `PlaceholderPage`, rather than having a separate XAML file for each screen. `LoginPage` is a XAML screen and uses a view model; the entire app does not use a single uniform MVVM pattern yet.

## 4. What each role can do

### Employee

The Employee tabs are currently:

- **Profile**: view account details; Employee can also open the profile update form.
- **Leave Balance**: see available and used days for leave types.
- **Apply Leave**: choose an active leave type, dates, and a reason, then submit a request.
- **My Leaves**: see the Employee's own requests, including their reason and status.
- **Leave Types**: view leave types and their availability information.

When a request is submitted, the server checks that the authenticated Employee is applying for themselves, that the leave type and account are active, that dates are valid, that the reason is present, that the request does not overlap another non-rejected request, and that enough balance remains.

### Manager

The Manager tabs are currently:

- **Dashboard**: counts for direct-report employees and their leave requests; approved/rejected counts are attributed to the signed-in Manager.
- **Profile**: view account details and edit the Manager's profile.
- **Employees**: view and manage employees under this Manager, create an employee, update employee details, and change employee status.
- **Approvals**: view pending leave requests in the Manager's scope and approve or reject them with optional comments.
- **Leave Types**: view available leave types.

The API derives the Manager's identity from the JWT. Employee listings and leave listings are scoped to direct reports. Approval and rejection are also checked by the backend so a Manager cannot act on a leave request belonging to another Manager's employee.

### Admin

The Admin tabs are currently:

- **Dashboard**: application-wide summary statistics.
- **Profile**: view the signed-in Admin's own details. The Admin profile screen does not show the Employee/Manager profile update control.
- **Users**: create users, view all users, update other users, change other users' active status, and manage Employee/Manager roles. The signed-in Admin's own user record is excluded from update and activate/deactivate controls; the backend also rejects attempts to update or change the status of the current Admin account.
- **Leave Types**: create, view, update, activate, and deactivate leave types.

Role changes are distinct from editing profile fields or active status. An Admin can convert an Employee to Manager or a Manager to Employee, with the required manager relationship handled by the API.

## 5. Leave request lifecycle

A typical request moves through these steps:

1. An Employee selects an active leave type and submits dates and a reason.
2. The API validates the request and stores it with `Pending` status.
3. The Employee can see it under **My Leaves**.
4. The Employee's Manager can see it in **Approvals**.
5. The Manager approves or rejects the pending request and may provide comments.
6. On approval, the API checks the employee's leave balance and updates the used days. On rejection, the request becomes rejected and its balance is not consumed.

The main leave request fields are defined by `Models/Entities/Leaverequest.cs` and include employee ID, leave type ID, start/end dates, reason, status, application date, approver, and comments.

## 6. Backend structure

The backend follows a common controller/service/database pattern:

1. **Controller** receives an HTTP request, checks role/identity, and returns an HTTP response.
2. **Service** contains application rules and queries.
3. **`AppDbContext`** maps Entity Framework Core entities to the SQL Server database.
4. **Database** stores login accounts, employees, leave types, leave requests, and balances.

Key backend folders/files:

- `Controllers/`: API routes, such as `EmployeeController`, `LeavesController`, `AdminController`, `DashboardController`, and `LeavetypesController`.
- `Services/`: business operations, such as `EmployeeService`, `LeaveService`, `AdminService`, `DashboardService`, and `LeavetypeService`.
- `DTO/`: request and response data shapes.
- `Models/Entities/`: database entities.
- `Data/AppDbContext.cs`: Entity Framework Core configuration and relationships.
- `Migrations/`: versioned database schema changes.
- `Program.cs`: backend configuration, dependency registration, SQL Server, authentication, and authorization.

### Important data relationships

- Each login account is linked to an employee record.
- An Employee has an optional `ManagerId`; a Manager's direct reports reference that Manager's employee ID.
- A leave request references one employee and one leave type.
- A leave balance tracks an employee's used and allowed days for a leave type.
- A newly created leave type receives a balance record for existing employees.

## 7. API routes used by the app

These are the main routes visible in the current client and backend. Authorization is enforced by backend role attributes and additional ownership checks.

| Purpose | Route | Typical permitted role(s) |
|---|---|---|
| Sign in | `POST api/login/login` | Unauthenticated request |
| Dashboard summary | `GET api/dashboard/leave-summary` | Manager, Admin |
| Current user's details | `GET api/employee/me` | Employee, Manager, Admin |
| Manager's employee list | `GET api/employee` | Manager; results are direct reports |
| Employee details by ID | `GET api/employee/getbyid/{id}` | Manager (direct report only), Employee (self only) |
| Employee leave balance | `GET api/employees/{employeeId}/leave-balance` | See backend authorization rules |
| Apply for leave | `POST api/leaves` | Employee (self only) |
| List leave requests | `GET api/leaves` | Employee (own requests), Manager (direct reports) |
| Get one leave request | `GET api/leaves/{id}` | Employee/Manager with ownership scope |
| Approve/reject leave | `PUT api/leaves/{id}/approve`, `PUT api/leaves/{id}/reject` | Manager (direct-report request only) |
| List leave types | `GET api/leavetypes/getall` | Employee, Manager, Admin |
| Admin user management | `api/admin/...` | Admin |
| Create/update leave types | `POST api/leavetypes/create-leavetype`, `PUT api/leavetypes/update-leavetype/{id}` | Admin and Manager in current API attributes |

For a definitive route/permission check, read the corresponding controller attribute and method body. Permissions can evolve as the project changes.

## 8. Duplicate and similar leave type names

`Services/LeavetypeService.cs` checks leave type names when creating or updating them. It uses a regular expression to split the name into words, ignores generic words such as **leave**, **leaves**, and **next**, and compares the remaining words without case sensitivity. If it finds a shared meaningful word, the API rejects the operation and identifies the existing leave type. For example, **Sick Leave** and **Next Sick** are treated as similar because both contain the meaningful word `Sick`.

This is a simple similarity rule, not a full spell-checker or semantic language model. It may also flag names that share a meaningful word even when an administrator intended them to be different.

## 9. Running and debugging locally

### Backend

1. Open `LeaveManagement.slnx` in Visual Studio, or open a PowerShell terminal in the solution folder.
2. Configure the SQL Server connection string and JWT issuer, audience, and signing key using the local development configuration expected by `Program.cs`. Do not commit real passwords or signing keys.
3. Ensure the database is available and migrations have been applied.
4. Start the `LeaveManagement` API project. Its launch profiles are in `Properties/launchSettings.json`.
5. Check the API URL and Swagger page if Swagger is enabled by the development configuration.

A typical command is:

```powershell
dotnet run --project .\LeaveManagement.csproj
```

### MAUI app

1. Set the API base address in `LeaveManagement.Mobile/Helpers/Constants.cs` to the URL reachable from the selected emulator/device.
2. Start the API first.
3. Select an available MAUI target in Visual Studio and run the mobile project.
4. Sign in with an account configured in the database and use the tabs for that account's role.

A typical Android build command is:

```powershell
dotnet build ..\LeaveManagement.Mobile\LeaveManagement.Mobile.csproj -f net10.0-android
```

**Localhost note:** `https://localhost:7076` is the current client base URL in `Constants.cs`. On an Android emulator or physical phone, `localhost` usually means the device itself, not the development computer. Use the emulator's host-machine address or your computer's reachable network address and configure HTTPS/certificates appropriately for development.

## 10. A simple debugging checklist

When something does not work:

1. Confirm the API is running and the mobile app points to the correct base URL.
2. Check the HTTP response and backend logs. `400` usually means invalid input/business-rule failure; `401` means missing/invalid login; `403` means authenticated but not allowed; `404` means the requested record/filter was not found.
3. Check that the signed-in account is active and has the expected role.
4. For Manager access, verify the employee's `ManagerId` matches the Manager's employee ID.
5. For leave approval, verify that the request is pending and belongs to a direct report.
6. Check the API DTO and the mobile model for matching JSON property names and types.
7. Rebuild both projects after code changes. If Visual Studio says a file is locked or the app is running under the debugger, stop debugging and build again.

## 11. Suggestions for a beginner exploring the code

A good order for reading the project is:

1. `RoleShellFactory.cs` — see how role tabs are selected.
2. `PlaceholderPage.cs` — follow a screen branch such as `"My Leaves"` or `"Approvals"`.
3. `AuthService.cs` — find the API method called by that screen.
4. The matching controller — see authorization and request handling.
5. The matching service — find validation and database queries.
6. The DTO/entity and `AppDbContext.cs` — understand the data shape and relationships.

For example, to follow leave application: look for the **Apply Leave** UI in `PlaceholderPage.cs`, then `ApplyLeaveAsync` in `AuthService.cs`, `ApplyLeave` in `LeavesController.cs`, `ApplyLeaveAsync` in `LeaveService.cs`, and finally `Leaverequest` / `Leavebalance` entities.
