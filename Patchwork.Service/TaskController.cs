using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Patchwork.Tasks;

namespace Patchwork.Service
{
    [ApiController]
    [Route("/")]
    public class TaskController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> taskTokens = new();
        private static readonly List<string> runningTasks = new();
        private static readonly object lockObj = new();

        [HttpPost("start")]
        public IActionResult Start()
        {
            string taskId = Guid.NewGuid().ToString();
            var cts = new CancellationTokenSource();
            taskTokens[taskId] = cts;
            lock (lockObj) runningTasks.Add(taskId);
            _ = Task.Run(async () =>
            {
                var task = new DemoTask();
                await task.RunAsync(new Dictionary<string, string>(), cts.Token);
                lock (lockObj) runningTasks.Remove(taskId);
                taskTokens.TryRemove(taskId, out _);
            });
            return new JsonResult(new { taskId });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            lock (lockObj)
            {
                return new JsonResult(new { runningTasks = runningTasks.ToArray() });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel()
        {
            string? taskId = null;
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var doc = System.Text.Json.JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("taskId", out var idProp))
                {
                    taskId = idProp.GetString();
                }
            }
            if (taskId != null && taskTokens.TryRemove(taskId, out var cts))
            {
                cts.Cancel();
                lock (lockObj) runningTasks.Remove(taskId);
                return new JsonResult(new { cancelled = true });
            }
            else
            {
                return new JsonResult(new { cancelled = false, error = "Task not found" }) { StatusCode = 404 };
            }
        }
    }
}
