namespace LeaveManagementSystem.Swagger;

public static class SwaggerRoleConfiguration
{
    // =========================================================
    // ADMIN ONLY APIs
    // =========================================================
    public static readonly HashSet<string> AdminOnlyRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Admin - User Management
            "POST:/api/admin/employees",
            "POST:/api/admin/managers",
            "GET:/api/admin/users",

            // Change User Role
            "PUT:/api/admin/users/{employeeId}/change-to-manager",
            "PUT:/api/admin/users/{employeeId}/change-to-employee",

            // Activate / Deactivate User
            "PUT:/api/admin/users/{employeeId}/status"
        };


    // =========================================================
    // MANAGER ONLY APIs
    // =========================================================
    public static readonly HashSet<string> ManagerOnlyRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Dashboard
            "GET:/api/dashboard/leave-summary",

            // Employees
            "GET:/api/employee",
            "DELETE:/api/employee/{id}",

            // Leave Types
            "POST:/api/leavetypes",
            "PUT:/api/leavetypes/{id}",
            "DELETE:/api/leavetypes/{id}",

            // Leaves
            "GET:/api/leaves",
            "PUT:/api/leaves/{id}/approve",
            "PUT:/api/leaves/{id}/reject"
        };


    // =========================================================
    // EMPLOYEE ONLY APIs
    // =========================================================
    public static readonly HashSet<string> EmployeeOnlyRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Employee's own details
            "GET:/api/employee/me",

            // Apply leave
            "POST:/api/leaves"
        };


    // =========================================================
    // SHARED APIs
    // =========================================================
    public static readonly HashSet<string> SharedRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Employee
            "GET:/api/employee/{id}",
            "PUT:/api/employee/{id}",

            // Leave Types
            "GET:/api/leavetypes",
            "GET:/api/leavetypes/{id}",

            // Leave Balance
            "GET:/api/employees/{employeeId}/leave-balance",

            // Leaves
            "GET:/api/leaves/{id}",
            "GET:/api/employees/{employeeId}/leaves"
        };


    // =========================================================
    // PUBLIC APIs
    // =========================================================
    public static readonly HashSet<string> PublicRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Login does not require JWT
            "POST:/api/login/login"
        };
}