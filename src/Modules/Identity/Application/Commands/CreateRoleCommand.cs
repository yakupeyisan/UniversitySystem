using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class CreateRoleCommand : IRequest<Result<RoleDto>>
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RoleType RoleType { get; set; }

    public CreateRoleCommand(string roleName, string description, RoleType roleType)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));

        RoleName = roleName.Trim();
        Description = description?.Trim() ?? string.Empty;
        RoleType = roleType;
    }

    public class Handler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RoleDto>> Handle(
            CreateRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating new role: {RoleName}", request.RoleName);

                var role = Role.Create(request.RoleName, request.RoleType, request.Description);
                await _roleRepository.AddAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role created successfully: {RoleId}", role.Id);
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", request.RoleName);
                return Result<RoleDto>.Failure("An unexpected error occurred while creating role");
            }
        }
    }
}