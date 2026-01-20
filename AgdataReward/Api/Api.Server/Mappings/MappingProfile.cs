using Api.Server.DTOs.Event;
using Api.Server.DTOs.Product;
using Api.Server.DTOs.Redemption;
using Api.Server.DTOs.Reward;
using Api.Server.DTOs.User;
using AutoMapper;
using Domain.Entities.Event;
using Domain.Entities.Product;
using Domain.Entities.Redemption;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.ValueObjects;

namespace Api.Server.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // =========================
        // Event mappings
        // =========================
        CreateMap<EventDefinition, EventDefinitionDto>()
            .ForMember(dest => dest.Instances,
                opt => opt.MapFrom(src => src.Instances))
            .ForMember(dest => dest.RewardRules,
                opt => opt.MapFrom(src => src.RewardRules));

        CreateMap<EventDefinitionCreateDto, EventDefinition>()
            .ConstructUsing(dto => new EventDefinition(
                Guid.NewGuid(),
                dto.Code,
                dto.Title,
                dto.StartDate,
                dto.EndDate));

        CreateMap<EventDefinitionUpdateDto, EventDefinition>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<EventInstance, EventInstanceDto>().ReverseMap();
        CreateMap<EventRewardRule, EventRewardRuleDto>().ReverseMap();

        // =========================
        // Product mappings
        // =========================
        CreateMap<ProductInformationCreateDto, ProductInformation>()
            .ForCtorParam("sku", opt => opt.MapFrom(src => new SKU(src.SKU)))
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("rewardPointsId", opt => opt.MapFrom(src => src.RewardPointsId));

        CreateMap<ProductInformation, ProductInformationDto>()
            .ForMember(dest => dest.SKU,
                opt => opt.MapFrom(src => src.SKU.Value));

        CreateMap<ProductInventory, ProductInventoryDto>();

        // =========================
        // Redemption mappings
        // =========================
        CreateMap<RedemptionRecord, RedemptionRecordDto>().ReverseMap();

        CreateMap<RedemptionRecordCreateDto, RedemptionRecord>()
            .ConstructUsing(dto => new RedemptionRecord(Guid.NewGuid(), dto.UserId, dto.ProductId));

        CreateMap<RedemptionRequest, RedemptionRequestDto>().ReverseMap();

        CreateMap<RedemptionRequestCreateDto, RedemptionRequest>()
            .ConstructUsing(dto => new RedemptionRequest(dto.RedemptionId, dto.PointsUsed));

        CreateMap<RedemptionRequestUpdateDto, RedemptionRequest>().ReverseMap();

        // =========================
        // Reward mappings
        // =========================
        CreateMap<RewardPoints, RewardPointsDto>().ReverseMap();
        CreateMap<RewardPointsCreateDto, RewardPoints>().ReverseMap();

        CreateMap<RewardTransaction, RewardTransactionDto>().ReverseMap();
        CreateMap<RewardTransactionCreateDto, RewardTransaction>().ReverseMap();
        CreateMap<RewardTransactionUpdateDto, RewardTransaction>().ReverseMap();
        CreateMap<RewardTransaction, Top3EmployeeRewardDto>().ReverseMap();

        // =========================
        // =========================
        // User mappings
        // =========================
        CreateMap<UserProfile, UserProfileDto>()
            .ForMember(dest => dest.EmployeeId,
                opt => opt.MapFrom(src => src.EmployeeId.Value))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.Roles,
                opt => opt.MapFrom(src => src.Roles.Select(r => r.Role.Name)))
            .ForMember(dest => dest.Account,
                opt => opt.MapFrom(src => src.Account));

        CreateMap<UserAccount, UserAccountDto>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
