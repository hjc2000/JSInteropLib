/**
 * 给指定元素添加事件监听函数
 * @param {Element} element
 * @param {string} eventName
 * @param {object} funObj
 */
export function AddElementEventListener(element, eventName, funObj)
{
	element.addEventListener(eventName, funObj.fun);
}

/**
 * 移除指定元素的事件监听函数
 * @param {Element} element
 * @param {string} eventName
 * @param {object} funObj
 */
export function RemoveElementEventListener(element, eventName, funObj)
{
	console.log("移除元素事件监听函数：");
	console.log(element);
	console.log(funObj);
	element.removeEventListener(eventName, funObj.fun);
}

/**
 * 将事件监听函数添加到 window 对象中
 * @param {string} eventName
 * @param {object} funObj
 */
export function AddWindowEventListener(eventName, funObj)
{
	window.addEventListener(eventName, funObj.fun);
}

/**
 * 从 window 对象中移除事件监听函数
 * @param {string} eventName
 * @param {object} funObj
 */
export function RemoveWindowEventListener(eventName, funObj)
{
	console.log("移除window事件监听函数：");
	console.log(funObj);
	window.removeEventListener(eventName, funObj.fun);
}

/**
 * 从 URL 下载文件
 * @param {string} url
 * @param {string} filename
 */
export async function DownloadFromUrl(url)
{
	const aElement = document.createElement('a');
	aElement.href = url;
	aElement.click();
	aElement.remove();
}

/**
 * 触发指定元素的指定事件
 * @param {Element} element
 */
export async function TriggerClickEvent(element)
{
	element.click();
}

/**
 * 检查指定源的脚本是不是已经包含了。
 * @param {string} src 脚本源。一般是 url。
 * @returns
 */
export function IsScriptAlreadyIncluded(src)
{
	let scripts = document.getElementsByTagName('script');
	for (let i = 0; i < scripts.length; i++)
	{
		if (scripts[i].getAttribute('src') === src)
		{
			return true;
		}
	}

	return false;
}

/**
 * 向 head 标签添加 script 标签
 * @param {string} scriptUrl
 * @param {object} callbackHelper CallbackHelper 对象的 DotNetHelper 属性
 */
export function AddScript(scriptUrl, callbackHelper)
{
	let element = document.createElement('script');
	element.setAttribute("src", scriptUrl);
	let onloadHandler = function ()
	{
		callbackHelper.invokeMethodAsync("Invoke");
		element.removeEventListener("load", onloadHandler);
	}

	element.addEventListener("load", onloadHandler);

	let head = document.head;
	head.appendChild(element);
}

/**
 * 检测指定的 css 是否已经被添加了
 * @param {string} href css 的 URL
 * @returns 已经添加了返回 true，没有添加返回 false。
 */
export function IsCssAlreadyIncluded(href)
{
	let links = document.getElementsByTagName('link');
	for (let i = 0; i < links.length; i++)
	{
		if (links[i].href === href && links[i].rel.toLowerCase() === 'stylesheet')
		{
			return true;
		}
	}

	return false;
}

/**
 * 添加一个指定路径的 css 文件
 * @param {string} href css 的 URL
 */
export function AddCss(href)
{
	let element = document.createElement('link');
	element.setAttribute("href", href);
	element.setAttribute("rel", "stylesheet");
	let head = document.head;
	head.appendChild(element);
}

/**
 * 动态添加内联样式到页面的 head 中
 * @param {string} cssText 包含 CSS 规则的字符串
 */
export function AddStyleByString(cssText)
{
	let styleElement = document.createElement('style');
	styleElement.textContent = cssText;
	let head = document.head;
	head.appendChild(styleElement);
}

/**
 * 打印到控制台
 * @param {object} data
 */
export function Log(data)
{
	console.log(data);
}

export async function download_stream(mime, fileName, contentStreamReference)
{
	const arrayBuffer = await contentStreamReference.arrayBuffer();
	const blob = new Blob([arrayBuffer], { type: mime });
	const url = URL.createObjectURL(blob);
	const anchorElement = document.createElement('a');
	anchorElement.href = url;
	anchorElement.download = fileName;
	anchorElement.click();
	anchorElement.remove();
	URL.revokeObjectURL(url);
}

let alert_times = 0;
export function alert(msg)
{
	console.log(`alert ${++alert_times}`);
	window.alert(msg);
}

/**
 * 
 * @param {Element} element
 */
export function ScrollIntoView(id)
{
	let element = document.getElementById(id);
	try
	{
		element.scrollIntoViewIfNeeded();
		return;
	}
	catch { }

	// 执行到这里说明上一个 try 失败了。
	try
	{
		element.scrollIntoView();
		return;
	}
	catch { }
}

/**
 * 
 * @param {HTMLInputElement} element
 */
export function FocusOnInputElement(element)
{
	console.log(element);
	element.focus();
	element.select();
}

/**
 * 
 * @param {HTMLElement} element
 */
export function FocusOnElement(element)
{
	console.log(element);
	element.focus();
}

/**
 * 
 * @param {HTMLElement} element
 * @param {string} style_name
 * @returns
 */
export function GetComputedStyle(element, style_name)
{
	return window.getComputedStyle(element)[style_name];
}

/**
 * 
 * @param {HTMLElement} element
 * @param {string} style_name
 * @param {string} style
 */
export function SetElementStyle(element, style_name, style)
{
	element.style[style_name] = style;
}
