﻿using System;
using System.Collections.Generic;
using Entities;

namespace MovieApp.Entities
{
    public partial class Film
    {
        public Film()
        {
            FilmActor = new HashSet<FilmActor>();
            FilmCategory = new HashSet<FilmCategory>();
        }

        public int FilmId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ReleaseYear { get; set; }
        public string RatingCode { get; set; }
        public int? Runtime { get; set; }
        public int? FilmImageId { get; set; }
        public int? RatingId { get; set; }

        public ICollection<FilmActor> FilmActor { get; set; }
        public ICollection<FilmCategory> FilmCategory { get; set; }
        public FilmImage FilmImage { get; set; }
        public Rating Rating { get; set; }
    }
}
