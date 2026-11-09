# MAUI Backend API Screens - Change Reference

## Purpose
This document summarizes the changes made to connect the **LeaveManagement.Mobile** app to backend controller APIs and render role-based screens.

## Scope
Project updated:
- `..\LeaveManagement.Mobile`

Backend referenced:
- `LeaveManagement` controllers under `Controllers/*`

## Files Changed
- `..\LeaveManagement.Mobile\Models\ApiModels.cs`
- `..\LeaveManagement.Mobile\Services\AuthService.cs`
- `..\LeaveManagement.Mobile\Helpers\RoleShellFactory.cs`
- `..\LeaveManagement.Mobile\Views\PlaceholderPage.cs`
- `..\LeaveManagement.Mobile\Views\LoginPage.xaml.cs`

---

## 1) New API Models
### File
- `..\LeaveManagement.Mobile\Models\ApiModels.cs`

### Added DTOs / request models
- `DashboardSummaryDto`
- `EmployeeDto`
- `CreateUserRequest`
- `ChangeStatusRequest`
- `LeaveTypeDto`
- `CreateLeaveTypeRequest`
- `UpdateLeaveTypeRequest`
- `LeaveBalanceDto`
- `LeaveRequestDto`
- `ApplyLeaveRequest`
- `LeaveActionRequest`
- `ApiEnvelope<T>`
- `ApiResult`
- `ApiResult<T>`

Purpose: strongly typed request/response handling for mobile-side API calls.

---

## 2) API Client Added
### File
- `..\LeaveManagement.Mobile\Services\AuthService.cs`

### What changed
- Existing `AuthService` retained for login/logout.
- Added `ApiService` in the same file for backend CRUD and workflow calls.
- `ApiService` supports both constructor forms:
  - `ApiService(HttpClient http)`
  - `ApiService(HttpClient http, SessionService session)`
- Adds bearer token header when session token is available.

### Endpoints wrapped
- Dashboard:
  - `GET api/dashboard/leave-summary`
- Employee:
  - `GET api/employee`
  - `GET api/employee/me`
  - `GET api/employee/getbyid/{id}`
  - `POST api/employee/createemployee`
  - `PUT api/employee/updateemployee/{id}`
  - `DELETE api/employee/{id}`
- Leave Types:
  - `GET api/leavetypes/getall`
  - `POST api/leavetypes/create-leavetype`
  - `PUT api/leavetypes/update-leavetype/{id}`
  - `DELETE api/leavetypes/delete-leavetype/{id}`
- Leave Balance:
  - `GET api/employees/{employeeId}/leave-balance`
- Leaves:
  - `GET api/leaves`
  - `GET api/leaves?employeeid={id}`
  - `GET api/leaves/{id}`
  - `GET api/employees/{employeeId}/leaves`
  - `POST api/leaves`
  - `PUT api/leaves/{id}/approve`
  - `PUT api/leaves/{id}/reject`
- Admin:
  - `GET api/admin/users`
  - `POST api/admin/employees`
  - `POST api/admin/managers`
  - `PUT api/admin/users/{id}/status?employeeId={id}`
  - `PUT api/admin/users/{id}/change-to-manager?employeeId={id}`
  - `GET api/admin/employees/{id}?employeeId={id}`
  - `PUT api/admin/employees/{id}?employeeId={id}`

---

## 3) Role-based Shell Tabs Updated
### File
- `..\LeaveManagement.Mobile\Helpers\RoleShellFactory.cs`

### Tab mapping now
- **Admin**: `Dashboard`, `Users`, `Employees`, `Leave Types`, `Leaves`
- **Manager**: `Dashboard`, `Employees`, `Approvals`, `Leave Types`, `Leaves`
- **Employee**: `Profile`, `Leave Balance`, `Leave Types`,`Apply Leave`, `My Leaves`

---

## 4) PlaceholderPage Converted to Data-backed Screens
### File
- `..\LeaveManagement.Mobile\Views\PlaceholderPage.cs`

### What now works
- Global actions:
  - Refresh current screen
  - Sign out
- Dashboard screen:
  - Loads and displays dashboard summary metrics
- Employees screen:
  - Loads employee list
- Users (Admin) screen:
  - Loads all users
  - Renders admin actions:
	- Create employee
	- Create manager
	- Change user role to manager
	- Activate / deactivate user
- Profile screen:
  - Loads logged-in user details
- Leave Types screen:
  - Lists leave types
- Leave Balance screen:
  - Loads leave balance for logged-in employee
- My Leaves / Leaves screens:
  - Displays leave request history
- Approvals screen:
  - Filters pending requests
  - Approve / reject actions with optional comments
- Apply Leave screen:
  - Form for leave type id, date range, reason
  - Submits leave request to backend

### Compile fix included
- Nullable date handling corrected:
  - `joinDate.Date ?? DateTime.Today`
  - `fromPicker.Date ?? DateTime.Today`
  - `toPicker.Date ?? DateTime.Today`

---

## 5) Login Page Initialization Fix
### File
- `..\LeaveManagement.Mobile\Views\LoginPage.xaml.cs`

### Change
- Added `InitializeComponent()` in constructor.

Why: ensures XAML content is initialized before binding and session restore logic runs.

---

## Validation Completed
- Build status after fixes: **successful**.

---

## Notes
- Most screens are currently implemented inside `PlaceholderPage` as a consolidated API-backed host.
- If you want cleaner long-term structure, next step is to split each tab into dedicated `Page + ViewModel` files while reusing `ApiService` and `ApiModels`.
