namespace MusicHub
{
    using System;
    using System.Linq;
    using Data;
    using Initializer;
    using System.Text;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            int duration = int.Parse(Console.ReadLine());

            string result = ExportSongsAboveDuration(context, duration);

            Console.WriteLine(result);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var albums = context.Albums
                            .Where(a => a.ProducerId.Value == producerId)
                            .Select(a => new
                            {
                                Name = a.Name,
                                ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                                ProducerName = a.Producer.Name,
                                Songs = a.Songs
                                            .OrderByDescending(s => s.Name)
                                            .ThenBy(s => s.Writer.Name)
                                            .Select(s => new
                                            {
                                                s.Name,
                                                s.Price,
                                                Writer = s.Writer.Name
                                            })
                                            .ToList(),
                                Price = a.Price

                            })
                            .ToList()
                            .OrderByDescending(a => a.Price);

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int x = 1;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{x}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.Writer}");

                    x++;
                }

                sb.AppendLine($"-AlbumPrice: {album.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songsAboveDuration = context.Songs
                                        .Include(s => s.SongPerformers)
                                        .ThenInclude(sp => sp.Performer)
                                        .Include(s => s.Writer)
                                        .Include(s => s.Album)
                                        .ThenInclude(a => a.Producer)
                                        .ToList()
                                        .Where(s => s.Duration.TotalSeconds > duration)
                                        .Select(s => new
                                        {
                                            Name = s.Name,
                                            PerformerName = s.SongPerformers
                                                                .Select(sp => $"{sp.Performer.FirstName} {sp.Performer.LastName}")
                                                                .FirstOrDefault(),
                                            Writer = s.Writer.Name,
                                            AlbumProducer = s.Album.Producer.Name,
                                            Duration = s.Duration.ToString("c")
                                        })
                                        .OrderBy(s => s.Name)
                                        .ThenBy(s => s.Writer)
                                        .ThenBy(s => s.PerformerName);

            int index = 1;

            foreach (var song in songsAboveDuration)
            {
                sb.AppendLine($"-Song #{index}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerformerName}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");

                index++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}
