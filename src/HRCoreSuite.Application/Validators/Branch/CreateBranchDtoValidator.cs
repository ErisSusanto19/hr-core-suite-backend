using FluentValidation;
using HRCoreSuite.Application.DTOs.Branch;

namespace HRCoreSuite.Application.Validators.Branch;

public class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
{
    public CreateBranchDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Properti 'name' wajib ada.")
            .NotEmpty().WithMessage("Nama cabang tidak boleh kosong.")
            .MaximumLength(100).WithMessage("Nama cabang tidak boleh lebih dari 100 karakter.");
    }
}