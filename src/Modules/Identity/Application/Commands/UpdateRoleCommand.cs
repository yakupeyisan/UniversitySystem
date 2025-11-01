using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Commands;
public class UpdateRoleCommand : IRequest<Result<RoleDto>>
{
    public UpdateRoleCommand(Guid roleId, string name, string description)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));
        RoleId = roleId;
        Name = name;
        Description = description?.Trim() ?? string.Empty;
    }
    public Guid RoleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public class Handler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Role>
            _roleRepository;
        public Handler(
            IRepository<Role>
                roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<RoleDto>> Handle(
            UpdateRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating role: {RoleId}", request.RoleId);
                var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                    return Result<RoleDto>.Failure("Role not found");
                }
                role.UpdateDescription(request.Description);
                await _roleRepository.UpdateAsync(role, cancellationToken);
                await _roleRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Role updated successfully");
                return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", request.RoleId);
                return Result<RoleDto>.Failure("An unexpected error occurred while updating role");
            }
        }
    }
}