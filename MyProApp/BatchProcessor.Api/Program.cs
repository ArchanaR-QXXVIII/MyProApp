using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthorization(options =>
//	options.AddPolicy("admin", policy =>
//	  policy.RequireClaim(JwtClaimTypes.Role, "admin")
//	)
//);

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors();
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "allowCors",
	builder =>
	{
		builder.WithOrigins("https://localhost/4200", "http://localhost/4200")
		.AllowCredentials()
		.AllowAnyHeader()
		.AllowAnyMethod();
	});
});

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors();
app.UseRouting();


app.MapPost("/process", ([FromBody] DataObject input) =>
{
	var result = new Result { /* populate based on input */ };
	return Results.Ok(result);
}).RequireAuthorization()
			.WithName("Process")
			.WithSummary("Post Call");
			//.WithOpenApi();



app.MapControllers();

app.Run();


