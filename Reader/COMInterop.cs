using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Autodesk.AutoCAD.InteropHelpers
{
	public static class ComInterop
	{
		[DllImport("ole32.dll", CharSet = CharSet.Unicode)]
		internal static extern int CLSIDFromProgID(string lpszProgID, out Guid lpclsid);
		[DllImport("ole32")]
		private static extern int CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid lpclsid);
		[DllImport("oleaut32.dll")]
		private static extern int GetActiveObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);
		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
		[DllImport("ole32.dll")]

		static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
		public static object GetActiveObject(string progId) => GetActiveObject(progId, true);

		public static object GetActiveObject(string progId, bool throwOnError = true)
		{
			if (progId == null)
				throw new ArgumentNullException(nameof(progId));

			var hr = CLSIDFromProgID(progId, out var clsid);
			if (hr < 0)
			{
				if (throwOnError)
					System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);

				return null;
			}

			hr = GetActiveObject(clsid, IntPtr.Zero, out var obj);
			if (hr < 0)
			{
				if (throwOnError)
					System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);
				return null;
			}

			return obj;
		}

		public static object GetActiveAcadApp(string name = "AutoCAD")
		{
			foreach (object comObject in GetActiveObjects())
			{
				try
				{
					string appname = ((dynamic)comObject).Name;
					if (appname?.StartsWith(name) == true)
						return comObject;
				}
				catch(Exception ex)
				{
                    Console.WriteLine($"exception: {ex.ToString()}");
                }
			}

			return null;
		}

		public static IEnumerable<object> GetActiveAcadDocuments()
		{
			foreach (object comObject in GetActiveObjects())
			{
				string name = string.Empty;
				try
				{
					name = ((dynamic)comObject).Name;
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.ToString());
					continue;
				}
				if (name.EndsWith(".dwg", StringComparison.OrdinalIgnoreCase))
					yield return comObject;
			}
		}

		public static IEnumerable<object> GetActiveObjects()
		{
			IRunningObjectTable rot;
			if (GetRunningObjectTable(0, out rot) == 0)
			{
				IEnumMoniker enumMoniker;
				rot.EnumRunning(out enumMoniker);
				IntPtr fetched = IntPtr.Zero;
				IMoniker[] monikers = new IMoniker[1];
				while (enumMoniker.Next(1, monikers, fetched) == 0)
				{
					IBindCtx bindCtx;
					CreateBindCtx(0, out bindCtx);
					object comObject = null;
					try
					{
						if (rot.GetObject(monikers[0], out comObject) != 0 || comObject == null)
							continue;
					}
					catch (System.Exception ex)
					{
						Console.WriteLine($"exception: {ex.ToString()}");
					}
					yield return comObject;
				}
			}
		}
	}
}
