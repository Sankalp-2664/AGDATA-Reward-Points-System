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

    Task<IEnumerable<PendingRedemptionDto>> GetAllPendingRequestsAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PendingRedemptionDto>> GetUserRedemptionHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

public class RedemptionApiService(
    IRedemptionService redemptionService,
    IRedemptionRecordRepository recordRepository,
    IRedemptionRequestRepository requestRepository,
    IUserRepository userRepository,
    IProductRepository productRepository,
    IMapper mapper) : IRedemptionApiService
{
    private readonly IRedemptionService _redemptionService = redemptionService;
    private readonly IRedemptionRecordRepository _recordRepository = recordRepository;
    private readonly IRedemptionRequestRepository _requestRepository = requestRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProductRepository _productRepository = productRepository;
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

    public async Task<IEnumerable<PendingRedemptionDto>> GetAllPendingRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var pendingRequests = await _redemptionService.GetAllPendingRequestsAsync();
        var result = new List<PendingRedemptionDto>();

        foreach (var request in pendingRequests)
        {
            var record = await _recordRepository.GetByIdAsync(request.RedemptionId);
            if (record == null) continue;

            var user = await _userRepository.GetByIdAsync(record.UserId);
            var product = await _productRepository.GetByIdAsync(record.ProductId);

            if (user == null || product == null) continue;

            result.Add(new PendingRedemptionDto
            {
                Id = request.Id,
                RedemptionId = request.RedemptionId,
                UserId = record.UserId,
                EmployeeId = user.EmployeeId.Value,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email.Value,
                ProductId = record.ProductId,
                ProductName = product.Name,
                PointsUsed = request.PointsUsed,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                RedeemedAt = record.RedeemedAt
            });
        }

        return result;
    }

    public async Task<IEnumerable<PendingRedemptionDto>> GetUserRedemptionHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRecords = await _recordRepository.GetByUserIdAsync(userId);
        var redemptionIds = userRecords.Select(r => r.Id).ToList();
        var requests = await _requestRepository.GetByRedemptionIdsAsync(redemptionIds);
        
        var result = new List<PendingRedemptionDto>();

        foreach (var record in userRecords)
        {
            var request = requests.FirstOrDefault(r => r.RedemptionId == record.Id);
            if (request == null) continue;

            var user = await _userRepository.GetByIdAsync(record.UserId);
            var product = await _productRepository.GetByIdAsync(record.ProductId);

            if (user == null || product == null) continue;

            result.Add(new PendingRedemptionDto
            {
                Id = request.Id,
                RedemptionId = request.RedemptionId,
                UserId = record.UserId,
                EmployeeId = user.EmployeeId.Value,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email.Value,
                ProductId = record.ProductId,
                ProductName = product.Name,
                PointsUsed = request.PointsUsed,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                RedeemedAt = record.RedeemedAt
            });
        }

        return result;
    }
}
