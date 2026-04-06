using AutoMapper;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository repo, IMapper mapper, ILogger<PatientService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<PatientDTO>> GetAllAsync(int page, int pageSize)
    {
        var patients = await _repo.GetAllWithDetailsAsync(page, pageSize);
        var totalCount = await _repo.CountAsync();
        return new PagedResult<PatientDTO>
        {
            Items = _mapper.Map<List<PatientDTO>>(patients),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public async Task<PatientDTO?> GetByIdAsync(int id)
    {
        var patient = await _repo.GetWithDetailsAsync(id);
        return patient == null ? null : _mapper.Map<PatientDTO>(patient);
    }

    public async Task<PatientDTO?> GetByUserIdAsync(int userId)
    {
        var patient = await _repo.GetByUserIdAsync(userId);
        return patient == null ? null : _mapper.Map<PatientDTO>(patient);
    }

    public async Task<PatientDTO> CreateAsync(int userId, CreatePatientDTO dto)
    {
        var patient = _mapper.Map<Patient>(dto);
        patient.UserId = userId;
        await _repo.AddAsync(patient);
        await _repo.SaveAsync();
        _logger.LogInformation("Patient profile created for UserId: {UserId}", userId);

        var created = await _repo.GetWithDetailsAsync(patient.Id);
        return _mapper.Map<PatientDTO>(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePatientDTO dto)
    {
        var patient = await _repo.GetByIdAsync(id);
        if (patient == null)
        {
            return false;
        }

        _mapper.Map(dto, patient);
        _repo.Update(patient);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> PatchAsync(int id, Dictionary<string, object> patchData)
    {
        var patient = await _repo.GetByIdAsync(id);
        if (patient == null)
        {
            return false;
        }

        foreach (var kvp in patchData)
        {
            var prop = typeof(Patient).GetProperty(kvp.Key);
            if (prop != null && prop.CanWrite)
            {
                var value = Convert.ChangeType(kvp.Value, prop.PropertyType);
                prop.SetValue(patient, value);
            }
        }

        _repo.Update(patient);
        await _repo.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _repo.GetByIdAsync(id);
        if (patient == null)
        {
            return false;
        }

        _repo.Delete(patient);
        await _repo.SaveAsync();
        return true;
    }
}
