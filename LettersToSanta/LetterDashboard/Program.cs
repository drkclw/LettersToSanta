using Azure.Identity;
using LetterDashboard.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<DatabaseService>();

string? cs = Environment.GetEnvironmentVariable("TextExtractionConfigCS");
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var settings = config.Build();

    config.AddAzureAppConfiguration(options =>
    {
        options.Connect(cs)
                    .Select(KeyFilter.Any, LabelFilter.Null)
                    .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(new DefaultAzureCredential());
                })                
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("Sentinel", refreshAll: true);
                });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
