namespace MovieApp.Entities {
    public class FilmImage
    {
        public int FilmImageId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public int FilmId { get; set; }
        public Film Film { get; set; }
    }
}