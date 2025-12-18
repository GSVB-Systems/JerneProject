using System.Net.Mail;
using Contracts.UserDTOs;
using Contracts.Validation;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using service.Repositories.Interfaces;
using service.Rules.RuleInterfaces;
using dataaccess.Entities.Enums;
using Sieve.Models;

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
                if (createDto == null) throw new InvalidRequestException("RegistrationDTO skal medtages.");

                if (string.IsNullOrWhiteSpace(createDto.Email))
                    throw new InvalidRequestException("Angiv Email adresse.");

                if (!IsValidEmail(createDto.Email))
                    throw new RangeValidationException("Angiv en gyldig Email adresse.");
                
                if(string.IsNullOrWhiteSpace(createDto.Firstname))
                    throw new InvalidRequestException("Der skal angives et fornavn.");
                
                if (string.IsNullOrWhiteSpace(createDto.Lastname))
                    throw new InvalidRequestException("Der skal angives et efternavn.");
                
                if (string.IsNullOrWhiteSpace(createDto.Role))
                    throw new InvalidRequestException("angiv en rolle for brugeren.");

                if (!Enum.TryParse<UserRole>(createDto.Role, true, out var parsedRole) ||
                    !Enum.IsDefined(typeof(UserRole), parsedRole))
                    throw new RangeValidationException("Rollen findes ikke i systemet.");


                if (string.IsNullOrWhiteSpace(createDto.Password))
                    throw new InvalidRequestException("Angiv et password.");

                var pwdAttr = new PasswordComplexityAttribute();
                if (!pwdAttr.IsValid(createDto.Password))
                    throw new RangeValidationException(pwdAttr.ErrorMessage);

                var emailInUse = await _repo.AsQueryable()
                    .AnyAsync(u => u.Email == createDto.Email);
                if (emailInUse)
                    throw new DuplicateResourceException($"Emailen '{createDto.Email}' findes allerede i systemet.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke oprette bruger, da den ikke opfylder alle krav.", ex);
            }
        }

        public async Task ValidateUpdateAsync(string id, UpdateUserDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("angiv et id for opdatering.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Brugeren med ID: '{id}' findes ikke.");

                if (updateDto == null) return;

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    if (!IsValidEmail(updateDto.Email))
                        throw new RangeValidationException("Angiv en gyldig Email adresse.");

                    var conflict = await _repo.AsQueryable()
                        .AnyAsync(u => u.UserID != id && u.Email == updateDto.Email);
                    if (conflict)
                        throw new DuplicateResourceException($"Emailen '{updateDto.Email}' er allerede i brug af en anden bruger.");
                }
                

                if (updateDto.Balance.HasValue && updateDto.Balance.Value < 0)
                    throw new RangeValidationException("Balance kan ikke være negativ.");

                if (updateDto.Role.HasValue && !Enum.IsDefined(typeof(UserRole), updateDto.Role.Value))
                    throw new RangeValidationException("invalid rolle angivet for brugeren.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere brugeren : {id}", ex);
            }
        }

        public async Task ValidateDeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("angiv et id for sletning.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Brugeren med ID: '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere bruger til sletning.", ex);
            }
        }

        public async Task ValidateUpdateBalanceAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for opdatering af balance.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"brugeren med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke validere regler for opdatering af balancen.", ex);
            }
        }

        public async Task ValidateExtendSubscriptionAsync(string id, int months)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for forlængelse af abonnement.");

                if (months <= 0)
                    throw new RangeValidationException("Måneder til forlængelse skal være større end nul.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke forlænge abonomentet for brugeren.", ex);
            }
        }

        public async Task ValidateIsSubscriptionActiveAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for at tjekke om abonnement er aktivt.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"kunne ikke validere for brugeren med id : {id}.", ex);
            }
        }
        
        public async Task ValidateGetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new InvalidRequestException("Angiv et id for at hente brugeren.");

                var exists = await _repo.AsQueryable()
                    .AnyAsync(u => u.UserID == id);
                if (!exists)
                    throw new ResourceNotFoundException($"Brugeren med id '{id}' findes ikke.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Kunne ikke finde brugeren", ex);
            }
        }

        public async Task ValidateGetAllAsync(SieveModel? parameters)
        {
            try
            {
                if (parameters == null) return;

                if (parameters.Page.HasValue && parameters.Page.Value <= 0)
                    throw new RangeValidationException("Side nummer skal være større end nul.");

                if (parameters.PageSize.HasValue && (parameters.PageSize.Value <= 0 || parameters.PageSize.Value > 1000))
                    throw new RangeValidationException("PageSize skal være mellem 1 og 1000.");
            }
            catch (RuleValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Kunne ikke hente bruger listen.", ex);
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