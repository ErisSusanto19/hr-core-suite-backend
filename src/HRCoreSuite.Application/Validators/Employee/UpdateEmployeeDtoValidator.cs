using FluentValidation;
using HRCoreSuite.Application.DTOs.Employee;
using HRCoreSuite.Application.Interfaces.Persistence;

namespace HRCoreSuite.Application.Validators.Employee;

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Nama pegawai tidak boleh kosong.");
        RuleFor(x => x.EmployeeNumber).NotEmpty().WithMessage("NIP tidak boleh kosong.");
        RuleFor(x => x.ContractEndDate)
            .GreaterThanOrEqualTo(x => x.ContractStartDate)
            .WithMessage("Tanggal akhir kontrak harus setelah atau sama dengan tanggal mulai.");
    }
}