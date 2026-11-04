# Mobile Login Connection Fix (Reference)

## Summary
This document captures the exact change made to fix the mobile login error:

- **Exception:** `System.Net.Http.HttpRequestException`
- **Inner Exception:** `System.Net.Sockets.SocketException`
- **Message:** `No connection could be made because the target machine actively refused it. (localhost:7123)`

## Root Cause
The mobile app was configured to call the API at:

- `https://localhost:7123`

But the backend API launch profile is configured to run on:

- `https://localhost:7076` (HTTPS)
- `http://localhost:5018` (HTTP)

Because nothing was listening on port `7123`, the socket connection was refused.

## Change Made
### File updated
- `LeaveManagement.Mobile/Helpers/Constants.cs`

### Before
```csharp
public const string BaseUrl = "https://localhost:7123";
```

### After
```csharp
public const string BaseUrl = "https://localhost:7076";
```

## Why this fixes the issue
The mobile app now targets the same HTTPS port as the backend API (`7076`), so the login request can reach a listening server endpoint.

## Validation Steps
1. Start backend API using the **https** launch profile.
2. Confirm API is reachable at `https://localhost:7076/swagger`.
3. Run `LeaveManagement.Mobile` and attempt login.
4. Verify the previous connection-refused exception no longer occurs.

## Notes
- `LoginPath` remains unchanged: `api/login/login`.
- If testing on Android/iOS emulator or physical device, `localhost` may not refer to the development machine. In that case, use a platform-specific host/IP mapping.
