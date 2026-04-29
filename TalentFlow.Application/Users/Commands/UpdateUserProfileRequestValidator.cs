//using FluentValidation;

//public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
//{
//    public UpdateUserProfileRequestValidator()
//    {
//        RuleFor(x => x.FullName).MaximumLength(100);
//        RuleFor(x => x.Bio).MaximumLength(1000);
//        RuleFor(x => x.PhoneNumber).Matches(@"^\+?\d{7,15}$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
//        RuleFor(x => x.ProfilePhoto).Must(f => f == null || f.Length <= 5 * 1024 * 1024)
//            .WithMessage("Profile photo must be 5 MB or smaller.");
//        RuleFor(x => x.ProfilePhoto).Must(f => f == null || new[] { "image/jpeg", "image/png", "image/webp" }.Contains(f.ContentType))
//            .WithMessage("Unsupported image type. Allowed: jpeg, png, webp");
//    }
//}
