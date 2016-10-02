using AutoMapper;
using KickSter.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.App_Start
{
    public class AutoMapperInitializer
    {        
        public static IMapper Configure()
        {                       
            var config = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<Post, MatchedPost>();
                //.ForMember(dto => dto.Score, opt => opt.UseValue(0))
                //.ForMember(dto => dto.Day, opt => opt.UseValue(false))
                //.ForMember(dto => dto.Arena, opt => opt.UseValue(false))
                //.ForMember(dto => dto.Zone, opt => opt.UseValue(false));

                cfg.CreateMap<UserProfile, User>();
                cfg.CreateMap<UserProfile, UserInfo>();
                //cfg.CreateMap<ArenaInfo, Arena>();
            });

            return config.CreateMapper();
        }
    }
}