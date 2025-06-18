using AutoMapper;
using GlassLewis.Application.Dtos.Requests.Company;
using CompanyEntity = GlassLewis.Domain.Entities.Company;

namespace GlassLewis.Infrastructure.Mapping.Profiles.Requests.Company;

/// <summary>  
/// AutoMapper profile for mapping <see cref="PatchCompanyRequestDto"/> to <see cref="CompanyEntity"/>.  
/// </summary>  
public class PatchCompanyRequestDtoProfile : Profile, IProfile
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="PatchCompanyRequestDtoProfile"/> class.  
    /// Configures mapping rules for <see cref="PatchCompanyRequestDto"/> to <see cref="CompanyEntity"/>.  
    /// </summary>  
    public PatchCompanyRequestDtoProfile()
    {
        CreateMap<PatchCompanyRequestDto, CompanyEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
