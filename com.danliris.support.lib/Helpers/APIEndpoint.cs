using System;
using System.Collections.Generic;
using System.Text;

namespace com.danliris.support.lib.Helpers
{
    public static class APIEndpoint
    {
        public static string Core { get; set; }
        public static string Inventory { get; set; }

		public static string ConnectionString { get; set; }
        public static string LocalConnectionString { get; set; }
        public static string Production { get; set; }
        public static string HostToHost { get; set; }
    }

    public static class CredentialCeisa
    {
        public static string Username { get; set; }
        public static string Password { get; set; }
    }

    public static class TokenCeisa
    {
        public static string refresh_token { get; set; }
        public static string token_ceisa { get; set; }
    }
}
