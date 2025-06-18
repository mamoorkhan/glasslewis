
using AutoMapper;
using GlassLewis.Application.Dtos.Responses.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Responses.Company;

/// <summary>  
/// AutoMapper profile for mapping between <see cref="CompanyEntity"/> and <see cref="GetCompanyResponseDto"/>.  
/// </summary>  
public class GetCompanyResponseDtoProfile : Profile, IProfile
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="GetCompanyResponseDtoProfile"/> class.  
    /// Configures the mapping between <see cref="CompanyEntity"/> and <see cref="GetCompanyResponseDto"/>.  
    /// </summary>  
    public GetCompanyResponseDtoProfile()
    {
        CreateMap<CompanyEntity, GetCompanyResponseDto>();
    }
}
