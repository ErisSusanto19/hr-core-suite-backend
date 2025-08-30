using FluentValidation;
using HRCoreSuite.Application.DTOs.Employee;
using HRCoreSuite.Application.Interfaces.Persistence;

namespace HRCoreSuite.Application.Validators.Employee;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator(IEmployeeRepository employeeRepository, IBranchRepository branchRepository, IPositionRepository positionRepository)
    {
        RuleSet("default", () =>
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Nama pegawai tidak boleh kosong.");
            RuleFor(x => x.EmployeeNumber).NotEmpty().WithMessage("NIP tidak boleh kosong.");
            RuleFor(x => x.ContractEndDate)
                .GreaterThanOrEqualTo(x => x.ContractStartDate)
                .WithMessage("Tanggal akhir kontrak harus setelah atau sama dengan tanggal mulai.");
        });

        RuleSet("async", () =>
        {
            RuleFor(x => x.EmployeeNumber)
                .MustAsync(async (number, token) => number == null || await employeeRepository.IsEmployeeNumberUniqueAsync(number, null, token))
                .WithMessage("NIP sudah terdaftar.");

            RuleFor(x => x.BranchId)
                .MustAsync(async (id, token) => await branchRepository.ExistsAsync(id, token))
                .WithMessage("Cabang yang dipilih tidak valid atau tidak ditemukan.");

            RuleFor(x => x.PositionId)
                .MustAsync(async (id, token) => await positionRepository.ExistsAsync(id, token))
                .WithMessage("Jabatan yang dipilih tidak valid atau tidak ditemukan.");
        });
    }
}