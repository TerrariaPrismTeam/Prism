using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Prism.Util
{
    public class HastebinHelper
    {

        private static readonly Regex HastebinKeyRegex = new Regex(@"{""key"":""(?<key>[a-z].*)""}", RegexOptions.Compiled);
        public static readonly string HastebinUrl = @"http://hastebin.com/";
        public static readonly string HastebinRequestUrl = HastebinUrl + "documents";

        public static string QuickUpload(string text)
        {
            string url = null;
            using (var web = new WebClient())
            {
                var reponse = web.UploadString(HastebinRequestUrl, text);
                var regexMatch = HastebinKeyRegex.Match(reponse);

                if (regexMatch.Success)
                {
                    url = HastebinUrl + regexMatch.Groups["key"];
                }                
            }
            return url; //Just wanted to exit the using before returning because I'm paranoid...
        }

    }
}
