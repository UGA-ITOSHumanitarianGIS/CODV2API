using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using CODV2API.DAL;
using CODV2API.Models;
using CODV2API.ViewModels;
using Newtonsoft.Json.Linq;

namespace CODV2API.Controllers.api
{
    /// <summary>
    /// Look up for the COD contents at the system level. JSON results are returned in what are commonly refererred to as Gazeteer format.
    /// </summary>
    public class LookupController : BaseApiController
    {
        private ApplicationContext db = new ApplicationContext();
        /// <summary>
        /// Retrieve location by level and latlng parameter with wkid units meters and World Mercator projection.
        /// If no levels are entered, all levels are returned. If no version is specified, the current version is returned.
        /// 
        /// Returns: Names and Pcodes
        /// </summary>
        /// <param name="latlong">-2.33,12.34</param>
        /// <param name="level">2</param>
        /// <param name="wkid">4326</param>
        // GET:  api/v1/themes/cod-ab/lookup/latlong/endpoint/lookup/latlng? latlng = 8.3579,-8.7123&level=2&version=current
        // [ResponseType(typeof(levellatlnglookup))]
        // admin2
        // <returns>
        // Returns Names and Pcodes
        // </returns>
        [Route("api/v1/themes/cod-ab/Lookup/latlng/{latlong?}/{wkid?}/{level?}")]
        [HttpGet()]
        [ActionName("latlng")]
        [ResponseType(typeof(levellatlngLookup))]
        public IHttpActionResult latlng(String latlong, String wkid, int? level)
        {
            try
            {
                List<string> eCoordsString = latlong.Split(',').ToList<string>();
                String xy = eCoordsString[1] + "," + eCoordsString[0];
                String iso2Rec = "";
                level0latlngLookup levelSummary = new level0latlngLookup();
                String cname = "";
                //String eCoords = "{\"x\":" + eCoordsString[1] + "\"y\" : " + eCoordsString[0] + ", \"spatialReference\" :  {\"wkid\" : " + wkid + "}}";
                //eCoords = System.Uri.EscapeDataString(eCoords);
                //string sURL = "https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/World_Countries/FeatureServer/0/query?where=&objectIds=&time=&geometry=%7B%22x%22+%3A+-335346.69%2C+%22y%22+%3A+2200009.54%2C+%22spatialReference%22+%3A+%7B%22wkid%22+%3A+3857%7D%7D&geometryType=esriGeometryPoint&inSR=&spatialRel=esriSpatialRelIntersects&resultType=none&distance=0.0&units=esriSRUnit_Meter&returnGeodetic=false&outFields=*&returnGeometry=true&returnCentroid=false&featureEncoding=esriDefault&multipatchOption=xyFootprint&maxAllowableOffset=&geometryPrecision=&outSR=&datumTransformation=&applyVCSProjection=false&returnIdsOnly=false&returnUniqueIdsOnly=false&returnCountOnly=false&returnExtentOnly=false&returnQueryGeometry=false&returnDistinctValues=false&cacheHint=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&having=&resultOffset=&resultRecordCount=&returnZ=false&returnM=false&returnExceededLimitFeatures=true&quantizationParameters=&sqlFormat=none&f=json&token=";
                string sURL = "https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/World_Countries/FeatureServer/0/query?where=&objectIds=&time=&geometry=" + xy + "&spatialReference=&wkid=&geometryType=esriGeometryPoint&inSR=" + wkid + "&spatialRel=esriSpatialRelIntersects&resultType=none&distance=0.0&units=esriSRUnit_Meter&returnGeodetic=false&outFields=*&returnGeometry=true&returnCentroid=false&featureEncoding=esriDefault&multipatchOption=xyFootprint&maxAllowableOffset=&geometryPrecision=&outSR=&datumTransformation=&applyVCSProjection=false&returnIdsOnly=false&returnUniqueIdsOnly=false&returnCountOnly=false&returnExtentOnly=false&returnQueryGeometry=false&returnDistinctValues=false&cacheHint=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&having=&resultOffset=&resultRecordCount=&returnZ=false&returnM=false&returnExceededLimitFeatures=true&quantizationParameters=&sqlFormat=none&f=json&token=";
                StringBuilder requestUrlSB = new StringBuilder(sURL);
                //hard coded for demo purposes

                HttpWebRequest request = WebRequest.Create(requestUrlSB.ToString()) as HttpWebRequest;
                request.KeepAlive = true;
                List<KeyValuePair<String, String>> countryList = new List<KeyValuePair<String, String>>();

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader responseStream = new StreamReader(response.GetResponseStream());
                    var res = responseStream.ReadToEnd();
                    JToken jsonObject = JToken.Parse(res);

                    foreach (JProperty item in jsonObject)
                    {
                        if (item.Name == "features")
                        {
                            JArray iso2Feat = JArray.Parse(item.Value.ToString());
                            foreach (var jt in iso2Feat)
                            {
                                iso2Rec = jt["attributes"]["ISO_CC"].ToString();
                                cname = jt["attributes"]["COUNTRY"].ToString();

                                countryList.Add(new KeyValuePair<String, String>(iso2Rec, cname));
                            }
                        }
                    }
                }
                if (level == 0)
                {

                    List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                    foreach (KeyValuePair<String, String> item in countryList)
                    {
                        levelSummary.admin0Name_local = item.Value;
                        levelSummary.admin0Name_ref = "Null";
                        levelSummary.admin0Pcode = item.Key;
                        levelSummary.status = "OK";
                        level0SummaryList.Add(levelSummary);

                    }
                    return Json(level0SummaryList);
                }


                //get all locations for the country. then get the path and query the service
                var lnData = db.LocationNames
                    .Join(db.Locations,
                    l => l.LocationId,
                    ln => ln.LocationId,
                    (l, ln) => new { l, ln })
                    .Join(db.Levels,
                    lvl => lvl.ln.LevelId,
                    alvl => alvl.LevelId,
                    (lvl, alvl) => new { lvl, alvl })//.Where(a => a.alvl.admin0PCode.ToLower() == iso2Rec.ToLower())
                    .Join(db.LocationMetadata,
                    lml => lml.lvl.l.LocationId,
                    ln => ln.LocationId,
                    (lml, ln) => new { lml.lvl.l, ln })
                    .Join(db.MetaDatas,
                    m => m.ln.MetadataId,
                    ml => ml.MetadataId,
                    (m, ml) => new { m.ln, ml })
                    .Join(db.ResourceMetadata,
                    rm => rm.ml.MetadataId,
                    rml => rml.MetadataId,
                    (rm, rml) => new { rm.ml, rml }).Where(a => a.ml.locationIso.ToLower() == iso2Rec.ToLower())
                    .Join(db.Resources,
                    tr => tr.rml.ResourceId,
                    trl => trl.ResourceId,
                    (tr, trl) => new { tr.rml, trl }).ToList();

                var lndata = lnData.Where(a => a.trl.Path.Contains("gistmaps")).ToList();



                //the following should be used in production, the test data is not loading the current version path
                //for some reason
                try
                {
                    var lndata2 = lndata.Except(lndata.Where(b => b.rml.resource.Path.Contains("V0_00"))).ToList();
                    var lndata3 = lndata2.Except(lndata2.Where(b => b.rml.resource.Path.Contains("pcode"))).ToList();

                    String svcRoot = lndata2[0].trl.Path.ToString();
                    svcRoot = svcRoot.Replace("V00_0", "COD_External");
                    int itrimstart = svcRoot.IndexOf("MapServer");

                    //The level may not be the service node. Get the right service node for the level in the input

                    if (itrimstart > -1)
                    {
                        svcRoot = svcRoot.Substring(0, itrimstart);
                    }
                    else
                    {
                        itrimstart = svcRoot.IndexOf("FeatureServer");
                        if (itrimstart > -1)
                            svcRoot = svcRoot.Substring(0, itrimstart);
                    }
                    int metaId = lndata2.Select(a => a.rml.MetadataId).FirstOrDefault();
                    string iso3rec = db.MetaDatas.Where(a => a.MetadataId == metaId).Select(r => r.locationIso).FirstOrDefault();
                    
                    string svcLevelNode = LevelServiceAGSNode(svcRoot, level);
                    String itosService = svcRoot + "MapServer/" + svcLevelNode + "/query?where=&text=&objectIds=&time=&geometry=" + xy + "&geometryType=esriGeometryPoint&inSR=" + wkid + "&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=false&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&resultOffset=&resultRecordCount=&queryByDistance=&returnExtentsOnly=false&datumTransformation=&parameterValues=&rangeValues=&f=pjson";

                    //TODO logic that parses the correct service by the admin_pcode name convention
                    // the list above has that.

                    requestUrlSB = new StringBuilder(itosService);
                    //hard coded for demo purposes

                    request = WebRequest.Create(requestUrlSB.ToString()) as HttpWebRequest;
                    request.KeepAlive = true;

                    List<levellatlngLookup> levelAllSummaryList = new List<levellatlngLookup>();

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader responseStream = new StreamReader(response.GetResponseStream());
                        var res = responseStream.ReadToEnd();
                        String atName = "fr";
                        //get language abbreviation for the service which is also used in the attribute collection
                        if (lndata3.Count() > 0)
                        {
                            String sp = lndata3[0].trl.Path.ToString();
                            int spsub = sp.IndexOf("COD_External");
                            sp = sp.Substring(spsub, 19);
                            if (sp.Length != 0)
                                sp = sp.Substring(sp.Length - 2);
                            atName = sp.ToLower();
                        } else
                        {
                            var gistLangHelper = new CODV2API.Handlers.GistMapsServiceHandler(null, iso3rec);
                            atName = gistLangHelper.service_lang_abbrev;
                            if (!(atName is null))
                                atName = atName.ToLower();
                        }
                        
                        JToken jsonObject = JToken.Parse(res);
                      
                        foreach (JProperty item in jsonObject)
                        {
                            if (item.Name == "features")
                            {
                                switch (level)
                                {
                                    case 4:
                                        JArray iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            levellatlngLookup level4Summary = new levellatlngLookup();
                                            level4Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level4Summary.admin0Name_ref = "Null";
                                            level4Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level4Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level4Summary.admin1Name_ref = "Null";
                                            level4Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level4Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level4Summary.admin2Name_ref = "Null";
                                            level4Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level4Summary.admin3Name_local = jt["attributes"]["admin3Name_" + atName].ToString();
                                            level4Summary.admin3Name_ref = "Null";
                                            level4Summary.admin3Pcode = jt["attributes"]["admin3Pcode"].ToString();
                                            level4Summary.admin3Name_local = jt["attributes"]["admin4Name_" + atName].ToString();
                                            level4Summary.admin3Name_ref = "Null";
                                            level4Summary.admin3Pcode = jt["attributes"]["admin4Pcode"].ToString();
                                            level4Summary.status = "OK";
                                            levelAllSummaryList.Add(level4Summary);

                                        }
                                        return Json(levelAllSummaryList);
                                        break;
                                    case 3:
                                        iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            levellatlngLookup level3Summary = new levellatlngLookup();
                                            level3Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level3Summary.admin0Name_ref = "Null";
                                            level3Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level3Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level3Summary.admin1Name_ref = "Null";
                                            level3Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level3Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level3Summary.admin2Name_ref = "Null";
                                            level3Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level3Summary.admin3Name_local = jt["attributes"]["admin3Name_" + atName].ToString();
                                            level3Summary.admin3Name_ref = "Null";
                                            level3Summary.admin3Pcode = jt["attributes"]["admin3Pcode"].ToString();
                                            level3Summary.status = "OK";
                                            levelAllSummaryList.Add(level3Summary);

                                        }
                                        return Json(levelAllSummaryList);
                                        break;
                                    case 2:
                                        iso2Feat = JArray.Parse(item.Value.ToString());

                                        List<level2latlngLookup> level2AllSummaryList = new List<level2latlngLookup>();
                                        foreach (var jt in iso2Feat)
                                        {
                                            level2latlngLookup level2Summary = new level2latlngLookup();
                                            level2Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level2Summary.admin0Name_ref = "Null";
                                            level2Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level2Summary.admin1Name_ref = "Null";
                                            level2Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level2Summary.admin2Name_ref = "Null";
                                            level2Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2AllSummaryList.Add(level2Summary);
                                        }
                                        return Json(level2AllSummaryList);
                                        break;
                                    case 1:
                                        //return level 1

                                        List<level1latlngLookup> level1SummaryList = new List<level1latlngLookup>();
                                        iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            level1latlngLookup level1Summary = new level1latlngLookup();
                                            level1Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level1Summary.admin1Name_ref = "Null";
                                            level1Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level1Summary.status = "OK";
                                            level1SummaryList.Add(level1Summary);
                                        }
                                        return Json(level1SummaryList);
                                        break;

                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                    levelSummary.admin0Name_local = cname;
                    levelSummary.admin0Name_ref = "Null";
                    levelSummary.admin0Pcode = iso2Rec;
                    levelSummary.status = "Not Supported in this release";
                    level0SummaryList.Add(levelSummary);
                    return Json(level0SummaryList);
                }
            }
            catch (Exception e)
            {
                List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                level0latlngLookup levelSummary = new level0latlngLookup();
                levelSummary.admin0Name_local = "";
                levelSummary.admin0Name_ref = "Null";
                levelSummary.admin0Pcode = "";
                levelSummary.status = "Check required parameters: <latlong=x,y> <wkid=z> <level=n>";
                level0SummaryList.Add(levelSummary);
                return Json(level0SummaryList);
            }

            return Json("");
        }
        /// <summary>
        /// Retrieve place names and Pcodes based on level and country iso2 (i.e. BF for Burkina Faso)
        /// If no levels are entered, all places are returned. If no version is specified, the current version is returned.
        /// 
        /// Returns: Names and Pcodes
        /// </summary>
        /// <param name="iso2">HN</param>
        /// <param name="level">2</param>
        // GET:  api/v1/themes/cod-ab/lookup/Get?level=2&iso2=BF
        // [ResponseType(typeof(levellatlnglookup))]
        // admin2
        [Route("api/v1/themes/cod-ab/lookup/{level?}/{iso2?}")]
        [HttpGet()]
        [ResponseType(typeof(levellatlngLookup))]
        public IHttpActionResult Get(int? level, String iso2)
        {
            try
            {

                String iso2Rec = "";
                level0latlngLookup levelSummary = new level0latlngLookup();
                String cname = "";
                iso2Rec = iso2;
               
                //query to get locationid for the matching iso2
                var lnData = db.Locations
                    .Join(db.Levels,
                    lvl => lvl.LevelId,
                    alvl => alvl.LevelId,
                    (lvl, alvl) => new { lvl, alvl }).Where(a => a.alvl.admin0PCode.ToLower() == iso2Rec.ToLower()).ToList();
                int lId = lnData.FirstOrDefault().lvl.LocationId;

                //query to get the locationiso at the location
                var lmquery = db.Locations
                .Join(db.LocationMetadata,
                l => l.LocationId,
                ln => ln.LocationId,
                (l, ln) => new { l, ln }).Where(a => a.l.LocationId == lId)
                    .Join(db.MetaDatas,
                    m => m.ln.MetadataId,
                    ml => ml.MetadataId,
                    (m, ml) => new { m.ln, ml })
                    .Join(db.ResourceMetadata,
                    rm => rm.ml.MetadataId,
                    rml => rml.MetadataId,
                    (rm, rml) => new { rm.ml, rml })
                    .Join(db.Resources,
                    tr => tr.rml.ResourceId,
                    trl => trl.ResourceId,
                    (tr, trl) => new { tr.rml, trl }).ToList();


                var lndata = lmquery.Where(a => a.rml.resource.Path.Contains("gistmaps")).ToList();

                var numL = db.Levels.Where(a => a.admin0PCode.ToLower() == iso2Rec.ToLower()).FirstOrDefault();
                int nl = numL.numLevels;
                int vInt = -1;
                if (level is null) { vInt = numL.numLevels; } else { vInt = (int)level; }
                //the following should be used in production, the test data is not loading the current version path
                //for some reason
                try
                {
                    var lndata2 = lndata.Except(lndata.Where(b => b.rml.resource.Path.Contains("V0_00"))).ToList();
                    var lndata3 = lndata2.Except(lndata2.Where(b => b.rml.resource.Path.Contains("pcode"))).ToList();

                    String svcRoot = lndata2[0].trl.Path.ToString();
                    svcRoot = svcRoot.Replace("V00_0", "COD_External");
                    int itrimstart = svcRoot.IndexOf("MapServer");

                    //The level may not be the service node. Get the right service node for the level in the input

                    if (itrimstart > -1)
                    {
                        svcRoot = svcRoot.Substring(0, itrimstart);
                    }
                    else
                    {
                        itrimstart = svcRoot.IndexOf("FeatureServer");
                        if (itrimstart > -1)
                            svcRoot = svcRoot.Substring(0, itrimstart);
                    }
                    int metaId = lndata2.Select(a => a.rml.MetadataId).FirstOrDefault();
                    string iso3rec = db.MetaDatas.Where(a => a.MetadataId == metaId).Select(r => r.locationIso).FirstOrDefault();

                    //The level may not be the service node. Get the right service node for the level in the input
                    string svcLevelNode = LevelServiceAGSNode(svcRoot, level);

                    //done: parse the level availability for the country to get the match in the level or the highest number

                    String itosService = svcRoot + "MapServer/" + svcLevelNode + "/query?where=0%3D0&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=false&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&resultOffset=&resultRecordCount=&queryByDistance=&returnExtentsOnly=false&datumTransformation=&parameterValues=&rangeValues=&f=pjson";
                    //String itosService = svcRoot + "MapServer" + svcNode + "query?where=&text=&objectIds=0%3D0&time=&geometry=&geometryType=esriGeometryPoint&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=*&returnGeometry=false&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&resultOffset=&resultRecordCount=&queryByDistance=&returnExtentsOnly=false&datumTransformation=&parameterValues=&rangeValues=&f=pjson";

                    HttpWebRequest request = WebRequest.Create(itosService) as HttpWebRequest;
                    request.KeepAlive = true;
  
                    List<levellatlngLookup> levelAllSummaryList = new List<levellatlngLookup>();

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader responseStream = new StreamReader(response.GetResponseStream());
                        var res = responseStream.ReadToEnd();
                      
                        String atName = "fr";
                        //get language abbreviation for the service which is also used in the attribute collection
                        if (lndata3.Count() > 0)
                        {
                            String sp = lndata3[0].trl.Path.ToString();
                            int spsub = sp.IndexOf("COD_External");
                            sp = sp.Substring(spsub, 19);
                            if (sp.Length != 0)
                                sp = sp.Substring(sp.Length - 2);
                            atName = sp.ToLower();
                        }
                        else
                        {
                            var gistLangHelper = new CODV2API.Handlers.GistMapsServiceHandler(null, iso3rec);
                            atName = gistLangHelper.service_lang_abbrev;
                            if (!(atName is null))
                                atName = atName.ToLower();
                        }

                        JToken jsonObject = JToken.Parse(res);

                        foreach (JProperty item in jsonObject)
                        {
                            if (item.Name == "features")
                            {
                                switch (level)
                                {
                                    case 4:
                                        JArray iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            levellatlngLookup level4Summary = new levellatlngLookup();
                                            level4Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level4Summary.admin0Name_ref = "Null";
                                            level4Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level4Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level4Summary.admin1Name_ref = "Null";
                                            level4Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level4Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level4Summary.admin2Name_ref = "Null";
                                            level4Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level4Summary.admin3Name_local = jt["attributes"]["admin3Name_" + atName].ToString();
                                            level4Summary.admin3Name_ref = "Null";
                                            level4Summary.admin3Pcode = jt["attributes"]["admin3Pcode"].ToString();
                                            level4Summary.admin3Name_local = jt["attributes"]["admin4Name_" + atName].ToString();
                                            level4Summary.admin3Name_ref = "Null";
                                            level4Summary.admin3Pcode = jt["attributes"]["admin4Pcode"].ToString();
                                            level4Summary.status = "OK";
                                            levelAllSummaryList.Add(level4Summary);

                                        }
                                        return Json(levelAllSummaryList);
                                        break;
                                    case 3:
                                        iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            levellatlngLookup level3Summary = new levellatlngLookup();
                                            level3Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level3Summary.admin0Name_ref = "Null";
                                            level3Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level3Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level3Summary.admin1Name_ref = "Null";
                                            level3Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level3Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level3Summary.admin2Name_ref = "Null";
                                            level3Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level3Summary.admin3Name_local = jt["attributes"]["admin3Name_" + atName].ToString();
                                            level3Summary.admin3Name_ref = "Null";
                                            level3Summary.admin3Pcode = jt["attributes"]["admin3Pcode"].ToString();
                                            level3Summary.status = "OK";
                                            levelAllSummaryList.Add(level3Summary);

                                        }
                                        return Json(levelAllSummaryList);
                                        break;
                                    case 2:
                                        iso2Feat = JArray.Parse(item.Value.ToString());

                                        List<level2latlngLookup> level2AllSummaryList = new List<level2latlngLookup>();
                                        foreach (var jt in iso2Feat)
                                        {
                                            level2latlngLookup level2Summary = new level2latlngLookup();
                                            level2Summary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            level2Summary.admin0Name_ref = "Null";
                                            level2Summary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level2Summary.admin1Name_ref = "Null";
                                            level2Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2Summary.admin2Name_local = jt["attributes"]["admin2Name_" + atName].ToString();
                                            level2Summary.admin2Name_ref = "Null";
                                            level2Summary.admin2Pcode = jt["attributes"]["admin2Pcode"].ToString();
                                            level2Summary.status = "OK";
                                            level2AllSummaryList.Add(level2Summary);
                                        }
                                        return Json(level2AllSummaryList);
                                        break;
                                    case 1:
                                        //return level 1

                                        List<level1latlngLookup> level1SummaryList = new List<level1latlngLookup>();
                                        iso2Feat = JArray.Parse(item.Value.ToString());
                                        foreach (var jt in iso2Feat)
                                        {
                                            level1latlngLookup level1Summary = new level1latlngLookup();
                                            level1Summary.admin1Name_local = jt["attributes"]["admin1Name_" + atName].ToString();
                                            level1Summary.admin1Name_ref = "Null";
                                            level1Summary.admin1Pcode = jt["attributes"]["admin1Pcode"].ToString();
                                            level1Summary.status = "OK";
                                            level1SummaryList.Add(level1Summary);
                                        }
                                        return Json(level1SummaryList);
                                        break;
                                    case 0:
                                        iso2Feat = JArray.Parse(item.Value.ToString());
                                        List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                                        foreach (var jt in iso2Feat)
                                        {
                                            levelSummary.admin0Name_local = jt["attributes"]["admin0Name_" + atName].ToString();
                                            levelSummary.admin0Name_ref = "Null";
                                            levelSummary.admin0Pcode = jt["attributes"]["admin0Pcode"].ToString();
                                            levelSummary.status = "OK";
                                            level0SummaryList.Add(levelSummary);
                                        }
                                        return Json(level0SummaryList);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                    levelSummary.admin0Name_local = cname;
                    levelSummary.admin0Name_ref = "Null";
                    levelSummary.admin0Pcode = iso2Rec;
                    levelSummary.status = "Not Supported in this release";
                    level0SummaryList.Add(levelSummary);
                    return Json(level0SummaryList);
                }
            }
            catch (Exception e)
            {
                List<level0latlngLookup> level0SummaryList = new List<level0latlngLookup>();
                level0latlngLookup levelSummary = new level0latlngLookup();
                levelSummary.admin0Name_local = "";
                levelSummary.admin0Name_ref = "Null";
                levelSummary.admin0Pcode = "";
                levelSummary.status = "Check required parameters: <level=n>  <=iso2=zz>";
                level0SummaryList.Add(levelSummary);
                return Json(level0SummaryList);
            }

            return Json("");
        }
        private string LevelServiceAGSNode(string Path, int? level)
        {
            string node = "";
            string id;
            string cname;
            StringBuilder requestUrlSB = new StringBuilder(Path + "MapServer?f=pjson");

            HttpWebRequest request = WebRequest.Create(requestUrlSB.ToString()) as HttpWebRequest;
            request.KeepAlive = true;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader responseStream = new StreamReader(response.GetResponseStream());
                var res = responseStream.ReadToEnd();
                JToken jsonObject = JToken.Parse(res);

                foreach (JProperty item in jsonObject)
                {
                    if (item.Name == "layers")
                    {
                        JArray iso2Feat = JArray.Parse(item.Value.ToString());
                        foreach (var jt in iso2Feat)
                        {
                            id = jt["id"].ToString();
                            cname = jt["name"].ToString();
                            if (level is null)
                                level = 0;
                            if (cname.Contains(level.ToString()))
                                node = id;
                        }
                    }
                }
            }

            return node;
        }
      
    }
      
}