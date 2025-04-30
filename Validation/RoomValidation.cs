using Connectify.Db.Model;
using FluentValidation;

namespace GachiHubBackend.Validation;

public class RoomValidation : AbstractValidator<Room>
{
    public RoomValidation()
    {
        RuleFor(r => r.Title)
            .NotEmpty().WithMessage("Название не может быть пустым")
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Название не может содержать спецсимволы");
    }
}