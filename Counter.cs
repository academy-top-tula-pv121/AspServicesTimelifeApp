namespace AspServicesTimelifeApp
{
    public interface ICounter
    {
        int Value { get; }
    }

    public class RandomCounter : ICounter
    {
        static Random rand = new Random();
        int value;
        public RandomCounter()
        {
            value = rand.Next(0, 1000000);
        }
        public int Value
        {
            get { return value; }
        }
    }

    public class CounterServices
    {
        public ICounter Counter { get; }
        public CounterServices(ICounter counter)
        {
            Counter = counter;
        }
    }

    /*
    public class CounterMiddleware
    {
        RequestDelegate next;
        int count = 0;
        public CounterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, ICounter counter, CounterServices services)
        {
            count++;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(@$"Request #{count}, 
Counter Value: {counter.Value}
Service Counter value: {services.Counter.Value}");
        }
    }
    */

    // include Service with Constructor
    //public class CounterMiddleware
    //{
    //    RequestDelegate next;
    //    CounterServices service;
    //    public CounterMiddleware(RequestDelegate next, CounterServices service)
    //    {
    //        this.next = next;
    //        this.service = service;
    //    }
    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        if (context.Request.Path == "/rand")
    //        {
    //            context.Response.ContentType = "text/html; charset=utf-8";
    //            await context.Response.WriteAsync($"Service Counter value: {service.Counter.Value}");
    //        }
    //        else
    //            await next.Invoke(context);
    //    }
    //}

    public class CounterMiddleware
    {
        RequestDelegate next;
        public CounterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, CounterServices service)
        {
            if (context.Request.Path == "/rand")
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"Service Counter value: {service.Counter.Value}");
            }
            else
                await next.Invoke(context);
        }
    }
}
