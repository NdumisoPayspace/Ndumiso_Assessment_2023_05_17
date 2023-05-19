namespace Ndumiso_Assessment_2023_05_17.Validators
{
    using FluentValidation;

    using Ndumiso_Assessment_2023_05_17.Models;
    public class ImageValidator : AbstractValidator<Image>
    {
        public ImageValidator()
        {
            this.RuleFor(_ => _.Description).NotNull();
        }
    }
}
