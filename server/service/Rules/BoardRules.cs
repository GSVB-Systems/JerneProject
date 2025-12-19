using System.ComponentModel.DataAnnotations;
using Contracts.BoardDTOs;
using Sieve.Models;
using service.Exceptions;

namespace service.Rules
{
    public class BoardRules : IBoardRules
    {
        public Task ValidateGetByIdAsync(string? id)
        {
            if (id == null) throw new InvalidRequestException("id er påkrævet.");
            if (string.IsNullOrWhiteSpace(id)) throw new InvalidRequestException("id må ikke være tomt eller kun bestå af mellemrum.");
            return Task.CompletedTask;
        }

        public Task ValidateGetAllAsync(SieveModel? parameters)
        {
            if (parameters == null) return Task.CompletedTask;
            if (parameters.Page.HasValue && parameters.Page.Value <= 0)
                throw new RangeValidationException("Page skal være større end 0.");
            if (parameters.PageSize.HasValue && parameters.PageSize.Value < 0)
                throw new RangeValidationException("PageSize skal være større end eller lig med 0.");
            return Task.CompletedTask;
        }

        public Task ValidateCreateAsync(CreateBoardDto createDto)
        {
            if (createDto == null) throw new InvalidRequestException("createDto er påkrævet.");
            if (createDto.Numbers == null) throw new ValidationException("Tal er påkrævet.");
            if (createDto.BoardSize <= 0) throw new ValidationException("BoardSize skal være større end 0.");
            if (createDto.Numbers.Count != createDto.BoardSize)
                throw new ValidationException("Antallet af tal skal være lig med BoardSize.");
            if (createDto.Week <= 0) throw new ValidationException("Week skal være mindst 1.");
            return Task.CompletedTask;
        }

        public Task ValidateUpdateAsync(string? id, UpdateBoardDto? updateDto)
        {
            if (id == null) throw new InvalidRequestException("id er påkrævet.");
            if (string.IsNullOrWhiteSpace(id)) throw new InvalidRequestException("id må ikke være tomt eller kun bestå af mellemrum.");

            if (updateDto?.Numbers != null && updateDto.BoardSize.HasValue)
            {
                var expected = updateDto.BoardSize.Value;
                if (updateDto.Numbers.Count != expected)
                    throw new ValidationException("Antallet af tal skal matche BoardSize, når den er angivet.");
            }

            return Task.CompletedTask;
        }

        public Task ValidateDeleteAsync(string? id)
        {
            if (id == null) throw new InvalidRequestException("id er påkrævet.");
            if (string.IsNullOrWhiteSpace(id)) throw new InvalidRequestException("id må ikke være tomt eller kun bestå af mellemrum.");
            return Task.CompletedTask;
        }
        
        public Task ValidateGetAllByUserIdAsync(string? userId, BoardQueryParameters? parameters)
        {
            if (userId == null) throw new InvalidRequestException("userId er påkrævet.");
            if (string.IsNullOrWhiteSpace(userId)) throw new InvalidRequestException("userId må ikke være tomt eller kun bestå af mellemrum.");

            if (parameters == null) return Task.CompletedTask;
            if (parameters.Page.HasValue && parameters.Page.Value <= 0)
                throw new RangeValidationException("Page skal være større end 0.");
            if (parameters.PageSize.HasValue && parameters.PageSize.Value < 0)
                throw new RangeValidationException("PageSize skal være større end eller lig med 0.");

            return Task.CompletedTask;
        }
    }
}