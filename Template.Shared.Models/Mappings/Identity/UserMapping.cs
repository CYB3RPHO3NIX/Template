using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Template.Database.Domain.Entities.Identity;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Shared.Models.Mappings.Identity
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
