using AutoMapper;
using HRCoreSuite.Application.DTOs.Employee;
using HRCoreSuite.Domain;

namespace HRCoreSuite.Application.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();

        CreateMap<Employee, EmployeeResponseDto>()
            .ForMember(dest => dest.BranchName, 
                       opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : string.Empty))
            .ForMember(dest => dest.PositionName, 
                       opt => opt.MapFrom(src => src.Position != null ? src.Position.Name : string.Empty));
    }
}