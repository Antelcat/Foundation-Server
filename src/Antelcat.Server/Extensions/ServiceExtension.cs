﻿using System.Data;
using System.Net;
using Antelcat.Server.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Antelcat.Server.Extensions;

public static partial class ServiceExtension
{
    public static IServiceCollection AddJwtSwaggerGen(this IServiceCollection collection)
        => collection.AddSwaggerGen(static o =>
        {
            o.OperationFilter<AuthorizationFilter>();
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Jwt Token Format like [ Bearer {Token} ]",
                Name = nameof(Authorization),
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
        });

    public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IConfiguration>();

    public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider, string field) =>
        serviceProvider.GetConfiguration().GetSection(field);

    public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider, params string[] fields) =>
        serviceProvider.GetConfiguration(string.Join(':', fields));
    
    public static string GetConfigurationString(this IServiceProvider serviceProvider, string field) =>
        serviceProvider.GetConfiguration()[field] 
        ?? throw new NoNullAllowedException($"Specified filed {field} is null");

    public static string GetConfigurationString(this IServiceProvider serviceProvider, params string[] fields) =>
        serviceProvider.GetConfigurationString(string.Join(':', fields));
    
    public static string GetConfigurationString(this IConfiguration configuration, params string[] fields) =>
        configuration[string.Join(':', fields)]
        ?? throw new NoNullAllowedException($"Specified filed {string.Join(':', fields)} is null");
}