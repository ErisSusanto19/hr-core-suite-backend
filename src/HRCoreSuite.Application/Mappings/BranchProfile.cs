using AutoMapper;
using HRCoreSuite.Application.DTOs.Branch;
using HRCoreSuite.Domain;

namespace HRCoreSuite.Application.Mappings;

public class BranchProfile : Profile
{
    public BranchProfile()
    {
        CreateMap<Branch, BranchResponseDto>();
        CreateMap<CreateBranchDto, Branch>();

        CreateMap<UpdateBranchDto, Branch>();
    }
}