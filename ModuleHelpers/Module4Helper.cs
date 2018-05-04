using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieApp.Entities;
using MovieApp.Extensions;
using MovieApp.Models;

namespace MovieApp
{
    public static class Module4Helper
    {
        public static void CommitTransaction()
        {
            Film film = new Film 
            {
                Title = "Willy Wonka & the Chocolate Factory",
                ReleaseYear = 1971,
                RatingId = 1
            };
            Actor actor = new Actor 
            {
                FirstName = "Gene",
                LastName = "Wilder"
            };
            using (var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                MoviesContext.Instance.Films.Add(film);
                MoviesContext.Instance.SaveChanges();
                MoviesContext.Instance.Actors.Add(actor);
                MoviesContext.Instance.SaveChanges();
                // If you were to remove this .Commit(), queries below would show the initial state, not the new state.
                transaction.Commit();
            }
            string filmFound = MoviesContext.Instance.Films.Any(f => f.Title == film.Title) ? "YES" : "NO";
            string actorFound = MoviesContext.Instance.Actors.Any(a => a.LastName == actor.LastName) ? "YES" : "NO";
            Console.WriteLine($"Film found: {filmFound}.", filmFound);
            Console.WriteLine($"Actor found: {actorFound}.", actorFound);
        }

        public static void RollbackTransactionException()
        {
            int count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film count: {count}");
            using(var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MoviesContext.Instance.Films.Add(new Film { Title = $"Temp film {i}" });
                    }
                    MoviesContext.Instance.SaveChanges();
                    throw new ApplicationException("Simulated exception.");
                    transaction.Commit();
                    Console.WriteLine("Transaction committed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception encountered: {ex.Message}.");
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back.");
                }
            }
            count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film count: {count}");

            // Clean up after ourselves.
            IEnumerable<Film> films = MoviesContext.Instance.Films.Where(f => f.Title.StartsWith("temp"));
            MoviesContext.Instance.Films.RemoveRange(films);
            MoviesContext.Instance.SaveChanges();
        }

        public static void RollbackTransactionBusinessRule()
        {
            int count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film count: {count}.");
            using (var transaction = MoviesContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MoviesContext.Instance.Films.Add(new Film { Title = $"Temp film {i}" });
                    }
                    MoviesContext.Instance.SaveChanges();
                    bool cancelReceivedFromUser = true;
                    if (cancelReceivedFromUser)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transaction rolled back due to user cancel.");
                    }
                    else
                    {
                        transaction.Commit();
                        Console.WriteLine("Transaction committed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception encountered: {ex.Message}");
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back due to exception.");
                }
            }
            count = MoviesContext.Instance.Films.Count();
            Console.WriteLine($"Film count: {count}.");

            // Clean up after ourselves.
            IEnumerable<Film> films = MoviesContext.Instance.Films.Where(f => f.Title.StartsWith("temp"));
            MoviesContext.Instance.Films.RemoveRange(films);
            MoviesContext.Instance.SaveChanges();
        }

        public static void QueryOptimization()
        {
            Console.WriteLine("Loading data (inefficient query)...");
            IEnumerable<Film> films = MoviesContext.Instance.Films;
            foreach (Film film in films)
            {
                MoviesContext.Instance.Entry(film).Collection(f => f.FilmActor).Load();
                foreach (FilmActor filmActor in film.FilmActor)
                {
                    MoviesContext.Instance.Entry(filmActor).Reference(fa => fa.Actor).Load();
                }
            }
            Console.WriteLine("Done.");

            Console.WriteLine("Loading data (efficient query)...");
            films = MoviesContext.Instance.Films.Include(f => f.FilmActor)
                                                .ThenInclude(fa => fa.Actor)
                                                .ToList();
            foreach (Film film in films)
            {
                // Do nothing.
            }
            Console.WriteLine("Done.");
        }


        public static void DetachedEntities1()
        {
            // Untracked entity added to context.
            Console.WriteLine("Untracked entity added to context.");
            try
            {
                Film film = new Film { FilmId = 1, Title = "test" };
                MoviesContext.Instance.Films.Add(film);
                MoviesContext.Instance.SaveChanges();               // Exception thrown here (by the database), since EF is not aware a film with same FilmId exists.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Untracked entity added to context. \n\tException: {ex.Message}. \n\tInner Exception: {ex.InnerException.Message}");
            }
            
            // Copy of a tracked entity added to context.
            Console.WriteLine("Copy of a tracked entity added to context.");
            try
            {
                Film film = MoviesContext.Instance.Films.First();
                Film newFilm = film.Copy<Film, Film>();
                MoviesContext.Instance.Films.Add(newFilm);          // Exception thrown here (by EF), since "film" is already tracked and "newFilm" has same FilmId value.
                MoviesContext.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Copy of a tracked entity reinserted. Exception: {ex.Message}");
            }
        }

        public static void DetachedEntities2()
        {
            // Detached entities - setting state.
            Console.WriteLine("Detached entities - setting state.");
            // Retrieve actor's name.
            const int actorId = 1;
            string name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Original Name:\t{name}");

            // Create a new actor with the same id as above. Start tracking it.
            ActorModel actorModel = new ActorModel {
                ActorId = actorId,
                FirstName = "Luke",
                LastName = "Skywalker"
            };
            var actor = actorModel.Copy<ActorModel, Actor>();
            MoviesContext.Instance.Actors.Attach(actor);        // Start tracking. Will attach in Unchanged state.
            Console.WriteLine("State on update: " + MoviesContext.Instance.Entry(actor).State.ToString());                      // Unchanged.
            MoviesContext.Instance.Entry(actor).State = EntityState.Modified;       
            Console.WriteLine("State on update, after change: " + MoviesContext.Instance.Entry(actor).State.ToString());        // Modified.
            MoviesContext.Instance.SaveChanges();               // SQL generated, since actor is in Modified state.
            // Name not changed because no update operation was performed.
            name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Updated Name:\t{name}");

            // Revert values.
            Console.WriteLine("State on revert, before any change: " + MoviesContext.Instance.Entry(actor).State.ToString());   // Unchanged. Got reset after previous .SaveChanges()
            actor.FirstName = "Mark";
            actor.LastName = "Hammil";
            Console.WriteLine("State on revert: " + MoviesContext.Instance.Entry(actor).State.ToString());                      // Above name update causes the switch to Modified.
            MoviesContext.Instance.SaveChanges();
            // Name not changed because no update operation was performed (again).
            name = MoviesContext.Instance.Actors.Where(a => a.ActorId == actorId).Select(a => $"{a.FirstName} {a.LastName}").First();
            Console.WriteLine($"Reverted Name:\t{name}");
        }

        public static void ExecuteRawSql()
        {
            Console.WriteLine(nameof(ExecuteRawSql));
        }

        public static void ExecuteStoredProcedure()
        {
            Console.WriteLine(nameof(ExecuteStoredProcedure));
        }

        public static void SeedDatabase()
        {
            Console.WriteLine(nameof(SeedDatabase));
        }
    }
}