using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine;
using RoomService.Application;
using RoomService.Application.Availability.Query;
using RoomService.Application.Date;
using RoomService.Application.Dto;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunWithOptions);

return;

static void RunWithOptions(Options opts)
{
    if (!File.Exists(opts.HotelsFile))
    {
        Console.WriteLine("Provided hotels file not found");
        return;
    }

    if (!File.Exists(opts.BookingsFile))
    {
        Console.WriteLine("Provided bookings file not found");
        return;
    }

    var hotels = LoadHotels(opts.HotelsFile);
    var bookings = LoadBookings(opts.BookingsFile);

    // NOTE: for testing you can use new FakeDateProvider(new DateOnly(2024, 11, 01));
    var dateProvider = new DateProvider();


    while (true)
    {
        Console.Write("Enter a command:");
        var input = Console.ReadLine();

        var pattern = @"^(\w+)\(([^)]*)\)$";
        var match = Regex.Match(input!, pattern);
        if (!match.Success)
        {
            Console.WriteLine(
                "Please enter a command. Search(H1,365, SGL) or Availability(H1,20240901, SGL) or Availability(H1,20240901-20240903, DBL) ");
            continue;
        }

        var commandName = match.Groups[1].Value;
        var arguments = match.Groups[2].Value
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();

        if (commandName == "Search")
        {
            if (arguments.Count != 3)
            {
                Console.WriteLine(
                    "Invalid number of arguments. Please enter a valid command. Search(H1,365, SGL)");
                continue;
            }

            var searchResult = new SearchQueryHandler(hotels, bookings, dateProvider)
                .Handle(new SearchQuery(arguments[0], int.Parse(arguments[1]), arguments[2]));

            foreach (var result in searchResult.Results)
            {
                Console.WriteLine(
                    $"({result.DataRange.Start.ToString("yyyyMMdd")}-{result.DataRange.End.ToString("yyyyMMdd")},{result.RoomsAvailable})");
            }

            Console.WriteLine();
        }
        else if (commandName == "Availability")
        {
            if (arguments.Count != 3)
            {
                Console.WriteLine(
                    "Invalid number of arguments. Please enter a valid command. Availability(H1,20240901, SGL) or Availability(H1,20240901-20240903, DBL)");
                continue;
            }

            var availabilityResult = new CheckAvailabilityQueryHandler(hotels, bookings)
                .Handle(new CheckAvailabilityQuery(arguments[0], arguments[1], arguments[2]));

            Console.WriteLine(availabilityResult.AvailableCount);
        }
        else if (commandName == "")
        {
            // exit the app
            break;
        }
        else

        {
            Console.WriteLine(
                "Unknown command. Please enter a valid command. Search(H1,365, SGL) or Availability(H1,20240901, SGL) or Availability(H1,20240901-20240903, DBL) ");
            continue;
        }
    }
}


static List<HotelDto> LoadHotels(string file)
{
    var hotelsStr = File.ReadAllText(file);
    var options = new JsonSerializerOptions
    {
        Converters = { new DateOnlyConverter() }
    };
    var hotels = JsonSerializer.Deserialize<List<HotelDto>>(hotelsStr, options)!;
    return hotels;
}

static List<BookingDto> LoadBookings(string file)
{
    var bookingsStr = File.ReadAllText(file);
    var options = new JsonSerializerOptions
    {
        Converters = { new DateOnlyConverter() }
    };
    var bookings = JsonSerializer.Deserialize<List<BookingDto>>(bookingsStr, options)!;
    return bookings;
}

internal class Options
{
    [Option('h', "hotels", Required = true, HelpText = "Full path to the hotels.json file")]
    public required string HotelsFile { get; set; }

    [Option('b', "bookings", Required = true, HelpText = "Full path to the bookings.json file")]
    public required string BookingsFile { get; set; }
}