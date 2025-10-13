# Visual Studio 2022 Fix Guide

## Problem
- ✅ **Console**: `dotnet run` works perfectly
- ❌ **Visual Studio**: F5/Ctrl+F5 doesn't work

## Solutions Applied

### 1. Fixed Launch Settings
Updated `Properties/launchSettings.json`:
- ✅ Added `"launchBrowser": true` to enable auto-launch
- ✅ Added IIS Express profile for Visual Studio compatibility
- ✅ Added proper IIS settings

### 2. Fixed HTTPS Redirection
Updated `Program.cs`:
- ✅ Disabled HTTPS redirection in development
- ✅ Only redirects HTTPS in production

### 3. Added Better Logging
- ✅ Added logging to controller to help debug issues
- ✅ Shows when endpoint is called and with what SQL

## How to Run from Visual Studio

### Option 1: Use HTTP Profile
1. In Visual Studio, click the **dropdown arrow** next to the Run button
2. Select **"http"** profile (not https)
3. Press **F5** or **Ctrl+F5**

### Option 2: Use IIS Express
1. In Visual Studio, click the **dropdown arrow** next to the Run button  
2. Select **"IIS Express"** profile
3. Press **F5** or **Ctrl+F5**

### Option 3: Manual URL
If Visual Studio opens the wrong URL:
1. Look at the console output for the correct URL
2. Manually navigate to `http://localhost:5163`
3. The Swagger UI should be available

## Testing the Endpoint

Once running from Visual Studio, test with:

**PowerShell:**
```powershell
$body = '"SELECT 1 as Test"'
Invoke-RestMethod -Uri 'http://localhost:5163/api/simplequery/execute' -Method POST -ContentType 'application/json' -Body $body
```

**curl (in Git Bash or WSL):**
```bash
curl -X 'POST' 'http://localhost:5163/api/simplequery/execute' -H 'Content-Type: application/json' -d '"SELECT 1 as Test"'
```

## Debugging Tips

### Check Console Output
Look for these lines in Visual Studio Output window:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5163
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Check Logs
The controller now logs when called:
```
info: QuerySheper.Controllers.SimpleQueryController[0]
      ExecuteQuery endpoint called with SQL: SELECT 1 as Test
```

### Common Issues

1. **Wrong Profile**: Make sure you're using "http" or "IIS Express" profile
2. **HTTPS Redirect**: The app now skips HTTPS redirect in development
3. **Port Conflicts**: Make sure port 5163 is not used by another application
4. **Firewall**: Windows Firewall might block the connection

## Expected Behavior

When working correctly:
- ✅ Visual Studio opens browser to `http://localhost:5163`
- ✅ Swagger UI is displayed
- ✅ API endpoint `/api/simplequery/execute` is accessible
- ✅ POST requests with SQL queries work
- ✅ Returns JSON with database results

## If Still Not Working

1. **Check Visual Studio Output**: Look for error messages
2. **Try Different Profile**: Switch between "http" and "IIS Express"
3. **Check Port**: Ensure port 5163 is available
4. **Restart Visual Studio**: Sometimes helps with profile issues
5. **Use Console**: Fall back to `dotnet run` if needed

The application should now work perfectly from both Visual Studio and console! 🚀
