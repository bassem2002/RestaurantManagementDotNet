using Backend.Data;
using Backend.Models;
using Backend.Repositories;
using Backend.Repository.Implementations;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================= CORS (FRONT 5282) =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy
            .WithOrigins("https://localhost:5282") // frontend origin here
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ================= Controllers =================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ================= Database =================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cnx")));

// ================= Identity =================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ================= JWT Authentication =================
var jwtKey = builder.Configuration["JWT:SecretKey"];
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// ================= Swagger =================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// ================= Repositories =================
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

// ================= Authorization Policies =================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ================= Build App =================
var app = builder.Build();

app.UseStaticFiles(); // Serve static files like images

// ================= Middleware Pipeline =================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// ⚠️ Must be before Authentication/Authorization
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// ================= 🔐 SEED ROLES & ADMIN =================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        // ===== 1️⃣ CRÉATION DES RÔLES =====
        string[] roles = { "Admin", "Client" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"[SEED] ✅ Rôle créé : {role}");
            }
        }

        // ===== 2️⃣ CRÉATION DE L'ADMIN =====
        var adminUser = await userManager.FindByNameAsync("admin");

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@admin.com",
                FullName = "Super Admin",
                Address = "Administration",
                Phone = "00000000",
                City = "Tunis",
                PostalCode = "1000",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123..");

            if (!result.Succeeded)
            {
                Console.WriteLine("[SEED] ❌ Erreur création admin");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($" - {error.Description}");
                }
            }
            else
            {
                Console.WriteLine("[SEED] ✅ Admin créé");
            }
        }

        // ===== 3️⃣ ASSIGNER LE RÔLE ADMIN =====
        var rolesAdmin = await userManager.GetRolesAsync(adminUser);

        if (!rolesAdmin.Contains("Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("[SEED] 🔐 Rôle Admin assigné");
        }

        Console.WriteLine("[SEED] 🎉 Initialisation terminée");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SEED] ❌ ERREUR : {ex.Message}");
    }
}

app.Run();
