using System.Diagnostics.Metrics;

namespace AspServicesTimelifeApp
{
    public interface IMessage
    {
        string Message { get; }
    }

    public class RuMessageService : IMessage
    {
        public string Message => "Привет мир";
    }

    public class EnMessageService : IMessage
    {
        public string Message => "Hello world";
    }

    public class MessageMiddleware
    {
        RequestDelegate next;
        IEnumerable<IMessage> messagesServices;
        public MessageMiddleware(RequestDelegate next, IEnumerable<IMessage> messagesServices)
        {
            this.next = next;
            this.messagesServices = messagesServices;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            string result = "";
            foreach (var mess in messagesServices)
                result += $"<h2>{mess.Message}</h2>";

            await context.Response.WriteAsync(result);
        }
    }

    interface IGenerator
    {
        int GenerateValue();
    }
    interface IReader
    {
        int ReadValue();
    }
    class ValueStorage : IGenerator, IReader
    {
        int value;
        public int GenerateValue()
        {
            value = new Random().Next(0, 1000000);
            return value;
        }
        public int ReadValue()
        {
            return value;
        }
    }

    class GeneratorMiddleware
    {
        RequestDelegate next;
        IGenerator generator;
        public GeneratorMiddleware(RequestDelegate next, IGenerator generator)
        {
            this.next = next;
            this.generator = generator;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/gen")
                await context.Response.WriteAsync($"New Value: {generator.GenerateValue()}");
            else
                await next.Invoke(context);
        }
    }

    class ReaderMiddleware
    {
        RequestDelegate next;
        IReader reader;
        public ReaderMiddleware(RequestDelegate next, IReader reader)
        {
            this.next = next;
            this.reader = reader;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync($"Current Value: {reader.ReadValue()}");
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            //WorkWithTimer();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<ValueStorage>();
            builder.Services.AddSingleton<IGenerator>(serv => serv.GetRequiredService<ValueStorage>());
            builder.Services.AddSingleton<IReader>(serv => serv.GetRequiredService<ValueStorage>());

            var app = builder.Build();

            app.UseMiddleware<GeneratorMiddleware>();
            app.UseMiddleware<ReaderMiddleware>();


            app.Run();
        }

        public static void WorkWithCounter()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddTransient<ICounter, RandomCounter>();
            builder.Services.AddTransient<CounterServices>();

            var app = builder.Build();

            app.UseMiddleware<CounterMiddleware>();
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("Example Service<br><a href='rand'>Random page</a>");
            });

            app.Run();
        }
        public static void WorkWithTimer()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddTransient<TimeService>();
            builder.Services.AddTransient<ITimer, TimerShort>();
            builder.Services.AddTransient<ITimer, TimerLong>();

            var app = builder.Build();

            app.UseMiddleware<TimerMiddleware>();

            app.Run();
        }
        public static void WorkWithMessage()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddTransient<IMessage, RuMessageService>();
            builder.Services.AddTransient<IMessage, EnMessageService>();
            var app = builder.Build();

            app.UseMiddleware<MessageMiddleware>();

            app.Run();
        }

    }
}