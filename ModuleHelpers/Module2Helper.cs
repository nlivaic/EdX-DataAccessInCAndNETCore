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
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.OrderBy(f => f.RatingCode)
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
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.OrderByDescending(f => f.RatingCode)
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
                    return f => f.RatingCode;
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
                              join r in ratings on f.RatingCode equals r.Code
                            select new { f.Title, r.Code, r.Name };
            ConsoleTable.From(filmRatings).Write();

            Console.WriteLine("-----------------------JOIN Linq method syntax--------------------------");
            filmRatings = MoviesContext.Instance.Films.Join(ratings, 
                                                                f => f.RatingCode,
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
            // Will throw an exception due to duplicate keys being inserted.
            MoviesContext.Instance.SaveChanges();
        }
        /// <summary>
        /// Enhanced Actors paging system.
        /// Sort by any column in ascending or descending order.
        /// </summary>
        public static void SelfAssessment()
        {
            Console.WriteLine("Page size: ");
            int pageSize = Console.ReadLine().ToInt();
            
            Console.WriteLine("Page number: ");
            int pageNumber = Console.ReadLine().ToInt();

            Console.WriteLine("Sort column: ");
            Console.WriteLine("\ti - Actor Id: ");
            Console.WriteLine("\tf - First Name: ");
            Console.WriteLine("\tl - Last Name: ");
            Func<Actor, object> sortColumn = GetActorSortColumn(Console.ReadKey());

            Console.WriteLine("Enter a sort order: ");
            Console.WriteLine("\ta - Ascending order: ");
            Console.WriteLine("\td - Descending order: ");
            ConsoleKeyInfo orderByDynamicKeyInfo = Console.ReadKey();

            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.OrderByDynamic(sortColumn, orderByDynamicKeyInfo)
                                                                          .Skip(-(pageNumber - 1) * pageSize)
                                                                          .Take(pageSize)
                                                                          .Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
        }

        private static Func<Actor, object> GetActorSortColumn(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.I:
                    return a => a.ActorId;
                case ConsoleKey.F:
                    return a => a.FirstName;
                case ConsoleKey.L:
                    return a => a.LastName;
                default:
                    throw new ArgumentException($"Unknown key: {keyInfo.KeyChar}.");
            }
        }
        private static IOrderedEnumerable<Actor> OrderByDynamic(this IEnumerable<Actor> source, Func<Actor, object> keySelector, ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.A:
                    return source.OrderBy(keySelector);
                case ConsoleKey.D:
                    return source.OrderByDescending(keySelector);
                default:
                    throw new ArgumentException($"Unknown key: {keyInfo.KeyChar}.");
            }
        }
    }
}