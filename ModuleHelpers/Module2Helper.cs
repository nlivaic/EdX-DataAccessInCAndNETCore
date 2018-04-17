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
    }
}