using System.Collections.Generic;

namespace MovieApp.Entities {
    public class Rating
    {
        public int RatingId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<Film> Films { get; set; }
    }
}