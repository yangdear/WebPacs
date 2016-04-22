using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebViewer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WebViewer.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserManagerController()
        { }
        public UserManagerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: UserManager
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">医院id</param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            List<RegisterViewModel> list = new List<RegisterViewModel>();
            if (id.HasValue)
            {
                var hospital = db.Hospitals.Find(id);
                if (hospital == null)
                {
                    return HttpNotFound();
                }
                foreach (var user in hospital.Operators)
                {
                    list.Add(new RegisterViewModel() { UserName = user.UserName, Hospital = user.Hospital });
                }
                ViewData.Model = list;
            }
            else
            {
                foreach (var user in UserManager.Users)
                {
                    list.Add(new RegisterViewModel() { UserName = user.UserName, Hospital = user.Hospital });
                }
                ViewData.Model = list;

            }
            return View();
        }

        public ActionResult Create(int id)
        {

            ViewBag.HospitalName = db.Hospitals.Find(id).HospitalName;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(int id, RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };
                user.HospitalId = id;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                    // 发送包含此链接的电子邮件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");
                    var r = UserManager.AddToRole(user.Id, "operator");
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}