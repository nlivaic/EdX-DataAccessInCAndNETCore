using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieApp
{
    public static class Module3Helper
    {
        public static void OneToOne()
        {
            Console.WriteLine("-------------------ALL FILMS AND FILM IMAGES ---------------------");
            IEnumerable<FilmDetailModel> model = MoviesContext.Instance.Films.Include(f => f.FilmImage)
                                                                             .Select(CreateFilmDetailModel);
            ConsoleTable.From(model).Write();

            Console.WriteLine("-------------------ONLY FILMS WITHOUT FILM IMAGES ---------------------");
            model = MoviesContext.Instance.Films.Include(f => f.FilmImage)
                                                .Where(f => f.FilmImage == null)
                                                .Select(CreateFilmDetailModel);
            ConsoleTable.From(model).Write();

            Console.WriteLine("-------------------ONLY FILMS IMAGES ---------------------");
            model = MoviesContext.Instance.FilmImages.Include(fi => fi.Film)
                                                     .Select(CreateFilmDetailModel);
            ConsoleTable.From(model).Write();
        }

        private static FilmDetailModel CreateFilmDetailModel(Film film)
        {
            FilmDetailModel model = film.Copy<Film, FilmDetailModel>();
            if (film.FilmImage != null) {
                model.FilmImageId = film.FilmImage.FilmImageId;
            }
            return model;
        }

        private static FilmDetailModel CreateFilmDetailModel(FilmImage filmImage)
        {
            FilmDetailModel model = filmImage.Film.Copy<Film, FilmDetailModel>();
            model.FilmImageId = filmImage.FilmImageId;
            return model;
        }

        public static void OneToMany()
        {
            Console.WriteLine("-------------------------------- ALL RATINGS AND ASSOCIATED FILMS ----------------------------------");
            IEnumerable<Rating> ratings = MoviesContext.Instance.Ratings.Include(r => r.Films);
            foreach (Rating rating in ratings)
            {
                Console.WriteLine($"Rating: {rating.RatingId}/{rating.Code}/{rating.Name}");
                if (rating.Films.Any()) {
                    foreach (Film film in rating.Films) {
                        Console.WriteLine($"\tFilm: {film.FilmId}/{film.Title}");
                    }
                } else {
                    Console.WriteLine($"\tNo films.");
                }
            }
            Console.WriteLine("-------------------------------- SINGLE RATING AND ASSOCIATED FILMS ----------------------------------");
            string searchRatingCode = "PG";
            Rating searchRating = MoviesContext.Instance.Ratings.Where(r => r.Code == searchRatingCode).SingleOrDefault();
            IEnumerable<Film> filmsByRating = MoviesContext.Instance.Films.Include(f => f.Rating)
                                                        .Where(f => f.RatingId == searchRating.RatingId)
                                                        .OrderBy(f => f.ReleaseYear);
            Console.WriteLine($"Rating: Rating: {searchRating.RatingId}/{searchRating.Code}/{searchRating.Name}");
            foreach (Film film in filmsByRating.OrderByDescending(f => f.ReleaseYear)) {
                Console.WriteLine($"\tFilm: {film.FilmId}/{film.Title}");
            }
            Console.WriteLine("--------------------------------------- GET RATING FROM FILM -----------------------------------------");
            int filmId = 1;
            Film searchFilm = MoviesContext.Instance.Films.Include(f => f.Rating)
                                                    .Where(f => f.FilmId == filmId).SingleOrDefault();
            Console.WriteLine($"Film: {searchFilm.FilmId}/{searchFilm.Title}");
            Console.WriteLine($"\tRating: Rating: {searchFilm.Rating.RatingId}/{searchFilm.Rating.Code}/{searchFilm.Rating.Name}");
        }

        public static void ManyToManySelect()
        {
            Console.WriteLine("--------------------------------------- LIST FILMS AND ACTORS -----------------------------------------");
            IEnumerable<Film> films = MoviesContext.Instance.Films
                                                            .Include(f => f.FilmActor)
                                                            .ThenInclude(fa => fa.Actor);
            foreach (Film film in films)
            {
                Console.WriteLine($"Film: {film.Title}");
                foreach (FilmActor filmActor in film.FilmActor)
                {
                    Console.WriteLine($"\tActor: {filmActor.Actor.FirstName} {filmActor.Actor.LastName}");
                }
            }
            Console.WriteLine("-------------------------------------- SORTED FILMS AND ACTORS ----------------------------------------");
            films = MoviesContext.Instance.Films.OrderBy(f => f.Title)
                                                .Include(f => f.FilmActor)
                                                .ThenInclude(fa => fa.Actor);
            foreach (Film film in films)
            {
                Console.WriteLine($"Film: {film.Title}");
                foreach (FilmActor filmActor in film.FilmActor.OrderBy(fa => fa.Actor.LastName).ThenBy(fa => fa.Actor.FirstName))
                {
                    Console.WriteLine($"\tActor: {filmActor.Actor.FirstName} {filmActor.Actor.LastName}");
                }
            }
            Console.WriteLine("----------------------------------------- ACTORS AND FILMS --------------------------------------------");
            IEnumerable<Actor> actors = MoviesContext.Instance.Actors.Include(a => a.FilmActor)
                                                                     .ThenInclude(fa => fa.Film);
            foreach (Actor actor in actors)
            {
                Console.WriteLine($"Actor: {actor.FirstName} {actor.LastName}");
                foreach (FilmActor filmActor in actor.FilmActor)
                {
                    Console.WriteLine($"\tFilm: {filmActor.Film.Title}");
                }
            }
            Console.WriteLine("----------------------------------------- SORTED ACTORS AND FILMS --------------------------------------------");
            actors = MoviesContext.Instance.Actors.OrderBy(a => a.LastName)
                                                  .ThenBy(a => a.FirstName)
                                                  .Include(a => a.FilmActor)
                                                  .ThenInclude(fa => fa.Film);
            foreach (Actor actor in actors)
            {
                Console.WriteLine($"Actor: {actor.FirstName} {actor.LastName}");
                foreach (FilmActor filmActor in actor.FilmActor.OrderByDescending(fa => fa.Film.ReleaseYear))
                {
                    Console.WriteLine($"\tFilm: {filmActor.Film.ReleaseYear} {filmActor.Film.Title}");
                }
            }
        }

        public static void ManyToManyInsert()
        {
            Console.WriteLine("----------------------------------------- ADD EXISTING ACTOR TO FILM --------------------------------------------");
            int filmId = 12;
            int actorId = 2;
            Film film = MoviesContext.Instance.Films.Include(f => f.FilmActor).Single(f => f.FilmId == filmId);
            Actor actor = MoviesContext.Instance.Actors.Single(a => a.ActorId == actorId);
            if (film.FilmActor.All(fa => fa.ActorId != actorId))
            {
                FilmActor filmActor = new FilmActor {
                    Actor = actor,
                    Film = film
                };
                MoviesContext.Instance.FilmActors.Add(filmActor);
                MoviesContext.Instance.SaveChanges();
            }
            Film film1 = MoviesContext.Instance.Films.Include(f => f.FilmActor)
                                                    .ThenInclude(fa => fa.Actor)
                                                    .SingleOrDefault(f => f.FilmId == filmId);
            Console.WriteLine($"Film: {film1.Title}");
            foreach (FilmActor filmActor1 in film1.FilmActor)
            {
                Console.WriteLine($"Actor: {filmActor1.Actor.ActorId} {filmActor1.Actor.LastName} {filmActor1.Actor.FirstName}");
            }
            Console.WriteLine("----------------------------------------- ADD NEW ACTOR TO FILM --------------------------------------------");
            filmId = 12;
            actor = new Actor { FirstName = "Stan", LastName = "Lee" };
            film = MoviesContext.Instance.Films.Include(f => f.FilmActor)
                                               .ThenInclude(fa => fa.Actor)
                                               .Single(f => f.FilmId == filmId);
            if (film.FilmActor.All(fa => fa.ActorId != actor.ActorId))
            {
                film.FilmActor.Add(new FilmActor {
                    Actor = actor,
                    Film = film
                });
                MoviesContext.Instance.SaveChanges();
            }
            film = MoviesContext.Instance.Films.Include(f => f.FilmActor)
                                               .ThenInclude(fa => fa.Actor)
                                               .Single(f => f.FilmId == filmId);
            Console.WriteLine($"Film: {film1.Title}");
            foreach (FilmActor filmActor1 in film.FilmActor)
            {
                Console.WriteLine($"Actor: {filmActor1.Actor.LastName} {filmActor1.Actor.FirstName}");
            }
            Console.WriteLine("----------------------------------------- ADD EXISTING FILM TO ACTOR --------------------------------------------");
            filmId = 12;
            actorId = 3;
            actor = MoviesContext.Instance.Actors.Include(a => a.FilmActor)
                                                 .Single(a => a.ActorId == actorId);
            if (actor.FilmActor.All(fa => fa.FilmId != filmId))
            {
                FilmActor filmActor = new FilmActor {
                    FilmId = filmId,    // Note we are using only film id, not the film itself. 
                                        // This saves querying the film (as opposed to actor example two sections earlier).
                    Actor = actor
                };
                actor.FilmActor.Add(filmActor);
                MoviesContext.Instance.SaveChanges();
            }
            actor = MoviesContext.Instance.Actors.Include(a => a.FilmActor)
                                                 .ThenInclude(fa => fa.Film)
                                                 .Single(a => a.ActorId == actorId);
            Console.WriteLine($"Actor: {actor.ActorId} {actor.FirstName} {actor.LastName}");
            foreach (FilmActor filmActor1 in actor.FilmActor.OrderByDescending(fa => fa.FilmId))
            {
                Console.WriteLine($"\t{filmActor1.FilmId} {filmActor1.Film.FilmId} {filmActor1.Film.Title}");
            }
        }

        public static void ManyToManyDelete()
        {
            Console.WriteLine(nameof(ManyToManyDelete));
        }
    }
}