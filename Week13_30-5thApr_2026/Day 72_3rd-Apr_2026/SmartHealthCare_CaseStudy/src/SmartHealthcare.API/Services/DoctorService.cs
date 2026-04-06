using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using SmartHealthcare.API.Data;
using SmartHealthcare.API.Repositories.Interfaces;
using SmartHealthcare.API.Services.Interfaces;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _repo;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DoctorService> _logger;
    private const string DoctorsCacheKey = "doctors_list";

    public DoctorService(IDoctorRepository repo, ApplicationDbContext context, IMapper mapper, IMemoryCache cache, ILogger<DoctorService> logger)
    {
        _repo = repo;
        _context = context;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<DoctorDTO>> GetAllAsync(int page, int pageSize)
    {
        var cacheKey = $"{DoctorsCacheKey}_{page}_{pageSize}";
        if (!_cache.TryGetValue(cacheKey, out PagedResult<DoctorDTO>? result))
        {
            var doctors = await _repo.GetAllWithDetailsAsync(page, pageSize);
            var totalCount = await _repo.CountAsync();
            result = new PagedResult<DoctorDTO>
            {
                Items = _mapper.Map<List<DoctorDTO>>(doctors),
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, result, cacheOptions);
            _logger.LogInformation("Doctor list cached for page {Page}", page);
        }

        return result!;
    }

    public async Task<DoctorDTO?> GetByIdAsync(int id)
    {
        var doctor = await _repo.GetWithDetailsAsync(id);
        return doctor == null ? null : _mapper.Map<DoctorDTO>(doctor);
    }

    public async Task<DoctorDTO?> GetByUserIdAsync(int userId)
    {
        var doctor = await _repo.GetByUserIdAsync(userId);
        return doctor == null ? null : _mapper.Map<DoctorDTO>(doctor);
    }

    public async Task<DoctorDTO> CreateAsync(int userId, CreateDoctorDTO dto)
    {
        var doctor = _mapper.Map<Doctor>(dto);
        doctor.UserId = userId;

        await _repo.AddAsync(doctor);
        await _repo.SaveAsync();

        foreach (var specId in dto.SpecializationIds)
        {
            _context.DoctorSpecializations.Add(new DoctorSpecialization
            {
                DoctorId = doctor.Id,
                SpecializationId = specId
            });
        }

        await _context.SaveChangesAsync();

        InvalidateAllDoctorsCaches();
        _logger.LogInformation("Doctor profile created for UserId: {UserId}", userId);
        var created = await _repo.GetWithDetailsAsync(doctor.Id);
        return _mapper.Map<DoctorDTO>(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDoctorDTO dto)
    {
        var doctor = await _repo.GetWithDetailsAsync(id);
        if (doctor == null)
        {
            return false;
        }

        _mapper.Map(dto, doctor);

        var existingSpecs = _context.DoctorSpecializations.Where(ds => ds.DoctorId == id);
        _context.DoctorSpecializations.RemoveRange(existingSpecs);

        foreach (var specId in dto.SpecializationIds)
        {
            _context.DoctorSpecializations.Add(new DoctorSpecialization
            {
                DoctorId = id,
                SpecializationId = specId
            });
        }

        _repo.Update(doctor);
        await _repo.SaveAsync();
        InvalidateAllDoctorsCaches();
        _logger.LogInformation("Doctor profile updated for Id: {Id}", id);
        return true;
    }

    public async Task<bool> PatchAsync(int id, Dictionary<string, object> patchData)
    {
        var doctor = await _repo.GetByIdAsync(id);
        if (doctor == null)
        {
            return false;
        }

        foreach (var kvp in patchData)
        {
            var prop = typeof(Doctor).GetProperty(kvp.Key);
            if (prop != null && prop.CanWrite && kvp.Key != "Id" && kvp.Key != "UserId")
            {
                var value = Convert.ChangeType(kvp.Value, prop.PropertyType);
                prop.SetValue(doctor, value);
            }
        }

        _repo.Update(doctor);
        await _repo.SaveAsync();
        InvalidateAllDoctorsCaches();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _repo.GetByIdAsync(id);
        if (doctor == null)
        {
            return false;
        }

        _repo.Delete(doctor);
        await _repo.SaveAsync();
        InvalidateAllDoctorsCaches();
        return true;
    }

    public async Task<PagedResult<DoctorDTO>> SearchBySpecializationAsync(string specialization, int page, int pageSize)
    {
        var doctors = await _repo.SearchBySpecializationAsync(specialization, page, pageSize);
        var totalCount = await _repo.CountBySpecializationAsync(specialization);
        return new PagedResult<DoctorDTO>
        {
            Items = _mapper.Map<List<DoctorDTO>>(doctors),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    private void InvalidateCache()
    {
        _cache.Remove(DoctorsCacheKey);
    }

    private void InvalidateAllDoctorsCaches()
    {
        // Invalidate all paginated doctor list caches (for pages 1-100 with various page sizes)
        for (int page = 1; page <= 100; page++)
        {
            for (int pageSize = 5; pageSize <= 50; pageSize += 5)
            {
                var cacheKey = $"{DoctorsCacheKey}_{page}_{pageSize}";
                _cache.Remove(cacheKey);
            }
        }
        _logger.LogInformation("All doctor list caches invalidated");
    }
}
