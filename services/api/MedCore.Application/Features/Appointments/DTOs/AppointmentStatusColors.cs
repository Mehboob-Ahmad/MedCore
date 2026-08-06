using System.Collections.Generic;

namespace MedCore.Application.Features.Appointments.DTOs;

/// <summary>
/// Centralized mapping of appointment statuses to display colors.
/// Used by both patient and doctor views for consistent calendar color-coding.
/// </summary>
public static class AppointmentStatusColors
{
    private static readonly Dictionary<string, string> StatusColorMap = new()
    {
        { "Reserved", "#94a3b8" },   // Slate
        { "Pending", "#f59e0b" },    // Amber
        { "Confirmed", "#3b82f6" },  // Blue
        { "Rejected", "#ef4444" },   // Red
        { "Completed", "#22c55e" },  // Green
        { "Cancelled", "#6b7280" },  // Gray
        { "NoShow", "#dc2626" },     // Dark Red
        { "Rescheduled", "#8b5cf6" } // Violet
    };

    public static string GetColor(string status) =>
        StatusColorMap.TryGetValue(status, out var color) ? color : "#6b7280";
}
