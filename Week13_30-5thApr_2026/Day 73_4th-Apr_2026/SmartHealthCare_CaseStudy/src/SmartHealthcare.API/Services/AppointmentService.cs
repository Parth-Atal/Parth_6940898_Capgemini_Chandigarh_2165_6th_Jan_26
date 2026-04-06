using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository repo, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<AppointmentDTO>> GetAllAsync(int page, int pageSize)
    {
        var items = await _repo.GetAllWithDetailsAsync(page, pageSize);
        var total = await _repo.CountAsync();
        return new PagedResult<AppointmentDTO>
        {
            Items = _mapper.Map<List<AppointmentDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<AppointmentDTO?> GetByIdAsync(int id)
    {
        var appt = await _repo.GetWithDetailsAsync(id);
        return appt == null ? null : _mapper.Map<AppointmentDTO>(appt);
    }

    public async Task<PagedResult<AppointmentDTO>> GetByPatientIdAsync(int patientId, int page, int pageSize)
    {
        var items = await _repo.GetByPatientIdAsync(patientId, page, pageSize);
        var total = await _repo.CountByPatientAsync(patientId);
        return new PagedResult<AppointmentDTO>
        {
            Items = _mapper.Map<List<AppointmentDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<AppointmentDTO>> GetByDoctorIdAsync(int doctorId, int page, int pageSize)
    {
        var items = await _repo.GetByDoctorIdAsync(doctorId, page, pageSize);
        var total = await _repo.CountByDoctorAsync(doctorId);
        return new PagedResult<AppointmentDTO>
        {
            Items = _mapper.Map<List<AppointmentDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<AppointmentDTO>> GetByDateAsync(DateTime date, int page, int pageSize)
    {
        var items = await _repo.GetByDateAsync(date, page, pageSize);
        var total = await _repo.CountByDateAsync(date);
        return new PagedResult<AppointmentDTO>
        {
            Items = _mapper.Map<List<AppointmentDTO>>(items),
            TotalCount = total,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<AppointmentDTO> CreateAsync(int patientId, CreateAppointmentDTO dto)
    {
        var appointment = _mapper.Map<Appointment>(dto);
        appointment.PatientId = patientId;
        appointment.Status = "Pending";
        appointment.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(appointment);
        await _repo.SaveAsync();

        _logger.LogInformation("Appointment booked: PatientId={PatientId}, DoctorId={DoctorId}, Date={Date}",
            patientId, dto.DoctorId, dto.AppointmentDate);

        var created = await _repo.GetWithDetailsAsync(appointment.Id);
        return _mapper.Map<AppointmentDTO>(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAppointmentDTO dto)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt == null)
        {
            return false;
        }

        _mapper.Map(dto, appt);
        _repo.Update(appt);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> PatchAsync(int id, Dictionary<string, object> patchData)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt == null)
        {
            return false;
        }

        foreach (var kvp in patchData)
        {
            var prop = typeof(Appointment).GetProperty(kvp.Key);
            if (prop != null && prop.CanWrite && kvp.Key != "Id")
            {
                var converted = kvp.Value?.ToString();
                if (prop.PropertyType == typeof(DateTime))
                {
                    prop.SetValue(appt, DateTime.Parse(converted!));
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(appt, int.Parse(converted!));
                }
                else
                {
                    prop.SetValue(appt, converted);
                }
            }
        }

        _repo.Update(appt);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt == null)
        {
            return false;
        }

        _repo.Delete(appt);
        await _repo.SaveAsync();
        return true;
    }
}
