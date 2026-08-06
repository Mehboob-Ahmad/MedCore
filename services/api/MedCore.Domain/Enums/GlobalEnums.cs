namespace MedCore.Domain.Enums;

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    Completed,
    Cancelled,
    NoShow
}

public enum Gender
{
    Male,
    Female,
    Other,
    PreferNotToSay
}

public enum BloodType
{
    APositive,
    ANegative,
    BPositive,
    BNegative,
    ABPositive,
    ABNegative,
    OPositive,
    ONegative
}

public enum AllergySeverity
{
    Mild,
    Moderate,
    Severe
}

public enum NotificationType
{
    AppointmentConfirmed,
    AppointmentCancelled,
    NewMessage,
    Reminder,
    SystemAlert
}

public enum NotificationChannel
{
    InApp,
    Email,
    SMS,
    Push
}

public enum SettingDataType
{
    String,
    Int,
    Boolean,
    Decimal,
    Json
}
