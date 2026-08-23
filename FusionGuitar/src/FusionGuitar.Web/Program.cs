using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FusionGuitar.Web;
using FusionGuitar.Web.Interop;
using FusionGuitar.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AudioInterop>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<ProgressService>();

await builder.Build().RunAsync();
