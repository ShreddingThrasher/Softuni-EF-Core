namespace Theatre.DataProcessor
{
    using System;
    using System.Text;
    using System.IO;
    using System.Linq;

    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    using Newtonsoft.Json;
    using System.Xml.Serialization;
    using Microsoft.EntityFrameworkCore;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            ExportTopTheaterDto[] dtos = context
                .Theatres
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= 20)
                .ToArray()
                .Select(t => new ExportTopTheaterDto()
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                        .Where(ticket => ticket.RowNumber <= 5)
                        .Sum(ticket => ticket.Price),
                    Tickets = t.Tickets
                        .Where(ticket => ticket.RowNumber <= 5)
                        .Select(ticket => new ExportTheaterTicketDto()
                        {
                            Price = ticket.Price,
                            RowNumber = ticket.RowNumber
                        })
                        .OrderByDescending(ticket => ticket.Price)
                        .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToArray();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(ExportPlayWithActorsDto[]), xmlRoot);

            ExportPlayWithActorsDto[] dtos = context
                .Plays
                .Where(p => p.Rating <= rating)
                .Include(p => p.Casts)
                .ToArray()
                .Select(p => new ExportPlayWithActorsDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                        .Where(c => c.IsMainCharacter)
                        .Select(c => new ExportPlayActorDto()
                        {
                            FullName = c.FullName,
                            MainCharacter =
                                $"Plays main character in '{p.Title}'."
                        })
                        .OrderByDescending(c => c.FullName)
                        .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToArray();

            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dtos, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
