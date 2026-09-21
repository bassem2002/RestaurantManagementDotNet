using Blazored.LocalStorage;
using Front;
using Front.Services;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Ajustez l'URL à celle exposée par votre API (https recommandé en dev si le front est en https)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7281/") });

// Enregistrer le service unique d'auth (implémente IAuthService et AuthenticationStateProvider)
builder.Services.AddScoped<AuthService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>());

// Services métier
builder.Services.AddScoped<CategorieServices>();
builder.Services.AddScoped<ItemServices>();
builder.Services.AddScoped<ClientServices>();
builder.Services.AddScoped<AdminOrderService>();
builder.Services.AddScoped<OrderClientService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<UserProfileService>();

// Services utilitaires
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<NotificationService>();

// Authorization
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

//logout service
builder.Services.AddScoped<AuthService>();



await builder.Build().RunAsync();
