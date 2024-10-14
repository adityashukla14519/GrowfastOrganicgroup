using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;

namespace GrowFast.Controllers
{
    public class CustomerController : Controller
    {
        SqlConnection con = new SqlConnection(@"Data Source=115.124.106.100;Initial Catalog=growfast;uid=growfast;pwd=P?B7ngp54w5Mqzzbx;");
        DbManager db = new DbManager();
        Activitylog activitylog = new Activitylog();

        // GET: Customer
        #region...Customer details

        public ActionResult Dashboard()
        {
            List<productCls> products = new List<productCls>();
            IPagedList<productCls> pagedProducts;
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    getCart();

                    //int pageSize = 10; // Number of items per page
                    //int pageNumber = (page ?? 1);

                    products = GetAll_Products();
                   // pagedProducts = products.ToPagedList(pageNumber, pageSize);

                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }
            return View(products);
        }

        public ActionResult Dashboard_with_login()
        {
            List<productCls> products = new List<productCls>();
            IPagedList<productCls> pagedProducts;
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    getCart();

                    //int pageSize = 10; // Number of items per page
                    //int pageNumber = (page ?? 1);

                    products = GetAll_Products();
                    // pagedProducts = products.ToPagedList(pageNumber, pageSize);

                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }
            return View(products);
        }
        public ActionResult Shop(int? page, int? pageSize,string category)
        {
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                url = "/Home/Login";
            }
            if (id != null && id != "")
            {
                ViewBag.customerid = Session["customerid"] + "";              
                ViewBag.category = category;
            }
                ViewBag.page = page;
            List<productCls> products = new List<productCls>();
            IPagedList<productCls> pagedProducts;
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    getCart();

                    int defaultPageSize = 10; // Default number of items per page
                    int pageNumber = (page ?? 1);
                    int selectedPageSize = pageSize ?? defaultPageSize;

                    products = GetAll_Products1(category);
                    pagedProducts = products.ToPagedList(pageNumber, selectedPageSize);

                    string Query = @"SELECT ProductCategory, COUNT(*) AS count FROM [tbl_pro_price_circlar] where Status='Active' GROUP BY ProductCategory";
                    DataTable dt = db.GetAllRecord(Query);
                    var data = dt;
                    ViewBag.category_group = dt;
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            {
                pagedProducts = new PagedList<productCls>(new List<productCls>(), 1, 10);
            }
            return View(pagedProducts);
        }

        public ActionResult Cart()
        {
            List<productCls> products = new List<productCls>();
            IPagedList<productCls> pagedProducts;
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    //getCart();

                    //int pageSize = 10; // Number of items per page
                    //int pageNumber = (page ?? 1);

                    //products = GetAll_Products();
                    //pagedProducts = products.ToPagedList(pageNumber, pageSize);

                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }
            return View();
        }

        public ActionResult product_detail()
        {
            List<productCls> products = new List<productCls>();
            IPagedList<productCls> pagedProducts;
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    //getCart();

                    //int pageSize = 10; // Number of items per page
                    //int pageNumber = (page ?? 1);

                    //products = GetAll_Products();
                    //pagedProducts = products.ToPagedList(pageNumber, pageSize);

                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }
            return View();
        }
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult ChangePassword()
        {
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                url = "/Home/Login";
            }
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect(url); ;
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string opass, string npass, string cnpass)
        {
            try
            {
                if (npass == opass)
                {
                    ViewBag.msg = "New Password and Old Password Can not be Same.";
                }
                else if (npass != cnpass)
                {
                    ViewBag.msg = "New Password and Confirm Password Not matched.";
                }
                else
                {
                    string query = "Update tbl_login set Password='" + npass + "' where Userid='" + Session["customerid"].ToString() + "'";
                    if (db.InsertUpdateDelete(query))
                    {
                        string query12 = "Update [tbl_customer] set Password='" + npass + "' where Customer_id='" + Session["customerid"].ToString() + "'";
                        if (db.InsertUpdateDelete(query12))
                        {
                            ////start send SMS and Whatsapp msg..

                            //string sql = "  SELECT * FROM [tbl_login] WHERE  Type = 'Customer' and Userid='" + Session["customerid"].ToString() + "'  AND  Token_id IS NOT NULL AND Token_id != ''";
                            //DataTable tbl = db.GetAllRecord(sql);
                            //if (tbl.Rows.Count > 0)
                            //{

                            //    string Mob = tbl.Rows[0]["Mobile"].ToString();
                            //    string Message = "Your password change Successfully!";
                            //    string name = tbl.Rows[0]["Username"].ToString();
                            //    string empID = tbl.Rows[0]["Userid"].ToString();

                            // //   WhatsappMessage.SendWhatsappSMS(Mob, Message);
                            //    Messaging.SendSMS(Mob, Message, "", name, "", "");

                            //    Messaging.SendPushNotification("Your password change Successfully!", Message, empID);
                            //}

                            ////end send SMS and Whatsapp msg..
                                
                                activitylog.Activitylogins("tbl_customer", db.getrowid("select Id from tbl_customer where Email='" + Session["usermail"].ToString() + "'").ToString(), query, "Success", "Password Update", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        }
                        else {
                            activitylog.Activitylogins("tbl_customer", db.getrowid("select Id from tbl_customer where Email='" + Session["usermail"].ToString() + "'").ToString(), query, "Success", "Password Update Failed", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        }

                        activitylog.Activitylogins("tbl_login", db.getrowid("select Id from tbl_login where Email='" + Session["usermail"].ToString() + "'").ToString(), query, "Success", "Password Update", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Password Updated";
                        //Logout();
                    }
                    else
                    {
                        activitylog.Activitylogins("tbl_login", db.getrowid("select Id from tbl_login where Email='" + Session["usermail"].ToString() + "'").ToString(), query, "Failed", "Password Update Failed", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Password Update Failed";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                url = "/Home/Login";
            }
            if (id != null && id != "")
            {
                if (token != null && token != "")
                {
                    //activitylog.Activitylogins("", "", "", "Success", "Logout Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());

                    db.InsertUpdateDelete("update tbl_login set Loginstatus='" + 0 + "',Logoutdatetime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where Userid='" + Session["customerid"] + "'");
                }
                else
                {
                    //activitylog.Activitylogins("", "", "", "Success", "Logout Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                }
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
                Response.Redirect(url);
            }
            else
            {
                Response.Redirect(url); ;
            }
            return View();
        }

        public ActionResult ViewProduct()
        {
            ViewBag.Message = TempData["Message"];

            List<productCls> products = GetAll_Products();
            return View(products);
        }

        private List<productCls> GetAll_Products()
        {
            List<productCls> productsList = new List<productCls>();

            try
            {
                string Query = "";
                Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' order by Id desc";

                DataTable dt = db.GetAllRecord(Query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        productCls products = new productCls
                        {

                            ID = Convert.ToString(row["Id"]),
                            Ptype = Convert.ToString(row["Ptype"]),
                            Name = Convert.ToString(row["Productname"]),
                            Description = Convert.ToString(row["Description"]),
                            Price = Convert.ToString(row["Sellprice"]),
                            Discount = Convert.ToString(row["Discountpercent"]),
                            //TaxPercent = Convert.ToString(row["TaxPercent"]),
                            Pimage = "/" + Convert.ToString(row["Pimage"]),
                            ProductCategory = Convert.ToString(row["ProductCategory"]),
                            Baserice = Convert.ToString(row["Offerprice"]),
                        };

                        productsList.Add(products);
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return productsList;
        }
        private List<productCls> GetAll_Products1(string Category)
        {
            List<productCls> productsList = new List<productCls>();

            try
            {
                string Query = "";
                if (Category!=null && Category != "")
                {
                    if(Category =="All")
                    {
                        Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' order by Id desc";
                    }
                    else
                    {
                        Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and ProductCategory='" + Category + "' order by Id desc";
                    }
                }
                else
                {
                    Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' order by Id desc";
                }
                

                DataTable dt = db.GetAllRecord(Query);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        productCls products = new productCls
                        {

                            ID = Convert.ToString(row["Id"]),
                            Ptype = Convert.ToString(row["Ptype"]),
                            Name = Convert.ToString(row["Productname"]),
                            Description = Convert.ToString(row["Description"]),
                            Price = Convert.ToString(row["Sellprice"]),
                            Discount = Convert.ToString(row["Discountpercent"]),
                            //TaxPercent = Convert.ToString(row["TaxPercent"]),
                            Pimage = "/" + Convert.ToString(row["Pimage"]),
                            ProductCategory = Convert.ToString(row["ProductCategory"]),
                            Baserice = Convert.ToString(row["Offerprice"]),
                        };

                        productsList.Add(products);
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return productsList;
        }
        public ActionResult Product_Details(string productID)
        {
            productCls products = new productCls();
            try
            {
                string pID = Encryption.Decrypt(productID);
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    string Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and Id='" + pID + "' order by Id desc";
                    DataTable dt = db.GetAllRecord(Query);


                    if (dt.Rows.Count > 0)
                    {

                        string Query1 = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and ProductCategory='" + dt.Rows[0]["ProductCategory"].ToString() + "' order by Id desc";
                        DataTable dttt = db.GetAllRecord(Query1);

                        ViewBag.MyData = dttt;

                        products.ID = dt.Rows[0]["Id"].ToString();
                        products.Name = dt.Rows[0]["Productname"].ToString();
                        products.Ptype = dt.Rows[0]["Ptype"].ToString();
                        products.Price = dt.Rows[0]["Sellprice"].ToString();
                        products.Discount = dt.Rows[0]["Discountpercent"].ToString();
                        //products.TaxPercent = dt.Rows[0]["TaxPercent"].ToString();
                        products.Description = dt.Rows[0]["Description"].ToString();
                        products.Pimage = "~/" + dt.Rows[0]["Pimage"].ToString();
                        products.Baserice = dt.Rows[0]["Offerprice"].ToString();
                        products.ProductCategory = dt.Rows[0]["ProductCategory"].ToString();
                        products.Oimage = dt.Rows[0]["Oimage"].ToString();

                        string[] otherImagePaths = dt.Rows[0]["Oimage"].ToString().Split(',');
                        int lastIndex = otherImagePaths.Length - 1;
                        while (lastIndex >= 0 && string.IsNullOrEmpty(otherImagePaths[lastIndex]))
                        {
                            lastIndex--;
                        }
                        products.OtherImg = otherImagePaths.Take(Math.Min(6, lastIndex + 1)).ToArray();
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return View(products);
        }

        private productCls GetProductById(string productId)
        {
            productCls products = new productCls();
            try
            {
                string Query = @"select [Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
      ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
      ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and Id='" + productId + "' order by Id desc";
                DataTable dt = db.GetAllRecord(Query);

                if (dt.Rows.Count > 0)
                {
                    products.ID = dt.Rows[0]["Id"].ToString();
                    products.Name = dt.Rows[0]["Name"].ToString();
                    products.Ptype = dt.Rows[0]["Ptype"].ToString();
                    products.Price = dt.Rows[0]["Price"].ToString();
                    products.Discount = dt.Rows[0]["Discount"].ToString();
                    products.TaxPercent = dt.Rows[0]["TaxPercent"].ToString();
                    products.Description = dt.Rows[0]["Description"].ToString();
                    products.Pimage = "~/" + dt.Rows[0]["Pimage"].ToString();
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return products;
        }

        public ActionResult AddToCart(string productId, int quantity, string PName, string  Price)
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    decimal Price_p = 0;
                    if (!string.IsNullOrEmpty(Price.ToString()))
                    {
                        Price_p = Convert.ToDecimal(Price);
                    }
                    else
                    {
                        Price_p = 0;
                    }
                    List<productCls> ShoppingCart = new List<productCls>();
                    try
                    {
                        string selquery = @"select [id],[Customer_id],[Name],[ProductId],[ProductName]
      ,[ProductQuantity],[status],[Datetime],[logid],[logname]
      , CAST(price AS DECIMAL(18, 2)) AS price
       , CAST([total_price] AS DECIMAL(18, 2)) AS[total_price]

      from[tbl_cart_details] where Customer_id='" + Session["customerid"] + "' and status='Add In Cart' and ProductId='" + productId + "'";
                        DataTable seldt = db.GetAllRecord(selquery);
                        if (seldt.Rows.Count > 0)
                        {
                            int Count_quantity = Convert.ToInt32(seldt.Rows[0]["ProductQuantity"]);
                            int total = Count_quantity + quantity;
                            decimal Count_price = Convert.ToDecimal(seldt.Rows[0]["total_price"]);
                            Price_p = Price_p * quantity;
                            decimal total_price = Count_price + Price_p;
                            string updateQuery = "update tbl_cart_details set ProductQuantity='" + total + "',total_price='" + total_price + "' where Customer_id='" + Session["customerid"] + "' and ProductId='" + productId + "' and id='" + seldt.Rows[0]["id"].ToString() + "'";
                            if (db.InsertUpdateDelete(updateQuery))
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Updated";
                            }
                            else
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Failed";
                            }
                        }
                        else
                        {
                            decimal totalPrice = quantity * Price_p;
                            string insertQuery = "insert into tbl_cart_details(Customer_id,[Name],ProductId,ProductName,ProductQuantity,status,[Datetime],[logid],[logname],price,total_price,memberType) values('" + Session["customerid"] + "','" + Session["username"] + "','" + productId + "','" + PName + "','" + quantity + "','Add In Cart','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["customerid"] + "','" + Session["username"] + "','" + Price + "','" + totalPrice + "','Customer')";

                            if (db.InsertUpdateDelete(insertQuery))
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, insertQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Inserted";
                            }
                            else
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, insertQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Failed";
                            }
                        }

                        getCart();
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
                            error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }
           
            return this.RedirectToAction("Product_Details", new { productID = productId });
        }

        public ActionResult AddToCart_modal(string productId, int quantity, string PName, string Price)
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    decimal Price_p = 0;
                    if (!string.IsNullOrEmpty(Price.ToString()))
                    {
                        Price_p = Convert.ToDecimal(Price);
                        
                    }
                    else
                    {
                        Price_p = 0;
                    }
                    List<productCls> ShoppingCart = new List<productCls>();
                    try
                    {
                        string selquery = @"select [id],[Customer_id],[Name],[ProductId],[ProductName]
      ,[ProductQuantity],[status],[Datetime],[logid],[logname]
      , CAST(price AS DECIMAL(18, 2)) AS price
       , CAST([total_price] AS DECIMAL(18, 2)) AS[total_price]

      from[tbl_cart_details] where Customer_id='" + Session["customerid"] + "' and status='Add In Cart' and ProductId='" + productId + "'";
                        DataTable seldt = db.GetAllRecord(selquery);
                        if (seldt.Rows.Count > 0)
                        {
                            int Count_quantity = Convert.ToInt32(seldt.Rows[0]["ProductQuantity"]);
                            int total = Count_quantity + quantity;

                            decimal Count_price = Convert.ToDecimal(seldt.Rows[0]["total_price"]);
                            Price_p = Price_p * quantity;
                            decimal total_price = Count_price + Price_p;

                            string updateQuery = "update tbl_cart_details set ProductQuantity='" + total + "',total_price='" + total_price + "' where Customer_id='" + Session["customerid"] + "' and ProductId='" + productId + "' and id='" + seldt.Rows[0]["id"].ToString() + "'";
                            if (db.InsertUpdateDelete(updateQuery))
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Updated";
                            }
                            else
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Failed";
                            }
                        }
                        else
                        {
                            decimal totalPrice = quantity * Price_p;
                            string insertQuery = "insert into tbl_cart_details(Customer_id,[Name],ProductId,ProductName,ProductQuantity,status,[Datetime],[logid],[logname],price,total_price,memberType) values('" + Session["customerid"] + "','" + Session["username"] + "','" + productId + "','" + PName + "','" + quantity + "','Add In Cart','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["customerid"] + "','" + Session["username"] + "','" + Price + "','" + totalPrice + "','Customer')";

                            if (db.InsertUpdateDelete(insertQuery))
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, insertQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Inserted";
                            }
                            else
                            {
                                string hid = Session["customerid"].ToString();
                                activitylog.Activitylogins("tbl_cart_details", hid, insertQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                                ViewBag.msg = "Data Failed";
                            }
                        }

                        getCart();
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
                            error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            { }

            return this.RedirectToAction("View_In_cart");
        }
        public ActionResult View_In_cart()
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            //productCls products = new productCls();
            //string Query = "select * from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["customerid"] + "' order by Id desc";
            //DataTable dt = db.GetAllRecord(Query);

            //if (dt.Rows.Count > 0)
            //{
            //    products.ID = dt.Rows[0]["Id"].ToString();
            //    products.Name = dt.Rows[0]["Name"].ToString();
            //    products.Ptype = dt.Rows[0]["Ptype"].ToString();
            //    products.Price = dt.Rows[0]["Price"].ToString();
            //    products.Discount = dt.Rows[0]["Discount"].ToString();
            //    products.TaxPercent = dt.Rows[0]["TaxPercent"].ToString();
            //    products.Description = dt.Rows[0]["Description"].ToString();
            //    products.Pimage = "~/" + dt.Rows[0]["Pimage"].ToString();
            //}
            return View();
        }

        public void getCart()
        {
            try
            {
                string sql = @"SELECT SUM(CAST(ProductQuantity AS INT)) AS totalQuantity
                FROM tbl_cart_details where Customer_id='" + Session["customerid"] + "' and status='Add In Cart' and [logid]='" + Session["customerid"] + "'";
                DataTable tbl = db.GetAllRecord(sql);
                if (tbl.Rows.Count > 0)
                {
                    Session["Card"] = tbl.Rows[0]["totalQuantity"].ToString();
                }
                else
                {
                    Session["Card"] = null;
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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

      

        public ActionResult Checkout(string proid)
        {
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                url = "/Home/Login";
            }
            if (id != null && id != "")
            {
              //  string productid = Encryption.Decrypt(proid);
                string productid = proid;
                ViewBag.productid = productid;
            }
            else
            {
                Response.Redirect(url); ;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection form)
        {
            string msg = "";
            try
            {
                var restableObject = Request.Form["restableObject"];
                var cname = Request.Form["cname"];
                var customerid = Request.Form["customerid"];
                var leadid = Request.Form["leadid"];
                var cnmailuid = Request.Form["cnmailuid"];
                var cnmobile = Request.Form["cnmobile"];
                var shippingaddr = Request.Form["shippingaddr"];
                var shippingpostal = Request.Form["shippingpostal"];
                var mail = Request.Form["mail"];
                var totalprice = Request.Form["totalprice"];
                var paymentmethod = Request.Form["paymentmethod"];
                var payslip = Request.Files["payslip"];
                //string productxml = JsontoXML(restableObject);

                string payslippath = "";
                //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/PayslipUpload/"))))
                //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/PayslipUpload/")));

                //string path = Path.Combine(Server.MapPath("~/Content/PayslipUpload"), payslip.FileName);
                //payslip.SaveAs(path);
                //payslippath = "Content/PayslipUpload/" + payslip.FileName;

                payslippath = APIs.PayslipUpload(payslip);

                DataTable dtres = Encryption.ConvertJSONToDataTable(restableObject);
                string productxml = Encryption.ConvertDatatableTo_XML(dtres);
                //DataTable dtxml = encryption.ConvertXmlTo_Datatable(xmlstr);

                DateTime currentDate = DateTime.Now;
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string orderid = "O" + currentYearmonth + "" + arandom(5);
                
                string query = "INSERT INTO tbl_order_summary(Order_id,Product_xml,Customer_name,Customer_id,Customer_mobile,Customer_address,Pincode,Total_amount,Payment_method,Payment_upload,Payment_status,logid,logname,Datetime,Membertype,Orderby)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','" + totalprice + "','" + paymentmethod + "','" + payslippath + "','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")')";
                if (db.InsertUpdateDelete(query))
                {
                    if (dtres.Rows.Count > 0)
                    {
                        string transactionid = "t" + currentYearmonth + "" + arandom(5);
                        for (int i = 0; i < dtres.Rows.Count; i++)
                        {
                            string Query = @"select * from [tbl_pro_price_circlar] where Id='" + dtres.Rows[i]["Product_Id"] + "' order by Id desc";
                            DataTable dt = db.GetAllRecord(Query);
                            if (dt.Rows.Count > 0)
                            {
                                string orderid22 = arandom(6);
                                string iquery = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[OTP],[ExpDelivery_date],[Remark],OrderID2)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','0','Active','','','0','Due','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")','','','','','','','','','Active','" + Session["username"] + "','" + Session["customerid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','','','" + dtres.Rows[i]["Product_Img"] + "','" + dtres.Rows[i]["Product_Id"] + "','" + dtres.Rows[i]["Product"] + "','" + dtres.Rows[i]["Price"] + "','" + dtres.Rows[i]["Quantity"] + "','" + dtres.Rows[i]["Total"] + "','','" + DateTime.Now.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss") + "','','"+ orderid22 + "')";
                                if (db.InsertUpdateDelete(iquery))
                                {
                                    
                                    msg = "Success";
                                }
                                else
                                {
                                    msg = "Fail";
                                }
                            }                               
                        }

                       

                        string tquery = "INSERT INTO [dbo].[tbl_order_transaction]([Datetime],[Customer_id],[Order_id],[Amount],[Opening_amount],[Closing_amount],[Transaction_id],[Transaction_by_id],[Tr_status],[Status],[logid],[logname],[log_mac],[log_IP]) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + customerid + "','" + orderid + "','0','0','0','" + transactionid + "','" + Session["customerid"] + "','Due','Paid','" + Session["customerid"] + "','" + Session["username"] + "','Mac','IP')";
                        db.InsertUpdateDelete(tquery);
                    }
                    else
                    {
                        msg = "";
                    }

                    //start send SMS and Whatsapp msg..

                    //string sql = "  SELECT * FROM [tbl_login] WHERE  Type = 'AFuser' AND  Token_id IS NOT NULL AND Token_id != ''";
                    //DataTable tbl = db.GetAllRecord(sql);
                    //if(tbl.Rows.Count>0)
                    //{
                    //    for(int hh=0;hh< tbl.Rows.Count;hh++)
                    //    {
                    //        string Mob = tbl.Rows[hh]["Mobile"].ToString();
                    //        string Message = "New Product for Confirmation";
                    //        string name = tbl.Rows[hh]["Username"].ToString();
                    //        string empID = tbl.Rows[hh]["Userid"].ToString();

                    //        WhatsappMessage.SendWhatsappSMS(Mob, Message);
                    //        Messaging.SendSMS(Mob, Message, "", name, "", "");

                    //        Messaging.SendPushNotification("New Product for Confirmation", Message, empID);
                    //    }
                    //}


                    // end send SMS and Whatsapp msg..
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return Content(msg);
        }

        [HttpPost]
        public ActionResult Checkout_old()
        {
            try
            {
                var restableObject = Request.Form["restableObject"];
                var cname = Request.Form["cname"];
                var customerid = Request.Form["customerid"];
                var leadid = Request.Form["leadid"];
                var cnmailuid = Request.Form["cnmailuid"];
                var cnmobile = Request.Form["cnmobile"];
                var shippingaddr = Request.Form["shippingaddr"];
                var shippingpostal = Request.Form["shippingpostal"];
                var mail = Request.Form["mail"];
                var totalprice = Request.Form["totalprice"];
                var paymentmethod = Request.Form["paymentmethod"];
                var payslip = Request.Files["payslip"];
                //string productxml = JsontoXML(restableObject);


                DataTable dtres = Encryption.ConvertJSONToDataTable(restableObject);
                string productxml = Encryption.ConvertDatatableTo_XML(dtres);
                //DataTable dtxml = encryption.ConvertXmlTo_Datatable(xmlstr);

                string payslippath = "";

                //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/PayslipUpload/"))))
                //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/PayslipUpload/")));

                //string path = Path.Combine(Server.MapPath("~/Content/PayslipUpload"), payslip.FileName);
                //payslip.SaveAs(path);
                //payslippath = "Content/PayslipUpload/" + payslip.FileName;
                payslippath = APIs.PayslipUpload(payslip);

                DateTime currentDate = DateTime.Now;
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string orderid = "O" + currentYearmonth + "" + arandom(5);
                string query = "INSERT INTO tbl_order(Order_id,Product_xml,Customer_name,Customer_id,Customer_mobile,Customer_address,Pincode,Total_amount,Order_status,Payment_method,Payment_upload,Payment_status,logid,logname,Datetime,Membertype,Orderby)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','" + totalprice + "','Active','" + paymentmethod + "','" + payslippath + "','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")')";
                if (db.InsertUpdateDelete(query))
                {
                    string query1 = "update tbl_lead set Orderstatus='Done' where Customerid='" + customerid + "'";
                    db.InsertUpdateDelete(query1);
                    return Content("Success");
                }
                else
                {
                    return Content("Fail");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                }

                catch
                {

                }
                return Content("Error");
            }
            finally
            {
                db.connectionstate();
            }
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

        private string JsontoXML(string TableObject)
        {
            // Parse the JSON string into a JObject
            JObject jsonData = JObject.Parse(TableObject);
            // Create an XML document
            XDocument xmlDoc = new XDocument();
            // Create the root element
            XElement rootElement = new XElement("Data");
            // Create the tableHeadings element
            XElement headingsElement = new XElement("tableHeadings");
            foreach (string heading in jsonData["tableHeadings"])
            {
                headingsElement.Add(new XElement("heading", heading));
            }
            // Create the tableData element
            XElement dataElement = new XElement("tableData");
            foreach (JArray row in jsonData["tableData"])
            {
                XElement rowElement = new XElement("row");
                foreach (string value in row)
                {
                    rowElement.Add(new XElement("column", value));
                }
                dataElement.Add(rowElement);
            }
            // Build the XML structure
            rootElement.Add(headingsElement);
            rootElement.Add(dataElement);
            xmlDoc.Add(rootElement);
            // Convert the XML document to string
            string xmlString = xmlDoc.ToString();
            return xmlString;
        }
        [HttpPost]
        public ActionResult DeletefromCart(int ProductId,int cartId)
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    string deleteQuery = "delete from tbl_cart_details where id='" + cartId + "' and ProductId='" + ProductId + "' and Customer_id='" + Session["customerid"] + "'";
                    if (db.InsertUpdateDelete(deleteQuery))
                    {
                        getCart();
                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, deleteQuery, "Success", "Delete Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Deleted";
                    }
                    else
                    {
                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, deleteQuery, "Success", "Delete Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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

        [HttpPost]
        public ActionResult Increment_InCart(int ProductId, int cartId,int ProductQty,string Price)
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    int Count_quantity = Convert.ToInt32(ProductQty);
                   
                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_details set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "' where Customer_id='" + Session["customerid"] + "' and ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                        getCart();

                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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

        [HttpPost]
        public ActionResult Decrement_InCart(int ProductId, int cartId, int ProductQty, string Price)
        {
            var dateee = "";
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    int Count_quantity = Convert.ToInt32(ProductQty);

                    decimal Count_price = Convert.ToDecimal(Price);
                    decimal total_price = Count_quantity * Count_price;

                    string updateQuery = "update tbl_cart_details set ProductQuantity='" + Count_quantity + "',total_price='" + total_price + "' where Customer_id='" + Session["customerid"] + "' and ProductId='" + ProductId + "' and id='" + cartId + "'";
                    if (db.InsertUpdateDelete(updateQuery))
                    {
                        getCart();

                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Updated";
                    }
                    else
                    {
                        string hid = Session["customerid"].ToString();
                        activitylog.Activitylogins("tbl_cart_details", hid, updateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                        ViewBag.msg = "Data Failed";
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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


        public JsonResult GetStatedetail(string State)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_state where Statecode='" + State + "'";
                activitylog.Activitylogins("tbl_state", State, query, "Failed", "", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string Statecode = dt.Rows[0]["Statecode"] + "";
                    string Statename = dt.Rows[0]["Statename"] + "";

                    res = new string[2] { Statecode, Statename };

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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
        public JsonResult GetCitydetail(string City)
        {
            string[] res = { };
            try
            {
                string query = "select * from tbl_city where Id='" + City + "'";
                activitylog.Activitylogins("tbl_city", City, query, "Failed", "", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    string Citycode = dt.Rows[0]["Id"] + "";
                    string Cityname = dt.Rows[0]["Cityname"] + "";

                    res = new string[2] { Citycode, Cityname };

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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
        public JsonResult GetCity(string State)
        {
            string res = "";
            try
            {
                res = "<option selected disabled value=''>Select one</option>";
                string query = "select * from tbl_city where Statecode='" + State + "'";
                activitylog.Activitylogins("tbl_city", "", query, "Failed", "", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                DataTable dt = db.GetAllRecord(query);
                activitylog.Activitylogupd("Success", "");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        res += "<option value='" + dt.Rows[i]["Id"] + "'>" + dt.Rows[i]["Cityname"] + "</option>";
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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

        public class UserDetails
        {
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string PinCode { get; set; }
            public string Address { get; set; }
            public string CustomerID { get; set; }
        }
        public JsonResult GetDetailsUser()
        {
            UserDetails UserData = new UserDetails();
            try
            {
                string Query34 = @"select * from [tbl_customer] where Customer_id='" + Session["customerid"].ToString() + "' and Status='Active'";
                DataTable tbll = db.GetAllRecord(Query34);
                if(tbll.Rows.Count>0)
                {
                    UserData = new UserDetails
                    {
                        Name = tbll.Rows[0]["Name"].ToString(),
                        Mobile = tbll.Rows[0]["Mobile"].ToString(),
                        Email = tbll.Rows[0]["Email"].ToString(),
                        State = tbll.Rows[0]["State"].ToString(),
                        City = tbll.Rows[0]["City"].ToString(),
                        PinCode = tbll.Rows[0]["Postal_code"].ToString(),
                        Address = tbll.Rows[0]["Address"].ToString(),
                        CustomerID = tbll.Rows[0]["Name"].ToString(),
                    };
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return Json(UserData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuyNow(string productID)
        {
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                url = "/Home/Login";
            }
            if (id != null && id != "")
            {
                ViewBag.productid = productID;
            }
            else
            {
                Response.Redirect(url); ;
            }
            return View();
        }
        [HttpPost]
        public ActionResult BuyNow()
        {
            string msg = "";
            try
            {
                var restableObject = Request.Form["restableObject"];
                var cname = Request.Form["cname"];
                var customerid = Request.Form["customerid"];
                var leadid = Request.Form["leadid"];
                var cnmailuid = Request.Form["cnmailuid"];
                var cnmobile = Request.Form["cnmobile"];
                var shippingaddr = Request.Form["shippingaddr"];
                var shippingpostal = Request.Form["shippingpostal"];
                var mail = Request.Form["mail"];
                var totalprice = Request.Form["totalprice"];
                var paymentmethod = Request.Form["paymentmethod"];
                var payslip = Request.Files["payslip"];
                //string productxml = JsontoXML(restableObject);

                string payslippath = "";
                //if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/PayslipUpload/"))))
                //    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/PayslipUpload/")));

                //string path = Path.Combine(Server.MapPath("~/Content/PayslipUpload"), payslip.FileName);
                //payslip.SaveAs(path);
                //payslippath = "Content/PayslipUpload/" + payslip.FileName;
                payslippath = APIs.PayslipUpload(payslip);

                DataTable dtres = Encryption.ConvertJSONToDataTable(restableObject);
                string productxml = Encryption.ConvertDatatableTo_XML(dtres);
                //DataTable dtxml = encryption.ConvertXmlTo_Datatable(xmlstr);

                DateTime currentDate = DateTime.Now;
                // Extract the year and month from the current date
                string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
                string orderid = "O" + currentYearmonth + "" + arandom(5);
                string query = "INSERT INTO tbl_order_summary(Order_id,Product_xml,Customer_name,Customer_id,Customer_mobile,Customer_address,Pincode,Total_amount,Payment_method,Payment_upload,Payment_status,logid,logname,Datetime,Membertype,Orderby)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','" + totalprice + "','" + paymentmethod + "','" + payslippath + "','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")')";
                if (db.InsertUpdateDelete(query))
                {
                    if (dtres.Rows.Count > 0)
                    {
                        string transactionid = "t" + currentYearmonth + "" + arandom(5);
                        for (int i = 0; i < dtres.Rows.Count; i++)
                        {
                            string Query = @"select * from [tbl_pro_price_circlar] where Id='" + dtres.Rows[i]["Product_Id"] + "' order by Id desc";
                            DataTable dt = db.GetAllRecord(Query);
                            if (dt.Rows.Count > 0)
                            {
                                string orderid22 = arandom(6);
                                string iquery = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[OTP],[ExpDelivery_date],[Remark],OrderID2)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','0','Active','','','0','Due','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")','','','','','','','','','Active','" + Session["username"] + "','" + Session["customerid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','','','" + dtres.Rows[i]["Product_Img"] + "','" + dtres.Rows[i]["Product_Id"] + "','" + dtres.Rows[i]["Product"] + "','" + dtres.Rows[i]["Price"] + "','" + dtres.Rows[i]["Quantity"] + "','" + dtres.Rows[i]["Total"] + "','','" + DateTime.Now.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss") + "','','"+ orderid22 + "')";
                                if (db.InsertUpdateDelete(iquery))
                                {
                                    //string query1 = "update tbl_lead set Orderstatus='Done' where Customerid='" + customerid + "'";
                                    //db.InsertUpdateDelete(query1);
                                    msg = "Success";
                                }
                                else
                                {
                                    msg = "Fail";
                                }
                            }
                        }
                        string tquery = "INSERT INTO [dbo].[tbl_order_transaction]([Datetime],[Customer_id],[Order_id],[Amount],[Opening_amount],[Closing_amount],[Transaction_id],[Transaction_by_id],[Tr_status],[Status],[logid],[logname],[log_mac],[log_IP]) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + customerid + "','" + orderid + "','0','0','0','" + transactionid + "','" + Session["customerid"] + "','Due','Paid','" + Session["customerid"] + "','" + Session["username"] + "','Mac','IP')";
                        db.InsertUpdateDelete(tquery);


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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            return Content(msg);

            //try
            //{
            //    var restableObject = Request.Form["restableObject"];
            //    var cname = Request.Form["cname"];
            //    var customerid = Request.Form["customerid"];
            //    var leadid = Request.Form["leadid"];
            //    var cnmailuid = Request.Form["cnmailuid"];
            //    var cnmobile = Request.Form["cnmobile"];
            //    var shippingaddr = Request.Form["shippingaddr"];
            //    var shippingpostal = Request.Form["shippingpostal"];
            //    var mail = Request.Form["mail"];
            //    var totalprice = Request.Form["totalprice"];
            //    var paymentmethod = Request.Form["paymentmethod"];
            //    var payslip = Request.Files["payslip"];
            //    //string productxml = JsontoXML(restableObject);


            //    Encryption encryption = new Encryption();
            //    DataTable dtres = encryption.ConvertJSONToDataTable(restableObject);
            //    string productxml = encryption.ConvertDatatableTo_XML(dtres);
            //    //DataTable dtxml = encryption.ConvertXmlTo_Datatable(xmlstr);

            //    string payslippath = "";

            //    if (!Directory.Exists(Path.Combine(Server.MapPath("~/Content/PayslipUpload/"))))
            //        Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Content/PayslipUpload/")));

            //    string path = Path.Combine(Server.MapPath("~/Content/PayslipUpload"), payslip.FileName);
            //    payslip.SaveAs(path);
            //    payslippath = "Content/PayslipUpload/" + payslip.FileName;

            //    DateTime currentDate = DateTime.Now;
            //    // Extract the year and month from the current date
            //    string currentYearmonth = currentDate.Day + "" + currentDate.Month + "" + currentDate.Year;
            //    string orderid = "O" + currentYearmonth + "" + arandom(5);
            //    string query = "INSERT INTO tbl_order(Order_id,Product_xml,Customer_name,Customer_id,Customer_mobile,Customer_address,Pincode,Total_amount,Order_status,Payment_method,Payment_upload,Payment_status,logid,logname,Datetime,Membertype,Orderby)VALUES('" + orderid + "','" + productxml + "','" + cname + "','" + customerid + "','" + cnmobile + "','" + shippingaddr + "','" + shippingpostal + "','" + totalprice + "','Active','" + paymentmethod + "','" + payslippath + "','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")')";
            //    if (db.InsertUpdateDelete(query))
            //    {
            //        string query1 = "update tbl_lead set Orderstatus='Done' where Customerid='" + customerid + "'";
            //        db.InsertUpdateDelete(query1);
            //        return Content("Success");
            //    }
            //    else
            //    {
            //        return Content("Fail");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        Error_15_16 error_15_16 = new Error_15_16();
            //        string pageUrl = "", moduleName = "", errorLine = "", errorMessage = "", errorName = "", errorTrace = "";
            //        // Get the page URL, if available
            //        pageUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            //        // Get the module name
            //        moduleName = ex.TargetSite.Module.Name;
            //        // Get the error line number, if available
            //        var stackTrace = ex.StackTrace;
            //        if (!string.IsNullOrEmpty(stackTrace))
            //        {
            //            var lineNumberIndex = stackTrace.LastIndexOf(":line ");
            //            if (lineNumberIndex >= 0)
            //            {
            //                var lineNumber = stackTrace.Substring(lineNumberIndex + 6);
            //                var newLineIndex = lineNumber.IndexOf(Environment.NewLine);
            //                if (newLineIndex >= 0)
            //                {
            //                    errorLine = lineNumber.Substring(0, newLineIndex);
            //                }
            //                else
            //                {
            //                    errorLine = lineNumber;
            //                }
            //            }
            //        }
            //        // Get the error message and name
            //        if (ex.Message.ToString().Length >= 1000)
            //        {
            //            errorMessage = ex.Message.Substring(1, 500);
            //        }
            //        else
            //        {
            //            errorMessage = ex.Message;
            //        }
            //        errorName = ex.GetType().FullName;
            //        // Get the error trace
            //        errorTrace = ex.StackTrace;
            //        error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
            //    }

            //    catch
            //    {

            //    }
            //    return Content("Error");
            //}
            //finally
            //{
            //    db.connectionstate();
            //}
        }


        public JsonResult GetSuggestions(string term)
        {
            var Suggestions = "";
            var suggestions = new List<string>();
            try {
                
                string query = "select top 15 Name from [tbl_pro_price_circular] WHERE Name LIKE @searchTerm";
                SqlParameter searchTermParam = new SqlParameter("@searchTerm", "%" + term + "%");
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.Add(searchTermParam);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suggestions.Add(reader["Productname"].ToString());
                        }
                    }
                }
                con.Close();

               //var filteredSuggestions = suggestions.Where(s => s.ToLower().Contains(term.ToLower()));
            }
            catch(Exception ex)
            { }
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

      
        public JsonResult View_product_in_modal(string productID)
        {
            string[] res = { };
            productCls products = new productCls();
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    string Query = @"
                        select[Id],[Ptype],[Productname],[Description],CAST(Sellprice AS DECIMAL(18, 2)) AS Sellprice
        ,[Discountpercent],[Pimage],[Oimage],[ProductCategory],[Status]
        ,[Datetime],[logid],[logname], CAST([Offerprice] AS DECIMAL(18, 2)) AS[Offerprice] from[tbl_pro_price_circlar] where Status='Active' and Id='" + productID + "' order by Id desc";
                    DataTable dt = db.GetAllRecord(Query);

                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.modalDt = dt;

                        string id = dt.Rows[0]["Id"].ToString() + "";
                        string encryId =Encryption.Encrypt(dt.Rows[0]["Id"].ToString()) + "";
                        string Name = dt.Rows[0]["Productname"].ToString() + "";
                        string Ptype = dt.Rows[0]["Ptype"].ToString() + "";
                        string Price = dt.Rows[0]["Sellprice"].ToString() + "";
                        string Discount = dt.Rows[0]["Discountpercent"].ToString() + "";
                        string TaxPercent = "";
                        string Description = dt.Rows[0]["Description"].ToString() + "";
                        string Pimage = dt.Rows[0]["Pimage"].ToString() + "";
                        string Baserice = dt.Rows[0]["Offerprice"].ToString() + "";
                        string ProductCategory = dt.Rows[0]["ProductCategory"].ToString() + "";
                        string[] otherImagePaths1 = dt.Rows[0]["Oimage"].ToString().Split(',');
                        int lastIndex1 = otherImagePaths1.Length - 1;
                        while (lastIndex1 >= 0 && string.IsNullOrEmpty(otherImagePaths1[lastIndex1]))
                        {
                            lastIndex1--;
                        }
                        string[] OtherImg1 = otherImagePaths1.Take(Math.Min(5, lastIndex1 + 1)).ToArray();

                        string otherImg1String = string.Join(",", OtherImg1);

                        res = new string[12] { id, Name, Ptype, Price, Discount, TaxPercent, Description, Pimage, Baserice, ProductCategory, otherImg1String, encryId };
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult My_order()
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            //productCls products = new productCls();
            //string Query = "select * from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["customerid"] + "' order by Id desc";
            //DataTable dt = db.GetAllRecord(Query);

            //if (dt.Rows.Count > 0)
            //{
            //    products.ID = dt.Rows[0]["Id"].ToString();
            //    products.Name = dt.Rows[0]["Name"].ToString();
            //    products.Ptype = dt.Rows[0]["Ptype"].ToString();
            //    products.Price = dt.Rows[0]["Price"].ToString();
            //    products.Discount = dt.Rows[0]["Discount"].ToString();
            //    products.TaxPercent = dt.Rows[0]["TaxPercent"].ToString();
            //    products.Description = dt.Rows[0]["Description"].ToString();
            //    products.Pimage = "~/" + dt.Rows[0]["Pimage"].ToString();
            //}
            return View();
        }

        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult My_order_details()
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                   
                 }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
            //productCls products = new productCls();
            //string Query = "select * from tbl_cart_details where Status='Add In Cart' and Customer_id='" + Session["customerid"] + "' order by Id desc";
            //DataTable dt = db.GetAllRecord(Query);

            //if (dt.Rows.Count > 0)
            //{
            //    products.ID = dt.Rows[0]["Id"].ToString();
            //    products.Name = dt.Rows[0]["Name"].ToString();
            //    products.Ptype = dt.Rows[0]["Ptype"].ToString();
            //    products.Price = dt.Rows[0]["Price"].ToString();
            //    products.Discount = dt.Rows[0]["Discount"].ToString();
            //    products.TaxPercent = dt.Rows[0]["TaxPercent"].ToString();
            //    products.Description = dt.Rows[0]["Description"].ToString();
            //    products.Pimage = "~/" + dt.Rows[0]["Pimage"].ToString();
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Cancel_MyOrder(string OrderId,string OrderId2)
        {
            try
            {
                if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
                {
                    //string UpdateQuery = "update tbl_order_summary set  Payment_status='Cancelled' where Order_id='" + OrderId + "' and Customer_id='" + Session["customerid"] + "'";


                   // if (db.InsertUpdateDelete(UpdateQuery))
                  //  {
                        string sql = "select * from tbl_order where Order_id='" + OrderId + "' and OrderID2='" + OrderId2 + "' and Customer_id='" + Session["customerid"] + "'";
                        DataTable tbl = db.GetAllRecord(sql);
                        if(tbl.Rows.Count>0)
                        {
                            string iquery = "INSERT INTO [tbl_order]([Order_id],[Product_xml],[Customer_name],[Customer_id],[Customer_mobile],[Customer_address],[Pincode],[Total_amount],[Order_status],[Payment_method],[Payment_upload],[Paid_amount],[Payment_status],[logid],[logname],[Datetime],[Membertype],[Orderby],[Invoiceid],[Drivername],[Driverid],[Drivermobile],[Vehiclename],[Vehicleid],[Transport_status],[Confirm_date],[Status],[Actionbyname],[Actionbyid],[ActionDatetime],[BranchName],[BranchCode],[Product_Img],[Product_id],[Product_name],[Product_price],[Product_quantity],[Total_proamount],[OTP],[ExpDelivery_date],[Remark],OrderID2)VALUES('" + tbl.Rows[0]["Order_id"] + "','" + tbl.Rows[0]["Product_xml"] + "','" + tbl.Rows[0]["Customer_name"] + "','" + tbl.Rows[0]["Customer_id"] + "','" + tbl.Rows[0]["Customer_mobile"] + "','" + tbl.Rows[0]["Customer_address"] + "','" + tbl.Rows[0]["Pincode"] + "','0','Cancelled','','','0','Due','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["membertype"] + "','" + Session["username"] + "(" + Session["customerid"] + ")','','','','','','','','','Active','" + Session["username"] + "','" + Session["customerid"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + tbl.Rows[0]["BranchName"] + "','" + tbl.Rows[0]["BranchCode"] + "','" + tbl.Rows[0]["Product_Img"] + "','" + tbl.Rows[0]["Product_id"] + "','" + tbl.Rows[0]["Product_name"] + "','" + tbl.Rows[0]["Product_price"] + "','" + tbl.Rows[0]["Product_quantity"] + "','" + tbl.Rows[0]["Total_proamount"] + "','','" + tbl.Rows[0]["ExpDelivery_date"] + "','','" + tbl.Rows[0]["OrderID2"] + "')";
                            if (db.InsertUpdateDelete(iquery))
                            {
                               string UpdateQuery22 = "update tbl_order set  Status='Inactive' where Order_id='" + OrderId + "' and Customer_id='" + Session["customerid"] + "' and OrderID2='" + OrderId2 + "' and Id='" + tbl.Rows[0]["Id"] + "'";
                                if (db.InsertUpdateDelete(UpdateQuery22))
                                {
                                    string hid = Session["customerid"].ToString();
                                    activitylog.Activitylogins("tbl_order_summary", hid, UpdateQuery22, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());

                                    activitylog.Activitylogins("tbl_order", hid, iquery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());

                                    ViewBag.msg = "Data updatd";
                                }
                            }
                        }
                  //  }
                 //   else
                  //  {
                        //string hid = Session["customerid"].ToString();
                        //activitylog.Activitylogins("tbl_order_summary", hid, UpdateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());

                        //activitylog.Activitylogins("tbl_order", hid, UpdateQuery, "Success", "Update Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());

                        //ViewBag.msg = "Data Failed";
                  //  }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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


        public ActionResult View_profile()
        {
            string url = "";
            string id = Session["customerid"] + "";
            string token = Session["gtokenid"] + "";
            if (token != null && token != "")
            {
                url = "/Home/AppLogin?token=" + token;
            }
            else
            {
                //url = "/Home/Login";
            }
            if (id != null && id != "")
            {

            }
            else
            {
                Response.Redirect(url); ;
            }
            return View();
        }


        //feedback open


        public JsonResult GiveFeedback()
        {
            string res = "";
            try
            {
                string attachmentList = ""; // Initialize an empty string to store file names

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    //string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);
                    //string filePath = Path.Combine(Server.MapPath("~/Content/Feedback"), uniqueFileName);
                    //file.SaveAs(filePath);

                    //// Concatenate the file names with a comma separator
                    //if (!string.IsNullOrEmpty(attachmentList))
                    //{
                    //    attachmentList += ","; // Add a comma if it's not the first file
                    //}

                    attachmentList += APIs.Feedback(file)+",";
                }
                var Order_id = Request.Form["Order_id"];
                var product_id = Request.Form["product_id"];
                var ratingInput = Request.Form["ratingInput"];
                var review = Request.Form["review"];

                // Your other logic here

                string pratings = "";
                string productname = "";
                for (int j = 0; j < Convert.ToInt32(ratingInput); j++)
                {
                    pratings += "<i class=@fa fa-star text-warning sstar@></i>";
                }

                string query5 = "select * from tbl_pro_price_circular Where Id='" + product_id + "'";
                DataTable dt5 = db.GetAllRecord(query5);
                if (dt5.Rows.Count > 0)
                {
                    productname = dt5.Rows[0]["Productname"].ToString();
                }

                string addcmpl = "insert into tbl_feedback(Ratingcount,Feedback,Attachment,Status,Customername,Customerid,Customeremail,Rating,Datetime,Productid,Productname,Orderid)VALUES('" + ratingInput + "','" + review + "','" + attachmentList + "','Active','" + Session["username"] + "','" + Session["customerid"] + "','" + Session["usermail"] + "','" + pratings + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + product_id + "','" + productname + "','" + Order_id + "')";

                if (db.InsertUpdateDelete(addcmpl))
                {
                    activitylog.Activitylogins("tbl_feedback", db.getmaxid("tbl_feedback").ToString(), addcmpl, "Success", "Insert Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                    ViewBag.msg = "Data Save Failed";
                }
                else
                {
                    activitylog.Activitylogins("tbl_feedback", "", addcmpl, "Failed", "Insert Failed", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                    ViewBag.msg = "Data Save Failed";
                }

                res = "Success";
            }
            catch (Exception ex)
            {
                res = "Error";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //feedback close


        #endregion   // end customer..











        //Start Support

        #region .... customer support....
        [HttpGet]
        public ActionResult addcomplain()
        {
            if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public ActionResult addcomplain(IEnumerable<HttpPostedFileBase> Attachement, string prevAttachement, FormCollection form)
        {
            try
            {
                string fdpath = "";
                if (Attachement != null)
                {
                    foreach (var file in Attachement)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            fdpath += APIs.ComplainUpload(file) + ",";
                        }
                        else
                        {
                            fdpath = prevAttachement;
                        }
                    }
                }
                else
                {
                    fdpath = "No file Upload";
                }

                string cmpID = arandom(6);
                string addcmpl = "INSERT INTO [dbo].[tbl_Complaint] ([CmpID] ,[ComplaintType] ,[Subject] ,[Priority] ,[Complaint] ,[Attachment] ,[Branchname] ,[Branchcode] ,[Complaint_status] ,[Remark] ,[Reply] ,[Assignname] ,[Assignid] ,[Status] ,[Logid] ,[Logname] ,[Datetime],Complaintbyid,Complaintbyname) VALUES ('" + cmpID + "','" + form["cmptype"] + "','" + form["Subject"] + "','" + form["Priority"] + "','" + form["complaint"] + "','" + fdpath + "','','','Open','" + form["complaint"] + "','" + form["complaint"] + "','','','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Session["customerid"] + "','" + Session["username"] + "')";

                if (db.InsertUpdateDelete(addcmpl))
                {
                    activitylog.Activitylogins("tbl_Complaint", db.getmaxid("tbl_Complaint").ToString(), addcmpl, "Success", "Insert Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                    ViewBag.msg = "Data Saved";
                }
                else
                {
                    activitylog.Activitylogins("tbl_Complaint", "", addcmpl, "Failed", "Insert Failed", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
                    error_15_16.Error_15_16ins(pageUrl, moduleName, errorLine, errorMessage.Replace("'", "`"), errorName.Replace("'", "`"), errorTrace.Replace("'", "`"), Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
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
        public JsonResult getcmpdetails(string CmpID)
        {
            string getcmp = "SELECT * FROM tbl_Complaint where CmpID='" + CmpID + "'";
            DataTable tbll = db.GetAllRecord(getcmp);
            string res1 = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult showcomplain()
        {
            if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public JsonResult getsubbycmptype(string selectedcmp)
        {
            string Jsonn;
            string getsub = "SELECT * FROM tbl_complain_subject where Status='Active' and Complaintype='" + selectedcmp + "'";
            DataTable tbll = db.GetAllRecord(getsub);

            Jsonn = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(Jsonn, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Reply(string cm)
        {
            if (Session["customerid"] + "" != null && Session["customerid"] + "" != "")
            {
                string query5 = "select * from tbl_Complaint Where CmpID='" + cm + "'";
                DataTable dt5 = db.GetAllRecord(query5);
                return View(dt5);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            
        }
        [HttpPost]
        public ActionResult Reply(string ctype, string sub, string priority, string cmp, string CmpID, string customerreply, IEnumerable<HttpPostedFileBase> Attachement, string prevAttachement)
        {
            string fdpath = "";
            foreach (var file in Attachement)
            {
                if (file != null && file.ContentLength > 0)
                {
                    fdpath += APIs.ComplainUpload(file) + ",";
                }
                else
                {
                    fdpath = prevAttachement;
                }
            }

            string compquery = "select * from tbl_Complaint where CmpID='" + CmpID + "' and Status='Active'";
            DataTable compdt = db.GetAllRecord(compquery);
            if (compdt.Rows.Count > 0)
            {
                string status = "";
                string Assignid = "", Assignname = "";
                if (compdt.Rows[0]["Complaint_status"] + "" == "Open")
                {
                    status = "Open";
                }
                else if (compdt.Rows[0]["Complaint_status"] + "" == "Assign")
                {
                    status = "Assign";
                    Assignname = compdt.Rows[0]["Assignname"] + "";
                    Assignid = compdt.Rows[0]["Assignid"] + "";
                }
                else if (compdt.Rows[0]["Complaint_status"] + "" == "In Process")
                {
                    status = "In Process";
                    Assignname = compdt.Rows[0]["Assignname"] + "";
                    Assignid = compdt.Rows[0]["Assignid"] + "";
                }
                else if (compdt.Rows[0]["Complaint_status"] + "" == "Close")
                {
                    status = "Open";
                    Assignname = compdt.Rows[0]["Assignname"] + "";
                    Assignid = compdt.Rows[0]["Assignid"] + "";
                }

                string addcmpl = "INSERT INTO [dbo].[tbl_Complaint] ([CmpID] ,[ComplaintType] ,[Subject] ,[Priority] ,[Complaint] ,[Attachment] ,[Branchname] ,[Branchcode] ,[Complaint_status] ,[Remark] ,[Reply] ,[Assignname] ,[Assignid] ,[Status] ,[Logid] ,[Logname] ,[Datetime],Complaintbyid,Complaintbyname) VALUES ('" + CmpID + "','" + compdt.Rows[0]["ComplaintType"] + "','" + compdt.Rows[0]["Subject"] + "','" + compdt.Rows[0]["Priority"] + "','" + compdt.Rows[0]["Complaint"] + "','" + fdpath + "','','','" + status + "','" + compdt.Rows[0]["Remark"] + "','" + customerreply + "','" + Assignname + "','" + Assignid + "','Active','" + Session["customerid"] + "','" + Session["username"] + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + compdt.Rows[0]["Complaintbyid"] + "','" + compdt.Rows[0]["Complaintbyname"] + "')";

                db.InsertUpdateDelete("update tbl_Complaint set Status='Inactive' where CmpID='" + CmpID + "'");
                if (db.InsertUpdateDelete(addcmpl))
                {
                    activitylog.Activitylogins("tbl_Complaint", db.getmaxid("tbl_Complaint").ToString(), addcmpl, "Success", "Insert Success", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                    ViewBag.msg = "Data Saved";
                    string query5 = "select * from tbl_Complaint Where CmpID='" + CmpID + "'";
                    DataTable dt5 = db.GetAllRecord(query5);
                    return View(dt5);
                }
                else
                {
                    activitylog.Activitylogins("tbl_Complaint", "", addcmpl, "Failed", "Insert Failed", Session["customerid"].ToString(), Session["username"].ToString(), Session["usermail"].ToString());
                    ViewBag.msg = "Data Save Failed";
                }
            }


            
            return View();
        }
        public JsonResult getall()
        {
            string getcmp = "SELECT * FROM tbl_Complaint where Complaintbyid='" + Session["customerid"] + "' and Status='Active'";
            DataTable tbll = db.GetAllRecord(getcmp);
            string res1 = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getopen()
        {
            string getcmp = "SELECT * FROM tbl_Complaint where Complaint_status='Open' and Complaintbyid='" + Session["customerid"] + "' and Status='Active'";
            DataTable tbll = db.GetAllRecord(getcmp);
            string res1 = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getprogress()
        {
            string getcmp = "SELECT * FROM tbl_Complaint where Complaint_status='In Process' and Complaintbyid='" + Session["customerid"] + "' and Status='Active'";
            DataTable tbll = db.GetAllRecord(getcmp);
            string res1 = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getclose()
        {
            string getcmp = "SELECT * FROM tbl_Complaint where Complaint_status='Close' and Complaintbyid='" + Session["customerid"] + "' and Status='Active'";
            DataTable tbll = db.GetAllRecord(getcmp);
            string res1 = JsonConvert.SerializeObject(tbll, Formatting.None);
            return Json(res1, JsonRequestBehavior.AllowGet);
        }



        #endregion

        //Close Support


    }
}
