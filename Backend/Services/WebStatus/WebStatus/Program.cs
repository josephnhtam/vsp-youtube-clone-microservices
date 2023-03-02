namespace WebStatus {
    public class Program {
        public static void Main (string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecksUI()
                            .AddInMemoryStorage();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseHealthChecksUI(options => {
                options.UIPath = "/hc-ui";
            });

            app.Map("/", () => Results.LocalRedirect("/hc-ui"));

            app.Run();
        }
    }
}