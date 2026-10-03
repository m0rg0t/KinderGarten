// Minimal awaitable adapter for tests; does not emulate WinRT cancellation or COM.
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace Windows.Foundation { public interface IAsyncOperation<T> { TaskAwaiter<T> GetAwaiter(); } internal sealed class Operation<T> : IAsyncOperation<T> { private readonly Task<T> task; public Operation(Task<T> task) { this.task=task; } public TaskAwaiter<T> GetAwaiter() { return task.GetAwaiter(); } } }
namespace System { public static class TestAsyncExtensions { public static global::Windows.Foundation.IAsyncOperation<T> AsAsyncOperation<T>(this Task<T> task) { return new global::Windows.Foundation.Operation<T>(task); } } }
