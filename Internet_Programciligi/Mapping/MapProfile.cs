using AutoMapper;
using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;

namespace Internet_Programciligi.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            // Course Mappings
            CreateMap<Course, CourseDTO>();
            CreateMap<CreateCourseDTO, Course>();
            CreateMap<UpdateCourseDTO, Course>();

            // User Mappings
            CreateMap<AppUser, UserDTO>();
            CreateMap<RegisterDTO, AppUser>();

            // Enrollment Mappings
            CreateMap<Enrollment, EnrollmentDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));
            CreateMap<CreateEnrollmentDTO, Enrollment>();
            CreateMap<UpdateEnrollmentDTO, Enrollment>();
        }
    }
} 