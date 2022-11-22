using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace MovieProject.Controllers
{
    public class MoviesController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(DataContext context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;   
        }
        [HttpGet]
        public async Task<ActionResult<List<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie?>> GetMovieById(Guid id)
        {
            return await _context.Movies.FindAsync(id)!;
        }
        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovie(Movie movie)
        {
            try
            {
                if (movie == null)
                return BadRequest();

                var createdMovie = await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();
                return Ok(createdMovie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new movie record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new movie record");

            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovieById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    NotFound();
                }

                var movieRecord = await _context.Movies.FindAsync(id);
                if (movieRecord == null && movieRecord?.Disable == true)
                {
                    return StatusCode(StatusCodes.Status304NotModified,
                   "Record not modified");
                }
                movieRecord.Id = id;
                _context.Movies.Remove(movieRecord);
                await _context.SaveChangesAsync();
                return Ok(movieRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting new movie record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting new movie record");
            }
        }
        [HttpGet("/reviews")]
        public async Task<ActionResult<List<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }
        [HttpGet("/reviews/{id}")]
        public async Task<ActionResult<Review?>> GetReviewById(Guid id)
        {
            return await _context.Reviews.FindAsync(id)!;
        }
        [HttpPost("/review/{id}")]
        public async Task<ActionResult<Movie>> CreateReviewByMovie(Guid id, [FromBody] Review review)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest();

                var movie = await _context.Movies.FindAsync(id);


                if (movie == null)
                    return BadRequest();

                review.Id = Guid.Empty; 
                review.MovieId = movie.Id;
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new review record");

            }
        }
    }
}
