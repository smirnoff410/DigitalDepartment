var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//заменить путь на свое локальное хранилище изображений
var storagePath = "D:\\Projects\\DigitalDepartment\\ObjectDetect\\storage";

app.MapPost("/upload_image", async (HttpContext context) =>
{
    var fileName = Guid.NewGuid().ToString();

    using (var fileStream = new FileStream($"{storagePath}\\{fileName}.jpg", FileMode.Create))
    {
        await context.Request.Body.CopyToAsync(fileStream);
    }

    return Results.Ok(new { FileName = fileName });
});

app.Run();
