namespace AspServicesTimelifeApp
{
    public interface ITimer
    {
        string Time { get; }
    }
    public class TimerLong : ITimer
    {
        public string Time { get; }
        public TimerLong()
        {
            Time = DateTime.Now.ToLongTimeString();
        }
    }

    public class TimerShort : ITimer
    {
        public string Time { get; }
        public TimerShort()
        {
            Time = DateTime.Now.ToShortTimeString();
        }
    }
    public class TimeService
    {
        //ITimer timer;
        IEnumerable<ITimer> timers;
        public TimeService(IEnumerable<ITimer> timers)
        {
            this.timers = timers;
        }

        //public string GetTime()
        //{
        //    return timer.Time;
        //}
        public string GetTime()
        {
            foreach (var timer in timers)
                if(timer is TimerShort)
                    return timer.Time;
            return "";
            //string result = "";
            //foreach (var timer in timers)
            //    result += timer.Time + " ";
            //return result;
        }
    }

    public class TimerMiddleware
    {
        RequestDelegate next;
        TimeService timeService;
        public TimerMiddleware(RequestDelegate next, TimeService timeService)
        {
            this.next = next;
            this.timeService = timeService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync($"Time: {timeService.GetTime()}");
        }
    }

}
