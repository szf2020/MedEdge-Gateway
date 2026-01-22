@echo off
REM GitHub Repository SEO Optimization Script (Windows)
REM For: MedEdge Gateway (https://github.com/bejranonda/MedEdge-Gateway)
REM Owner: bejranonda
REM Purpose: Update repository description and topics with optimal SEO keywords

REM Prerequisites:
REM - GitHub CLI installed (https://cli.github.com/)
REM - Logged in as repository owner: gh auth login
REM - Run from repository directory

setlocal enabledelayedexpansion

set REPO=bejranonda/MedEdge-Gateway
set DESCRIPTION=Production-grade Medical Device IoT Platform - FHIR R4 Healthcare Interoperability - Real-Time Clinical AI - Anomaly Detection - Edge Gateway

echo.
echo ==================================
echo GitHub Repository SEO Optimization
echo ==================================
echo.
echo Repository: %REPO%
echo Action: Update description and topics
echo.

REM Step 1: Update repository description
echo Step 1/2: Updating repository description...
echo Description: %DESCRIPTION%
echo.

gh repo edit %REPO% --description "%DESCRIPTION%" >nul 2>&1

if %errorlevel% equ 0 (
    echo [OK] Description updated successfully
) else (
    echo [INFO] Update may require owner permissions
    echo        Verify manually in Settings - Repository Details
)

echo.

REM Step 2: Add GitHub topics
echo Step 2/2: Adding GitHub topics...
echo.

set "TOPICS=medical-device-iot,fhir-r4,healthcare-interoperability,clinical-ai,edge-computing,aspnet-core,real-time-monitoring,iot-platform,medical-informatics,anomaly-detection,signal-r,blazor,mqtt,modbus,ai-engine,industrial-iot,edge-gateway,fhir-transformation,clinical-thresholds,healthcare-data"

echo Topics to add:
for %%T in (%TOPICS%) do echo  - %%T

echo.

REM Call GitHub API to set topics
(
echo {
echo   "names": [
echo     "medical-device-iot",
echo     "fhir-r4",
echo     "healthcare-interoperability",
echo     "clinical-ai",
echo     "edge-computing",
echo     "aspnet-core",
echo     "real-time-monitoring",
echo     "iot-platform",
echo     "medical-informatics",
echo     "anomaly-detection",
echo     "signal-r",
echo     "blazor",
echo     "mqtt",
echo     "modbus",
echo     "ai-engine",
echo     "industrial-iot",
echo     "edge-gateway",
echo     "fhir-transformation",
echo     "clinical-thresholds",
echo     "healthcare-data"
echo   ]
echo }
) | gh api "repos/%REPO%/topics" -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2022-11-28" --input - >nul 2>&1

if %errorlevel% equ 0 (
    echo [OK] Topics added successfully
) else (
    echo [INFO] Topics update may require API access
    echo        Verify manually in Settings - Topics
)

echo.
echo ==================================
echo [OK] GitHub SEO Optimization Complete
echo ==================================
echo.
echo Verification Steps:
echo 1. Go to: https://github.com/%REPO%
echo 2. Check repository description (under repo name)
echo 3. Check topics in About section
echo 4. All 20 topics should be visible
echo.
echo Expected Results:
echo - Increased visibility in GitHub search
echo - Better ranking for medical device IoT keywords
echo - Improved Google search ranking
echo - More stars and contributions
echo.
echo Monitor Progress:
echo - View count: Settings - Insights - Traffic
echo - Search position: GitHub search for 'medical device iot'
echo.
pause
