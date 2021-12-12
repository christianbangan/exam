using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApplication1.Models.Repository
{
    interface IMovieRepository
    {
        List<Movie> GetMovies();
        Movie GetMovie(int id);
        int AddMovie(FormCollection form, string path);

        List<Schedule> GetSchedules(int id);
    }
}
