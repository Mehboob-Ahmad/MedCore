using System.Reflection;
using MedicHp.Application.Common;
using MedicHp.Domain.Common;
using MedicHp.Domain.Entities.Admin;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Lookup;
using MedicHp.Domain.Entities.Messaging;
using MedicHp.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor) : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    // Core
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailVerification> EmailVerifications => Set<EmailVerification>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<MedicHp.Domain.Entities.Core.File> Files => Set<MedicHp.Domain.Entities.Core.File>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // Clinical
    public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();
    public DbSet<PatientChronicCondition> PatientChronicConditions => Set<PatientChronicCondition>();
    public DbSet<PatientMedication> PatientMedications => Set<PatientMedication>();
    public DbSet<PatientSurgery> PatientSurgeries => Set<PatientSurgery>();
    public DbSet<PatientSurgeryDocument> PatientSurgeryDocuments => Set<PatientSurgeryDocument>();
    public DbSet<PatientHospitalization> PatientHospitalizations => Set<PatientHospitalization>();
    public DbSet<PatientMedicalReport> PatientMedicalReports => Set<PatientMedicalReport>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
    public DbSet<DoctorPaymentMethod> DoctorPaymentMethods => Set<DoctorPaymentMethod>();
    public DbSet<DoctorSpecialization> DoctorSpecializations => Set<DoctorSpecialization>();
    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();
    public DbSet<DoctorUnavailability> DoctorUnavailabilities => Set<DoctorUnavailability>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AppointmentStatusHistory> AppointmentStatusHistories => Set<AppointmentStatusHistory>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<ConsultationVital> ConsultationVitals => Set<ConsultationVital>();
    public DbSet<ConsultationAddendum> ConsultationAddenda => Set<ConsultationAddendum>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<PatientFavoriteDoctor> PatientFavoriteDoctors => Set<PatientFavoriteDoctor>();
    public DbSet<DoctorQualification> DoctorQualifications => Set<DoctorQualification>();
    public DbSet<DoctorCertification> DoctorCertifications => Set<DoctorCertification>();
    public DbSet<AiChatMessage> AiChatMessages => Set<AiChatMessage>();
    // Clinical Productivity
    public DbSet<ConsultationTemplate> ConsultationTemplates => Set<ConsultationTemplate>();
    public DbSet<PrescriptionTemplate> PrescriptionTemplates => Set<PrescriptionTemplate>();
    public DbSet<PrescriptionTemplateItem> PrescriptionTemplateItems => Set<PrescriptionTemplateItem>();
    public DbSet<DoctorFavoriteMedicine> DoctorFavoriteMedicines => Set<DoctorFavoriteMedicine>();

    // Messaging
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<WhatsAppMessage> WhatsAppMessages => Set<WhatsAppMessage>();

    // Admin
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<DemoRequest> DemoRequests => Set<DemoRequest>();

    // Lookup
    public DbSet<City> Cities => Set<City>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Symptom> Symptoms => Set<Symptom>();
    public DbSet<SymptomSpecialization> SymptomSpecializations => Set<SymptomSpecialization>();
    public DbSet<Disease> Diseases => Set<Disease>();
    public DbSet<DiseaseSpecialization> DiseaseSpecializations => Set<DiseaseSpecialization>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain Events dispatching would happen here before/after saving
        return await base.SaveChangesAsync(cancellationToken);
    }
}
