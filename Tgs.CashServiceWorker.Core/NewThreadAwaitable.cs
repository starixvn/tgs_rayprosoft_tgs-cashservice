using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.YieldAwaitable;

namespace Tgs.CashServiceWorker;

public readonly struct NewThreadAwaitable
{
    public static readonly NewThreadAwaitable Default;

    public YieldAwaiter GetAwaiter() => default;

    public readonly struct NewThreadAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>WaitCallback that invokes the Action supplied as object state.</summary>
        private static readonly WaitCallback _WaitCallbackRunAction = RunAction;

        public void UnsafeOnCompleted(Action continuation)
        {
            QueueContinuation(continuation, false);
        }

        public void OnCompleted(Action continuation)
        {
            QueueContinuation(continuation, true);
        }

        private static void QueueContinuation(Action continuation, bool flowContext)
        {
            if (continuation is null)
                throw new ArgumentNullException(nameof(continuation));

            // If we're targeting the default scheduler, queue to the thread pool, so that we go into the global
            // queue.  As we're going into the global queue, we might as well use QUWI, which for the global queue is
            // just a tad faster than task, due to a smaller object getting allocated and less work on the execution path.
            TaskScheduler scheduler = TaskScheduler.Current;
            if (scheduler == TaskScheduler.Default)
                _ = flowContext
                    ? ThreadPool.QueueUserWorkItem(_WaitCallbackRunAction, continuation)
                    : ThreadPool.UnsafeQueueUserWorkItem(_WaitCallbackRunAction, continuation);
            // We're targeting a custom scheduler, so queue a task.
            else
                _ = Task.Factory.StartNew(
                    continuation,
                    default,
                    TaskCreationOptions.PreferFairness,
                    scheduler);
        }

        /// <summary>Runs an Action delegate provided as state.</summary>
        /// <param name="state">The Action delegate to invoke.</param>
        private static void RunAction(object? state) { ((Action)state!)(); }
    }
}
