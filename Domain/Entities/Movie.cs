namespace Domain
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; } 
        public string Year { get; set; } 
        public string Genres { get; set; } 
        public bool Disable { get; set; }
        public DateTime? CreatedDate { get; set; }
        public ICollection<Review>? Reviews { get; set; } 
    }
}