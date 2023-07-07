using RecipesAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

app.UseAuthorization();

app.MapControllers();

app.MapPut("/recipe", async (DataContext context, List<Recipe> recipes) =>
{
    foreach (var item in recipes)
    {
        var find = await context.Recipes.FindAsync(item.Id);
        if (find is null)
        {
            await context.Recipes.AddAsync(item);
        }
        else
        {
            find.Description = item.Description;
            find.Name = item.Name;
            find.imagePath = item.imagePath;
            find.Ingredients = item.Ingredients;
        }

        await context.SaveChangesAsync();
    }

    return Results.Ok(await context.Recipes.ToListAsync());
});

app.MapGet("/recipe", async (DataContext context) => Results.Ok(await context.Recipes.ToListAsync()));

app.Run();