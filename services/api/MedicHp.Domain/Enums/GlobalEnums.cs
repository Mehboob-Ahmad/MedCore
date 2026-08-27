namespace MedicHp.Domain.Enums;

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

public enum WhatsAppMessageDirection
{
    Incoming,
    Outgoing
}

public enum WhatsAppMessageType
{
    Text,
    Image,
    Document,
    Audio,
    Video,
    Location,
    Contact,
    Sticker,
    Interactive,
    Button,
    Template,
    Reaction,
    Unknown
}

public enum WhatsAppMessageStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed
}

public enum WhatsAppConnectionStatus
{
    NotConnected,
    Pending,
    Connected,
    ReauthorizationRequired,
    Disconnected,
    Failed
}

public enum PaymentMethodType
{
    Cash,
    BankTransfer,
    JazzCash,
    Easypaisa,
    Other
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Overdue
}
