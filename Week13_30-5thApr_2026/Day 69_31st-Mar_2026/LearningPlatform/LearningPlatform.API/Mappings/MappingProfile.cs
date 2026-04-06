using AutoMapper;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;

// Explicit alias to avoid ambiguity with AutoMapper.Profile
using UserProfile = LearningPlatform.API.Models.Profile;

namespace LearningPlatform.API.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile));

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Profile mappings
            CreateMap<UserProfile, ProfileDto>();
            CreateMap<UpdateProfileDto, UserProfile>();

            // Course mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.Username : string.Empty))
                .ForMember(dest => dest.LessonCount,
                    opt => opt.MapFrom(src => src.Lessons != null ? src.Lessons.Count : 0))
                .ForMember(dest => dest.EnrollmentCount,
                    opt => opt.MapFrom(src => src.Enrollments != null ? src.Enrollments.Count : 0));

            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateCourseDto, Course>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Lesson mappings
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.CourseTitle,
                    opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : string.Empty))
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src => src.Course != null && src.Course.Instructor != null
                        ? src.Course.Instructor.Username : string.Empty));
        }
    }
}
