using System;
using System.Collections.Generic;

namespace MovieApp.Models
{
    public class FilmModel
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        public int? ReleaseYear { get; set; }
        public string Rating { get; set; }
        public int? Runtime { get; set; }
    }
}
