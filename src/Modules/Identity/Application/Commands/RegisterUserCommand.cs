using AutoMapper;
using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using Identity.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Commands;
public class RegisterUserCommand : IRequest<Result<UserDto>>
{
    public RegisterUserCommand(RegisterUserRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public RegisterUserRequest Request { get; set; }
    public class Handler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<User>
            _userRepository;
        public Handler(
            IRepository<User>
                userRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            ILogger<Handler> logger, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService;
        }
        public async Task<Result<UserDto>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to register new user with email: {Email}",
                    request.Request.Email);
                if (await _userRepository.ExistsAsync(new UserByEmailSpecification(request.Request.Email),
                        cancellationToken))
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Request.Email);
                    return Result<UserDto>.Failure($"Email '{request.Request.Email}' is already registered");
                }
                if (!_passwordHasher.ValidatePasswordStrength(request.Request.Password))
                {
                    _logger.LogWarning("Password does not meet strength requirements");
                    return Result<UserDto>.Failure(
                        $"Password does not meet requirements: {_passwordHasher.GetPasswordRequirements()}");
                }
                var (hashedPassword, salt) = _passwordHasher.HashPassword(request.Request.Password);
                var user = User.Create(
                    request.Request.Email,
                    request.Request.FirstName,
                    request.Request.LastName,
                    hashedPassword,
                    salt,
                    _currentUserService.UserId
                );
                await _userRepository.AddAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("User successfully registered with email: {Email}", request.Request.Email);
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error during user registration");
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration");
                return Result<UserDto>.Failure("An unexpected error occurred during registration");
            }
        }
    }
}