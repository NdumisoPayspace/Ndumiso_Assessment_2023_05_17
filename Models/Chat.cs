namespace Ndumiso_Assessment_2023_05_17.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UserQuestion { get; set; }
        public string? Answer { get; set; }
    }
}
