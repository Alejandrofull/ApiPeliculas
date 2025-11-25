using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PeliculasFrontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

const string ApiBaseUrl = "https://localhost:7281/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(ApiBaseUrl)
});

await builder.Build().RunAsync();
