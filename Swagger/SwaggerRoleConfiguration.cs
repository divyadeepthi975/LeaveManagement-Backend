namespace LeaveManagementSystem.Swagger;

public static class SwaggerRoleConfiguration
{
    // Manager can access these APIs only
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


    // Employee can access these APIs only
    public static readonly HashSet<string> EmployeeOnlyRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Employee's own details
            "GET:/api/employee/me",

            // Apply leave
            "POST:/api/leaves"
        };


    // Both Manager and Employee can access these APIs
    // Employee is restricted to their own data by controller logic.
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


    // No JWT required
    public static readonly HashSet<string> PublicRoutes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "POST:/api/auth/login"
        };
}