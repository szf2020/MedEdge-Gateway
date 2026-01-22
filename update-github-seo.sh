#!/bin/bash

# GitHub Repository SEO Optimization Script
# For: MedEdge Gateway (https://github.com/bejranonda/MedEdge-Gateway)
# Owner: bejranonda
# Purpose: Update repository description and topics with optimal SEO keywords

# Prerequisites:
# - GitHub CLI installed (https://cli.github.com/)
# - Logged in as repository owner: gh auth login
# - Run from repository directory or specify full repo path

REPO="bejranonda/MedEdge-Gateway"
DESCRIPTION="Production-grade Medical Device IoT Platform | FHIR R4 Healthcare Interoperability | Real-Time Clinical AI | Anomaly Detection | Edge Gateway"

echo "=================================="
echo "GitHub Repository SEO Optimization"
echo "=================================="
echo ""
echo "Repository: $REPO"
echo "Action: Update description and topics"
echo ""

# Step 1: Update repository description
echo "Step 1/2: Updating repository description..."
echo "Description: $DESCRIPTION"
echo ""

gh repo edit $REPO --description "$DESCRIPTION" 2>/dev/null

if [ $? -eq 0 ]; then
    echo "✅ Description updated successfully"
else
    echo "⚠️  Note: Update may require owner permissions"
    echo "   Verify manually in Settings → Repository Details"
fi

echo ""

# Step 2: Add GitHub topics
echo "Step 2/2: Adding GitHub topics..."
echo ""

# Topics array
declare -a TOPICS=(
    "medical-device-iot"
    "fhir-r4"
    "healthcare-interoperability"
    "clinical-ai"
    "edge-computing"
    "aspnet-core"
    "real-time-monitoring"
    "iot-platform"
    "medical-informatics"
    "anomaly-detection"
    "signal-r"
    "blazor"
    "mqtt"
    "modbus"
    "ai-engine"
    "industrial-iot"
    "edge-gateway"
    "fhir-transformation"
    "clinical-thresholds"
    "healthcare-data"
)

# Convert array to JSON format for API call
TOPICS_JSON=$(printf '%s\n' "${TOPICS[@]}" | jq -R . | jq -s .)

echo "Topics to add:"
echo "$TOPICS_JSON" | jq .[]
echo ""

# Call GitHub API to set topics
gh api "repos/$REPO/topics" \
    -H "Accept: application/vnd.github+json" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    --input - <<EOF 2>/dev/null
{
  "names": [
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
  ]
}
EOF

if [ $? -eq 0 ]; then
    echo "✅ Topics added successfully"
else
    echo "⚠️  Note: Topics update may require API access"
    echo "   Verify manually in Settings → Topics"
fi

echo ""
echo "=================================="
echo "✅ GitHub SEO Optimization Complete"
echo "=================================="
echo ""
echo "Verification Steps:"
echo "1. Go to: https://github.com/bejranonda/MedEdge-Gateway"
echo "2. Check repository description (under repo name)"
echo "3. Check topics in About section"
echo "4. All 20 topics should be visible"
echo ""
echo "Expected Results:"
echo "- Increased visibility in GitHub search"
echo "- Better ranking for medical device IoT keywords"
echo "- Improved Google search ranking"
echo "- More stars and contributions"
echo ""
echo "Monitor Progress:"
echo "- View count: Settings → Insights → Traffic"
echo "- Search position: GitHub search for 'medical device iot'"
echo ""
