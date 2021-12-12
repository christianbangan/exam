using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Repository;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MovieRepository));

        [HttpGet]
        public ActionResult Index()
        {
            //continue here: bug in available seats
            MovieRepository repository = new MovieRepository();
            var movies = repository.GetMovies();
            List<int> availableSeats = new List<int>();
            

            Dictionary<string, int> seats = new Dictionary<string, int>(); ;

            foreach (var item in movies)
            {
                item.AvailableSeatsDictionary = new Dictionary<string, int>();
                item.Schedules = repository.GetSchedules(item.Id);
                
                foreach (var sched in item.Schedules)
                {

                    availableSeats.Add(repository.AvailableSeats(item.Id, sched.ScheduleTime));
                    item.AvailableSeats = availableSeats;
                }

                for (int i = 0; i < item.Schedules.Count; i++)
                {

                    item.AvailableSeatsDictionary.Add(item.Schedules[i].ScheduleTime, item.AvailableSeats[i]);
                }

            }
           
            return View(movies);
        }
        public ActionResult Reserve(int id)
        {
            MovieRepository repository = new MovieRepository();
            Movie movie = repository.GetMovieById(id);
            movie.Seats = repository.GetSeats(id);
            foreach (var item in movie.Seats)
            {
                movie.Status = item.Status;
            }
            return View(movie);
        }

        [HttpPost]
        public ActionResult AddMoviePost(FormCollection formCollection, HttpPostedFileBase file)
        {
            if (this.ModelState.IsValid)
            {
                if (file != null && file.ContentType.Contains("image"))
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine("../../public/images", fileName);
                    file.SaveAs(this.HttpContext.Server.MapPath("~/public/images/") + file.FileName);

                    MovieRepository repository = new MovieRepository();
                    repository.AddMovie(formCollection, path);

                    return RedirectToAction("Index");
                }
            }

            return View();

        }

        public ActionResult AddMovie()
        {
            //var test = formCollection;
            //MovieRepository repository = new MovieRepository();
            //var movie = new Movie();
            //movie.Schedules = repository.GetSchedules();
            return View();

        }


        [HttpPost]
        public ActionResult EditImage(int id, HttpPostedFileBase file)
        {
            var test = this.ModelState;
            if (this.ModelState.IsValid)
            {
                TempData["hideError"] = true;

                if (file != null && file.ContentType.Contains("image"))
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine("../../public/images", fileName);
                    file.SaveAs(this.HttpContext.Server.MapPath("~/public/images/") + file.FileName);

                    MovieRepository repository = new MovieRepository();
                    repository.EditImage(id, path);

                    TempData["hideError"] = false;
                    TempData["validation"] = "Image updated successfully.";
                    TempData["flag"] = 1;
                    TempData["display"] = "block";
                }
                else
                {
                    TempData["validation"] = "Please upload IMAGE file only.";
                    TempData["hideError"] = false;
                    TempData["flag"] = 0;
                    TempData["display"] = "block";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ReservePost(int id, FormCollection form)
        {
            if (this.ModelState.IsValid)
            {
                MovieRepository repository = new MovieRepository();
                repository.AddTransaction(id, form);
               
            }
            return RedirectToAction("Index");

        }
    }
}