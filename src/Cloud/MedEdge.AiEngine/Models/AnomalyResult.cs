namespace MedEdge.AiEngine.Models;

public enum RiskLevel
{
    Low,
    Moderate,
    High,
    Critical
}

public record AnomalyResult(
    RiskLevel Severity,
    string Finding,
    string Recommendation,
    DateTime DetectedAt
);
