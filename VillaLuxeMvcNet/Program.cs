using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VillaLuxeMvcNet.Data;
using VillaLuxeMvcNet.Helpers;
using VillaLuxeMvcNet.Repositories;
using VillaLuxeMvcNet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddSingleton<HelperUploadImages>();
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie();

/*string connectionString = builder.Configuration.GetConnectionString("SqlServerVillas");
*//*builder.Services.AddTransient<RepositoryVillas>();
builder.Services.AddTransient<RepositotyUsuarios>();*/
//builder.Services.AddTransient<ServiceVillas>();
/*builder.Services.AddDbContext<VillaContext>
	(options => options.UseSqlServer(connectionString));*/
string azureKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");
BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);
builder.Services.AddTransient<IRepositoryVillas, ServiceVillas>();
//string connectionString =
//    builder.Configuration.GetConnectionString("SqlServerHospital");
//builder.Services.AddDbContext<HospitalContext>
//    (options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
