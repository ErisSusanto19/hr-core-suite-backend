using FluentValidation;
using HRCoreSuite.Application.DTOs.Position;

namespace HRCoreSuite.Application.Validators.Position;

public class CreatePositionDtoValidator : AbstractValidator<CreatePositionDto>
{
    public CreatePositionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Properti 'name' wajib ada.")
            .NotEmpty().WithMessage("Nama jabatan tidak boleh kosong.")
            .MaximumLength(100).WithMessage("Nama jabatan tidak boleh lebih dari 100 karakter.");
    }
}