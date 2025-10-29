namespace Academic.Application.DTOs;

public class UpdateGradeRequest
{
    public Guid GradeId { get; set; }
    public float MidtermScore { get; set; }
    public float FinalScore { get; set; }
}