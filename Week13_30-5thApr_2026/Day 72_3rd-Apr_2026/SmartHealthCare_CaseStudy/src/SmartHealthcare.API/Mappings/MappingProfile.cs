using AutoMapper;
using SmartHealthcare.Models.DTOs;
using SmartHealthcare.Models.Entities;

namespace SmartHealthcare.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>();

        CreateMap<Patient, PatientDTO>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email));
        CreateMap<CreatePatientDTO, Patient>();
        CreateMap<UpdatePatientDTO, Patient>();

        CreateMap<Doctor, DoctorDTO>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email))
            .ForMember(d => d.Specializations, opt => opt.MapFrom(s => s.DoctorSpecializations.Select(ds => ds.Specialization.Name).ToList()));
        CreateMap<CreateDoctorDTO, Doctor>();
        CreateMap<UpdateDoctorDTO, Doctor>();

        CreateMap<Appointment, AppointmentDTO>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.User.FullName))
            .ForMember(d => d.DoctorName, opt => opt.MapFrom(s => s.Doctor.User.FullName));
        CreateMap<CreateAppointmentDTO, Appointment>();
        CreateMap<UpdateAppointmentDTO, Appointment>();

        CreateMap<Prescription, PrescriptionDTO>();
        CreateMap<CreatePrescriptionDTO, Prescription>();

        CreateMap<Medicine, MedicineDTO>();
        CreateMap<CreateMedicineDTO, Medicine>();

        CreateMap<Specialization, SpecializationDTO>();
    }
}
