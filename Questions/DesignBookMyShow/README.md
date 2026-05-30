# Design BookMyShow — Movie Ticket Booking System

A low-level design implementation of an online movie ticket booking platform (like BookMyShow / Fandango) in C#.

## Problem Statement

Design an online movie ticket booking system where:
- Cities have multiple theatres, each with multiple screens
- Each screen has seats of different types (Regular, Platinum, Recliner)
- Shows are scheduled on screens for specific movies and times
- Users can search for shows by movie name and city
- Users can book seats for a show (with locking to prevent double-booking)
- Bookings can be confirmed or cancelled
- Pricing varies by seat type and can change based on strategy (e.g., weekend surcharge)

## Entity Hierarchy

```
City
 └── Theatre
      └── Screen
           ├── Seats (Regular, Platinum, Recliner)
           └── Shows
                ├── Movie
                └── Per-Show Seat Availability (Available / Pending / Booked)
```

## Class Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            BOOKING SERVICE                                  │
│                                                                             │
│  SearchShows(movieName, cityName) → List<Show>                              │
│  BookSeats(user, show, seats) → Booking                                     │
│  ConfirmBooking(bookingId)                                                  │
│  CancelBooking(bookingId)                                                   │
│                                                                             │
│  Uses: IPricingStrategy                                                     │
│  Manages: List<City>, List<Booking>                                         │
└─────────────────────────────────────────────────────────────────────────────┘
         │
         │ delegates pricing to
         ▼
┌──────────────────────┐        ┌────────────────────────────┐
│  IPricingStrategy     │◄───────│  StandardPricing            │
│                       │        │  Regular=200, Plat=350,     │
│  GetPrice(seat, show) │        │  Recliner=500               │
└──────────────────────┘        └────────────────────────────┘
         ▲
         │ wraps (Decorator)
┌────────┴───────────────────────────────────────────────────┐
│  WeekendPricing                                             │
│  Wraps another IPricingStrategy                             │
│  Sat/Sun → 1.5x multiplier, else → base price              │
└─────────────────────────────────────────────────────────────┘

┌──────────┐     ┌───────────┐     ┌──────────┐     ┌────────┐
│   City    │────▶│  Theatre   │────▶│  Screen   │────▶│  Show   │
│           │  *  │            │  *  │           │  *  │         │
│  Name     │     │  Name      │     │  Name     │     │ Movie   │
│  Theatres │     │  Screens   │     │  Seats    │     │ Start   │
└──────────┘     └───────────┘     │  Shows    │     │ Seats   │
                                    └──────────┘     └────────┘

┌──────────┐     ┌───────────┐     ┌────────────┐
│   Seat    │     │   Movie    │     │   User      │
│           │     │            │     │             │
│  Name     │     │  MovieName │     │  Id         │
│  SeatType │     └───────────┘     │  Name       │
└──────────┘                        └────────────┘

┌─────────────────────────┐
│        Booking           │
│                          │
│  Id (GUID)               │
│  User                    │
│  Show                    │
│  Seats                   │
│  TotalAmount             │
│  BookingStatus           │
│  BookingTime             │
└─────────────────────────┘
```

## Seat Booking Flow

```
User selects seats
        │
        ▼
  ┌─────────────┐     ╔═══════════════════════╗
  │ TryLockSeats │────▶║ All seats Available?   ║
  └─────────────┘     ╚═══════════════════════╝
                          │             │
                         YES            NO
                          │             │
                          ▼             ▼
                   Mark seats      Return false
                   as PENDING      (booking fails)
                          │
                          ▼
                  Calculate price
                  (via IPricingStrategy)
                          │
                          ▼
                   Create Booking
                   (status: Pending)
                          │
                 ┌────────┴────────┐
                 ▼                 ▼
          ConfirmBooking     CancelBooking
          Seats → BOOKED    Seats → AVAILABLE
          Status: Confirmed Status: Cancelled
```

## Design Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **Strategy** | `IPricingStrategy` | Pricing logic varies (standard vs. weekend vs. premium). Swap strategies without changing booking logic |
| **Decorator** | `WeekendPricing` wraps `StandardPricing` | Adds weekend surcharge on top of any base pricing strategy. Can be layered |

## Key Design Decisions

### 1. Per-Show Seat Status (not per-Screen)
Each `Show` maintains its own `Dictionary<Seat, SeatStatus>`. The same physical seat can be "Booked" for the 6pm show and "Available" for the 9pm show. The `Seat` object is shared, but the status is per-show.

### 2. Two-Phase Booking (Lock → Confirm/Cancel)
Seats go through three states: `Available → Pending → Booked/Available`. The `Pending` state prevents double-booking — if two users try to book the same seat simultaneously, `TryLockSeats` will fail for one of them.

### 3. Pricing as Decorator Chain
`WeekendPricing` wraps any `IPricingStrategy`. This means you can stack behaviors:
```csharp
var pricing = new WeekendPricing(new StandardPricing());
// Weekend: 1.5x of standard price
// Weekday: standard price
```

### 4. LINQ-based Search
`SearchShows` flattens the City → Theatre → Screen → Show hierarchy using `SelectMany`, making the query clean and declarative.

## Enums

```
SeatStatus: Available | Pending | Booked
SeatType:   Regular | Platinum | Recliner
BookingStatus: Pending | Confirmed | Cancelled
```

## Usage Example

```csharp
// Setup
var pricing = new WeekendPricing(new StandardPricing());
var service = new BookingService(pricing);

var city = new City("Mumbai");
var theatre = new Theatre("PVR Phoenix");
var screen = new Screen("Screen 1");  // auto-creates 30 seats (10 each type)

theatre.AddScreen(screen);
city.AddTheatre(theatre);
service.AddCities(city);

// Add a show
var movie = new Movie("Inception");
var show = screen.AddShows(movie, new DateTime(2026, 6, 1, 18, 0, 0));

// Search
var shows = service.SearchShows("Inception", "Mumbai");

// Book
var user = new User("U1", "Rahul");
var availableSeats = show.GetAvailableSeats().Take(2).ToList();
var booking = service.BookSeats(user, show, availableSeats);

// Confirm
service.ConfirmBooking(booking.Id);
```

## Possible Extensions

- **Payment integration**: Add `IPaymentGateway` strategy for different payment methods
- **Timeout on pending**: Auto-release seats if not confirmed within N minutes
- **Seat map UI**: Row/column based seat layout instead of flat list
- **Discounts/coupons**: Another decorator layer on pricing
- **Notifications**: Observer pattern to notify users on booking confirmation
- **Concurrency**: Add `lock` statements around `TryLockSeats` for thread safety
