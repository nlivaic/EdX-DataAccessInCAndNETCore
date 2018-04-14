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
    public static class Module1Helper
    {
        internal static void SelectList()
        {
            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        internal static void SelectById()
        {
            Console.WriteLine("Enter actor id: ");
            int actorId = Console.ReadLine().ToInt();
            Actor actor = MoviesContext.Instance.Actors.SingleOrDefault(a => a.ActorId == actorId);
            if (actor == null) {
                Console.WriteLine($"Actor with ID {actorId} not found.");
            } else {
                Console.WriteLine($"ID: {actor.ActorId} Name: {actor.FirstName} {actor.LastName}.");
            }

            Console.WriteLine("Enter film title: ");
            string title = Console.ReadLine();
            Film film = MoviesContext.Instance.Films.FirstOrDefault(f => f.Title.Contains(title));
            if (film == null) {
                Console.WriteLine($"Film titled {title} not found.");
            } else {
                Console.WriteLine($"Film id: {film.FilmId}, title: {title}, release year: {film.ReleaseYear}, rating: {film.Rating}. ");
            }
            
        }
        internal static void CreateItem()
        {
            /* Create Actor */
            Console.WriteLine("Add an actor.");
            Console.WriteLine("Enter a first name.");
            string firstName = Console.ReadLine();
            Console.WriteLine("Enter a last name.");
            string lastName = Console.ReadLine();
            
            Actor actor = new Actor { FirstName = firstName, LastName = lastName };
            MoviesContext.Instance.Actors.Add(actor);
            MoviesContext.Instance.SaveChanges();
            
            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.Where(a => a.ActorId == actor.ActorId).Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();

            /* Create Film */
            Console.WriteLine("Add a film.");
            Console.WriteLine("Enter a title.");
            string title = Console.ReadLine();
            Console.WriteLine("Enter a description.");
            string description = Console.ReadLine();
            Console.WriteLine("Enter a release year.");
            int releaseYear = Console.ReadLine().ToInt();
            Console.WriteLine("Enter a rating.");
            string rating = Console.ReadLine();

            Film film = new Film { Title = title, Description = description, ReleaseYear = releaseYear, Rating = rating };
            MoviesContext.Instance.Films.Add(film);
            MoviesContext.Instance.SaveChanges();

            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.Where(f => f.FilmId == film.FilmId).Select(f => f.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }

        internal static void UpdateItem()
        {
            /* Update Actor */
            Console.WriteLine("Update an actor.");
            Console.WriteLine("Enter an Actor Id: ");
            int actorId = Console.ReadLine().ToInt();
            Actor actor = MoviesContext.Instance.Actors.SingleOrDefault(a => a.ActorId == actorId);
            if (actor == null) {
                Console.WriteLine($"Actor with id {actor.ActorId} not found.");
            } else {
                ConsoleTable.From(new[] { actor.Copy<Actor, ActorModel>() }).Write();
                Console.WriteLine("Enter the First name: ");
                string firstName = Console.ReadLine().Trim();
                Console.WriteLine("Enter the Last name: ");
                string lastName = Console.ReadLine().Trim();
                actor.FirstName = firstName;
                actor.LastName = lastName;

                MoviesContext.Instance.SaveChanges();
                IEnumerable<ActorModel> actorModels = MoviesContext.Instance.Actors.Where(a => a.ActorId == actor.ActorId).Select(a => a.Copy<Actor, ActorModel>());
                ConsoleTable.From(actorModels).Write();
            }

            /* Update Film */
            Console.WriteLine("Update a film.");
            Console.WriteLine("Enter Film Id: ");
            int filmId = Console.ReadLine().ToInt();
            Film film = MoviesContext.Instance.Films.SingleOrDefault(f => f.FilmId == filmId);
            if (film == null) {
                Console.WriteLine($"Film with id {film.FilmId} not found.");
            } else {
                ConsoleTable.From(new[] { film.Copy<Film, FilmModel>() }).Write();
                Console.WriteLine("Enter a title.");
                string title = Console.ReadLine().Trim();
                Console.WriteLine("Enter a description.");
                string description = Console.ReadLine().Trim();
                Console.WriteLine("Enter a release year.");
                int releaseYear = Console.ReadLine().ToInt();
                Console.WriteLine("Enter a rating.");
                string rating = Console.ReadLine().Trim();
                if (!String.IsNullOrEmpty(title) && film.Title != title) {
                    film.Title = title;
                }
                if (!String.IsNullOrEmpty(description) && film.Description != description) {
                    film.Description = description;
                }
                if (releaseYear > 0 && film.ReleaseYear != releaseYear) {
                    film.ReleaseYear = releaseYear;
                }
                if (!String.IsNullOrEmpty(rating) && film.Rating != rating) {
                    film.Rating = rating;
                }
                MoviesContext.Instance.SaveChanges();
                IEnumerable<FilmModel> filmModels = MoviesContext.Instance.Films.Where(f => f.FilmId == film.FilmId).Select(f => f.Copy<Film, FilmModel>());
                ConsoleTable.From(filmModels).Write();
            }
        }
        
        internal static void DeleteItem()
        {
            /* Delete an actor. */
            Console.WriteLine("Delete an actor.");
            Console.WriteLine("Enter an Actor Id: ");
            int actorId = Console.ReadLine().ToInt();
            Actor actor = MoviesContext.Instance.Actors.SingleOrDefault(a => a.ActorId == actorId);
            if (actor == null) {
                Console.WriteLine($"Actor with Id {actorId} not found.");
            } else {
                Console.WriteLine("Existing actors:");
                WriteActors();
                MoviesContext.Instance.Actors.Remove(actor);
                MoviesContext.Instance.SaveChanges();
                Console.WriteLine("Actor removed.");
                WriteActors();
            }
            
            /* Delete a film. */
            Console.WriteLine("Delete a film.");
            Console.WriteLine("Enter a Film Title: ");
            string filmTitle = Console.ReadLine();
            Film film = MoviesContext.Instance.Films.FirstOrDefault(f => f.Title.Contains(filmTitle));
            if (film == null) {
                Console.WriteLine($"Film with title {filmTitle} not found.");
            } else {
                ConsoleTable.From(new[] { film.Copy<Film, FilmModel>() }).Write();
                Console.WriteLine("Are you sure you want to delete this film?");
                if (Console.ReadKey().Key == ConsoleKey.Y) {
                    MoviesContext.Instance.Films.Remove(film);
                    MoviesContext.Instance.SaveChanges();
                    WriteFilms();
                } else {
                    Console.WriteLine("No films deleted.");
                }
            }
        }

        internal static void SelfAssessment()
        {
            string filmTitleTemplate = "Test Film {0}";
            string updateTitlePrefix = "Awesome ";
            Console.WriteLine("All current films:");
            WriteFilms();
            /* Create films. */
            Console.WriteLine("Create new films, 5 of them:");
            for (int i = 1; i <= 5; i++)
            {
                string filmTitle = String.Format(filmTitleTemplate, i);
                Film film = new Film { Title = filmTitle };
                MoviesContext.Instance.Films.Add(film);
            }
            MoviesContext.Instance.SaveChanges();
            Console.WriteLine("5 movies created.");
            WriteFilms();
            

            /* Update films. */
            Console.WriteLine("Press any key to update the films.");
            Console.ReadKey();
            IEnumerable<Film> updateFilms = MoviesContext.Instance.Films;
            foreach (Film film in updateFilms)
            {
                // Only even numbered movies.
                if (film.FilmId % 2 == 0)
                {
                    film.Title = String.Concat(updateTitlePrefix, film.Title);
                }
            }
            MoviesContext.Instance.SaveChanges();
            Console.WriteLine("Movies updated.");
            WriteFilms();

            /* Delete films. */
            Console.WriteLine("Press any key to delete the 5 films.");
            Console.ReadKey();
            IEnumerable<Film> films = MoviesContext.Instance.Films.Where(f => f.Title.Contains("test film"));
            MoviesContext.Instance.Films.RemoveRange(films);
            MoviesContext.Instance.SaveChanges();
            Console.WriteLine("5 movies deleted.");
            WriteFilms();

            /* Cleanup film data. */
            Console.WriteLine("Press any key to cleanup film data.");
            Console.ReadKey();
            IEnumerable<Film> cleanupFilms = MoviesContext.Instance.Films.Where(f => f.Title.StartsWith(updateTitlePrefix));
            foreach (Film film in cleanupFilms)
            {
                film.Title = film.Title.Replace(updateTitlePrefix, String.Empty);
            }
            MoviesContext.Instance.SaveChanges();
            Console.WriteLine("Film data cleaned up.");
            WriteFilms();
        }
        

        private static void WriteActors()
        {
            IEnumerable<ActorModel> actors = MoviesContext.Instance.Actors.Select(a => a.Copy<Actor, ActorModel>());
            ConsoleTable.From(actors).Write();
        }
        private static void WriteFilms()
        {
            IEnumerable<FilmModel> films = MoviesContext.Instance.Films.Select(a => a.Copy<Film, FilmModel>());
            ConsoleTable.From(films).Write();
        }
    }
}