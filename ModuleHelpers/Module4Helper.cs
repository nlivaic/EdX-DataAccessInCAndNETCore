using System;
using System.Collections.Generic;
using System.Linq;
using MovieApp.Entities;

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
            Console.WriteLine(nameof(QueryOptimization));
        }

        public static void DetachedEntities()
        {
            Console.WriteLine(nameof(DetachedEntities));
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