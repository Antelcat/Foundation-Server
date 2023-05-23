﻿using Antelcat.Foundation.Core.Implements.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Antelcat.Foundation.Server.Implements;

public abstract class ControllerActivatorBase<TServiceProvider> : IControllerActivator 
    where TServiceProvider : IServiceProvider
{
    protected abstract TServiceProvider ProvideService(IServiceProvider provider);
    public object Create(ControllerContext context)
    {
        if (context == null)
            throw new ArgumentNullException($"{nameof(ControllerContext)} is null");
        var type = context.ActionDescriptor.ControllerTypeInfo.AsType();
        var provider = context.HttpContext.RequestServices;
        if (provider is not TServiceProvider)
            provider = ProvideService(context.HttpContext.RequestServices);
        return provider.GetRequiredService(type);
    }
    public void Release(ControllerContext context, object controller)
    {
        if (context == null) throw new ArgumentNullException($"{nameof(ControllerContext)} is null");
        switch (controller)
        {
            case null:
                throw new ArgumentNullException(nameof(controller));
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
}

public class AutowiredControllerActivator<TAttribute> : ControllerActivatorBase<AutowiredServiceProvider<TAttribute>> 
    where TAttribute : Attribute
{
    protected override AutowiredServiceProvider<TAttribute> ProvideService(IServiceProvider provider) => new(provider);
}

public class CachedAutowiredControllerActivator<TAttribute> : ControllerActivatorBase<CachedAutowiredServiceProvider<TAttribute>> 
    where TAttribute : Attribute
{
    protected override CachedAutowiredServiceProvider<TAttribute> ProvideService(IServiceProvider provider) => new(provider);
}