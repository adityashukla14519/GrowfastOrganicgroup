using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Growfast.Controllers
{
    public class HomeController : Controller
    {
        DbManager db = new DbManager();
        Activitylog activitylog = new Activitylog();
        private string GetIP()
        {
            // Get all network interfaces on the system
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Find the first interface that is not a loopback device and has an IP address assigned
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet)
                {
                    // Get the IP properties of the interface
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    // Find the first IP address assigned to the interface
                    foreach (IPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
                    {
                        if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ipAddressInfo.Address))
                        {
                            // Found the IP address, return it
                            return ipAddressInfo.Address.ToString();
                        }
                    }
                }
            }
            return null;
        }
        [OutputCache(CacheProfile = "NoCache",Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Registration()
        {
            return View();
        }
        [OutputCache(CacheProfile = "NoCache", Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Login()
        {
            if (Session["suid"] + "" != null && Session["suid"] + "" != "")
            {
                Response.Redirect("/Admin/Index");
            }
            else if (Session["auid"] + "" != null && Session["auid"] + "" != "")
            {
                Response.Redirect("/Hr/Index");
            }
            else if (Session["userid"] + "" != null && Session["userid"] + "" != "")
            {
                Response.Redirect("/User/Index");
            }
            else if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
            {
                Response.Redirect("/Customer/Dashboard");
            }

            else
            {

            }
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Login(string loginid, string password, string mobile)
        {
            try
            {
                string query = "select * FROM tbl_login WHERE (Emailid='" + loginid + "' or  Mobile='" + mobile + "') and Password='" + password + "' and Status='Active' ";
                //activitylog.Activitylogins("tbl_login", "", query, "Failed", "", loginid, "", loginid);
                DataTable dt = db.GetAllRecord(query);
                //activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string branchname = dt.Rows[0]["Branchname"] + "";
                    string branchcode = dt.Rows[0]["Branchcode"] + "";
                     Session["logmails"] = useremail;
                    if (type == "Sadmin")
                    {
                        if (status == "Active")
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                            Session["smail"] = useremail;
                            Session["suid"] = userid;
                            Session["suname"] = username;
                            //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "You are not a active user", userid, username, useremail);
                            ViewBag.msg = "You are not a active user";
                        }

                    }
                    else if (type == "Admin")
                    {
                        if (status == "Active")
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                            Session["amail"] = useremail;
                            Session["auid"] = userid;
                            Session["auname"] = username;
                            Session["abrname"] = branchname;
                            Session["abrcode"] = branchcode;

                            string branchquery = "Select * from tbl_branch where Branchid='" + branchcode + "'";
                            DataTable branchdt = db.GetAllRecord(branchquery);
                            if (branchdt.Rows.Count > 0)
                            {
                                Session["Companyidm"] = branchdt.Rows[0]["Companyid"] + "";
                                Session["Companynamem"] = branchdt.Rows[0]["Companyname"] + "";
                                Session["Companyprefixm"] = branchdt.Rows[0]["Companyprefix"] + "";
                            }

                            //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                            return RedirectToAction("Index", "Hr");

                        }
                        else
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "You are not a active user", userid, username, useremail);
                            ViewBag.msg = "You are not a active user";
                        }
                    }
                    else if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "Ouser" || type == "AFuser")
                    {
                        string selquery = "Select * from tbl_registration where Employee_id='" + userid + "' and Status='Approved'";
                        DataTable seldt = db.GetAllRecord(selquery);
                        if (seldt.Rows.Count > 0)
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                            Session["membertype"] = type;
                            Session["usermail"] = useremail;
                            Session["userid"] = userid; 
                            Session["userphon"] = userphon;
                            Session["username"] = username;

                            Session["emprid"] = seldt.Rows[0]["Id"] + "";
                            Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                            Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                            Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                            Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                            Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";

                            Session["Companyid"] = seldt.Rows[0]["Companyid"] + "";
                            Session["Zoneid"] = seldt.Rows[0]["Zoneid"] + "";
                            Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                            Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                            Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                            Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";
                            Session["designation"] = seldt.Rows[0]["Designation"] + "";
                            Session["Premises"] = seldt.Rows[0]["Premises"] + "";

                            string branchquery = "Select * from tbl_branch where Branchid='" + seldt.Rows[0]["Branchcode"] + "'";
                            DataTable branchdt = db.GetAllRecord(branchquery);
                            if (branchdt.Rows.Count > 0)
                            {
                                Session["Yardid"] = branchdt.Rows[0]["Yardid"] + "";
                                Session["Yardname"] = branchdt.Rows[0]["Yardname"] + "";
                                Session["Yardrowid"] = branchdt.Rows[0]["Yardrowid"] + "";
                            }
                            if (type == "Suser")
                            {
                                string groupquery = "Select * from tbl_group where Employee_id='" + userid + "' and Status='Active' and Month_Year='" + DateTime.Now.ToString("M-yyyy") + "'";
                                DataTable groupdt = db.GetAllRecord(groupquery);
                                if (groupdt.Rows.Count > 0)
                                {
                                    Session["Companyid"] = groupdt.Rows[0]["Companyid"] + "";
                                    Session["Zoneid"] = groupdt.Rows[0]["Zoneid"] + "";
                                    Session["Regionid"] = groupdt.Rows[0]["Regionid"] + "";
                                    Session["Divisionid"] = groupdt.Rows[0]["Divisionid"] + "";
                                    Session["Branchid"] = groupdt.Rows[0]["Branchid"] + "";
                                    Session["Teamid"] = groupdt.Rows[0]["Teamid"] + ""; 
                                    Session["Groupname"] = groupdt.Rows[0]["Groups"] + "";
                                    Session["Groupid"] = groupdt.Rows[0]["Groupid"] + "";
                                    Session["CGroupid"] = groupdt.Rows[0]["Groupid"] + "";
                                    Session["CGroupname"] = groupdt.Rows[0]["Groups"] + "";
                                }
                                    
                            }
                            if (type == "Auser")
                            {
                                string servicequery = "Select * from tbl_servicearea where Employee_id='" + userid + "'";
                                DataTable servicedt = db.GetAllRecord(servicequery);
                                if (servicedt.Rows.Count > 0)
                                {
                                    string State = "", District="", Tahsil="", Block = "", Pincode="", reporting_id="", reporting_rowid="", reporting_name="";
                                    for (int i=0;i< servicedt.Rows.Count; i++)
                                    {
                                        State+= servicedt.Rows[0]["State"] + ",";
                                        District += servicedt.Rows[0]["District"] + ",";
                                        Tahsil += servicedt.Rows[0]["Tahsil"] + ",";
                                        Block += servicedt.Rows[0]["Vill_town"] + ",";
                                        Pincode += servicedt.Rows[0]["Pincode"] + ",";
                                    }
                                    State = State.Substring(0, State.Length - 1);
                                    District = District.Substring(0, District.Length - 1);
                                    Tahsil = Tahsil.Substring(0, Tahsil.Length - 1);
                                    Block = Block.Substring(0, Block.Length - 1);
                                    Pincode = Pincode.Substring(0, Pincode.Length - 1);

                                    Session["State"] = State;
                                    Session["District"] = District;
                                    Session["Tahsil"] = Tahsil;
                                    Session["Block"] = Block;
                                    Session["Pincode"] = Pincode;
                                    Session["reporting_id"] = servicedt.Rows[0]["reporting_id"] + "";
                                    Session["reporting_rowid"] = servicedt.Rows[0]["reporting_rowid"] + "";
                                    Session["reporting_name"] = servicedt.Rows[0]["reporting_name"] + "";
                                }

                            }
                            
                            //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            ViewBag.msg = "You are not a active user";
                        }

                    }
                    else if (type == "Customer")
                    {
                        // return RedirectToAction("Index", "Customer");

                        string selquery = "Select * from [tbl_customer] where Customer_id='" + userid + "' and Status='Active'";
                        DataTable seldt = db.GetAllRecord(selquery);
                        if (seldt.Rows.Count > 0)
                        {
                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                            Session["membertype"] = type;
                            Session["usermail"] = useremail;
                            Session["customerid"] = userid;
                            Session["username"] = username;
                            //Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                            //Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                            //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                            return RedirectToAction("Dashboard", "Customer");
                        }
                        else
                        {
                            ViewBag.msg = "You are not a active user";
                        }

                    }

                    else
                    {

                        //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "Invalid User Type", userid, username, useremail);
                        ViewBag.msg = "Invalid User Type";
                    }

                }
                else
                {

                    //activitylog.Activitylogins("tbl_login", "", query, "Failed", "Invalid User Id or Password", loginid, "", loginid);
                    ViewBag.msg = "Invalid User Id or Password";
                }
            }
            catch (Exception ex)
            {

                ViewBag.msg = "Invalid User Id or Password";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult ForgotPassword(string mailid)
        {
            try
            {
                string query1 = "", query2 = "", query3 = "";
                string query = "select * FROM tbl_login WHERE Emailid='" + mailid + "'";
                activitylog.Activitylogins("tbl_login", "", query, "Failed", "", mailid, "", mailid);
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string name = dt.Rows[0]["Username"] + "";
                    //string loginid = dt.Rows[0]["Email"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string password = "win_" + RandomPassword();

                    //string apiquery = "select * FROM tbl_email_api";
                    //activitylog.Activitylogins("tbl_email_api", "", apiquery, "Failed", "", userid, username, useremail);
                    //DataTable apidt = db.GetAllRecord(apiquery);
                    //activitylog.Activitylogupd("Success", "");
                    //if (apidt.Rows.Count > 0)
                    //{
                    //    string apiusername = apidt.Rows[0]["Userid"] + "";
                    //    string apipassword = apidt.Rows[0]["Password"] + "";
                    //    string apihostname = apidt.Rows[0]["Hostname"] + "";
                    //    string apiportno = apidt.Rows[0]["Portno"] + "";
                    //    string body = "Hii " + name + ", \n Your Loginid is : " + useremail + " and Password is : " + password + ".";
                    //MailMessage mail = new MailMessage();
                    //mail.From = new MailAddress(apiusername);

                    //mail.To.Add(mailid);
                    //mail.Subject = "Forget Password";

                    //mail.Body = body;
                    //mail.IsBodyHtml = true;
                    //SmtpClient smt = new SmtpClient();
                    //smt.Host = apihostname;
                    //System.Net.NetworkCredential ntwd = new NetworkCredential();
                    //ntwd.UserName = apiusername; //Your Email ID  
                    //ntwd.Password = apipassword; // Your Password  
                    //smt.EnableSsl = false;
                    //smt.UseDefaultCredentials = true;
                    //smt.Credentials = ntwd;
                    //smt.Port = Convert.ToInt32(apiportno);
                    //smt.Timeout = 10000;



                    if (status == "Active")
                    {
                        if (type == "Sadmin")
                        {
                            query2 = "Update tbl_login set Password='" + password + "' where Emailid='" + mailid + "' ";
                            if (db.InsertUpdateDelete(query2))
                            {
                                //sendmail.SendMail(apiusername, apipassword, apihostname, apiportno, mailid, "Forget Password", body);
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Success", "Password Update", userid, username, useremail);
                                ViewBag.msg = "Mail Sent";
                                return RedirectToAction("Login", "Home");

                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Failed", "Password Update Failed", userid, username, useremail);
                                ViewBag.msg = "Mail Sending Failed";
                            }
                        }
                        else if (type == "Admin")
                        {
                            query2 = "Update tbl_login set Password='" + password + "' where Emailid='" + mailid + "' ";
                            query3 = "Update tbl_company set Password='" + password + "' where Companymail='" + mailid + "' ";
                            if (db.InsertUpdateDelete(query2) && db.InsertUpdateDelete(query3))
                            {
                                //sendmail.SendMail(apiusername, apipassword, apihostname, apiportno, mailid, "Forget Password", body);
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Success", "Password Update", userid, username, useremail);
                                activitylog.Activitylogins("tbl_company", db.getrowid("select Id from tbl_company where Companymail='" + mailid + "'").ToString(), query3, "Success", "Password Update", userid, username, useremail);
                                ViewBag.msg = "Mail Sent";
                                return RedirectToAction("Login", "Home");
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Failed", "Password Update Failed", userid, username, useremail);
                                activitylog.Activitylogins("tbl_company", db.getrowid("select Id from tbl_company where Companymail='" + mailid + "'").ToString(), query3, "Failed", "Password Update Failed", userid, username, useremail);
                                ViewBag.msg = "Mail Sending Failed";
                            }
                        }
                        else if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "Ouser" || type == "AFuser")
                        {
                            query2 = "Update tbl_login set Password='" + password + "' where Emailid='" + mailid + "' ";
                            query3 = "Update tbl_registration set pass='" + password + "' where Email='" + mailid + "' ";
                            if (db.InsertUpdateDelete(query2) && db.InsertUpdateDelete(query3))
                            {
                                //sendmail.SendMail(apiusername, apipassword, apihostname, apiportno, mailid, "Forget Password", body);
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Success", "Password Update", userid, username, useremail);
                                activitylog.Activitylogins("tbl_registration", db.getrowid("select Id from tbl_registration where Email='" + mailid + "'").ToString(), query3, "Success", "Password Update", userid, username, useremail);
                                ViewBag.msg = "Mail Sent";
                                return RedirectToAction("Login", "Home");
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Failed", "Password Update Failed", userid, username, useremail);
                                activitylog.Activitylogins("tbl_registration", db.getrowid("select Id from tbl_registration where Email='" + mailid + "'").ToString(), query3, "Failed", "Password Update Failed", userid, username, useremail);
                                ViewBag.msg = "Mail Sending Failed";
                            }
                        }
                        else if (type == "Customer")
                        {
                            query2 = "Update tbl_login set Password='" + password + "' where Emailid='" + mailid + "' ";
                            query3 = "Update tbl_customer set Password='" + password + "' where Email='" + mailid + "' ";
                            if (db.InsertUpdateDelete(query2) && db.InsertUpdateDelete(query3))
                            {
                                //sendmail.SendMail(apiusername, apipassword, apihostname, apiportno, mailid, "Forget Password", body);
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Success", "Password Update", userid, username, useremail);
                                activitylog.Activitylogins("tbl_customer", db.getrowid("select Id from tbl_customer where Email='" + mailid + "'").ToString(), query3, "Success", "Password Update", userid, username, useremail);
                                ViewBag.msg = "Mail Sent";
                                return RedirectToAction("Login", "Home");
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Failed", "Password Update Failed", userid, username, useremail);
                                activitylog.Activitylogins("tbl_customer", db.getrowid("select Id from tbl_customer where Email='" + mailid + "'").ToString(), query3, "Failed", "Password Update Failed", userid, username, useremail);
                                ViewBag.msg = "Mail Sending Failed";
                            }
                        }

                        else
                        {
                            activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query2, "Failed", "Mail Sending Failed", userid, username, useremail);
                            ViewBag.msg = "Invalid User Type";
                        }
                    }

                    else
                    {
                        activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "You are not a active user", userid, username, useremail);
                        ViewBag.msg = "You are not a active user";
                    }
                    //}
                    //else
                    //{
                    //    activitylog.Activitylogins("tbl_email_api", "", apiquery, "Failed", "Email APi not Registered for this company.", mailid, "", mailid);
                    //    ViewBag.msg = "Email APi not Registered for this company.";
                    //}


                }
                else
                {
                    activitylog.Activitylogins("tbl_login", "", query, "Failed", "We can not find a user with that email address.", mailid, "", mailid);
                    ViewBag.msg = "We can not find a user with that email address.";
                }

            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", ""), errorName.Replace("'", ""), errorTrace.Replace("'", "`"), mailid, "", mailid);
                }

                catch
                {

                }
                ViewBag.msg = "Mail Sending Error";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult LoginWithOtp()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult LoginWithOtp(string otp, string loginid)
        {
            string mail = "";
            try
            {
                string query = "select * FROM tbl_login WHERE Mobile='" + loginid + "' and OTP='" + otp + "' ";
                activitylog.Activitylogins("tbl_login", "", query, "Failed", "", loginid, "", loginid);
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string otptime = dt.Rows[0]["OTP_Time"] + "";
                    string currenttime = DateTime.Now.ToString("HH:mm:ss");
                    TimeSpan duration = DateTime.Parse(currenttime).Subtract(DateTime.Parse(otptime));
                    // the time span in string format
                    TimeSpan maxtime = TimeSpan.Parse("00:02:00");
                    TimeSpan mintime = TimeSpan.Parse("00:00:00");
                    if (duration <= maxtime && duration > mintime)
                    {
                        if (type == "Sadmin")
                        {
                            if (status == "Active")
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["smail"] = useremail;
                                Session["suid"] = userid;
                                Session["suname"] = username;
                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Index", "Admin");

                            }
                            else
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "You are not a active user", userid, username, useremail);
                                ViewBag.msg = "You are not a active user";
                            }

                        }
                        else if (type == "Admin")
                        {
                            if (status == "Active")
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["amail"] = useremail;
                                Session["auid"] = userid;
                                Session["auname"] = username;
                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Index", "Hr");

                            }
                            else
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "You are not a active user", userid, username, useremail);
                                ViewBag.msg = "You are not a active user";
                            }
                        }
                        else if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "Ouser" || type == "AFuser")
                        {
                            string selquery = "Select * from tbl_registration where Employee_id='" + userid + "' and Status='Approved'";
                            DataTable seldt = db.GetAllRecord(selquery);
                            if (seldt.Rows.Count > 0)
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["membertype"] = type;
                                Session["usermail"] = useremail;
                                Session["userid"] = userid;
                                Session["userphon"] = userphon;
                                Session["username"] = username;

                                Session["emprid"] = seldt.Rows[0]["Id"] + "";
                                Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";

                                Session["Companyid"] = seldt.Rows[0]["Companyid"] + "";
                                Session["Zoneid"] = seldt.Rows[0]["Zoneid"] + "";
                                Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";
                                Session["designation"] = seldt.Rows[0]["Designation"] + "";
                                Session["Premises"] = seldt.Rows[0]["Premises"] + "";

                                string branchquery = "Select * from tbl_branch where Branchid='" + seldt.Rows[0]["Branchcode"] + "'";
                                DataTable branchdt = db.GetAllRecord(branchquery);
                                if (branchdt.Rows.Count > 0)
                                {
                                    Session["Yardid"] = branchdt.Rows[0]["Yardid"] + "";
                                    Session["Yardname"] = branchdt.Rows[0]["Yardname"] + "";
                                    Session["Yardrowid"] = branchdt.Rows[0]["Yardrowid"] + "";
                                }
                                if (type == "Suser")
                                {
                                    string groupquery = "Select * from tbl_group where Employee_id='" + userid + "' and Status='Active' and Month_Year='" + DateTime.Now.ToString("M-yyyy") + "'";
                                    DataTable groupdt = db.GetAllRecord(groupquery);
                                    if (groupdt.Rows.Count > 0)
                                    {
                                        Session["Companyid"] = groupdt.Rows[0]["Companyid"] + "";
                                        Session["Zoneid"] = groupdt.Rows[0]["Zoneid"] + "";
                                        Session["Regionid"] = groupdt.Rows[0]["Regionid"] + "";
                                        Session["Divisionid"] = groupdt.Rows[0]["Divisionid"] + "";
                                        Session["Branchid"] = groupdt.Rows[0]["Branchid"] + "";
                                        Session["Teamid"] = groupdt.Rows[0]["Teamid"] + "";
                                        Session["Groupname"] = groupdt.Rows[0]["Groups"] + "";
                                        Session["Groupid"] = groupdt.Rows[0]["Groupid"] + "";
                                        Session["CGroupid"] = groupdt.Rows[0]["Groupid"] + "";
                                        Session["CGroupname"] = groupdt.Rows[0]["Groups"] + "";
                                    }

                                }
                                if (type == "Auser")
                                {
                                    string servicequery = "Select * from tbl_servicearea where Employee_id='" + userid + "'";
                                    DataTable servicedt = db.GetAllRecord(servicequery);
                                    if (servicedt.Rows.Count > 0)
                                    {
                                        string State = "", District = "", Tahsil = "", Block = "", Pincode = "", reporting_id = "", reporting_rowid = "", reporting_name = "";
                                        for (int i = 0; i < servicedt.Rows.Count; i++)
                                        {
                                            State += servicedt.Rows[0]["State"] + ",";
                                            District += servicedt.Rows[0]["District"] + ",";
                                            Tahsil += servicedt.Rows[0]["Tahsil"] + ",";
                                            Block += servicedt.Rows[0]["Vill_town"] + ",";
                                            Pincode += servicedt.Rows[0]["Pincode"] + ",";
                                        }
                                        State = State.Substring(0, State.Length - 1);
                                        District = District.Substring(0, District.Length - 1);
                                        Tahsil = Tahsil.Substring(0, Tahsil.Length - 1);
                                        Block = Block.Substring(0, Block.Length - 1);
                                        Pincode = Pincode.Substring(0, Pincode.Length - 1);

                                        Session["State"] = State;
                                        Session["District"] = District;
                                        Session["Tahsil"] = Tahsil;
                                        Session["Block"] = Block;
                                        Session["Pincode"] = Pincode;
                                        Session["reporting_id"] = servicedt.Rows[0]["reporting_id"] + "";
                                        Session["reporting_rowid"] = servicedt.Rows[0]["reporting_rowid"] + "";
                                        Session["reporting_name"] = servicedt.Rows[0]["reporting_name"] + "";
                                    }

                                }

                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Index", "User");
                            }
                            else
                            {
                                ViewBag.msg = "You are not a active user";
                            }

                        }
                        else if (type == "Customer")
                        {
                            // return RedirectToAction("Index", "Customer");

                            string selquery = "Select * from [tbl_customer] where Customer_id='" + userid + "' and Status='Active'";
                            DataTable seldt = db.GetAllRecord(selquery);
                            if (seldt.Rows.Count > 0)
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["membertype"] = type;
                                Session["usermail"] = useremail;
                                Session["customerid"] = userid;
                                Session["username"] = username;
                                //Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                //Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Dashboard", "Customer");
                            }
                            else
                            {
                                ViewBag.msg = "You are not a active user";
                            }

                        }

                        else
                        {

                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "Invalid User Type", userid, username, useremail);
                            ViewBag.msg = "Invalid User Type";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Otp Has been Expired! Resend Otp.";
                    }
                }
                else
                {
                    activitylog.Activitylogins("tbl_login", "", query, "Failed", "Invalid OTP", loginid, "", loginid);
                    ViewBag.msg = "Invalid OTP";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), mail, "", mail);
                }

                catch
                {

                }
                ViewBag.msg = "An Error Occured";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        public JsonResult OTPSend(string Mob)
        {
            string msg = "", mail = "";
            try
            {
                string query = "select * FROM tbl_login WHERE Mobile='" + Mob + "'";
                activitylog.Activitylogins("tbl_login", "", query, "Failed", "", Mob, "", Mob);
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string rid = dt.Rows[0]["Id"] + "";
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string name = dt.Rows[0]["Username"] + "";
                    string mobile = dt.Rows[0]["Mobile"] + "";
                    string otptime = dt.Rows[0]["OTP_Time"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string currenttime = DateTime.Now.ToString("HH:mm:ss");
                    TimeSpan duration = DateTime.Parse(currenttime).Subtract(DateTime.Parse(otptime));
                    // the time span in string format
                    DateTime getotptime = DateTime.Parse(otptime).AddMinutes(1);
                    DateTime newDateTime = DateTime.Parse(otptime).AddMinutes(1);
                    TimeSpan maxtime = TimeSpan.Parse("00:01:00");
                    TimeSpan mintime = TimeSpan.Parse("00:00:00");

                    if (duration > maxtime || duration < mintime)
                    {
                        if (status == "Active")
                        {
                            string otp = random().ToString();
                            string Message = "Your one time authentication PIN is '" + otp + "'.Please do not share with anyone.Winaxis employee and partners will never ask you for it. WINAXIS";

                            string query1 = "Update tbl_login set OTP='" + otp + "', OTP_Time='" + DateTime.Now.ToString("HH:mm:ss") + "' where Mobile='" + Mob + "' ";
                            if (db.InsertUpdateDelete(query1))
                            {
                                string[] replacementValues = { otp };
                                Messaging.SendSMSNew(Mob, replacementValues, "OTP For Login", name, userid, username);
                                Messaging.SendWhatsappSMSNew1(Mob, "OTP Login", name , userid, name, Session["emprid"] + "", "", "", replacementValues,true);
                                activitylog.Activitylogins("tbl_login", rid, query1, "Success", "OTP Sent on Your Number", userid, username, useremail);
                                msg = "OTP Sent on Your Number";
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", rid, query1, "Failed", "Otp Sending failed", userid, username, useremail);
                                msg = "OTP Sending Failed";
                            }
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_login", rid, query, "Success", "You are not a active user", userid, username, useremail);
                            msg = "You are not a active user";
                        }
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_login", rid, query, "Failed", "You can get otp after 10 mintuts", userid, username, useremail);
                        msg = "You can get otp after " + getotptime.ToString("hh:mm") + " o'clock";
                    }

                }
                else
                {
                    activitylog.Activitylogins("tbl_login", "", query, "Failed", "We can not find a user with that Mobile number.", Mob, "", Mob);
                    msg = "We can not find a user with that Mobile number.";
                }

            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    if (mail == "")
                    {
                        mail = Mob;
                    }
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), mail, "", mail);
                }

                catch
                {

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        private string random()
        {
            var random = new Random();
            var numlength = 4;
            var number = string.Empty;
            var possibleChar = "0123456789";

            for (var i = 0; i < numlength; i++)
            {
                var randomIndex = random.Next(0, possibleChar.Length - 1);
                number += possibleChar[randomIndex];
            }
            return number;
        }
        private string RandomPassword()
        {
            var random = new Random();
            var numlength = 5;
            var number = string.Empty;
            var possibleChar = "abcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < numlength; i++)
            {
                var randomIndex = random.Next(0, possibleChar.Length - 1);
                number += possibleChar[randomIndex];
            }
            return number;
        }


        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Applogin(string token)
        {
            if (Request.QueryString[0].Length > 0)
            {
                if (Session["suid"] + "" != null && Session["suid"] + "" != "")
                {
                    Response.Redirect("/Admin/Index");
                }
                else if (Session["auid"] + "" != null && Session["auid"] + "" != "")
                {
                    Response.Redirect("/Hr/Index");
                }
                else if (Session["userid"] + "" != null && Session["userid"] + "" != "")
                {
                    Response.Redirect("/User/Index");
                }
                else if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    Response.Redirect("/Customer/Dashboard");
                }

                else
                {
                    string tokenid = Request.QueryString[0].ToString();
                    if (tokenid != "" && tokenid != null)
                    {
                        string query = "select * from tbl_login where Token_id='" + tokenid + "' and Loginstatus='1' and Status='Active'";
                        DataTable dt = db.GetAllRecord(query);
                        if (dt.Rows.Count > 0)
                        {
                            string gtokenid = dt.Rows[0]["Token_id"] + "";
                            string username = dt.Rows[0]["Username"] + "";
                            string userid = dt.Rows[0]["Userid"] + "";
                            string userphon = dt.Rows[0]["Mobile"] + "";
                            string useremail = dt.Rows[0]["Emailid"] + "";
                            string type = dt.Rows[0]["Type"] + "";
                            if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "AFuser" || type == "Ouser")
                            {
                                string selquery = "Select * from tbl_registration where Employee_id='" + userid + "' and Status='Approved'";
                                DataTable seldt = db.GetAllRecord(selquery);
                                if (seldt.Rows.Count > 0)
                                {
                                    Session["membertype"] = type;
                                    Session["usermail"] = useremail;
                                    Session["userid"] = userid;
                                    Session["userphon"] = userphon;
                                    Session["username"] = username;

                                    Session["emprid"] = seldt.Rows[0]["Id"] + "";
                                    Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                    Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                    Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                    Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";

                                    Session["Companyid"] = seldt.Rows[0]["Companyid"] + "";
                                    Session["Zoneid"] = seldt.Rows[0]["Zoneid"] + "";
                                    Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                    Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                    Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";
                                    Session["designation"] = seldt.Rows[0]["Designation"] + "";
                                    Session["Premises"] = seldt.Rows[0]["Premises"] + "";

                                    string branchquery = "Select * from tbl_branch where Branchid='" + seldt.Rows[0]["Branchcode"] + "'";
                                    DataTable branchdt = db.GetAllRecord(branchquery);
                                    if (branchdt.Rows.Count > 0)
                                    {
                                        Session["Yardid"] = branchdt.Rows[0]["Yardid"] + "";
                                        Session["Yardname"] = branchdt.Rows[0]["Yardname"] + "";
                                        Session["Yardrowid"] = branchdt.Rows[0]["Yardrowid"] + "";
                                    }
                                    if (type == "Suser")
                                    {
                                        string groupquery = "Select * from tbl_group where Employee_id='" + userid + "'";
                                        DataTable groupdt = db.GetAllRecord(groupquery);
                                        if (groupdt.Rows.Count > 0)
                                        {
                                            Session["Companyid"] = groupdt.Rows[0]["Companyid"] + "";
                                            Session["Zoneid"] = groupdt.Rows[0]["Zoneid"] + "";
                                            Session["Regionid"] = groupdt.Rows[0]["Regionid"] + "";
                                            Session["Divisionid"] = groupdt.Rows[0]["Divisionid"] + "";
                                            Session["Branchid"] = groupdt.Rows[0]["Branchid"] + "";
                                            Session["Teamid"] = groupdt.Rows[0]["Teamid"] + "";
                                            Session["Groupname"] = groupdt.Rows[0]["Groups"] + "";
                                            Session["Groupid"] = groupdt.Rows[0]["Groupid"] + "";
                                            Session["CGroupid"] = groupdt.Rows[0]["Groupid"] + "";
                                            Session["CGroupname"] = groupdt.Rows[0]["Groups"] + "";
                                        }

                                    }
                                    if (type == "Auser")
                                    {
                                        string servicequery = "Select * from tbl_servicearea where Employee_id='" + userid + "'";
                                        DataTable servicedt = db.GetAllRecord(servicequery);
                                        if (servicedt.Rows.Count > 0)
                                        {
                                            string State = "", District = "", Tahsil = "", Block = "", Pincode = "", reporting_id = "", reporting_rowid = "", reporting_name = "";
                                            for (int i = 0; i < servicedt.Rows.Count; i++)
                                            {
                                                State += servicedt.Rows[0]["State"] + ",";
                                                District += servicedt.Rows[0]["District"] + ",";
                                                Tahsil += servicedt.Rows[0]["Tahsil"] + ",";
                                                Block += servicedt.Rows[0]["Vill_town"] + ",";
                                                Pincode += servicedt.Rows[0]["Pincode"] + ",";
                                            }
                                            State = State.Substring(0, State.Length - 1);
                                            District = District.Substring(0, District.Length - 1);
                                            Tahsil = Tahsil.Substring(0, Tahsil.Length - 1);
                                            Block = Block.Substring(0, Block.Length - 1);
                                            Pincode = Pincode.Substring(0, Pincode.Length - 1);

                                            Session["State"] = State;
                                            Session["District"] = District;
                                            Session["Tahsil"] = Tahsil;
                                            Session["Block"] = Block;
                                            Session["Pincode"] = Pincode;
                                            Session["reporting_id"] = servicedt.Rows[0]["reporting_id"] + "";
                                            Session["reporting_rowid"] = servicedt.Rows[0]["reporting_rowid"] + "";
                                            Session["reporting_name"] = servicedt.Rows[0]["reporting_name"] + "";
                                        }

                                    }
                                    Session["gtokenid"] = tokenid;
                                    //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                    return RedirectToAction("Index", "User");
                                }
                                else
                                {
                                    ViewBag.msg = "You are not a active user";
                                    TempData["token"] = tokenid;
                                }

                            }
                            else if (type == "Customer")
                            {
                                string selquery = "Select * from [tbl_customer] where Customer_id='" + userid + "' and Status='Active'";
                                DataTable seldt = db.GetAllRecord(selquery);
                                if (seldt.Rows.Count > 0)
                                {
                                    //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                    Session["membertype"] = type;
                                    Session["usermail"] = useremail;
                                    Session["customerid"] = userid;
                                    Session["username"] = username;

                                    Session["gtokenid"] = tokenid;
                                    //Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                    //Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                    return RedirectToAction("Dashboard", "Customer");
                                }
                                else
                                {
                                    ViewBag.msg = "You are not a active user";
                                    TempData["token"] = tokenid;
                                }

                            }
                            else
                            {
                                TempData["token"] = tokenid;
                            }
                        }
                        else
                        {
                            TempData["token"] = tokenid;

                        }
                    }
                }
                
            }
            else
            {
                //ViewBag.msg = "Token not found";
            }

            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Applogin(string loginid, string password, string mobile, string htokenid)
        {
            string logid = "";
            string tokenid = null;
            string url = "";
            if (Request.QueryString[0].ToString() != null && Request.QueryString[0].ToString() != "")
            {
                tokenid = Request.QueryString[0].ToString();
                url = "/Home/Applogin?token=" + tokenid;
            }
            else if (TempData["token"] != null && TempData["token"] + "" != "")
            {
                tokenid = TempData["token"] + "";
                url = "/Home/Applogin?token=" + tokenid;
            }
            else if (htokenid != null && htokenid != "")
            {
                tokenid = htokenid;
                url = "/Home/Applogin?token=" + tokenid;
            }
            else
            {

            }
            ViewBag.token = tokenid;
            try
            {
                string query = "select * FROM tbl_login WHERE (Emailid='" + loginid + "' or  Mobile='" + mobile + "') and Password='" + password + "' ";
                //activitylog.Activitylogins("tbl_login", "", query, "Failed", "", loginid, "", loginid);
                DataTable dt = db.GetAllRecord(query);
                //activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";

                    if (tokenid != "" && tokenid != null)
                    {
                        DataTable dataTable = db.GetAllRecord("select Token_id from tbl_login where Id='" + id + "'");
                        if (dataTable.Rows.Count > 0)
                        {
                            if (dataTable.Rows[0]["Token_id"] == null || dataTable.Rows[0]["Token_id"] + "" == "")
                            {
                                db.InsertUpdateDelete("update tbl_login set Token_id='" + tokenid + "' where Id='" + id + "'");
                            }
                            else if (dataTable.Rows[0]["Token_id"] + "" != tokenid)
                            {
                                db.InsertUpdateDelete("update tbl_login set Token_id='" + tokenid + "' where Id='" + id + "'");
                            }

                        }
                        db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Id='" + id + "'");


                        if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "Ouser" || type == "AFuser")
                        {
                            string selquery = "Select * from tbl_registration where Employee_id='" + userid + "' and Status='Approved'";
                            DataTable seldt = db.GetAllRecord(selquery);
                            if (seldt.Rows.Count > 0)
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["membertype"] = type;
                                Session["usermail"] = useremail;
                                Session["userid"] = userid;
                                Session["userphon"] = userphon;
                                Session["username"] = username;

                                Session["emprid"] = seldt.Rows[0]["Id"] + "";
                                Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";

                                Session["Companyid"] = seldt.Rows[0]["Companyid"] + "";
                                Session["Zoneid"] = seldt.Rows[0]["Zoneid"] + "";
                                Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";
                                Session["designation"] = seldt.Rows[0]["Designation"] + "";
                                Session["Premises"] = seldt.Rows[0]["Premises"] + "";

                                string branchquery = "Select * from tbl_branch where Branchid='" + seldt.Rows[0]["Branchcode"] + "'";
                                DataTable branchdt = db.GetAllRecord(branchquery);
                                if (branchdt.Rows.Count > 0)
                                {
                                    Session["Yardid"] = branchdt.Rows[0]["Yardid"] + "";
                                    Session["Yardname"] = branchdt.Rows[0]["Yardname"] + "";
                                    Session["Yardrowid"] = branchdt.Rows[0]["Yardrowid"] + "";
                                }
                                if (type == "Suser")
                                {
                                    string groupquery = "Select * from tbl_group where Employee_id='" + userid + "'";
                                    DataTable groupdt = db.GetAllRecord(groupquery);
                                    if (groupdt.Rows.Count > 0)
                                    {
                                        Session["Companyid"] = groupdt.Rows[0]["Companyid"] + "";
                                        Session["Zoneid"] = groupdt.Rows[0]["Zoneid"] + "";
                                        Session["Regionid"] = groupdt.Rows[0]["Regionid"] + "";
                                        Session["Divisionid"] = groupdt.Rows[0]["Divisionid"] + "";
                                        Session["Branchid"] = groupdt.Rows[0]["Branchid"] + "";
                                        Session["Teamid"] = groupdt.Rows[0]["Teamid"] + "";
                                        Session["Groupname"] = groupdt.Rows[0]["Groups"] + "";
                                        Session["Groupid"] = groupdt.Rows[0]["Groupid"] + "";
                                        Session["CGroupid"] = groupdt.Rows[0]["Groupid"] + "";
                                        Session["CGroupname"] = groupdt.Rows[0]["Groups"] + "";
                                    }

                                }
                                if (type == "Auser")
                                {
                                    string servicequery = "Select * from tbl_servicearea where Employee_id='" + userid + "'";
                                    DataTable servicedt = db.GetAllRecord(servicequery);
                                    if (servicedt.Rows.Count > 0)
                                    {
                                        string State = "", District = "", Tahsil = "", Block = "", Pincode = "", reporting_id = "", reporting_rowid = "", reporting_name = "";
                                        for (int i = 0; i < servicedt.Rows.Count; i++)
                                        {
                                            State += servicedt.Rows[0]["State"] + ",";
                                            District += servicedt.Rows[0]["District"] + ",";
                                            Tahsil += servicedt.Rows[0]["Tahsil"] + ",";
                                            Block += servicedt.Rows[0]["Vill_town"] + ",";
                                            Pincode += servicedt.Rows[0]["Pincode"] + ",";
                                        }
                                        State = State.Substring(0, State.Length - 1);
                                        District = District.Substring(0, District.Length - 1);
                                        Tahsil = Tahsil.Substring(0, Tahsil.Length - 1);
                                        Block = Block.Substring(0, Block.Length - 1);
                                        Pincode = Pincode.Substring(0, Pincode.Length - 1);

                                        Session["State"] = State;
                                        Session["District"] = District;
                                        Session["Tahsil"] = Tahsil;
                                        Session["Block"] = Block;
                                        Session["Pincode"] = Pincode;
                                        Session["reporting_id"] = servicedt.Rows[0]["reporting_id"] + "";
                                        Session["reporting_rowid"] = servicedt.Rows[0]["reporting_rowid"] + "";
                                        Session["reporting_name"] = servicedt.Rows[0]["reporting_name"] + "";
                                    }

                                }
                                Session["gtokenid"] = tokenid;
                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Index", "User");
                            }
                            else
                            {
                                ViewBag.msg = "You are not a active user";
                                TempData["token"] = tokenid;
                            }
                        }
                        else if (type == "Customer")
                        {
                            // return RedirectToAction("Index", "Customer");

                            string selquery = "Select * from [tbl_customer] where Customer_id='" + userid + "' and Status='Active'";
                            DataTable seldt = db.GetAllRecord(selquery);
                            if (seldt.Rows.Count > 0)
                            {
                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                Session["membertype"] = type;
                                Session["usermail"] = useremail;
                                Session["customerid"] = userid;
                                Session["username"] = username;

                                Session["gtokenid"] = tokenid;
                                //Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                //Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                return RedirectToAction("Dashboard", "Customer");
                            }
                            else
                            {
                                ViewBag.msg = "You are not a active user";
                                TempData["token"] = tokenid;
                            }

                        }

                        else
                        {

                            //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "Invalid User Type", userid, username, useremail);
                            ViewBag.msg = "Invalid User Type";
                            TempData["token"] = tokenid;
                        }
                    }
                    else
                    {
                        TempData["token"] = tokenid;
                        //ViewBag.msg = "Invalid Token Id";
                    }



                }
                else
                {
                    TempData["token"] = tokenid;
                    //activitylog.Activitylogins("tbl_login", "", query, "Failed", "Invalid User Id or Password", loginid, "", loginid);
                    ViewBag.msg = "Invalid User Id or Password";
                }
            }

            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;

                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), loginid, "", logid);
                }

                catch
                {

                }
                ViewBag.msg = "Something Went wrong";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult AppForgotPassword(string token)
        {
            ViewBag.token = token;
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult AppForgotPassword(string mailid, string htokenid)
        {
            string tokenid = null;
            string url = "";
            if (Request.QueryString[0].ToString() != null && Request.QueryString[0].ToString() != "")
            {
                tokenid = Request.QueryString[0].ToString();
                url = "/Home/Applogin?token=" + tokenid;
            }
            else if (TempData["token"] != null && TempData["token"] != "")
            {
                tokenid = TempData["token"] + "";
                url = "/Home/Applogin?token=" + tokenid;
            }
            else if (htokenid != null && htokenid != "")
            {
                tokenid = htokenid;
                url = "/Home/Applogin?token=" + tokenid;
            }
            else
            {

            }
            ViewBag.token = tokenid;
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult AppLoginWithOtp(string token)
        {
            ViewBag.token = token;
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult AppLoginWithOtp(string otp, string loginid, string htokenid)
        {
            string tokenid = null;
            string url = "";
            if (Request.QueryString[0].ToString() != null && Request.QueryString[0].ToString() != "")
            {
                tokenid = Request.QueryString[0].ToString();
                url = "/Home/AppLoginWithOtp?token=" + tokenid;
            }
            else if (TempData["token"] != null && TempData["token"] != "")
            {
                tokenid = TempData["token"] + "";
                url = "/Home/AppLoginWithOtp?token=" + tokenid;
            }
            else if (htokenid != null && htokenid != "")
            {
                tokenid = htokenid;
                url = "/Home/AppLoginWithOtp?token=" + tokenid;
            }
            else
            {

            }
            ViewBag.token = tokenid;

            try
            {


                string query = "select * FROM tbl_login WHERE Mobile='" + loginid + "' and OTP='" + otp + "' ";
                activitylog.Activitylogins("tbl_login", "", query, "Failed", "", loginid, "", loginid);
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string otptime = dt.Rows[0]["OTP_Time"] + "";
                    string currenttime = DateTime.Now.ToString("HH:mm:ss");
                    TimeSpan duration = DateTime.Parse(currenttime).Subtract(DateTime.Parse(otptime));
                    // the time span in string format
                    TimeSpan maxtime = TimeSpan.Parse("00:02:00");
                    TimeSpan mintime = TimeSpan.Parse("00:00:00");
                    if (duration <= maxtime && duration > mintime)
                    {

                        if (tokenid != "" && tokenid != null)
                        {
                            DataTable dataTable = db.GetAllRecord("select Token_id from tbl_login where Id='" + id + "'");
                            if (dataTable.Rows.Count > 0)
                            {
                                if (dataTable.Rows[0]["Token_id"] == null || dataTable.Rows[0]["Token_id"] + "" == "")
                                {
                                    db.InsertUpdateDelete("update tbl_login set Token_id='" + tokenid + "' where Id='" + id + "'");
                                }
                                else if (dataTable.Rows[0]["Token_id"] + "" != tokenid)
                                {
                                    db.InsertUpdateDelete("update tbl_login set Token_id='" + tokenid + "' where Id='" + id + "'");
                                }

                            }
                            db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Id='" + id + "'");

                            if (type == "Suser" || type == "Auser" || type == "Iuser" || type == "Tuser" || type == "Ouser" || type == "AFuser")
                            {
                                string selquery = "Select * from tbl_registration where Employee_id='" + userid + "' and Status='Approved'";
                                DataTable seldt = db.GetAllRecord(selquery);
                                if (seldt.Rows.Count > 0)
                                {
                                    //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                    Session["membertype"] = type;
                                    Session["usermail"] = useremail;
                                    Session["userid"] = userid;
                                    Session["userphon"] = userphon;
                                    Session["username"] = username;

                                    Session["emprid"] = seldt.Rows[0]["Id"] + "";
                                    Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                    Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                    Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                    Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";

                                    Session["Companyid"] = seldt.Rows[0]["Companyid"] + "";
                                    Session["Zoneid"] = seldt.Rows[0]["Zoneid"] + "";
                                    Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    Session["ubrname"] = seldt.Rows[0]["Branchname"] + "";
                                    Session["ubrcode"] = seldt.Rows[0]["Branchcode"] + "";
                                    Session["desigorder"] = seldt.Rows[0]["designation_order"] + "";
                                    Session["designation"] = seldt.Rows[0]["Designation"] + "";
                                    Session["Premises"] = seldt.Rows[0]["Premises"] + "";

                                    string branchquery = "Select * from tbl_branch where Branchid='" + seldt.Rows[0]["Branchcode"] + "'";
                                    DataTable branchdt = db.GetAllRecord(branchquery);
                                    if (branchdt.Rows.Count > 0)
                                    {
                                        Session["Yardid"] = branchdt.Rows[0]["Yardid"] + "";
                                        Session["Yardname"] = branchdt.Rows[0]["Yardname"] + "";
                                        Session["Yardrowid"] = branchdt.Rows[0]["Yardrowid"] + "";
                                    }
                                    if (type == "Suser")
                                    {
                                        string groupquery = "Select * from tbl_group where Employee_id='" + userid + "'";
                                        DataTable groupdt = db.GetAllRecord(groupquery);
                                        if (groupdt.Rows.Count > 0)
                                        {
                                            Session["Companyid"] = groupdt.Rows[0]["Companyid"] + "";
                                            Session["Zoneid"] = groupdt.Rows[0]["Zoneid"] + "";
                                            Session["Regionid"] = groupdt.Rows[0]["Regionid"] + "";
                                            Session["Divisionid"] = groupdt.Rows[0]["Divisionid"] + "";
                                            Session["Branchid"] = groupdt.Rows[0]["Branchid"] + "";
                                            Session["Teamid"] = groupdt.Rows[0]["Teamid"] + "";
                                            Session["Groupname"] = groupdt.Rows[0]["Groups"] + "";
                                            Session["Groupid"] = groupdt.Rows[0]["Groupid"] + "";
                                            Session["CGroupid"] = groupdt.Rows[0]["Groupid"] + "";
                                            Session["CGroupname"] = groupdt.Rows[0]["Groups"] + "";
                                        }

                                    }
                                    if (type == "Auser")
                                    {
                                        string servicequery = "Select * from tbl_servicearea where Employee_id='" + userid + "'";
                                        DataTable servicedt = db.GetAllRecord(servicequery);
                                        if (servicedt.Rows.Count > 0)
                                        {
                                            string State = "", District = "", Tahsil = "", Block = "", Pincode = "", reporting_id = "", reporting_rowid = "", reporting_name = "";
                                            for (int i = 0; i < servicedt.Rows.Count; i++)
                                            {
                                                State += servicedt.Rows[0]["State"] + ",";
                                                District += servicedt.Rows[0]["District"] + ",";
                                                Tahsil += servicedt.Rows[0]["Tahsil"] + ",";
                                                Block += servicedt.Rows[0]["Vill_town"] + ",";
                                                Pincode += servicedt.Rows[0]["Pincode"] + ",";
                                            }
                                            State = State.Substring(0, State.Length - 1);
                                            District = District.Substring(0, District.Length - 1);
                                            Tahsil = Tahsil.Substring(0, Tahsil.Length - 1);
                                            Block = Block.Substring(0, Block.Length - 1);
                                            Pincode = Pincode.Substring(0, Pincode.Length - 1);

                                            Session["State"] = State;
                                            Session["District"] = District;
                                            Session["Tahsil"] = Tahsil;
                                            Session["Block"] = Block;
                                            Session["Pincode"] = Pincode;
                                            Session["reporting_id"] = servicedt.Rows[0]["reporting_id"] + "";
                                            Session["reporting_rowid"] = servicedt.Rows[0]["reporting_rowid"] + "";
                                            Session["reporting_name"] = servicedt.Rows[0]["reporting_name"] + "";
                                        }

                                    }
                                    Session["gtokenid"] = tokenid;
                                    //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                    return RedirectToAction("Index", "User");
                                }
                                else
                                {
                                    ViewBag.msg = "You are not a active user";
                                    TempData["token"] = tokenid;
                                }

                            }
                            else if (type == "Customer")
                            {
                                // return RedirectToAction("Index", "Customer");

                                string selquery = "Select * from [tbl_customer] where Customer_id='" + userid + "' and Status='Active'";
                                DataTable seldt = db.GetAllRecord(selquery);
                                if (seldt.Rows.Count > 0)
                                {
                                    //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Success", "Login Success", userid, username, useremail);
                                    Session["membertype"] = type;
                                    Session["usermail"] = useremail;
                                    Session["customerid"] = userid;
                                    Session["username"] = username;

                                    Session["gtokenid"] = tokenid;
                                    //Session["shiftstarttime"] = seldt.Rows[0]["Shiftstarttime"] + "";
                                    //Session["shiftendtime"] = seldt.Rows[0]["Shiftendtime"] + "";
                                    //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 1 + "',Logindatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + userid + "'");
                                    return RedirectToAction("Dashboard", "Customer");
                                }
                                else
                                {
                                    ViewBag.msg = "You are not a active user";
                                    TempData["token"] = tokenid;
                                }

                            }

                            else
                            {

                                //activitylog.Activitylogins("tbl_login", dt.Rows[0]["Id"].ToString(), query, "Failed", "Invalid User Type", userid, username, useremail);
                                ViewBag.msg = "Invalid User Type";
                                TempData["token"] = tokenid;
                            }
                        }
                        else
                        {
                            //ViewBag.msg = "Invalid Token Id";
                            TempData["token"] = tokenid;
                        }

                        
                    }
                    else
                    {
                        ViewBag.msg = "Otp Has been Expired! Resend Otp.";
                        TempData["token"] = tokenid;
                    }
                }
                else
                {
                    activitylog.Activitylogins("tbl_login", "", query, "Failed", "Invalid OTP", loginid, "", loginid);
                    ViewBag.msg = "Invalid OTP";
                    TempData["token"] = tokenid;
                }

            }
           catch(Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;

                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), loginid, "", loginid);
                }

                catch
                {

                }
                ViewBag.msg = "Something Went wrong";
                TempData["token"] = tokenid;
            }
            return View();
        }
        public JsonResult AppOTPSend(string Mob)
        {
            string msg = "", mail = "";
            try
            {
                string query = "select * FROM tbl_login WHERE Mobile='" + Mob + "' and Type!='Sadmin' and Type!='Admin' ";
                activitylog.Activitylogins("tbl_login", "", query, "Failed", "", Mob, "", Mob);
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string rid = dt.Rows[0]["Id"] + "";
                    string type = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string name = dt.Rows[0]["Username"] + "";
                    string mobile = dt.Rows[0]["Mobile"] + "";
                    string otptime = dt.Rows[0]["OTP_Time"] + "";
                    string username = dt.Rows[0]["Username"] + "";
                    string userid = dt.Rows[0]["Userid"] + "";
                    string userphon = dt.Rows[0]["Mobile"] + "";
                    string useremail = dt.Rows[0]["Emailid"] + "";
                    string currenttime = DateTime.Now.ToString("HH:mm:ss");
                    TimeSpan duration = DateTime.Parse(currenttime).Subtract(DateTime.Parse(otptime));
                    // the time span in string format
                    DateTime getotptime = DateTime.Parse(otptime).AddMinutes(1);
                    DateTime newDateTime = DateTime.Parse(otptime).AddMinutes(1);
                    TimeSpan maxtime = TimeSpan.Parse("00:01:00");
                    TimeSpan mintime = TimeSpan.Parse("00:00:00");

                    if (duration > maxtime || duration < mintime)
                    {
                        if (status == "Active")
                        {
                            string otp = random().ToString();


                            string query1 = "Update tbl_login set OTP='" + otp + "', OTP_Time='" + DateTime.Now.ToString("HH:mm:ss") + "' where Mobile='" + Mob + "' ";
                            if (db.InsertUpdateDelete(query1))
                            {
                                string[] replacementValues = { otp };
                                Messaging.SendSMSNew(Mob, replacementValues, "OTP For Login", name, userid, username);
                                Messaging.SendWhatsappSMSNew1(Mob, "OTP Login", name, userid, name, Session["emprid"] + "", "", "", replacementValues, true);
                                activitylog.Activitylogins("tbl_login", rid, query1, "Success", "OTP Sent on Your Number", userid, username, useremail);
                                msg = "OTP Sent on Your Number";
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_login", rid, query1, "Failed", "Otp Sending failed", userid, username, useremail);
                                msg = "OTP Sending Failed";
                            }
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_login", rid, query, "Success", "You are not a active user", userid, username, useremail);
                            msg = "You are not a active user";
                        }
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_login", rid, query, "Failed", "You can get otp after 10 mintuts", userid, username, useremail);
                        msg = "You can get otp after " + getotptime.ToString("hh:mm") + " o'clock";
                    }

                }
                else
                {
                    activitylog.Activitylogins("tbl_login", "", query, "Failed", "We can not find a user with that Mobile number.", Mob, "", Mob);
                    msg = "We can not find a user with that Mobile number.";
                }

            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    if (mail == "")
                    {
                        mail = Mob;
                    }
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), mail, "", mail);
                }

                catch
                {

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Register(FormCollection form)
        {
            try
            {
                string fileName = ""; string str = "", str2 = ""; string fpth = ""; string qrpth = "";
                GenQRCode qr = new GenQRCode();

                string hid = form["hid"];

                if (hid != null && hid != "")
                {

                }
                else
                {
                    string customerid = "C_" + random();
                    string query1 = "SELECT * FROM tbl_customer WHERE (Customer_id='" + customerid + "' OR Mobile='" + form["mobile"] + "' OR Email='" + form["email"] + "')";
                    activitylog.Activitylogins("tbl_customer", "", query1, "Failed", "", "", "", "");
                    DataTable dt = db.GetAllRecord(query1);
                    activitylog.Activitylogupd("Success", "");
                    string query2 = "SELECT * FROM tbl_login WHERE Emailid='" + form["mail"] + "' OR Mobile='" + form["mobile"] + "'";
                    activitylog.Activitylogins("tbl_login", "", query2, "Failed", "", "", "", "");
                    DataTable dt2 = db.GetAllRecord(query2);
                    activitylog.Activitylogupd("Success", "");
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            str += dt.Rows[i]["Email"] + ",";
                            str += dt.Rows[i]["Customer_id"] + ",";
                            str += dt.Rows[i]["Mobile"] + ",";
                        }
                        string[] strArray = str.Split(',');

                        if (strArray.Contains(form["mail"]) && strArray.Contains(form["mobile"]) && strArray.Contains(customerid))
                        {
                            ViewBag.msg = "Customer id, Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["mobile"]) && strArray.Contains(customerid))
                        {
                            ViewBag.msg = "Customer id, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["mail"]) && strArray.Contains(customerid))
                        {
                            ViewBag.msg = "Customer id, Email Already Exist";
                        }
                        else if (strArray.Contains(form["mail"]) && strArray.Contains(form["mobile"]))
                        {
                            ViewBag.msg = "Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(customerid))
                        {
                            ViewBag.msg = "Customer id Already Exist";
                        }
                        else if (strArray.Contains(form["mobile"]))
                        {
                            ViewBag.msg = "Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["mail"]))
                        {
                            ViewBag.msg = "Email Already Exist";
                        }
                    }
                    else if (dt2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            str2 += dt2.Rows[i]["Emailid"] + ",";
                            str2 += dt2.Rows[i]["Mobile"] + ",";
                        }
                        string[] strArray = str.Split(',');

                        if (strArray.Contains(form["mail"]) && strArray.Contains(form["mobile"]))
                        {
                            ViewBag.msg = "Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["mobile"]))
                        {
                            ViewBag.msg = "Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["mail"]))
                        {
                            ViewBag.msg = "Email Already Exist";
                        }
                    }
                    else
                    {
                        string pass = random();
                        string bodytext = "Welcome To 'GROWFAST ORGANIC DIMOND', dear " + form["name"] + ", your Registration has been Successfully.Your Loginid is : Email = " + form["email"] + " or Mobile : " + form["mobile"] + " and Password is : " + pass + ". To Login This Portal <a href='http://growfastgroups.com/'> Click Here </a>";

                        string query = "insert into tbl_customer(Customer_id ,Name ,Mobile ,Email ,Password ,Address ,State ,City ,Postal_code ,Profile_pic ,Status,Datetime,Log_ID ,Log_name ,Log_Email) values('" + customerid + "','" + form["name"] + "','" + form["mobile"] + "','" + form["email"] + "','" + pass + "','" + form["address"] + "','" + form["state"] + "','" + form["city"] + "','" + form["postal_code"] + "','pic','Active','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','', '', '')";

                        string lquery = "insert into tbl_login(Username,Userid,Emailid,Mobile,Password,Type,Status,Datetime,OTP_Time) values('" + form["name"] + "','" + customerid + "','" + form["email"] + "','" + form["mobile"] + "','" + pass + "','customer','Active','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("HH:mm:ss") + "')";

                        if (db.InsertUpdateDelete(query) && db.InsertUpdateDelete(lquery))
                        {
                            activitylog.Activitylogins("tbl_customer", db.getmaxid("tbl_customer").ToString(), query, "Success", "Insert Succcess", customerid, form["mobile"], form["mail"]);
                            activitylog.Activitylogins("tbl_login", db.getmaxid("tbl_login").ToString(), lquery, "Success", "Insert Succcess", customerid, form["mobile"] ,form["mail"]);

                            //sendmail.SendMailEmployee(acode, form["mail"], "Registration Successfully", bodytext);

                            ViewBag.AlertMessage = customerid + " Registration Success";
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_customer", "", query, "Failed", "Insert Failed", customerid, form["mobile"] ,form["mail"]);
                            activitylog.Activitylogins("tbl_login", "", lquery, "Failed", "Insert Failed", customerid, form["mobile"], form["mail"]);
                            ViewBag.AlertMessage = customerid + " Registration Failed";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["userid"].ToString(), form["mobile"] ,form["mail"]);
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View();
        }
        public ActionResult Privacy()
        {
            return View();
        }
        public ActionResult Terms()
        {
            return View();
        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Inauguration1()
        {
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Inauguration2()
        {
            return View();
        }
        private string random(int numlength)
        {
            var random = new Random();
            var number = string.Empty;
            var possibleChar = "1234567890";

            for (var i = 0; i < numlength; i++)
            {
                var randomIndex = random.Next(0, possibleChar.Length - 1);
                number += possibleChar[randomIndex];
            }
            return number;
        }
        private string arandom(int numlength)
        {
            var random = new Random();
            var number = string.Empty;
            var possibleChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            for (var i = 0; i < numlength; i++)
            {
                var randomIndex = random.Next(0, possibleChar.Length - 1);
                number += possibleChar[randomIndex];
            }
            return number;
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ChangeEmpcode1()
        {
            return View();
        }
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult ChangeEmpcode1(string oldempcode,string newempcode)
        {
            try
            {
                db.InsertUpdateDelete("update tbl_attendance set Employeeid = '" + newempcode + "' where Employeeid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_company] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_division] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_branch1] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_group] set Employee_id = '" + newempcode + "' where Employee_id = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_cart_details] set logid = '" + newempcode + "' where logid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_customer] set Log_ID = '" + newempcode + "' where Log_ID = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_leadnew] set Logid = '" + newempcode + "' where Logid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_leadnew] set Supportempid = '" + newempcode + "' where Supportempid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_leave] set LogId = '" + newempcode + "' where LogId = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_leave] set Employeeid = '" + newempcode + "' where Employeeid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_login] set Userid = '" + newempcode + "' where Userid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_notification] set Employeeid = '" + newempcode + "' where Employeeid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_order] set logid = '" + newempcode + "' where logid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_order] set Actionbyid = '" + newempcode + "' where Actionbyid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_order_summary] set logid = '" + newempcode + "' where logid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_order_transaction] set Transaction_by_id = '" + newempcode + "' where Transaction_by_id = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_order_transaction] set logid = '" + newempcode + "' where logid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_region] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_registration] set Employee_id = '" + newempcode + "' where Employee_id = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_servicearea] set Employee_id = '" + newempcode + "' where Employee_id = '" + oldempcode + "'");
                db.InsertUpdateDelete(" update[tbl_servicearea] set reporting_id = '" + newempcode + "' where reporting_id = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_team] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                db.InsertUpdateDelete("update[tbl_zone] set Uniquid = '" + newempcode + "' where Uniquid = '" + oldempcode + "'");
                ViewBag.msg = "Updated";
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), "", "", "");
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View();
        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult salerentry()
        {
            string id = Session["suid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult GetCustomer(string Mobile)
        {
            string msg = "";
            try
            {
                string query1 = "SELECT * FROM tbl_customer WHERE Mobile='" + Mobile + "'";
                
                DataTable dt = db.GetAllRecord(query1);
               
                string query2 = "SELECT * FROM tbl_login WHERE Mobile='" + Mobile + "'";
                
                DataTable dt2 = db.GetAllRecord(query2);

                if (dt.Rows.Count > 0)
                {
                    string queryl = "SELECT Top (10) Leadid FROM tbl_leadnew where Customerid='" + dt.Rows[0]["Customer_id"] + "' and Leadid is not null";
                    DataTable dtl = db.GetAllRecord(queryl);
                    if (dtl.Rows.Count > 0)
                    {
                        Session["Leadid"] = dtl.Rows[0]["Leadid"] + "";
                        Session["Customer_id"] = dt.Rows[0]["Customer_id"] + "";
                        msg = JsonConvert.SerializeObject(dt, Formatting.None);
                    }
                }
                else if (dt2.Rows.Count > 0)
                {
                    msg = "Mobile Already Exist";
                }
                else
                {
                    msg = "";
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                db.connectionstate();
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProduct(string ptype)
        {
            string msg = "";
            try
            {
                string query1 = "select[Id],[Brand],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status],[Datetime],[logid],[logname], packaging_material, CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice], CAST([Quantity] AS DECIMAL(18, 2)) AS[Quantity],punit from[tbl_pro_price_circlar] where Status = 'Active' and PType = '" + ptype + "'  order by Id desc";

                DataTable dt = db.GetAllRecord(query1);

                dt.Columns.Add("incart", typeof(string));

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // Construct the query to fetch cart details for the current product
                        string jkshd = "select * from tbl_cart_details where Customer_id='" + Session["Customer_id"] + "' and logid='" + Session["useridh"] + "' and Status='Add In Cart' and ProductId='" + row["Id"] + "'";
                        DataTable crdt = db.GetAllRecord(jkshd);
                        
                        // Check if the cart details exist for the current product
                        if (crdt.Rows.Count > 0)
                        {
                            // Update the "incart" column for the current row
                            row["incart"] = "In Cart";
                        }
                        else
                        {
                            // Update the "incart" column for the current row
                            row["incart"] = "Not In Cart";
                        }
                    }
                    
                    msg = JsonConvert.SerializeObject(dt, Formatting.None);

                }
                else
                {
                    msg = "";
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                db.connectionstate();
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployee(string Hid)
        {
            string json = null;
            string query = "select * from tbl_registration where Employee_id='" + Hid + "'";
            
            DataTable dt = db.GetAllRecord(query);
            if (dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(dt, formatting: Formatting.None);

                Session["membertypeh"] = dt.Rows[0]["Employee_Type"] + "";
                Session["usermailh"] = dt.Rows[0]["Email"] + "";
                Session["useridh"] = dt.Rows[0]["Employee_id"] + "";
                Session["userphonh"] = dt.Rows[0]["Mobile_no"] + "";
                Session["usernameh"] = dt.Rows[0]["Name"] + "";

                Session["empridh"] = dt.Rows[0]["Id"] + "";
                Session["shiftstarttimeh"] = dt.Rows[0]["Shiftstarttime"] + "";
                Session["shiftendtimeh"] = dt.Rows[0]["Shiftendtime"] + "";
                Session["ubrnameh"] = dt.Rows[0]["Branchname"] + "";
                Session["ubrcodeh"] = dt.Rows[0]["Branchcode"] + "";
                Session["desigorderh"] = dt.Rows[0]["designation_order"] + "";

                Session["Companyidh"] = dt.Rows[0]["Companyid"] + "";
                Session["Zoneidh"] = dt.Rows[0]["Zoneid"] + "";
                Session["shiftendtimeh"] = dt.Rows[0]["Shiftendtime"] + "";
                Session["ubrnameh"] = dt.Rows[0]["Branchname"] + "";
                Session["ubrcodeh"] = dt.Rows[0]["Branchcode"] + "";
                Session["desigorderh"] = dt.Rows[0]["designation_order"] + "";
                Session["designationh"] = dt.Rows[0]["Designation"] + "";
                Session["Premisesh"] = dt.Rows[0]["Premises"] + "";

                string branchquery = "Select * from tbl_branch where Branchid='" + dt.Rows[0]["Branchcode"] + "'";
                DataTable branchdt = db.GetAllRecord(branchquery);
                if (branchdt.Rows.Count > 0)
                {
                    Session["Yardidh"] = branchdt.Rows[0]["Yardid"] + "";
                    Session["Yardnameh"] = branchdt.Rows[0]["Yardname"] + "";
                    Session["Yardrowidh"] = branchdt.Rows[0]["Yardrowid"] + "";
                }
                if (Session["membertypeh"]+"" == "Suser")
                {
                    string groupquery = "Select * from tbl_group where Employee_id='" + Session["useridh"] + "' and Status='Active'";
                    DataTable groupdt = db.GetAllRecord(groupquery);
                    if (groupdt.Rows.Count > 0)
                    {
                        Session["Companyidh"] = groupdt.Rows[0]["Companyid"] + "";
                        Session["Zoneidh"] = groupdt.Rows[0]["Zoneid"] + "";
                        Session["Regionidh"] = groupdt.Rows[0]["Regionid"] + "";
                        Session["Divisionidh"] = groupdt.Rows[0]["Divisionid"] + "";
                        Session["Branchidh"] = groupdt.Rows[0]["Branchid"] + "";
                        Session["Teamidh"] = groupdt.Rows[0]["Teamid"] + "";
                        Session["Groupnameh"] = groupdt.Rows[0]["Groups"] + "";
                        Session["Groupidh"] = groupdt.Rows[0]["Groupid"] + "";
                        Session["CGroupidh"] = groupdt.Rows[0]["Groupid"] + "";
                        Session["CGroupnameh"] = groupdt.Rows[0]["Groups"] + "";

                        string groupqueryh = "Select * from tbl_group where Groupid='" + groupdt.Rows[0]["Groupid"] + "' and Member_type='Head' and Status='Active'";
                        DataTable groupdth = db.GetAllRecord(groupqueryh);

                        string queryr = "select * from tbl_registration where Id='" + groupdth.Rows[0]["Employee_rowid"] + "'";

                        DataTable dtr = db.GetAllRecord(queryr);
                        if (dtr.Rows.Count > 0)
                        {
                            Session["usermailg"] = dt.Rows[0]["Email"] + "";
                            Session["useridg"] = dt.Rows[0]["Employee_id"] + "";
                            Session["usernameg"] = dt.Rows[0]["Name"] + "";

                            Session["empridg"] = dt.Rows[0]["Id"] + "";
                        }
                        else
                        {

                        }
                    }
                }




            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicle(string Type)
        {
            string res = "<option selected value=''>Select one</option>";
            string getcmp = "SELECT * FROM tbl_vehicle where Vehicletype='" + Type + "'";
            DataTable tbll = db.GetAllRecord(getcmp);
            if (tbll.Rows.Count > 0)
            {
                for (int i = 0; i < tbll.Rows.Count; i++)
                {
                    res += "<option value='" + tbll.Rows[i]["Vehicleid"] + "'>" + tbll.Rows[i]["Vehicle_name"] + " (" + tbll.Rows[i]["Registration_no"] + ") - " + tbll.Rows[i]["Loading_capicity"] + "</option>";

                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeec(string Hid)
        {
            string json = null;
            string query = "select * from tbl_registration where Employee_id='" + Hid + "'";

            DataTable dt = db.GetAllRecord(query);
            if (dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(dt, formatting: Formatting.None);

                Session["membertypec"] = dt.Rows[0]["Employee_Type"] + "";
                Session["usermailc"] = dt.Rows[0]["Email"] + "";
                Session["useridc"] = dt.Rows[0]["Employee_id"] + "";
                Session["userphonc"] = dt.Rows[0]["Mobile_no"] + "";
                Session["usernamec"] = dt.Rows[0]["Name"] + "";

                Session["empridc"] = dt.Rows[0]["Id"] + "";
                Session["shiftstarttimec"] = dt.Rows[0]["Shiftstarttime"] + "";
                Session["shiftendtimec"] = dt.Rows[0]["Shiftendtime"] + "";
                Session["ubrnamec"] = dt.Rows[0]["Branchname"] + "";
                Session["ubrcodec"] = dt.Rows[0]["Branchcode"] + "";
                Session["desigorderc"] = dt.Rows[0]["designation_order"] + "";

                Session["Companyidc"] = dt.Rows[0]["Companyid"] + "";
                Session["Zoneidc"] = dt.Rows[0]["Zoneid"] + "";
                Session["shiftendtimec"] = dt.Rows[0]["Shiftendtime"] + "";
                Session["ubrnamec"] = dt.Rows[0]["Branchname"] + "";
                Session["ubrcodec"] = dt.Rows[0]["Branchcode"] + "";
                Session["desigorderc"] = dt.Rows[0]["designation_order"] + "";
                Session["designationc"] = dt.Rows[0]["Designation"] + "";
                Session["Premisesc"] = dt.Rows[0]["Premises"] + "";

                string branchquery = "Select * from tbl_branch where Branchid='" + dt.Rows[0]["Branchcode"] + "'";
                DataTable branchdt = db.GetAllRecord(branchquery);
                if (branchdt.Rows.Count > 0)
                {
                    Session["Yardidc"] = branchdt.Rows[0]["Yardid"] + "";
                    Session["Yardnamec"] = branchdt.Rows[0]["Yardname"] + "";
                    Session["Yardrowidc"] = branchdt.Rows[0]["Yardrowid"] + "";
                }
                if (Session["membertypec"] + "" == "Suser")
                {
                    string groupquery = "Select * from tbl_group where Employee_id='" + Session["useridc"] + "' and Status='Active'";
                    DataTable groupdt = db.GetAllRecord(groupquery);
                    if (groupdt.Rows.Count > 0)
                    {
                        Session["Companyidc"] = groupdt.Rows[0]["Companyid"] + "";
                        Session["Zoneidc"] = groupdt.Rows[0]["Zoneid"] + "";
                        Session["Regionidc"] = groupdt.Rows[0]["Regionid"] + "";
                        Session["Divisionidc"] = groupdt.Rows[0]["Divisionid"] + "";
                        Session["Branchidc"] = groupdt.Rows[0]["Branchid"] + "";
                        Session["Teamidc"] = groupdt.Rows[0]["Teamid"] + "";
                        Session["Groupnamec"] = groupdt.Rows[0]["Groups"] + "";
                        Session["Groupidc"] = groupdt.Rows[0]["Groupid"] + "";
                        Session["CGroupidc"] = groupdt.Rows[0]["Groupid"] + "";
                        Session["CGroupnamec"] = groupdt.Rows[0]["Groups"] + "";
                    }

                }




            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeed(string Hid)
        {
            string json = null;
            string query = "select * from tbl_registration where Employee_id='" + Hid + "'";
            
            DataTable dt = db.GetAllRecord(query);
            
            if (dt.Rows.Count > 0)
            {
                json = JsonConvert.SerializeObject(dt, formatting: Formatting.None);
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult onchange(string Ptype)
        {
            string json = "";
            try
            {
                string query = "select DISTINCT proid,Brand from tbl_pro_price_circlar WHERE Status = 'Active' AND ptype = '" + Ptype + "'ORDER BY proid ASC";/* "select * from tbl_pro_price_circlar where ptype='" + Ptype + "'";*/
                DataTable dt = db.GetAllRecord(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(dt, Formatting.None);
                }
                else
                {
                    ViewBag.msg = "No data available for " + Ptype + ".";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProducFullDetails(string Productid)
        {
            string res = "";
            try
            {
                string query = "select * from tbl_pro_price_circlar where proid='" + Productid + "'";
                
                DataTable dt = db.GetAllRecord(query);
                
                res = JsonConvert.SerializeObject(dt, Formatting.None);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        private string checkrandom(string orderid22, string order2)
        {
            if (order2 == orderid22)
            {
                string orderid2 = arandom(6);
                checkrandom(orderid2, order2);
            }
            else
            {

            }
            return orderid22;
        }



















        //Dknocks
        public JsonResult Dknocks()
        {
            string res = "";
            var loc = Request.Form["loc"];
            var latitude = Request.Form["latitude"];
            var longitude = Request.Form["longitude"];
            var support = Request.Form["support"];
            var mobile = Request.Form["mobile"];
            var name = Request.Form["name"];
            var address = Request.Form["address"];
            var tahsil = Request.Form["tahsil"];
            var block = Request.Form["block"];
            var Cityname = Request.Form["Cityname"];
            var Statename = Request.Form["Statename"];
            var pincode = Request.Form["pincode"];
            var email = Request.Form["email"];
            var irrigation = Request.Form["irrigation"];
            var occupation = Request.Form["occupation"];
            var stopwatchinp = Request.Form["stopwatchinp"];

            try
            {
                // Create a JObject to store data
                JObject jsonObject = new JObject();
                jsonObject["Location"] = loc;
                jsonObject["Latitude"] = latitude;
                jsonObject["Longitude"] = longitude;

                // Convert JObject to JSON string
                string jsonlocation = jsonObject.ToString();

                string query = "";
                string hid = support;
                string query1 = "SELECT * FROM tbl_customer AS c LEFT JOIN tbl_login AS l ON c.Mobile = l.Mobile WHERE c.Mobile = '" + mobile + "'";
                activitylog.Activitylogins("tbl_customer,tbl_login", "", query1, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                DataTable dt = db.GetAllRecord(query1);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    ViewBag.AlertMessage = "Dk's Done Of this Customer You Redirected to demo process.";
                }
                else
                {

                    string nextstatus = "";
                    if (irrigation == "Agriculture")
                    {
                        nextstatus = "Open";
                    }
                    else
                    {
                        nextstatus = "Close";
                    }
                    DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                    // Extract the year and month from the current date
                    string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                    string customerid = "C" + currentYearmonth + "" + arandom(5);
                    string leadid = "L" + currentYearmonth + "" + arandom(5);
                    string addressf = address + "," + tahsil + "," + block + "," + Cityname + "," + Statename + "," + pincode;
                    if (hid == "Yes")
                    {

                        query = "Update [tbl_leadnew] Set [Customerid]='" + customerid + "' ,[Name]='" + name + "' ,[Mobile]='" + mobile + "' ,[Email]='" + email + "' ,[Statename]='" + Statename + "' ,[Cityname]='" + Cityname + "' ,[Address]='" + addressf + "' ,[Pincode]='" + pincode + "' ,[Occupation]='" + occupation + "' ,[Land_Irrigation]='" + irrigation + "' ,[Taken_time]='" + stopwatchinp + "' ,[Lead_status]='Done',Nextstatus='" + nextstatus + "',[Status]='Active' ,[BranchName]='" + Session["ubrnameh"] + "' ,[BranchCode]='" + Session["ubrcodeh"] + "' ,[Logid]='" + Session["useridh"] + "' ,[Logname]='" + Session["usernameh"] + "' ,[releavedatetime]='" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "' where id='" + Session["Rowid"] + "'";

                    }
                    else
                    {

                        query = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus) VALUES('" + leadid + "','" + customerid + "','" + name + "','" + mobile + "','" + email + "','" + Statename + "','" + Cityname + "','" + addressf + "','" + pincode + "','" + occupation + "','" + irrigation + "','Close','XML','" + support + "','" + jsonlocation + "','','','','','','" + stopwatchinp + "','DKS','Done','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "')";

                    }

                    string pass = random(5);
                    string bodytext = "Dear " + name + ", your Registration has been Successfully.Your Loginid is : " + mobile + " and Password is : " + pass + ". To Login This Portal <a href='https://growfastgroups.com/'> Click Here </a>\n GROWFAST";


                    string cquery = "insert into tbl_customer(Customer_id ,Name ,Mobile ,Email ,Password ,Full_address,Address ,State ,City ,Postal_code ,Tahsil,Block ,Profile_pic ,Status,Datetime,Log_ID ,Log_name ,Log_Email,BranchName,BranchCode,RegistrationType,Groupname,Groupid) values('" + customerid + "','" + name + "','" + mobile + "','" + email + "','" + pass + "','" + addressf + "','" + address + "','" + Statename + "','" + Cityname + "','" + pincode + "','" + tahsil + "','" + block + "','pic','Lead','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["useridh"].ToString() + "', '" + Session["usernameh"].ToString() + "', '" + Session["usermailh"].ToString() + "','" + Session["ubrnameh"].ToString() + "', '" + Session["ubrcode"].ToString() + "','Lead','" + Session["Groupnameh"] + "','" + Session["Groupidh"] + "')";

                    string lquery = "insert into tbl_login(Username,Userid,Emailid,Mobile,Password,Type,Status,Datetime,OTP_Time,Branchname,Branchcode) values('" + name + "','" + customerid + "','" + email + "','" + mobile + "','" + pass + "','Customer','Active','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("HH:mm:ss") + "','" + Session["ubrnameh"].ToString() + "', '" + Session["ubrcodeh"].ToString() + "')";

                    if (db.InsertUpdateDelete(query) && db.InsertUpdateDelete(cquery) && db.InsertUpdateDelete(lquery))
                    {
                        res = leadid;
                        activitylog.Activitylogins("tbl_lead", db.getmaxid("tbl_lead").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_customer", db.getmaxid("tbl_customer").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_login", db.getmaxid("tbl_login").ToString(), lquery, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                        if (email != "")
                        {
                            //Messaging.SendMailEmployee("", form["email"], "Registration Successfully", bodytext);
                        }
                        string[] replacementValues = { "GROWFAST ORGANIC DIMOND" };
                        //Messaging.SendWhatsappSMSNew1(mobile, "OTP Login", Session["username"] + "", Session["userid"] + "", Session["username"] + "", Session["emprid"] + "", "", "", replacementValues);


                        if (irrigation == "Agriculture")
                        {
                            ViewBag.AlertMessage = "Success";
                            //Response.Redirect("/User/BSA?customerid=" + customerid + "&leadid=" + leadid);
                        }


                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_customer", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_login", "", lquery, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.AlertMessage = customerid + " Registration Failed";
                        activitylog.Activitylogins("tbl_lead", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    }
                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }


            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //BSA
        public JsonResult BSA()
        {
            string res = "";
            var loc = Request.Form["loc"];
            var latitude = Request.Form["latitude"];
            var longitude = Request.Form["longitude"];
            var support = Request.Form["support"];
            var crfrd = Request.Form["crfrd"];
            var irrigationc = Request.Form["irrigationc"];
            var irrieq = Request.Form["irrieq"];
            var cropnm = Request.Form["cropnm"];
            var fertilizernm = Request.Form["fertilizernm"];
            var interested = Request.Form["interested"];
            var problems = Request.Form["problems"];
            var commercial = Request.Form["commercial"];
            var profitable = Request.Form["profitable"];
            
            var stopwatchinp = Request.Form["stopwatchinp"];

            var customerid = Request.Form["Customer_id"];
            var leadid = Request.Form["leadid"];
            var date = Request.Form["date"];
            try
            {
                // Create a JObject to store data
                JObject jsonObject = new JObject();
                jsonObject["Location"] = loc;
                jsonObject["Latitude"] = latitude;
                jsonObject["Longitude"] = longitude;

                // Convert JObject to JSON string
                string jsonlocation = jsonObject.ToString();

                string query = "";
                string hid = support;
                string nextstatus = "";
                string Status = "";
                JObject jsonObject1 = new JObject();
                if (crfrd == "Know")
                {
                    jsonObject1["Crop_ferti_details"] = crfrd;
                    jsonObject1["Land_arc"] = irrigationc;
                    jsonObject1["Irrigation_Eqp"] = irrieq;
                    jsonObject1["Cropdetail"] = cropnm;
                    jsonObject1["Fertilizerdetail"] = fertilizernm;
                    jsonObject1["Nextmeetdate"] = "";
                    jsonObject1["Nextmeettime"] = "";

                    jsonObject1["Intrestedarea"] = interested;
                    jsonObject1["Facing_Problems"] = problems;
                    jsonObject1["Commercial_planting"] = commercial;
                    jsonObject1["Profitable"] = profitable;

                    nextstatus = "Open";
                    Status = "Done";
                }
                else
                {
                    jsonObject1["Crop_ferti_details"] = crfrd;
                    jsonObject1["OPersontname"] = "";
                    jsonObject1["OPersontcontact"] = "";
                    jsonObject1["Nextmeetdate"] ="";
                    jsonObject1["Nextmeettime"] = "";
                    nextstatus = "Open";
                    Status = "Follow Up";
                    
                }

                // Convert JObject to JSON string
                string jsondata1 = jsonObject1.ToString();

                string query1 = "SELECT * FROM tbl_customer  WHERE Customer_id = '" + customerid + "'";
                activitylog.Activitylogins("tbl_customer", "", query1, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                DataTable dt = db.GetAllRecord(query1);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    if (hid == "Yes")
                    {
                        query = "Update [tbl_leadnew] Set [Customerid]='" + customerid + "' ,[Name]='" + dt.Rows[0]["Name"] + "' ,[Mobile]='" + dt.Rows[0]["Mobile"] + "' ,[Email]='" + dt.Rows[0]["Email"] + "' ,[Statename]='" + dt.Rows[0]["State"] + "' ,[Cityname]='" + dt.Rows[0]["City"] + "' ,[Address]='" + dt.Rows[0]["Full_address"] + "' ,[Pincode]='" + dt.Rows[0]["Postal_code"] + "' ,[Occupation]='' ,[Land_Irrigation]='' ,[Taken_time]='" + stopwatchinp + "' ,[Lead_status]='Done',Nextstatus='" + nextstatus + "',[Status]='Active' ,[BranchName]='" + Session["ubrnameh"] + "' ,[BranchCode]='" + Session["ubrcodeh"] + "' ,[Logid]='" + Session["useridh"] + "' ,[Logname]='" + Session["usernameh"] + "' ,[releavedatetime]='" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "',XML='" + jsondata1 + "',Crop_ferti_details='" + crfrd + "' where id='" + Session["Rowid"] + "'";

                    }
                    else
                    {
                        query = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,Crop_ferti_details) VALUES('" + leadid + "','" + customerid + "','" + dt.Rows[0]["Name"] + "','" + dt.Rows[0]["Mobile"] + "','" + dt.Rows[0]["Email"] + "','" + dt.Rows[0]["State"] + "','" + dt.Rows[0]["City"] + "','" + dt.Rows[0]["Full_address"] + "','" + dt.Rows[0]["Postal_code"] + "','','','Close','" + jsondata1 + "','" + support + "','" + jsonlocation.Replace("'", "") + "','','','','','','" + stopwatchinp + "','BSA','" + Status + "','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "','" + crfrd + "')";

                    }

                    if (db.InsertUpdateDelete(query))
                    {
                        res = leadid;
                        activitylog.Activitylogins("tbl_leadnew", db.getmaxid("tbl_leadnew").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                        if (crfrd == "Know")
                        {
                            ViewBag.AlertMessage = "Success";
                            //Response.Redirect("/User/Demo?customerid=" + customerid + "&leadid=" + leadid);
                        }
                        else
                        {
                            ViewBag.AlertMessage = "Success";
                            //Response.Redirect("/User/VDknocks");
                        }


                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_leadnew", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    }
                }
                else
                {

                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //DknocksBSA
        public JsonResult DknocksBSA()
        {
            string res = "";
            var loc = Request.Form["loc"];
            var latitude = Request.Form["latitude"];
            var longitude = Request.Form["longitude"];
            var support = Request.Form["support"];
            var mobile = Request.Form["mobile"];
            var name = Request.Form["name"];
            var address = Request.Form["address"];
            var tahsil = Request.Form["tahsil"];
            var block = Request.Form["block"];
            var Cityname = Request.Form["Cityname"];
            var Statename = Request.Form["Statename"];
            var pincode = Request.Form["pincode"];
            var email = Request.Form["email"];
            var irrigation = Request.Form["irrigation"];
            var occupation = Request.Form["occupation"];
            var stopwatchinp = Request.Form["stopwatchinp"];

            var crfrd = Request.Form["crfrd"];
            var irrigationc = Request.Form["irrigationc"];
            var irrieq = Request.Form["irrieq"];
            var cropnm = Request.Form["cropnm"];
            var fertilizernm = Request.Form["fertilizernm"];
            var interested = Request.Form["interested"];
            var problems = Request.Form["problems"];
            var commercial = Request.Form["commercial"];
            var profitable = Request.Form["profitable"];

            try
            {
                // Create a JObject to store data
                JObject jsonObject = new JObject();
                jsonObject["Location"] = loc;
                jsonObject["Latitude"] = latitude;
                jsonObject["Longitude"] = longitude;

                // Convert JObject to JSON string
                string jsonlocation = jsonObject.ToString();

                string query = "";
                string hid = support;
                string query1 = "SELECT * FROM tbl_customer AS c LEFT JOIN tbl_login AS l ON c.Mobile = l.Mobile WHERE c.Mobile = '" + mobile + "'";
                activitylog.Activitylogins("tbl_customer,tbl_login", "", query1, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                DataTable dt = db.GetAllRecord(query1);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    res = "Dk's Done Of this Customer.";
                }
                else
                {

                    string nextstatus = "";
                    if (irrigation == "Agriculture")
                    {
                        nextstatus = "Open";
                    }
                    else
                    {
                        nextstatus = "Close";
                    }
                    DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                    // Extract the year and month from the current date
                    string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                    string customerid = "C" + currentYearmonth + "" + arandom(5);
                    string leadid = "L" + currentYearmonth + "" + arandom(5);
                    string addressf = address + "," + tahsil + "," + block + "," + Cityname + "," + Statename + "," + pincode;
                    if (hid == "Yes")
                    {

                        query = "Update [tbl_leadnew] Set [Customerid]='" + customerid + "' ,[Name]='" + name + "' ,[Mobile]='" + mobile + "' ,[Email]='" + email + "' ,[Statename]='" + Statename + "' ,[Cityname]='" + Cityname + "' ,[Address]='" + addressf + "' ,[Pincode]='" + pincode + "' ,[Occupation]='" + occupation + "' ,[Land_Irrigation]='" + irrigation + "' ,[Taken_time]='" + stopwatchinp + "' ,[Lead_status]='Done',Nextstatus='" + nextstatus + "',[Status]='Active' ,[BranchName]='" + Session["ubrnameh"] + "' ,[BranchCode]='" + Session["ubrcodeh"] + "' ,[Logid]='" + Session["useridh"] + "' ,[Logname]='" + Session["usernameh"] + "' ,[releavedatetime]='" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "' where id='" + Session["Rowid"] + "'";

                    }
                    else
                    {

                        query = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus) VALUES('" + leadid + "','" + customerid + "','" + name + "','" + mobile + "','" + email + "','" + Statename + "','" + Cityname + "','" + addressf + "','" + pincode + "','" + occupation + "','" + irrigation + "','Close','XML','" + support + "','" + jsonlocation + "','','','','','','" + stopwatchinp + "','DKS','Done','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "')";

                    }

                    string pass = random(5);
                    string bodytext = "Dear " + name + ", your Registration has been Successfully.Your Loginid is : " + mobile + " and Password is : " + pass + ". To Login This Portal <a href='https://growfastgroups.com/'> Click Here </a>\n GROWFAST";


                    string cquery = "insert into tbl_customer(Customer_id ,Name ,Mobile ,Email ,Password ,Full_address,Address ,State ,City ,Postal_code ,Tahsil,Block ,Profile_pic ,Status,Datetime,Log_ID ,Log_name ,Log_Email,BranchName,BranchCode,RegistrationType,Groupname,Groupid) values('" + customerid + "','" + name + "','" + mobile + "','" + email + "','" + pass + "','" + addressf + "','" + address + "','" + Statename + "','" + Cityname + "','" + pincode + "','" + tahsil + "','" + block + "','pic','Lead','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["useridh"].ToString() + "', '" + Session["usernameh"].ToString() + "', '" + Session["usermailh"].ToString() + "','" + Session["ubrnameh"].ToString() + "', '" + Session["ubrcodeh"].ToString() + "','Lead','" + Session["Groupnameh"] + "','" + Session["Groupidh"] + "')";

                    string lquery = "insert into tbl_login(Username,Userid,Emailid,Mobile,Password,Type,Status,Datetime,OTP_Time,Branchname,Branchcode) values('" + name + "','" + customerid + "','" + email + "','" + mobile + "','" + pass + "','Customer','Active','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("HH:mm:ss") + "','" + Session["ubrnameh"].ToString() + "', '" + Session["ubrcodeh"].ToString() + "')";

                    if (db.InsertUpdateDelete(query) && db.InsertUpdateDelete(cquery) && db.InsertUpdateDelete(lquery))
                    {
                        res = leadid;
                        activitylog.Activitylogins("tbl_lead", db.getmaxid("tbl_lead").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_customer", db.getmaxid("tbl_customer").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_login", db.getmaxid("tbl_login").ToString(), lquery, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                        if (email != "")
                        {
                            //Messaging.SendMailEmployee("", form["email"], "Registration Successfully", bodytext);
                        }



                        
                        string queryb = "";
                        string Status = "";
                        JObject jsonObject1 = new JObject();
                        if (crfrd == "Know")
                        {
                            jsonObject1["Crop_ferti_details"] = crfrd;
                            jsonObject1["Land_arc"] = irrigationc;
                            jsonObject1["Irrigation_Eqp"] = irrieq;
                            jsonObject1["Cropdetail"] = cropnm;
                            jsonObject1["Fertilizerdetail"] = fertilizernm;
                            jsonObject1["Nextmeetdate"] = "";
                            jsonObject1["Nextmeettime"] = "";

                            jsonObject1["Intrestedarea"] = interested;
                            jsonObject1["Facing_Problems"] = problems;
                            jsonObject1["Commercial_planting"] = commercial;
                            jsonObject1["Profitable"] = profitable;

                            nextstatus = "Open";
                            Status = "Done";
                        }
                        else
                        {
                            jsonObject1["Crop_ferti_details"] = crfrd;
                            jsonObject1["OPersontname"] = "";
                            jsonObject1["OPersontcontact"] = "";
                            jsonObject1["Nextmeetdate"] = "";
                            jsonObject1["Nextmeettime"] = "";
                            nextstatus = "Open";
                            Status = "Follow Up";

                        }

                        // Convert JObject to JSON string
                        string jsondata1 = jsonObject1.ToString();

                        string query1b = "SELECT * FROM tbl_customer  WHERE Customer_id = '" + customerid + "'";
                        activitylog.Activitylogins("tbl_customer", "", query1b, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        DataTable dtb = db.GetAllRecord(query1b);
                        activitylog.Activitylogupd("Success", "");
                        if (dtb.Rows.Count > 0)
                        {
                            queryb = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,Crop_ferti_details) VALUES('" + leadid + "','" + customerid + "','" + dtb.Rows[0]["Name"] + "','" + dtb.Rows[0]["Mobile"] + "','" + dtb.Rows[0]["Email"] + "','" + dtb.Rows[0]["State"] + "','" + dtb.Rows[0]["City"] + "','" + dtb.Rows[0]["Full_address"] + "','" + dtb.Rows[0]["Postal_code"] + "','','','Close','" + jsondata1 + "','" + support + "','" + jsonlocation.Replace("'", "") + "','','','','','','" + stopwatchinp + "','BSA','" + Status + "','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "','" + crfrd + "')";

                            if (db.InsertUpdateDelete(queryb))
                            {
                                res = leadid;
                                activitylog.Activitylogins("tbl_leadnew", db.getmaxid("tbl_leadnew").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                                if (crfrd == "Know")
                                {
                                    ViewBag.AlertMessage = "Success";
                                    res = "Success";
                                    //Response.Redirect("/User/Demo?customerid=" + customerid + "&leadid=" + leadid);
                                }
                                else
                                {
                                    ViewBag.AlertMessage = "Success";
                                    res = "Success";
                                    //Response.Redirect("/User/VDknocks");
                                }

                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_leadnew", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                            }
                        }
                        else
                        {

                        }



                        string[] replacementValues = { "GROWFAST ORGANIC DIMOND" };
                        //Messaging.SendWhatsappSMSNew(mobile, replacementValues, "Registration Successfully", name, Session["useridh"] + "", Session["usernameh"] + "", Session["empridh"] + "");

                        if (irrigation == "Agriculture")
                        {
                            ViewBag.AlertMessage = "Success";
                            //Response.Redirect("/User/BSA?customerid=" + customerid + "&leadid=" + leadid);
                        }


                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_customer", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        activitylog.Activitylogins("tbl_login", "", lquery, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.AlertMessage = customerid + " Registration Failed";
                        activitylog.Activitylogins("tbl_lead", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    }
                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        //Demo
        public JsonResult Demo()
        {
            string res = "";
            var loc = Request.Form["loc"];
            var latitude = Request.Form["latitude"];
            var longitude = Request.Form["longitude"];
            var support = Request.Form["support"];
            var doubtname = Request.Form["doubtname"];
            var pproductid = Request.Form["pproductid"];
            var pproductname = Request.Form["pproductname"];
            var pbrandname = Request.Form["pbrandname"];
            var pproductcategory = Request.Form["pproductcategory"];
            var pproductunittype = Request.Form["pproductunittype"];
            var typeproduct = Request.Form["typeproduct"];
            var Order = Request.Form["Order"];

            var stopwatchinp = Request.Form["stopwatchinp"];
            var customerid = Request.Form["Customer_id"];
            var leadid = Request.Form["leadid"];
            try
            {

                // Create a JObject to store data
                JObject jsonObject = new JObject();
                jsonObject["Location"] = loc;
                jsonObject["Latitude"] = latitude;
                jsonObject["Longitude"] = longitude;
                // Convert JObject to JSON string
                string jsonlocation = jsonObject.ToString();

                string query = "";
                string hid = support;
                string nextstatus = "";
                string Status = "";
                JObject jsonObject1 = new JObject();
                if (doubtname == "No Doubts")
                {
                    jsonObject1["Productid"] = pproductid;
                    jsonObject1["Productname"] = pproductname;
                    jsonObject1["Brandname"] = pbrandname;
                    jsonObject1["Productcategory"] =pproductcategory;
                    jsonObject1["Productunittype"] = pproductunittype;
                    jsonObject1["Doubts"] = doubtname;

                    nextstatus = "Open";
                    Status = "Done";
                    
                }
                else if (doubtname == "Follow Up")
                {
                    jsonObject1["Productid"] = pproductid;
                    jsonObject1["Productname"] = pproductname;
                    jsonObject1["Brandname"] = pbrandname;
                    jsonObject1["Productcategory"] =pproductcategory;
                    jsonObject1["Productunittype"] = pproductunittype;
                    jsonObject1["Doubts"] = doubtname;

                    nextstatus = "Open";
                    Status = "Follow Up";
                    
                }
                else
                {
                    jsonObject1["Productid"] = pproductid;
                    jsonObject1["Productname"] = pproductname;
                    jsonObject1["Brandname"] = pbrandname;
                    jsonObject1["Productcategory"] =pproductcategory;
                    jsonObject1["Productunittype"] = pproductunittype;
                    jsonObject1["Doubts"] = doubtname;

                    nextstatus = "Close";
                    Status = "Done";
                }

                // Convert JObject to JSON string
                string jsondata1 = jsonObject1.ToString();

                string query1 = "SELECT * FROM tbl_customer  WHERE Customer_id = '" + customerid + "'";
                activitylog.Activitylogins("tbl_customer", "", query1, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                DataTable dt = db.GetAllRecord(query1);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    if (hid == "Yes")
                    {
                        query = "Update [tbl_leadnew] Set [Customerid]='" + customerid + "' ,[Name]='" + dt.Rows[0]["Name"] + "' ,[Mobile]='" + dt.Rows[0]["Mobile"] + "' ,[Email]='" + dt.Rows[0]["Email"] + "' ,[Statename]='" + dt.Rows[0]["State"] + "' ,[Cityname]='" + dt.Rows[0]["City"] + "' ,[Address]='" + dt.Rows[0]["Full_address"] + "' ,[Pincode]='" + dt.Rows[0]["Postal_code"] + "' ,[Occupation]='' ,[Land_Irrigation]='' ,[Taken_time]='" + stopwatchinp + "' ,[Lead_status]='" + Status + "',Nextstatus='" + nextstatus + "',[Status]='Active' ,[BranchName]='" + Session["ubrnameh"] + "' ,[BranchCode]='" + Session["ubrcodeh"] + "' ,[Logid]='" + Session["useridh"] + "' ,[Logname]='" + Session["usernameh"] + "' ,[releavedatetime]='" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "',XML='" + jsondata1 + "',OrderDetails='" + Order + "' where id='" + Session["Rowid"] + "'";

                    }
                    else
                    {
                        query = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,OrderDetails) VALUES('" + leadid + "','" + customerid + "','" + dt.Rows[0]["Name"] + "','" + dt.Rows[0]["Mobile"] + "','" + dt.Rows[0]["Email"] + "','" + dt.Rows[0]["State"] + "','" + dt.Rows[0]["City"] + "','" + dt.Rows[0]["Full_address"] + "','" + dt.Rows[0]["Postal_code"] + "','','','Close','" + jsondata1 + "','" +support + "','" + jsonlocation + "','','','','','','" + stopwatchinp + "','Demo','" + Status + "','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "','" + Order + "')";

                    }

                    if (db.InsertUpdateDelete(query))
                    {
                        res = leadid;
                        activitylog.Activitylogins("tbl_leadnew", db.getmaxid("tbl_leadnew").ToString(), query, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                        if (doubtname == "No Doubts")
                        {
                            ViewBag.AlertMessage = "Success";
                            Session["typeproduct"] = typeproduct;
                            //Response.Redirect("/User/Order?customerid=" + customerid + "&leadid=" + leadid);
                        }
                        else
                        {
                            ViewBag.AlertMessage = "Success";
                            //Response.Redirect("/User/VBSA");
                        }



                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_leadnew", "", query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    }
                }
                else
                {

                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Addtocart(string id, string Customer)
        {
            string res = "";
            try
            {
                string query = "select * from tbl_cart_details where Customer_id='" + Customer + "' and ProductId='" + id + "' and logid='" + Session["useridh"] + "'";
                activitylog.Activitylogins("tbl_state", "", query, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {

                }
                else
                {
                    string cquery = "select * from tbl_customer where Customer_id='" + Customer + "'";
                    activitylog.Activitylogins("tbl_customer", "", cquery, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    DataTable cdt = db.GetAllRecord(cquery);
                    activitylog.Activitylogupd("Success", "");

                    string pquery = "select * from tbl_pro_price_circlar where Id='" + id + "'";
                    activitylog.Activitylogins("tbl_pro_price_circlar", "", pquery, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    DataTable pdt = db.GetAllRecord(pquery);
                    activitylog.Activitylogupd("Success", "");
                    Double total_quantity = 1;

                    string insertQuery = "";
                    if (pdt.Rows.Count > 0)
                    {
                        if (pdt.Rows[0]["Ptype"] + "" == "Plant")
                        {
                            total_quantity = 10;
                            insertQuery = "insert into tbl_cart_details(Customer_id,Name,ProductId,ProductName,ProductQuantity,status,[Datetime],[logid],[logname],price,total_price,total_quantity,Producttype,offerprice,bulkprice) values('" + cdt.Rows[0]["Customer_id"] + "','" + cdt.Rows[0]["Name"] + "','" + pdt.Rows[0]["Id"] + "','" + pdt.Rows[0]["Productname"] + "','1','Add In Cart','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["Useridh"] + "','" + Session["usernameh"] + "','" + pdt.Rows[0]["Offerprice"] + "','" + pdt.Rows[0]["Offerprice"] + "','" + total_quantity + "','" + pdt.Rows[0]["Ptype"] + "','" + pdt.Rows[0]["Sellprice"] + "','" + pdt.Rows[0]["Bulkprice"] + "')";
                        }
                        else
                        {
                            insertQuery = "insert into tbl_cart_details(Customer_id,Name,ProductId,ProductName,ProductQuantity,status,[Datetime],[logid],[logname],price,total_price,total_quantity,Producttype,offerprice) values('" + cdt.Rows[0]["Customer_id"] + "','" + cdt.Rows[0]["Name"] + "','" + pdt.Rows[0]["Id"] + "','" + pdt.Rows[0]["Productname"] + "','1','Add In Cart','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["Useridh"] + "','" + Session["usernameh"] + "','" + pdt.Rows[0]["Offerprice"] + "','" + pdt.Rows[0]["Sellprice"] + "','" + total_quantity + "','" + pdt.Rows[0]["Ptype"] + "','" + pdt.Rows[0]["Sellprice"] + "')";
                        }
                    }

                    if (db.InsertUpdateDelete(insertQuery))
                    {
                        activitylog.Activitylogins("tbl_cart_details", cdt.Rows[0]["Customer_id"] + "", insertQuery, "Success", "Insert Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        res = "Data Inserted";

                        DataTable resdt = db.GetAllRecord("select * from tbl_cart_details where Customer_id='" + Customer + "' and ProductId='" + id + "' and logid='" + Session["useridh"] + "'");

                        res = JsonConvert.SerializeObject(resdt, Formatting.None);
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_cart_details", cdt.Rows[0]["Customer_id"] + "", insertQuery, "Fail", "Insert Fail", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        res = "Data Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Increment_InCart(string customerid, int ProductId, int cartId, int ProductQty, string Price, string offerprice, string bulkprice, string Ptype)
        {
            try
            {
                if (customerid != "")
                {
                    Double total_quantity = 1;
                    if (Ptype == "Plant")
                    {
                        total_quantity = Convert.ToDouble(ProductQty) * 10;

                        if (Convert.ToInt32(total_quantity) < 99 && Convert.ToInt32(total_quantity) > 0)
                        {

                        }
                        else if (Convert.ToInt32(total_quantity) < 199 && Convert.ToInt32(total_quantity) > 99)
                        {
                            Price = offerprice;
                        }
                        else if (Convert.ToInt32(total_quantity) > 199)
                        {
                            Price = bulkprice;
                        }


                    }
                    else
                    {
                        total_quantity = ProductQty;
                        Price = offerprice;
                    }

                    int Count_quantity = Convert.ToInt32(ProductQty);

                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_details set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "',total_quantity='" + total_quantity + "' where Customer_id='" + customerid + "' and ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                       
                        string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["usreid"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
                else
                {
                    int Count_quantity = Convert.ToInt32(ProductQty);
                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_sales set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "' where ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                        //getCart(customerid, ProductId + "");

                        //string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_sales", "", updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        //string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_sales", "", updateQuery, "Success", "Update Success", Session["usreid"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(new { success = true });
        }
        public JsonResult Decrement_InCart(string customerid, int ProductId, int cartId, int ProductQty, string Price, string offerprice, string bulkprice, string Ptype)
        {
            var dateee = "";
            try
            {
                if (customerid != "")
                {
                    Double total_quantity = 1;
                    if (Ptype == "Plant")
                    {
                        total_quantity = Convert.ToDouble(ProductQty) * 10;

                        if (Convert.ToInt32(total_quantity) < 99 && Convert.ToInt32(total_quantity) > 0)
                        {

                        }
                        else if (Convert.ToInt32(total_quantity) < 199 && Convert.ToInt32(total_quantity) > 99)
                        {
                            Price = offerprice;
                        }
                        else if (Convert.ToInt32(total_quantity) > 199)
                        {
                            Price = bulkprice;
                        }
                    }
                    else
                    {
                        total_quantity = ProductQty;
                        Price = offerprice;
                    }
                    int Count_quantity = Convert.ToInt32(ProductQty);

                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_details set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "',total_quantity='" + total_quantity + "' where Customer_id='" + customerid + "' and ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                        
                        string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
                else
                {
                    int Count_quantity = Convert.ToInt32(ProductQty);

                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_sales set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "' where  ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                        //getCart(customerid, ProductId + "");

                        //string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_sales", "", updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        //string hid = customerid.ToString();
                        activitylog.Activitylogins("tbl_cart_sales", "", updateQuery, "Success", "Update Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(new { success = true, updatedValue = dateee });

        }
        public JsonResult DeletefromCart(string Customerid, int ProductId, int cartId)
        {
            string msg = "";
            try
            {
                string deleteQuery = "delete from tbl_cart_details where id='" + cartId + "' and ProductId='" + ProductId + "' and Customer_id='" + Customerid + "'";
                if (db.InsertUpdateDelete(deleteQuery))
                {
                    string hid = Customerid;
                    activitylog.Activitylogins("tbl_cart_details", hid, deleteQuery, "Success", "Delete Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    msg = "Data Deleted";
                }
                else
                {
                    string hid = Customerid;
                    activitylog.Activitylogins("tbl_cart_details", hid, deleteQuery, "Success", "Delete Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                    msg = "Data Failed";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["userid"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewSelectedProduct(string customerid, string proid, string leadid,string typeproduct)
        {
            string msg = "";
            string Query = "";
            if (proid != "" && proid != null)
            {
                Query = "select *,CAST([total_price] AS DECIMAL(18, 2)) as Total_price1,CAST([price] AS DECIMAL(18, 2)) as price1,CAST([offerprice] AS DECIMAL(18, 2)) as offerprice1,CAST([bulkprice] AS DECIMAL(18, 2)) as bulkprice1 from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["Customer_id"] + "' and ProductId='" + proid + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "' order by Id desc";
            }
            else
            {
                Query = "select *,CAST([total_price] AS DECIMAL(18, 2)) as Total_price1,CAST([price] AS DECIMAL(18, 2)) as price1,CAST([offerprice] AS DECIMAL(18, 2)) as offerprice1,CAST([bulkprice] AS DECIMAL(18, 2)) as bulkprice1 from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["Customer_id"] + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "' order by Id desc";
            }


            DataTable dt = db.GetAllRecord(Query);
            string res = "";
            res+="<div class='row' style='margin:5px;padding:5px;'> <div class='card' style='margin:0px;padding:0px;'> <div class='shopping-cart' style='margin:0px;padding:5px;'>  <div class='row table-responsive' style='margin:0px;padding:0px;'>  <table class='table table-bordered table-hover' style='width:100%;'>";
                            if (dt.Rows.Count > 0)
                            {
                                string ql = "";
                                if (proid != "" && proid != null)
                                {
                                    ql = @"SELECT SUM(CAST([total_price] AS DECIMAL(18, 2))) AS [total_price] FROM tbl_cart_details where Customer_id='" + Session["Customer_id"] + "' and ProductId='" + proid + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "'";
                                }
                                else
                                {
                                    ql = @"SELECT SUM(CAST([total_price] AS DECIMAL(18, 2))) AS [total_price] FROM tbl_cart_details where Customer_id='" + Session["Customer_id"] + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "'";
                                }

                                DataTable totalDb = db.GetAllRecord(ql);

                res += "<thead> <tr> <th style='width:17%; text-align: center'>Image</th> <th style='width:45%; text-align: left'>Product</th> <th style='width:8%; text-align: center'>Price</th> <th style='width:30%; text-align: center'>Unit</th> <th style='width:2%; text-align: center'>Remove</th> <th style='width:8%; text-align: center'>Quantity</th> <th style='width:8%; text-align: center'>Total</th> </tr>  </thead>";

                                for (int jj = 0; jj < dt.Rows.Count; jj++)
                                {
                                    string Query11 = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status] ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and Id='" + dt.Rows[jj]["ProductId"] + "' and PType='" + typeproduct + "'";
                                    DataTable dt1 = db.GetAllRecord(Query11);
                                    if (dt1.Rows.Count > 0)
                                    {
                                        res += "<tbody> <tr> <td> <center><img src='"
                            + URL.ApiURL()+""+dt1.Rows[0]["Pimage"] + "' height ='80' width='80' onerror='this.src=\"/Content/Img/defaultimg1.png\"'></center> </td> <td> <b>" + dt1.Rows[0]["Productname"] + "</b><br />"+ dt1.Rows[0]["Description"] + "</td> <td>";
                                                    if (dt.Rows[jj]["Producttype"] + "" == "Plant")
                                                    {
                                                        if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") < 99 && Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 0)
                                                        {
                                res += "<center>"+dt.Rows[jj]["price1"]+"</center>";
                                                        }
                                                        else if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") < 199 && Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 99)
                                                        {
                                res += "<center>"+ dt.Rows[jj]["offerprice1"] + "</center>";
                                                        }
                                                        else if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 199)
                                                        {
                                res += "<center>"+ dt.Rows[jj]["bulkprice1"] + "</center>";
                                                        }
                                                    }
                                                    else
                                                    {
                            res += "<center>"+ dt.Rows[jj]["offerprice1"] + "</center>";
                                                    }

                                                res += "</td> <td style='text-align: justify;'> <div class='p-2'> <div class='row row-cols-1 row-cols-sm-3 border-1 text-center'> <div class='col border border-1 rounded-start-2'> <a href='#' class='btn btn-sm border-0' onclick='incrementCart(\"" + Session["Customer_id"] + "\", \"" + dt.Rows[jj]["ProductId"] + "\", \"" + dt.Rows[jj]["id"] + "\",\"" + dt.Rows[jj]["ProductQuantity"] + "\",\"" + dt.Rows[jj]["price1"] + "\",\"" + dt1.Rows[0]["Ptype"] + "\",\"" + dt.Rows[jj]["offerprice1"] + "\",\"" + dt.Rows[jj]["bulkprice1"] + "\")' ><span class='fa fa-plus'></span></a> </div>  <div class='col border border-1'>  <a class='btn btn-sm border-0'> <span class='text-dark' style='color:black'> <span id='counting_"+dt.Rows[jj]["id"]+"' onclick='quantity(\"addquantity_" + dt.Rows[jj]["id"] + "\",\"counting_"+ dt.Rows[jj]["id"] +"\",\"" + Session["Customer_id"] + "\",\"" +dt.Rows[jj]["ProductId"] + "\",\"" +dt.Rows[jj]["id"] + "\",\"" +dt.Rows[jj]["price1"] + "\",\"" +dt1.Rows[0]["Ptype"] + "\",\"" +dt.Rows[jj]["offerprice1"] + "\",\"" +dt.Rows[jj]["bulkprice1"] + "\")'> "+dt.Rows[jj]["ProductQuantity"]+"  </span> <input type='text' id='addquantity_"+ dt.Rows[jj]["id"] + "' style='width:50px;display:none' class='form-control shadow-none p-1' /> </span> </a> </div> <div class='col border border-1 rounded-end-2'><button type='button' class='btn btn-sm border-0' onclick='decrementCart(\"" + Session["Customer_id"] + "\",\"" + dt.Rows[jj]["ProductId"] + "\",\"" + dt.Rows[jj]["id"] + "\",\"" + dt.Rows[jj]["ProductQuantity"] + "\",\"" + dt.Rows[jj]["price1"] + "\",\"" + dt1.Rows[0]["Ptype"] + "\",\"" + dt.Rows[jj]["offerprice1"] + "\",\"" + dt.Rows[jj]["bulkprice1"] + "\")'><span class='fa fa-minus'></span></button></div> </div> </div> </td> <td> <center> <a href='#' onclick= 'deleteRecord(\"" + Session["Customer_id"] + "\",\"" +dt.Rows[jj]["ProductId"]+ "\",\"" +dt.Rows[jj]["id"]+ "\")' class='btn btn-sm btn-outline-danger border-0 delete-button'><i class='fa fa-trash'></i></a> </center>'</td> <td> <center> "+dt.Rows[jj]["total_quantity"]+" </center> </td> <td> <center> "+dt.Rows[jj]["Total_price1"]+" </center> </td> </tr> </tbody>";
                                    }
                                }
                res += "<tr> <td colspan='7'> <div class='totals'> <div class='totals-item'> <label><b>Grand Total</b></label> <div class='totals-value' id='cart-subtotal'><b>Rs. " + totalDb.Rows[0]["total_price"] + "</b></div> </div> </div> </td> </tr>";
                            }
            res += "</table> </div> <div class='row'> <div class='col-12  d-flex text-end'><button class='btn btncolor btn-sm' type='button'  onclick='Viewcheckout()'><b>Checkout</b></button> </div> </div>  </div> </div> </div>";

            msg = res;


            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viewcheckout(string customerid, string proid, string leadid, string typeproduct)
        {
            string res = "";
            string Query = "";
            if (proid != "" && proid != null)
            {
                Query = "select *,CAST([total_price] AS DECIMAL(18, 2)) as Total_price1,CAST([price] AS DECIMAL(18, 2)) as price1,CAST([offerprice] AS DECIMAL(18, 2)) as offerprice1,CAST([bulkprice] AS DECIMAL(18, 2)) as bulkprice1 from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["Customer_id"] + "' and ProductId='" + proid + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "' order by Id desc";
            }
            else
            {
                Query = "select *,CAST([total_price] AS DECIMAL(18, 2)) as Total_price1,CAST([price] AS DECIMAL(18, 2)) as price1,CAST([offerprice] AS DECIMAL(18, 2)) as offerprice1,CAST([bulkprice] AS DECIMAL(18, 2)) as bulkprice1 from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["Customer_id"] + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "' order by Id desc";
            }


            DataTable dt = db.GetAllRecord(Query);



            string Query34 = @"select * from tbl_customer where Customer_id='" + Session["Customer_id"] + "'";
            DataTable tbll = db.GetAllRecord(Query34);
            string name = "", mobile = "", mail = "", state = "", district = "", address = "", pincode = "", totalamount = "", full_address = "", tahsil = "", block = "";
            if (tbll.Rows.Count > 0)
            {
                name = tbll.Rows[0]["Name"] + "";
                mobile = tbll.Rows[0]["Mobile"] + "";
                mail = tbll.Rows[0]["Email"] + "";
                state = tbll.Rows[0]["State"] + "";
                district = tbll.Rows[0]["City"] + "";
                address = tbll.Rows[0]["Address"] + "";
                pincode = tbll.Rows[0]["Postal_code"] + "";
                full_address = tbll.Rows[0]["Full_address"] + "";
                tahsil = tbll.Rows[0]["Tahsil"] + "";
                block = tbll.Rows[0]["Block"] + "";
            }


            res += "<div class='row row-cols-1 row-cols-sm-1'> <div class='col mb-3'> <div class='card'> <div class='card-header bg-light mb-0 pb-0' id='dvcard_hdetails'> <div class='row row-cols-1 row-cols-sm-2 '> <div class='col col-sm-8'>  <h6> <span class='bg-light text-dark ps-2 pe-1 text-center rounded-1'> 1 </span> <label class='ms-2' id='lbl_details'>";
                                if (tbll.Rows.Count > 0)
                                {
                res += "Customer Details<i class='fa fa-check text-success' id='fa_details'></i>";
                                }
            res += "</label><br /> <span style='font-size:12px' class='ps-4 ms-1' id='detnamenum'> <b> " + name + "</b> +91 " + mobile + " </span> </h6>  </div> <div class='col col-sm-4 mb-2 justify-content-end d-flex'> <button type='button' class='btn btncolor' style='height:30px;width:100px' id='btn_chdetails'>Change</button> </div>  </div>  </div> <div class='card-body hidden' id='dvcard_bdetails'> <div class='row row-cols-1 row-cols-sm-4' style='margin:0px;padding:0px;'> <div class='col'> <span class='text-danger' id='sppersonal'></span> <input type='hidden' id='hid' name='hid' /> <h6 class='mt-2'>Name</h6> <input type='text' class='form-control shadow-none readonly' name='cname' id='cname' required readonly placeholder='Enter Your Name' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='" + name + "' /> <div class='invalid-feedback'> Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>Mobile Number</h6> <input type='text' class='form-control shadow-none readonly' id='cmobile' name='cmobile' readonly maxlength='10' required oninput='validateNumericInput(this, 10, 'spnum')' pattern='^[0-9]*$' title='Please enter only letters, and spaces' placeholder='Enter Your Mobile No.' value='" + mobile + "' /> <div class='invalid-feedback' id='spnum'>     Please enter only letters, and spaces </div>  </div>  <div class='col'> <h6 class='mt-2'> Email</h6> <input type='email' class='form-control shadow-none readonly' id='cmailuid' name='cmailuid' readonly required oninput='validateEmail('cmailuid','spmail')' placeholder='Enter Your Email' value='" + mail + "' /> <span id='spmail'></span> </div> <div class='col justify-content-end d-flex pt-2'> <button type='button' class='btn btncolor ps-3 mt-4 pe-3' style='height:30px;width:100px' id='btn_nextdetails'> Next</button> </div> <div class='col-12 col-sm-12' style='margin:0px;padding:7px;'> <input type='checkbox' class='shadow-none' name='contactcheck' id='contactcheck' required /> Change Contact Information  </div> </div> <div class='row row-cols-1 row-cols-sm-3' style='margin:0px;padding:0px;' id='contact_info'> <div class='col'>  <h6 class='mt-2'>New Contact Number</h6> <input type='text' class='form-control shadow-none' id='cnmobile' name='cnmobile' maxlength='10' required oninput='validateNumericInput(this, 10, 'nspnum')' pattern='^[0-9]*$' title='Please enter only letters, and spaces' placeholder='Enter Your Mobile No.' value='" + mobile + "' /> <div class='invalid-feedback' id='nspnum'>  Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>New Email Number</h6> <input type='email' class='form-control shadow-none' id='cnmailuid' name='cnmailuid' required oninput='validateEmail('cmailuid','nspmail')' placeholder='Enter Your Email' value='" + mail + "' /> <div class='invalid-feedback' id='nspmail'>  Please enter only letters, and spaces </div> </div>  </div> </div> </div> </div>";

            res += "<div class='col mb-3'>  <div class='card'> <div class='card-header bg-light' id='dvcard_haddredd'>  <div class='row row-cols-1 row-cols-sm-2'> <div class='col col-sm-8'> <h6>     <span class='bg-light text-dark ps-2 pe-1 text-center rounded-1'> 2 </span> <label class='ms-2' id='lbl_address'>  Delivery Adderss <i class='fa fa-check text-success' id='fa_address'></i></label><br />     <span style='font-size:12px' class='ps-4 ms-1' id='spanaddress'> <b> " + full_address + "</b> </span> </h6></div> <div class='col col-sm-4 justify-content-end d-flex'> <button type='button' class='btn btncolor' style='height:30px;width:100px' id='btn_chaddress'>Change</button> </div>  </div> </div> <div class='card-body hidden' id='dvcard_baddredd'> <div class='row row-cols-1 row-cols-sm-1' style='margin:0px;padding:0px;'> <div class='col-12' style='margin:0px;padding:7px;'> <span class='text-danger' id='spshpping'></span> <h6 class='mt-2'>Address</h6> <input type='text' class='form-control shadow-none readonly' name='Name' id='txtaddress' required readonly placeholder='Enter Your Address' onkeyup='validateTextbox(this)' pattern='^[-a-zA-Z0-9 &()',./\n_ ]*$' title='Please enter only letters, and spaces' value='" + full_address + "' /> <div class='invalid-feedback'>     Please enter only letters, and spaces </div> </div> </div> <div class='row row-cols-1 row-cols-sm-4' style='margin:0px;padding:0px;'> <div class='col-12 col-sm-12' style='margin:0px;padding:7px;'> <input type='checkbox' class='shadow-none' name='addresscheck' id='addresscheck' required /> Change Shipping Address </div> <div class='col'> <h6 class='mt-2'>Address</h6> <input type='text' class='form-control shadow-none' name='shippingaddr' id='shippingaddr' required placeholder='Enter Address' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='" + address + "' /> <div class='invalid-feedback'>     Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>Block</h6> <input type='text' class='form-control shadow-none' name='shippingblock' id='shippingblock' required placeholder='Enter Block' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='" + block + "' /><div class='invalid-feedback'> Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>Tahsil</h6> <input type='text' class='form-control shadow-none' name='shippingtahsil' id='shippingtahsil' required placeholder='Enter Tahsil' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='"+tahsil + "' /> <div class='invalid-feedback'>     Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>Postal code</h6> <input type='text' class='form-control shadow-none' id='shippingpostal' name='shippingpostal' maxlength='6' required oninput='validateNumericInput(this, 6, 'spnump')' onkeyup='getStateCityDetails()' pattern='^[0-9]*$' title='Please enter only letters, and spaces' placeholder='Enter Your Pin/Postal Code' value='" + pincode + "' /> <div class='invalid-feedback' id='spnump'>     Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>District</h6> <input type='text' class='form-control shadow-none readonly' readonly name='shippingdist' id='shippingdist' required placeholder='Enter District' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='" + district + "' /> <div class='invalid-feedback'>     Please enter only letters, and spaces </div> </div> <div class='col'> <h6 class='mt-2'>State</h6> <input type='text' class='form-control shadow-none readonly'  readonly name='shippingstate' id='shippingstate' required placeholder='Enter State' onkeyup='validateTextbox(this)' pattern='^[a-zA-Z ]*$' title='Please enter only letters, and spaces' value='" + state + "' /> <div class='invalid-feedback'>     Please enter only letters, and spaces </div> </div> <div class='col'>  </div> <div class='col justify-content-end d-flex pt-2'> <button type='button' class='btn btncolor ps-3 mt-4 pe-3' style='height:30px;width:100px' id='btn_nextaddress'> Next</button> </div> </div> </div> </div> </div>";

            res += "<div class='col mb-3'> <div class='card'> <div class='card-header bg-light' id='dvcard_hborders'> <div class='row row-cols-1 row-cols-sm-2'>  <div class='col col-sm-8'>  <h6> <span class='bg-light text-dark ps-2 pe-1 text-center rounded-1'> 3 </span> <label class='ms-2' id='lbl_ordersummary'>Order Summary</label><br />  <span style='font-size:12px' class='ps-4 ms-1' id='spordersummary'> <b> "+ dt.Rows.Count + " Item</b> </span>  </h6>  </div> <div class='col col-sm-4 justify-content-end d-flex'> <button type='button' class='btn btncolor' style='height:30px;width:100px' id='btn_vieworder'>View</button> </div> </div>  </div>  <div class='card-body' id='dvcard_bborders'> <div class='row table-responsive' style='margin:0px;padding:0px;'>";
                    if (dt.Rows.Count > 0)
                    {
                        string ql = "";
                res += "<table class='table table-bordered table-hover' style='width:100%;' id='tblproduct'>";
                            if (proid != "" && proid != null)
                            {
                                ql = "SELECT SUM(CAST([total_price] AS DECIMAL(18, 2))) AS [total_price] FROM tbl_cart_details where Customer_id='" + Session["Customer_id"] + "' and ProductId='" + proid+ "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "'";
                            }
                            else
                            {
                                ql = "SELECT SUM(CAST([total_price] AS DECIMAL(18, 2))) AS [total_price] FROM tbl_cart_details where Customer_id='" + Session["Customer_id"] + "' and logid='" + Session["useridh"] + "' and Producttype ='" + typeproduct + "'";
                            }

                            DataTable totalDb = db.GetAllRecord(ql);
                totalamount = totalDb.Rows[0]["total_price"] +"";

                res += "<thead class='theadb text-start text-center'> <tr> <th style='width:15%; text-align: center;display:none'>Product_type</th> <th style='width:50%; text-align: center;display:none'>Product_Img</th> <th style='width:50%; text-align: center;display:none'>Product_Id</th> <th style='width:50%; text-align: center'>Product</th> <th style='width:50%; text-align: center'>Price</th> <th style='width:15%; text-align: center'>Quantity</th> <th style='width:15%; text-align: center'>Total</th> <th style='width:15%; text-align: center'>Points</th> </tr>  </thead>  <tbody>";
                                for (int jj = 0; jj < dt.Rows.Count; jj++)
                                {
                                    string Query11 = "select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status] ,[Datetime],[logid],[logname],Points,Brand, CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar]  where Id='" + dt.Rows[jj]["ProductId"] + "' and PType='" + typeproduct + "'";
                                    DataTable dt22 = db.GetAllRecord(Query11);
                                    if (dt22.Rows.Count > 0)
                                    {
                                        double total_point = Convert.ToDouble(dt22.Rows[0]["Points"]) * Convert.ToDouble(dt.Rows[jj]["ProductQuantity"]);

                        res += "<tr> <td style='display: none' class='text-center'>  " + dt22.Rows[0]["Ptype"] + " </td> <td style='display: none' class='text-center'>" + dt22.Rows[0]["Pimage"] + " </td> <td style='display: none' class='text-center'>" + dt22.Rows[0]["Id"] + " </td> <td class='text-start fw-bold'>" + dt22.Rows[0]["Brand"] + " </td> <td class='text-center'>";
                                                if (dt.Rows[jj]["Producttype"] + "" == "Plant")
                                                {
                                                    if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") < 99 && Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 0)
                                                    {
                                res += @dt.Rows[jj]["price1"];
                                                    }
                                                    else if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") < 199 && Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 99)
                                                    {
                                res += @dt.Rows[jj]["offerprice1"];
                                                    }
                                                    else if (Convert.ToInt32(dt.Rows[jj]["total_quantity"] + "") > 199)
                                                    {
                                res += @dt.Rows[jj]["bulkprice1"];
                                                    }
                                                }
                                                else
                                                {
                            res += @dt.Rows[jj]["offerprice1"];
                                                }
                        res += "</td> <td class='text-center'>" + dt.Rows[jj]["total_quantity"] + "</td> <td class='text-end'>" + dt.Rows[jj]["total_price1"] + " </td> <td class='text-end'>" + total_point + " </td>  </tr>";

                                    }
                                }

                           res += " </tbody> <tfoot> <tr> <td colspan='5' class='text-end'> <label><b>Grand Total</b></label> &nbsp; : &nbsp; <b>Rs. "+ totalDb.Rows[0]["total_price"] + "</b> <input type='hidden' id='totalval' value='"+ totalDb.Rows[0]["total_price"] + "' /> </td> </tr> </tfoot>  </table>";
                    }
            res += "</div> </div> </div> </div>";

    string query = "select * from tbl_paymentmethod where Status='Active'";
        activitylog.Activitylogins("tbl_paymentmethod", "", query, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
        DataTable dtpay = db.GetAllRecord(query);
        activitylog.Activitylogupd("Success", "");
        int count = 1;

            res += "<input type='hidden' name='totalpriceall' id='totalpriceall' value='" + totalamount + "'><div class='col mb-3' id='dv_continue'> <div class='card'> <div class='card-header bg-light'>     <div class='row row-cols-1 row-cols-sm-2'> <div class='col'>     <h6><span class='pe-1 text-center rounded-1'>  </span> <label>Order Confirmation message sent on this number <label id='lblmobile'> " + mobile + " </label></label></h6></div> <div class='col justify-content-end d-flex'>     <button type='button' class='btn btncolor' style='height:30px;width:100px' id='btn_conorder'>Continue</button>     </div>     </div> </div><div class='card-body hidden'> </div> </div> </div> <div class='col mb-3'> <div class='card'> <div class='card-header bg-light' id='dvcard_hconorder'>     <div class='row row-cols-1 row-cols-sm-2'> <div class='col'>     <h6><span class='bg-light text-dark ps-2 pe-1 text-center rounded-1'> 4 </span> <label class='ms-2'>Payment Options</label></h6> </div> <div class='col justify-content-end d-flex'> </div>     </div> </div> <div class='card-body hidden' id='dvcard_bconorder'>     <h6 id='sptotalpayble'></h6> <div class='row row-cols-1 row-cols-sm-1' style='margin:0px;padding:0px;'> <div class='col'>     <h6 class='mt-2'>Select Payment Method</h6>     <input id='suip' name='payment' value='upi' type='radio'>     <label for='suip'> UPI</label>&nbsp;&nbsp;     <input id='sqr' name='payment' value='qr' type='radio'>     <label for='sqr'>QR Code</label>&nbsp;&nbsp;     <input id='scheck' name='payment' value='check' type='radio'>     <label for='scheck'> CHEQUE</label>&nbsp;&nbsp;     <input id='scash' name='payment' value='cash' type='radio'>     <label for='scash'>Cash</label> </div> <div class='col hidden' id='upiDetails'>     <div class='row row-cols-1 row-cols-sm-4'>         <div class='col'>";
                if (dtpay.Rows.Count > 0)             
            {                 
                for (int i = 0; i < dtpay.Rows.Count; i++)                
                {                    
                    if (dtpay.Rows[i]["Methodtype"] + "" == "UPI")                     
                    {
                        res += "<h5 class='p-2'> "+dtpay.Rows[i]["UPIQR"]+" </h5> ";
         }                
}             
}
            res += "</div> </div> </div> <div class='col hidden' id='qrCodeDetails'>     <div class='row row-cols-1 row-cols-sm-4'>         <div class='col'>  ";           if (dtpay.Rows.Count > 0)             
                {                 
                for (int i = 0; i < dtpay.Rows.Count; i++)                 
                {                     
                    if (dtpay.Rows[i]["Methodtype"] + "" != "UPI")                    
                    {
                        res += " <img src='"+ URL.ApiURL()+""+dtpay.Rows[i]["UPIQR"] + "' style='height:100px;width:100px' onerror='this.src=\"/Content/Img/defaultimg1.png\"' />  ";                   
                            }                 
                }             
            }
            res += " </div>  </div> </div> <div class='col-12 col-sm-6'>     <label for='utrcheck'>Enter UTR no/cheque no/Cash Amount</label>     <input type='text' class='form-control shadow-none' name='utrcheck' id='utrcheck' required /> </div> <div class='col-12 col-sm-6' id='dvchequedate'>     <label for='payslip'>Select cheque date</label>     <input type='date' class='form-control shadow-none' name='chequedate' id='chequedate' required /> </div> <div class='col-12 col-sm-6' style='display:none'>     <label for='payslip'>Upload Payment Slip</label>     <input type='file' class='form-control shadow-none' name='payslip' id='payslip' required /> </div> <div class='col-12 col-sm-12 justify-content-end d-flex mt-2'>     <button type='button' class='btn btncolor' id='confirm_order' style='height:30px;width:100px'><b>Confirm Order</b></button> </div> <span class='text-danger' id='spvalidmsg'></span>     </div> </div> </div> </div> </div>";


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmPayment(string OrderId, string Paidamount, string Paymentstatus, string Paidamountval)
        {
            string res = "";
            string[] res1 = { };
            try
            {
                string status = "";
                string Query = @"select * from tbl_order_summary where Order_id='" + OrderId + "' order by Id desc";
                DataTable dts = db.GetAllRecord(Query);
                if (dts.Rows.Count > 0)
                {
                    int j = 0;
                    string trpro = "";
                    double balanceamount = 0;
                    string totalamount = dts.Rows[0]["Total_amount"] + "";
                    string agentname = dts.Rows[0]["logname"] + "";
                    string agentid = dts.Rows[0]["logid"] + "";
                    string query1 = "update tbl_order_summary set Paid_amount='" + Paidamount + "',Payment_status='" + Paymentstatus + "' where Order_id='" + OrderId + "'";

                    if (db.InsertUpdateDelete(query1))
                    {
                        string jhsd = "select * from tbl_order_transaction where Order_id='" + OrderId + "' and Tr_status='Due' and Status='Requested'";
                        DataTable trdt = db.GetAllRecord(jhsd);
                        string paymentmethod = "";
                        if (trdt.Rows.Count > 0)
                        {
                            paymentmethod = trdt.Rows[0]["Payment_method"] + "";

                            balanceamount = Convert.ToDouble(totalamount) - Convert.ToDouble(Paidamount);
                            Double closingam = Convert.ToDouble(Paidamountval) - Convert.ToDouble(Paidamount);
                            if (closingam == 0)
                            {
                                status = "No Due";
                            }
                            else
                            {
                                status = "Due";
                            }
                            string uptrq = "update tbl_order_transaction set Amount='" + Paidamount + "',Opening_amount='" + Paidamountval + "',Closing_amount='" + closingam + "',Tr_status='" + status + "',Status='Confirmed' where Order_id='" + OrderId + "' and Transaction_id='" + trdt.Rows[0]["Transaction_id"] + "'";
                            db.InsertUpdateDelete(uptrq);
                        }
                        activitylog.Activitylogins("tbl_order_summary", OrderId, query1, "Success", "Insert Success", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                        res = "Data Updated";
                        res1 = new string[9] { "Data Updated", OrderId, dts.Rows[0]["Id"] + "", dts.Rows[0]["Customer_mobile"] + "", "", dts.Rows[0]["Customer_name"] + "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
                        paymentmethod = dts.Rows[0]["Payment_method"] + "";
                        string squery = "select * from tbl_order where Order_id='" + OrderId + "' and Status='Active'";
                        DataTable dt = db.GetAllRecord(squery);
                        if (dt.Rows.Count > 0)
                        {
                            string productname = "";
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                                // Extract the year and month from the current date
                                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                                string invoiceid = "I" + currentYearmonth + "" + arandom(5);
                                string otp = random(4);

                                string query = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[Product_type],[OTP],[Remark],OrderID2,Points,Permanent_address,Tahsil,Block,Address,State,District)VALUES('" + dt.Rows[i]["Order_id"] + "','" + dt.Rows[i]["Product_xml"] + "','" + dt.Rows[i]["Customer_name"] + "','" + dt.Rows[i]["Customer_id"] + "','" + dt.Rows[i]["Customer_mobile"] + "','" + dt.Rows[i]["Customer_address"] + "','" + dt.Rows[i]["Pincode"] + "','" + dt.Rows[i]["Total_amount"] + "','Confirmed','" + dt.Rows[i]["Payment_method"] + "','" + dt.Rows[i]["Payment_upload"] + "','0','" + status + "','" + dt.Rows[i]["logid"] + "','" + dt.Rows[i]["logname"] + "','" + DateTime.Parse(dt.Rows[i]["Datetime"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[i]["Membertype"] + "','" + dt.Rows[i]["Orderby"] + "','" + invoiceid + "','" + dt.Rows[i]["Drivername"] + "','" + dt.Rows[i]["Driverid"] + "','" + dt.Rows[i]["Drivermobile"] + "','" + dt.Rows[i]["Vehiclename"] + "','" + dt.Rows[i]["Vehicleid"] + "','Active','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','Active','" + Session["usernamec"] + "','" + Session["useridc"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[i]["BranchName"] + "','" + dt.Rows[i]["BranchCode"] + "','" + dt.Rows[i]["Product_Img"] + "','" + dt.Rows[i]["Product_id"] + "','" + dt.Rows[i]["Product_name"] + "','" + dt.Rows[i]["Product_price"] + "','" + dt.Rows[i]["Product_quantity"] + "','" + dt.Rows[i]["Total_proamount"] + "','" + dt.Rows[i]["Product_type"] + "','" + otp + "','" + dt.Rows[i]["Remark"] + "','" + dt.Rows[i]["OrderID2"] + "','" + dt.Rows[i]["Points"] + "','" + dt.Rows[i]["Permanent_address"] + "','" + dt.Rows[i]["Tahsil"] + "','" + dt.Rows[i]["Block"] + "','" + dt.Rows[i]["Address"] + "','" + dt.Rows[i]["State"] + "','" + dt.Rows[i]["District"] + "' )";

                                db.InsertUpdateDelete("update tbl_order set Status='Inactive' where Order_id='" + OrderId + "' and OrderID2='" + dt.Rows[i]["OrderID2"] + "' and id='" + dt.Rows[i]["id"] + "'");
                                if (db.InsertUpdateDelete(query))
                                {
                                    activitylog.Activitylogins("tbl_order", dt.Rows[i]["id"] + "", query, "Success", "Insert Success", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                                    ViewBag.msg = "Data Updated";

                                    if (productname == "")
                                    {
                                        productname = dt.Rows[i]["Product_name"] + "";
                                    }
                                    else
                                    {
                                        productname += "," + dt.Rows[i]["Product_name"] + "";
                                    }

                                }
                                else
                                {
                                    activitylog.Activitylogins("tbl_order", dt.Rows[i]["id"] + "", query, "Failed", "Insert Failed", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                                    res = "Update Failed";
                                    res1 = new string[9] { "Update Failed", OrderId, dt.Rows[0]["Id"] + "", dt.Rows[0]["Customer_mobile"] + "", "", dt.Rows[0]["Customer_name"] + "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
                                }
                            }



                            res1 = new string[9] { "Success", OrderId, dt.Rows[0]["Id"] + "", dt.Rows[0]["Customer_mobile"] + "", "", dt.Rows[0]["Customer_name"] + "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
                        }
                        else
                        {
                            res = "Order Not Found";
                            res1 = new string[9] { "Order Not Found", OrderId, dt.Rows[0]["Id"] + "", dt.Rows[0]["Customer_mobile"] + "", "", dt.Rows[0]["Customer_name"] + "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
                        }



                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_order_summary", OrderId, query1, "Failed", "Insert Failed", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                        res = "Update Failed";
                        res1 = new string[9] { "Update Failed", OrderId, "", "", "", "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", ""), errorName.Replace("'", ""), errorTrace.Replace("'", "`"), Session["userid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                }

                catch
                {

                }
                res = "Error";
                res1 = new string[9] { "Error", OrderId, "", "", "", "", Session["useridc"] + "", Session["usernamec"] + "", Session["empridc"] + "" };
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Expecteddilivery(string OrderId, string Expectedfrom, string Expectedto, string Remark)
        {
            string msg = "";
            try
            {
                string status = "";
                string Query = @"select * from tbl_order_summary where Order_id='" + OrderId + "' order by Id desc";
                DataTable sdt = db.GetAllRecord(Query);
                if (sdt.Rows.Count > 0)
                {
                    status = sdt.Rows[0]["Payment_status"] + "";
                    if (status != "Active")
                    {
                        string squery = "select * from tbl_order where Order_id='" + OrderId + "' and Status='Active'";
                        DataTable dt = db.GetAllRecord(squery);
                        if (dt.Rows.Count > 0)
                        {
                            DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                            // Extract the year and month from the current date
                            string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                            string invoiceid = "I" + currentYearmonth + "" + arandom(5);
                            string otp = random(4);

                            string formattedfromDate = DateTime.ParseExact(Expectedfrom, "yyyy-M-d", null).ToString("dd-MM-yyyy");
                            string formattedtoDate = DateTime.ParseExact(Expectedto, "yyyy-M-d", null).ToString("dd-MM-yyyy");

                            string query = "Update [tbl_order] set [ExpDelivery_date]='" + formattedfromDate + " to " + formattedtoDate + "',[Remark]='" + Remark + "'  where Order_id='" + OrderId + "' and Status='Active'";

                            if (db.InsertUpdateDelete(query))
                            {
                                db.InsertUpdateDelete("Update [tbl_order_summary] set [ExpDelivery_date]='" + formattedfromDate + " to " + formattedtoDate + "' where Order_id='" + OrderId + "'");

                                activitylog.Activitylogins("tbl_order", OrderId, query, "Success", "Insert Success", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                                msg = "Data Updated";
                                string productname = "";
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (productname == "")
                                    {
                                        productname = dt.Rows[i]["Product_name"] + "";
                                    }
                                    else
                                    {
                                        productname += "," + dt.Rows[i]["Product_name"] + "";
                                    }
                                }


                               
                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_order", OrderId, query, "Failed", "Insert Failed", Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                                msg = "Update Failed";
                            }

                        }
                        else
                        {
                            msg = "Order Not Found";
                        }
                    }
                    else
                    {
                        msg = "Please Verify Payment";
                    }

                }
                else
                {
                    msg = "Order Not Found";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", ""), errorName.Replace("'", ""), errorTrace.Replace("'", "`"), Session["useridc"].ToString(), Session["usernamec"].ToString(), Session["usermailc"].ToString());
                }

                catch
                {
                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Dispatchorder(string hid, string hOrderid, string hOrderid2, string OTP, string productquantity, string productprice, string productamount, string product_point)
        {
            string[] res = { };
            try
            {
                string squery = "select * from tbl_order where Order_id='" + hOrderid + "' and OrderID2='" + hOrderid2 + "' and Status='Active' and OTP='" + OTP + "'";
                DataTable dt = db.GetAllRecord(squery);

                if (dt.Rows.Count > 0)
                {
                    if (productquantity == dt.Rows[0]["Product_quantity"] + "")
                    {

                    }
                    else
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("Product_type", typeof(string));
                        dataTable.Columns.Add("Product_Img", typeof(string));
                        dataTable.Columns.Add("Product_Id", typeof(string));
                        dataTable.Columns.Add("Product", typeof(string));
                        dataTable.Columns.Add("Price", typeof(string));
                        dataTable.Columns.Add("Quantity", typeof(string));
                        dataTable.Columns.Add("Total", typeof(string));
                        dataTable.Columns.Add("Points", typeof(string));

                        dt.Rows[0]["Product_quantity"] = productquantity;
                        dt.Rows[0]["Product_price"] = productprice;
                        dt.Rows[0]["Total_proamount"] = productamount;
                        dt.Rows[0]["Points"] = product_point;

                        string proxml = dt.Rows[0]["Product_xml"] + "";
                        DataTable xmldt = Encryption.ConvertXmlTo_Datatable(proxml);
                        if (xmldt.Rows.Count > 0)
                        {
                            for (int i = 0; i < xmldt.Rows.Count; i++)
                            {
                                DataRow newRow = dataTable.NewRow();
                                newRow["Product_type"] = xmldt.Rows[i]["Product_type"] + "";
                                newRow["Product_Img"] = xmldt.Rows[i]["Product_Img"] + "";
                                newRow["Product_Id"] = xmldt.Rows[i]["Product_Id"] + "";
                                newRow["Product"] = xmldt.Rows[i]["Product"] + "";

                                if (xmldt.Rows[i]["Product_Id"] == dt.Rows[0]["Product_id"])
                                {
                                    newRow["Price"] = dt.Rows[0]["Product_price"] + "";
                                    newRow["Quantity"] = dt.Rows[0]["Product_quantity"] + "";
                                    newRow["Total"] = dt.Rows[0]["Total_proamount"] + "";
                                    newRow["Points"] = dt.Rows[0]["Points"] + "";
                                }
                                else
                                {
                                    newRow["Price"] = xmldt.Rows[i]["Price"] + "";
                                    newRow["Quantity"] = xmldt.Rows[i]["Quantity"] + "";
                                    newRow["Total"] = xmldt.Rows[i]["Total"] + "";
                                    newRow["Points"] = xmldt.Rows[i]["Points"] + "";
                                }
                                dataTable.Rows.Add(newRow);
                            }
                        }
                        proxml = Encryption.ConvertDatatableTo_XML(xmldt);
                        dt.Rows[0]["Product_xml"] = proxml;
                    }
                    string query = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[Product_type],[OTP],[ExpDelivery_date],[Remark],OrderID2,Points,Permanent_address,Tahsil,Block,Address,State,District,Dilivery_month,Invoicepdf)VALUES('" + dt.Rows[0]["Order_id"] + "','" + dt.Rows[0]["Product_xml"] + "','" + dt.Rows[0]["Customer_name"] + "','" + dt.Rows[0]["Customer_id"] + "','" + dt.Rows[0]["Customer_mobile"] + "','" + dt.Rows[0]["Customer_address"] + "','" + dt.Rows[0]["Pincode"] + "','" + dt.Rows[0]["Total_amount"] + "','Delivered','" + dt.Rows[0]["Payment_method"] + "','" + dt.Rows[0]["Payment_upload"] + "','" + dt.Rows[0]["Paid_amount"] + "','" + dt.Rows[0]["Payment_status"] + "','" + dt.Rows[0]["logid"] + "','" + dt.Rows[0]["logname"] + "','" + DateTime.Parse(dt.Rows[0]["Datetime"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[0]["Membertype"] + "','" + dt.Rows[0]["Orderby"] + "','" + dt.Rows[0]["Invoiceid"] + "','" + dt.Rows[0]["Drivername"] + "','" + dt.Rows[0]["Driverid"] + "','" + dt.Rows[0]["Drivermobile"] + "','" + dt.Rows[0]["Vehiclename"] + "','" + dt.Rows[0]["Vehicleid"] + "','Delivered','" + DateTime.Parse(dt.Rows[0]["Confirm_date"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','Active','" + Session["usernameh"] + "','" + Session["useridh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[0]["BranchName"] + "','" + dt.Rows[0]["BranchCode"] + "','" + dt.Rows[0]["Product_Img"] + "','" + dt.Rows[0]["Product_id"] + "','" + dt.Rows[0]["Product_name"] + "','" + dt.Rows[0]["Product_price"] + "','" + dt.Rows[0]["Product_quantity"] + "','" + dt.Rows[0]["Total_proamount"] + "','" + dt.Rows[0]["Product_type"] + "','" + dt.Rows[0]["OTP"] + "','" + dt.Rows[0]["ExpDelivery_date"] + "','" + dt.Rows[0]["Remark"] + "','" + dt.Rows[0]["OrderID2"] + "','" + dt.Rows[0]["Points"] + "','" + dt.Rows[0]["Permanent_address"] + "','" + dt.Rows[0]["Tahsil"] + "','" + dt.Rows[0]["Block"] + "','" + dt.Rows[0]["Address"] + "','" + dt.Rows[0]["State"] + "','" + dt.Rows[0]["District"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("MMM-yyyy") + "' ,'" + dt.Rows[0]["Invoicepdf"] + "')";

                    db.InsertUpdateDelete("update tbl_order set Status='Inactive' where Order_id='" + hOrderid + "' and OrderID2='" + hOrderid2 + "' and id='" + dt.Rows[0]["id"] + "'");
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_order", hOrderid, query, "Success", "Insert Success", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                        string cusdetails = "Select * from tbl_customer where Customer_id='" + dt.Rows[0]["Customer_id"] + "'";
                        DataTable dtlead = db.GetAllRecord(cusdetails);
                        if (dtlead.Rows.Count > 0)
                        {
                            JObject jsonObject = new JObject();
                            jsonObject["Location"] = "location";
                            jsonObject["Latitude"] = "latitude";
                            jsonObject["Longitude"] = "longitude";

                            // Convert JObject to JSON string
                            string jsonlocation = jsonObject.ToString();

                            JObject jsonObject1 = new JObject();
                            jsonObject1["Productdetail"] = dt.Rows[0]["Product_xml"] + "";
                            // Convert JObject to JSON string
                            string jsondata1 = jsonObject1.ToString();
                            string leadid = "";
                            DataTable ldt = db.GetAllRecord("select Leadid from tbl_leadnew where Customerid='" + dt.Rows[0]["Customer_id"] + "' ");
                            if (ldt.Rows.Count > 0)
                            {
                                leadid = ldt.Rows[0]["Leadid"] + "";
                            }

                            string querylead = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,OrderDetails) VALUES('" + leadid + "','" + dt.Rows[0]["Customer_id"] + "','" + dtlead.Rows[0]["Name"] + "','" + dtlead.Rows[0]["Mobile"] + "','" + dtlead.Rows[0]["Email"] + "','" + dtlead.Rows[0]["State"] + "','" + dtlead.Rows[0]["City"] + "','" + dtlead.Rows[0]["Full_address"] + "','" + dtlead.Rows[0]["Postal_code"] + "','','','Close','" + jsondata1 + "','No','" + jsonlocation + "','','','','','','','Documentation','Done','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','Close','')";
                            db.InsertUpdateDelete(querylead);
                        }

                        //Stock Transaction query

                        string queryp = "select * from tbl_diliverypointstock where Productid='" + dt.Rows[0]["Product_id"] + "' and Groupid='" + Session["Groupidh"] + "'";
                        DataTable dtp = db.GetAllRecord(queryp);
                        if (dtp.Rows.Count > 0)
                        {
                            string Debit = dtp.Rows[0]["Debit"] + "";
                            string balance = dtp.Rows[0]["Balance"] + "";
                            double allcdebit = Convert.ToDouble(Debit) + Convert.ToDouble(dt.Rows[0]["Product_quantity"]);
                            double allbalance = Convert.ToDouble(balance) - Convert.ToDouble(dt.Rows[0]["Product_quantity"]);
                            string upquery = "";
                            upquery = "update tbl_diliverypointstock set Debit='" + allcdebit + "', Balance='" + allbalance + "' where Productid='" + dt.Rows[0]["Product_id"] + "'  and Groupid='" + Session["Groupidh"] + "'";

                            if (db.InsertUpdateDelete(upquery))
                            {
                                activitylog.Activitylogins("tbl_diliverypointstock", db.getmaxid("tbl_diliverypointstock") + "", upquery, "Success", "Insert/Update Success", Session["useridh"] + "", Session["usernameh"] + "", Session["usermailh"] + "");


                                //Stock Transaction query Stock table
                                string query2 = "insert into tbl_producttransaction(ProductId,ProductName,Quntity,Ttype,logname,logid,Date_time,BranchName,BranchCode,Yardid,Yardname,Yardrowid) values('" + dt.Rows[0]["Product_id"] + "','" + dt.Rows[0]["Product_name"] + "','" + dt.Rows[0]["Product_quantity"] + "','Dr.','" + Session["usernameh"] + "','" + Session["useridh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["Yardidh"] + "','" + Session["Yardnameh"] + "','" + Session["Yardrowidh"] + "')";

                                db.InsertUpdateDelete(query2);

                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_diliverypointstock", db.getmaxid("tbl_diliverypointstock") + "", upquery, "Failed", "Insert/Update Failed", Session["useridh"] + "", Session["usernameh"] + "", Session["usermailh"] + "");
                                ViewBag.msg = "Data Not Updated";
                            }
                        }

                        ViewBag.msg = "Data Updated";

                        string Gstin = "";

                        
                        res = new string[11] { "Success", hOrderid, hOrderid2, dt.Rows[0]["Invoiceid"] + "", dt.Rows[0]["id"] + "", dt.Rows[0]["Customer_mobile"] + "", "", dt.Rows[0]["Customer_name"] + "", Session["useridh"] + "", Session["usernameh"] + "", Session["empridh"] + "" };
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_order", hOrderid, query, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        ViewBag.msg = "Insert Failed";
                        res = new string[6] { "Failed", hOrderid, hOrderid2, dt.Rows[0]["Invoiceid"] + "", dt.Rows[0]["id"] + "", dt.Rows[0]["Customer_mobile"] + "" };
                    }
                }
                else
                {
                    res = new string[6] { "Invalid OTP", hOrderid, hOrderid2, "", "", "" };
                    ViewBag.msg = "Invalid OTP";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", ""), errorName.Replace("'", ""), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                res = new string[6] { "Error", hOrderid, hOrderid2, "", "", "" };
            }
            finally
            {
                db.connectionstate();
            }
            string content = string.Join(",", res); // Convert string array to a single comma-separated string
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult shiporder(string hOrderid, string hOrderid2, string driverid, string vehicleid,string drivername,string drivercontact,string remark)
        {
            try
            {
                string Query = @"select * from tbl_order_summary where Order_id='" + hOrderid + "' order by Id desc";
                DataTable sdt = db.GetAllRecord(Query);
                if (sdt.Rows.Count > 0)
                {
                    DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                    // Extract the year and month from the current date
                    string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;

                    string nVehicleid = "V" + currentYearmonth + "" + arandom(5);

                    string squery = "select * from tbl_order where Order_id='" + hOrderid + "' and OrderID2='" + hOrderid2 + "' and Status='Active'";
                    DataTable dt = db.GetAllRecord(squery);

                    if (dt.Rows.Count > 0)
                    {
                        string expecteddilivery = dt.Rows[0]["ExpDelivery_date"] + "";
                        if (expecteddilivery != "")
                        {

                            string upquery = "";

                            if (driverid == "Other")
                            {
                                driverid = driverid;
                                drivername = drivername;
                                drivercontact = drivercontact;
                            }
                            else
                            {
                                string queryd = "select * from tbl_registration where Employee_id='" + driverid + "'";
                                activitylog.Activitylogins("tbl_registration", driverid + "", queryd, "Failed", "", Session["useridg"] + "", Session["usernameg"] + "", Session["usermailg"] + "");
                                DataTable dtd = db.GetAllRecord(queryd);
                                activitylog.Activitylogupd("Success", "");
                                if (dtd.Rows.Count > 0)
                                {
                                    driverid = driverid;
                                    drivername = dtd.Rows[0]["Name"] + "";
                                    drivercontact = dtd.Rows[0]["Mobile_no"] + "";
                                }
                            }
                            string vehiclename = "";
                            string queryv = "select * from tbl_vehicle where Vehicleid='" + vehicleid + "'";
                            activitylog.Activitylogins("tbl_vehicle", vehicleid + "", queryv, "Failed", "", Session["useridg"] + "", Session["usernameg"] + "", Session["usermailg"] + "");
                            DataTable dtv = db.GetAllRecord(queryv);
                            activitylog.Activitylogupd("Success", "");
                            if (dtv.Rows.Count > 0)
                            {
                                vehiclename = dtv.Rows[0]["Vehicle_name"] + " - " + dtv.Rows[0]["Registration_no"] + "";
                                nVehicleid = vehicleid;

                                //drivername = dtv.Rows[0]["Driver_name"] + "";
                                //drivermob = dtv.Rows[0]["Driver_contact"] + "";
                            }


                            //DateTime dateTimeValue = DateTime.Parse(dt1.Rows[0]["Datetime"] + "");
                            //DateTime Confirm_date = DateTime.Parse(dt1.Rows[0]["Confirm_date"] + "");

                            string query = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[Product_type],[OTP],[ExpDelivery_date],[Remark],OrderID2,Points,Permanent_address,Tahsil,Block,Address,State,District)VALUES('" + dt.Rows[0]["Order_id"] + "','" + dt.Rows[0]["Product_xml"] + "','" + dt.Rows[0]["Customer_name"] + "','" + dt.Rows[0]["Customer_id"] + "','" + dt.Rows[0]["Customer_mobile"] + "','" + dt.Rows[0]["Customer_address"] + "','" + dt.Rows[0]["Pincode"] + "','" + dt.Rows[0]["Total_amount"] + "','Dispatch','" + dt.Rows[0]["Payment_method"] + "','" + dt.Rows[0]["Payment_upload"] + "','" + dt.Rows[0]["Paid_amount"] + "','" + dt.Rows[0]["Payment_status"] + "','" + dt.Rows[0]["logid"] + "','" + dt.Rows[0]["logname"] + "','" + DateTime.Parse(dt.Rows[0]["Datetime"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[0]["Membertype"] + "','" + dt.Rows[0]["Orderby"] + "','" + dt.Rows[0]["Invoiceid"] + "','" + drivername + "','" + driverid + "','" + drivercontact + "','" + vehiclename + "','" + nVehicleid + "','Shipped','" + DateTime.Parse(dt.Rows[0]["Confirm_date"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','Active','" + Session["usernameg"] + "','" + Session["useridg"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + dt.Rows[0]["BranchName"] + "','" + dt.Rows[0]["BranchCode"] + "','" + dt.Rows[0]["Product_Img"] + "','" + dt.Rows[0]["Product_id"] + "','" + dt.Rows[0]["Product_name"] + "','" + dt.Rows[0]["Product_price"] + "','" + dt.Rows[0]["Product_quantity"] + "','" + dt.Rows[0]["Total_proamount"] + "','" + dt.Rows[0]["Product_type"] + "','" + dt.Rows[0]["OTP"] + "','" + dt.Rows[0]["ExpDelivery_date"] + "','" + remark + "','" + dt.Rows[0]["OrderID2"] + "','" + dt.Rows[0]["Points"] + "','" + dt.Rows[0]["Permanent_address"] + "','" + dt.Rows[0]["Tahsil"] + "','" + dt.Rows[0]["Block"] + "','" + dt.Rows[0]["Address"] + "','" + dt.Rows[0]["State"] + "','" + dt.Rows[0]["District"] + "' )";

                            db.InsertUpdateDelete("update tbl_order set Status='Inactive' where Order_id='" + hOrderid + "' and OrderID2='" + hOrderid2 + "' and id='" + dt.Rows[0]["id"] + "'");
                            if (db.InsertUpdateDelete(query))
                            {
                                activitylog.Activitylogins("tbl_order", hOrderid, query, "Success", "Insert Success", Session["useridg"] + "", Session["usernameg"] + "", Session["usermailg"] + "");
                                ViewBag.msg = "Data Updated";

                               


                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_order", hOrderid, query, "Failed", "Insert Failed", Session["useridg"] + "", Session["usernameg"] + "", Session["usermailg"] + "");
                                ViewBag.msg = "Update Failed";
                            }
                           
                        }
                        else
                        {
                            ViewBag.msg = "Please give expected delivery date first";
                        }

                    }
                    else
                    {
                        ViewBag.msg = "Order Not Found";
                    }

                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["userid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                }

                catch
                {

                }
            }
            finally
            {

                db.connectionstate();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }









        public JsonResult FinalSubmit()
        {
            string msg = "";
            try
            {
                var restableObject = Request.Form["restableObject"];
                var cname = Request.Form["cname"];
                var customerid = Session["Customer_id"];
                var leadid = Session["Leadid"];
                var cnmailuid = Request.Form["cnmailuid"];
                var cnmobile = Request.Form["cnmobile"];
                var shippingaddr = Request.Form["shippingaddr"];
                var shippingpostal = Request.Form["shippingpostal"];
                var shippingblock = Request.Form["shippingblock"];
                var shippingtahsil = Request.Form["shippingtahsil"];
                var shippingdist = Request.Form["shippingdist"];
                var shippingstate = Request.Form["shippingstate"];
                var mail = Request.Form["mail"];
                var totalprice = Request.Form["totalprice"];
                var paymentmethod = Request.Form["paymentmethod"];
                var utrcheck = Request.Form["utrcheck"];
                var chequedate = Request.Form["chequedate"];
                var loc = Request.Form["loc"];
                var latitude = Request.Form["latitude"];
                var longitude = Request.Form["longitude"];

                var Expectedfrom = Request.Form["Expectedfrom"];
                var Expectedto = Request.Form["Expectedto"];
                var expectedRemark = Request.Form["expectedRemark"];

                var driverid = Request.Form["driverid"];
                var vehicleid = Request.Form["vehicleid"];
                var drivername = Request.Form["drivername"];
                var drivercontact = Request.Form["drivercontact"];

                var pproductid = Request.Form["pproductid"];
                var pproductname = Request.Form["pproductname"];
                var pbrandname = Request.Form["pbrandname"];
                var pproductcategory = Request.Form["pproductcategory"];
                var pproductunittype = Request.Form["pproductunittype"];

                // Create a JObject to store data
                JObject jsonObject = new JObject();
                jsonObject["Location"] = loc.Replace("'", "");
                jsonObject["Latitude"] = latitude;
                jsonObject["Longitude"] = longitude;

                // Convert JObject to JSON string
                string jsonlocation = jsonObject.ToString();

                JObject jsonObject11 = new JObject();
                jsonObject11["Productid"] = pproductid;
                jsonObject11["Productname"] = pproductname;
                jsonObject11["Brandname"] = pbrandname;
                jsonObject11["Productcategory"] = pproductcategory;
                jsonObject11["Productunittype"] = pproductunittype;
                jsonObject11["Doubts"] = "No Doubts";


                // Convert JObject to JSON string
                string jsondata11 = jsonObject11.ToString();


                var fullshippingaddress = shippingaddr + "," + shippingblock + "," + shippingtahsil + "," + shippingdist + "," + shippingstate + "," + shippingpostal;
                
                string upcusquery = "";
                string cashamount = "0";
               
                if (cashamount == "")
                {
                    cashamount = "0";
                }
                DataTable dtres = Encryption.ConvertJSONToDataTable(restableObject);
                string productxml = Encryption.ConvertDatatableTo_XML(dtres);

                string Query34 = @"select * from tbl_customer where Customer_id='" + customerid + "'";
                string Full_address = "", Tahsil = "", Block = "", Address = "", State = "", City = "", Postal_code = "";
                DataTable tbll = db.GetAllRecord(Query34);
                if (tbll.Rows.Count > 0)
                {
                    if (tbll.Rows[0]["Status"] + "" == "Lead")
                    {
                        upcusquery = "update tbl_customer set Status='Active' where Customer_id='" + customerid + "'";
                    }
                    Full_address = tbll.Rows[0]["Full_address"] + "";
                    Tahsil = tbll.Rows[0]["Tahsil"] + "";
                    Block = tbll.Rows[0]["Block"] + "";
                    Address = tbll.Rows[0]["Address"] + "";
                    State = tbll.Rows[0]["State"] + "";
                    City = tbll.Rows[0]["City"] + "";
                    Postal_code = tbll.Rows[0]["Postal_code"] + "";

                    try
                    {
                        string queryd = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,OrderDetails) VALUES('" + leadid + "','" + customerid + "','" + tbll.Rows[0]["Name"] + "','" + tbll.Rows[0]["Mobile"] + "','" + tbll.Rows[0]["Email"] + "','" + tbll.Rows[0]["State"] + "','" + tbll.Rows[0]["City"] + "','" + tbll.Rows[0]["Full_address"] + "','" + tbll.Rows[0]["Postal_code"] + "','','','Close','" + jsondata11 + "','No','" + jsonlocation + "','','','','','','','Demo','Done','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','Open','')";


                        if (db.InsertUpdateDelete(queryd))
                        {
                            activitylog.Activitylogins("tbl_leadnew", db.getmaxid("tbl_leadnew").ToString(), queryd, "Success", "Insert Succcess", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());

                            ViewBag.AlertMessage = "Success";

                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_leadnew", "", queryd, "Failed", "Insert Failed", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }

                DateTime currentDate = DateTime.Parse(Session["SelectedDate"] + "");
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string orderid = "O" + currentYearmonth + "" + arandom(5);
                string query = "INSERT INTO tbl_order_summary(Order_id,Product_xml,Customer_name,Customer_id,Customer_mobile,Customer_address,Pincode,Total_amount,Payment_method,Payment_upload,Payment_status,logid,logname,Datetime,Membertype,Orderby,Paid_amount,Utr_Check,[BranchName],[BranchCode])VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + fullshippingaddress + "','" + Postal_code + "','" + totalprice + "','" + paymentmethod + "','','Active','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertypeh"] + "','" + Session["usernameh"] + "(" + Session["userphonh"] + ")','" + cashamount + "','" + utrcheck + "','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "')";
                if (db.InsertUpdateDelete(query))
                {
                    if (dtres.Rows.Count > 0)
                    {
                        string transactionid = "t" + currentYearmonth + "" + arandom(5);
                        string order2 = "";
                        for (int i = 0; i < dtres.Rows.Count; i++)
                        {
                            string Query = @"select * from [tbl_pro_price_circlar] where Id='" + dtres.Rows[i]["Product_Id"] + "' order by Id desc";
                            DataTable dt = db.GetAllRecord(Query);
                            if (dt.Rows.Count > 0)
                            {
                                string orderid2 = arandom(5) + i;
                                string randomres = checkrandom(orderid2, order2);
                                string orderid22 = randomres;

                                string iquery = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[Product_type],[OTP],[ExpDelivery_date],[Remark],OrderID2,Points,Permanent_address,Tahsil,Block,Address,State,District)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + fullshippingaddress + "','" + Postal_code + "','0','Active','','','0','Due','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertypeh"] + "','" + Session["usernameh"] + "(" + Session["userphonh"] + ")','','','','','','','','','Active','" + Session["usernameh"] + "','" + Session["useridh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + dtres.Rows[i]["Product_Img"] + "','" + dtres.Rows[i]["Product_Id"] + "','" + dtres.Rows[i]["Product"] + "','" + dtres.Rows[i]["Price"] + "','" + dtres.Rows[i]["Quantity"] + "','" + dtres.Rows[i]["Total"] + "','" + dtres.Rows[i]["Product_type"] + "','','','','" + orderid22 + "','" + dtres.Rows[i]["Points"] + "','" + Full_address + "','" + Tahsil + "','" + Block + "','" + Address + "','" + State + "','" + City + "')";
                                if (db.InsertUpdateDelete(iquery))
                                {
                                    order2 = orderid2;
                                    msg = "Success";
                                }
                                else
                                {
                                    msg = "Fail";
                                }
                            }
                        }

                        string tquery = "INSERT INTO [dbo].[tbl_order_transaction]([Datetime],[Customer_id],[Order_id],[Amount],[Opening_amount],[Closing_amount],[Transaction_id],[Transaction_by_id],[Tr_status],[Status],[logid],[logname],[log_mac],[log_IP],Payment_slip,Payment_method,Utr_Check,checkdate) VALUES('" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + customerid + "','" + orderid + "','0','0','0','" + transactionid + "','" + Session["useridh"] + "','Due','Requested','" + Session["useridh"] + "','" + Session["usernameh"] + "','Mac','IP','','" + paymentmethod + "','" + utrcheck + "','" + chequedate + "')";
                        db.InsertUpdateDelete(tquery);

                        

                        string res = "";
                        string querylead = "";
                        string hid = "No";
                        string nextstatus = "Close";
                        string Status = "Done";
                        JObject jsonObject1 = new JObject();
                        jsonObject1["Productdetail"] = productxml;
                        // Convert JObject to JSON string
                        string jsondata1 = jsonObject1.ToString();

                        string query1 = "SELECT * FROM tbl_customer  WHERE Customer_id = '" + customerid + "'";
                        activitylog.Activitylogins("tbl_customer", "", query1, "Failed", "", Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                        DataTable dtlead = db.GetAllRecord(query1);
                        activitylog.Activitylogupd("Success", "");
                        if (dtlead.Rows.Count > 0)
                        {
                            if (hid == "Yes")
                            {
                                querylead = "Update [tbl_leadnew] Set [Customerid]='" + customerid + "' ,[Name]='" + dtlead.Rows[0]["Name"] + "' ,[Mobile]='" + dtlead.Rows[0]["Mobile"] + "' ,[Email]='" + dtlead.Rows[0]["Email"] + "' ,[Statename]='" + dtlead.Rows[0]["State"] + "' ,[Cityname]='" + dtlead.Rows[0]["City"] + "' ,[Address]='" + dtlead.Rows[0]["Full_address"] + "' ,[Pincode]='" + dtlead.Rows[0]["Postal_code"] + "' ,[Occupation]='' ,[Land_Irrigation]='' ,[Taken_time]='' ,[Lead_status]='" + Status + "',Nextstatus='" + nextstatus + "',[Status]='Active' ,[BranchName]='" + Session["ubrnameh"] + "' ,[BranchCode]='" + Session["ubrcodeh"] + "' ,[Logid]='" + Session["useridh"] + "' ,[Logname]='" + Session["usernameh"] + "' ,[releavedatetime]='" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "',XML='" + jsondata1 + "',OrderDetails='' where id='" + Session["Rowid"] + "'";


                            }
                            else
                            {
                                querylead = "INSERT INTO [dbo].[tbl_leadnew] ([Leadid] ,[Customerid] ,[Name] ,[Mobile] ,[Email] ,[Statename] ,[Cityname] ,[Address] ,[Pincode] ,[Occupation] ,[Land_Irrigation] ,[Support_reqstatus] ,[XML] ,[Support] ,[Location_In] ,[Supportempname] ,[Supportempid] ,[Supportemprid] ,[Supportlocation_In] ,[Location_diffrence] ,[Taken_time] ,[Lead_status_for] ,[Lead_status] ,[Status] ,[BranchName] ,[BranchCode] ,[Logid] ,[Logname] ,[Datetime],Nextstatus,OrderDetails) VALUES('" + leadid + "','" + customerid + "','" + dtlead.Rows[0]["Name"] + "','" + dtlead.Rows[0]["Mobile"] + "','" + dtlead.Rows[0]["Email"] + "','" + dtlead.Rows[0]["State"] + "','" + dtlead.Rows[0]["City"] + "','" + dtlead.Rows[0]["Full_address"] + "','" + dtlead.Rows[0]["Postal_code"] + "','','','Close','" + jsondata1 + "','No','" + jsonlocation + "','','','','','','','Order','" + Status + "','Active','" + Session["ubrnameh"] + "','" + Session["ubrcodeh"] + "','" + Session["useridh"] + "','" + Session["usernameh"] + "','" + DateTime.Parse(Session["SelectedDate"] + "").ToString("yyyy-MM-dd HH:mm:ss") + "','" + nextstatus + "','')";

                            }

                            if (db.InsertUpdateDelete(querylead))
                            {
                                msg = "Success";
                                if (tbll.Rows[0]["Status"] + "" == "Lead")
                                {
                                    db.InsertUpdateDelete(upcusquery);
                                }


                                try
                                {
                                    ConfirmPayment(orderid, totalprice, "No Due", totalprice);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    Expecteddilivery(orderid, Expectedfrom, Expectedto, expectedRemark);
                                }
                                catch
                                {

                                }
                                try
                                {
                                    string shipremark = "";
                                    string squery = "select * from tbl_order where Order_id='" + orderid + "' and Status='Active'";
                                    DataTable dt = db.GetAllRecord(squery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            shiporder(orderid, dt.Rows[i]["OrderID2"]+"", driverid, vehicleid, drivername, drivercontact, shipremark);
                                        }
                                    }
                                    else
                                    {
                                        res = "Order Not Found";
                                    }
                                    
                                }
                                catch
                                {

                                }
                                try
                                {
                                    string squery = "select * from tbl_order where Order_id='" + orderid + "' and Status='Active'";
                                    DataTable dt = db.GetAllRecord(squery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            Dispatchorder("", orderid, dt.Rows[i]["OrderID2"]+"", dt.Rows[i]["OTP"]+"", dt.Rows[i]["Product_quantity"]+"", dt.Rows[i]["Product_price"]+"", "0", "0" );
                                        }
                                    }
                                    else
                                    {
                                        res = "Order Not Found";
                                    }

                                }
                                catch
                                {

                                }



                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_leadnew", "", query, "Failed", "Insert Failed", Session["userid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                            }
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        msg = "";
                    }

                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["useridh"].ToString(), Session["usernameh"].ToString(), Session["usermailh"].ToString());
                }

                catch
                {

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

     
        [HttpPost]
        public ActionResult StoreDateInSession(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                Session["SelectedDate"] = parsedDate.ToString();

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Invalid date format." });
            }
        }



        public ActionResult NewCanAppli(string i)
        {
            ViewBag.id = i;
            return View();
        }
        [HttpPost]
        public ActionResult NewCanAppli(FormCollection form, HttpPostedFileBase canimage, string prevAttachement, string hfile, string canmobile, string photoview, HttpPostedFileBase upresume)
        {
            string msg = "";
            var restableObject = Request.Form["restableObject"];
            var restableObject2 = Request.Form["restableObject2"];
            var hid = Request.Form["hid"];
            try
            {
                string productxml2 = "";
                DataTable dtres = Encryption.ConvertJSONToDataTable(restableObject);
                if (restableObject2 != "null" || restableObject2 != "")
                {
                    DataTable dtres2 = Encryption.ConvertJSONToDataTable2(restableObject2);
                    productxml2 = Encryption.ConvertDatatableTo_XML(dtres2);
                }
                string productxml = Encryption.ConvertDatatableTo_XML(dtres);



                string fileName = ""; string str = "", str2 = ""; string fpth = ""; string qrpth = "";
                string acode = Session["auid"] + "";
                string auname = Session["auname"] + "";

                hid = form["hid"];
                if (hid != "" && hid != null)
                {
                    string fdpath = "";
                    if (canimage != null && canimage.ContentLength > 0)
                    {
                        fdpath = APIs.Candidatedocs(canimage, "cnim");
                    }
                    string resume = "";
                    if (upresume != null && upresume.ContentLength > 0)
                    {
                        resume = APIs.Candidatedocs(upresume, "rsm");
                    }
                    if (dtres.Rows.Count > 0)
                    {
                        string query = "update tbl_HRM set Appliedfor='" + form["Appliedfor"] + "', canname='" + form["canname"] + "', cangardian='" + form["cangardian"] + "', canmail='" + form["canmail"] + "', canmobile='" + form["canmobile"] + "', canage='" + form["canage"] + "', cangender='" + form["cangender"] + "', canmaritalstatus='" + form["canmaritalstatus"] + "', canNationality='" + form["canNationality"] + "', canReligion='" + form["canReligion"] + "', canimage='" + fdpath + "', Pcity='" + form["Pcity"] + "', Ppincode='" + form["Ppincode"] + "', Pstate='" + form["Pstate"] + "', Pcountry='" + form["Pcountry"] + "', Tcity='" + form["Tcity"] + "', Tcountry='" + form["Tcountry"] + "', Tstate='" + form["Tstate"] + "', XML_qualification= '" + productxml + "', Tpincode='" + form["Tpincode"] + "', logname='" + Session["logname"] + "', logid='" + Session["logid"] + "', candob='" + form["candob"] + "',DateTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',XML_precompany='" + productxml2 + "',vehicle='" + form["vehicle"] + "',Licenceno='" + form["Licenceno"] + "',reference='" + form["reference"] + "',referalName='" + form["referalName"] + "',referalMob='" + form["referalMob"] + "',upresume='" + resume + "',workStatus='Filled',status='Active',Vehicleno='" + form["Vehicleno"] + "',Jobtittlefor='" + form["Jobtittlefor"] + "' where Id='" + hid + "'";
                        if (db.InsertUpdateDelete(query))
                        {
                            string Message = "Hi " + form["canname"] + " , Your Job Application form has been submitted successfully. You will get the message for the scheduled interview date and time soon at Growfast.";
                            string[] replacementValues = { form["canname"] };

                            // Messaging.SendSMS(canmobile, replacementValues, Message, form["canname"]);

                            Messaging.SendWhatsappSMSNew1(canmobile, "Application_form_submit", form["canname"], Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", "", "", replacementValues, true);
                            msg = "Success";
                        }
                        else
                        {
                            msg = "Fail";
                        }
                    }
                }
                else
                {
                    ViewBag.msg = "Error";

                }
            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", ""), errorName.Replace("'", ""), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }
                catch
                {
                    msg = "Please check Url";
                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Content(msg);

        }
        public ActionResult Uploaddocs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Uploaddocs(FormCollection form, HttpPostedFileBase canimage, string canmobile, string photoview, HttpPostedFileBase upresume, HttpPostedFileBase highsch, HttpPostedFileBase Inter, HttpPostedFileBase Gradmrk, HttpPostedFileBase aadhar, HttpPostedFileBase pan, HttpPostedFileBase releiving, HttpPostedFileBase letter, HttpPostedFileBase signed)
        {
            string msg = "";
            var hid = Request.Form["hid"];
            try
            {

                string fdpath = "";
                if (canimage != null && canimage.ContentLength > 0)
                {
                    fdpath = APIs.Candidatedocs(canimage, "cnim");
                }
                string highschmar = "";

                if (highsch != null && highsch.ContentLength > 0)
                {
                    highschmar = APIs.Candidatedocs(highsch, "rsm");
                }
                string Intermediate = "";
                if (Inter != null && Inter.ContentLength > 0)
                {
                    Intermediate = APIs.Candidatedocs(Inter, "rsm");
                }
                string Gradmrksht = "";
                if (Gradmrk != null && Gradmrk.ContentLength > 0)
                {
                    Gradmrksht = APIs.Candidatedocs(Gradmrk, "rsm");
                }
                string aadharcd = "";
                if (aadhar != null && aadhar.ContentLength > 0)
                {
                    aadharcd = APIs.Candidatedocs(aadhar, "rsm");
                }
                string Pan = "";
                if (pan != null && pan.ContentLength > 0)
                {
                    Pan = APIs.Candidatedocs(pan, "rsm");
                }
                string releivingdoc = "";
                if (releiving != null && releiving.ContentLength > 0)
                {
                    releivingdoc = APIs.Candidatedocs(releiving, "rsm");
                }
                string Lettercn = "";
                if (letter != null && letter.ContentLength > 0)
                {
                    Lettercn = APIs.Candidatedocs(letter, "rsm");
                }
                string signedcn = "";
                if (signed != null && signed.ContentLength > 0)
                {
                    signedcn = APIs.Candidatedocs(signed, "rsm");
                }

                string query = " insert into tbl_docsonboarding (canname,canmail,canmobile,Jobtittlefor,joiningdate,canimage,highsch,Inter,Gradmrk,aadhar,pan,releiving,letter,signed,logid,logname,logmail) values (" + form["canname"] + "," + form["canmail"] + "," + form["canmobile"] + "," + form["Jobtittlefor"] + "," + form["joiningdate"] + "," + fdpath + "," + highschmar + "," + Intermediate + "," + Gradmrksht + "," + aadharcd + "," + Pan + "," + releivingdoc + "," + Lettercn + "," + signedcn + "," + form["canmobile"] + "," + form["canname"] + "," + form["canmail"] + ");";
                if (db.InsertUpdateDelete(query))
                {
                    msg = "Success";
                }
                else
                {
                    msg = "Fail";
                }


            }
            catch (Exception ex)
            {
                try
                {
                    Error_15_16 error_15_16 = new Error_15_16();
                    string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
                    // Get the page URL, if available
                    pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    // Get the module name
                    moduleName = ex.TargetSite.Module.Name;
                    // Get the error line number, if available
                    var stackTrace = ex.StackTrace;
                    if (!string.IsNullOrEmpty(stackTrace))
                    {
                        var lineNumberIndex = stackTrace.LastIndexOf(":line ");
                        if (lineNumberIndex >= 0)
                        {
                            var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
                            var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
                            if (newLineIndex >= 0)
                            {
                                errorLine = lineNumber.Substring(0, newLineIndex);
                            }
                            else
                            {
                                errorLine = lineNumber;
                            }
                        }
                    }
                    // Get the error message and name
                    if (ex.Message.ToString().Length >= 1000)
                    {
                        errorMessage = ex.Message.Substring(1, 500);
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    errorName = ex.GetType().FullName;
                    // Get the error trace
                    errorTrace = ex.StackTrace;
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }
                catch
                {
                    ViewBag.msg = "Please check Url";
                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return RedirectToAction("NewCanAppli");
        }

    }
}