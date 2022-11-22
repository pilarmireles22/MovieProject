using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Movies.Any()) return;

            var reviews = new List<Review>
            {
                new Review {
                    Description= "Pelicula super buena" ,
                },
                new Review {
                    Description= "Horrible Pelicula" ,
                }
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "Avengers: Age of Ultron",
                    Year = "2015",
                    Genres = "Action",
                    Disable = true,
                    CreatedDate = DateTime.Now,
                    Reviews = reviews.ToList(),

                },
              new Movie
                {
                    Title = "Super Man",
                    Year = "2020",
                    Genres = "Action",
                    Disable = true,
                    CreatedDate = DateTime.Now,
                    Reviews = reviews.ToList(),

                }
            };

            await context.Movies.AddRangeAsync(movies);
            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }
    }
}