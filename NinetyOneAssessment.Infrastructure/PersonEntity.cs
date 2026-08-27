using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NinetyOneAssessment.Infrastructure;

[Table("People")]
public class PersonEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("First Name")]
    public string? FirstName { get; set; }
    
    [Required]
    [MaxLength(100)]
    [Column("Second Name")]
    public string? SecondName { get; set; }
    
    [Required]
    [Column("Score")]
    public int Score { get; set; }
}