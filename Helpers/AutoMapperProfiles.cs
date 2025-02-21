using System.Linq;
using AutoMapper;
using Storage.API.DTOs;
using Storage.API.Models;
using Storage.API_CAN.DTOs;
using Storage.API_CAN.Models;

namespace Storage.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(s => s.UserPhoto.FirstOrDefault(p => p.IsMain).Url));;
            CreateMap<Componentas, ComponetsForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<Photo, PhotosForDto>();
            CreateMap<ComponentForUpdateDto, Componentas>();
            CreateMap<ReelForUpdateDto, Reel>();
            CreateMap<LocationForRegisterDto, Reel>();
            CreateMap<Reel, ReelsForListDto>()
                .ForMember(dest => dest.PhotoUrl2, opt => opt.MapFrom(s => s.Photos2.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<Photo, PhotosForReturnDto>();
            CreateMap<PhotosForCreationDto, Photo>();
            CreateMap<History, HistoryForListDto>();
            CreateMap<UserForRegisterDTO, User>();
            CreateMap<User, UserForListAdminDto>();
            CreateMap<ReelForTakeDto, Reel>();
            CreateMap<PhotosForCreationDto, UserPhoto>();
            CreateMap<UserPhoto, PhotosForReturnDto>();
            CreateMap<TaskForUpdateDto, TaskName>();
        }
    }
} 