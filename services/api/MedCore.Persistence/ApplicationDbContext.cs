using System.Reflection;
using MedCore.Application.Common;
using MedCore.Domain.Common;
using MedCore.Domain.Entities.Admin;
using MedCore.Domain.Entities.Clinical;
using MedCore.Domain.Entities.Core;
using MedCore.Domain.Entities.Lookup;
using MedCore.Domain.Entities.Messaging;
using MedCore.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Persistence;

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
    public DbSet<MedCore.Domain.Entities.Core.File> Files => Set<MedCore.Domain.Entities.Core.File>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // Clinical
    public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<PatientAllergy> PatientAllergies => Set<PatientAllergy>();
    public DbSet<PatientChronicCondition> PatientChronicConditions => Set<PatientChronicCondition>();
    public DbSet<PatientMedication> PatientMedications => Set<PatientMedication>();
    public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
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
    
    // Clinical Productivity
    public DbSet<ConsultationTemplate> ConsultationTemplates => Set<ConsultationTemplate>();
    public DbSet<PrescriptionTemplate> PrescriptionTemplates => Set<PrescriptionTemplate>();
    public DbSet<PrescriptionTemplateItem> PrescriptionTemplateItems => Set<PrescriptionTemplateItem>();
    public DbSet<DoctorFavoriteMedicine> DoctorFavoriteMedicines => Set<DoctorFavoriteMedicine>();

    // Messaging
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    // Admin
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

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
