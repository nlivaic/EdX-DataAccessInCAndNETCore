using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ConsoleTables;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;

namespace MovieApp
{
    public static class Module2Helper
    {
        public static void Sort()
        {
            /* Sort actors. */
            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.OrderBy(a => a.LastName)
                                                                            .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();

            /* Sort films. */
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.OrderBy(f => f.Rating)
                                                                        .ThenBy(f => f.ReleaseYear)
                                                                        .ThenBy(f => f.Title)
                                                                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void SortDescending()
        {
            /* Sort actors by descending. */
            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.OrderByDescending(a => a.LastName)
                                                                          .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();

            /* Sort movies by descending then ascending. */
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.OrderByDescending(f => f.Rating)
                                                                       .ThenBy(f => f.ReleaseYear)
                                                                       .Select(a => a.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void Skip()
        {
            /* Skip actors. */
            IEnumerable<FilmModel> actors = MoviesContext.Instance.Films.OrderBy(f => f.Title)
                                                                        .Skip(3)
                                                                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(actors).Write();
        }
        public static void Take()
        {
            /* Skip actors. */
            IEnumerable<FilmModel> actors = MoviesContext.Instance.Films.OrderBy(f => f.Title)
                                                                        .Take(5)
                                                                        .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(actors).Write();
        }
        public static void Paging()
        {
            Console.WriteLine("Enter page size: ");
            int pageSize = Console.ReadLine().ToInt();
            Console.WriteLine("Enter page number: ");
            int pageNr = Console.ReadLine().ToInt();
            Console.WriteLine("Enter a sort column:");
            Console.WriteLine("\ti - Film ID");
            Console.WriteLine("\tt - Title");
            Console.WriteLine("\ty - Year");
            Console.WriteLine("\tr - Rating");
            ConsoleKeyInfo key = Console.ReadKey();
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.OrderBy(GetSort(key))
                                                                       .Skip((pageNr - 1) * pageSize)
                                                                       .Take(pageSize)
                                                                       .Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        private static Expression<Func<Film, object>> GetSort(ConsoleKeyInfo keyInfo) {
            switch (keyInfo.Key) {
                case ConsoleKey.I:
                    return f => f.FilmId;
                case ConsoleKey.R:
                    return f => f.Rating;
                case ConsoleKey.Y:
                    return f => f.ReleaseYear;
                default:
                    return f => f.Title;
            }
        }
        public static void LinqBasics()
        {
            IEnumerable<ActorModel> actors = from a in MoviesContext.Instance.Actors
                                            where a.FirstName.Contains("ar")
                                          orderby a.FirstName descending
                                           select a.Copy<Actor, ActorModel>();
            ConsoleTable.From(actors).Write();
        }
        public static void LambdaBasics()
        {
            // ...            
        }
        public static void LinqVsLambda()
        {
            Console.WriteLine("-----------------------GROUP---------------------------");
            var actorGroups = MoviesContext.Instance.Actors
                                               .GroupBy(a => a.FirstName[0]);
            foreach (var actorGroup in actorGroups)
            {
                Console.WriteLine($"Group key: {actorGroup.Key}");
                foreach (var actor in actorGroup) {
                    Console.WriteLine($"\tActor: {actor.LastName}");
                }
            }
            Console.WriteLine("-----------------------JOIN Linq query syntax---------------------------");
            var ratings = new[] {
            new { Code = "G", Name = "General Audiences"},
            new { Code = "PG", Name = "Parental Guidance Suggested"},
            new { Code = "PG-13", Name = "Parents Strongly Cautioned"},
            new { Code = "R", Name = "Restricted"},
            };

            var filmRatings = from f in MoviesContext.Instance.Films
                              join r in ratings on f.Rating equals r.Code
                            select new { f.Title, r.Code, r.Name };
            ConsoleTable.From(filmRatings).Write();

            Console.WriteLine("-----------------------JOIN Linq method syntax--------------------------");
            filmRatings = MoviesContext.Instance.Films.Join(ratings, 
                                                                f => f.Rating,
                                                                r => r.Code,
                                                                (f, r) => new { f.Title, r.Code, r.Name });
            ConsoleTable.From(filmRatings).Write();
        }
        public static void MigrationAddColumn()
        {
            Film film = MoviesContext.Instance.Films.Where(f => f.Title.Contains("first avenger")).FirstOrDefault();
            if (film != null) {
                Console.WriteLine("Please enter movie runtime:");
                int runtime = Console.ReadLine().ToInt();
                film.Runtime = runtime;
                MoviesContext.Instance.SaveChanges();
            }
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
        public static void MigrationAddTable()
        {
            ApplicationUser user = new ApplicationUser {
                UserName = "testuser",
                InvalidLoginAttempts = 0
            };
            MoviesContext.Instance.ApplicationUsers.Add(user);
            MoviesContext.Instance.SaveChanges();
            
            var users = MoviesContext.Instance.ApplicationUsers;
            ConsoleTable.From(users).Write();
        }
        public static void CompositeKeys()
        {
            var data = new[] {
                new FilmInfo { Title = "Thor", ReleaseYear = 2011, Rating = "PG-13" },
                new FilmInfo { Title = "The Avengers", ReleaseYear = 2012, Rating = "PG-13" },
                new FilmInfo { Title = "Rogue One", ReleaseYear = 2016, Rating = "PG-13" }
            };
            MoviesContext.Instance.FilmInfos.AddRange(data);
            MoviesContext.Instance.SaveChanges();
        }
    }
}