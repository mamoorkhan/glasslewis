using AutoMapper;
using GlassLewis.Application.Dtos.Responses.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Responses.Company;

/// <summary>
/// AutoMapper profile for mapping between <see cref="CompanyEntity"/> and <see cref="CreateCompanyResponseDto"/>.
/// </summary>
public class CreateCompanyResponseDtoProfile : Profile, IProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCompanyResponseDtoProfile"/> class.
    /// Configures the mapping between <see cref="CompanyEntity"/> and <see cref="CreateCompanyResponseDto"/>.
    /// </summary>
    public CreateCompanyResponseDtoProfile()
    {
        CreateMap<CompanyEntity, CreateCompanyResponseDto>();
    }
}
