using Microsoft.Extensions.Logging;
using MovieProject.Controllers;
using Persistence;

namespace MovieProjectTest
{
    public class Tests
    {

        private readonly DataContext _context;
        private readonly ILogger<MoviesController> _logger;
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void GetMovies()
        {
            var controller = new MoviesController(null, null);
            _ = controller.GetMovies();
            Assert.Pass();
        }
        [Test]
        public void GetReviews()
        {
            var controller = new MoviesController(null, null);
            _ = controller.GetReviews();
            Assert.Pass();
        }
        [Test]
        [TestCase("546b4421-7b59-4146-8d1b-e1135ac4ad33")]
        public void GetMoviesById(Guid id)
        {
            var controller = new MoviesController(null, null);
            _ = controller.GetMovieById(id);
            Assert.Pass();
        }
        [Test]
        [TestCase("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public void GetReviewById(Guid id)
        {
            var controller = new MoviesController(null, null);
            _ = controller.GetReviewById(id);
            Assert.Pass();
        }
    }
}