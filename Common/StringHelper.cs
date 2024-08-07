//using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public static class StringHelper
    {
        public static string RemoveQueryStringByKey(string url, string key)
        {
            try
            {
                var uri = new Uri(url);

                // this gets all the query string key value pairs as a collection
                var newQueryString = HttpUtility.ParseQueryString(uri.Query);

                // this removes the key if exists
                newQueryString.Remove(key);

                // this gets the page path from root without QueryString
                string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

                return newQueryString.Count > 0
                    ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                    : pagePathWithoutQueryString;
            }
            catch (Exception)
            {
                return url;
            }
        }
        //public static string FindPhoneNumber(string text, string country="VN")
        //{
        //    try
        //    {
        //        var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
        //        var phoneNumber = phoneNumberUtil.FindNumbers(text, country).Select(x=>x.Number).Select(x=> "+"+ x.CountryCode+ x.NationalNumber.ToString());
        //        if (phoneNumber.Count() > 0)
        //        {
        //            var phones = String.Join(", ", phoneNumber);
        //            return phones;
        //        }
        //        else return null;
                
                
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}
