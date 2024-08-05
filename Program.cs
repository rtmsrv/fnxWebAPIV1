using fnxWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo

    {

        Version = "v1",

        Title = "JWT Api",

        Description = "Secures API using JWT",

        Contact = new OpenApiContact

        {

            Name = "yossi tzdaka",

            Email = "service@rtmsrv.com",

        }

    });

    // To Enable authorization using Swagger (JWT)

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()

    {

        Name = "Authorization",

        Type = SecuritySchemeType.ApiKey,

        Scheme = "Bearer",

        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement

                {

                    {

                          new OpenApiSecurityScheme

                            {

                                Reference = new OpenApiReference

                                {

                                    Type = ReferenceType.SecurityScheme,

                                    Id = "Bearer"

                                }

                            },

                            new string[] {}

                    }

                });

});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

.AddJwtBearer(options =>

{

 options.TokenValidationParameters = new TokenValidationParameters

 {

     ValidateIssuer = true,

     ValidateAudience = true,

     ValidateLifetime = true,

     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),

     ValidateIssuerSigningKey = true,

     ValidIssuer = builder.Configuration["Jwt:Issuer"],

     ValidAudience = builder.Configuration["Jwt:Audience"],

 };

});

builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
