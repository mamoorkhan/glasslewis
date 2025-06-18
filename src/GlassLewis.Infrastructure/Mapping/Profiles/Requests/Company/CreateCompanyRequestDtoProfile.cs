using AutoMapper;
using GlassLewis.Application.Dtos.Requests.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Requests.Company;

/// <summary>  
/// AutoMapper profile for mapping <see cref="CreateCompanyRequestDto"/> to <see cref="CompanyEntity"/>.  
/// </summary>  
public class CreateCompanyRequestDtoProfile : Profile, IProfile
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="CreateCompanyRequestDtoProfile"/> class.  
    /// Configures the mapping between <see cref="CreateCompanyRequestDto"/> and <see cref="CompanyEntity"/>.  
    /// </summary>  
    public CreateCompanyRequestDtoProfile()
    {
        CreateMap<CreateCompanyRequestDto, CompanyEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
