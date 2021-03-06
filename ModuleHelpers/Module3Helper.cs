using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            Console.WriteLine("---------------------------------------------DELETING WITH ENTITIES---------------------------------------------");
            // Fetch.
            FilmActor filmActor = GetRandomFilmActor();
            int filmId = filmActor.FilmId;
            int actorId = filmActor.ActorId;
            Write(filmActor);
            // Delete.
            FilmActor entity = MoviesContext.Instance.FilmActors.Single(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            MoviesContext.Instance.FilmActors.Remove(entity);
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.SingleOrDefault(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
            // Insert back again.
            MoviesContext.Instance.Add(new FilmActor {
                FilmId = filmId,
                ActorId = actorId
            });
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.SingleOrDefault(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
            
            Console.WriteLine("---------------------------------------------DELETING WITH ONE ENTITY AND ONE ID VALUE---------------------------------------------");
            // We will use the Film and ActorId properties to delete the entity.
            // Fetch.
            filmActor = GetRandomFilmActor();
            filmId = filmActor.FilmId;
            actorId = filmActor.ActorId;
            Write(filmActor);
            // Delete.
            Film film = MoviesContext.Instance.Films.Single(f => f.FilmId == filmId);
            entity = new FilmActor {
                Film = film,
                ActorId = actorId
            };
            MoviesContext.Instance.FilmActors.Remove(entity);
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.SingleOrDefault(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
            // Insert back again.
            filmActor = new FilmActor {
                Film = film,
                ActorId = actorId
            };
            MoviesContext.Instance.FilmActors.Add(filmActor);
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.SingleOrDefault(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
            Console.WriteLine("---------------------------------------------DELETING WITH ID VALUES---------------------------------------------");
            // We will use the FilmId and ActorId properties to delete the entity.
            // Fetch.
            filmActor = GetRandomFilmActor();
            filmId = filmActor.FilmId;
            actorId = filmActor.ActorId;
            Write(filmActor);
            // Delete.
            entity = new FilmActor
            {
                FilmId = filmId,
                ActorId = actorId
            };
            MoviesContext.Instance.FilmActors.Remove(entity);
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.SingleOrDefault(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
            // Insert back again.
            MoviesContext.Instance.FilmActors.Add(
                new FilmActor
                {
                    FilmId = filmId,
                    ActorId = actorId
                }
            );
            MoviesContext.Instance.SaveChanges();
            filmActor = MoviesContext.Instance.FilmActors.Single(fa => fa.ActorId == actorId && fa.FilmId == filmId);
            Write(filmActor);
        }

        private static void Write(FilmActor filmActor)
        {
            if (filmActor == null)
            {
                Console.WriteLine("Film Actor Not Found");
                return;
            }
            Film film = filmActor.Film;
            Actor actor = filmActor.Actor;
            if (film == null)
            {
                film = MoviesContext.Instance.Films.Single(f => f.FilmId == filmActor.FilmId);
            }
            if (actor == null)
            {
                actor = MoviesContext.Instance.Actors.Single(a => a.ActorId == filmActor.ActorId);
            }
            Console.WriteLine($"Film: {film.FilmId}  -  {film.Title}\t Actor: {actor.ActorId}  -  {actor.FirstName} {actor.LastName}");
        }
        private static FilmActor GetRandomFilmActor()
        {
            int count = MoviesContext.Instance.FilmActors.Count();
            var skip = new Random().Next(0, count);
            return MoviesContext.Instance.FilmActors
                        .Skip(skip)
                        .Select(fa => new FilmActor
                        {
                            FilmId = fa.FilmId,
                            ActorId = fa.ActorId
                        })
                        .First();
        }
        public static void LazyLoadFilm()
        {
            Console.WriteLine("---------------------------------------------LAZY LOAD ACTORS FOR FILM---------------------------------------------");
            int filmId = 12;
            Film film = MoviesContext.Instance.Films.Single(f => f.FilmId == filmId);
            MoviesContext.Instance.Entry(film).Collection(f => f.FilmActor).Load();
            Console.WriteLine($"From Film: {film.FilmId} {film.Title}");
            foreach (FilmActor filmActor in film.FilmActor)
            {
                MoviesContext.Instance.Entry(filmActor).Reference(fa => fa.Actor).Load();
                Console.WriteLine($"\tFrom FilmActor: {filmActor.ActorId} {filmActor.FilmId}");
                Console.WriteLine($"\tFrom Actor: {filmActor.Actor.FirstName} {filmActor.Actor.LastName}");
            }
        }

        public static void LazyLoadCategory()
        {
            Console.WriteLine("---------------------------------------------LAZY LOAD FILMS FOR CATEGORIES---------------------------------------------");
            IEnumerable<Category> categories = MoviesContext.Instance.Categories;
            foreach (Category category in categories)
            {
                MoviesContext.Instance.Entry(category).Collection(c => c.FilmCategory).Load();
                Console.WriteLine($"Category: {category.Name}");
                if (category.FilmCategory.Any())
                {
                    foreach (FilmCategory filmCategory in category.FilmCategory)
                    {
                        MoviesContext.Instance.Entry(filmCategory).Reference(fc => fc.Film).Load();
                        Console.WriteLine($"\t{filmCategory.Film.FilmId} - {filmCategory.Film.Title}");
                    }
                }
            }
        }

        public static void EagerLoadFilm()
        {
            
            Console.WriteLine("---------------------------------------------DISPLAY FILM AND ACTORS---------------------------------------------");
            
        }

        public static void EagerLoadCategory()
        {
            var categories = MoviesContext.Instance.Categories.Include(c=>c.FilmCategory)
                                                              .ThenInclude(fc=>fc.Film);
            foreach (var category in categories)
            {
                Console.WriteLine($"Category: {category.CategoryId} - {category.Name}");
                if (category.FilmCategory.Any())
                {
                    foreach (var filmCategory in category.FilmCategory)
                    {
                        MoviesContext.Instance.Entry(filmCategory).Reference(fc => fc.Film);
                        Console.WriteLine($"\t{filmCategory.Film.FilmId} - {filmCategory.Film.Title}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNo Films");
                }
            }
        }

        public static void SelfAssessment()
        {
            int ratingId = 3;
            Rating rating = MoviesContext.Instance.Ratings.Include(r => r.Films)
                                                          .Single(r => r.RatingId == ratingId);
            Console.WriteLine($"Rating: {rating.Name}");
            foreach (Film film in rating.Films)
            {
                film.Title = (film.FilmId % 2 == 1 ? "odd " : "even ") + film.Title;
            }
            MoviesContext.Instance.SaveChanges();
            // Console.WriteLine($"-------------------------------------------CATEGORIES AND MOVIES AFTER CHANGE-----------------------------------------");
            // IEnumerable<Category> categories = MoviesContext.Instance.Categories.Include(c => c.FilmCategory)
            //                                                                     .ThenInclude(fc => fc.Film);
            // foreach (Category category in categories)
            // {
            //     Console.WriteLine($"Category: {category.CategoryId} {category.Name}");
            //     foreach (FilmCategory filmCategory in category.FilmCategory)
            //     {
            //         Console.WriteLine($"\t{filmCategory.Film.FilmId} {filmCategory.Film.Title}");
            //     }
            // }
            Console.WriteLine($"-------------------------------------------RATINGS AND MOVIES AFTER CHANGE-----------------------------------------");
            IEnumerable<Rating> ratings = MoviesContext.Instance.Ratings.Include(r => r.Films);
            foreach (Rating rating1 in ratings)
            {
                Console.WriteLine($"Rating: {rating1.Name}");
                foreach (Film film1 in rating1.Films)
                {
                    Console.WriteLine($"\t{film1.FilmId} {film1.Title}");
                }
            }
            Console.WriteLine($"--------------------------------------REVERT FILM TITLES-----------------------------------");
            rating = MoviesContext.Instance.Ratings.Include(r => r.Films).Single(r => r.RatingId == ratingId);
            foreach (Film film in rating.Films)
            {
                film.Title = Regex.Replace(film.Title, "^odd |^even ", String.Empty);
            }
            MoviesContext.Instance.SaveChanges();
            Console.WriteLine($"-------------------------------------------RATINGS AND MOVIES ORIGINAL-----------------------------------------");
            ratings = MoviesContext.Instance.Ratings.Include(r => r.Films);
            foreach (Rating rating1 in ratings)
            {
                Console.WriteLine($"Rating: {rating1.Name}");
                foreach (Film film1 in rating1.Films)
                {
                    Console.WriteLine($"\t{film1.FilmId} {film1.Title}");
                }
            }
            Console.WriteLine($"----------------------------------------FILMS RELEASED ON EVEN NUMBER YEAR--------------------------------------");
            IEnumerable<Film> evenYearFilms = MoviesContext.Instance.Films.Where(f => f.ReleaseYear % 2 == 0);
            ConsoleTable.From(evenYearFilms).Write();
        }        
    }
}