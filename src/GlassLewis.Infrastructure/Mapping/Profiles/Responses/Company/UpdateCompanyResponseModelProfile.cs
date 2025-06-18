using AutoMapper;
using GlassLewis.Application.Dtos.Responses.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Responses.Company;

/// <summary>  
/// Provides mapping configuration for converting <see cref="CompanyEntity"/> to <see cref="UpdateCompanyResponseDto"/>.  
/// </summary>  
public class UpdateCreateCompanyResponseDtoProfile : Profile, IProfile
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="UpdateCreateCompanyResponseDtoProfile"/> class.  
    /// Configures the mapping between <see cref="CompanyEntity"/> and <see cref="UpdateCompanyResponseDto"/>.  
    /// </summary>  
    public UpdateCreateCompanyResponseDtoProfile()
    {
        CreateMap<CompanyEntity, UpdateCompanyResponseDto>();
    }
}
