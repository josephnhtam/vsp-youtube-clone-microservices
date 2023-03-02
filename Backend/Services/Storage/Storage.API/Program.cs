namespace Storage {
    public class Program {
        public static async Task Main (string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.ConfigureServices();

            app.ConfigurePipeline();

            await app.InitializeAsync();

            await app.RunAsync();
        }
    }
}