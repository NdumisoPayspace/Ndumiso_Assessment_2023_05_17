using FluentValidation;
using Ndumiso_Assessment_2023_05_17.Models;

namespace Ndumiso_Assessment_2023_05_17.Validators
{
    public class TextValidator : AbstractValidator<Text>
    {
        public TextValidator()
        {
            this.RuleFor(_ => _.UserText).NotNull();
            this.RuleFor(_ => _.Task).NotNull();
        }
    }
}
