(function () {
    "use strict";

    console.log("Swagger role filter loaded.");


    // =========================================================
    // PUBLIC APIs
    // =========================================================
    const publicRoutes = new Set([
        "POST:/api/login/login",
        "POST:/api/login/register"
    ]);


    // =========================================================
    // MANAGER ONLY APIs
    // =========================================================
    const managerOnlyRoutes = new Set([
        "GET:/api/dashboard/leave-summary",

        "GET:/api/employee",
        "DELETE:/api/employee/{id}",

        "GET:/api/leaves",
        "PUT:/api/leaves/{id}/approve",
        "PUT:/api/leaves/{id}/reject",

        "POST:/api/leavetypes",
        "PUT:/api/leavetypes/{id}",
        "DELETE:/api/leavetypes/{id}"
    ]);


    // =========================================================
    // EMPLOYEE ONLY APIs
    // =========================================================
    const employeeOnlyRoutes = new Set([
        "GET:/api/employee/me",
        "POST:/api/leaves"
    ]);


    // =========================================================
    // SHARED APIs
    // =========================================================
    const sharedRoutes = new Set([
        "GET:/api/employee/{id}",
        "PUT:/api/employee/{id}",

        "GET:/api/leavetypes",
        "GET:/api/leavetypes/{id}",

        "GET:/api/employees/{employeeid}/leave-balance",

        "GET:/api/leaves/{id}",
        "GET:/api/employees/{employeeid}/leaves"
    ]);


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

        /*
         * Remove trailing slash
         */
        path = path.replace(/\/+$/, "");

        /*
         * Convert route parameters to lowercase
         *
         * {id}          -> {id}
         * {employeeId}  -> {employeeid}
         */
        path = path.replace(
            /\{([^}]+)\}/g,
            function (_, parameter) {
                return "{" + parameter.toLowerCase() + "}";
            }
        );

        /*
         * Route itself is case-insensitive
         */
        path = path.toLowerCase();

        return method + ":" + path;
    }


    // =========================================================
    // DECODE JWT
    // =========================================================
    function decodeJwt(token) {

        try {

            const parts = token.split(".");

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

            return JSON.parse(atob(payload));

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

        const payload = decodeJwt(token);

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

        if (Array.isArray(role)) {
            role = role[0];
        }

        if (!role) {
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
                    publicRoutes.has(key)
                ) {

                    operation.style.display =
                        "";

                }
                else {

                    operation.style.display =
                        "none";
                }

            });

        removeEmptyTags();
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
                "No JWT -> public APIs only"
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
                "Role not found -> protected APIs hidden"
            );

            hideAllProtectedOperations();

            return;
        }


        // =====================================================
        // PROCESS APIs
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
            // PUBLIC
            // =================================================
            if (
                publicRoutes.has(key)
            ) {

                operation.style.display =
                    "";

                return;
            }


            // =================================================
            // MANAGER ONLY
            // =================================================
            if (
                managerOnlyRoutes.has(key)
            ) {

                if (
                    role === "manager"
                ) {

                    operation.style.display =
                        "";

                }
                else {

                    operation.style.display =
                        "none";
                }

                return;
            }


            // =================================================
            // EMPLOYEE ONLY
            // =================================================
            if (
                employeeOnlyRoutes.has(key)
            ) {

                if (
                    role === "employee"
                ) {

                    operation.style.display =
                        "";

                }
                else {

                    operation.style.display =
                        "none";
                }

                return;
            }


            // =================================================
            // SHARED
            // =================================================
            if (
                sharedRoutes.has(key)
            ) {

                /*
                 * Both Manager and Employee
                 * can see shared APIs.
                 */
                if (
                    role === "manager" ||
                    role === "employee"
                ) {

                    operation.style.display =
                        "";

                }
                else {

                    operation.style.display =
                        "none";
                }

                return;
            }


            // =================================================
            // UNKNOWN
            // =================================================
            console.warn(
                "Unknown API hidden:",
                key
            );

            operation.style.display =
                "none";

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

                    tag.style.display =
                        "none";

                }
                else {

                    tag.style.display =
                        "";
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
    // CHECK EVERY SECOND
    // =========================================================
    setInterval(
        updateVisibility,
        1000
    );

})();