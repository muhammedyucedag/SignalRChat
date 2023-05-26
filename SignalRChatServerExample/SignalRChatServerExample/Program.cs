using SignalRChatServerExample.Hubs;

namespace SignalRChatServerExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options=> 
            options.AddDefaultPolicy(policy => 
            policy.AllowCredentials()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed(x=>true)));
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<ChatHub>("/chathub");
            app.MapControllers();

            app.Run();
        }
    }
}