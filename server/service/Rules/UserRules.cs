using System.Net.Mail;
using Contracts.UserDTOs;
using Contracts.Validation;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using dataaccess.Entities.Enums;

namespace service.Rules
{
    public class UserRules : IUserRules
    {
        private readonly IUserRepository _repo;

        public UserRules(IUserRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task ValidateCreateAsync(RegisterUserDto createDto)
        {
            try
            {
                if (createDto == null) throw new InvalidRequestException("Create DTO must be provided.");

                if (string.IsNullOrWhiteSpace(createDto.Email))
                    throw new InvalidRequestException("Email must be provided.");

                if (!IsValidEmail(createDto.Email))
                    throw new RangeValidationException("Invalid email format.");
                
                if(string.IsNullOrWhiteSpace(createDto.Firstname))
                    throw new InvalidRequestException("First name must be provided.");
                
                if (string.IsNullOrWhiteSpace(createDto.Lastname))
                    throw new InvalidRequestException("Last name must be provided.");
                
                if (string.IsNullOrWhiteSpace(createDto.Role))
                    throw new InvalidRequestException("Role must be provided.");

                if (!Enum.TryParse<UserRole>(createDto.Role, true, out var parsedRole) ||
                    !Enum.IsDefined(typeof(UserRole), parsedRole))
                    throw new RangeValidationException("Invalid role value.");


                if (string.IsNullOrWhiteSpace(createDto.Password))
                    throw new InvalidRequestException("Suitable password must be provided.");

                var pwdAttr = new PasswordComplexityAttribute();
                if (!pwdAttr.IsValid(createDto.Password))
                    throw new RangeValidationException(pwdAttr.ErrorMessage);

                var emailInUse = await _repo.AsQueryable()
                    .AnyAsync(u => u.Email == createDto.Email);
                if (emailInUse)
                    throw new DuplicateResourceException($"Email '{createDto.Email}' is already registered.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to validate user creation rules.", ex);
            }
        }

        public async Task ValidateUpdateAsync(string id, UpdateUserDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id must be provided for update.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"User with id '{id}' does not exist.");

                if (updateDto == null) return;

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    if (!IsValidEmail(updateDto.Email))
                        throw new RangeValidationException("Invalid email format.");

                    var conflict = await _repo.AsQueryable()
                        .AnyAsync(u => u.UserID != id && u.Email == updateDto.Email);
                    if (conflict)
                        throw new DuplicateResourceException($"Email '{updateDto.Email}' is already in use by another user.");
                }
                

                if (updateDto.Balance.HasValue && updateDto.Balance.Value < 0)
                    throw new RangeValidationException("Balance cannot be negative.");

                if (updateDto.Role.HasValue && !Enum.IsDefined(typeof(UserRole), updateDto.Role.Value))
                    throw new RangeValidationException("Invalid role value.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to validate user update rules for id '{id}'.", ex);
            }
        }

        public async Task ValidateDeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Id must be provided for delete.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"User with id '{id}' does not exist.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to validate user delete rules for id '{id}'.", ex);
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}