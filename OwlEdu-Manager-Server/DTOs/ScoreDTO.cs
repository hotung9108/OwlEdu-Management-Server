namespace OwlEdu_Manager_Server.DTOs
{
    //public class ScoreDTO
    //{
    //    public string? Title { get; set; }
    //    public string StudentId { get; set; } = null!;
    //    public string ClassId { get; set; } = null!;
    //    public string? TeacherId { get; set; }
    //    public decimal? Lisening { get; set; }
    //    public decimal? Speaking { get; set; }
    //    public decimal? Reading { get; set; }
    //    public decimal? Writing { get; set; }
    //    public string Type { get; set; } = null!;
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //}
    public class ScoreRequest
    {
        public string? Title { get; set; }
        public string StudentId { get; set; } = null!;
        public string ClassId { get; set; } = null!;
        public string? TeacherId { get; set; }
        public decimal? Lisening { get; set; }
        public decimal? Speaking { get; set; }
        public decimal? Reading { get; set; }
        public decimal? Writing { get; set; }
        public string Type { get; set; } = null!;
    }
    public class ScoreRequestUPDATE
    {
        public decimal? Lisening { get; set; }
        public decimal? Speaking { get; set; }
        public decimal? Reading { get; set; }
        public decimal? Writing { get; set; }
        public string Type { get; set; } = null!;
    }
    public class ScoreResponse
    {
        public string? Title { get; set; }
        public string StudentId { get; set; } = null!;
        public string ClassId { get; set; } = null!;
        public string? TeacherId { get; set; }
        public decimal? Lisening { get; set; }
        public decimal? Speaking { get; set; }
        public decimal? Reading { get; set; }
        public decimal? Writing { get; set; }
        public string Type { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
