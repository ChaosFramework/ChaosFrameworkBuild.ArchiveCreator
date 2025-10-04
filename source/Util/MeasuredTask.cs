using Stopwatch = System.Diagnostics.Stopwatch;

namespace ChaosFrameworkBuild.ArchiveCreator.Util
{
    class MeasuredTask
    {
        static readonly string FLOAT_FORMAT = ChaosUtil.Debug.StringUtils.GetFloatFormat(0, 3);

        public static Result Run<Result>(ChaosArchive task, System.Func<Result> action, string successMessage)
        {
            Stopwatch tm = new Stopwatch();
            tm.Start();
            Result result = action();
            tm.Stop();
            double secs = tm.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            task.LogMsg(string.Format(successMessage, secs.ToString(FLOAT_FORMAT)));
            return result;
        }
    }
}
