using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Configuration;

namespace CODV2API.Handlers
{
    /// <summary>
    /// Helper companion class to the GISTMaps services schema for universal system access to individual data items. The GISTMaps AGS resource
    /// provides country level services with a 3 character iso identifier an _ and a 2 character language extension in the service name to
    /// distinguish the primary language for attribution. The language additionally indicates the fieldnames to further distinguish values.
    /// Used in the COD Services API.
    /// </summary>
    public class GistMapsServiceHandler
    {
        private string _service_lang_abbrev;
        private int _service_level_number_total;
        private String inServiceUrl;
        private String inISO3;
        private string gistAGS = ConfigurationManager.AppSettings["AGSServiceRoot"];

        //<summary>
        // The 2 character iso code abbreviation for the language of attribution is returned for the service.
        //</summary
        public String service_lang_abbrev
        {
            get
            {
                return  this._service_lang_abbrev;
            }
            set { }

        }
        public int service_level_number_total;
        
        public GistMapsServiceHandler(string service_root_url, string iso3){
            //urlpath is inpath of gistmaps services: https://gistmaps.itos.uga.edu/arcgis/rest/services/COD_External
            this._service_lang_abbrev = ServiceAGSLang(service_root_url, iso3);
            this._service_level_number_total = -1;
            this.inISO3 = iso3;
            this.inServiceUrl = service_root_url;
            service_lang_abbrev = this.service_lang_abbrev;
        }
        //A method to query the list of services deployed, given the country 3 character iso code.
        //Parse the service name for the language. Good for when the service url is not known for the country.
        private string ServiceAGSLang(string Path, string pCode)
        {
            string lang_abbr = "";
            string id;
            string cname;
            if (String.IsNullOrWhiteSpace(Path))
                Path = gistAGS;

            StringBuilder requestUrlSB = new StringBuilder(Path + "?f=pjson");
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrlSB.ToString()) as HttpWebRequest;
                request.KeepAlive = true;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
                    var res = responseStream.ReadToEnd();
                    JToken jsonObject = JToken.Parse(res);

                    foreach (JProperty item in jsonObject)
                    {
                        if (item.Name == "services")
                        {
                            JArray iso2Feat = JArray.Parse(item.Value.ToString());
                            foreach (var jt in iso2Feat)
                            {
                                cname = jt["name"].ToString();
                                if (!cname.Contains("pcode"))
                                {
                                    if (cname.Contains("/" + pCode.ToUpper()))
                                    {
                                        if (!(cname.ToLower().Contains("lookup")))
                                            lang_abbr = cname.Substring(cname.Length - 2);
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception e)
            {
                throw;
            }

            return lang_abbr;
        }
    }
}