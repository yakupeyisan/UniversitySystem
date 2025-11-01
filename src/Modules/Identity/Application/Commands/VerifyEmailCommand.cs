using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Commands;
public class VerifyEmailCommand : IRequest<Result<UserDto>>
{
    public VerifyEmailCommand(Guid userId, string verificationCode)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(verificationCode))
            throw new ArgumentException("Verification code cannot be empty", nameof(verificationCode));
        UserId = userId;
        VerificationCode = verificationCode.Trim();
    }
    public Guid UserId { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public class Handler : IRequestHandler<VerifyEmailCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<User>
            _userRepository;
        public Handler(
            IRepository<User>
                userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<UserDto>> Handle(
            VerifyEmailCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Verifying email for user: {UserId}", request.UserId);
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }
                if (user.IsEmailVerified)
                {
                    _logger.LogWarning("Email already verified for user: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("Email is already verified");
                }
                user.VerifyEmail();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Email verified successfully for user: {UserId}", request.UserId);
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while verifying email");
            }
        }
    }
}