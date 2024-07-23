using Microsoft.JSInterop;

namespace JSInteropLib;

/// <summary>
///		帮助简化将回调函数传给 js，让 js 调用的过程。
/// </summary>
public class CallbackHelper : IAsyncDisposable
{
	/// <summary>
	///		构造函数。
	/// </summary>
	public CallbackHelper()
	{
		DotNetHelper = DotNetObjectReference.Create(this);
	}

	private bool _disposed = false;

	/// <summary>
	///		异步释放。
	///		释放后，回调不可用，js 如果在释放后调用回调，会引发无法预知的结果。
	/// </summary>
	/// <returns></returns>
	public async ValueTask DisposeAsync()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		GC.SuppressFinalize(this);

		await ValueTask.CompletedTask;
		DotNetHelper.Dispose();
	}

	/// <summary>
	///		此 CallbackHelper 对象的 DotNetObjectReference
	/// </summary>
	public DotNetObjectReference<CallbackHelper> DotNetHelper { get; private set; }

	/// <summary>
	///		在这里添加需要被回调的函数
	/// </summary>
	public Action? CallbackAction { get; set; }

	/// <summary>
	///		让 JS 调用的函数，将本类对象的 DotNetHelper 属性传递给 js，js 中可以通过如下
	///		的方式调用本类对象的 Invoke 方法
	///		dotnetHelper.invokeMethodAsync("Invoke");
	/// </summary>
	[JSInvokable]
	public void Invoke()
	{
		CallbackAction?.Invoke();
	}
}

/// <summary>
///		帮助简化将回调函数传给 js，让 js 调用的过程。
/// </summary>
/// <typeparam name="T"></typeparam>
public class CallbackHelper<T> : IAsyncDisposable
{
	/// <summary>
	///		构造函数
	/// </summary>
	public CallbackHelper()
	{
		DotNetHelper = DotNetObjectReference.Create(this);
	}

	private bool _disposed = false;

	/// <summary>
	///		异步释放。
	///		释放后，回调不可用，js 如果在释放后调用回调，会引发无法预知的结果。
	/// </summary>
	/// <returns></returns>
	public async ValueTask DisposeAsync()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		GC.SuppressFinalize(this);

		await ValueTask.CompletedTask;
		DotNetHelper.Dispose();
	}

	/// <summary>
	///		此 CallbackHelper 对象的 DotNetObjectReference
	/// </summary>
	public DotNetObjectReference<CallbackHelper<T>> DotNetHelper { get; private set; }

	/// <summary>
	///		在这里添加需要被回调的函数
	/// </summary>
	public Action<T>? CallbackAction { get; set; }

	/// <summary>
	///		让 JS 调用的函数，将本类对象的 DotNetHelper 属性传递给 js，js 中可以通过如下
	///		的方式调用本类对象的 Invoke 方法
	///		dotnetHelper.invokeMethodAsync("Invoke", param);
	/// </summary>
	[JSInvokable]
	public void Invoke(T param)
	{
		CallbackAction?.Invoke(param);
	}
}
