using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(StartupExtensionWithSqlite.MySqliteStartup))]

namespace StartupExtensionWithSqlite
{
    internal class MySqliteStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            string version = Marshal.PtrToStringAnsi(sqlite3_libversion());
            builder.UseSetting("SqliteVersion", version);
        }

        [DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_libversion();
    }
}
