# Hotel Room Booking API

RESTful API for hotel room bookings built on **ASP.NET Core** and **Entity Framework Core**.

## What it can do

- Search hotels by name or partial name
- Find available rooms for given dates, number of guests, and (optionally) room type.
- Create room bookings with unique booking reference number
- Retrieve booking details by reference number
- Admin endpoints for seeding and resetting test data
- Swagger/OpenAPI documentation

## Context

In this example, there are a number of hotels, each with 6 rooms. Two of them are single, two are double, and two are deluxe double rooms. The deluxe and double can accomodate up to 2 guests, and a single room accomodates only 1. No more guests can be allocated to the room. Guests stay in the same room throughout their stay. Rooms cannot be double booked.

The example provides a simple API which could be imaged as the backend used by an application allowing users to search for hotels, check availability of rooms, and book rooms if they are available.

## Technology

- **.NET** 10.0
- **ASP.NET Core Web API**
- **Entity Framework Core** 10
- **SQLite** database for demonstration purposes
- **[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** for Swagger/OpenAPI
- **[Bogus](https://github.com/bchavez/Bogus)** for seed data generation

## Install

This requires [.NET 10 SDK](https://dotnet.microsoft.com/download).

1. Clone the repository and go into the directory `mj-hotel-book-api` in Powershell.
2. Install the required packages: `dotnet restore`, and build: `dotnet build`.
3. Run the application: `dotnet run` and view the application on a web browser at [http://localhost:5000](http://localhost:5000). This will display the Swagger UI for the API.

## Database

For simplicity, the API uses SQLite database (`hotel_booking.db`) which is automatically created on first run.

## Swagger/OpenAPI documention

When running the server, this can be accessed at [localhost:5000/swagger/v1/swagger.json](http://localhost:5000/swagger/v1/swagger.json). There is also a saved copy in the root of this project.

## API Endpoints

### Hotels


| Method | Endpoint                                                                                             | Description                                                                  |
| ------ | ---------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| GET    | `/api/hotels?name={name}`                                                                            | Search hotels by name (partial case-insensitive match)                       |
| GET    | `/api/hotels/{id}`                                                                                   | Get hotel details by ID                                                      |
| GET    | `/api/hotels/{id}/rooms/available?checkIn={date}&checkOut={date}&guestCount={count}&roomType={type}` | Find available rooms (NOTE: roomType is optional:**Single, Double, Deluxe**) |

### Bookings


| Method | Endpoint                    | Description                     |
| ------ | --------------------------- | ------------------------------- |
| POST   | `/api/bookings`             | Create a new booking            |
| GET    | `/api/bookings/{reference}` | Get booking by reference number |

### Admin (Testing)


| Method | Endpoint           | Description                   |
| ------ | ------------------ | ----------------------------- |
| POST   | `/api/admin/seed`  | Seed database with test data  |
| POST   | `/api/admin/reset` | Remove all data from database |

## Testing the API

### Using Swagger UI

1. Start the application with `dotnet run`
2. Navigate to [http://localhost:5000](http://localhost:5000) in your browser
3. Use Swagger UI to test all endpoints

### Examples

#### 1. Seed the database with test data:

```bash
POST /api/admin/seed
```

Response:

```bash
{
  "message": "Database seeded successfully with test data."
}
```

#### 2. Search for a hotel:

```bash
GET /api/hotels?name=Hotel
```

Response:

```bash
[
  {
    "id": 14,
    "name": "Streich Group Hotel",
    "address": "708 Kassandra Forge, Dooleyton, Niger",
    "totalRooms": 6
  },
  {
    "id": 15,
    "name": "Robel and Sons Hotel",
    "address": "75230 Gottlieb Village, Tonyport, Sri Lanka",
    "totalRooms": 6
  },

  ...
```

#### 3. Get hotel details

```bash
GET /api/hotels/15
```

Response:

```bash
{
  "id": 15,
  "name": "Robel and Sons Hotel",
  "address": "75230 Gottlieb Village, Tonyport, Sri Lanka",
  "rooms": [
    {
      "id": 85,
      "roomNumber": "101",
      "roomTypeName": "Single",
      "capacity": 1
    },
    {
      "id": 86,
      "roomNumber": "102",
      "roomTypeName": "Single",
      "capacity": 1
    },
    {
      "id": 87,
      "roomNumber": "103",
      "roomTypeName": "Double",
      "capacity": 2
    },
    {
      "id": 88,
      "roomNumber": "104",
      "roomTypeName": "Double",
      "capacity": 2
    },
    {
      "id": 89,
      "roomNumber": "105",
      "roomTypeName": "Deluxe",
      "capacity": 2
    },
    {
      "id": 90,
      "roomNumber": "106",
      "roomTypeName": "Deluxe",
      "capacity": 2
    }
  ]
}
```

#### 4. Check available rooms:

```bash
GET /api/hotels/15/rooms/available?checkIn=2026-07-15&checkOut=2026-07-18&guestCount=2
```

Response:

```bash
[
  {
    "roomId": 87,
    "roomNumber": "103",
    "roomTypeName": "Double",
    "capacity": 2,
    "hotelId": 15,
    "hotelName": "Robel and Sons Hotel"
  },
  {
    "roomId": 88,
    "roomNumber": "104",
    "roomTypeName": "Double",
    "capacity": 2,
    "hotelId": 15,
    "hotelName": "Robel and Sons Hotel"
  },
  {
    "roomId": 89,
    "roomNumber": "105",
    "roomTypeName": "Deluxe",
    "capacity": 2,
    "hotelId": 15,
    "hotelName": "Robel and Sons Hotel"
  },
  {
    "roomId": 90,
    "roomNumber": "106",
    "roomTypeName": "Deluxe",
    "capacity": 2,
    "hotelId": 15,
    "hotelName": "Robel and Sons Hotel"
  }
]
```

#### 5. Create a booking:

```bash
POST /api/bookings
Content-Type: application/json

{
  "hotelId": 15,
  "roomType": "Double",
  "firstName": "John",
  "lastName": "Doe",
  "address": "123 Main St, London, UK",
  "contactNumber": "+44 7700 900000",
  "guestCount": 2,
  "checkInDate": "2026-07-15",
  "checkOutDate": "2026-07-18"
}
```

The system will find and allocate the first available room that can accommodate the guest count. The response includes the allocated room details.

Response:

```bash
{
  "id": 47,
  "bookingReference": "BK-20260702-180385B3",
  "firstName": "John",
  "lastName": "Doe",
  "address": "123 Main St, London, UK",
  "contactNumber": "+44 7700 900000",
  "guestCount": 2,
  "checkInDate": "2026-07-15",
  "checkOutDate": "2026-07-18",
  "createdAt": "2026-07-02T12:56:15.63665Z",
  "room": {
    "id": 87,
    "roomNumber": "103",
    "roomTypeName": "Double",
    "capacity": 2
  },
  "hotelName": "Robel and Sons Hotel",
  "hotelAddress": "75230 Gottlieb Village, Tonyport, Sri Lanka"
}
```

#### 6. Retrieve booking details:

```bash
GET /api/bookings/BK-20260702-180385B3

```

Response:

```bash
{
  "id": 47,
  "bookingReference": "BK-20260702-180385B3",
  "firstName": "John",
  "lastName": "Doe",
  "address": "123 Main St, London, UK",
  "contactNumber": "+44 7700 900000",
  "guestCount": 2,
  "checkInDate": "2026-07-15",
  "checkOutDate": "2026-07-18",
  "createdAt": "2026-07-02T12:56:15.63665",
  "room": {
    "id": 87,
    "roomNumber": "103",
    "roomTypeName": "Double",
    "capacity": 2
  },
  "hotelName": "Robel and Sons Hotel",
  "hotelAddress": "75230 Gottlieb Village, Tonyport, Sri Lanka"
}
```

#### 7. Double booking attempt

Repeat step 5 twice. The first repeat will mean all the double rooms in the hotel are full for those dates. The second repeat will be met with the response:

```bash
{
  "message": "No 'Double' rooms available for the specified dates. Please try a different room type or different dates."
}
```

#### Reset for Fresh Testing

To clear all test data:

```bash
POST /api/admin/reset
```

Response:
```bash
{
  "message": "Database reset successfully. All test data has been removed."
}
```

Note that this does not remove the room types from the database.
