using AutoMapper;
using HRCoreSuite.Application.DTOs.Position;
using HRCoreSuite.Domain;

namespace HRCoreSuite.Application.Mappings;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<Position, PositionResponseDto>();
        CreateMap<CreatePositionDto, Position>();

        CreateMap<UpdatePositionDto, Position>();
    }
}