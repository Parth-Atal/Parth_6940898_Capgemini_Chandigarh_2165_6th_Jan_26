using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieLibrary
{
    
    public interface IFilm
    {
        string Title { get; set; }
        string Director { get; set; }
        int Year { get; set; }
    }

    
    public class Film : IFilm
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public int Year { get; set; }

        public Film(string title, string director, int year)
        {
            Title = title;
            Director = director;
            Year = year;
        }
    }

    
    public class FilmLibrary
    {
        private List<Film> films = new List<Film>();

        
        public void AddFilm(Film film)
        {
            films.Add(film);
        }

        
        public void RemoveFilm(string title)
        {
            films.RemoveAll(f => f.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

       
        public List<Film> GetFilmsByDirector(string director)
        {
            return films
                .Where(f => f.Director.Equals(director, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        
        public List<Film> SearchFilms(string query)
        {
            return films
                .Where(f =>
                    f.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    f.Director.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        
        public int GetFilmCount()
        {
            return films.Count;
        }
    }
}