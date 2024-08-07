﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JSInteropLib;

#region 生命周期
/// <summary>
///		封装一些 js 操作。
/// </summary>
/// <param name="jsrt"></param>
public partial class JSOp(IJSRuntime jsrt) : IAsyncDisposable
{
	private bool _disposed = false;

	/// <summary>
	///		释放后无法再进行 js 操作。
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

		if (_jsm != null)
		{
			await _jsm.DisposeAsync();
		}
	}

	/// <summary>
	/// 内置模块
	/// </summary>
	private JSModule _jsm = new(jsrt, "./_content/JSInteropLib/JSOp.js");
}
#endregion

#region 调试相关
public partial class JSOp
{
	/// <summary>
	///		调用 js 向控制台打印。
	///		本重载是打印一个换行符。
	/// </summary>
	public void Log()
	{
		Log('\n');
	}

	/// <summary>
	///		调用 js 向控制台打印。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="data"></param>
	public async void Log<T>(T data)
	{
		await _jsm.InvokeVoidAsync("Log", data);
	}

	/// <summary>
	///		以异步的方式调用 js 向控制台打印内容。
	///		本重载是打印一个换行符。
	/// </summary>
	/// <returns></returns>
	public async ValueTask LogAsync()
	{
		await LogAsync('\n');
	}

	/// <summary>
	///		以异步的方式调用 js 向控制台打印内容。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="data"></param>
	/// <returns></returns>
	public async ValueTask LogAsync<T>(T data)
	{
		await _jsm.InvokeVoidAsync("Log", data);
	}

	/// <summary>
	///		调用 js，弹出 alert 窗口，显示指定的消息。
	/// </summary>
	/// <param name="msg"></param>
	/// <returns></returns>
	public async Task AlertAsync(string msg)
	{
		await _jsm.InvokeVoidAsync("alert", msg);
	}
}
#endregion

#region JS 事件相关
public partial class JSOp
{
	/// <summary>
	/// 触发指定 DOM 元素的点击事件
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public async ValueTask TriggerClickEvent(ElementReference? element)
	{
		if (element == null)
		{
			return;
		}

		await _jsm.InvokeVoidAsync("TriggerClickEvent", element);
	}
}
#endregion

#region 文件相关功能
public partial class JSOp
{
	/// <summary>
	///		通过 URL 下载文件。
	///		会将这个 url 直接给 a 标签，然后点击，让浏览器自己去下载。
	/// </summary>
	/// <param name="url"></param>
	/// <returns></returns>
	public async ValueTask DownloadFromUrl(string url)
	{
		await _jsm.InvokeVoidAsync("DownloadFromUrl", url);
	}

	/// <summary>
	///		输入一个 Stream 对象，将这个对象传输至 js，调用浏览器的 API 下载这个 Stream 的内容。
	///		流会被完整复制到内存中，然后通过 blob url 给 a 标签进行下载，所以不要用此函数下载一个
	///		很长的流。
	/// </summary>
	/// <param name="mime">下载文件的 mime 类型</param>
	/// <param name="fileName">下载文件的文件名</param>
	/// <param name="stream">要被下载的流</param>
	/// <returns></returns>
	public async ValueTask DownloadFromStream(string mime, string fileName, Stream stream)
	{
		using DotNetStreamReference dotNetStreamReference = new(stream);
		await _jsm.InvokeVoidAsync("download_stream", mime, fileName, dotNetStreamReference);
	}

	/// <summary>
	///		检查指定 URL 的脚本是否已经添加了。
	/// </summary>
	/// <param name="script_url"></param>
	/// <returns></returns>
	public async Task<bool> IsScriptAlreadyIncluded(string script_url)
	{
		return await _jsm.InvokeAsync<bool>("IsScriptAlreadyIncluded", script_url);
	}

	/// <summary>
	///		向 head 标签中添加 script 标签，添加成功后会回调 helper 中的 CallbackAction。
	/// </summary>
	/// <remarks>
	///		本函数会先检查是否已经添加 script_url 的脚本，如果已经添加，不会再次添加。
	/// </remarks>
	/// <param name="scriptUrl"></param>
	/// <param name="helper"></param>
	/// <returns></returns>
	private async ValueTask AddScriptAsync(string scriptUrl, CallbackHelper helper)
	{
		if (await IsScriptAlreadyIncluded(scriptUrl))
		{
			// 已经添加过的不重复添加
			helper.CallbackAction?.Invoke();
			return;
		}

		await _jsm.InvokeVoidAsync("AddScript", scriptUrl, helper.DotNetHelper);
	}

	/// <summary>
	///		向 head 标签中添加 script 标签，添加成功后函数返回。
	///		本函数会先检查是否已经添加 script_url 的脚本，如果已经添加，不会再次添加。
	/// </summary>
	/// <param name="script_url"></param>
	/// <returns></returns>
	public async ValueTask AddScriptAsync(string script_url)
	{
		TaskCompletionSource addScriptComplete = new();
		await using CallbackHelper callbackHelper = new();
		callbackHelper.CallbackAction += () =>
		{
			addScriptComplete.TrySetResult();
		};

		await AddScriptAsync(script_url, callbackHelper);
		await addScriptComplete.Task;
	}

	/// <summary>
	///		检查指定的 css 是否已经添加过了。
	/// </summary>
	/// <param name="url">css 的 URL。</param>
	/// <returns>已经添加过了返回 true，没有添加过返回 false。</returns>
	public async Task<bool> IsCssAlreadyIncluded(string url)
	{
		return await _jsm.InvokeAsync<bool>("IsCssAlreadyIncluded", url);
	}

	/// <summary>
	///		添加指定 URL 的 css 文件。
	/// </summary>
	/// <param name="url">css 文件的 URL。</param>
	/// <returns></returns>
	public async Task AddCssAsync(string url)
	{
		if (await IsCssAlreadyIncluded(url))
		{
			// 已经添加过的不重复添加
			return;
		}

		await _jsm.InvokeVoidAsync("AddCss", url);
	}
}
#endregion

#region DOM 元素操作
public partial class JSOp
{
	/// <summary>
	///		将 DOM 元素滚动到视野中
	/// </summary>
	/// <param name="id">DOM 元素的 id</param>
	/// <returns></returns>
	public async Task ScrollIntoView(string id)
	{
		await _jsm.InvokeVoidAsync("ScrollIntoView", id);
	}

	/// <summary>
	///		聚焦到输入框，并选中所有文字。
	///		<br/>* 需要定义一个 TaskCompletionSource _render_tcs，然后在 OnAfterRender
	///		函数中设置结果，调用本函数的地方需要等待 _render_tcs。
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public async Task FocusOnInputElement(ElementReference element)
	{
		await _jsm.InvokeVoidAsync("FocusOnInputElement", element);
	}

	/// <summary>
	///		聚焦到指定元素上。
	///		<br/>* 需要定义一个 TaskCompletionSource _render_tcs，然后在 OnAfterRender
	///		函数中设置结果，调用本函数的地方需要等待 _render_tcs。
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	public async Task FocusOnElement(ElementReference element)
	{
		await _jsm.InvokeVoidAsync("FocusOnElement", element);
	}

	/// <summary>
	///		获取计算样式。
	/// </summary>
	/// <param name="element"></param>
	/// <param name="styleName"></param>
	/// <returns></returns>
	public async Task<string> GetComputedStyle(ElementReference element, string styleName)
	{
		return await _jsm.InvokeAsync<string>("GetComputedStyle", element, styleName);
	}

	/// <summary>
	///		设置指定元素的指定样式。
	/// </summary>
	/// <param name="element"></param>
	/// <param name="styleName"></param>
	/// <param name="style"></param>
	/// <returns></returns>
	public async Task SetElementStyle(ElementReference element, string styleName, string style)
	{
		await _jsm.InvokeVoidAsync("SetElementStyle", element, styleName, style);
	}

	/// <summary>
	///		添加字符串的 css 文本到 style 标签中，然后将 style 标签放到 head 标签中。
	/// </summary>
	/// <param name="cssText"></param>
	/// <returns></returns>
	public async Task AddStyleAsync(string cssText)
	{
		await _jsm.InvokeVoidAsync("AddStyleByString", cssText);
	}
}
#endregion
