using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosConnection")));

// Registro de Repositorios (AddScoped)
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Carga de datos iniciales (Seed Data) si la tabla está vacía
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    // 1. Categorías iniciales (3)
    if (!await context.Categorias.AnyAsync())
    {
        var categorias = new List<Categoria>
        {
            new Categoria { Nombre = "Tecnología e Innovación", Activo = true },
            new Categoria { Nombre = "Gestión y Emprendimiento", Activo = true },
            new Categoria { Nombre = "Diseño y Experiencia de Usuario", Activo = true }
        };
        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();
    }

    // 2. Eventos iniciales (4)
    if (!await context.Eventos.AnyAsync())
    {
        var catTecnologia = await context.Categorias.FirstAsync(c => c.Nombre == "Tecnología e Innovación");
        var catGestion = await context.Categorias.FirstAsync(c => c.Nombre == "Gestión y Emprendimiento");
        var catDiseno = await context.Categorias.FirstAsync(c => c.Nombre == "Diseño y Experiencia de Usuario");

        var eventos = new List<Evento>
        {
            new Evento
            {
                Titulo = "Cumbre de Inteligencia Artificial 2026",
                Fecha = DateTime.Today.AddDays(15).AddHours(9),
                Lugar = "Auditorio Principal Tecsup",
                Aforo = 200,
                Precio = 50.00m,
                Modalidad = "Presencial",
                CategoriaId = catTecnologia.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Seminario de Cloud y Microservicios",
                Fecha = DateTime.Today.AddDays(20).AddHours(18),
                Lugar = "Plataforma Virtual Teams",
                Aforo = 500,
                Precio = 0.00m,
                Modalidad = "Virtual",
                CategoriaId = catTecnologia.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Taller Práctico de Lean Startup",
                Fecha = DateTime.Today.AddDays(25).AddHours(10),
                Lugar = "Campus San Borja - Aula 302",
                Aforo = 40,
                Precio = 120.00m,
                Modalidad = "Presencial",
                CategoriaId = catGestion.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Masterclass de UX/UI para Aplicaciones Web",
                Fecha = DateTime.Today.AddDays(30).AddHours(16),
                Lugar = "Sala Zoom Pro",
                Aforo = 150,
                Precio = 35.50m,
                Modalidad = "Virtual",
                CategoriaId = catDiseno.Id,
                Activo = true
            }
        };
        await context.Eventos.AddRangeAsync(eventos);
        await context.SaveChangesAsync();
    }

    // 3. Asistentes iniciales (5)
    if (!await context.Asistentes.AnyAsync())
    {
        var asistentes = new List<Asistente>
        {
            new Asistente
            {
                Nombres = "Carlos Mendoza Silva",
                Dni = "71234567",
                Correo = "carlos.mendoza@email.com",
                Telefono = "987654321",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Lucía Fernández Ramos",
                Dni = "72345678",
                Correo = "lucia.fernandez@email.com",
                Telefono = "987654322",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Mateo Quispe Huamán",
                Dni = "73456789",
                Correo = "mateo.quispe@email.com",
                Telefono = "987654323",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Valeria Torres Romero",
                Dni = "74567890",
                Correo = "valeria.torres@email.com",
                Telefono = "987654324",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Jorge Vargas Castillo",
                Dni = "75678901",
                Correo = "jorge.vargas@email.com",
                Telefono = "987654325",
                Activo = true
            }
        };
        await context.Asistentes.AddRangeAsync(asistentes);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
