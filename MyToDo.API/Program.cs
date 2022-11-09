using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MyToDo.API;
using MyToDo.API.Context;
using MyToDo.API.Context.Repository;
using MyToDo.API.Extensions;
using MyToDo.API.Service;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

//�������ݿ����ӷ�ʽ������ȡappsettings.json �е�ConnectionStrings
builder.Services.AddDbContext<MyToDoContext>(option =>
{
   var connectionString = builder.Configuration.GetConnectionString("ToDoConnection");
   option.UseSqlite(connectionString);
}).AddUnitOfWork<MyToDoContext>()
            .AddCustomRepository<ToDo, ToDoRepository>()
            .AddCustomRepository<Memo, MemoRepository>()
            .AddCustomRepository<User, UserRepository>();

builder.Services.AddTransient<IToDoService, ToDoService>();
builder.Services.AddTransient<IMemoService, MemoService>();
builder.Services.AddTransient<ILoginService, LoginService>();
//���AutoMapper
var automapperConfog = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProFile());
});

builder.Services.AddSingleton(automapperConfog.CreateMapper());

//////////////////////////////////////////////////////////////////////////////////////////////////////
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   
   
}

app.UseAuthorization();

app.MapControllers();

app.Run();
