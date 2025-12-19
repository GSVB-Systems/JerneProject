using System.Threading.Tasks;

namespace service.Rules.RuleInterfaces
{
    public interface IBoardMatcherRules
    {
        Task ValidateGetBoardsContainingNumbersAsync(string winningBoardId);
        Task ValidateGetBoardsContainingNumbersWithDecrementerAsync(string winningBoardId);
    }
}

