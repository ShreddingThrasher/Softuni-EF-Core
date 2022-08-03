namespace Theatre.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Theatre.Data;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.DataProcessor.ImportDto;
    using System.IO;
    using Theatre.Data.Models;
    using System.Globalization;
    using Theatre.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPlayDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportPlayDto[] playDtos = (ImportPlayDto[])serializer.Deserialize(reader);

            List<Play> plays = new List<Play>();

            foreach (var dto in playDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan duration = TimeSpan.ParseExact(dto.Duration, "c", CultureInfo.InvariantCulture);

                if(duration.Hours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isGenreValid = Enum.TryParse(dto.Genre, out Genre genre);

                if (!isGenreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if(dto.Description == string.Empty)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                plays.Add(new Play()
                {
                    Title = dto.Title,
                    Duration = duration,
                    Rating = dto.Rating,
                    Genre = (Genre)Enum.Parse(typeof(Genre), dto.Genre),
                    Description = dto.Description,
                    Screenwriter = dto.Screenwriter
                });

                sb.AppendLine(string.Format(SuccessfulImportPlay, dto.Title, dto.Genre, dto.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Casts");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCastDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportCastDto[] castDtos = (ImportCastDto[])serializer.Deserialize(reader);

            List<Cast> casts = new List<Cast>();

            foreach (var dto in castDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                casts.Add(new Cast()
                {
                    FullName = dto.FullName,
                    IsMainCharacter = dto.IsMainCharacter,
                    PhoneNumber = dto.PhoneNumber,
                    PlayId = dto.PlayId
                });

                sb.AppendLine(string.Format(SuccessfulImportActor,
                    dto.FullName, dto.IsMainCharacter ? "main" : "lesser"));
            }

            context.Casts.AddRange(casts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTheatreWithTicketsDto[] theaterDtos = JsonConvert
                .DeserializeObject<ImportTheatreWithTicketsDto[]>(jsonString);

            List<Theatre> theatres = new List<Theatre>();

            foreach (var dto in theaterDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatre = new Theatre()
                {
                    Name = dto.Name,
                    NumberOfHalls = dto.NumberOfHalls,
                    Director = dto.Director
                };

                foreach (var ticketDto in dto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    theatre.Tickets.Add(new Ticket()
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId
                    });
                }

                theatres.Add(theatre);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, 
                    theatre.Name, theatre.Tickets.Count));
            }

            context.Theatres.AddRange(theatres);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
