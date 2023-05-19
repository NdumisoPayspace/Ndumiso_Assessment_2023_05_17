namespace Ndumiso_Assessment_2023_05_17.Validators
{
    using FluentValidation;

    using Ndumiso_Assessment_2023_05_17.Models;

    public class ChatValidator : AbstractValidator<Chat>
    {
        public ChatValidator()
        {
            this.RuleFor(_ => _.UserQuestion).NotNull();
        }
    }
}
