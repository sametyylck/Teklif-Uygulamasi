using BL.Control.IdControl;
using BL.Control.Resim;
using BL.Control.Sepet;
using BL.Control.Stoklar;
using DAL.Service.KurService;
using BL.UserService;
using DAL.DTO;
using DAL.Interface;
using DAL.Models;
using DAL.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Globalization;
using System.Text;
using Validation.Kullanicilar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using BL.Control.Teklif;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IndustrialKýtchenEquipmentsDbContext>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standart Yetkilendirme Baslýgý (\"token\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
          new string[] { }
      }
  });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

//builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
//       policy =>
//       {
//           policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//       }));

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("NgOrigins", b =>
    {
        b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services.AddMemoryCache();
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

//Repository InterFace
builder.Services.AddScoped<IKullanýcýlar, RegisterRepository>();
builder.Services.AddScoped<IMusteriler, MusterilerRepository>();
builder.Services.AddScoped<ISepet, SepetRepository>();
builder.Services.AddScoped<IStokKategori, StokKategoriRepository>();
builder.Services.AddScoped<IStoklar, StoklarRepository>();
builder.Services.AddScoped<IStokResim, StokResimRepository>();
builder.Services.AddScoped<IParaBirimi, ParaBirimiRepository>();
builder.Services.AddScoped<ITeklifler, TekliflerRepository>();
builder.Services.AddScoped<IResim, Resim>();
builder.Services.AddScoped<IDil, DilRepository>();
builder.Services.AddScoped<IBirim, BirimRepository>();
builder.Services.AddScoped<IRevizyon, Revizyon>();
builder.Services.AddScoped<IPdf , Deneme12>();
builder.Services.AddScoped<IAltFirmaRepository, AltFirmaRepository>();
builder.Services.AddScoped<IKullanýcý, KullanýcýRepository>();
builder.Services.AddScoped<ITeklifControl, TeklifControl>();
builder.Services.AddScoped<IGrup, GrupRepository>();






builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(builder.Configuration.GetConnectionString("SqlConnection")));
//FluentValidation
builder.Services.AddScoped<IValidator<RegisterDTO>, KullanicilarValidations>();
builder.Services.AddScoped<IValidator<MusteriInsert>, MusteriValidation>();
builder.Services.AddScoped<IValidator<MusteriUpdate>, MusteriUpdateValidation>();
builder.Services.AddScoped<IValidator<IdControl>, IdControlValidation>();
builder.Services.AddScoped<IValidator<SepetDetayUpdate>, SepetUpdateDetayValidation>();
builder.Services.AddScoped<IValidator<SepetDetayInsert>, SepetInsertDetayValidation>();
builder.Services.AddScoped<IValidator<SepetInsert>, SepetInsertValidations>();
builder.Services.AddScoped<IValidator<SepetUpdate>, SepetUpdateValidations>();



//Control
builder.Services.AddScoped<IStoklarService, StoklarService>();
builder.Services.AddScoped<IDControl, IdControlService>();
builder.Services.AddScoped<IKurService, KurService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISepetControl, SepetControl>();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("NgOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
