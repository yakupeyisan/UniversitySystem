using AutoMapper;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ActivateRoleCommand : IRequest<Result<RoleDto>>
{
    public Guid RoleId { get; set; }

    public ActivateRoleCommand(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        RoleId = roleId;
    }

    public class Handler : IRequestHandler<ActivateRoleCommand, Result<RoleDto>>
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
            ActivateRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Activating role: {RoleId}", request.RoleId);

                var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }

                role.Activate();
                await _roleRepository.UpdateAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role activated successfully");
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating role");
                return Result<RoleDto>.Failure("An unexpected error occurred while activating role");
            }
        }
    }
}