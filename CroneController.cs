using Newtonsoft.Json;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace GrowFast.Controllers
{
    public class CroneController : Controller
    {
        // GET: Crone
        DbManager db = new DbManager();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FestivalMessage()
        {
            string msg = "";
            try
            {
                string festname = "", Message = "", Mobile = "", Name="";
                string soap="", dlt="",header_id = "";
                string msgquery = "select * from [tbl_DLTTemplate] where Template_name='Festival'";
                DataTable msgdt = db.GetAllRecord(msgquery);
                if (msgdt.Rows.Count > 0)
                {
                    dlt = msgdt.Rows[0]["PEID"] + "";
                    header_id = msgdt.Rows[0]["Header_id"] + "";
                    soap = msgdt.Rows[0]["Templateid"] + "";
                    Message = (msgdt.Rows[0]["Template_text"] + "").Replace("``", "\\").Replace("`", "'");

                }


                string cusquery = "select * from tbl_customer where Status='Active111'";
                DataTable cusdt = db.GetAllRecord(cusquery); 
                string userquery = "select * from tbl_registration where Status='Approved'";
                DataTable userdt = db.GetAllRecord(userquery);


                string query = "select * from tbl_Holiday where CONVERT(date, Date) ='" + DateTime.Now.ToString("yyyy-MM-dd")+"' and Status='Active'";
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    for(int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (festname == "")
                        {
                            festname += dt.Rows[i]["Holiday"] + "";
                        }
                        else
                        {
                            festname += ", "+dt.Rows[i]["Holiday"] + "";
                        }
                        
                        
                        //Message = Message.Replace("{#var#}", festname);

                        
                    }

                    if (cusdt.Rows.Count > 0)
                    {
                        for (int j = 0; j < cusdt.Rows.Count; j++)
                        {
                            if (Mobile == "")
                            {
                                Mobile += cusdt.Rows[j]["Mobile"] + "";
                            }
                            else
                            {
                                Mobile += ","+cusdt.Rows[j]["Mobile"] + "";
                            }
                            if (Name == "")
                            {
                                Name += cusdt.Rows[j]["Name"] + "";
                            }
                            else
                            {
                                Name += "," + cusdt.Rows[j]["Name"] + "";
                            }
                            
                            Mobile = "8081327024";
                            Name = "Sandhya";
                        }
                    }
                    else
                    {
                        msg = "Customer Not Found";
                    }
                    if (userdt.Rows.Count > 0)
                    {
                        for (int j = 0; j < userdt.Rows.Count; j++)
                        {
                            if (Mobile == "")
                            {
                                Mobile += userdt.Rows[j]["Mobile"] + "";
                            }
                            else
                            {
                                Mobile += "," + userdt.Rows[j]["Mobile"] + "";
                            }
                            if (Name == "")
                            {
                                Name += userdt.Rows[j]["Name"] + "";
                            }
                            else
                            {
                                Name += "," + userdt.Rows[j]["Name"] + "";
                            }
                            //Mobile = "8081327024";
                            //Name = "Sandhya";
                        }
                    }
                    else
                    {
                        msg = "User Not Found";
                    }
                    if (Mobile != "")
                    {
                        string[] mobArray = Mobile.Split(',');
                        string[] nameArray = Name.Split(',');
                        // Using a for loop to iterate over mobArray
                        for (int i = 0; i < mobArray.Length; i++)
                        {
                            string[] replacementValues = { festname };
                            Messaging.SendSMSNew(mobArray[i], replacementValues, "Festival", nameArray[i], "Crone","Crone");
                            Messaging.SendWhatsappSMSNew(mobArray[i], replacementValues, "Festival", nameArray[i], "Crone", "Crone", "Crone");
                        }
                        
                        //Mobile = Mobile.Substring(0, Mobile.Length - 1);
                        //string contact = "http://bulksms.turt.info/submitsms.jsp?user=GROWFAST&key=e71001e98dXX&mobile=" + Mobile.ToString() + "&message=" + Message + "&senderid=" + header_id + "&accusage=1&entityid=" + dlt + "&tempid=" + soap + "";


                        //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(contact);
                        //using (WebResponse response1 = webRequest.GetResponse())
                        //using (StreamReader rd = new StreamReader(response1.GetResponseStream()))
                        //{
                        //    soap = rd.ReadToEnd();
                        //}
                        //msg = soap;


                        ////WhatsApp Message


                        ////Messaging.SendWhatsappSMSNew(cnmobile, replacementValues, "Booking Message", cname, Session["userid"] + "", Session["username"] + "", Session["emprid"] + "");


                    }
                    else
                    {
                        msg = "User Not Found";
                    }
                }
                else
                {
                    msg = "Holiday Not Found";
                }

            }
            catch (Exception ex)
            {
                msg = "Failed";
            }
            return Content(msg);
        }

        public ActionResult other()
        {
            string[] replacementValues = { "sandhya", "Lucknow", "sandhya", "Lucknow UP" };
            Messaging.SendSMSNew("8081327024", replacementValues, "Preparation Message","Sandhya Patel", "Crone", "Crone");
            return Content("success");
        }

        public ActionResult TestWhatsAppMessage()
        {
            string[] replacementValues = { "sandhya", "Lucknow", "sandhya", "Lucknow UP" };
            string msg=Messaging.SendWhatsappSMSNew("918081327024@c.us", replacementValues, "Preparation Message", "Sandhya", "san123", "sandhya", "2");
            return Content(msg);
        }

        public ActionResult SendAPKWhatsAppMessage()
        {
            string[] replacementValues = { "sandhya", "Lucknow", "sandhya", "Lucknow UP" };
            //string msg = Messaging.SendWhatsappSMfileSNew("916306564130@c.us", "https://growfastgroups.com/Application/app-debug.apk", "Android App", "Garima", string templatename, string username, string logid, string logname, string logrowid);

            string msg = "";
            string instanceid = "6831";
            string tokenkey = "5JJbcWQOffibWs89LBJeJJcb2GnHckdX4UCcDmGs75fa6cb8";

            string apiUrl = "https://waapi.app/api/v1/instances/" + instanceid + "/client/action/send-media";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenkey + "");
                Uri requestUri = new Uri(apiUrl);
                var requestBodyObject = new { chatId = "916306564130@c.us", mediaUrl = "https://growfastgroups.com/Application/app-debug.apk", mediaCaption = "Android App", mediaName = "GODApp" };

                // request.AddJsonBody("{\"chatId\":\"919519838425@c.us\",\"mediaUrl\":\"https://growfastorganics.com/Content/ProductImage/182024114325AM_default.jpg\",\"mediaCaption\":\"Test Image\",\"mediaName\":\"default.jpg\"}", false);



                string requestBodyJson = JsonConvert.SerializeObject(requestBodyObject);
                HttpContent requestbody = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(requestUri, requestbody).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                if (jsonObject.status == "success")
                {
                    string status = jsonObject.data.status;
                    msg = jsonObject.data.status;
                    if (jsonObject.data.status == "success")
                    {
                        
                        //db.InsertUpdateDelete("insert into tbl_whatsapp_transaction(Temprowid,Name,mobile,Header,Status,StatusMessage,Message,Datetime,Adminrowid,Logname,Logid) values('" + temprowid + "','name','" + MobileNumber + "','" + Message + "','" + jsonObject.status + "','" + jsonObject.status + "','" + Message + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logrowid + "','" + logname + "','" + logid + "')");
                    }
                    else
                    {
                        
                        //db.InsertUpdateDelete("insert into tbl_whatsapp_transaction(Temprowid,Name,mobile,Header,Status,StatusMessage,Message,Datetime,Adminrowid,Logname,Logid) values('" + temprowid + "','name','" + MobileNumber + "','" + Message + "','" + jsonObject.status + "','" + jsonObject.status + "','" + Message + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logrowid + "','" + logname + "','" + logid + "')");
                    }
                }
                else
                {
                    msg = jsonObject.status;

                    
                    //db.InsertUpdateDelete("insert into tbl_whatsapp_transaction(Temprowid,Name,mobile,Header,Status,StatusMessage,Message ,Datetime,Adminrowid,Logname,Logid) values('" + temprowid + "','name','" + MobileNumber + "','" + Message + "','" + jsonObject.status + "','" + jsonObject.status + "','" + Message + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + logrowid + "','" + logname + "','" + logid + "')");
                }

            }

            return Content(msg);
        }

        public ActionResult CopyWhatsAppMessage()
        {
            string query = "select * from [tbl_whatsapp_transaction] where StatusMessage='success'";
            DataTable dt = db.GetAllRecord(query);
            if (dt.Rows.Count > 0)
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    string query1 = "insert into tbl_All_Message_transaction([receiver_name],[reviever_mobile],[Header],[Status],[StatusMessage],[Message],[Datetime],[create_by],[create_name],[empid],[msg_type],[status_description],[membertype_from],[membertype_to],[from_msg],[sender_mobile],[sms_header],[sms_response_body],[status1]) values('" + dt.Rows[i]["Name"] + "','" + dt.Rows[i]["mobile"] + "','" + dt.Rows[i]["Header"] + "','Active','true','" + dt.Rows[i]["Message"] + "','" + DateTime.Parse(dt.Rows[i]["Datetime"]+"").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[i]["create_by"] + "','" + dt.Rows[i]["create_by"] + "','" + dt.Rows[i]["empid"] + "','Text','" + dt.Rows[i]["StatusMessage"] + "','" + dt.Rows[i]["create_by"] + "','" + dt.Rows[i]["mobile"] + "','Whatsapp','','" + dt.Rows[i]["StatusMessage"] + "','" + dt.Rows[i]["jsonresponse"] + "','true')";
                    db.InsertUpdateDelete(query1);
                }
            }
            return View();
        }

        public ActionResult sendlink()
        {
            string msg = "";
            string Message ="Hey, click <a href='\"https://example.com\"'>here</a> to visit the website.";
            string apiUrl = "https://waapi.app/api/v1/instances/7323/client/action/send-message";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer oYeqfVfmNevUJww547Hc4OO5QJ6DRL3eUTPEetBo7980a517");
                Uri requestUri = new Uri(apiUrl);
                var requestBodyObject = new { chatId = "918081327024@c.us", message = Message };
                string requestBodyJson = JsonConvert.SerializeObject(requestBodyObject);
                HttpContent requestbody = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(requestUri, requestbody).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                if (jsonObject.status == "success")
                {
                    string status = jsonObject.data.status;
                    msg = jsonObject.data.status;
                    
                }
                else
                {
                    msg = jsonObject.status;

                    
                }

            }
            return Content(msg);

        }

        public ActionResult FestivalMessage123()
        {
            string msg = "";


            try
            {
                
                string apiUrl = "https://backend.api-wa.co/campaign/smartping/api/v2";
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://backend.api-wa.co/campaign/smartping/api/v2");
                    var content = new StringContent(" {\r\n \"apiKey\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY2NjAzMjQ4ODA1N2QxNDQ1Y2VmNmY2YyIsIm5hbWUiOiJHT0QiLCJhcHBOYW1lIjoiQWlTZW5zeSIsImNsaWVudElkIjoiNjY1MDU5ZTQ5YWI3MTE3NTZkYzc1MzY0IiwiYWN0aXZlUGxhbiI6Ik5PTkUiLCJpYXQiOjE3MTc1ODAzNjB9.J1qcCr5BYtxjEkjw9zghGH4lpeWZ1tJVbhnykYzxFOI\",\r\n    \"campaignName\": \"Festival_123\",\r\n    \"destination\": \"+918081327024\",\r\n    \"userName\": \"Sandhya\",\r\n    \"media\": {\r\n        \"url\": \"https://upload.wikimedia.org/wikipedia/commons/3/3f/JPEG_example_flower.jpg\",\r\n        \"filename\": \"png\"\r\n    },\r\n    \"templateParams\": [\r\n        \"Sandhya Patel\"\r\n    ]\r\n}", null, "application/json");
                    request.Content = content;

                    HttpResponseMessage response = httpClient.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine(response.Content.ReadAsStringAsync());
                    

                }
            }
            catch (Exception ex)
            {

            }

            return Content(msg);
        }
        public ActionResult Genpdf()
        {
            string bootstrapCss = @"
<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/5.3.0/css/bootstrap.min.css' integrity='sha384-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX' crossorigin='anonymous'>
";

string htmlFormData = bootstrapCss + @"
<style>

td { border: 1px solid black; padding:5px}
</style>
<div class='row row-cols-1 row-cols-sm-1 p-2 pb-0 no-print' id='invoice1'>
    <div class='col'>
        <div class='row row-cols-1 row-cols-sm-1 p-1 pb-0 no-print me-2'>
            <div class='col'>
                <table class='table table-bordered table-striped border-dark print-div' style='width: 100%;' cellspacing='0'>
                    <tr class='text-black'>
<td style='width: 100%;text-align:center' colspan='2'><div style='width:100%'><img src='https://growfastgroups.com/Content/Img/Growfast%20Logo.png' style='height: 40px;' />
</div><div style='width:100%;text-align:center'><label>Corporate office - RS Plaza 3rd floor Ring Road Kalyanpur near Jagrani Hospital Lucknow (U.P)-226022<br /> Regional office - Plot no.12, karamveer Nagar, Nizamuddin Chouraha A&P International School, Bhopal <b> PHONE NO- 9415344987 <br /> Email: agriculturegod@gmail.com  <br /> Website: www.growfastgroups.com</b> </label>
</div></td></tr>
                    <tr class='text-black'>
                        <td class='text-center' colspan='8' style='text-align:center'><label><b>ADVANCE RECEIPT (CUSTOMER COPY)</b></label></td>
                    </tr>
                    <tr class='text-black'>
                        <td class='text-left' style='width: 50%;'><label><b>ORDER NO : O54q36hjdgfj</b></label></td>
                        <td class='text-left' style='width: 50%;'><label><b>DATE : 534:367:3</b></label></td>
                    </tr>
                    <tr class='text-black'>
                        <td class='text-left' style='width: 50%;'>
                            <label><b>RECEIVED WITH THANKS FROM</b><br>kjsdhk<br>hgdsfgsdkfgdshfjdshfkjdsh<br>MOB - 876386487326</label>
                        </td>
                        <td class='text-left' style='width: 50%;'>
                            <img src='http://growfastgroups.com/Content/Img/verified.png' style='height: 25px;' /><label><span style='margin-top:30px;padding-top:10px !important'> verified</span></label>
                        </td>
                    </tr>
                    <tr class='text-black'>
                        <td class='text-left' colspan='8'><label><b>ADVANCE AMOUNT OF RS. 1000/- by 5757</b></label></td>
                    </tr>
                    <tr class='text-black'>
                        <td class='text-left' colspan='8'><label><b>PRODUCT- product</b></label></td>
                    </tr>
                    <tr>
                        <td><label>By Growfast</label></td>
                        <td><label><b>PAYMENT DATE : jaghdj</b></label></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>";

string filename = "pdf113.pdf";
string path1 = Server.MapPath("~/Content/");
PdfGenerateConfig pdfGenerateConfig = new PdfGenerateConfig();
pdfGenerateConfig.PageSize = PageSize.A4;
pdfGenerateConfig.PageOrientation = PageOrientation.Portrait;
pdfGenerateConfig.SetMargins(20);

PdfSharp.Pdf.PdfDocument pdfDocument = PdfGenerator.GeneratePdf(htmlFormData, pdfGenerateConfig);

using (MemoryStream memoryStream = new MemoryStream())
{
    pdfDocument.Save(memoryStream);
    pdfDocument.Close();
    byte[] bytes = memoryStream.ToArray();
    string fpth = System.IO.Path.Combine(path1, filename);
    System.IO.File.WriteAllBytes(fpth, bytes);
}




            return Content("");
        }
        public ActionResult pushnotification()
        {
            string msg = "";


           

            return Content(msg);
        }
    }
}