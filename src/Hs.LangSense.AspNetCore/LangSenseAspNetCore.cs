using System.Text;using Microsoft.AspNetCore.Builder;using Microsoft.AspNetCore.Http;using Microsoft.Extensions.DependencyInjection;using Hs.LangSense;
namespace Hs.LangSense.AspNetCore;
/// <summary>Normalizes supported request query values before downstream processing.</summary>
public sealed class LangSenseRequestNormalizationMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){if(context.Request.QueryString.HasValue)context.Request.QueryString=new QueryString(Normalizer.Normalize(Uri.UnescapeDataString(context.Request.QueryString.Value!)));await next(context);}}
/// <summary>Stores lightweight request analysis in HttpContext.Items.</summary>
public sealed class LangSenseRequestAnalysisMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){context.Items["LangSense.RequestAnalysis"]=new {Path=context.Request.Path.ToString(),Method=context.Request.Method,Query=context.Request.QueryString.ToString()};await next(context);}}
/// <summary>Runs LangSense detection against supported request query values.</summary>
public sealed class LangSenseValidationMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){if(context.Request.QueryString.HasValue){foreach(var item in context.Request.Query)foreach(var v in item.Value) _=SmartValue.Analyze(v);}await next(context);}}
/// <summary>Sanitizes sensitive query values before downstream processing.</summary>
public sealed class LangSenseSanitizationMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){if(context.Request.QueryString.HasValue)context.Request.QueryString=new QueryString(SensitiveData.Redact(Uri.UnescapeDataString(context.Request.QueryString.Value!)));await next(context);}}
/// <summary>Provides scrubbed request data for application logging.</summary>
public sealed class LangSenseSensitiveLoggingMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){context.Items["LangSense.ScrubQuery"]=SensitiveData.Mask(context.Request.QueryString.Value??"");await next(context);}}
/// <summary>Masks supported sensitive values in text and JSON responses.</summary>
public sealed class LangSenseResponseMaskingMiddleware(RequestDelegate next){public async Task Invoke(HttpContext context){var original=context.Response.Body;await using var buffer=new MemoryStream();context.Response.Body=buffer;try{await next(context);buffer.Position=0;var body=await new StreamReader(buffer,Encoding.UTF8).ReadToEndAsync();var contentType=context.Response.ContentType??"";if(contentType.Contains("json",StringComparison.OrdinalIgnoreCase)||contentType.StartsWith("text/",StringComparison.OrdinalIgnoreCase))body=SensitiveData.Mask(body);var bytes=Encoding.UTF8.GetBytes(body);context.Response.Body=original;context.Response.ContentLength=bytes.Length;await original.WriteAsync(bytes);}finally{context.Response.Body=original;}}}
/// <summary>Registers LangSense and exposes ASP.NET Core middleware extensions.</summary>
public static class LangSenseAspNetCoreExtensions
{
 /// <summary>Enables AddLangSense.</summary>
 /// <summary>Enables UseLangSenseNormalization.</summary>
 /// <summary>Enables UseLangSenseRequestAnalysis.</summary>
 /// <summary>Enables UseLangSenseValidation.</summary>
 /// <summary>Enables UseLangSenseSanitization.</summary>
 /// <summary>Enables UseLangSenseSensitiveLogging.</summary>
 /// <summary>Enables UseLangSenseResponseMasking.</summary>
 public static IServiceCollection AddLangSense(this IServiceCollection services,Action<LangSenseOptions>? configure=null){var o=new LangSenseOptions();configure?.Invoke(o);services.AddSingleton(o);return services;}
 public static IApplicationBuilder UseLangSenseNormalization(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseRequestNormalizationMiddleware>();
 public static IApplicationBuilder UseLangSenseRequestAnalysis(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseRequestAnalysisMiddleware>();
 public static IApplicationBuilder UseLangSenseValidation(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseValidationMiddleware>();
 public static IApplicationBuilder UseLangSenseSanitization(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseSanitizationMiddleware>();
 public static IApplicationBuilder UseLangSenseSensitiveLogging(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseSensitiveLoggingMiddleware>();
 public static IApplicationBuilder UseLangSenseResponseMasking(this IApplicationBuilder app)=>app.UseMiddleware<LangSenseResponseMaskingMiddleware>();
}
