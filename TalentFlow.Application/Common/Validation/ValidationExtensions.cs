using FluentValidation;

namespace TalentFlow.Application.Common.Validation
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates that an email contains '@' and ends with .com, .co, or .uk
        /// </summary>
        public static IRuleBuilderOptions<T, string> ValidTalentFlowEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required")
                .Matches(@"^[^@\s]+@[^@\s]+\.(com|co|uk)$")
                .WithMessage("Email must be a valid address ending with .com, .co, or .uk");
        }
    }
}
