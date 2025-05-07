using AutoMapper;
using RNPM.API.ViewModels;
using RNPM.API.ViewModels.Core;
using RNPM.Common.Models;
using RNPM.Common.ViewModels;
using RNPM.Common.ViewModels.Core;
using RNPM.Common.ViewModels.UserManagement;
using RNPM.Common.ViewModels.UserManagement;

namespace RNPM.Common.Base;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //user and user roles
        CreateMap<ApplicationUser, UserViewModel>()
            .ForMember(dst => dst.Role, opts => opts.MapFrom(src => src.UserRoles.FirstOrDefault().Role.Name));
        
        CreateMap<ApplicationRole, RoleViewModel>();
        CreateMap<Application, ApplicationViewModel>();
        CreateMap<ScreenComponentRender, RenderInstanceViewModel>();
    }
}