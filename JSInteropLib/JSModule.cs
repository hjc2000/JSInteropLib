using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace JSInteropLib;

/// <summary>
///		简化导入 js 模块的过程。
/// </summary>
public class JSModule : IJSObjectReference
{
	/// <summary>
	///		构造函数
	/// </summary>
	/// <param name="jsrt">js 运行时。</param>
	/// <param name="js_file_path">要导入的 js 模块的路径。</param>
	public JSModule(IJSRuntime jsrt, string js_file_path)
	{
		Init(jsrt, js_file_path);
	}

	private TaskCompletionSource _initTcs = new();

	private async void Init(IJSRuntime jsrt, string js_file_path)
	{
		Module = await jsrt.InvokeAsync<IJSObjectReference>("import", js_file_path);
		_initTcs.TrySetResult();
	}

	private bool _disposed = false;

	/// <summary>
	///		释放后模块不再可用。
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

		if (Module != null)
		{
			await Module.DisposeAsync();
		}
	}

	/// <summary>
	/// 用户构建此类对象时导入的自定义模块
	/// </summary>
	public IJSObjectReference Module { get; private set; } = default!;

	#region IJSObjectReference 接口实现
	/// <summary>
	/// 如果感觉不对劲，记得 using Microsoft.JSInterop;
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="identifier"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
	{
		await _initTcs.Task;
		return await Module.InvokeAsync<TValue>(identifier, args);
	}

	/// <summary>
	/// 如果感觉不对劲，记得 using Microsoft.JSInterop;
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	/// <param name="identifier"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
	{
		await _initTcs.Task;
		return await Module.InvokeAsync<TValue>(identifier, cancellationToken, args);
	}
	#endregion
}