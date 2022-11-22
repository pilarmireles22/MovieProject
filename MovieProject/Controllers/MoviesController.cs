using Application.Common.Enums;
using Application.Helpers;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;
using System.Collections;
using System.Text;

namespace MovieProject.Controllers
{
    public class MoviesController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ILogger<MoviesController> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly IRedisCaching _redisCaching;

        public MoviesController(DataContext context, ILogger<MoviesController> logger, IDistributedCache distributedCache, IRedisCaching redisCaching)
        {
            _context = context;
            _logger = logger;
            _distributedCache = distributedCache;
            _redisCaching = redisCaching;
        }
        [HttpGet]
        public async Task<ActionResult<List<Movie>>> GetMovies()
        {
            try
            {
                var cacheKey = "MoviesList";
                var movieList = new List<Movie>();
                movieList = await _redisCaching.GetFromRedis(movieList, cacheKey);
                if (movieList == null)
                {
                    movieList = await _context.Movies.ToListAsync();
                    movieList = _redisCaching.GetFromDatabase(movieList, cacheKey);
                }
                return Ok(movieList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie list record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error getting movie record");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie?>> GetMovieById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    NotFound();

                var movie = await _context.Movies.FindAsync(id);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie by id record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error getting movie by id record");
            }
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
                    NotFound();

                var movieRecord = await _context.Movies.FindAsync(id);
                if (movieRecord == null && movieRecord?.Disable == true)
                {
                    _logger.LogError("Record not modified");
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

            try
            {
                var cacheKey = "ReviewsList";
                var reviewList = new List<Review>();
                reviewList = await _redisCaching.GetFromRedis(reviewList, cacheKey);
                if (reviewList == null)
                {
                    reviewList = await _context.Reviews.ToListAsync();
                    if (reviewList == null)
                        return BadRequest();

                    reviewList = _redisCaching.GetFromDatabase(reviewList, cacheKey);
                }
                return Ok(reviewList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error getting review record");
            }
        }
        [HttpGet("/reviews/{id}")]
        public async Task<ActionResult<Review?>> GetReviewById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest();

                var movie = await _context.Reviews.FindAsync(id);

                if (movie == null)
                    return BadRequest();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review record by id");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error getting review record by id");
            }
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
                _logger.LogError(ex, "Error creating new review record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new review record");

            }
        }

        [HttpGet("/filter")]
        public async Task<ActionResult<Movie>> FilterMovies(string title, int page = 0, int pageSize = 10, SortType sortType = SortType.ASC)
        {
            try
            {
                IQueryable<Movie> movieList =  _context.Movies;
                if (movieList == null)
                   return BadRequest();

                
                movieList = !string.IsNullOrEmpty(title)? movieList.Where(x => x.Title.ToLower().Contains(title.ToLower())) : movieList;
                movieList = movieList.Skip(page * pageSize).Take(pageSize);

                if (sortType == SortType.ASC)
                {
                    movieList = movieList.OrderBy(x => x.Title);
                }
                else
                {
                    movieList = movieList.OrderByDescending(x => x.Title);
                }

                return Ok(await movieList.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering movie list record");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error filtering movie record");
            }
        }
    }
}
