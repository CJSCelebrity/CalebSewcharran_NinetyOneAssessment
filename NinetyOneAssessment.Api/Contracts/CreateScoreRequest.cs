using System.ComponentModel.DataAnnotations;

namespace NinetyOneAssessment.Api.Contracts;

public class CreateScoreRequest
{
    [Required(AllowEmptyStrings  = false)]
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string SecondName { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Score must be zero or greater")]
    public int Score { get; set; }
}