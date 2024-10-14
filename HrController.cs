using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace Growfast.Controllers
{
    public class HrController : Controller
    {
        // GET: Hr

        DbManager db = new DbManager();
        EncryptDecrypt ed = new EncryptDecrypt();
        Activitylog activitylog = new Activitylog();
        Messaging message = new Messaging();
        // GET: Client
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Index()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Employee()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult AttendanceInfo(string type, string date, string locationId, string subLocationId, string departmentId, string subDepartmentId, string designationId, string shiftId, string areaId)
        {
            List<Attendanceinfo> Attendanceinfo = new List<Attendanceinfo>();
            try
            {
                ViewBag.nm = type;
                string query = "";
                if (type == "in")
                {
                    query = "SELECT * FROM tbl_attendance WHERE Punchin_time IS NOT NULL AND DATE='" + date + "' and Branchcode='" + Session["abrcode"] + "'";

                }
                else if (type == "out")
                {
                    query = "SELECT * FROM tbl_attendance WHERE Punchout_time IS NOT NULL AND DATE='" + date + "' and Branchcode='" + Session["abrcode"] + "'";
                }
                activitylog.Activitylogins("tbl_attendance", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        Attendanceinfo attendanceinfo = new Attendanceinfo
                        {
                            id = Convert.ToInt32(row["id"]),
                            Emprowid = Convert.ToString(row["Emprowid"]),
                            Employeeid = Convert.ToString(row["Employeeid"]),
                            Name = Convert.ToString(row["Name"]),
                            Department = Convert.ToString(row["Department"]),
                            Designation = Convert.ToString(row["Designation"]),
                            Shiftname = Convert.ToString(row["Shiftname"]),
                            Starttime = Convert.ToString(row["Starttime"]),
                            Endtime = Convert.ToString(row["Endtime"]),
                            date = Convert.ToString(row["Date"]),
                            punchintime = Convert.ToString(row["Punchin_time"]),
                            punchouttime = Convert.ToString(row["Punchout_time"]),
                            Workhours = Convert.ToString(row["Working_hours"]),
                            punchinlocation = Convert.ToString(row["Punchin_location"]),
                            punchoutlocation = Convert.ToString(row["Punchout_location"]),
                        };

                        Attendanceinfo.Add(attendanceinfo);
                    }
                }
                else
                {
                    ViewBag.msg = "failed";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View(Attendanceinfo);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult EmployeeInfo(string type, string date, string locationId, string subLocationId, string departmentId, string subDepartmentId, string designationId, string shiftId, string areaId)
        {
            List<EmployeeInfo> EmployeeInfo = new List<EmployeeInfo>();
            try
            {
                ViewBag.nm = type;
                string query = "";
                if (type == "all")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "'  where  tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                }
                else if (type == "active")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE  tbl_registration.Status='Approved' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                else if (type == "inactive")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE  tbl_registration.Status!='Approved' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                else if (type == "permanent")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE  tbl_registration.Status='Approved'and tbl_registration.Employeementtype='Premanent' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                else if (type == "provision")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE  tbl_registration.Status='Approved' and tbl_registration.Employeementtype='Provisonal' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                else if (type == "present")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE tbl_attendance.Emprowid IS NOT NULL  and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                else if (type == "absent")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_attendance.Date = '" + date + "' WHERE tbl_attendance.Emprowid IS NULL  and tbl_registration.Branchcode='" + Session["abrcode"] + "' and tbl_registration.Status='Approved'";
                }
                else if (type == "late-comers")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND DATEADD(MINUTE, 10, tbl_registration.Shiftstarttime) <= tbl_attendance.Punchin_time and  tbl_attendance.Date = '" + date + "' WHERE tbl_attendance.Emprowid IS NOT NULL and tbl_registration.Branchcode='" + Session["abrcode"] + "' ";
                }
                else if (type == "early-leavers")
                {
                    query = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND tbl_registration.Shiftendtime > tbl_attendance.Punchout_time and tbl_attendance.Date = '" + date + "' WHERE tbl_attendance.Emprowid IS NOT NULL and tbl_registration.Branchcode='" + Session["abrcode"] + "'";
                }
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        EmployeeInfo employeeInfo = new EmployeeInfo
                        {
                            id = Convert.ToInt32(row["id"]),
                            Employeeid = Convert.ToString(row["Employee_id"]),
                            Name = Convert.ToString(row["Name"]),
                            Department = Convert.ToString(row["Department_name"]),
                            Designation = Convert.ToString(row["Designation"]),
                            Managername = Convert.ToString(row["Managername"]),
                            Email = Convert.ToString(row["Email"]),
                            Premice = Convert.ToString(row["Premises"]),
                            //Location = Convert.ToString(row["Location"]),
                            punchintime = Convert.ToString(row["Punchin_time"]),
                            punchouttime = Convert.ToString(row["Punchout_time"]),
                            Status = Convert.ToString(row["Status"]),
                            imagepath = Convert.ToString(row["Employeeimage"]),
                        };

                        EmployeeInfo.Add(employeeInfo);
                    }
                }
                else
                {
                    ViewBag.msg = "failed";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View(EmployeeInfo);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult View(int id, string name)
        {
            List<regmastermodel> registrations = new List<regmastermodel>();
            try
            {
                ViewBag.nm = name;
                string query = "";
                if (name == "department")
                {
                    query = "SELECT * FROM tbl_registration LEFT JOIN tbl_department ON tbl_registration.Department_name = tbl_department.Departmant WHERE tbl_department.Id='" + id + "' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                }
                else if (name == "designation")
                {
                    query = "SELECT * FROM tbl_registration LEFT JOIN tbl_designation ON tbl_registration.Designation = tbl_designation.Designation WHERE tbl_designation.Id='" + id + "' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                }
                else if (name == "subdepartment")
                {
                    query = "SELECT * FROM tbl_registration LEFT JOIN tbl_subdepartment ON tbl_registration.Subdepartment = tbl_subdepartment.Subdepartmant WHERE tbl_subdepartment.Id='" + id + "' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                }
                else if (name == "shift")
                {
                    query = "SELECT * FROM tbl_registration LEFT JOIN tbl_shift ON tbl_registration.Shiftname = tbl_shift.Shiftname WHERE tbl_shift.Id='" + id + "' and tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                }
                activitylog.Activitylogins("tbl_registration,tbl_" + name + "", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];

                        regmastermodel registration = new regmastermodel
                        {
                            Employeeid = Convert.ToString(row["Employee_id"]),
                            Name = Convert.ToString(row["Name"]),
                            Department = Convert.ToString(row["Department_name"]),
                            Subdepartment = Convert.ToString(row["Subdepartment"]),
                            Shift = Convert.ToString(row["Shifttiming"]),
                            Designation = Convert.ToString(row["Designation"]),
                            Status = Convert.ToString(row["Status"]),
                        };

                        registrations.Add(registration);
                    }
                }
                else
                {
                    ViewBag.msg = "failed";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View(registrations);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Attendance()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult GetAttendance(string Start_date, string End_date, string Department, string Empnm, string Shift, string Designation)
        {
            string res = "", tbldata = "", tbl = "", json = "";
            try
            {
                DateTime startdate = DateTime.ParseExact(Start_date, "d/M/yyyy", null);
                string formatteStart_date = startdate.ToString("yyyy-MM-dd");
                DateTime enddate = DateTime.ParseExact(End_date, "d/M/yyyy", null);
                string formattedEnd_date = enddate.ToString("yyyy-MM-dd");

                //string squery = "select * from tbl_attendance where (CONVERT(DATE, Date)>='" + formatteStart_date + "' and CONVERT(DATE, Date)<='" + formattedEnd_date + "') and tbl_attendance.Branchcode='" + Session["abrcode"] + "'";

                string squery = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid  WHERE tbl_attendance.Emprowid IS NOT NULL and (CONVERT(DATE, tbl_attendance.Date)>='" + formatteStart_date + "' and CONVERT(DATE, tbl_attendance.Date)<='" + formattedEnd_date + "') and tbl_registration.Branchcode='" + Session["abrcode"] + "'";

                //string squery = "SELECT * FROM tbl_registration LEFT OUTER JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid WHERE tbl_attendance.Emprowid IS NULL and (CONVERT(DATE, tbl_attendance.Date)>='" + formatteStart_date + "' and CONVERT(DATE, tbl_attendance.Date)<='" + formattedEnd_date + "')  and tbl_registration.Branchcode='" + Session["abrcode"] + "' and tbl_registration.Status='Approved'";

                if (!string.IsNullOrEmpty(Department))
                {
                    squery += " AND tbl_attendance.Department = '" + Department + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND tbl_attendance.Emprowid = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Designation))
                {
                    squery += " AND tbl_attendance.Designation = '" + Designation + "'";
                }
                if (!string.IsNullOrEmpty(Shift))
                {
                    squery += " AND tbl_attendance.Shiftname = '" + Shift + "'";
                }
                activitylog.Activitylogins("tbl_attendance", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public string GeneratedEmpcode()
        {

            return "employeeid";
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult AddNewEmployee(string Emp,string t,string Id)
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {
                try
                {
                    ViewBag.mainid = Emp;
                    //string hid = ed.Decrypt(Emp, true);
                    string hid = Emp;
                    ViewBag.id = hid;
                    ViewBag.t = Id;
                    if (t == "n")
                    {
                        string lastnumstr = "";
                        int lastnum = db.getmaxidnumber("tbl_registration", Session["Companyidm"] + "");
                        if (lastnum >= 0 && lastnum < 10)
                        {
                            lastnumstr = "0000" + (lastnum + 1);
                        }
                        else if (lastnum >= 10 && lastnum < 100)
                        {
                            lastnumstr = "000" + (lastnum + 1);
                        }
                        else if (lastnum >= 100 && lastnum < 1000)
                        {
                            lastnumstr = "00" + (lastnum + 1);
                        }
                        else if (lastnum >= 1000 && lastnum < 10000)
                        {
                            lastnumstr = "0" + (lastnum + 1);
                        }
                        else
                        {
                            lastnumstr = "" + (lastnum + 1);
                        }
                        string originalString = Session["Companyprefixm"] + ""; // Example input string
                        // Store in two variables
                        string prefix = originalString.Substring(0, 6);
                        string lastDigit = originalString.Substring(originalString.Length - 1);
                        string employeeid = prefix + "" + lastnumstr + "" + lastDigit;
                        ViewBag.empcode = employeeid;
                        ViewBag.Idnumber = lastnumstr;


                        ViewBag.head = "Add New";
                    }
                    else {
                        if (hid == "0")
                        {
                            string lastnumstr = "";
                            int lastnum = db.getmaxidnumber("tbl_registration", Session["Companyidm"] + "");
                            if (lastnum >= 0 && lastnum < 10)
                            {
                                lastnumstr = "0000" + (lastnum + 1);
                            }
                            else if (lastnum >= 10 && lastnum < 100)
                            {
                                lastnumstr = "000" + (lastnum + 1);
                            }
                            else if (lastnum >= 100 && lastnum < 1000)
                            {
                                lastnumstr = "00" + (lastnum + 1);
                            }
                            else if (lastnum >= 1000 && lastnum < 10000)
                            {
                                lastnumstr = "0" + (lastnum + 1);
                            }
                            else
                            {
                                lastnumstr = "" + (lastnum + 1);
                            }
                            string originalString = Session["Companyprefixm"] + ""; // Example input string
                                                                                    // Store in two variables
                            string prefix = originalString.Substring(0, 6);
                            string lastDigit = originalString.Substring(originalString.Length - 1);
                            string employeeid = prefix + "" + lastnumstr + "" + lastDigit;
                            ViewBag.empcode = employeeid;
                            ViewBag.Idnumber = lastnumstr;


                            ViewBag.head = "Add New";
                        }
                        else
                        {
                            ViewBag.head = "Update Employee";
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
                        error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
            }
            else
            {
                Response.Redirect("/Home/Login");
            }

            return View();
        }
        public JsonResult GetEmployee(string Hid)
        {
            string json = null;
            try
            {
                string query = "select * from tbl_registration where Id=" + Hid + "";
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(dt, formatting: Formatting.None);
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewEmployee(FormCollection form)
        {
            try
            {
                int nowbalanceleave;
                string fileName = ""; string str = "", str2 = ""; string fpth = ""; string qrpth = "";
                string acode = Session["auid"] + "";
                string auname = Session["auname"] + "";
                GenQRCode qr = new GenQRCode();

                string hid = form["hid"];

                if (hid != "0" && hid != null && hid != "")
                {
                    string res = qr.QRCode(form["empcode"], form["empname"], form["departmentname"], form["empmail"], form["empmobile"], "GROWFAST ORGANIC DIAMOND PVT. LTD.", form["designationname"], form["emerconmob"], form["emerconrel"], form["emerconname"], "RS Plaza Level near 2 Jagrani hospital", form["empaddress"]);
                    if (res != null && res != "")
                    {
                        qrpth = res;
                    }
                    else
                    {
                        qrpth = form["qrpth"];
                    }

                    string selquery = "select Totalleave,Balanceleave from tbl_registration where Id='" + form["hid"] + "'";
                    DataTable sdt = db.GetAllRecord(selquery);
                    if (sdt.Rows.Count > 0)
                    {
                        int gettotalleave, getbalanceleave, nowaddleave;
                        gettotalleave = Convert.ToInt32(sdt.Rows[0]["Totalleave"]);
                        getbalanceleave = Convert.ToInt32(sdt.Rows[0]["Balanceleave"]);

                        nowaddleave = Convert.ToInt32(form["totalleave"]) - gettotalleave;
                        nowbalanceleave = getbalanceleave + nowaddleave;
                    }
                    else
                    {
                        nowbalanceleave = Convert.ToInt32(form["totalleave"]);
                    }
                    // string query = "Update tbl_registration set Name='" + form["empname"] + "',Email='" + form["empmail"] + "',Mobile_no='" + form["empmobile"] + "',Employee_Type='" + form["usertype"] + "', Department_name='" + form["departmentname"] + "', Designation='" + form["designationname"] + "', Status='Approved',Type='user', Gardianname='" + form["empgardian"] + "', Address='" + form["empaddress"] + "', Temporaryaddress='" + form["emptempaddress"] + "', Alternatemobile='" + form["empaltmobile"] + "', Dateofbirth='" + form["empdob"] + "', Gender='" + form["empgender"] + "', Maritalstatus='" + form["maritalstatus"] + "', Premises='" + form["premises"] + "', Bloodgroup='" + form["bloodgroup"] + "', Departmentid='" + form["departmentid"] + "', Managername='" + form["managername"] + "', Managercode='" + form["managerid"] + "', Employeementtype='" + form["employeementtype"] + "', Bankname='" + form["bankname"] + "', Accountnumber='" + form["accountno"] + "', Accountholdername='" + form["accholdernm"] + "', IFSC='" + form["ifsc"] + "', PFNo='" + form["pfno"] + "', UANNo='" + form["uanno"] + "', ESIC='" + form["esic"] + "', Paymentmode='" + form["paymentmode"] + "', DOL='" + form["dol"] + "', PAN='" + form["panno"] + "', Emerconname='" + form["emerconname"] + "', Emerconmobile='" + form["emerconmob"] + "', Emerconrelation='" + form["emerconrel"] + "',Shiftid='" + form["shift"] + "' ,Shifttiming='" + form["shifttiming"] + "',Shiftname= '" + form["shiftname"] + "',Shiftstarttime= '" + form["shiftstarttime"] + "',Shiftendtime= '" + form["shiftendtime"] + "', Activatecheckin='" + form["checkinmode"] + "', Dateofjoining='" + form["doj"] + "',Nofchild='" + form["nofchild"] + "',Citizenship='" + form["citizen"] + "',Flexigours='" + form["flexihrs"] + "',QRpath='" + qrpth + "',Date_time='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Basicsalary= '" + form["basicsalary"] + "',Houserentallowance= '" + form["houserentallow"] + "',Conveyanceallowance = '" + form["conveyanceallow"] + "',Medicalallowance = '" + form["medicalallow"] + "',Specialallowance = '" + form["specialallow"] + "',Grosssalary= '" + form["grosssalary"] + "',EPF= '" + form["epf"] + "',HealthInsurance = '" + form["healthensurance"] + "',Professionaltax = '" + form["professionaltax"] + "',TDS = '" + form["tds"] + "',Totaldeduction = '" + form["totaldeduction"] + "',Netpay = '" + form["netpay"] + "',Personalleave='" + form["perleave"] + "',Casualleave = '" + form["casleave"] + "',Sickleave = '" + form["sickleave"] + "',OtherLeave = '" + form["othleave"] + "',Totalleave='" + form["totalleave"] + "',Balanceleave='" + nowbalanceleave + "',designation_order='" + form["designationorder"] + "',designationid='" + form["designationid"] + "' where Id='" + hid + "'";
                    string query = "Update tbl_registration set Name='" + form["empname"] + "',Email='" + form["empmail"] + "',Mobile_no='" + form["empmobile"] + "',Employee_Type='" + form["usertype"] + "', Department_name='" + form["departmentname"] + "', Designation='" + form["designationname"] + "', Status='Approved',Type='user', Gardianname='" + form["empgardian"] + "', Address='" + form["empaddress"] + "', Temporaryaddress='" + form["emptempaddress"] + "', Alternatemobile='" + form["empaltmobile"] + "', Dateofbirth='" + form["empdob"] + "', Gender='" + form["empgender"] + "', Maritalstatus='" + form["maritalstatus"] + "', Premises='" + form["premises"] + "', Bloodgroup='" + form["bloodgroup"] + "', Departmentid='" + form["departmentid"] + "', Managername='" + form["managername"] + "', Managercode='" + form["managerid"] + "', Employeementtype='" + form["employeementtype"] + "', Bankname='" + form["bankname"] + "', Accountnumber='" + form["accountno"] + "', Accountholdername='" + form["accholdernm"] + "', IFSC='" + form["ifsc"] + "', PFNo='" + form["pfno"] + "',UANNo='" + form["uanno"] + "',ESIC='" + form["esic"] + "', Paymentmode='" + form["paymentmode"] + "', DOL='" + form["DOL"] + "', PAN='" + form["PAN"] + "', Emerconname='" + form["emerconname"] + "', Emerconmobile='" + form["emerconmob"] + "', Emerconrelation='" + form["emerconrel"] + "',Shiftid='" + form["shift"] + "' ,Shifttiming='" + form["shifttiming"] + "',Shiftname= '" + form["shiftname"] + "',Shiftstarttime= '" + form["shiftstarttime"] + "',Shiftendtime= '" + form["shiftendtime"] + "', Activatecheckin='" + form["Activatecheckin"] + "', Dateofjoining='" + form["doj"] + "',Nofchild='" + form["Nofchild"] + "',Citizenship='" + form["citizen"] + "',Flexigours='" + form["flexihrs"] + "',QRpath='" + qrpth + "',Date_time='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Basicsalary= '" + form["basicsalary"] + "',Houserentallowance= '" + form["houserentallow"] + "',Conveyanceallowance = '" + form["conveyanceallow"] + "',Medicalallowance = '" + form["medicalallow"] + "',Specialallowance = '" + form["specialallow"] + "',Grosssalary= '" + form["grosssalary"] + "',EPF= '" + form["epf"] + "',HealthInsurance = '" + form["healthensurance"] + "',Professionaltax = '" + form["professionaltax"] + "',TDS = '" + form["tds"] + "',Totaldeduction = '" + form["totaldeduction"] + "',Netpay = '" + form["netpay"] + "',Personalleave='" + form["perleave"] + "',Casualleave = '" + form["casleave"] + "',Sickleave = '" + form["sickleave"] + "',OtherLeave = '" + form["othleave"] + "',Totalleave='" + form["totalleave"] + "',Balanceleave='" + nowbalanceleave + "',designation_order='" + form["designationorder"] + "',designationid='" + form["designationid"] + "',Aadhar_no='" + form["Aadhar_no"] + "',emptempPincode='" + form["emptempPincode"] + "',emptempDistrict='" + form["emptempDistrict"] + "',emptempState='" + form["emptempState"] + "',Pincode='" + form["Pincode"] + "',District='" + form["District"] + "',State='" + form["State"] + "' where Id='" + hid + "'";

                    string lquery = "update tbl_login set Username='" + form["empname"] + "',Emailid='" + form["empmail"] + "',Mobile='" + form["empmobile"] + "',Type='" + form["usertype"] + "' where  Emprowid='" + hid + "'";


                    if (db.InsertUpdateDelete(query) && db.InsertUpdateDelete(lquery))
                    {

                        activitylog.Activitylogins("tbl_registration", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.AlertMessage = form["empcode"] + " Registration Approved";
                        string lqueryHRM = "update tbl_HRM set  workStatus='Registered' where  canname='" + form["empname"] + "'AND canmobile='" + form["empmobile"] + "'";
                        string lqueryres = "update tbl_docsonboarding set Employee_id='" + form["empcode"] + "' where  canname='" + form["empname"] + "'AND canmobile='" + form["empmobile"] + "'";
                        if (db.InsertUpdateDelete(lqueryHRM) && db.InsertUpdateDelete(lqueryres))
                        {
                            activitylog.Activitylogins("tbl_HRM", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_registration", hid, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.AlertMessage = form["empcode"] + " Approval Failed";
                    }
                }
                else
                {
                    string query1 = "SELECT * FROM tbl_registration WHERE (Email='" + form["empmail"] + "' OR Mobile_no='" + form["empmobile"] + "' OR Employee_id='" + form["empcode"] + "')";
                    activitylog.Activitylogins("tbl_registration", "", query1, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    DataTable dt = db.GetAllRecord(query1);
                    activitylog.Activitylogupd("Success", "");
                    string query2 = "SELECT * FROM tbl_login WHERE Emailid='" + form["empmail"] + "' OR Mobile='" + form["empmobile"] + "'";
                    activitylog.Activitylogins("tbl_login", "", query2, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    DataTable dt2 = db.GetAllRecord(query2);
                    activitylog.Activitylogupd("Success", "");
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            str += dt.Rows[i]["Email"] + ",";
                            str += dt.Rows[i]["Employee_id"] + ",";
                            str += dt.Rows[i]["Mobile_no"] + ",";
                        }
                        string[] strArray = str.Split(',');

                        if (strArray.Contains(form["empmail"]) && strArray.Contains(form["empmobile"]) && strArray.Contains(form["empcode"]))
                        {
                            ViewBag.msg = "Employeeid, Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empmobile"]) && strArray.Contains(form["empcode"]))
                        {
                            ViewBag.msg = "Employeeid, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empmail"]) && strArray.Contains(form["empcode"]))
                        {
                            ViewBag.msg = "Employeeid, Email Already Exist";
                        }
                        else if (strArray.Contains(form["empmail"]) && strArray.Contains(form["empmobile"]))
                        {
                            ViewBag.msg = "Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empcode"]))
                        {
                            ViewBag.msg = "Employeeid Already Exist";
                        }
                        else if (strArray.Contains(form["empmobile"]))
                        {
                            ViewBag.msg = "Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empmail"]))
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

                        if (strArray.Contains(form["empmail"]) && strArray.Contains(form["empmobile"]))
                        {
                            ViewBag.msg = "Email, Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empmobile"]))
                        {
                            ViewBag.msg = "Mobile Already Exist";
                        }
                        else if (strArray.Contains(form["empmail"]))
                        {
                            ViewBag.msg = "Email Already Exist";
                        }
                    }
                    else
                    {
                        string res = qr.QRCode(form["empcode"], form["empname"], form["departmentname"], form["empmail"], form["empmobile"], "GROWFAST ORGANIC DIAMOND PVT. LTD.", form["designationname"], form["emerconmob"], form["emerconrel"], form["emerconname"], "RS Plaza Level near 2 Jagrani hospital", form["empaddress"]);
                        if (res != null && res != "")
                        {
                            qrpth = res;
                        }
                        else
                        {
                            qrpth = form["qrpth"];
                        }

                        string pass = random();

                        //string query = "insert into tbl_registration(Employee_id, Employee_Type, Name, Mobile_no, Email, pass, Department_name, Designation, Status, Date_time, Push_notification, Email_notification, Sms_notification, Type, Gardianname, Address, Temporaryaddress, Alternatemobile, Dateofbirth, Gender, Maritalstatus, Premises, Bloodgroup, Departmentid, Managername, Managercode, Employeementtype, Bankname, Accountnumber, Accountholdername, IFSC, PFNo,UANNo, ESIC, Paymentmode, DOL, PAN, Emerconname, Emerconmobile, Emerconrelation,Shiftid, Shifttiming, Shiftname, Shiftstarttime, Shiftendtime, Activatecheckin, Dateofjoining, Nofchild, Citizenship, Flexigours,QRpath,Basicsalary, Houserentallowance, Conveyanceallowance,Medicalallowance, Specialallowance, Grosssalary, EPF, HealthInsurance, Professionaltax, TDS, Totaldeduction, Netpay, Pendingleave, Approveleave, Balanceleave, LWPleave,Personalleave,Casualleave,Sickleave,OtherLeave,Totalleave,logname,logid,Branchname,Branchcode,designation_order,designationid,Idnumber,Mcompanyname,Mcompanyid) values('" + form["empcode"] + "','" + form["usertype"] + "','" + form["empname"] + "','" + form["empmobile"] + "','" + form["empmail"] + "','" + pass + "','" + form["departmentname"] + "','" + form["designationname"] + "','Approved','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','on','on','on','user','" + form["empgardian"] + "','" + form["empaddress"] + "','" + form["emptempaddress"] + "','" + form["empaltmobile"] + "','" + form["empdob"] + "','" + form["empgender"] + "','" + form["maritalstatus"] + "','" + form["premises"] + "','" + form["bloodgroup"] + "','" + form["departmentid"] + "','" + form["managername"] + "','" + form["managerid"] + "','" + form["employeementtype"] + "','" + form["bankname"] + "','" + form["accountno"] + "','" + form["accholdernm"] + "','" + form["ifsc"] + "','" + form["pfno"] + "','" + form["uanno"] + "','" + form["esic"] + "','" + form["paymentmode"] + "','" + form["dol"] + "','" + form["panno"] + "','" + form["emerconname"] + "','" + form["emerconmob"] + "','" + form["emerconrel"] + "','" + form["shift"] + "','" + form["shifttiming"] + "','" + form["shiftname"] + "','" + form["shiftstarttime"] + "','" + form["shiftendtime"] + "','" + form["checkinmode"] + "','" + form["doj"] + "','" + form["nofchild"] + "','" + form["citizen"] + "','" + form["flexihrs"] + "','" + qrpth + "','" + form["basicsalary"] + "','" + form["houserentallow"] + "','" + form["conveyanceallow"] + "','" + form["medicalallow"] + "','" + form["specialallow"] + "','" + form["grosssalary"] + "','" + form["epf"] + "','" + form["healthensurance"] + "','" + form["professionaltax"] + "','" + form["tds"] + "','" + form["totaldeduction"] + "','" + form["netpay"] + "','0','0','" + form["totalleave"] + "','0','" + form["perleave"] + "','" + form["casleave"] + "','" + form["sickleave"] + "','" + form["othleave"] + "','" + form["totalleave"] + "','" + Session["auname"] + "','" + Session["auid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["designationorder"] + "','" + form["designationid"] + "','" + form["Idnumber"] + "','" + Session["Companynamem"] + "','" + Session["Companyidm"] + "')";
                        string query = "insert into tbl_registration(Employee_id, Employee_Type, Name, Mobile_no, Email, pass, Department_name, Designation, Status, Date_time, Push_notification, Email_notification, Sms_notification, Type, Gardianname, Address, Temporaryaddress, Alternatemobile, Dateofbirth, Gender, Maritalstatus, Premises, Bloodgroup, Departmentid, Managername, Managercode, Employeementtype, Bankname, Accountnumber, Accountholdername, IFSC, PFNo, UANNo, ESIC, Paymentmode, DOL, PAN, Emerconname, Emerconmobile, Emerconrelation,Shiftid, Shifttiming, Shiftname, Shiftstarttime, Shiftendtime, Activatecheckin, Dateofjoining, Nofchild, Citizenship, Flexigours,QRpath,Basicsalary, Houserentallowance, Conveyanceallowance,Medicalallowance, Specialallowance, Grosssalary, EPF, HealthInsurance, Professionaltax, TDS, Totaldeduction, Netpay, Pendingleave, Approveleave, Balanceleave, LWPleave,Personalleave,Casualleave,Sickleave,OtherLeave,Totalleave,logname,logid,Branchname,Branchcode,designation_order,designationid,Idnumber,Mcompanyname,Mcompanyid,Aadhar_no,emptempPincode,emptempDistrict,emptempState,Pincode,District,State) values('" + form["empcode"] + "','" + form["usertype"] + "','" + form["empname"] + "','" + form["empmobile"] + "','" + form["empmail"] + "','" + pass + "','" + form["departmentname"] + "','" + form["designationname"] + "','Approved','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','on','on','on','user','" + form["empgardian"] + "','" + form["empaddress"] + "','" + form["emptempaddress"] + "','" + form["empaltmobile"] + "','" + form["empdob"] + "','" + form["empgender"] + "','" + form["maritalstatus"] + "','" + form["premises"] + "','" + form["bloodgroup"] + "','" + form["departmentid"] + "','" + form["managername"] + "','" + form["managerid"] + "','" + form["employeementtype"] + "','" + form["bankname"] + "','" + form["accountno"] + "','" + form["accholdernm"] + "','" + form["ifsc"] + "','" + form["pfno"] + "','" + form["uanno"] + "','" + form["esic"] + "','" + form["paymentmode"] + "','" + form["DOL"] + "','" + form["PAN"] + "','" + form["emerconname"] + "','" + form["emerconmob"] + "','" + form["emerconrel"] + "','" + form["shift"] + "','" + form["shifttiming"] + "','" + form["shiftname"] + "','" + form["shiftstarttime"] + "','" + form["shiftendtime"] + "','" + form["Activatecheckin"] + "','" + form["doj"] + "','" + form["Nofchild"] + "','" + form["citizen"] + "','" + form["flexihrs"] + "','" + qrpth + "','" + form["basicsalary"] + "','" + form["houserentallow"] + "','" + form["conveyanceallow"] + "','" + form["medicalallow"] + "','" + form["specialallow"] + "','" + form["grosssalary"] + "','" + form["epf"] + "','" + form["healthensurance"] + "','" + form["professionaltax"] + "','" + form["tds"] + "','" + form["totaldeduction"] + "','" + form["netpay"] + "','0','0','" + form["totalleave"] + "','0','" + form["perleave"] + "','" + form["casleave"] + "','" + form["sickleave"] + "','" + form["othleave"] + "','" + form["totalleave"] + "','" + Session["auname"] + "','" + Session["auid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["designationorder"] + "','" + form["designationid"] + "','" + form["Idnumber"] + "','" + Session["Companynamem"] + "','" + Session["Companyidm"] + "','" + form["Aadhar_no"] + "','" + form["emptempPincode"] + "','" + form["emptempDistrict"] + "','" + form["emptempState"] + "','" + form["Pincode"] + "','" + form["District"] + "','" + form["State"] + "')";

                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins("tbl_registration", db.getmaxid("tbl_registration").ToString(), query, "Success", "Insert Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                            string lquery = "insert into tbl_login(Username,Userid,Emailid,Mobile,Password,Type,Status,Datetime,OTP_Time,Emprowid,Branchname,Branchcode) values('" + form["empname"] + "','" + form["empcode"] + "','" + form["empmail"] + "','" + form["empmobile"] + "','" + pass + "','" + form["usertype"] + "','Active','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + db.getrowid("select Id from tbl_registration where Employee_id='" + form["empcode"] + "'") + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";

                            string lqueryee = "update tbl_HRM set Employee_id='" + form["empcode"] + "', workStatus='Registered' where  canname='" + form["empname"] + "'AND canmobile='" + form["empmobile"] + "'";


                            if (db.InsertUpdateDelete(lquery))
                            {
                                activitylog.Activitylogins("tbl_login", db.getmaxid("tbl_login").ToString(), lquery, "Success", "Insert Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                db.InsertUpdateDelete(lqueryee);
                                ViewBag.AlertMessage = form["empcode"] + " Registration Approved";
                                //sendmail.SendMailEmployee("", form["empmail"], "Registration Successfully", bodytext);

                                string[] replacementValues = { "GROWFAST ORGANIC DIMOND" };
                                //Messaging.SendSMS(form["empmobile"], replacementValues, "Thank You Message for Joining");
                                //Messaging.SendWhatsappSMSNew(form["empmobile"],replacementValues, "Thank You Message for Joining", form["empname"] + "", Session["auid"] + "", Session["auname"] + "", "");

                                string insquery = "insert into tbl_notification(Employeeid,Employeename,Notification_Header,Notification_Body,Status,Date_time) values ('" + form["managerid"] + "','" + form["managername"] + "','Team Member Added','" + form["empname"] + " Join Your team as " + form["designation"] + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                string lqueryeeres = "update tbl_docsonboarding set Employee_id='" + form["empcode"] + "' where  canname='" + form["empname"] + "'AND canmobile='" + form["empmobile"] + "'";

                                if (db.InsertUpdateDelete(insquery) && db.InsertUpdateDelete(lqueryeeres))
                                {
                                    //pushNotification.SendPushNotification("Team Member Added", form["empname"] + " Join Your team as " + form["designation"] + ".", form["managerid"] + "", acode);

                                    activitylog.Activitylogins("tbl_notification", db.getmaxid("tbl_notification").ToString(), insquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                    //ViewBag.AlertMessage = "Notification Send";
                                }
                                else
                                {
                                    activitylog.Activitylogins("tbl_notification", "", insquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                    //ViewBag.AlertMessage = "Notification Sending Failed";
                                }
                                //ViewBag.AlertMessage = form["empcode"] + " Registration Approved";
                            }
                            else
                            {

                                activitylog.Activitylogins("tbl_login", "", lquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                ViewBag.AlertMessage = form["empcode"] + " Approval Failed";
                            }

                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_registration", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }
                catch
                {

                }
                ViewBag.msg = "Contact no or mailid or Empcode already exist";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        public JsonResult UpdateDepartment(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_department where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_department", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string name = dt.Rows[0]["Departmant"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";

                    res = new string[5] { id, name, status, companyname, companyid };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDesignation(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_designation where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_designation", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string name = dt.Rows[0]["Designation"] + "";
                    //string orderno = dt.Rows[0]["Ordernumber"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string departmentid = dt.Rows[0]["Departmentid"] + "";
                    string department = dt.Rows[0]["Department"] + "";
                    string Designation_order = dt.Rows[0]["Designation_order"] + "";


                    res = new string[8] { id, name, status, companyname, companyid, departmentid, department, Designation_order };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCompany(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_company where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_company", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Company = dt.Rows[0]["Company"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";

                    res = new string[5] { id, Company, status, companyname, companyid };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateZone(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_zone where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_zone", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Zone = dt.Rows[0]["Zone"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";


                    res = new string[7] { id, Zone, status, companyname, companyid, Companyid, Companyname };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRegion(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_region where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_region", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Region = dt.Rows[0]["Region"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";
                    string Zoneid = dt.Rows[0]["Zoneid"] + "";
                    string Zonename = dt.Rows[0]["Zonename"] + "";


                    res = new string[9] { id, Region, status, companyname, companyid, Companyid, Companyname, Zoneid, Zonename };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDivision(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_division where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_division", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Division = dt.Rows[0]["Division"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";
                    string Zoneid = dt.Rows[0]["Zoneid"] + "";
                    string Zonename = dt.Rows[0]["Zonename"] + "";
                    string Regionid = dt.Rows[0]["Regionid"] + "";
                    string Regionname = dt.Rows[0]["Regionname"] + "";


                    res = new string[11] { id, Division, status, companyname, companyid, Companyid, Companyname, Zoneid, Zonename, Regionid, Regionname };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateBranch1(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_branch1 where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_branch1", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Branch = dt.Rows[0]["Branch"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";
                    string Zoneid = dt.Rows[0]["Zoneid"] + "";
                    string Zonename = dt.Rows[0]["Zonename"] + "";
                    string Regionid = dt.Rows[0]["Regionid"] + "";
                    string Regionname = dt.Rows[0]["Regionname"] + "";
                    string Divisionid = dt.Rows[0]["Divisionid"] + "";
                    string Divisionname = dt.Rows[0]["Divisionname"] + "";


                    res = new string[13] { id, Branch, status, companyname, companyid, Companyid, Companyname, Zoneid, Zonename, Regionid, Regionname, Divisionid, Divisionname };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateTeam(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_team where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_team", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Team = dt.Rows[0]["Team"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";
                    string Zoneid = dt.Rows[0]["Zoneid"] + "";
                    string Zonename = dt.Rows[0]["Zonename"] + "";
                    string Regionid = dt.Rows[0]["Regionid"] + "";
                    string Regionname = dt.Rows[0]["Regionname"] + "";
                    string Divisionid = dt.Rows[0]["Divisionid"] + "";
                    string Divisionname = dt.Rows[0]["Divisionname"] + "";
                    string Branchid = dt.Rows[0]["Branchid"] + "";
                    string Branchname = dt.Rows[0]["Branchname"] + "";


                    res = new string[15] { id, Team, status, companyname, companyid, Companyid, Companyname, Zoneid, Zonename, Regionid, Regionname, Divisionid, Divisionname, Branchid, Branchname };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateGroup(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_group where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_group", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Group = dt.Rows[0]["Groups"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";
                    string Companyid = dt.Rows[0]["Companyid"] + "";
                    string Companyname = dt.Rows[0]["Companyname"] + "";
                    string Zoneid = dt.Rows[0]["Zoneid"] + "";
                    string Zonename = dt.Rows[0]["Zonename"] + "";
                    string Regionid = dt.Rows[0]["Regionid"] + "";
                    string Regionname = dt.Rows[0]["Regionname"] + "";
                    string Divisionid = dt.Rows[0]["Divisionid"] + "";
                    string Divisionname = dt.Rows[0]["Divisionname"] + "";
                    string Branchid = dt.Rows[0]["Branchid"] + "";
                    string Branchname = dt.Rows[0]["Branchname"] + "";
                    string Teamid = dt.Rows[0]["Teamid"] + "";
                    string Teamname = dt.Rows[0]["Teamname"] + "";


                    res = new string[17] { id, Group, status, companyname, companyid, Companyid, Companyname, Zoneid, Zonename, Regionid, Regionname, Divisionid, Divisionname, Branchid, Branchname, Teamid, Teamname };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecordorg_old(string Tablename, string Columnname, string columnvalue)
        {
            string res = "";
            try
            {
                string query = "select * from " + Tablename + " where " + Columnname + "='" + columnvalue + "'";
                activitylog.Activitylogins("tbl_region", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");

                res = JsonConvert.SerializeObject(dt, Formatting.None);
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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecordorg(string Tablename, string Columnname, string columnvalue)
        {
            string res = "";
            try
            {
                string query = "select * from " + Tablename + " ";

                if (columnvalue != "" && columnvalue != null)
                {
                    query += " where ";
                    string[] arrayOfStrings = columnvalue.Split(',');

                    // Display each element in the array
                    foreach (string element in arrayOfStrings)
                    {
                        query += " " + Columnname + "='" + element + "' or";
                    }
                    if (query.EndsWith(" or"))
                    {
                        query = query.Substring(0, query.Length - 3);
                    }
                }
                activitylog.Activitylogins("tbl_region", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");

                res = JsonConvert.SerializeObject(dt, Formatting.None);
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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }




        public JsonResult UpdateBranch(string Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_branch where Branchid='" + Up + "'";
                activitylog.Activitylogins("tbl_branch", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string name = dt.Rows[0]["Branch"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["logname"] + "";
                    string companyid = dt.Rows[0]["logid"] + "";

                    string State = dt.Rows[0]["State"] + "";
                    string City = dt.Rows[0]["City"] + "";
                    string Cityid = dt.Rows[0]["Cityid"] + "";
                    string Stateid = dt.Rows[0]["Stateid"] + "";

                    res = new string[9] { id, name, status, companyname, companyid, State, City, Cityid, Stateid };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Leave()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Leave(FormCollection form, HttpPostedFileBase attatch)
        {
            try
            {
                string acode = Session["auid"] + "";
                string auname = Session["auname"] + "";
                string query = "", descemp = "";
                int getpendingleave;
                string empid = "";

                string fileName = "", attatchpth = "";
                if (attatch != null)
                {
                    string respth = APIs.LeaveUpload(attatch);
                    if (respth != "" && respth != null)
                    {
                        attatchpth = respth;
                    }
                    else
                    {
                        ViewBag.msg = "File not Upload'";
                        attatchpth = "Content/Img/defaultimg1.png";
                    }
                }
                else
                {
                    ViewBag.msg = "Please select a file";
                    attatchpth = "Content/Img/defaultimg1.png";
                }
                if (form["dur"] == "Multiple")
                {
                    DateTime FDT = Convert.ToDateTime(form["fromdate"]);
                    DateTime TDT = Convert.ToDateTime(form["todate"]);
                    TimeSpan difference = TDT - FDT;
                    var days = (difference.TotalDays) + 1;
                    query = "INSERT INTO tbl_leave(Leave_id,Emprowid,Employeeid,Name,Leaveduration,From_date,To_date,Total_day,Reason,Status,Attachment,Date,Leave_type,Department,Designation,Logname,LogId,Managername,Managercode,BranchName,BranchCode,Premise,Regionid) VALUES('" + random() + "','" + form["emprid"] + "','" + form["empid"] + "','" + form["empname"] + "','" + form["dur"] + "','" + form["fromdate"] + "','" + form["todate"] + "','" + days + "','" + form["reason"] + "','Pending','" + attatchpth + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + form["leavetype"] + "','" + form["department"] + "','" + form["desig"] + "','" + auname + "','" + acode + "','" + form["manegername"] + "','" + form["manegerid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                    descemp = "Leave applied from " + form["fromdate"] + " to " + form["todate"] + "";
                }
                else
                {
                    query = "INSERT INTO tbl_leave(Leave_id,Emprowid,Employeeid,Name,Leaveduration,From_date,To_date,Total_day,Reason,Status,Attachment,Date,Leave_type,Department,Designation,Logname,LogId,Managername,Managercode,BranchName,BranchCode,Premise,Regionid) VALUES('" + random() + "','" + form["emprid"] + "','" + form["empid"] + "','" + form["empname"] + "','" + form["dur"] + "','" + form["fromdate"] + "','" + form["fromdate"] + "','1','" + form["reason"] + "','Pending','" + attatchpth + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + form["leavetype"] + "','" + form["department"] + "','" + form["desig"] + "','" + auname + "','" + acode + "','" + form["manegername"] + "','" + form["manegerid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                    descemp = "Leave applied from " + form["fromdate"] + " to " + form["fromdate"] + "";
                }
                string selquery = "select Pendingleave,id from tbl_registration where Id='" + form["emprid"] + "' ";
                activitylog.Activitylogins("tbl_registration", "", selquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable sdt = db.GetAllRecord(selquery); ;
                activitylog.Activitylogupd("Success", "");
                if (sdt.Rows.Count > 0)
                {
                    getpendingleave = Convert.ToInt32(sdt.Rows[0]["Pendingleave"]);
                    empid = sdt.Rows[0]["Id"].ToString();
                }
                else
                {
                    getpendingleave = 0;
                }
                int totalpendingleave = getpendingleave + 1;
                string regquery = "update tbl_registration set Pendingleave=" + totalpendingleave + " where Id='" + form["emprid"] + "'";
                if (db.InsertUpdateDelete(query) && db.InsertUpdateDelete(regquery))
                {
                    string header1 = "Leave Applied Successfully";
                    string header2 = form["empname"] + " applied for leave(s)";

                    string insquery = "insert into tbl_notification(Employeeid,Employeename,Notification_Header,Notification_Body,Status,Date_time,BranchName,BranchCode) values ('" + form["empid"] + "','" + form["empname"] + "','" + header1 + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["auname"] + "','" + Session["auid"] + "'),('" + form["managerid"] + "','" + form["managername"] + "','" + header2 + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                    if (db.InsertUpdateDelete(insquery))
                    {
                        Messaging.SendPushNotification(header2, descemp + " has been pending", Session["managerid"] + "");
                        Messaging.SendPushNotification(header1, descemp, form["empid"]);

                        activitylog.Activitylogins("tbl_notification", db.getmaxid("tbl_notification").ToString(), insquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        //ViewBag.AlertMessage = "Notification Send";

                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_notification", "", insquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        //ViewBag.AlertMessage = "Notification Sending Failed";
                    }
                    activitylog.Activitylogins("tbl_registration", empid, regquery, "Success", "Update Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    activitylog.Activitylogins("tbl_leave", db.getmaxid("tbl_leave").ToString(), query, "Success", "Insert Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Data Saved";
                }
                else
                {
                    activitylog.Activitylogins("tbl_registration", empid, regquery, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    activitylog.Activitylogins("tbl_leave", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Data Save Error";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        public JsonResult GetLeave(string Start_date, string End_date, string Department, string Empnm, string Status, string Designation)
        {
            string res = "", tbldata = "", tbl = "", json = "";
            try
            {
                DateTime startdate = DateTime.ParseExact(Start_date, "d/M/yyyy", null);
                string formatteStart_date = startdate.ToString("yyyy-MM-dd");
                DateTime enddate = DateTime.ParseExact(End_date, "d/M/yyyy", null);
                string formattedEnd_date = enddate.ToString("yyyy-MM-dd");

                string squery = "select * from tbl_leave where (CONVERT(DATE, Date)>='" + formatteStart_date + "' and CONVERT(DATE, Date)<='" + formattedEnd_date + "') and BranchCode='" + Session["abrcode"] + "' and Status!='Inactive'";
                if (!string.IsNullOrEmpty(Department))
                {
                    squery += " AND Department = '" + Department + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND Emprowid = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Designation))
                {
                    squery += " AND Designation = '" + Designation + "'";
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    squery += " AND Status = '" + Status + "'";
                }
                activitylog.Activitylogins("tbl_leave", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Loan()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Payment()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult EMonthsummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult EDaysummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult SMonthsummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult SDaysummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult MonthlyPerformance()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult SalaryReport()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ExpenceReport()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ProSalesReport()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult GetMonthsummary(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            string res = "", tbldata = "", tbl = "", subquery = " Where tbl_registration.Status='Approved' and tbl_registration.Branchcode='" + Session["abrcode"] + "'", regsubquery = ""; int holidaycount = 0;
            StringBuilder htmlTable = new StringBuilder();
            try
            {
                string holidayquery = "select * from tbl_Holiday where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "' and Status='Active'";
                activitylog.Activitylogins("tbl_Holiday", "", holidayquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable holidaydt = db.GetAllRecord(holidayquery);
                activitylog.Activitylogupd("Success", "");
                if (holidaydt.Rows.Count > 0)
                {
                    holidaycount = holidaydt.Rows.Count;
                }
                else
                {
                    holidaycount = 0;
                }
                int count = 1;
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    count++;
                }
                int addday = count - 2;
                int span = count / 2;
                int span2 = span + 1;
                DateTime startDate = new DateTime(Year, Month, 1);
                DateTime endDate = startDate.AddDays(addday);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date != null)
                    {
                        regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Working_hours,'' ) ELSE NULL END) AS WH" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_leave.From_date) = " + date.Day + " THEN ISNULL(tbl_leave.Total_day,0 ) ELSE NULL END) AS Totalleave" + date.Day + ",";

                    }

                }
                regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

                string empquery = "", desigquery = "", departquery = "", managerquery = "";
                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " tbl_attendance.Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += " and (" + managerquery + ")";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " tbl_attendance.Emprowid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += " and (" + empquery + ")";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " tbl_registration.Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " and (" + desigquery + ")";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " tbl_registration.Department_name='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " and (" + departquery + ")";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }
                //string squery = "select * from tbl_attendance where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "' and ("+subquery+")";
                string attquery = "SELECT tbl_registration.Name,tbl_registration.Id,tbl_registration.Employee_id,tbl_registration.Employee_type,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime," +
     regsubquery + " FROM tbl_registration LEFT JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND MONTH(tbl_attendance.Date) = " + Month + " LEFT JOIN tbl_leave ON tbl_registration.Id = tbl_leave.Emprowid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY  tbl_registration.Name,tbl_registration.Employee_id,tbl_registration.Id,tbl_registration.Employee_type,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime";
                activitylog.Activitylogins("tbl_registration,tbl_attendance", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {
                    htmlTable.Append("<tbody class='theadb text-center'>");
                    htmlTable.Append("<tr>");
                    htmlTable.Append("<th>ID</th>");
                    htmlTable.Append("<th>Employee Id</th>");
                    htmlTable.Append("<th>Name</th>");
                    htmlTable.Append("<th>Department</th>");
                    htmlTable.Append("<th>Designation</th>");
                    //htmlTable.Append("<th>Type</th>");
                    htmlTable.Append("<th>Total Day</th>");
                    htmlTable.Append("<th>Present</th>");
                    htmlTable.Append("<th>Holiday</th>");
                    htmlTable.Append("<th>Absent</th>");
                    htmlTable.Append("<th>Week Off</th>");
                    htmlTable.Append("<th>Leaves</th>");
                    htmlTable.Append("<th>Half Day</th>");
                    htmlTable.Append("<th>Total Paid Day</th>");
                    htmlTable.Append("<th>Total Late Arrival</th>");
                    htmlTable.Append("<th>Early Departure</th>");
                    htmlTable.Append("</tr>");
                    for (int i = 0; i < attdt.Rows.Count; i++)
                    {
                        int present = 0, absent = 0, leavecount = 0, latearrival = 0, earlydeparture = 0, halfday = 0, totalday = 0, weekofcount = 0, paiddaycount;
                        int j = i + 1;

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            totalday++;

                        }
                        DataTable weekoffdt = db.GetAllRecord("select * from tbl_weekoff where Status='Active'");
                        if (weekoffdt.Rows.Count > 0)
                        {
                            for (int wo = 0; wo < weekoffdt.Rows.Count; wo++)
                            {
                                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                                {
                                    if (date.ToString("dddd") == weekoffdt.Rows[wo]["Weekday"] + "")
                                    {
                                        weekofcount++;
                                    }

                                }
                            }

                        }
                        else
                        {
                            weekofcount = 0;
                        }

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                if (DateTime.Parse(attdt.Rows[i]["PI" + date.Day + ""] + "") > DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + ""))
                                {
                                    latearrival++;
                                }

                            }

                        }
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                if (DateTime.Parse(attdt.Rows[i]["PO" + date.Day + ""] + "") < DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + ""))
                                {
                                    earlydeparture++;
                                }

                            }

                        }
                        TimeSpan dur = DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + "") - DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + "");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["WH" + date.Day + ""] + "" != "" && attdt.Rows[i]["WH" + date.Day + ""] + "" != null)
                            {
                                if (TimeSpan.Parse(attdt.Rows[i]["WH" + date.Day + ""] + "") < dur)
                                {
                                    halfday++;
                                }

                            }

                        }
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                        //    {
                        //        present++;
                        //    }

                        //}
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                present++;
                            }

                        }
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != "" && attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != null )
                        //    {
                        //        leavecount += Convert.ToInt32(attdt.Rows[i]["Totalleave" + date.Day + ""]);
                        //    }

                        //}
                        absent = totalday - (present + holidaycount + leavecount + weekofcount);
                        paiddaycount = present + holidaycount + weekofcount;
                        //halfday = latearrival + earlydeparture;
                        htmlTable.Append("<tr class='bg-light-subtle text-dark' padding='10px'>");
                        htmlTable.Append("<td>" + j + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Employee_id"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Department_name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Designation"] + "</td>");
                        //htmlTable.Append("<td>" + attdt.Rows[i]["Employee_type"] + "</td>");
                        htmlTable.Append("<td>" + totalday + "</td>");
                        htmlTable.Append("<td>" + present + "</td>");
                        htmlTable.Append("<td>" + holidaycount + "</td>");
                        htmlTable.Append("<td>" + absent + "</td>");
                        htmlTable.Append("<td>" + weekofcount + "</td>");
                        htmlTable.Append("<td>" + leavecount + "</td>");
                        htmlTable.Append("<td>" + halfday + "</td>");
                        htmlTable.Append("<td>" + paiddaycount + "</td>");
                        htmlTable.Append("<td>" + latearrival + "</td>");
                        htmlTable.Append("<td>" + earlydeparture + "</td>");
                        htmlTable.Append("</tr>");



                    }
                    htmlTable.Append("</tbody>");
                }
                else
                {
                    htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
                }


                //DataTable dt = db.GetAllRecord(squery);
                string json = JsonConvert.SerializeObject(attdt, Formatting.None);
                //return View();
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

                }
                htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
            }
            finally
            {
                db.connectionstate();
            }

            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDaysummary(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            string res = "", tbldata = "", tbl = "", subquery = " Where tbl_registration.Status='Approved' and tbl_registration.Branchcode='" + Session["abrcode"] + "' ", regsubquery = "";
            StringBuilder htmlTable = new StringBuilder();
            try
            {
                int count = 1;
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    count++;
                }
                int addday = count - 2;
                int span = count / 2;
                int span2 = span + 1;
                DateTime startDate = new DateTime(Year, Month, 1);
                DateTime endDate = startDate.AddDays(addday);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date != null)
                    {
                        regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Overtime,'' ) ELSE NULL END) AS OT" + date.Day + ",";

                    }

                }
                regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

                string empquery = "", desigquery = "", departquery = "", managerquery = "";
                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " tbl_attendance.Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += " and (" + managerquery + ")";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " tbl_attendance.Emprowid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += " and (" + empquery + ")";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " tbl_attendance.Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " and (" + desigquery + ")";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " tbl_attendance.Department='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " and (" + departquery + ")";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }
                //string squery = "select * from tbl_attendance where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "'  and ("+subquery+")";
                string attquery = "SELECT tbl_registration.Name,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Employee_Type,tbl_registration.Employee_id,tbl_registration.Id," +
     regsubquery + " FROM tbl_registration LEFT JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY tbl_registration.Name,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Employee_Type,tbl_registration.Employee_id,tbl_registration.Id";
                activitylog.Activitylogins("tbl_registration,tbl_attendance", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                //string monthName = new DateTime(Year, Month, 1).ToString("MMMM");
                //int spancount = count + 7;
                if (attdt.Rows.Count > 0)
                {
                    htmlTable.Append("<tbody class='theadb text-center'>");
                    //htmlTable.Append("<tr><td colspan='" + spancount + "' class='text-center fs-6 hrow'>reheh Daily Performance Report " + monthName + " - " + Year + "</th></tr>");
                    htmlTable.Append("<tr>");
                    htmlTable.Append("<th>Employee Code</th>");
                    //htmlTable.Append("<th>Device Person Id</th>");
                    htmlTable.Append("<th>Name</th>");
                    htmlTable.Append("<th>Department</th>");
                    htmlTable.Append("<th>Designation</th>");
                    //htmlTable.Append("<th>Type</th>");
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        htmlTable.Append("<th>" + date.Day + "</th>");

                    }
                    htmlTable.Append("<th>Total Duty</th>");
                    htmlTable.Append("<th>Totl OT(Hours)</th>");
                    htmlTable.Append("</tr>");
                    for (int i = 0; i < attdt.Rows.Count; i++)
                    {
                        int totalduty = 0;
                        int totalovertime = 0;
                        htmlTable.Append("<tr class='bg-light-subtle text-dark' padding='10px'>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Employee_id"] + "</td>");
                        //htmlTable.Append("<td>-</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Department_name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Designation"] + "</td>");
                        //htmlTable.Append("<td>" + attdt.Rows[i]["Employee_Type"] + "</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            string pival = "", poval = "";
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                DateTime pidate = DateTime.ParseExact(attdt.Rows[i]["PI" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                pival = "P";
                            }
                            else
                            {
                                pival = "A"
        ;
                            }
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                DateTime podate = DateTime.ParseExact(attdt.Rows[i]["PO" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                poval = "P";
                                totalduty++;
                            }
                            else
                            {
                                poval = "A";
                            }
                            htmlTable.Append("<td>" + pival + " " + poval + "</td>");
                            if (attdt.Rows[i]["OT" + date.Day + ""] + "" != "" && attdt.Rows[i]["OT" + date.Day + ""] + "" != null)
                            {
                                DateTime otdate = DateTime.ParseExact(attdt.Rows[i]["OT" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                totalovertime += Convert.ToInt32(otdate.ToString("HH"));
                            }
                            else
                            {
                                totalovertime += 0;
                            }

                        }


                        htmlTable.Append("<td>" + totalduty + "</td>");
                        htmlTable.Append("<td>" + totalovertime + "</td>");
                        htmlTable.Append("</tr>");
                    }

                    htmlTable.Append("</tbody>");
                }
                else
                {
                    htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
                }

                //DataTable dt = db.GetAllRecord(squery);
                string json = JsonConvert.SerializeObject(attdt, Formatting.None);
                //return View();
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

                }
                htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
            }
            finally
            {
                db.connectionstate();
            }

            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMonthperfor(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            string res = "", tbldata = "", tbl = "", subquery = " Where tbl_registration.Status='Approved' and tbl_registration.Branchcode='" + Session["abrcode"] + "' ", regsubquery = ""; int holidaycount;
            StringBuilder htmlTable = new StringBuilder();
            try
            {
                string holidayquery = "select * from tbl_Holiday where MONTH(Date)='" + Month + "' and YEAR(Date)='" + Year + "' and Status='Active'";
                activitylog.Activitylogins("tbl_Holiday", "", holidayquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable holidaydt = db.GetAllRecord(holidayquery);
                activitylog.Activitylogupd("Success", "");
                if (holidaydt.Rows.Count > 0)
                {
                    holidaycount = holidaydt.Rows.Count;
                }
                else
                {
                    holidaycount = 0;
                }
                int count = 1;
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    count++;
                }
                int addday = count - 2;
                int span = count / 2;
                int span2 = span + 1;
                DateTime startDate = new DateTime(Year, Month, 1);
                DateTime endDate = startDate.AddDays(addday);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date != null)
                    {
                        regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Working_hours,'' ) ELSE NULL END) AS WH" + date.Day + ",";

                    }

                }
                regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

                string empquery = "", desigquery = "", departquery = "", managerquery = "";
                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " tbl_attendance.Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += " and (" + managerquery + ")";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " tbl_attendance.Emprowid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += " and (" + empquery + ")";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " tbl_attendance.Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " and (" + desigquery + ")";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " tbl_attendance.Department='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " and (" + departquery + ")";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }
                //string squery = "select * from tbl_attendance where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "' and ("+subquery+")";
                string attquery = "SELECT tbl_registration.Name,tbl_registration.Employee_type,tbl_registration.Employee_id,tbl_registration.Id,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime," +
     regsubquery + " FROM tbl_registration LEFT JOIN tbl_attendance ON tbl_registration.Id = tbl_attendance.Emprowid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY tbl_registration.Name,tbl_registration.Employee_type,tbl_registration.Employee_id,tbl_registration.Id,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime";
                activitylog.Activitylogins("tbl_registration,tbl_attendance", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {

                    htmlTable.Append("<tbody class='text-center'>");
                    for (int i = 0; i < attdt.Rows.Count; i++)
                    {

                        int present = 0, absent = 0, leavecount = 0, latearrival = 0, earlydeparture = 0, halfday = 0, totalday = 0, weekofcount = 0;
                        int j = i + 1;

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            totalday++;

                        }

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                if (DateTime.Parse(attdt.Rows[i]["PI" + date.Day + ""] + "") > DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + ""))
                                {
                                    latearrival++;
                                }

                            }

                        }
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                if (DateTime.Parse(attdt.Rows[i]["PO" + date.Day + ""] + "") < DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + ""))
                                {
                                    earlydeparture++;
                                }

                            }

                        }
                        TimeSpan dur = DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + "") - DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + "");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["WH" + date.Day + ""] + "" != "" && attdt.Rows[i]["WH" + date.Day + ""] + "" != null)
                            {
                                if (TimeSpan.Parse(attdt.Rows[i]["WH" + date.Day + ""] + "") < dur)
                                {
                                    halfday++;
                                }

                            }

                        }
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                        //    {
                        //        present++;
                        //    }

                        //}
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                present++;
                            }

                        }
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != "" && attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != null)
                        //    {
                        //        leavecount += Convert.ToInt32(attdt.Rows[i]["Totalleave" + date.Day + ""]);
                        //    }

                        //}
                        DataTable weekoffdt = db.GetAllRecord("select * from tbl_weekoff where Status='Active'");
                        if (weekoffdt.Rows.Count > 0)
                        {
                            for (int wo = 0; wo < weekoffdt.Rows.Count; wo++)
                            {
                                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                                {
                                    if (date.ToString("dddd") == weekoffdt.Rows[wo]["Weekday"] + "")
                                    {
                                        weekofcount++;
                                    }

                                }
                            }

                        }
                        else
                        {
                            weekofcount = 0;
                        }
                        absent = totalday - (present + holidaycount + leavecount + weekofcount);


                        htmlTable.Append("<tr class='theadb text-center'>");
                        htmlTable.Append("<td colspan='" + span + "' class='text-start'> <b> Name:</b> " + attdt.Rows[i]["Name"] + "(" + attdt.Rows[i]["Employee_id"] + ")</td>");
                        htmlTable.Append("<td colspan='" + span2 + "' class='text-end'> <b>Present:</b> " + present + "  <b>Absent:</b> " + absent + "  <b>Leaves:</b> " + leavecount + "  <b>Holiday:</b> " + holidaycount + "  <b>Weekoff:</b> " + weekofcount + " </td>");
                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td></td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            htmlTable.Append("<th>" + date.Day + "</th>");

                        }
                        htmlTable.Append("</tr>");

                        htmlTable.Append("<tr>");
                        htmlTable.Append("<th>In :</th>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                DateTime pidate = DateTime.ParseExact(attdt.Rows[i]["PI" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + pidate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");

                        htmlTable.Append("<tr>");
                        htmlTable.Append("<th>Out :</th>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                DateTime podate = DateTime.ParseExact(attdt.Rows[i]["PO" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + podate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<th>Work Hours :</th>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["WH" + date.Day + ""] + "" != "" && attdt.Rows[i]["WH" + date.Day + ""] + "" != null)
                            {
                                DateTime whdate = DateTime.ParseExact(attdt.Rows[i]["WH" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + whdate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        //htmlTable.Append("<tr>");
                        //htmlTable.Append("<td>Overtime: :</td>");
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["PI" + date.Day + ""]+"" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                        //    {
                        //        htmlTable.Append("<td>" + attdt.Rows[i]["PI" + date.Day + ""] + "</td>");
                        //    }
                        //    else
                        //    {
                        //        htmlTable.Append("<td> - </td>");
                        //    }

                        //}
                        //htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<th>In Status :</th>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                htmlTable.Append("<td>P</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td>A</td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<th>Out Status :</th>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                htmlTable.Append("<td>P</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td>A</td>");
                            }

                        }
                        htmlTable.Append("</tr>");

                    }
                    htmlTable.Append("</tbody>");
                }
                else
                {
                    htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
                }
                //DataTable dt = db.GetAllRecord(squery);
                string json = JsonConvert.SerializeObject(attdt, Formatting.None);
                //return View();
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

                }
                htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
            }
            finally
            {
                db.connectionstate();
            }

            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ChangePassword()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string emp, string npass, string cnpass)
        {
            try
            {
                if (emp == "Self")
                {
                    if (npass != cnpass)
                    {
                        ViewBag.msg = "New Password and Confirm Password Not matched.";
                    }
                    else
                    {
                        string query2 = "Update tbl_login set Password='" + npass + "' where Emailid='" + Session["amail"].ToString() + "'";

                        if (db.InsertUpdateDelete(query2))
                        {
                            activitylog.Activitylogins("tbl_login", db.getrowid("select Id from tbl_login where Emailid='" + Session["amail"].ToString() + "'").ToString(), query2, "Success", "Password Update", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                            ViewBag.msg = "Password Updated";
                            Logout();
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_login", db.getrowid("select Id from tbl_login where Emailid='" + Session["amail"].ToString() + "'").ToString(), query2, "Failed", "Password Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Password Update Failed";
                        }
                    }
                }
                else
                {
                    if (npass != cnpass)
                    {
                        ViewBag.msg = "New Password and Confirm Password Not matched.";
                    }
                    else
                    {
                        string query = "Update tbl_login set Password='" + npass + "' where Userid='" + emp + "' ";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins("tbl_login", "", query, "Success", "Password Update", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            //ViewBag.msg = "Password Updated";

                            string query1 = "Update tbl_registration set pass='" + npass + "' where Employee_id='" + emp + "' ";
                            if (db.InsertUpdateDelete(query1))
                            {
                                DataTable sdt = db.GetAllRecord("select Mobile_no,Email from tbl_registration where Employee_id='" + emp + "'");

                                string bodytext = "Your password has been updated By Admin. Your nepassword is " + npass + "";

                                //Messaging.SendMailEmployee("",sdt.Rows[0]["Email"]+"", "Registration Successfully", bodytext);
                                //Messaging.SendSMS(sdt.Rows[0]["Mobile_no"]+"", bodytext, "", "", "", "");
                                //Messaging.SendWhatsappSMS(sdt.Rows[0]["Mobile_no"]+"", bodytext);

                                activitylog.Activitylogins("tbl_registration", "", query1, "Success", "Password Update", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                ViewBag.msg = "Password Updated";

                            }
                            else
                            {
                                activitylog.Activitylogins("tbl_registration", "", query1, "Failed", "Password Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                                ViewBag.msg = "Password Update Failed";
                            }
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_login", "", query, "Failed", "Password Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Password Update Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public ActionResult Logout()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {
                activitylog.Activitylogins("", "", "", "Success", "Logout Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                //db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 0 + "',Logoutdatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + Session["auid"] + "' and Companycode='" + Session["auid"] + "'");
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                Response.Redirect("/Home/Login");
            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult ApproveRegistration(string Ap)
        {
            string msg = "";
            try
            {
                string query = "Update tbl_registration set Status='Approved' where id='" + Ap + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_registration", Ap, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Approved";
                }
                else
                {
                    activitylog.Activitylogins("tbl_registration", Ap, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Approval Failed";
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

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BlockUser(string Bp)
        {
            string msg = "";
            try
            {
                string query = "Update tbl_registration set Status='Block' where id='" + Bp + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_registration", Bp, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Blocked";
                }
                else
                {
                    activitylog.Activitylogins("tbl_registration", Bp, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Block Failed";
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
        public ActionResult AssignLeave(int id)
        {
            string lid = Session["auid"] + "";
            if (lid != null && lid != "")
            {
                try
                {
                    string query = "select * from tbl_leave where Leave_id=" + id + "";
                    activitylog.Activitylogins("tbl_leave", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    DataTable dt = db.GetAllRecord(query);
                    activitylog.Activitylogupd("Success", "");
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.id = dt.Rows[0]["Id"] + "";
                        ViewBag.leaveid = dt.Rows[0]["Leave_id"] + "";
                        ViewBag.emprid = dt.Rows[0]["Emprowid"] + "";
                        ViewBag.empid = dt.Rows[0]["Employeeid"] + "";
                        ViewBag.empname = dt.Rows[0]["Name"] + "";
                        ViewBag.Premise = dt.Rows[0]["Premise"] + "";
                        ViewBag.Regionid = dt.Rows[0]["Regionid"] + "";
                        ViewBag.depart = dt.Rows[0]["Department"] + "";
                        ViewBag.desig = dt.Rows[0]["Designation"] + "";
                        ViewBag.leavetype = dt.Rows[0]["Leave_type"] + "";
                        ViewBag.duratuon = dt.Rows[0]["Leaveduration"] + "";
                        ViewBag.fromdate = dt.Rows[0]["From_date"] + "";
                        ViewBag.todate = dt.Rows[0]["To_date"] + "";
                        ViewBag.totalday = dt.Rows[0]["Total_day"] + "";
                        ViewBag.logid = dt.Rows[0]["LogId"] + "";
                        ViewBag.logname = dt.Rows[0]["Logname"] + "";
                        ViewBag.reason = dt.Rows[0]["Reason"] + "";
                        DateTime date = DateTime.Parse(dt.Rows[0]["Date"] + "");
                        ViewBag.reqdate = date.ToString("yyyy-MM-dd");
                        ViewBag.attatchment = dt.Rows[0]["Attachment"] + "";
                        DateTime fdate = DateTime.Parse(dt.Rows[0]["From_date"] + "");
                        ViewBag.fromDate = fdate.ToString("yyyy-MM-dd");
                        DateTime tdate = DateTime.Parse(dt.Rows[0]["To_date"] + "");
                        ViewBag.toDate = tdate.ToString("yyyy-MM-dd");

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

                    }
                    ViewBag.msg = "Error";
                }
                finally
                {
                    db.connectionstate();
                }
            }
            else
            {
                Response.Redirect("/Home/Login");
            }


            return View();
        }
        [HttpPost]
        public ActionResult AssignLeave(FormCollection form)
        {
            try
            {
                string query = "", regquery = "", empid = "";
                DateTime FDT = Convert.ToDateTime(form["employeefromdate"]);
                DateTime TDT = Convert.ToDateTime(form["employeetodate"]);
                TimeSpan difference = TDT - FDT;
                var days = (difference.TotalDays) + 1;

                string descemp = "Leave applied from " + FDT.ToString("yyyy-MM-dd") + " to " + TDT.ToString("yyyy-MM-dd") + " has been " + form["employeestatus"] + "";

                if (form["employeestatus"] == "Rejected")
                {
                    int getpendingleave;
                    query = "INSERT INTO tbl_leave(Leave_id,Emprowid,Employeeid,Name,Leaveduration,From_date,To_date,Total_day,Reason,Remark,Date,Status,Attachment,Approvaldate,Leave_type,Department,Designation,Logname,LogId,Managername,Managercode,BranchName,BranchCode,Premise,Regionid) VALUES('" + form["leaveid"] + "','" + form["emprid"] + "','" + form["employeeid"] + "','" + form["employeename"] + "','" + form["employeedur"] + "','" + FDT.ToString("yyyy-MM-dd") + "','" + TDT.ToString("yyyy-MM-dd") + "','" + days + "','" + form["reason"] + "','" + form["Remark"] + "','" + form["reqdate"] + "','" + form["employeestatus"] + "','" + form["attatchment"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + form["employeeltype"] + "','" + form["employeedepartment"] + "','" + form["employeedesig"] + "','" + Session["auname"] + "','" + Session["auid"] + "','" + form["employeemanagername"] + "','" + form["employeemanagerid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["premise"] + "','" + form["regionid"] + "')";

                    string upquery = "update tbl_leave set Status='Inactive' where Leave_id='" + form["leaveid"] + "'";

                    string selquery = "select * from tbl_registration where Id='" + form["emprid"] + "'";
                    activitylog.Activitylogins("tbl_registration", "", selquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    DataTable sdt = db.GetAllRecord(selquery); ;
                    activitylog.Activitylogupd("Success", "");
                    if (sdt.Rows.Count > 0)
                    {
                        empid = sdt.Rows[0]["Id"].ToString();
                        getpendingleave = Convert.ToInt32(sdt.Rows[0]["Pendingleave"]);
                        //gettotalleave = Convert.ToInt32(sdt.Rows[0]["Totalleave"]);
                        //getbalanceleave = Convert.ToInt32(sdt.Rows[0]["Balanceleave"]);
                        //getapproveleave = Convert.ToInt32(sdt.Rows[0]["Approveleave"]);
                        //getLPWleave = Convert.ToInt32(sdt.Rows[0]["LWPleave"]);
                        //DateTime date = DateTime.ParseExact(sdt.Rows[0]["Dateofjoining"] + "", "d/M/yyyy", null);
                        ////joiningdate = date.ToString();
                        //DateTime currentDate = DateTime.Now;
                        //TimeSpan duration = currentDate - date;
                        //totalDays = Convert.ToInt32(duration.TotalDays);
                    }
                    else
                    {
                        getpendingleave = 0;
                        //gettotalleave = 0;
                        //getbalanceleave = 0;
                        //getapproveleave = 0;
                        //getLPWleave = 0;
                        //totalDays = 0;
                    }
                    //int totalLWPleave = getLPWleave + Convert.ToInt32(days);
                    int totalpendingleave = getpendingleave - 1;
                    //totalapproveleave = getapproveleave + Convert.ToInt32(days);
                    regquery = "update tbl_registration set Pendingleave=" + totalpendingleave + "  where Id='" + form["emprid"] + "'";
                    if (db.InsertUpdateDelete(regquery) && db.InsertUpdateDelete(upquery) && db.InsertUpdateDelete(query))
                    {
                        string header = "Leave " + form["employeestatus"] + " HR Head";
                        string insquery = "insert into tbl_notification(Employeeid,Employeename,Notification_Header,Notification_Body,Status,Date_time,BranchName,BranchCode) values ('" + form["employeeid"] + "','" + form["employeename"] + "','" + header + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["auname"] + "','" + Session["auid"] + "'),('" + form["employeemanagerid"] + "','" + form["employeemanagername"] + "','" + header + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                        if (db.InsertUpdateDelete(insquery))
                        {
                            if (form["premise"] == "Field Work")
                            {
                                Messaging.SendPushNotification(header, descemp, form["employeemanagerid"] + "");
                            }
                            Messaging.SendPushNotification(header, descemp, form["employeeid"]);

                            activitylog.Activitylogins("tbl_notification", db.getmaxid("tbl_notification").ToString(), insquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            //ViewBag.AlertMessage = "Notification Send";

                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_notification", "", insquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            //ViewBag.AlertMessage = "Notification Sending Failed";
                        }
                        activitylog.Activitylogins("tbl_registration", empid, regquery, "Success", "Update Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        activitylog.Activitylogins("tbl_leave", form["rvid"], upquery, "Success", "Update Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        activitylog.Activitylogins("tbl_leave", db.getmaxid("tbl_leave").ToString(), query, "Success", "Insert Succcess", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        Response.Write("<script>window.location.href = '/Hr/Leave'</script>");
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_registration", empid, regquery, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        activitylog.Activitylogins("tbl_leave", form["rvid"], upquery, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        activitylog.Activitylogins("tbl_leave", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Save Error";
                    }
                }
                else
                {
                    query = "INSERT INTO tbl_leave(Leave_id,Emprowid,Employeeid,Name,Leaveduration,From_date,To_date,Total_day,Reason,Remark,Date,Status,Attachment,Approvaldate,Leave_type,Department,Designation,Logname,LogId,Managername,Managercode,BranchName,BranchCode,Premise,Regionid) VALUES('" + form["leaveid"] + "','" + form["emprid"] + "','" + form["employeeid"] + "','" + form["employeename"] + "','" + form["employeedur"] + "','" + FDT.ToString("yyyy-MM-dd") + "','" + TDT.ToString("yyyy-MM-dd") + "','" + days + "','" + form["reason"] + "','" + form["Remark"] + "','" + form["reqdate"] + "','" + form["employeestatus"] + "','" + form["attatchment"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + form["employeeltype"] + "','" + form["employeedepartment"] + "','" + form["employeedesig"] + "','" + Session["auname"] + "','" + Session["auid"] + "','" + form["employeemanagername"] + "','" + form["employeemanagerid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["premise"] + "','" + form["regionid"] + "')";
                    string upquery = "update tbl_leave set Status='Inactive' where Leave_id='" + form["leaveid"] + "' ";
                    if (db.InsertUpdateDelete(upquery) && db.InsertUpdateDelete(query))
                    {
                        string header = "Leave " + form["employeestatus"] + " HR Head";
                        string insquery = "insert into tbl_notification(Employeeid,Employeename,Notification_Header,Notification_Body,Status,Date_time,BranchName,BranchCode) values ('" + form["employeeid"] + "','" + form["employeename"] + "','" + header + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["auname"] + "','" + Session["auid"] + "'),('" + form["employeemanagerid"] + "','" + form["employeemanagername"] + "','" + header + "','" + descemp + ".','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                        if (db.InsertUpdateDelete(insquery))
                        {
                            if (form["premise"] == "Field Work")
                            {
                                Messaging.SendPushNotification(header, descemp, form["employeemanagerid"] + "");
                            }
                            Messaging.SendPushNotification(header, descemp, form["employeeid"]);

                            activitylog.Activitylogins("tbl_notification", db.getmaxid("tbl_notification").ToString(), insquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            //ViewBag.AlertMessage = "Notification Send";

                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_notification", "", insquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            //ViewBag.AlertMessage = "Notification Sending Failed";
                        }
                        Response.Write("<script>window.location.href = '/Hr/Leave'</script>");
                    }
                    else
                    {
                        ViewBag.msg = "Data Save Error";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        private string random()
        {
            var random = new Random();
            var numlength = 5;
            var number = string.Empty;
            var possibleChar = "1234567890";

            for (var i = 0; i < numlength; i++)
            {
                var randomIndex = random.Next(0, possibleChar.Length - 1);
                number += possibleChar[randomIndex];
            }
            return number;
        }
        public JsonResult GetDesignation(string Depart)
        {
            string res = "";
            try
            {
                res = "<option selected value=''>Select one</option>";
                string query = "select * from tbl_designation where Departmentid='" + Depart + "' and Status='Active'";
                activitylog.Activitylogins("tbl_designation", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Status"] + "" == "Active")
                        {
                            res += "<option value='" + dt.Rows[i]["Id"] + "'>" + dt.Rows[i]["Designation"] + "</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }

                catch
                {

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Managerdetails(string Depart)
        {
            string res = "";
            try
            {
                res = "<option selected disabled value=''>Select one</option>";
                string query = "select * from tbl_registration where Department_name='" + Depart + "' and Status='Approved' and Branchcode='" + Session["abrcode"] + "'";
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        res += "<option value='" + dt.Rows[i]["Id"] + "'>" + dt.Rows[i]["Name"] + "</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }

                catch
                {

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmpDetails(string Empid)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_registration where Id='" + Empid + "'";
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["id"] + "";
                    string empid = dt.Rows[0]["Employee_id"] + "";
                    string emptype = dt.Rows[0]["Employee_Type"] + "";
                    string empname = dt.Rows[0]["Name"] + "";
                    string empmobile = dt.Rows[0]["Mobile_no"] + "";
                    string empmail = dt.Rows[0]["Email"] + "";
                    string empdepart = dt.Rows[0]["Department_name"] + "";
                    string empdesig = dt.Rows[0]["Designation"] + "";
                    string empmanagername = dt.Rows[0]["Managername"] + "";
                    string empmanagercode = dt.Rows[0]["Managercode"] + "";

                    string shiftname = dt.Rows[0]["Shiftname"] + "";
                    string starttime = dt.Rows[0]["Shiftstarttime"] + "";
                    string endtime = dt.Rows[0]["Shiftendtime"] + "";
                    string shifttiming = dt.Rows[0]["Shifttiming"] + "";

                    string doj = dt.Rows[0]["Dateofjoining"] + "";
                    res = new string[15] { id, empid, emptype, empname, empmobile, empmail, empdepart, empdesig, empmanagername, empmanagercode, doj, shiftname, starttime, endtime, shifttiming };
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

                }

            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShiftdata(string Shiftid)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_shift where Id='" + Shiftid + "' and Status='Active'";
                activitylog.Activitylogins("tbl_shift", Shiftid, query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string name = dt.Rows[0]["Shiftname"] + "";
                    string starttime = dt.Rows[0]["Starttime"] + "";
                    string endtime = dt.Rows[0]["Endtime"] + "";
                    string status = dt.Rows[0]["Status"] + "";



                    res = new string[5] { id, name, starttime, endtime, status };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
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
        public ActionResult Notification()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Notification(string empid, string desig, string depart, string manager, string title, string description)
        {
            try
            {
                string subquery = ""; string query = "", Employeeid = "", Employeename = "", inssubquery = "";
                if (empid != null && empid != "")
                {
                    string[] empArray = empid.Split(',');
                    foreach (string id in empArray)
                    {
                        subquery += " Employee_id='" + id + "' or";
                    }
                }
                if (desig != null && desig != "")
                {
                    string[] desigArray = desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        subquery += " Designation='" + designame + "' or";
                    }
                }
                if (depart != null && depart != "")
                {
                    string[] departArray = depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        subquery += " Department_name='" + departnm + "' or";
                    }
                }
                if (manager != null && manager != "")
                {
                    string[] managerArray = manager.Split(',');
                    foreach (string managernm in managerArray)
                    {
                        subquery += " Managername='" + managernm + "' or";
                    }
                }
                if (subquery.EndsWith(" or"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 3);
                }
                if (subquery != null && subquery != "")
                {
                    subquery = " and (" + subquery + ")";
                }
                query = "select Employee_id,Name from tbl_registration where BranchCode='" + Session["abrcode"] + "' " + subquery + "";

                ViewBag.msg = query;
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query); ;
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Employeeid += dt.Rows[i]["Employee_id"] + ",";
                        Employeename += dt.Rows[i]["Name"] + ",";
                    }
                    Employeeid = Employeeid.Substring(0, Employeeid.Length - 1);
                    Employeename = Employeename.Substring(0, Employeename.Length - 1);
                    string[] Employeearray = Employeeid.Split(',');
                    string[] Employeenmarray = Employeename.Split(',');

                    for (int i = 0; i < Employeearray.Length; i++)
                    {
                        inssubquery += "('" + Employeearray[i] + "','" + Employeenmarray[i] + "','" + title + "','" + description + "','Unread','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["auname"] + "','" + Session["auid"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "'),";
                    }

                    if (inssubquery.EndsWith(","))
                    {
                        inssubquery = inssubquery.Substring(0, inssubquery.Length - 1);
                    }
                    string insquery = "insert into tbl_notification(Employeeid,Employeename,Notification_Header,Notification_Body,Status,Date_time,logname,logid,BranchName,BranchCode) values " + inssubquery + "";
                    if (db.InsertUpdateDelete(insquery))
                    {
                        for (int i = 0; i < Employeearray.Length; i++)
                        {
                            Messaging.SendPushNotification(title, description, Employeearray[i]);
                        }
                        activitylog.Activitylogins("tbl_notification", db.getmaxid("tbl_notification").ToString(), insquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.AlertMessage = "Notification Send";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_notification", "", insquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.AlertMessage = "Notification Sending Failed";
                    }
                }
                else
                {
                    ViewBag.AlertMessage = "No Employee Found";
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

                }
                ViewBag.AlertMessage = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return View();
        }
        public JsonResult GetDesignationrep(string Depart)
        {
            string res = "";
            try
            {
                res = "<option selected value=''>Select one</option>";
                string query = "select * from tbl_designation where Department='" + Depart + "' and Status='Active'";
                activitylog.Activitylogins("tbl_designation", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Status"] + "" == "Active")
                        {
                            res += "<option value='" + dt.Rows[i]["Designation"] + "'>" + dt.Rows[i]["Designation"] + "</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }

                catch
                {

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeerep(string Desig, string Depart)
        {
            string res = "";
            try
            {

                res = "<option selected value=''>Select one</option>";
                string query = "select * from tbl_registration where Status='Approved' and Branchcode='" + Session["abrcode"] + "'";
                if (Depart != "" && Depart != null)
                {
                    query += " and Department_name='" + Depart + "'";
                }
                if (Desig != "" && Desig != null)
                {
                    query += " and Designation='" + Desig + "'";
                }

                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Status"] + "" == "Approved")
                        {
                            res += "<option value='" + dt.Rows[i]["Employee_id"] + "'>" + dt.Rows[i]["Name"] + " (" + dt.Rows[i]["Branchname"] + ")</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }

                catch
                {

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Support Module Open 

        public JsonResult Statuschangetive(string id, string status, string tblnm)
        {
            string msg = "";
            try
            {
                string squery = "select Status from " + tblnm + " where Id='" + id + "'";
                DataTable dt = db.GetAllRecord(squery);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Status"] + "" == "Active")
                    {
                        string query = "Update " + tblnm + " set Status='Inactive' where Id='" + id + "'";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Inactive";
                        }
                        else
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Failed";
                        }
                    }
                    else if (dt.Rows[0]["Status"] + "" == "Inactive")
                    {
                        string query = "Update " + tblnm + " set Status='Active' where Id='" + id + "'";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Active";
                        }
                        else
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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

        // Support Module Close



        // Inventry Module Open
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult BProductStock()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult BProductStock(FormCollection form)
        {
            string xmlData = "";
            try
            {
                DateTime currentDate = DateTime.Now;
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string requestid = "R" + currentYearmonth + "" + arandom(5);
                string query = "INSERT INTO [dbo].[tbl_product_distribution] ([Requestid],[ProductId] ,[ProductBrandName] ,[ProductName] ,[Quntity] ,[Unit_type] ,[RequestProductQuantity] ,[Remark] ,[BranchName] ,[BranchCode] ,[RequestStatus] ,[Status] ,[Logname] ,[Logid] ,[Date_time]) VALUES ('" + requestid + "','" + form["productid"] + "','" + form["productbrandname"] + "','" + form["productname"] + "','" + form["quantity"] + "','" + form["unittype"] + "','" + form["addstock"] + "','" + form["remark"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','Requested','Active','" + Session["auname"] + "','" + Session["auid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_product_distribution", db.getmaxid("tbl_product_distribution").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Request Sent";
                }
                else
                {
                    activitylog.Activitylogins("tbl_product_distribution", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Request Not Sent";
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
                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }

        public JsonResult ReturnProductStock()
        {
            string res = "";
            var productid = Request.Form["productid"];
            var productbrandname = Request.Form["productbrandname"];
            var productname = Request.Form["productname"];
            var quantity = Request.Form["quantity"];
            var unittype = Request.Form["unittype"];
            var addstock = Request.Form["addstock"];
            var remark = Request.Form["remark"];
            try
            {
                DateTime currentDate = DateTime.Now;
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string requestid = "R" + currentYearmonth + "" + arandom(5);
                string query = "INSERT INTO [dbo].[tbl_product_distribution] ([Requestid],[ProductId] ,[ProductBrandName] ,[ProductName] ,[Quntity] ,[Unit_type] ,[RequestProductQuantity] ,[Remark] ,[BranchName] ,[BranchCode] ,[RequestStatus] ,[Status] ,[Logname] ,[Logid] ,[Date_time]) VALUES ('" + requestid + "','" + productid + "','" + productbrandname + "','" + productname + "','" + quantity + "','" + unittype + "','" + addstock + "','" + remark + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','Return','Active','" + Session["auname"] + "','" + Session["auid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_product_distribution", db.getmaxid("tbl_product_distribution").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Request Sent";
                }
                else
                {
                    activitylog.Activitylogins("tbl_product_distribution", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Request Not Sent";
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
                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public string ConvertXmlToJson(string xmlString)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);

                // Convert XML to JSON using Newtonsoft.Json
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                // Convert the JSON string to JObject and extract the relevant part
                JObject jsonObject = JObject.Parse(jsonText);
                JToken dataToken = jsonObject["Data"]["tableData"]["row"];

                // If there are multiple rows, we want it as an array
                if (dataToken is JArray)
                {
                    jsonObject["Data"]["tableData"]["row"] = dataToken;
                }
                else
                {
                    // If there's only one row, wrap it in an array
                    JArray jsonArray = new JArray();
                    jsonArray.Add(dataToken);
                    jsonObject["Data"]["tableData"]["row"] = jsonArray;
                }

                return jsonObject.ToString();
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return "";
        }


        public JsonResult GetProductStock(string Productid, string branchid)
        {
            string[] res = { };
            try
            {
                string query = "select tbl_pro_price_circlar.Id,tbl_pro_price_circlar.Brand,tbl_pro_price_circlar.Quantity,tbl_pro_price_circlar.ProductCategory,tbl_pro_price_circlar.punit,tbl_pro_price_circlar.Balance_stock,tbl_pro_price_circlar.Ptype,tbl_pro_price_circlar.Productname,tbl_productstock.BranchCode,tbl_productstock.BranchName,tbl_productstock.Credit,tbl_productstock.Debit,tbl_productstock.Balance from tbl_pro_price_circlar Left join tbl_productstock on tbl_pro_price_circlar.Id=tbl_productstock.Productid where tbl_productstock.BranchCode='" + branchid + "' and tbl_pro_price_circlar.Id='" + Productid + "'";

                string pquery = "select * from tbl_pro_price_circlar where Id='" + Productid + "'";
                DataTable pdt = db.GetAllRecord(pquery);

                activitylog.Activitylogins("tbl_pro_price_circlar", Productid, query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string BranchCode = dt.Rows[0]["BranchCode"] + "";
                    if (BranchCode == branchid)
                    {

                    }
                    else
                    {
                        dt.Rows[0]["Credit"] = 0;
                        dt.Rows[0]["Debit"] = 0;
                        dt.Rows[0]["Balance"] = 0;
                    }
                    string id = dt.Rows[0]["Id"] + "";
                    string Brand = dt.Rows[0]["Brand"] + "";
                    string Quantity = dt.Rows[0]["Quantity"] + "";
                    string ProductCategory = dt.Rows[0]["ProductCategory"] + "";
                    string punit = dt.Rows[0]["punit"] + "";
                    string Balance_stock = dt.Rows[0]["Balance_stock"] + "";
                    string Ptype = dt.Rows[0]["Ptype"] + "";
                    string Productname = dt.Rows[0]["Productname"] + "";
                    string Credit = dt.Rows[0]["Credit"] + "";
                    string Debit = dt.Rows[0]["Debit"] + "";
                    string Balance = dt.Rows[0]["Balance"] + "";

                    res = new string[12] { id, Brand, Productname, Quantity, ProductCategory, punit, Balance_stock, Ptype, Credit, Debit, Balance, BranchCode };

                }
                else if (pdt.Rows.Count > 0)
                {
                    string id = pdt.Rows[0]["Id"] + "";
                    string Brand = pdt.Rows[0]["Brand"] + "";
                    string Quantity = pdt.Rows[0]["Quantity"] + "";
                    string ProductCategory = pdt.Rows[0]["ProductCategory"] + "";
                    string punit = pdt.Rows[0]["punit"] + "";
                    string Balance_stock = pdt.Rows[0]["Balance_stock"] + "";
                    string Ptype = pdt.Rows[0]["Ptype"] + "";
                    string Productname = pdt.Rows[0]["Productname"] + "";
                    string Credit = "0";
                    string Debit = "0";
                    string Balance = "0";
                    string BranchCode = "";

                    res = new string[12] { id, Brand, Productname, Quantity, ProductCategory, punit, Balance_stock, Ptype, Credit, Debit, Balance, BranchCode };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProductStock(string Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_productstock  where id='" + Up + "'";
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    DataTable dt2 = db.GetAllRecord("select Ptype from tbl_pro_price_circlar where Id='" + dt.Rows[0]["Productid"] + "'");
                    string id = dt.Rows[0]["Id"] + "";
                    string Productid = dt.Rows[0]["Productid"] + "";
                    string Productname = dt.Rows[0]["Productname"] + "";
                    string Credit = dt.Rows[0]["Credit"] + "";
                    string Debit = dt.Rows[0]["Debit"] + "";
                    string balance = dt.Rows[0]["Balance"] + "";
                    string BranchName = dt.Rows[0]["BranchName"] + "";
                    string BranchCode = dt.Rows[0]["BranchCode"] + "";
                    string GodownName = dt.Rows[0]["Godownname"] + "";
                    string GodownCode = dt.Rows[0]["GodownCode"] + "";
                    string Ptype = dt2.Rows[0]["Ptype"] + "";

                    res = new string[11] { id, Productid, Productname, Credit, Debit, balance, Ptype, BranchName, BranchCode, GodownName, GodownCode };

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
        public JsonResult GetProductDesrequest(string Id)
        {
            string json = "";
            try
            {
                string attquery = "SELECT * from tbl_product_distribution where Id='" + Id + "'";
                activitylog.Activitylogins("tbl_product_distribution", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(attdt, Formatting.None);
                }
                else
                {
                    json = "Data Not Found";
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReceiveProductStock()
        {
            string res = "";
            var requestid = Request.Form["requestid"];
            var productid = Request.Form["productid"];
            var productbrandname = Request.Form["productbrandname"];
            var productname = Request.Form["productname"];
            var quantity = Request.Form["quantity"];
            var unittype = Request.Form["unittype"];
            var addstock = Request.Form["addstock"];
            var remark = Request.Form["remark"];
            try
            {
                string reqquery = "select * from tbl_product_distribution where Requestid='" + requestid + "' and Status='Active'";
                DataTable reqdt = db.GetAllRecord(reqquery);
                if (reqdt.Rows.Count > 0)
                {
                    string reqiquery = "INSERT INTO [dbo].[tbl_product_distribution] ([Requestid],[ProductId] ,[ProductBrandName] ,[ProductName] ,[Quntity] ,[Unit_type] ,[RequestProductQuantity] ,[Remark] ,[BranchName] ,[BranchCode] ,[RequestStatus] ,[Status] ,[Logname] ,[Logid] ,[Date_time]) VALUES ('" + requestid + "','" + reqdt.Rows[0]["ProductId"] + "','" + reqdt.Rows[0]["ProductBrandName"] + "','" + reqdt.Rows[0]["ProductName"] + "','" + reqdt.Rows[0]["Quntity"] + "','" + reqdt.Rows[0]["Unit_type"] + "','" + reqdt.Rows[0]["RequestProductQuantity"] + "','" + remark + "','" + reqdt.Rows[0]["BranchName"] + "','" + reqdt.Rows[0]["BranchCode"] + "','Received','Active','" + Session["auname"] + "','" + Session["auid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                    db.InsertUpdateDelete("update tbl_product_distribution set Status='Inactive' where Requestid='" + requestid + "'");

                    if (db.InsertUpdateDelete(reqiquery))
                    {
                        activitylog.Activitylogins("tbl_product_distribution", db.getmaxid("tbl_product_distribution").ToString(), reqiquery, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Request Approved";

                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_product_distribution", "", reqiquery, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Request Not Approved";
                    }
                }
                else
                {
                    ViewBag.msg = "Request Not Found";
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
                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        // Inventry Module Close





















        public ActionResult EmployeeDirectory()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult EmpJoining()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Empworking()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Empsalary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }

        public ActionResult Customer()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Purpose()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }

        public ActionResult LeaveMaster()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult LeaveType()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult LeaveType(string hid, string leavetype, string isattatch, string isshort, string status)
        {
            try
            {
                string acode = Session["auid"] + "";
                string auname = Session["auname"] + "";
                if (hid != "" && hid != null)
                {
                    string query = "update tbl_leave_type set Leavetype='" + leavetype + "',Isattatchment='" + isattatch + "',Shortleave='" + isshort + "',Status='" + status + "',Datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Companyid='" + acode + "' and Id='" + hid + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_leave_type", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_leave_type", hid, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Update failed";
                    }
                }
                else
                {
                    string query = "insert into tbl_leave_type(Leavetype,Isattatchment,Shortleave,Status,Companyname,Companyid,Datetime) values('" + leavetype + "','" + isattatch + "','" + isshort + "','" + status + "','" + auname + "','" + acode + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_leave_type", db.getmaxid("tbl_leave_type").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Saved";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_leave_type", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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



        public ActionResult DailyPerformance()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Monthsummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Daysummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult DayTimesummary()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult EmpJoiningReport()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult GetWorkRecord(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            string res = "", tbldata = "", tbl = "", subquery = " Where tbl_registration.Companyid='" + Session["auid"] + "' and tbl_registration.Status='Approved' ", regsubquery = "";
            StringBuilder htmlTable = new StringBuilder();
            try
            {
                int count = 1;
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    count++;
                }
                int addday = count - 2;
                int span = count + 1;
                DateTime startDate = new DateTime(Year, Month, 1);
                DateTime endDate = startDate.AddDays(addday);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date != null)
                    {
                        regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " AND MONTH(tbl_attendance.Date) = " + Month + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " AND MONTH(tbl_attendance.Date) = " + Month + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " AND MONTH(tbl_attendance.Date) = " + Month + "  THEN ISNULL(tbl_attendance.Working_hours,'' ) ELSE NULL END) AS WH" + date.Day + "," +
    "GROUP_CONCAT(IF(DAY(tbl_checkin.Date) = DAY(tbl_attendance.Date) AND DAY(tbl_attendance.Date) =" + date.Day + ", ISNULL(tbl_checkin.Checkin_time, ''), '') SEPARATOR ',') AS CI" + date.Day + "," +
    "GROUP_CONCAT(IF(DAY(tbl_checkin.Date) = DAY(tbl_attendance.Date) AND DAY(tbl_attendance.Date) = " + date.Day + ", ISNULL(tbl_checkin.Checkout_time, ''), '') SEPARATOR ',') AS CO" + date.Day + ",";


                    }

                }
                regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

                string empquery = "", desigquery = "", departquery = "", managerquery = "";
                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " tbl_attendance.Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += " and (" + managerquery + ")";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " tbl_attendance.Employeeid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += " and (" + empquery + ")";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " tbl_attendance.Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " and (" + desigquery + ")";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " tbl_attendance.Department='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " and (" + departquery + ")";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }

                //string squery = "select * from tbl_attendance where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "' and Companyid='" + Session["auid"] + "' and ("+subquery+")";
                string attquery = "select tbl_attendance.Employeeid,tbl_attendance.Name,tbl_attendance.Department,tbl_attendance.Designation,tbl_attendance.Starttime,tbl_attendance.Endtime," + regsubquery + " from tbl_registration LEFT JOIN (tbl_attendance LEFT JOIN tbl_checkin ON tbl_attendance.Employeeid = tbl_checkin.Employee_id AND MONTH(tbl_attendance.Date) = " + Month + ") ON tbl_registration.Employee_id = tbl_attendance.Employeeid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY tbl_attendance.Employeeid,tbl_attendance.Name,tbl_attendance.Department,tbl_attendance.Designation,tbl_attendance.Starttime,tbl_attendance.Endtime";
                activitylog.Activitylogins("tbl_attendance,tbl_checkin", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {
                    htmlTable.Append("<tbody class='text-center'>");
                    for (int i = 0; i < attdt.Rows.Count; i++)
                    {
                        DateTime starttime = DateTime.ParseExact(attdt.Rows[i]["Starttime"] + "", "H:m:ss", CultureInfo.InvariantCulture);
                        DateTime endtime = DateTime.ParseExact(attdt.Rows[i]["Endtime"] + "", "H:m:ss", CultureInfo.InvariantCulture);
                        htmlTable.Append("<tr class='bg-dark text-light'>");
                        htmlTable.Append("<td colspan='" + span + "' class='text-start'> <b>Name:</b> " + attdt.Rows[i]["Name"] + "(" + attdt.Rows[i]["Employeeid"] + ") <b>Department:</b> " + attdt.Rows[i]["Department"] + " <b>Designation:</b> " + attdt.Rows[i]["Designation"] + " <b>Shift Start Time:</b> " + starttime.ToString("HH:mm") + " <b>Shift End Time:</b> " + endtime.ToString("HH:mm") + "</td>");

                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td></td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            htmlTable.Append("<td>" + date.Day + "</td>");

                        }
                        htmlTable.Append("</tr>");

                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>In :</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                DateTime pidate = DateTime.ParseExact(attdt.Rows[i]["PI" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + pidate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");

                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>Out :</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                DateTime podate = DateTime.ParseExact(attdt.Rows[i]["PO" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + podate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>Work Hours :</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["WH" + date.Day + ""] + "" != "" && attdt.Rows[i]["WH" + date.Day + ""] + "" != null)
                            {
                                DateTime whdate = DateTime.ParseExact(attdt.Rows[i]["WH" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                htmlTable.Append("<td>" + whdate.ToString("HH:mm") + "</td>");
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        //htmlTable.Append("<tr>");
                        //htmlTable.Append("<td>Overtime:</td>");
                        //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        //{
                        //    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                        //    {
                        //        htmlTable.Append("<td>" + attdt.Rows[i]["PI" + date.Day + ""] + "</td>");
                        //    }
                        //    else
                        //    {
                        //        htmlTable.Append("<td> - </td>");
                        //    }

                        //}
                        //htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>Checkin Time:</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["CI" + date.Day + ""] + "" != "" && attdt.Rows[i]["CI" + date.Day + ""] + "" != null)
                            {
                                string cival = "";
                                string ciValue = attdt.Rows[i]["CI" + date.Day].ToString();
                                string[] ciArray = ciValue.Split(',');
                                foreach (string ci in ciArray)
                                {
                                    if (ci != null & ci != "")
                                    {
                                        DateTime cidate = DateTime.ParseExact(ci + "", "HH:mm:ss", CultureInfo.InvariantCulture);

                                        cival += cidate.ToString("HH:mm") + " | ";
                                    }
                                }
                                if (cival.EndsWith(" | "))
                                {
                                    cival = cival.Substring(0, cival.Length - 3);
                                }
                                if (cival != null & cival != "")
                                {
                                    htmlTable.Append("<td>" + cival + "</td>");
                                }
                                else
                                {
                                    htmlTable.Append("<td> - </td>");
                                }
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>Checkout Time:</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (attdt.Rows[i]["CO" + date.Day + ""] + "" != "" && attdt.Rows[i]["CO" + date.Day + ""] + "" != null)
                            {
                                string coval = "";
                                string coValue = attdt.Rows[i]["CO" + date.Day].ToString();
                                string[] coArray = coValue.Split(',');
                                foreach (string co in coArray)
                                {
                                    if (co != null & co != "")
                                    {
                                        DateTime codate = DateTime.ParseExact(co + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                        coval += codate.ToString("HH:mm") + " | ";
                                    }
                                }
                                if (coval.EndsWith(" | "))
                                {
                                    coval = coval.Substring(0, coval.Length - 3);
                                }
                                if (coval != null & coval != "")
                                {
                                    htmlTable.Append("<td>" + coval + "</td>");
                                }
                                else
                                {
                                    htmlTable.Append("<td> - </td>");
                                }
                            }
                            else
                            {
                                htmlTable.Append("<td> - </td>");
                            }

                        }
                        htmlTable.Append("</tr>");

                    }
                    htmlTable.Append("</tbody>");
                }

                else
                {
                    htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
                }
                //DataTable dt = db.GetAllRecord(squery);
                string json = JsonConvert.SerializeObject(attdt, Formatting.None);
                //return View();
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

                }
                htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
            }
            finally
            {
                db.connectionstate();
            }

            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSalaryRecord(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            //Working Process
            string res = "", tbldata = "", tbl = "", subquery = " Where tbl_attendance.Companyid='" + Session["auid"] + "' ", regsubquery = ""; int holidaycount = 0;
            string holidayquery = "select * from tbl_Holiday where MONTH(Date)='" + Month + "' and YEAR(Date)='" + Year + "' and Companyid='" + Session["auid"] + "' and Status='Active'";
            DataTable holidaydt = db.GetAllRecord(holidayquery);
            if (holidaydt.Rows.Count > 0)
            {
                holidaycount = holidaydt.Rows.Count;
            }
            else
            {
                holidaycount = 0;
            }
            //string leavequery = "select * from tbl_holiday where MONTH(From_date)='" + Month + "' and YEAR(From_date)='" + Year + "' and Companyid='" + Session["auid"] + "'";
            //DataTable leavedt = db.GetAllRecord(leavequery);
            //if (leavedt.Rows.Count > 0)
            //{
            //    for(int i = 0; i < leavedt.Rows.Count; i++)
            //    {
            //        if(leavedt.Rows[i])
            //    }
            //}
            //else
            //{

            //}
            StringBuilder htmlTable = new StringBuilder();
            int count = 1;
            for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
            {
                count++;
            }
            int addday = count - 2;
            int span = count / 2;
            int span2 = span + 1;
            DateTime startDate = new DateTime(Year, Month, 1);
            DateTime endDate = startDate.AddDays(addday);
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date != null)
                {
                    regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
"MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
"MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Working_hours,'' ) ELSE NULL END) AS WH" + date.Day + "," +
"MAX(CASE WHEN DAY(tbl_leave.From_date) = " + date.Day + " THEN ISNULL(tbl_leave.Total_day,0 ) ELSE NULL END) AS Totalleave" + date.Day + ",";

                }

            }
            regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

            string empquery = "", desigquery = "", departquery = "", managerquery = "";
            if (Managernm != null && Managernm != "")
            {
                string[] managerArray = Managernm.Split(',');
                foreach (string id in managerArray)
                {
                    managerquery += " tbl_attendance.Managercode='" + id + "' or";
                }
                managerquery = managerquery.Substring(0, managerquery.Length - 3);
                subquery += " and (" + managerquery + ")";
            }
            if (Empid != null && Empid != "")
            {
                string[] empArray = Empid.Split(',');
                foreach (string id in empArray)
                {
                    empquery += " tbl_attendance.Employeeid='" + id + "' or";
                }
                empquery = empquery.Substring(0, empquery.Length - 3);
                subquery += " and (" + empquery + ")";
            }
            if (Desig != null && Desig != "")
            {
                string[] desigArray = Desig.Split(',');
                foreach (string designame in desigArray)
                {
                    desigquery += " tbl_attendance.Designation='" + designame + "' or";
                }
                desigquery = desigquery.Substring(0, desigquery.Length - 3);
                subquery += " and (" + desigquery + ")";
            }
            if (Depart != null && Depart != "")
            {
                string[] departArray = Depart.Split(',');
                foreach (string departnm in departArray)
                {
                    departquery += " tbl_attendance.Department='" + departnm + "' or";
                }
                departquery = departquery.Substring(0, departquery.Length - 3);
                subquery += " and (" + departquery + ")";
            }
            if (subquery.EndsWith(" and"))
            {
                subquery = subquery.Substring(0, subquery.Length - 4);
            }
            //string squery = "select * from tbl_attendance where MONTH(Date)='" + Month + "' And YEAR(Date)='" + Year + "' and Companyid='" + Session["auid"] + "' and ("+subquery+")";
            string attquery = "SELECT tbl_registration.Name,tbl_registration.Employee_id,tbl_registration.Employee_type,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime," +
 regsubquery + " FROM tbl_registration LEFT JOIN tbl_attendance ON tbl_registration.Employee_id = tbl_attendance.Employeeid AND MONTH(tbl_attendance.Date) = " + Month + " LEFT JOIN tbl_leave ON tbl_registration.Employee_id = tbl_leave.Employeeid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY tbl_registration.Name,tbl_registration.Employee_id,tbl_registration.Employee_type,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Shiftstarttime,tbl_registration.Shiftendtime";
            DataTable attdt = db.GetAllRecord(attquery);
            htmlTable.Append("<tbody class='text-center'>");
            htmlTable.Append("<tr class='bg-dark text-light'>");
            htmlTable.Append("<td>ID</td>");
            htmlTable.Append("<td>Employee Id</td>");
            htmlTable.Append("<td>Name</td>");
            htmlTable.Append("<td>Department</td>");
            htmlTable.Append("<td>Designation</td>");
            htmlTable.Append("<td>Type</td>");
            htmlTable.Append("<td>Total Day</td>");
            htmlTable.Append("<td>Present</td>");
            htmlTable.Append("<td>Holiday</td>");
            htmlTable.Append("<td>Absent</td>");
            htmlTable.Append("<td>Week Off</td>");
            htmlTable.Append("<td>Leaves</td>");
            htmlTable.Append("<td>Half Day</td>");
            htmlTable.Append("<td>Total Paid Day</td>");
            htmlTable.Append("<td>Total Late Arrival</td>");
            htmlTable.Append("<td>Early Departure</td>");
            htmlTable.Append("</tr>");
            for (int i = 0; i < attdt.Rows.Count; i++)
            {
                int present = 0, absent = 0, leavecount = 0, latearrival = 0, earlydeparture = 0, halfday = 0, totalday = 0, weekofcount = 0, paiddaycount, fuldayhalf = 0;
                int j = i + 1;

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    totalday++;

                }
                DataTable weekoffdt = db.GetAllRecord("select * from tbl_weekoff where Companyid='" + Session["auid"] + "' and Status='Active'");
                if (weekoffdt.Rows.Count > 0)
                {
                    for (int wo = 0; wo < weekoffdt.Rows.Count; wo++)
                    {
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            if (date.ToString("dddd") == weekoffdt.Rows[wo]["Weekday"] + "")
                            {
                                weekofcount++;
                            }

                        }
                    }

                }
                else
                {
                    weekofcount = 0;
                }

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                    {
                        if (DateTime.Parse(attdt.Rows[i]["PI" + date.Day + ""] + "") > DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + ""))
                        {
                            latearrival++;
                        }

                    }

                }
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                    {
                        if (DateTime.Parse(attdt.Rows[i]["PO" + date.Day + ""] + "") < DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + ""))
                        {
                            earlydeparture++;
                        }

                    }

                }
                TimeSpan dur = DateTime.Parse(attdt.Rows[i]["Shiftendtime"] + "") - DateTime.Parse(attdt.Rows[i]["Shiftstarttime"] + "");
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (attdt.Rows[i]["WH" + date.Day + ""] + "" != "" && attdt.Rows[i]["WH" + date.Day + ""] + "" != null)
                    {
                        if (TimeSpan.Parse(attdt.Rows[i]["WH" + date.Day + ""] + "") < dur)
                        {
                            halfday++;
                        }

                    }

                }
                //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                //{
                //    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                //    {
                //        present++;
                //    }

                //}
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null && attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                    {
                        present++;
                    }

                }
                //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                //{
                //    if (attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != "" && attdt.Rows[i]["Totalleave" + date.Day + ""] + "" != null )
                //    {
                //        leavecount += Convert.ToInt32(attdt.Rows[i]["Totalleave" + date.Day + ""]);
                //    }

                //}
                absent = totalday - (present + holidaycount + leavecount + weekofcount);
                fuldayhalf = halfday / 2;
                paiddaycount = (present + holidaycount + weekofcount) - fuldayhalf;
                //halfday = latearrival + earlydeparture;
                htmlTable.Append("<tr class='bg-light-subtle' padding='10px'>");
                htmlTable.Append("<td>" + j + "</td>");
                htmlTable.Append("<td>" + attdt.Rows[i]["Employee_id"] + "</td>");
                htmlTable.Append("<td>" + attdt.Rows[i]["Name"] + "</td>");
                htmlTable.Append("<td>" + attdt.Rows[i]["Department_name"] + "</td>");
                htmlTable.Append("<td>" + attdt.Rows[i]["Designation"] + "</td>");
                htmlTable.Append("<td>" + attdt.Rows[i]["Employee_type"] + "</td>");
                htmlTable.Append("<td>" + totalday + "</td>");
                htmlTable.Append("<td>" + present + "</td>");
                htmlTable.Append("<td>" + holidaycount + "</td>");
                htmlTable.Append("<td>" + absent + "</td>");
                htmlTable.Append("<td>" + weekofcount + "</td>");
                htmlTable.Append("<td>" + leavecount + "</td>");
                htmlTable.Append("<td>" + halfday + "</td>");
                htmlTable.Append("<td>" + paiddaycount + "</td>");
                htmlTable.Append("<td>" + latearrival + "</td>");
                htmlTable.Append("<td>" + earlydeparture + "</td>");
                htmlTable.Append("</tr>");



            }
            htmlTable.Append("</tbody>");
            //DataTable dt = db.GetAllRecord(squery);
            string json = JsonConvert.SerializeObject(attdt, Formatting.None);
            //return View();
            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDaytimesummary(int Month, int Year, string Managernm, string Depart, string Desig, string Empid)
        {
            string subquery = " Where tbl_registration.Companyid='" + Session["auid"] + "' and tbl_registration.Status='Approved' ", regsubquery = "";
            StringBuilder htmlTable = new StringBuilder();
            try
            {
                int count = 1;
                for (int j = 1; j <= DateTime.DaysInMonth(Year, Month); j++)
                {
                    count++;
                }
                int addday = count - 2;
                DateTime startDate = new DateTime(Year, Month, 1);
                DateTime endDate = startDate.AddDays(addday);
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date != null)
                    {
                        regsubquery += "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchin_time, '') ELSE NULL END) AS PI" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Punchout_time,'' ) ELSE NULL END) AS PO" + date.Day + "," +
    "MAX(CASE WHEN DAY(tbl_attendance.Date) = " + date.Day + " THEN ISNULL(tbl_attendance.Overtime,'' ) ELSE NULL END) AS OT" + date.Day + ",";

                    }
                }
                regsubquery = regsubquery.Substring(0, regsubquery.Length - 1);

                string empquery = "", desigquery = "", departquery = "", managerquery = "";
                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " tbl_attendance.Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += " and (" + managerquery + ")";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " tbl_attendance.Employeeid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += " and (" + empquery + ")";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " tbl_attendance.Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " and (" + desigquery + ")";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " tbl_attendance.Department='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " and (" + departquery + ")";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }
                string attquery = "SELECT tbl_registration.Name,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Employee_Type,tbl_registration.Employee_id," +
     regsubquery + " FROM tbl_registration LEFT JOIN tbl_attendance ON tbl_registration.Employee_id = tbl_attendance.Employeeid AND MONTH(tbl_attendance.Date) = " + Month + " " + subquery + " GROUP BY tbl_registration.Name,tbl_registration.Department_name,tbl_registration.Designation,tbl_registration.Employee_Type,tbl_registration.Employee_id";
                activitylog.Activitylogins("tbl_registration,tbl_attendance", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {
                    htmlTable.Append("<tbody class='text-center'>");
                    htmlTable.Append("<tr class='bg-dark text-light'>");
                    htmlTable.Append("<td>Employee Code</td>");
                    //htmlTable.Append("<td>Device Person Id</td>");
                    htmlTable.Append("<td>Name</td>");
                    htmlTable.Append("<td>Department</td>");
                    htmlTable.Append("<td>Designation</td>");
                    //htmlTable.Append("<td>Type</td>");
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        htmlTable.Append("<td>" + date.Day + "</td>");

                    }
                    htmlTable.Append("<td>Total Duty</td>");
                    htmlTable.Append("<td>Totl OT(Hours)</td>");
                    htmlTable.Append("</tr>");
                    for (int i = 0; i < attdt.Rows.Count; i++)
                    {
                        int totalduty = 0;
                        int totalovertime = 0;
                        htmlTable.Append("<tr>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Employee_id"] + "</td>");
                        //htmlTable.Append("<td>-</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Department_name"] + "</td>");
                        htmlTable.Append("<td>" + attdt.Rows[i]["Designation"] + "</td>");
                        //htmlTable.Append("<td>" + attdt.Rows[i]["Employee_Type"] + "</td>");
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            string pival = "", poval = "";
                            if (attdt.Rows[i]["PI" + date.Day + ""] + "" != "" && attdt.Rows[i]["PI" + date.Day + ""] + "" != null)
                            {
                                DateTime pidate = DateTime.ParseExact(attdt.Rows[i]["PI" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                pival = pidate.ToString("HH:mm");
                            }
                            else
                            {
                                pival = "-"
        ;
                            }
                            if (attdt.Rows[i]["PO" + date.Day + ""] + "" != "" && attdt.Rows[i]["PO" + date.Day + ""] + "" != null)
                            {
                                DateTime podate = DateTime.ParseExact(attdt.Rows[i]["PO" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                poval = podate.ToString("HH:mm");
                                totalduty++;
                            }
                            else
                            {
                                poval = "-";
                            }

                            htmlTable.Append("<td>" + pival + " " + poval + "</td>");
                            if (attdt.Rows[i]["OT" + date.Day + ""] + "" != "" && attdt.Rows[i]["OT" + date.Day + ""] + "" != null)
                            {
                                DateTime otdate = DateTime.ParseExact(attdt.Rows[i]["OT" + date.Day + ""] + "", "HH:mm:ss", CultureInfo.InvariantCulture);
                                totalovertime += Convert.ToInt32(otdate.ToString("HH"));
                            }
                            else
                            {
                                totalovertime += 0;
                                ;
                            }

                        }


                        htmlTable.Append("<td>" + totalduty + "</td>");
                        htmlTable.Append("<td>" + totalovertime + "</td>");
                        htmlTable.Append("</tr>");
                    }

                    htmlTable.Append("</tbody>");
                }
                else
                {
                    htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
                }

                //DataTable dt = db.GetAllRecord(squery);
                string json = JsonConvert.SerializeObject(attdt, Formatting.None);
                //return View();
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

                }
                htmlTable.Append("<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>");
            }
            finally
            {
                db.connectionstate();
            }

            return Json(htmlTable.ToString(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDailyperfor(string Date, string Managernm, string Depart, string Desig, string Empid)
        {
            string res = "", tbldata = "", tbl = "", subquery = "", regsubquery = "";

            string empquery = "", desigquery = "", departquery = "", managerquery = "";
            string json = "";
            try
            {

                if (Managernm != null && Managernm != "")
                {
                    string[] managerArray = Managernm.Split(',');
                    foreach (string id in managerArray)
                    {
                        managerquery += " Managercode='" + id + "' or";
                    }
                    managerquery = managerquery.Substring(0, managerquery.Length - 3);
                    subquery += "(" + managerquery + ") and";
                }
                if (Empid != null && Empid != "")
                {
                    string[] empArray = Empid.Split(',');
                    foreach (string id in empArray)
                    {
                        empquery += " Employeeid='" + id + "' or";
                    }
                    empquery = empquery.Substring(0, empquery.Length - 3);
                    subquery += "(" + empquery + ") and";
                }
                if (Desig != null && Desig != "")
                {
                    string[] desigArray = Desig.Split(',');
                    foreach (string designame in desigArray)
                    {
                        desigquery += " Designation='" + designame + "' or";
                    }
                    desigquery = desigquery.Substring(0, desigquery.Length - 3);
                    subquery += " (" + desigquery + ") and";
                }
                if (Depart != null && Depart != "")
                {
                    string[] departArray = Depart.Split(',');
                    foreach (string departnm in departArray)
                    {
                        departquery += " Department='" + departnm + "' or";
                    }
                    departquery = departquery.Substring(0, departquery.Length - 3);
                    subquery += " (" + departquery + ") and";
                }
                if (subquery.EndsWith(" and"))
                {
                    subquery = subquery.Substring(0, subquery.Length - 4);
                }
                if (subquery != null & subquery != "")
                {
                    subquery = subquery + " and ";
                }
                DateTime date = DateTime.ParseExact(Date, "d/MM/yyyy", CultureInfo.InvariantCulture);
                string outputDate = date.ToString("yyyy-MM-dd");
                string attquery = "SELECT * from tbl_attendance where Date='" + outputDate + "' and " + subquery + "";
                activitylog.Activitylogins("tbl_attendance", "", attquery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable attdt = db.GetAllRecord(attquery);
                activitylog.Activitylogupd("Success", "");
                if (attdt.Rows.Count > 0)
                {
                    //DataTable dt = db.GetAllRecord(squery);
                    json = JsonConvert.SerializeObject(attdt, Formatting.None);
                    //return View();
                }
                else
                {
                    json = "<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>";
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

                }
                json = "<div class='row justify-content-center d-flex m-5' id='datanotdound'><img src='/Content/Img/nodata1.png' class='w-50 h-50 align-items-center'/></div>";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEmpdata(string Empid)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_registration where Id='" + Empid + "'";
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    //string id = dt.Rows[0]["Id"]+"";
                    string empid = dt.Rows[0]["Employee_id"] + "";
                    string emptype = dt.Rows[0]["Employee_Type"] + "";
                    string empname = dt.Rows[0]["Name"] + "";
                    string empmobile = dt.Rows[0]["Mobile_no"] + "";
                    string empmail = dt.Rows[0]["Email"] + "";
                    string empdepart = dt.Rows[0]["Department_name"] + "";
                    string empdesig = dt.Rows[0]["Designation"] + "";
                    string empgardian = dt.Rows[0]["Gardianname"] + "";
                    string empadd = dt.Rows[0]["Address"] + "";
                    string emptempadd = dt.Rows[0]["Temporaryaddress"] + "";
                    string empaltmobile = dt.Rows[0]["Alternatemobile"] + "";
                    string empdob = dt.Rows[0]["Dateofbirth"] + "";
                    string empgender = dt.Rows[0]["Gender"] + "";
                    string empmarital = dt.Rows[0]["Maritalstatus"] + "";
                    string workpremis = dt.Rows[0]["Premises"] + "";
                    string empblood = dt.Rows[0]["Bloodgroup"] + "";
                    string empsubdepart = dt.Rows[0]["Bloodgroup"] + "";
                    string empmanagername = dt.Rows[0]["Managercode"] + "";
                    string Managername = dt.Rows[0]["Managername"] + "";
                    string empemptype = dt.Rows[0]["Employeementtype"] + "";
                    string banknm = dt.Rows[0]["Bankname"] + "";
                    string empaccnumber = dt.Rows[0]["Accountnumber"] + "";
                    string empaccholdnm = dt.Rows[0]["Accountholdername"] + "";
                    string checkin = dt.Rows[0]["Activatecheckin"] + "";
                    string ifsc = dt.Rows[0]["IFSC"] + "";
                    string pfno = dt.Rows[0]["PFNo"] + "";
                    string esic = dt.Rows[0]["ESIC"] + "";
                    string paymentmode = dt.Rows[0]["Paymentmode"] + "";
                    string dol = dt.Rows[0]["DOL"] + "";
                    string pan = dt.Rows[0]["PAN"] + "";
                    string emerconname = dt.Rows[0]["Emerconname"] + "";
                    string emerconmob = dt.Rows[0]["Emerconmobile"] + "";
                    string emerconrel = dt.Rows[0]["Emerconrelation"] + "";
                    string shifttiming = dt.Rows[0]["Shifttiming"] + "";
                    string shiftname = dt.Rows[0]["Shiftname"] + "";
                    string shiftstart = dt.Rows[0]["Shiftstarttime"] + "";
                    string shiftend = dt.Rows[0]["Shiftendtime"] + "";
                    string doj = dt.Rows[0]["Dateofjoining"] + "";
                    string nofchild = dt.Rows[0]["Nofchild"] + "";
                    string citizen = dt.Rows[0]["Citizenship"] + "";
                    string flexhr = dt.Rows[0]["Flexigours"] + "";
                    string empimg = dt.Rows[0]["Employeeimage"] + "";
                    string qrimg = dt.Rows[0]["QRpath"] + "";



                    string basicsal = dt.Rows[0]["Basicsalary"] + "";
                    string horentall = dt.Rows[0]["Houserentallowance"] + "";
                    string convetall = dt.Rows[0]["Conveyanceallowance"] + "";
                    string mediall = dt.Rows[0]["Medicalallowance"] + "";
                    string spacall = dt.Rows[0]["Specialallowance"] + "";
                    string grosalary = dt.Rows[0]["Grosssalary"] + "";
                    string epf = dt.Rows[0]["EPF"] + "";
                    string healthens = dt.Rows[0]["HealthInsurance"] + "";
                    string profetax = dt.Rows[0]["Professionaltax"] + "";
                    string tds = dt.Rows[0]["TDS"] + "";
                    string totaldeduc = dt.Rows[0]["Totaldeduction"] + "";
                    string netpay = dt.Rows[0]["Netpay"] + "";

                    string Totalleave = dt.Rows[0]["Totalleave"] + "";
                    string Pendingleave = dt.Rows[0]["Pendingleave"] + "";
                    string Approveleave = dt.Rows[0]["Approveleave"] + "";
                    string Balanceleave = dt.Rows[0]["Balanceleave"] + "";
                   
                    res = new string[59] { empid, emptype, empname, empmobile, empmail, empdepart, empdesig, empgardian, empadd, emptempadd, empaltmobile, empdob, empgender, empmarital, workpremis, empblood, empsubdepart, empmanagername, Managername, empemptype, banknm, empaccnumber, empaccholdnm, checkin, ifsc, pfno, esic, paymentmode, dol, pan, emerconname, emerconmob, emerconrel, shifttiming, shiftname, shiftstart, shiftend, doj, nofchild, citizen, flexhr, empimg, qrimg, basicsal, horentall, convetall, mediall, spacall, grosalary, epf, healthens, profetax, tds, totaldeduc, netpay, Totalleave, Pendingleave, Approveleave, Balanceleave };
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult active(string Ap, string tblnm)
        {
            string msg = "";
            try
            {
                string query = "Update " + tblnm + " set Status='Active' where id='" + Ap + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins(tblnm, Ap, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Active";
                }
                else
                {
                    activitylog.Activitylogins(tblnm, Ap, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Failed";
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

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
        public JsonResult inactive(string Bp, string tblnm)
        {
            string msg = "";
            try
            {
                string query = "Update " + tblnm + " set Status='Inactive' where id='" + Bp + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins(tblnm, Bp, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Blocked";
                }
                else
                {
                    activitylog.Activitylogins(tblnm, Bp, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    msg = "Failed";
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

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCheckin(string Start_date, string End_date, string Department, string Empnm, string Designation)
        {
            string res = "", tbldata = "", tbl = "", json = "";

            try
            {
                DateTime startdate = DateTime.ParseExact(Start_date, "d/M/yyyy", null);
                string formatteStart_date = startdate.ToString("yyyy-MM-dd");
                DateTime enddate = DateTime.ParseExact(End_date, "d/M/yyyy", null);
                string formattedEnd_date = enddate.ToString("yyyy-MM-dd");

                string squery = "select * from tbl_checkin where (Date>='" + formatteStart_date + "' and Date<='" + formattedEnd_date + "')";
                if (!string.IsNullOrEmpty(Department))
                {
                    squery += " AND Department = '" + Department + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND Employee_id = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Designation))
                {
                    squery += " AND Designation = '" + Designation + "'";
                }
                activitylog.Activitylogins("tbl_checkin", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetLocationTrack(string Start_date, string End_date, string Department, string Empnm, string Shift, string Designation)
        {
            string res = "", tbldata = "", tbl = "", json = "";
            try
            {
                DateTime startdate = DateTime.ParseExact(Start_date, "d/M/yyyy", null);
                string formatteStart_date = startdate.ToString("yyyy-MM-dd");
                DateTime enddate = DateTime.ParseExact(End_date, "d/M/yyyy", null);
                string formattedEnd_date = enddate.ToString("yyyy-MM-dd");

                string squery = "select * from tbl_location_track where (Date>='" + formatteStart_date + "' and Date<='" + formattedEnd_date + "') ";
                if (!string.IsNullOrEmpty(Department))
                {
                    squery += " AND Department = '" + Department + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND Employeeid = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Designation))
                {
                    squery += " AND Designation = '" + Designation + "'";
                }
                if (!string.IsNullOrEmpty(Shift))
                {
                    squery += " AND Shiftname = '" + Shift + "'";
                }
                activitylog.Activitylogins("tbl_location_track", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetShiftList(string Start_date, string End_date, string Empnm, string Shift)
        {
            string res = "", tbldata = "", tbl = "", json = "";
            try
            {
                //DateTime startdate = DateTime.ParseExact(Start_date, "dd/MM/yyyy", null);
                //string formatteStart_date = startdate.ToString("yyyy-MM-dd");
                //DateTime enddate = DateTime.ParseExact(End_date, "dd/MM/yyyy", null);
                //string formattedEnd_date = enddate.ToString("yyyy-MM-dd");

                string squery = "select * from tbl_assignshift";
                if (!string.IsNullOrEmpty(Start_date))
                {
                    squery += " AND Fromdate >= '" + Start_date + "'";
                }
                if (!string.IsNullOrEmpty(End_date))
                {
                    squery += " AND Todate<= '" + End_date + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND Employeeid = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Shift))
                {
                    squery += " AND Shiftname = '" + Shift + "'";
                }
                activitylog.Activitylogins("tbl_assignshift", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Getpunchincheckinimage(string id, string Empids, string Tblnm)
        {
            string res = "", tbldata = "", tbl = "", json = "";

            try                               
            {
                string squery = "select * from " + Tblnm + " where Employeeid='" + Empids + "'";

                activitylog.Activitylogins(Tblnm, "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            //return View();
            return Json(json, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEmpJoining(string Department, string Empnm, string Designation)
        {
            string res = "", tbldata = "", tbl = "", json = "";
            try
            {
                string squery = "select * from tbl_registration";
                if (!string.IsNullOrEmpty(Department))
                {
                    squery += " AND Department_name = '" + Department + "'";
                }
                if (!string.IsNullOrEmpty(Empnm))
                {
                    squery += " AND Employee_id = '" + Empnm + "'";
                }
                if (!string.IsNullOrEmpty(Designation))
                {
                    squery += " AND Designation = '" + Designation + "'";
                }
                activitylog.Activitylogins("tbl_registration", "", squery, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(squery);
                activitylog.Activitylogupd("Success", "");
                json = JsonConvert.SerializeObject(dt, Formatting.None);
                //return View();
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

                }
                json = "Error";
            }
            finally
            {
                db.connectionstate();
            }

            return Json(json, JsonRequestBehavior.AllowGet);

        }


        public JsonResult Gettype(string Tblname)
        {
            string res = "<option selected disabled value=''>Select one</option>"; string query = "";
            try
            {
                switch (Tblname)
                {
                    case "tbl_department":
                        query = "select * from " + Tblname + "";
                        activitylog.Activitylogins(Tblname, "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        DataTable dt1 = db.GetAllRecord(query);
                        activitylog.Activitylogupd("Success", "");
                        if (dt1.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                if (dt1.Rows[i]["Status"] + "" == "Active")
                                {
                                    res += "<option value='" + dt1.Rows[i]["Departmant"] + "'>" + dt1.Rows[i]["Departmant"] + "" + "</option>";
                                }
                            }
                        }
                        break;
                    case "tbl_designation":
                        query = "select * from " + Tblname + "";
                        activitylog.Activitylogins(Tblname, "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        DataTable dt2 = db.GetAllRecord(query);
                        activitylog.Activitylogupd("Success", "");
                        if (dt2.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt2.Rows.Count; i++)
                            {
                                if (dt2.Rows[i]["Status"] + "" == "Active")
                                {
                                    res += "<option value='" + dt2.Rows[i]["Designation"] + "'>" + dt2.Rows[i]["Designation"] + "" + "</option>";
                                }
                            }
                        }
                        break;
                    case "tbl_registration":
                        query = "select * from " + Tblname + "";
                        activitylog.Activitylogins(Tblname, "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        DataTable dt3 = db.GetAllRecord(query);
                        activitylog.Activitylogupd("Success", "");
                        if (dt3.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt3.Rows.Count; i++)
                            {
                                if (dt3.Rows[i]["Status"] + "" == "Approved")
                                {
                                    res += "<option value='" + dt3.Rows[i]["Employee_id"] + "'>" + dt3.Rows[i]["Name"] + "(" + dt3.Rows[i]["Employee_id"] + ")</option>";
                                }
                            }
                        }
                        break;
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

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJoiningDetails(string Empid)
        {
            string query = ""; string msg = "";
            try
            {
                if (Empid != null && Empid != "")
                {
                    query = "select * from tbl_registration where Employee_id='" + Empid + "'";
                }
                else
                {
                    query = "select * from tbl_registration";
                }
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                msg = dt.ToString();
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

                }
                msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetShiftname(string Depart)
        {
            string res = "";
            try
            {
                res = "<option selected disabled value=''>Select one</option>";
                string query = "select * from tbl_shift where Department='" + Depart + "' and Status='Active'";
                activitylog.Activitylogins("tbl_shift", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Status"] + "" == "Active")
                        {
                            res += "<option value='" + dt.Rows[i]["Id"] + "'>" + dt.Rows[i]["Shiftname"] + "(" + dt.Rows[i]["Starttime"] + "-" + dt.Rows[i]["Endtime"] + ")</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }

                catch
                {

                }
                res = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEmpLeaveHistory(string Empid)
        {
            string res = "";
            int ptypeleave = 0, ctypeleave = 0, stypeleave = 0, otypeleave = 0, ltypeleave = 0;
            int pcount = 0, ccount = 0, scount = 0, ocount = 0, lcount = 0;
            string query = "select * from tbl_leave where Employeeid='" + Empid + "'  and Status!='Inactive'";
            DataTable dt = db.GetAllRecord(query);
            string query1 = "select * from tbl_registration where Id='" + Empid + "'";
            DataTable dt1 = db.GetAllRecord(query1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Status"] + "" == "Approved")
                    {
                        if (dt.Rows[i]["Leave_type"] + "" == "Personal")
                        {
                            pcount = pcount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Casual")
                        {
                            ccount = ccount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Sick")
                        {
                            scount = scount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Other")
                        {
                            ocount = ocount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "LWP")
                        {
                            lcount = lcount + 1;
                        }
                    }
                }
            }
            if (dt1.Rows.Count > 0)
            {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    ptypeleave = Convert.ToInt32(dt1.Rows[i]["Personalleave"] + "");
                    ctypeleave = Convert.ToInt32(dt1.Rows[i]["Casualleave"] + "");
                    stypeleave = Convert.ToInt32(dt1.Rows[i]["Sickleave"] + "");
                    otypeleave = Convert.ToInt32(dt1.Rows[i]["OtherLeave"] + "");
                    ltypeleave = 0;

                }
            }

            int pbalanceleave = ptypeleave - pcount;
            int cbalanceleave = ctypeleave - ccount;
            int sbalanceleave = stypeleave - scount;
            int obalanceleave = otypeleave - ocount;
            int lbalanceleave = ltypeleave - lcount;

            if (pbalanceleave < 0)
            {
                pbalanceleave = 0;
            }
            if (cbalanceleave < 0)
            {
                cbalanceleave = 0;
            }
            if (sbalanceleave < 0)
            {
                sbalanceleave = 0;
            }
            if (obalanceleave < 0)
            {
                obalanceleave = 0;
            }
            if (lbalanceleave < 0)
            {
                lbalanceleave = 0;
            }
            res = "<table class='table table-responsive table-bordered'><thead class='bg-dark text-white text-center'><tr><th>Leave Type</th><th>Total Leave</th><th>Approved Leave</th><th>Remaining Leave</th></tr></thead><tbody class='text-center'><tr><td>Personal Leave</td><td> " + ptypeleave + " </td><td> " + pcount + " </td><td>" + pbalanceleave + "</td></tr><tr><td>Casual Leave</td><td> " + ctypeleave + " </td><td> " + ccount + " </td><td>" + cbalanceleave + "</td></tr><tr><td>Sick Leave</td><td> " + stypeleave + " </td><td> " + scount + " </td><td>" + sbalanceleave + "</td></tr><tr><td>Other Leave</td><td> " + otypeleave + " </td><td> " + ocount + " </td><td>" + obalanceleave + "</td></tr><tr><td>LWP Leave</td><td> " + ltypeleave + " </td><td> " + lcount + " </td><td> " + lbalanceleave + " </td></tr></tbody></table>";
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeaveCount(string Empid, string Leavetype)
        {
            int[] res = { };
            int typeleave = 0; int count = 0;
            string query = "select * from tbl_leave where Emprowid='" + Empid + "'  and Status!='Inactive'";
            DataTable dt = db.GetAllRecord(query);
            string query1 = "select * from tbl_registration where Id='" + Empid + "'";
            DataTable dt1 = db.GetAllRecord(query1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Leavetype == dt.Rows[i]["Leave_type"] + "")
                    {
                        if (dt.Rows[i]["Status"] + "" == "Approved")
                        {
                            count = count + 1;
                        }
                    }
                }
            }
            if (dt1.Rows.Count > 0)
            {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if (Leavetype == "Personal")
                    {
                        typeleave = Convert.ToInt32(dt1.Rows[i]["Personalleave"] + "");
                    }
                    else if (Leavetype == "Casual")
                    {
                        typeleave = Convert.ToInt32(dt1.Rows[i]["Casualleave"] + "");
                    }
                    else if (Leavetype == "Sick")
                    {
                        typeleave = Convert.ToInt32(dt1.Rows[i]["Sickleave"] + "");
                    }
                    else if (Leavetype == "Other")
                    {
                        typeleave = Convert.ToInt32(dt1.Rows[i]["OtherLeave"] + "");
                    }
                    else
                    {
                        typeleave = 0;
                    }
                }
            }

            int balanceleave = typeleave - count;
            if (balanceleave < 0)
            {
                balanceleave = 0;
            }
            res = new int[3] { typeleave, count, balanceleave };
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllLeaveCount(string Empid)
        {
            int[] res = { };
            int ptypeleave = 0, ctypeleave = 0, stypeleave = 0, otypeleave = 0, ltypeleave = 0;
            int pcount = 0, ccount = 0, scount = 0, ocount = 0, lcount = 0;
            string query = "select * from tbl_leave where Employeeid='" + Empid + "' and Status!='Inactive'";
            DataTable dt = db.GetAllRecord(query);
            string query1 = "select * from tbl_registration where Id='" + Empid + "'";
            DataTable dt1 = db.GetAllRecord(query1);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Status"] + "" == "Approved")
                    {
                        if (dt.Rows[i]["Leave_type"] + "" == "Personal")
                        {
                            pcount = pcount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Casual")
                        {
                            ccount = ccount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Sick")
                        {
                            scount = scount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "Other")
                        {
                            ocount = ocount + 1;
                        }
                        else if (dt.Rows[i]["Leave_type"] + "" == "LWP")
                        {
                            lcount = lcount + 1;
                        }
                    }
                }
            }
            if (dt1.Rows.Count > 0)
            {
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    ptypeleave = Convert.ToInt32(dt1.Rows[i]["Personalleave"] + "");
                    ctypeleave = Convert.ToInt32(dt1.Rows[i]["Casualleave"] + "");
                    stypeleave = Convert.ToInt32(dt1.Rows[i]["Sickleave"] + "");
                    otypeleave = Convert.ToInt32(dt1.Rows[i]["OtherLeave"] + "");
                    ltypeleave = 0;

                }
            }

            int pbalanceleave = ptypeleave - pcount;
            int cbalanceleave = ctypeleave - ccount;
            int sbalanceleave = stypeleave - scount;
            int obalanceleave = otypeleave - ocount;
            int lbalanceleave = ltypeleave - lcount;

            if (pbalanceleave < 0)
            {
                pbalanceleave = 0;
            }
            if (cbalanceleave < 0)
            {
                cbalanceleave = 0;
            }
            if (sbalanceleave < 0)
            {
                sbalanceleave = 0;
            }
            if (obalanceleave < 0)
            {
                obalanceleave = 0;
            }
            if (lbalanceleave < 0)
            {
                lbalanceleave = 0;
            }
            res = new int[5] { pbalanceleave, cbalanceleave, sbalanceleave, obalanceleave, lbalanceleave };
            return Json(res, JsonRequestBehavior.AllowGet);
        }







        //Open Transport Module
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult VehicleType()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult VehicleType(string hid, string vehicletype, string status)
        {
            string userid = Session["auid"] + "";
            string username = Session["auname"] + "";
            try
            {
                if (hid != "" && hid != null)
                {
                    string query = "update tbl_vehiclettype set type='" + vehicletype + "',Status='" + status + "',Datetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Id='" + hid + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_vehiclettype", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_vehiclettype", hid, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Update Failed";
                    }
                }
                else
                {
                    string selquery = "select * from tbl_vehiclettype where type='" + vehicletype + "'";
                    DataTable dt = db.GetAllRecord(selquery);
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.msg = "This VehicleType Already Exist";
                    }
                    else
                    {
                        string query = "insert into tbl_vehiclettype(type,Branchname,Branchid,Status,Logname,Logid,Datetime) values('" + vehicletype + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + status + "','" + username + "','" + userid + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins("tbl_vehiclettype", db.getmaxid("tbl_vehiclettype").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Saved";
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_vehiclettype", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult updvehicletype(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_vehiclettype where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_vehiclettype", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string name = dt.Rows[0]["Type"] + "";
                    string status = dt.Rows[0]["Status"] + "";
                    string companyname = dt.Rows[0]["Logname"] + "";
                    string companyid = dt.Rows[0]["Logid"] + "";

                    res = new string[5] { id, name, status, companyname, companyid };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Vehicle()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Vehicle(FormCollection form, string hid)
        {
            try
            {
                if (hid != "" && hid != null)
                {
                    string query = "update [tbl_vehicle] set [Vehicletype]='" + form["vehicletype"] + "',[Registration_no]='" + form["registrationno"] + "',[Chassis_no]='" + form["chassisno"] + "',[Engine_no]='" + form["engineno"] + "',[Vehicle_name]='" + form["vehiclename"] + "',[Model_no]='" + form["modelno"] + "',[Loading_capicity]='" + form["loadingcapacity"] + "',[Insurance_policy_no]='" + form["insuranceno"] + "',[Insurance_valid_upto]='" + form["insurancevalid"] + "',[Fitness_valid_upto]='" + form["fitnessvalid"] + "',[Tax_upto]='" + form["taxupto"] + "',[Pollution_certificate_no]='" + form["pollutioncerno"] + "',[Pollutioncer_valid_upto]='" + form["pollutioncervalid"] + "',[Driver_name]='" + form["drivername"] + "',[Driver_contact]='" + form["drivercontact"] + "',[Driver_DL]='" + form["driverdlno"] + "',[Driver_DL_valid]='" + form["dlvalid"] + "',[Status]='" + form["status"] + "',[Description]='" + form["description"] + "' where Id='" + hid + "'";


                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_vehicle", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_vehicle", hid, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Update Failed";
                    }
                }
                else
                {
                    string selquery = "select * from tbl_vehicle where Registration_no='" + form["registrationno"] + "'";
                    DataTable dt = db.GetAllRecord(selquery);
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.msg = "This Vehicle Already Exist";
                    }
                    else
                    {
                        DateTime currentDate = DateTime.Now;
                        // Extract the year and month from the current date
                        string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                        string Vehicleid = "V" + currentYearmonth + "" + arandom(5);

                        string query = "INSERT INTO [dbo].[tbl_vehicle]([Vehicletype],[Vehicleid],[Registration_no],[Chassis_no],[Engine_no],[Vehicle_name],[Model_no],[Loading_capicity],[Insurance_policy_no],[Insurance_valid_upto],[Fitness_valid_upto],[Tax_upto],[Pollution_certificate_no],[Pollutioncer_valid_upto],[Driver_name],[Driver_contact],[Driver_DL],[Driver_DL_valid],[Status],[Description],[Logid],[Logname],[Datetime])VALUES ('" + form["vehicletype"] + "','" + Vehicleid + "','" + form["registrationno"] + "','" + form["chassisno"] + "','" + form["engineno"] + "','" + form["vehiclename"] + "','" + form["modelno"] + "','" + form["loadingcapacity"] + "','" + form["insuranceno"] + "','" + form["insurancevalid"] + "','" + form["fitnessvalid"] + "','" + form["taxupto"] + "','" + form["pollutioncerno"] + "','" + form["pollutioncervalid"] + "','" + form["drivername"] + "','" + form["drivercontact"] + "','" + form["driverdlno"] + "','" + form["dlvalid"] + "','Active','" + form["description"] + "','" + Session["auid"] + "','" + Session["auname"] + "','" + DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss") + "')";

                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins("tbl_vehicle", db.getmaxid("tbl_vehicle").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Saved";
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_vehicle", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult Updatevehicle(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_vehicle where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_vehicle", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Vehicleid = dt.Rows[0]["Vehicleid"] + "";
                    string Vehicletype = dt.Rows[0]["Vehicletype"] + "";
                    string Registration_no = dt.Rows[0]["Registration_no"] + "";
                    string Chassis_no = dt.Rows[0]["Chassis_no"] + "";
                    string Engine_no = dt.Rows[0]["Engine_no"] + "";
                    string Vehicle_name = dt.Rows[0]["Vehicle_name"] + "";
                    string Model_no = dt.Rows[0]["Model_no"] + "";
                    string Loading_capicity = dt.Rows[0]["Loading_capicity"] + "";
                    string Insurance_policy_no = dt.Rows[0]["Insurance_policy_no"] + "";
                    string Insurance_valid_upto = dt.Rows[0]["Insurance_valid_upto"] + "";
                    string Fitness_valid_upto = dt.Rows[0]["Fitness_valid_upto"] + "";
                    string Tax_upto = dt.Rows[0]["Tax_upto"] + "";
                    string Pollution_certificate_no = dt.Rows[0]["Pollution_certificate_no"] + "";
                    string Pollutioncer_valid_upto = dt.Rows[0]["Pollutioncer_valid_upto"] + "";
                    string Driver_name = dt.Rows[0]["Driver_name"] + "";
                    string Driver_contact = dt.Rows[0]["Driver_contact"] + "";
                    string Driver_DL = dt.Rows[0]["Driver_DL"] + "";
                    string Driver_DL_valid = dt.Rows[0]["Driver_DL_valid"] + "";
                    string Status = dt.Rows[0]["Status"] + "";
                    string Description = dt.Rows[0]["Description"] + "";
                    res = new string[21] { id, Vehicleid, Vehicletype, Registration_no, Chassis_no, Engine_no, Vehicle_name, Model_no, Loading_capicity, Insurance_policy_no, Insurance_valid_upto, Fitness_valid_upto, Tax_upto, Pollution_certificate_no, Pollutioncer_valid_upto, Driver_name, Driver_contact, Driver_DL, Driver_DL_valid, Status, Description };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }





        //HRM MODULE Methods and Pages

        public JsonResult GetJobTitles(string department)
        {
            try
            {
                string query = "SELECT Designation FROM tbl_designation WHERE Department ='" + department + "' AND Status = 'Active'";
                DataTable dt = db.GetAllRecord(query);
                List<string> jobTitles = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    jobTitles.Add(row["Designation"].ToString());
                }

                return Json(jobTitles, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult onStock(string Department)
        {
            string json = "";
            try
            {
                string query = "select Designation from tbl_designation where Department='" + Department + "'";
                DataTable dt = db.GetAllRecord(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(dt, Formatting.None);
                    activitylog.Activitylogins("tbl_designation", Department, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                }
                else
                {
                    activitylog.Activitylogins("tbl_designation", Department, query, "Failed", "Get Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Get Failed";
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
                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Resignedemp()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult EmpRecritment()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult EmpRecritment(string hid, FormCollection form)
        {
            try
            {
                if (hid != "" && hid != null)
                {
                    string query = "update tbl_emprecruitment set Department='" + form["Department"] + "', Jobtittle='" + form["Jobtittle"] + "',Jobtype='" + form["Jobtype"] + "',Worklocation='" + form["Worklocation"] + "',Qualification='" + form["Qualification"] + "',MandatorySkill='" + form["MandatorySkill"] + "',Description='" + form["Description"] + "',Experience='" + form["Experience"] + "',Noposition='" + form["Noposition"] + "',Datetime='" + DateTime.Now.ToString("yyyy-MM-dd") + "',Salary='" + form["Salary"] + "',Branchname='" + Session["abrname"] + "',Branchcode='" + Session["abrcode"] + "' where Id='" + hid + "'";

                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_emprecruitment", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_emprecruitment", hid, query, "Failed", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Update Error";
                    }
                }
                else
                {
                    string selquery = "select * from tbl_emprecruitment where Jobtittle='" + form["Jobtittle"] + "' AND Jobtype='" + form["Jobtype"] + "' And Status='Active'";
                    DataTable dt = db.GetAllRecord(selquery);
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.msg = "This Post is Already Exist";
                    }
                    else
                    {
                        string query = "insert into tbl_emprecruitment(Department,Jobtittle,Jobtype,Worklocation,Qualification,MandatorySkill,Description,Status,Experience,Noposition,Datetime,Salary,Branchname,Branchcode) values('" + form["Department"] + "','" + form["Jobtittle"] + "','" + form["Jobtype"] + "','" + form["Worklocation"] + "','" + form["Qualification"] + "','" + form["MandatorySkill"] + "','" + form["Description"] + "','Active','" + form["Experience"] + "','" + form["Noposition"] + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + form["Salary"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "')";
                        if (db.InsertUpdateDelete(query))
                        {


                            //string[] replacementValues = { "GROWFAST ORGANIC DIMOND" };

                            activitylog.Activitylogins("tbl_emprecruitment", db.getmaxid("tbl_emprecruitment").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Saved";
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_emprecruitment", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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


        public ActionResult SendentryForm()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult SendentryForm(string hid, FormCollection form, string canmobile, string linkurl)
        {
            try
            {
                string Send = "";
                string selquery = "select * from tbl_HRM where canname='" + form["canname"] + "'And canmobile='" + form["canmobile"] + "' And workStatus='Send'";
                DataTable dt = db.GetAllRecord(selquery);
                if (dt.Rows.Count > 0)
                {
                    ViewBag.msg = "This data is Already Exist";
                }
                else
                {
                    string query = "insert into tbl_HRM(canname,canmobile,status,Datetime,workStatus,Companyname,Companyid,Branchname,Branchcode,Appliedfor,Jobtittlefor,Jobrowid) values('" + form["canname"] + "','" + form["canmobile"] + "','Active','" + DateTime.Now.ToString("yyyy-MM-dd") + "','Send','" + Session["Companynamem"] + "','" + Session["Companyidm"] + "','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["Appliedfor"] + "','" + form["Jobtittlefor"] + "','" + form["Jobrowid"] + "')";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_HRM", db.getmaxid("tbl_HRM").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        string ssd = "";

                        //string Message = "Dear "+form["canname"]+", your resume has been shortlisted. Please fill the form <a href='" +URL.MainUrl()+ "/Home/NewCanAppli?id="+ db.getmaxid("tbl_HRM").ToString() + "'> Click here </a>.";

                        string Message = "Hi " + form["canname"] + " ,Thank  You for showing interest in Growfast here is the link for submitting your Job Application Form. <a href='" + URL.MainUrl1() + "/Home/NewCanAppli?id=" + db.getmaxid("tbl_HRM").ToString() + "'> Click here </a> Click on the link to fill application form.";
                        string[] replacementValues = { form["canname"], URL.MainUrl1() + "/Home/NewCanAppli?id=" + db.getmaxid("tbl_HRM").ToString() };

                        //  Messaging.SendSMS(canmobile, replacementValues,Message, form["canname"]);
                        Messaging.SendWhatsappSMSNew1(canmobile, "Application_form_link", form["canname"], Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", "", "", replacementValues, true);
                        ViewBag.msg = "Data Saved";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_HRM", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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

        public ActionResult ScheduleInterview()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ScheduleInterview(string hid, FormCollection form, string canmobile)
        {
            try
            {
                string query = "Update tbl_HRM set status ='Active',workStatus ='Scheduled',InterviewPlace ='" + form["InterviewPlace"] + "',DateTime='" + DateTime.Now.ToString("yyyy-MM-dd") + "',Interviewdate='" + form["Interviewdate"] + "',Interviewstarttime='" + form["Interviewstarttime"] + "',Interviewendtime='" + form["Interviewendtime"] + "' where Id='" + hid + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_HRM", db.getmaxid("tbl_HRM").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                    string query2 = "select * from tbl_HRM where  Id='" + hid + "'";
                    activitylog.Activitylogins("tbl_HRM", hid.ToString(), query2, "success", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    DataTable dt2 = db.GetAllRecord(query2);
                    string Jobtittle = dt2.Rows[0]["Jobtittlefor"].ToString();
                    string companyname = "Growfast Groups of Companies";
                    string contactinfo = "9415344987";
                    string timing = form["Interviewstarttime"].ToString() + " To " + form["Interviewendtime"].ToString();


                    string ssd = "";
                    string Mobile = form["canmobile"];

                    string Message = "Hi " + form["canname"] + " , Thank  you for applying to the " + Jobtittle + "  position at " + companyname + ". After reviewing your application, We’re excited to invite you to interview for role " + Jobtittle + ". Your interview  will be conducted on " + form["Interviewdate"] + "  at " + timing + " .Contact Info:- " + contactinfo + " \n Growfast";

                    string[] replacementValues = { form["canname"], Jobtittle, companyname, Jobtittle, form["Interviewdate"], timing, contactinfo };
                    //Messaging.SendSMS(Mobile, replacementValues,Message);

                    Messaging.SendWhatsappSMSNew1(canmobile, "Interview_Schedule", form["canname"], Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", "", "", replacementValues, true);

                    ViewBag.msg = "Data Saved";
                }
                else
                {
                    activitylog.Activitylogins("tbl_Interview", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Data Save Failed";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return View();
        }
        public JsonResult Statuschangetivehr(string id, string status, string tblnm)
        {
            string msg = "";
            try
            {
                string squery = "select Status from " + tblnm + " where Id='" + id + "'";
                DataTable dt = db.GetAllRecord(squery);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Status"] + "" == "Active")
                    {
                        string query = "Update " + tblnm + " set Status='Inactive' where Id='" + id + "'";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Inactive";
                        }
                        else
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Failed";
                        }
                    }
                    else if (dt.Rows[0]["Status"] + "" == "Inactive")
                    {
                        string query = "Update " + tblnm + " set Status='Active' where Id='" + id + "'";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Active";
                        }
                        else
                        {
                            activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            msg = "Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult Rejectcandidate(string id, string workStatus, string tblnm)
        {
            string msg = "";
            try
            {
                string squery = "select workStatus from " + tblnm + " where Id='" + id + "'";
                DataTable dt = db.GetAllRecord(squery);
                if (dt.Rows.Count > 0)
                {

                    string query = "Update " + tblnm + " set workStatus='Rejected' where Id='" + id + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        msg = "Inactive";
                    }
                    else
                    {
                        activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        msg = "Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult Selectcandidate(string id, string workStatus, string tblnm)
        {
            string msg = "";
            try
            {
                string squery = "select * from " + tblnm + " where Id='" + id + "'";
                DataTable dt = db.GetAllRecord(squery);
                if (dt.Rows.Count > 0)
                {
                    string name = dt.Rows[0]["canname"].ToString();
                    string canmobile = dt.Rows[0]["canmobile"].ToString();
                    string Jobtittle = dt.Rows[0]["Jobtittlefor"].ToString();
                    string query = "Update " + tblnm + " set workStatus='Selected' where Id='" + id + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins(tblnm, id, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        msg = "Selected";
                        string ssd = "";

                        string Message = " Hi, " + name + "  Congratulations!! Its our Good pleasure to inform you that you are selected in our company for the position of " + Jobtittle + ". We are ready to onboard you and you will receive your offer letter shortly. \n Growfast";

                        string[] replacementValues = { name, Jobtittle };
                        //Messaging.SendSMS(Mobile, replacementValues,Message);

                        Messaging.SendWhatsappSMSNew1(canmobile, "candidate_selected_Message", name, Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", "", "", replacementValues, true);
                    }
                    else
                    {
                        activitylog.Activitylogins(tblnm, id, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        msg = "Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult UpdateHRemp(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_emprecruitment where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_emprecruitment", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Department = dt.Rows[0]["Department"] + "";
                    string Jobtittle = dt.Rows[0]["Jobtittle"] + "";
                    string Jobtype = dt.Rows[0]["Jobtype"] + "";
                    string Worklocation = dt.Rows[0]["Worklocation"] + "";
                    string Status = dt.Rows[0]["Status"] + "";
                    string Qualification = dt.Rows[0]["Qualification"] + "";
                    string MandatorySkill = dt.Rows[0]["MandatorySkill"] + "";
                    string Experience = dt.Rows[0]["Experience"] + "";
                    string Noposition = dt.Rows[0]["Noposition"] + "";
                    string Description = dt.Rows[0]["Description"] + "";
                    string Salary = dt.Rows[0]["Salary"] + "";

                    res = new string[12] { id, Department, Jobtittle, Jobtype, Worklocation, Status, Qualification, MandatorySkill, Experience, Noposition, Description, Salary };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecruitmentDash()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult CanProfile()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public JsonResult CanLoadMore(string from, string tablename, string Searchby, string Buttontype, string Searchid)
        {
            string query2 = "";
            string res = "", data1 = "", subquery = "";
            try
            {
                //query2 = "SELECT * FROM " + tablename + " where (Mobile_no='" + Searchby + "' or Name LIKE '%" + Searchby + "%' or Designation= LIKE '%" + Searchby + "%' or Department_name= LIKE '%" + Searchby + "%') and Status ='Approved' ORDER BY Id DESC";


                query2 = "SELECT * FROM " + tablename + " WHERE Id<'" + from + "' and ( Name LIKE '%" + Searchby + "%' OR Designation LIKE '%" + Searchby + "%'OR Department_name LIKE '%" + Searchby + "%') AND Status = 'Approved' and Branchcode = '" + Session["abrcode"] + "' ORDER BY Id DESC ";

                DataTable dt = db.GetAllRecord(query2);

                res = JsonConvert.SerializeObject(dt, formatting: Formatting.None);

            }
            catch (Exception ex)
            { }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewAllDetails()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        public ActionResult Onboarding()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }

        public ActionResult Updateofferletter(FormCollection form, string hids, HttpPostedFileBase offerletter, string prevAttachement, string hfile, string photoview, string linkurl)
        {
            string msg = "";
            try
            {

                string fdpath = ""; string fileName = "";
                if (offerletter != null && offerletter.ContentLength > 0)
                {
                    fdpath = APIs.Candidatedocs(offerletter, "ofl");
                    fileName = offerletter.FileName;
                }
                else
                {
                    fileName = "ofl";
                    fdpath = photoview;
                }
                string selquery = "select * from tbl_HRM where Id= '" + hids + "'";
                DataTable dt = db.GetAllRecord(selquery);
                string name = dt.Rows[0]["canname"].ToString();
                string canmobile = dt.Rows[0]["canmobile"].ToString();
                string Jobtittle = dt.Rows[0]["Jobtittlefor"].ToString();
                string query = "Update tbl_HRM set offerletter='" + fdpath + "',joiningdate='" + form["joiningdate"] + "',designation='" + form["designation"] + "' where Id='" + hids + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_HRM", hids, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                    string ssd = "";

                    // please check url(link) and pass correct.

                    string Message = "Hi, " + name + " , We are very excited to release offer letter for the position of " + Jobtittle + " at Growfast Group of Companies. Click on the link below to download your offer letter. Link - <a href='" + URL.ApiURL() + "" + fdpath + "'> Click here </a>. Submit the form and upload relevant documents to complete onboarding by clicking on the below link. <a href='" + URL.MainUrl1() + "/Home/NewCanAppli?id=" + db.getmaxid("tbl_HRM").ToString() + "'> Click here </a>. We are looking forward to welcoming you to our team and the exciting journey ahead. \n Growfast";

                    string[] replacementValues = { name, Jobtittle, URL.ApiURL() + "" + fdpath, URL.MainUrl1() + "/Home/Uploaddocs?id=" + db.getmaxid("tbl_HRM").ToString() };

                    //Messaging.SendSMS(canmobile, replacementValues, Message);

                    Messaging.SendWhatsappSMSNew1(canmobile, "Issue_Offer_Letter", name, Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", URL.ApiURL() + "" + fdpath, fileName, replacementValues, true);
                }
                else
                {
                    activitylog.Activitylogins("tbl_HRM", hids, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

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

                }
                //msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return RedirectToAction("Onboarding");

        }
        public ActionResult onlyUpdateofferletter(FormCollection form, string hidsf, HttpPostedFileBase offerletter, string prevAttachement, string hfile, string photoview)
        {
            string msg = "";
            try
            {
                string fdpath = "";
                if (offerletter != null && offerletter.ContentLength > 0)
                {
                    fdpath = APIs.Candidatedocs(offerletter, "ofl");
                }
                else
                {
                    fdpath = photoview;
                }

                string query = "Update tbl_HRM set offerletter='" + fdpath + "' where Id='" + hidsf + "'";
                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_HRM", hidsf, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                }
                else
                {
                    activitylog.Activitylogins("tbl_HRM", hidsf, query, "Failed", "Update Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

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

                }
                //msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return RedirectToAction("Onboarding");

        }


        public ActionResult Redrive()
        {
            string id = Session["auid"] + "";
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect("/Home/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Redrive(string hid, FormCollection form, string linkurl, string canmobile)
        {
            try
            {
                if (hid != "" && hid != null)
                {
                    string query = "update tbl_redrive set Recruitmentname='" + form["Recruitmentname"] + "', Manager='" + form["Manager"] + "',Location='" + form["Location"] + "',Openjob='" + form["Openjob"] + "',Vacancy='" + form["Vacancy"] + "',Totalhires='" + form["Totalhires"] + "',startdate='" + form["startdate"] + "',enddate='" + form["enddate"] + "',status='Active',Datetime='" + DateTime.Now.ToString("yyyy-MM-dd") + "',Branchname='" + Session["abrname"] + "',Branchcode='" + Session["abrcode"] + "',Managerid='" + form["Managerid"] + "' where Id='" + hid + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        activitylog.Activitylogins("tbl_redrive", hid, query, "Success", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_redrive", hid, query, "Failed", "Update Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                        ViewBag.msg = "Data Update Error";
                    }
                }
                else
                {
                    string selquery = "select * from tbl_redrive where Recruitmentname='" + form["Recruitmentname"] + "' And Manager='" + form["Manager"] + "' AND Managerid='" + form["Managerid"] + "' And status='Active'";
                    DataTable dt = db.GetAllRecord(selquery);
                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.msg = "This Post is Already Exist";
                    }
                    else
                    {
                        // Generate a random 5-digit number
                        Random rnd = new Random();
                        int randomNumber = rnd.Next(10000, 100000); // Generates a random number between 10000 and 99999

                        // Construct the Row_Id with prefix "RE" and random number
                        string driveid = "RE" + randomNumber.ToString();
                        string query = "insert into tbl_redrive(Recruitmentname,Manager,Location,Openjob,Vacancy,Totalhires,startdate,enddate,status,Datetime,Drive_id,logid,logname,Branchname,Branchcode,Managerid) values('" + form["Recruitmentname"] + "','" + form["Manager"] + "','" + form["Location"] + "','" + form["Openjob"] + "','"
                                       + form["Vacancy"] + "','" + form["Totalhires"] + "','" + form["startdate"] + "','" + form["enddate"]
                                       + "','Active','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + driveid + "','logid','logname','" + Session["abrname"] + "','" + Session["abrcode"] + "','" + form["Managerid"] + "')";
                        if (db.InsertUpdateDelete(query))
                        {
                            activitylog.Activitylogins("tbl_redrive", db.getmaxid("tbl_redrive").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());

                            string selqueryre = "SELECT Mobile_no FROM tbl_registration WHERE Name = '" + form["Manager"] + "'";
                            DataTable dts = db.GetAllRecord(selqueryre);
                            if (dts.Rows.Count > 0)
                            {

                                canmobile = dts.Rows[0]["Mobile_no"].ToString();
                            }
                            string ssd = "";
                            string Message = "Dear " + form["Manager"] + ", Please fill the form <a href='" + URL.MainUrl1() + "/User/RegistrationBydrive?id=" + db.getmaxid("tbl_redrive").ToString() + "'> Click here </a>.";
                            string[] replacementValues = { form["Manager"], URL.MainUrl1() + "/User/RegistrationBydrive?id=" + db.getmaxid("tbl_redrive").ToString() };
                            // Messaging.SendSMS(canmobile, replacementValues, Message, form["Manager"]);
                            //  Messaging.SendWhatsappSMSNew1(canmobile, "Application_form_link", form["canname"], Session["auid"] + "", Session["auname"] + "", Session["amail"] + "", "", "", replacementValues, true);
                            ViewBag.msg = "Data Saved";
                        }
                        else
                        {
                            activitylog.Activitylogins("tbl_redrive", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                            ViewBag.msg = "Data Save Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
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
        public JsonResult updateredrive(int Up)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_redrive where Id='" + Up + "'";
                activitylog.Activitylogins("tbl_redrive", Up.ToString(), query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string id = dt.Rows[0]["Id"] + "";
                    string Recruitmentname = dt.Rows[0]["Recruitmentname"] + "";
                    string Manager = dt.Rows[0]["Manager"] + "";
                    string Location = dt.Rows[0]["Location"] + "";
                    string Openjob = dt.Rows[0]["Openjob"] + "";
                    string Vacancy = dt.Rows[0]["Vacancy"] + "";
                    string Totalhires = dt.Rows[0]["Totalhires"] + "";
                    string startdatesp = dt.Rows[0]["startdate"] + "";
                    string enddatesp = dt.Rows[0]["enddate"] + "";
                    string managerid = dt.Rows[0]["Managerid"] + "";
                    res = new string[10] { id, Recruitmentname, Manager, Location, Openjob, Vacancy, Totalhires, startdatesp, enddatesp, managerid };

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

                }
            }
            finally
            {
                db.connectionstate();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManagerid(string manager)
        {
            try
            {
                string query = "SELECT Employee_id FROM tbl_registration WHERE Name ='" + manager + "' AND Status = 'Approved'";
                DataTable dt = db.GetAllRecord(query);
                List<string> jobTitles = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    jobTitles.Add(row["Employee_id"].ToString());
                }

                return Json(jobTitles, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult driveInactive(string id)
        {
            try
            {
                string query = "Update tbl_redrive set status='Inactive' where Id='" + id + "'";

                if (db.InsertUpdateDelete(query))
                {
                    activitylog.Activitylogins("tbl_redrive", db.getmaxid("tbl_redrive").ToString(), query, "Success", "Insert Success", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Data Saved";
                }
                else
                {
                    activitylog.Activitylogins("tbl_redrive", "", query, "Failed", "Insert Failed", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                    ViewBag.msg = "Data Save Failed";
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return RedirectToAction("Redrive");
        }
        public JsonResult Sendlinkdataonboarding(string Empid)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_HRM where Id='" + Empid + "'";
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    string Jobtittlefor = dt.Rows[0]["Jobtittlefor"] + "";
                    string id = dt.Rows[0]["Id"] + "";
                    res = new string[2] { Jobtittlefor, id };
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeenew(string Hid)
        {
            string json = null;
            try
            {
                string query = "select * from tbl_HRM where Id=" + Hid + "";
                activitylog.Activitylogins("tbl_registration", "", query, "Failed", "", Session["auid"].ToString(), Session["auname"].ToString(), Session["amail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    json = JsonConvert.SerializeObject(dt, formatting: Formatting.None);
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

                }
                ViewBag.msg = "Error";
            }
            finally
            {
                db.connectionstate();
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCandata(string Empid)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_registration where Id='" + Empid + "'";
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    //string id = dt.Rows[0]["Id"]+"";
                    string empid = dt.Rows[0]["Employee_id"] + "";
                    string emptype = dt.Rows[0]["Employee_Type"] + "";
                    string empname = dt.Rows[0]["Name"] + "";
                    string empmobile = dt.Rows[0]["Mobile_no"] + "";
                    string empmail = dt.Rows[0]["Email"] + "";
                    string empdepart = dt.Rows[0]["Department_name"] + "";
                    string empdesig = dt.Rows[0]["Designation"] + "";
                    string empgardian = dt.Rows[0]["Gardianname"] + "";
                    string empadd = dt.Rows[0]["Address"] + "";
                    string emptempadd = dt.Rows[0]["Temporaryaddress"] + "";
                    string empaltmobile = dt.Rows[0]["Alternatemobile"] + "";
                    string empdob = dt.Rows[0]["Dateofbirth"] + "";
                    string empgender = dt.Rows[0]["Gender"] + "";
                    string empmarital = dt.Rows[0]["Maritalstatus"] + "";
                    string workpremis = dt.Rows[0]["Premises"] + "";
                    string empblood = dt.Rows[0]["Bloodgroup"] + "";
                    string empsubdepart = dt.Rows[0]["Bloodgroup"] + "";
                    string empmanagerid = dt.Rows[0]["Managercode"] + "";
                    string empmanagername = dt.Rows[0]["Managername"] + "";
                    string empemptype = dt.Rows[0]["Employeementtype"] + "";
                    string banknm = dt.Rows[0]["Bankname"] + "";
                    string empaccnumber = dt.Rows[0]["Accountnumber"] + "";
                    string empaccholdnm = dt.Rows[0]["Accountholdername"] + "";
                    string checkin = dt.Rows[0]["Activatecheckin"] + "";
                    string ifsc = dt.Rows[0]["IFSC"] + "";
                    string pfno = dt.Rows[0]["PFNo"] + "";
                    string uanno = dt.Rows[0]["UANNo"] + "";
                    string esic = dt.Rows[0]["ESIC"] + "";
                    string paymentmode = dt.Rows[0]["Paymentmode"] + "";
                    string dol = dt.Rows[0]["DOL"] + "";
                    string pan = dt.Rows[0]["PAN"] + "";
                    string emerconname = dt.Rows[0]["Emerconname"] + "";
                    string emerconmob = dt.Rows[0]["Emerconmobile"] + "";
                    string emerconrel = dt.Rows[0]["Emerconrelation"] + "";
                    string shifttiming = dt.Rows[0]["Shifttiming"] + "";
                    string shiftname = dt.Rows[0]["Shiftname"] + "";
                    string shiftstart = dt.Rows[0]["Shiftstarttime"] + "";
                    string shiftend = dt.Rows[0]["Shiftendtime"] + "";
                    string doj = dt.Rows[0]["Dateofjoining"] + "";

                    string citizen = dt.Rows[0]["Citizenship"] + "";
                    string nofchild = dt.Rows[0]["Nofchild"] + "";
                    string flexhr = dt.Rows[0]["Flexigours"] + "";
                    string empimg = dt.Rows[0]["Employeeimage"] + "";
                    string qrimg = dt.Rows[0]["QRpath"] + "";


                    string basicsal = dt.Rows[0]["Basicsalary"] + "";
                    string horentall = dt.Rows[0]["Houserentallowance"] + "";
                    string convetall = dt.Rows[0]["Conveyanceallowance"] + "";
                    string mediall = dt.Rows[0]["Medicalallowance"] + "";
                    string spacall = dt.Rows[0]["Specialallowance"] + "";
                    string grosalary = dt.Rows[0]["Grosssalary"] + "";
                    string epf = dt.Rows[0]["EPF"] + "";
                    string healthens = dt.Rows[0]["HealthInsurance"] + "";
                    string profetax = dt.Rows[0]["Professionaltax"] + "";
                    string tds = dt.Rows[0]["TDS"] + "";
                    string totaldeduc = dt.Rows[0]["Totaldeduction"] + "";
                    string netpay = dt.Rows[0]["Netpay"] + "";
                    string adhar = dt.Rows[0]["Aadhar_no"] + "";
                    string Nation = dt.Rows[0]["Citizenship"] + "";
                    string emptempPincode = dt.Rows[0]["emptempPincode"] + "";
                    string emptempDistrict = dt.Rows[0]["emptempDistrict"] + "";
                    string emptempState = dt.Rows[0]["emptempState"] + "";
                    string Pincode = dt.Rows[0]["Pincode"] + "";
                    string State = dt.Rows[0]["State"] + "";
                    string District = dt.Rows[0]["District"] + "";
                    string pnb = dt.Rows[0]["PAN"] + "";

                    res = new string[65] { empid, emptype, empname, empmobile, empmail, empdepart, empdesig, empgardian, empadd, emptempadd, empaltmobile, empdob, empgender, empmarital, workpremis, empblood, empsubdepart, empmanagername, empmanagerid, empemptype, banknm, empaccnumber, empaccholdnm, checkin, ifsc, pfno, uanno, esic, paymentmode, dol, pan, emerconname, emerconmob, emerconrel, shifttiming, shiftname, shiftstart, shiftend, doj, citizen, nofchild, flexhr, empimg, qrimg, basicsal, horentall, convetall, mediall, spacall, grosalary, epf, healthens, profetax, tds, totaldeduc, netpay, adhar, Nation, emptempPincode, emptempDistrict, emptempState, Pincode, State, District, pnb };
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getpuloaddocs(string Empid, string Name, string Mobile)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_docsonboarding where canmobile = '" + Mobile + "' AND canname = '" + Name + "' "; /*canmobile = '" + Mobile + "' AND canname = '" + Name + "'*/
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    string canimage = dt.Rows[0]["canimage"] + "";
                    string highsch = dt.Rows[0]["highsch"] + "";
                    string Inter = dt.Rows[0]["Inter"] + "";
                    string Gradmrk = dt.Rows[0]["Gradmrk"] + "";
                    string aadhar = dt.Rows[0]["aadhar"] + "";
                    string pan = dt.Rows[0]["pan"] + "";
                    string releiving = dt.Rows[0]["releiving"] + "";
                    string letter = dt.Rows[0]["letter"] + "";
                    string signed = dt.Rows[0]["signed"] + "";

                    res = new string[9] { canimage, highsch, Inter, Gradmrk, aadhar, pan, releiving, letter, signed };
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Getdocsforprofile(string Empid, string Name, string Mobile)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_docsonboarding where Employee_id = '" + Empid + "'";
                DataTable dt = db.GetAllRecord(query);
                if (dt.Rows.Count > 0)
                {
                    string canimage = dt.Rows[0]["canimage"] + "";
                    string highsch = dt.Rows[0]["highsch"] + "";
                    string Inter = dt.Rows[0]["Inter"] + "";
                    string Gradmrk = dt.Rows[0]["Gradmrk"] + "";
                    string aadhar = dt.Rows[0]["aadhar"] + "";
                    string pan = dt.Rows[0]["pan"] + "";
                    string releiving = dt.Rows[0]["releiving"] + "";
                    string letter = dt.Rows[0]["letter"] + "";
                    string signed = dt.Rows[0]["signed"] + "";

                    res = new string[9] { canimage, highsch, Inter, Gradmrk, aadhar, pan, releiving, letter, signed };
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
}