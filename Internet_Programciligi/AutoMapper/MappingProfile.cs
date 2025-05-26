using AutoMapper;
using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;

namespace Internet_Programciligi.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.TeacherProfile.Name))
                .ForMember(dest => dest.LessonsCount, opt => opt.MapFrom(src => src.Lessons.Count(l => l.IsActive)))
                .ForMember(dest => dest.StudentCount, opt => 
                    opt.MapFrom(src => src.Enrollments != null ? 
                        src.Enrollments.Count(e => e.IsActive) : 0))
                .ForMember(dest => dest.AverageRating, opt => 
                    opt.MapFrom(src => src.Reviews != null && src.Reviews.Any() ? 
                        src.Reviews.Average(r => r.Rating) : 0));

            CreateMap<CreateCourseDTO, Course>();
            CreateMap<UpdateCourseDTO, Course>();

            CreateMap<Enrollment, EnrollmentDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.Course.ImageUrl));

            CreateMap<CreateEnrollmentDTO, Enrollment>();
            CreateMap<UpdateEnrollmentDTO, Enrollment>();
        }
    }
} 