using System;
using System.Xml.Serialization;

namespace OGService
{
    [XmlRoot("item")]
    public class UpdateInfoEventArgs : EventArgs
    {
        private string _changelogURL;
        private string _downloadURL;

        /// <inheritdoc />
        public UpdateInfoEventArgs()
        {

        }

        /// <summary>
        ///     If new update is available then returns true otherwise false.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        ///     If there is an error while checking for update then this property won't be null.
        /// </summary>
        [XmlIgnore]
        public Exception Error { get; set; }

        /// <summary>
        ///     Download URL of the update file.
        /// </summary>
        //[XmlElement("url")]
        public string DownloadURL { get; set; }

        /// <summary>
        ///     URL of the webpage specifying changes in the new update.
        /// </summary>
        //[XmlElement("changelog")]
        //public string ChangelogURL
        //{
        //    get => GetURL(AutoUpdaters.BaseUri, _changelogURL);
        //    set => _changelogURL = value;
        //}

        /// <summary>
        ///     Returns newest version of the application available to download.
        /// </summary>
        [XmlElement("version")]
        public string CurrentVersion { get; set; }

        /// <summary>
        ///     Returns version of the application currently installed on the user's PC.
        /// </summary>
        public Version InstalledVersion { get; set; }


        /// <summary>
        ///     Command line arguments used by Installer.
        /// </summary>
        [XmlElement("args")]
        public string InstallerArgs { get; set; }

        /// <summary>
        ///     Checksum of the update file.
        /// </summary>
        [XmlElement("checksum")]
        public CheckSum CheckSum { get; set; }

        internal static string GetURL(Uri baseUri, string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                Uri uri = new Uri(baseUri, url);

                if (uri.IsAbsoluteUri)
                {
                    url = uri.AbsoluteUri;
                }
            }

            return url;
        }
    }



    /// <summary>
    ///     Checksum class to fetch the XML values for checksum.
    /// </summary>
    public class CheckSum
    {
        /// <summary>
        ///     Hash of the file.
        /// </summary>
        [XmlText]
        public string Value { get; set; }

        /// <summary>
        ///     Hash algorithm that generated the hash.
        /// </summary>
        [XmlAttribute("algorithm")]
        public string HashingAlgorithm { get; set; }
    }
}
