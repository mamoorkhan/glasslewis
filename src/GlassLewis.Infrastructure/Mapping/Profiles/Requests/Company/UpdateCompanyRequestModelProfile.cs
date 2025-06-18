using AutoMapper;
using GlassLewis.Application.Dtos.Requests.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Requests.Company;

/// <summary>  
/// Provides mapping configuration for updating a company entity using the UpdateCompanyRequestDto.  
/// </summary>  
public class UpdateCompanyRequestDtoProfile : Profile, IProfile
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="UpdateCompanyRequestDtoProfile"/> class.  
    /// Configures mappings between <see cref="UpdateCompanyRequestDto"/> and <see cref="CompanyEntity"/>.  
    /// </summary>  
    public UpdateCompanyRequestDtoProfile()
    {
        CreateMap<UpdateCompanyRequestDto, CompanyEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
