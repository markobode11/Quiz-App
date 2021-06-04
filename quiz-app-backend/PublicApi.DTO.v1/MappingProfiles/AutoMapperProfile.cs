using AutoMapper;

namespace PublicApi.DTO.v1.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.App.Question, PublicApi.DTO.v1.Question>().ReverseMap();
            CreateMap<Domain.App.Question, PublicApi.DTO.v1.QuestionEdit>().ReverseMap();
            CreateMap<Domain.App.Question, PublicApi.DTO.v1.QuestionCreate>().ReverseMap();
            
            CreateMap<Domain.App.Answer, PublicApi.DTO.v1.Answer>().ReverseMap();
            CreateMap<Domain.App.Answer, PublicApi.DTO.v1.AnswerCreate>().ReverseMap();
            
            CreateMap<Domain.App.Enums.EType, PublicApi.DTO.v1.Enums.EType>().ReverseMap();
            
            CreateMap<Domain.App.Answerable, PublicApi.DTO.v1.Answerable>().ReverseMap();
            CreateMap<Domain.App.Answerable, PublicApi.DTO.v1.AnswerableCreate>().ReverseMap();
            
            CreateMap<Domain.App.AppUserAnswerable, PublicApi.DTO.v1.AppUserAnswerable>().ReverseMap();
        }
    }
}