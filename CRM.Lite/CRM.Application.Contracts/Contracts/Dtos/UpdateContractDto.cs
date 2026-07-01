using System.ComponentModel.DataAnnotations;

namespace CRM.Application.Contracts.Contracts.Dtos;

public class UpdateContractDto : CreateContractDto
{
    [Required(ErrorMessage = "合同ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "合同ID无效")]
    public int Id { get; set; }

    public int Status { get; set; }
}
