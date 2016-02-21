using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MvcCms.Models
{
    public static class StringExtensions
    {
        public static string MakeUrlFriendly(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;

            value = value.ToLowerInvariant().Replace(" ", "-");
            value = Regex.Replace(value, @"[^0-9a-z-]", String.Empty);

            return value;
        }
    }
}