# RoomService

> The application should read from files containing hotel data and booking data, then allow a user to check room availability for a specified hotel, daterange, and room type

## How to run?

**Using ready docker image:**

1. `docker run -it ghcr.io/toumash/room-service-cli:3add639691e501ff3ffb93594e237d1ac7fdbf18 --hotels hotels.json --bookings bookings.json`

**Using docker:**

1. Place the `hotels.json` and `bookings.json` files inside `RoomService.CLI` directory
2. `docker build -t room-service -f RoomService.CLI\Dockerfile .`
3. `docker run -it room-service --hotels hotels.json --bookings bookings.json`

**Using .net 8 installed on the system:**

1. Install .net 8
2. Run with full paths to the input files `dotnet run --hotels hotels.json --bookings bookings.json`
