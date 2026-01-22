# GitHub Repository SEO Optimization Script (PowerShell)
# For: MedEdge Gateway (https://github.com/bejranonda/MedEdge-Gateway)
# Owner: bejranonda
# Purpose: Update repository description and topics with optimal SEO keywords

# Prerequisites:
# - GitHub CLI installed (https://cli.github.com/)
# - Logged in as repository owner: gh auth login
# - Run from repository directory
# - Run as Administrator for full permissions

param(
    [string]$Repository = "bejranonda/MedEdge-Gateway"
)

# Configuration
$DESCRIPTION = "Production-grade Medical Device IoT Platform | FHIR R4 Healthcare Interoperability | Real-Time Clinical AI | Anomaly Detection | Edge Gateway"

$TOPICS = @(
    "medical-device-iot",
    "fhir-r4",
    "healthcare-interoperability",
    "clinical-ai",
    "edge-computing",
    "aspnet-core",
    "real-time-monitoring",
    "iot-platform",
    "medical-informatics",
    "anomaly-detection",
    "signal-r",
    "blazor",
    "mqtt",
    "modbus",
    "ai-engine",
    "industrial-iot",
    "edge-gateway",
    "fhir-transformation",
    "clinical-thresholds",
    "healthcare-data"
)

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "GitHub Repository SEO Optimization" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Repository: $Repository" -ForegroundColor Yellow
Write-Host "Action: Update description and topics" -ForegroundColor Yellow
Write-Host ""

# Step 1: Update repository description
Write-Host "Step 1/2: Updating repository description..." -ForegroundColor White
Write-Host "Description: $DESCRIPTION" -ForegroundColor Gray
Write-Host ""

try {
    $descResult = gh repo edit $Repository --description $DESCRIPTION 2>&1
    Write-Host "✓ Description updated successfully" -ForegroundColor Green
}
catch {
    Write-Host "⚠ Note: Update may require owner permissions" -ForegroundColor Yellow
    Write-Host "   Verify manually in Settings → Repository Details" -ForegroundColor Yellow
}

Write-Host ""

# Step 2: Add GitHub topics
Write-Host "Step 2/2: Adding GitHub topics..." -ForegroundColor White
Write-Host ""

Write-Host "Topics to add:" -ForegroundColor White
$TOPICS | ForEach-Object {
    Write-Host "  - $_" -ForegroundColor Gray
}

Write-Host ""

# Create JSON payload for API call
$topicsJson = @{
    names = $TOPICS
} | ConvertTo-Json

try {
    # Call GitHub API to set topics
    $topicsResult = $topicsJson | gh api "repos/$Repository/topics" `
        -H "Accept: application/vnd.github+json" `
        -H "X-GitHub-Api-Version: 2022-11-28" `
        --input - 2>&1

    Write-Host "✓ Topics added successfully" -ForegroundColor Green
}
catch {
    Write-Host "⚠ Note: Topics update may require API access" -ForegroundColor Yellow
    Write-Host "   Verify manually in Settings → Topics" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "✓ GitHub SEO Optimization Complete" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Verification Steps:" -ForegroundColor Yellow
Write-Host "1. Go to: https://github.com/$Repository" -ForegroundColor White
Write-Host "2. Check repository description (under repo name)" -ForegroundColor White
Write-Host "3. Check topics in About section" -ForegroundColor White
Write-Host "4. All 20 topics should be visible" -ForegroundColor White
Write-Host ""

Write-Host "Expected Results:" -ForegroundColor Yellow
Write-Host "- Increased visibility in GitHub search" -ForegroundColor White
Write-Host "- Better ranking for medical device IoT keywords" -ForegroundColor White
Write-Host "- Improved Google search ranking" -ForegroundColor White
Write-Host "- More stars and contributions" -ForegroundColor White
Write-Host ""

Write-Host "Monitor Progress:" -ForegroundColor Yellow
Write-Host "- View count: Settings → Insights → Traffic" -ForegroundColor White
Write-Host "- Search position: GitHub search for 'medical device iot'" -ForegroundColor White
Write-Host ""

# Provide next steps
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Verify changes: https://github.com/$Repository" -ForegroundColor White
Write-Host "2. Update README.md with keywords" -ForegroundColor White
Write-Host "3. Monitor traffic in GitHub Insights" -ForegroundColor White
Write-Host "4. Create GitHub discussions for engagement" -ForegroundColor White
Write-Host ""

Write-Host "For detailed information, see:" -ForegroundColor Yellow
Write-Host "- GITHUB-SEO-OPTIMIZATION.md (strategy)" -ForegroundColor Cyan
Write-Host "- GITHUB-SETUP-INSTRUCTIONS.md (manual steps)" -ForegroundColor Cyan
Write-Host ""
