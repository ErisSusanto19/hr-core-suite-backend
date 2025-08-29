using FluentValidation;
using HRCoreSuite.Application.DTOs.Employee;

namespace HRCoreSuite.Application.Validators.Employee;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama pegawai tidak boleh kosong.");

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty().WithMessage("NIP tidak boleh kosong.");

        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.PositionId).NotEmpty();
    }
}