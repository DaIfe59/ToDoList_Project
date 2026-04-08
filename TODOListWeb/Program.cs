using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TODOListWeb;
using TODOListWeb.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Здесь подключаем ApiService
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5143/") });
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();