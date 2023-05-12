var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// STEP 1:
builder.Services.AddPayDotNet(builder.Configuration)
    .AddInMemoryStore()
    .AddStripe(config =>
    {
        config.
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// STEP 1:
// REQUIRED TO ADD in RazorPage app.
app.MapControllers();

app.Run();