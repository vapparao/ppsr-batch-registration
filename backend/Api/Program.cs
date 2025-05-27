using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Features.BatchOperation.Commands;
using Application.Features.BatchOperation.Queries;
using Infrastructure.Services.SignalR;
using Infrastructure.Services.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Application.Mappings.BatchProfile).Assembly);
builder.Services.AddAutoMapper(typeof(Application.Mappings.RegistrationProfile).Assembly);
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateBatchCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ProcessBatchCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetBatchByChecksumQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetBatchByIdQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetBatchesByClientIdQuery).Assembly);

});
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INotificationPublisher, BatchHubService>();



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHub<BatchHub>("/batchHub");
app.Run();
