using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Application.Features.Productivity.Queries.GetDoctorAnalytics;
using MedCore.Domain.Entities.Clinical;
using MockQueryable.Moq;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace MedCore.Application.UnitTests.Features.Productivity;

public class GetDoctorAnalyticsQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Appointment>> _appointmentRepoMock;
    private readonly Mock<IGenericRepository<Consultation>> _consultationRepoMock;
    private readonly Mock<IGenericRepository<Prescription>> _prescriptionRepoMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly GetDoctorAnalyticsQueryHandler _handler;

    public GetDoctorAnalyticsQueryHandlerTests()
    {
        _appointmentRepoMock = new Mock<IGenericRepository<Appointment>>();
        _consultationRepoMock = new Mock<IGenericRepository<Consultation>>();
        _prescriptionRepoMock = new Mock<IGenericRepository<Prescription>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        _handler = new GetDoctorAnalyticsQueryHandler(
            _appointmentRepoMock.Object,
            _consultationRepoMock.Object,
            _prescriptionRepoMock.Object,
            _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccurateAnalytics()
    {
        var doctorId = Guid.NewGuid();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(doctorId);

        var today = DateTime.UtcNow.Date;

        var appointments = new List<Appointment>
        {
            new Appointment { DoctorId = doctorId, ScheduledAt = today, Status = "Completed", PatientId = Guid.NewGuid() },
            new Appointment { DoctorId = doctorId, ScheduledAt = today, Status = "Confirmed", PatientId = Guid.NewGuid() }
        }.AsQueryable().BuildMock();

        var consultations = new List<Consultation>
        {
            new Consultation { DoctorId = doctorId, IsFinalized = true, FollowUpDate = today.AddDays(2) }
        }.AsQueryable().BuildMock();

        var prescriptions = new List<Prescription>
        {
            new Prescription { Consultation = new Consultation { DoctorId = doctorId, IsFinalized = true } }
        }.AsQueryable().BuildMock();

        _appointmentRepoMock.Setup(r => r.GetQueryable()).Returns(appointments);
        _consultationRepoMock.Setup(r => r.GetQueryable()).Returns(consultations);
        _prescriptionRepoMock.Setup(r => r.GetQueryable()).Returns(prescriptions);

        var result = await _handler.Handle(new GetDoctorAnalyticsQuery(), CancellationToken.None);

        result.PatientsToday.Should().Be(1);
        result.UpcomingAppointments.Should().Be(1);
        result.ConsultationsCompleted.Should().Be(1);
        result.PrescriptionsIssued.Should().Be(1);
        result.PendingFollowUps.Should().Be(1);
    }
}
