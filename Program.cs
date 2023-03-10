using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using AuthorizationServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbContext>(options =>
{
    options.UseInMemoryDatabase(nameof(DbContext));
    options.UseOpenIddict();
});
builder.Services.AddOpenIddict()
    .AddCore(options =>
{
    options.UseEntityFrameworkCore()
    .UseDbContext<DbContext>();
})
    .AddServer(options =>
    {
        options
        .AllowAuthorizationCodeFlow()
        .RequireProofKeyForCodeExchange()
        .AllowClientCredentialsFlow()
        .AllowRefreshTokenFlow();
        options
        .AddEphemeralEncryptionKey()
        .AddEphemeralSigningKey()
        .DisableAccessTokenEncryption();
        options
        .RegisterScopes("api");
     
        options
        .SetAuthorizationEndpointUris("/connect/authorize")
        .SetTokenEndpointUris("/connect/token")
        .SetUserinfoEndpointUris("/connect/userinfo");
        options
        .UseAspNetCore()
        .EnableTokenEndpointPassthrough()
        .EnableAuthorizationEndpointPassthrough()
        .EnableUserinfoEndpointPassthrough();


    });

builder.Services.AddHostedService<TestData>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/account/login";
        });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
