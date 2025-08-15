using AutoMapper;
using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;
using System.Text.Json;

namespace SilkRoad.Core.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GameInstance, GameInstanceDto>()
                .ForMember(dest => dest.Entities, opt => opt.MapFrom(src => src.Entities))
                .ForMember(dest => dest.EntityRelationships, opt => opt.MapFrom(src => src.EntityRelationships))
                .ForMember(dest => dest.GameStates, opt => opt.MapFrom(src => src.GameStates))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .PreserveReferences();

            CreateMap<Entity, EntityDto>()
                .ForMember(dest => dest.EntityAttributes, opt => opt.MapFrom(src => src.EntityAttributes))
                .ForMember(dest => dest.EntityRelationshipSourceEntities, opt => opt.MapFrom(src => src.EntityRelationshipSourceEntities))
                .ForMember(dest => dest.EntityRelationshipTargetEntities, opt => opt.MapFrom(src => src.EntityRelationshipTargetEntities))
                .PreserveReferences();

            CreateMap<EntityAttribute, EntityAttributeDto>()
                .ForMember(dest => dest.AttributeValue, opt => opt.ConvertUsing(new JsonStringToObjectValueConverter()));

            CreateMap<EntityRelationship, EntityRelationshipDto>()
                .PreserveReferences();

            CreateMap<GameState, GameStateDto>()
                .ForMember(dest => dest.StateValue, opt => opt.ConvertUsing(new JsonStringToObjectValueConverter()));

            CreateMap<User, UserDto>();
        }
    }

    public class JsonStringToObjectValueConverter : IValueConverter<string, object>
    {
        public object Convert(string source, ResolutionContext context)
        {
            return source != null ? JsonSerializer.Deserialize<object>(source, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) : null;
        }
    }
}