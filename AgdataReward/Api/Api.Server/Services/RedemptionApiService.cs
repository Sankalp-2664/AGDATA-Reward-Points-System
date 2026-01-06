using Api.Server.DTOs.Redemption;
using Application.Interfaces;
using AutoMapper;

namespace Api.Server.Services;

/// <summary>
/// API-facing redemption service that hides domain entities and mapping
/// from controllers, so controllers stay thin.
/// </summary>
public interface IRedemptionApiService
{
    Task<RedemptionRecordDto> RequestRedemptionAsync(
        RedemptionRecordCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<RedemptionRecordDto?> GetRedemptionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task ApproveRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task RejectRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CompleteRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

public class RedemptionApiService(
    IRedemptionService redemptionService,
    IMapper mapper) : IRedemptionApiService
{
    private readonly IRedemptionService _redemptionService = redemptionService;
    private readonly IMapper _mapper = mapper;

    public async Task<RedemptionRecordDto> RequestRedemptionAsync(
        RedemptionRecordCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // Domain API still works with IDs (Guid)
        var record = await _redemptionService.RequestRedemptionAsync(dto.UserId, dto.ProductId);
        return _mapper.Map<RedemptionRecordDto>(record);
    }

    public async Task<RedemptionRecordDto?> GetRedemptionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var record = await _redemptionService.GetRedemptionByIdAsync(id);
        return record is null
            ? null
            : _mapper.Map<RedemptionRecordDto>(record);
    }

    public async Task ApproveRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _redemptionService.ApproveRedemptionAsync(id);
    }

    public async Task RejectRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _redemptionService.RejectRedemptionAsync(id);
    }

    public async Task CompleteRedemptionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _redemptionService.CompleteRedemptionAsync(id);
    }
}
