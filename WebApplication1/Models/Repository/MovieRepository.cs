using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace WebApplication1.Models.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MovieRepository));
        private static readonly string cs = WebConfigurationManager.ConnectionStrings["connection"].ConnectionString;
        private static SqlConnection connection = null;

        public SqlConnection Connection(string connectionString)
        {
            return new SqlConnection(cs);
        }

        public int AddMovie(FormCollection form, string imagePath)
        {
            int result = 0;
            SqlParameter seatParam = null;
            SqlParameter schedParam = null;

            try
            {
                using (connection = Connection(cs))
                {
                    SqlCommand sqlCommand = new SqlCommand("spInsertMovie", connection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    //string test = form.GetValue("Title").AttemptedValue;
                    sqlCommand.Parameters.Add(new SqlParameter("@Title", form.GetValue("Title").AttemptedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("@Description", form.GetValue("Description").AttemptedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("@Image_path", imagePath));
                    sqlCommand.Parameters.Add(new SqlParameter("@Date_added", DateTime.Now));
                    SqlParameter outputParam = new SqlParameter("@Identity", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    sqlCommand.Parameters.Add(outputParam);


                    connection.Open();

                    result = sqlCommand.ExecuteNonQuery();

                    int id = int.Parse(outputParam.Value.ToString());

                    connection.Close();

                    if (result > 0)
                    {
                        connection.Open();
                        sqlCommand = new SqlCommand("spInsertSeats", connection);
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        sqlCommand.Parameters.Add(new SqlParameter("@Identity", id));

                        foreach (string item in form.GetValues("seats"))
                        {
                            seatParam = new SqlParameter("@SeatValue", item);
                            sqlCommand.Parameters.Add(seatParam);

                            result = sqlCommand.ExecuteNonQuery();

                            if (result > 0)
                            {
                                sqlCommand.Parameters.Remove(seatParam);
                            }
                        }

                        connection.Close();

                        sqlCommand = new SqlCommand("spInsertSchedules", connection);
                        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        var test1 = outputParam;
                        var test2 = outputParam.Value;
                        sqlCommand.Parameters.Add(new SqlParameter("@Identity", id));

                        connection.Open();
                        foreach (string item in form.GetValues("schedule"))
                        {
                            schedParam = new SqlParameter("@ScheduleTime", item);
                            sqlCommand.Parameters.Add(new SqlParameter("@ScheduleTime", item));

                            result = sqlCommand.ExecuteNonQuery();

                            if (result > 0)
                            {
                                sqlCommand.Parameters.RemoveAt("@ScheduleTime");
                            }
                        }


                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }

            return result;
        }

        public Movie GetMovie(int id)
        {
            throw new NotImplementedException();
        }

        public List<Movie> GetMovies()
        {
            List<Movie> movies = new List<Movie>();
            Movie movie = null;

            try
            {
                using (connection = Connection(cs))
                {
                    SqlCommand sqlCommand = new SqlCommand("spGetMovies", connection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        movie = new Movie()
                        {
                            Id = int.Parse(rdr["MovieId"].ToString()),
                            Title = rdr["Title"].ToString(),
                            Description = rdr["Description"].ToString(),
                            image_path = rdr["image_path"].ToString()
                        };


                        movies.Add(movie);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }

            return movies;
        }

        public List<Schedule> GetSchedules(int id)
        {
            List<Schedule> schedules = new List<Schedule>();
            Schedule schedule = null;

            try
            {
                using (connection = Connection(cs))
                {
                    SqlCommand sqlCommand = new SqlCommand("spGetSchedules", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));
                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        schedule = new Schedule()
                        {
                            ScheduleId = int.Parse(rdr["ScheduleId"].ToString()),
                            ScheduleTime = rdr["ScheduleTime"].ToString(),
                            MovieId = int.Parse(rdr["MovieId"].ToString())
                        };


                        schedules.Add(schedule);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }

            return schedules;
        }

        public int EditImage(int id, string newPath)
        {
            int result = 0;
            try
            {
                using (connection = Connection(cs))
                {
                    SqlCommand sqlCommand = new SqlCommand("spEditImage", connection);
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@id", id));
                    sqlCommand.Parameters.Add(new SqlParameter("@newPath", newPath));

                    connection.Open();

                    result = sqlCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        log.Info($"Image edited successfully with id: {id}");
                    }
                }
            }
            catch (Exception e)
            {

                log.Error(e.Message);
            }

            return result;

        }

        public int AvailableSeats(int id, string time)
        {
            int result = 0;
            try
            {
                using (connection = Connection(cs))
                {

                    SqlCommand sqlCommand = new SqlCommand("spAvailableSeats", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));
                    sqlCommand.Parameters.Add(new SqlParameter("@ScheduleTime", time));

                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        result = int.Parse(rdr["TotalAvailable"].ToString());
                    }
                }
            }
            catch (Exception e)
            {

                log.Error(e.Message);
            }

            return result;
        }

        public int AddTransaction(int id, FormCollection form)
        {
            int result = 0;
            SqlParameter seatParam = null;

            try
            {
                using (connection = Connection(cs))
                {

                    SqlCommand sqlCommand = new SqlCommand("spInsertTransactions", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@CustomerName", form.GetValue("CustomerName").AttemptedValue));
                    sqlCommand.Parameters.Add(new SqlParameter("@TransactionDate", DateTime.Now));
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));

                    connection.Open();

                    foreach (string item in form.GetValues("seats"))
                    {
                        seatParam = new SqlParameter("@ReservedSeats", item);
                        sqlCommand.Parameters.Add(seatParam);

                        result = sqlCommand.ExecuteNonQuery();

                        if (result > 0)
                        {
                            sqlCommand.Parameters.Remove(seatParam);
                        }
                    }
                }

            }
            catch (Exception e)
            {

                log.Debug(e.Message);
            }

            return result;
        }

        public Movie GetMovieById(int id)
        {
            Movie movie = null;

            try
            {
                using (connection = Connection(cs))
                {

                    SqlCommand sqlCommand = new SqlCommand("spGetMovie", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));

                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        movie = new Movie()
                        {
                            Id = int.Parse(rdr["MovieId"].ToString()),
                            Title = rdr["Title"].ToString(),
                            image_path = rdr["image_path"].ToString()
                        };
                    }
                }
            }
            catch (Exception e)
            {

                log.Error(e.Message);
            }

            return movie;
        }

        public List<Reservation> GetReservations(int id)
        {
            List<Reservation> reservations = null;
            Reservation reservation = null;
            
            try
            {
                using (connection = Connection(cs))
                {

                    SqlCommand sqlCommand = new SqlCommand("@spGetTransactionsById", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));

                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        reservation = new Reservation()
                        {
                            TransactionId = int.Parse(rdr["TransactionId"].ToString()),
                            CustomerName = rdr["CustomerName"].ToString(),
                            TransactionDate = Convert.ToDateTime(rdr["TransactionDate"].ToString()),
                            ReservedSeat = rdr["ReservedSeats"].ToString(),
                            SeatId = int.Parse(rdr["SeatId"].ToString()),
                            SeatValue = rdr["SeatValue"].ToString(),
                            Status = rdr["Status"].ToString(),
                            MovieId = int.Parse(rdr["MovieId"].ToString())
                            
                        };

                        reservations.Add(reservation);
                    }
                }
            }
            catch (Exception e)
            {

                log.Error(e.Message);
            }

            return reservations;
        }

        public List<Seat> GetSeats(int id)
        {
            List<Seat> seats = new List<Seat>();
            Seat seat = null;

            try
            {
                using (connection = Connection(cs))
                {

                    SqlCommand sqlCommand = new SqlCommand("spGetSeatsById", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@MovieId", id));

                    connection.Open();

                    SqlDataReader rdr = sqlCommand.ExecuteReader();

                    while (rdr.Read())
                    {
                        seat = new Seat()
                        {
                            SeatId = int.Parse(rdr["SeatId"].ToString()),
                            SeatValue = rdr["SeatValue"].ToString(),
                            Status = rdr["Status"].ToString(),
                            MovieId = int.Parse(rdr["MovieId"].ToString())
                        };

                        seats.Add(seat);
                    }
                }
            }
            catch (Exception e)
            {

                log.Error(e.Message);
            }

            return seats;
        }

    }
}