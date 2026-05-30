// PROBLEM: BookMyShow — Movie Ticket Booking System
// Difficulty: Advanced (Tier 3)
// Interview Style: Design from scratch
//
// Write your solution below:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

public enum SeatStatus{
    Available, 
    Pending,
    Booked
}

public enum SeatType
{
    Regular,
    Platinum,
    Recliner
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public interface IPricingStrategy
{
    decimal GetPrice(Seat seat, Show show);
}

public class StandardPricing : IPricingStrategy
{
    public decimal GetPrice(Seat seat, Show show){
        if(seat.SeatType == SeatType.Regular)
        {
            return 200;
        }
        else if(seat.SeatType == SeatType.Platinum)
        {
            return 350;
        }
        else
        {
            return 500;
        }
    }
}

public class WeekendPricing : IPricingStrategy
{
    public IPricingStrategy Strategy;

    public WeekendPricing(IPricingStrategy strategy)
    {
        Strategy = strategy;
    }

    public decimal GetPrice(Seat seat, Show show)
    {
        if(show.StartTime.DayOfWeek == DayOfWeek.Saturday || show.StartTime.DayOfWeek == DayOfWeek.Sunday){
            return 1.5m * Strategy.GetPrice(seat, show);
        }
        return Strategy.GetPrice(seat, show);
    }
}

public class City{
    public string Name;
    public List<Theatre> Theatres;
    public City(string name){
        Name = name;
        Theatres = new();
    }

    public void AddTheatre(Theatre theatre){
        Theatres.Add(theatre);
    }

    public void RemoveTheatre(Theatre theatre){
        if(Theatres.Contains(theatre)){
            Theatres.Remove(theatre);
        }
    }
}

public class Theatre
{
    public string Name; 
    public List<Screen> Screens;
    public Theatre(string name){
        Name = name;
        Screens = new();
    }

    public void AddScreen(Screen screen){
        Screens.Add(screen);
    }

}

public class Screen
{
    public string Name;
    public List<Seat> Seats;
    public List<Show> Shows;
    public Screen(string name){
        Name = name;
        Seats = new();
        Shows = new();

        for(int i = 0; i < 10; i++){
            Seats.Add(new Seat($"Seat-{i+1}-{SeatType.Regular}", SeatType.Regular));
        }

        for(int i = 0; i < 10; i++){
            Seats.Add(new Seat($"Seat-{i+1}-{SeatType.Platinum}", SeatType.Platinum));
        }
        for(int i = 0; i < 10; i++){
            Seats.Add(new Seat($"Seat-{i+1}-{SeatType.Recliner}", SeatType.Recliner));
        }
    }

    public Show AddShows(Movie movie, DateTime startTime){
        var show = new Show(movie, startTime, this.Seats);
        Shows.Add(show);
        return show;
    }

    public void RemoveShow(Show show){
        if(Shows.Contains(show)){
            Shows.Remove(show);
        }
    }


}

public class Seat{
    public string Name;
    public SeatType SeatType;
    public Seat(string name, SeatType seatType){
        Name = name;
        SeatType = seatType;
    }

}

public class Show{
    public DateTime StartTime;
    public Dictionary<Seat, SeatStatus> Seats;
    public Movie Movie;

    public Show(Movie movie, DateTime startTime, List<Seat> screenSeats){
        Movie = movie;
        StartTime = startTime;
        Seats = new();
        foreach (var seat in screenSeats)
        Seats[seat] = SeatStatus.Available; 
    }

    public bool TryLockSeats(List<Seat> seats){
        foreach(Seat s in seats)
        {
            if(!Seats.ContainsKey(s) || Seats[s] != SeatStatus.Available)
                return false;
        }

        foreach(Seat s in seats)
            Seats[s] = SeatStatus.Pending;

        return true;
    }

    public void ConfirmSeats(List<Seat> seats){
        foreach(Seat s in seats){
            if(Seats[s] == SeatStatus.Pending)
            {
                Seats[s] = SeatStatus.Booked;
            }
        }
    }

    public void ReleaseSeats(List<Seat> seats)
    {
        foreach(Seat s in seats){
            if(Seats[s] == SeatStatus.Pending)
            {
                Seats[s] = SeatStatus.Available;
            }
        }
    }

    public List<Seat> GetAvailableSeats()
    {
        return Seats.Where(kv => kv.Value == SeatStatus.Available).Select(kv => kv.Key).ToList();
    }
}

public class Movie{
    public string MovieName;

    public Movie( string name){
        MovieName = name;
    }

}

public class User
{
    public string Id;
    public string Name;

    public User(string id, string name){
        Id = id;
        Name = name;
    }
}


public class Booking{
    public string Id;
    public User User;
    public Show Show;
    public List<Seat> Seats;
    public decimal TotalAmount;
    public BookingStatus BookingStatus;
    public DateTime BookingTime;
    public Booking(User user, Show show, List<Seat> seats, decimal totalAmount)
    {
        Id= Guid.NewGuid().ToString().Substring(0, 12);
        User = user;
        Show = show;
        Seats = seats;
        TotalAmount = totalAmount;
        BookingTime= DateTime.Now;
    }
}
public class BookingService
{
    public List<City> Cities;
    public List<Booking> Bookings;
    public IPricingStrategy Pricing;
    public BookingService(IPricingStrategy pricing){
        Cities = new();
        Bookings = new();
        Pricing = pricing;
    }

    public void AddCities(City city)
    {
        Cities.Add(city);
    }

    public List<Show> SearchShows(string movieName, string cityName){
        City? city = Cities.FirstOrDefault(c => c.Name == cityName);
        if(city == null) return new List<Show> ();
        List<Show> shows = city.Theatres
                            .SelectMany(t => t.Screens).SelectMany(s => s.Shows).Where(show => show.Movie.MovieName.Contains(movieName)).ToList();
        
        return shows;
    }

    public Booking BookSeats(User user, Show show, List<Seat> seats){
        if(show.TryLockSeats(seats) == false){
            throw new Exception("Unable to book seats");
        }
        decimal amount = 0; 
        foreach(var seat in seats){
            amount += Pricing.GetPrice(seat, show);
        }
        Booking booking = new Booking(user, show, seats, amount);
        Bookings.Add(booking);
        return booking;
    }

    public void ConfirmBooking(string bookingId){
        Booking booking = Bookings.FirstOrDefault(b => b.Id == bookingId);
        if(booking == null)
        {
            throw new Exception("No booking found!");
        }
        booking.Show.ConfirmSeats(booking.Seats);
        booking.BookingStatus = BookingStatus.Confirmed;
    }
    public void CancelBooking(string bookingId){
        Booking booking = Bookings.FirstOrDefault(b => b.Id == bookingId);
        if(booking == null)
        {
            throw new Exception("No booking found!");
        }
        booking.Show.ReleaseSeats(booking.Seats);
        booking.BookingStatus = BookingStatus.Cancelled;
    }

}
