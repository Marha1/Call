using Application;
using Application.Middleware;
using Infrastructure.ChatHub;

var builder = WebApplication.CreateBuilder(args);

// Добавляем все сервисы из расширения
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Middleware Configuration
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ProfanityMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub").RequireCors("AllowSpecificOrigin");
});

app.Run();