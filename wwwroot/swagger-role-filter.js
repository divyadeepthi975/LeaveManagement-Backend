
(function () {
    "use strict";

    console.log("Swagger role filter loaded.");

    // =========================================================
    // NORMALIZE ROUTE
    // =========================================================
    function normalizeRoute(method, path) {

        method = method
            .trim()
            .toUpperCase();

        path = path
            .trim()
            .replace(/\s+/g, "");

        // Remove trailing slash
        path = path.replace(/\/+$/, "");

        // Normalize route parameters
        path = path.replace(
            /\{([^}]+)\}/g,
            function (_, parameter) {
                return "{" + parameter.toLowerCase() + "}";
            }
        );

        // Route matching is case-insensitive
        path = path.toLowerCase();

        return method + ":" + path;
    }


    // =========================================================
    // NORMALIZE ROUTE KEY
    // =========================================================
    function normalizeRouteKey(route) {

        const separatorIndex = route.indexOf(":");

        if (separatorIndex === -1) {
            return route.trim().toLowerCase();
        }

        const method =
            route.substring(0, separatorIndex);

        const path =
            route.substring(separatorIndex + 1);

        return normalizeRoute(method, path);
    }


    // =========================================================
    // PUBLIC ROUTES
    // =========================================================
    const publicRoutes = new Set([
        "POST:/api/login/login"
    ]);


    // =========================================================
    // ROLE BASED API ACCESS
    //
    // Every API is defined ONCE.
    //
    // This prevents clashes such as:
    // Admin + Manager
    // Manager + Employee
    // Admin + Manager + Employee
    // =========================================================
    const routeAccess = {

        // =====================================================
        // ADMIN CONTROLLER
        // =====================================================

        "POST:/api/admin/employees": [
            "admin"
        ],

        "POST:/api/admin/managers": [
            "admin"
        ],

        "GET:/api/admin/users": [
            "admin"
        ],

        "PUT:/api/admin/users/{employeeid}/change-to-manager": [
            "admin"
        ],

        "PUT:/api/admin/users/{employeeid}/change-to-employee": [
            "admin"
        ],

        "PUT:/api/admin/users/{employeeid}/status": [
            "admin"
        ],
        
        " GET :/ api / admin / employees / { employeeId }":[
            "admin"
       ],
        "PUT :/ api / admin / employees / { employeeId }":[
            "admin"
    ],

        // =====================================================
        // DASHBOARD
        // Manager + Admin
        // =====================================================

        "GET:/api/dashboard/leave-summary": [
            "admin",
            "manager"
        ],


        // =====================================================
        // EMPLOYEE CONTROLLER
        // =====================================================

        // Manager only
        "POST:/api/employee/createemployee": [
            "manager"
        ],

        // Manager only
        "GET:/api/employee": [
            "manager"
        ],

        // Manager 
        "GET:/api/employee/getbyid/{id}": [
            "manager"
        ],

        // Admin + Manager + Employee
        "GET:/api/employee/me": [
            "admin",
            "manager",
            "employee"
        ],

        // Manager + Employee
        "PUT:/api/employee/updateemployee/{id}": [
            "manager",
            "employee"
        ],

        // Manager only
        "DELETE:/api/employee/{id}": [
            "manager"
        ],


        // =====================================================
        // LEAVE BALANCE
        // Manager + Employee
        // =====================================================

        "GET:/api/employees/{employeeid}/leave-balance": [
            "manager",
            "employee"
        ],


        // =====================================================
        // LEAVES
        // =====================================================

        // Employee only
        "POST:/api/leaves": [
            "employee"
        ],

        // Manager + Employee
        "GET:/api/leaves": [
            "manager",
            "employee"
        ],

        // Manager + Employee
        "GET:/api/leaves/{id}": [
            "manager",
            "employee"
        ],

        // Manager + Employee
        "GET:/api/employees/{employeeid}/leaves": [
            "manager",
            "employee"
        ],

        // Manager only
        "PUT:/api/leaves/{id}/approve": [
            "manager"
        ],

        // Manager only
        "PUT:/api/leaves/{id}/reject": [
            "manager"
        ],


        // =====================================================
        // LEAVE TYPES
        // =====================================================

        // Admin + Manager + Employee
        "GET:/api/leavetypes/getall": [
            "admin",
            "manager",
            "employee"
        ],

        // Admin + Manager + Employee
        "GET:/api/leavetypes/get-by-id/{id}": [
            "admin",
            "manager",
            "employee"
        ],

        // Admin + Manager
        "POST:/api/leavetypes/create-leavetype": [
            "admin",
            "manager"
        ],

        // Admin + Manager
        "PUT:/api/leavetypes/update-leavetype/{id}": [
            "admin",
            "manager"
        ],

        // Admin + Manager
        "DELETE:/api/leavetypes/delete-leavetype/{id}": [
            "admin",
            "manager"
        ]
    };


    // =========================================================
    // NORMALIZE PUBLIC ROUTES
    // =========================================================
    const normalizedPublicRoutes =
        new Set(
            [...publicRoutes].map(normalizeRouteKey)
        );


    // =========================================================
    // NORMALIZE ROLE ACCESS MAP
    // =========================================================
    const normalizedRouteAccess = {};

    Object.keys(routeAccess).forEach(route => {

        const normalizedRoute =
            normalizeRouteKey(route);

        normalizedRouteAccess[normalizedRoute] =
            routeAccess[route].map(role =>
                role.toLowerCase()
            );
    });


    // =========================================================
    // DECODE JWT
    // =========================================================
    function decodeJwt(token) {

        try {

            const parts =
                token.split(".");

            if (parts.length !== 3) {
                return null;
            }

            let payload = parts[1];

            payload = payload
                .replace(/-/g, "+")
                .replace(/_/g, "/");

            while (payload.length % 4 !== 0) {
                payload += "=";
            }

            return JSON.parse(
                atob(payload)
            );

        }
        catch (error) {

            console.error(
                "JWT decode failed:",
                error
            );

            return null;
        }
    }


    // =========================================================
    // GET ROLE FROM JWT
    // =========================================================
    function getRoleFromJwt(token) {

        const payload =
            decodeJwt(token);

        if (!payload) {
            return null;
        }

        console.log(
            "JWT Payload:",
            payload
        );


        let role =
            payload.role ||
            payload.Role ||
            payload.roles ||
            payload.Roles ||
            payload[
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            ];


        // Handle array of roles
        if (Array.isArray(role)) {

            role = role[0];
        }


        if (!role) {

            console.warn(
                "No role found in JWT."
            );

            return null;
        }


        return String(role)
            .trim()
            .toLowerCase();
    }


    // =========================================================
    // GET JWT FROM SWAGGER
    // =========================================================
    function getCurrentJwt() {

        try {

            if (!window.ui) {
                return null;
            }


            const state =
                window.ui.getState();

            const auth =
                state.get("auth");


            if (!auth) {
                return null;
            }


            const authorized =
                auth.get("authorized");


            if (
                !authorized ||
                authorized.size === 0
            ) {

                return null;
            }


            const authorizedObject =
                authorized.toJS();


            console.log(
                "Swagger authorized object:",
                authorizedObject
            );


            const bearer =
                authorizedObject.Bearer ||
                authorizedObject.bearer;


            if (!bearer) {
                return null;
            }


            let token = null;


            if (typeof bearer === "string") {

                token = bearer;

            }
            else if (bearer.value) {

                token = bearer.value;

            }
            else if (bearer.schema) {

                token = bearer.schema;
            }


            if (!token) {
                return null;
            }


            token = token
                .replace(/^Bearer\s+/i, "")
                .trim();


            return token;

        }
        catch (error) {

            console.error(
                "Unable to get JWT:",
                error
            );

            return null;
        }
    }


    // =========================================================
    // GET OPERATION KEY
    // =========================================================
    function getOperationKey(operation) {

        const pathElement =
            operation.querySelector(
                ".opblock-summary-path"
            );

        const methodElement =
            operation.querySelector(
                ".opblock-summary-method"
            );


        if (
            !pathElement ||
            !methodElement
        ) {

            return null;
        }


        const method =
            methodElement.textContent;

        const path =
            pathElement.textContent;


        return normalizeRoute(
            method,
            path
        );
    }


    // =========================================================
    // HIDE ALL PROTECTED APIs
    // =========================================================
    function hideAllProtectedOperations() {

        document
            .querySelectorAll(".opblock")
            .forEach(operation => {

                const key =
                    getOperationKey(operation);


                if (!key) {
                    return;
                }


                if (
                    normalizedPublicRoutes.has(key)
                ) {

                    operation.style.display = "";

                }
                else {

                    operation.style.display = "none";
                }

            });


        removeEmptyTags();
    }


    // =========================================================
    // CHECK WHETHER ROLE CAN ACCESS ROUTE
    // =========================================================
    function canRoleAccessRoute(
        role,
        routeKey
    ) {

        const allowedRoles =
            normalizedRouteAccess[routeKey];


        if (!allowedRoles) {

            return false;
        }


        return allowedRoles.includes(role);
    }


    // =========================================================
    // UPDATE VISIBILITY
    // =========================================================
    function updateVisibility() {

        const operations =
            document.querySelectorAll(
                ".opblock"
            );


        if (operations.length === 0) {
            return;
        }


        // =====================================================
        // GET JWT
        // =====================================================
        const token =
            getCurrentJwt();


        // =====================================================
        // NO JWT
        // =====================================================
        if (!token) {

            console.log(
                "No JWT -> showing public APIs only."
            );


            hideAllProtectedOperations();

            return;
        }


        // =====================================================
        // GET ROLE
        // =====================================================
        const role =
            getRoleFromJwt(token);


        console.log(
            "Swagger detected role:",
            role
        );


        // =====================================================
        // ROLE NOT FOUND
        // =====================================================
        if (!role) {

            console.warn(
                "Role not found -> protected APIs hidden."
            );


            hideAllProtectedOperations();

            return;
        }


        // =====================================================
        // PROCESS EVERY API
        // =====================================================
        operations.forEach(operation => {

            const key =
                getOperationKey(operation);


            if (!key) {
                return;
            }


            console.log(
                "Swagger API:",
                key
            );


            // =================================================
            // PUBLIC API
            // =================================================
            if (
                normalizedPublicRoutes.has(key)
            ) {

                operation.style.display = "";

                return;
            }


            // =================================================
            // PROTECTED API
            // =================================================
            if (
                canRoleAccessRoute(
                    role,
                    key
                )
            ) {

                console.log(
                    "SHOW:",
                    key,
                    "for role:",
                    role
                );


                operation.style.display = "";

                return;
            }


            // =================================================
            // UNKNOWN OR UNAUTHORIZED API
            // =================================================
            console.log(
                "HIDE:",
                key,
                "for role:",
                role
            );


            operation.style.display = "none";

        });


        removeEmptyTags();
    }


    // =========================================================
    // REMOVE EMPTY TAGS
    // =========================================================
    function removeEmptyTags() {

        document
            .querySelectorAll(
                ".opblock-tag"
            )
            .forEach(tag => {

                const operations =
                    tag.querySelectorAll(
                        ".opblock"
                    );


                if (
                    operations.length === 0
                ) {

                    return;
                }


                const visibleOperations =
                    Array.from(
                        operations
                    ).filter(
                        operation =>
                            operation.style.display !== "none"
                    );


                if (
                    visibleOperations.length === 0
                ) {

                    tag.style.display = "none";

                }
                else {

                    tag.style.display = "";
                }

            });
    }


    // =========================================================
    // MUTATION OBSERVER
    // =========================================================
    let updating = false;


    const observer =
        new MutationObserver(() => {

            if (updating) {
                return;
            }


            updating = true;


            setTimeout(() => {

                updateVisibility();

                updating = false;

            }, 100);

        });


    // =========================================================
    // START OBSERVER
    // =========================================================
    function startObserver() {

        if (!document.body) {
            return;
        }


        observer.observe(
            document.body,
            {
                childList: true,
                subtree: true
            }
        );


        updateVisibility();
    }


    // =========================================================
    // START
    // =========================================================
    if (
        document.readyState ===
        "loading"
    ) {

        document.addEventListener(
            "DOMContentLoaded",
            startObserver
        );

    }
    else {

        startObserver();
    }


    // =========================================================
    // CHECK PERIODICALLY
    // =========================================================
    setInterval(
        updateVisibility,
        1000
    );

})();

