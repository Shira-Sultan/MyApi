using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MyApi.Interfaces;
using MyApi.Services;
using MyApi.Middlewares;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.TokenValidationParameters = myBooksTokenService.GetTokenValidationParameters();
        });

builder.Services.AddAuthorization(cfg =>
   {
       cfg.AddPolicy("Admin", policy => policy.RequireClaim("type","Admin"));
       cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User","Admin"));
   });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo { Title = "Books", Version = "v1" });
   c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
       In = ParameterLocation.Header,
       Description = "Please enter JWT with Bearer into field",
       Name = "Authorization",
       Type = SecuritySchemeType.ApiKey
   });
   c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme
                        {
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                    new string[] {}
                }
   });
});

builder.Services.AddControllers();
builder.Services.AddBook();
builder.Services.AddUser();
// builder.Services.AddSingleton<IBooksService,BooksService>();
// builder.Services.AddSingleton<IUsersService,UsersService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// app.UselogMiddleware("file.txt");
// app.UseTokenExpMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseOurLog();
// app.Use(async (c, n) => 
// {
//     await c.Response.WriteAsync("My 3rd Middleware start\n");
//     await n();
//     await c.Response.WriteAsync("My 3rd Middleware end\n");
// });
// app.Use(async (context, next) => 
// {
//     await context.Response.WriteAsync("My 4th Middleware start\n");
//     await next();
//     await context.Response.WriteAsync("My 4th Middleware end\n");
// });
// app.UseOur5th();
// app.Run(async context => await context.Response.WriteAsync("My terminal Middleware\n"));


app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
