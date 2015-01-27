using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WoodDigital.Controllers
{
    public class siteController : Controller
    {

        public const String _email_from_login = "landingmakers@gmail.com";
        public const String _email_from = "landingmakers@gmail.com";
        public const String _email_from_pass = "landingmakers123.ru";
        public const String _email_to = "info@wood-digital.com";
        public const String _email_to2 = "lexx_moscow@bk.ru";
        public const int _email_port = 587;
        public const String _email_host = "smtp.gmail.com";
        public const bool _email_ssl = true;

        public ActionResult index()
        {
            return View();
        }
        public ActionResult main()
        {
            return View();
        }

        // ОТПРАВКА ЗАКАЗА
        [HttpPost, ValidateInput(false)]
        public JsonResult sendmail()
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential(_email_from_login, _email_from_pass);
                client.Port = _email_port;
                client.Host = _email_host;
                client.EnableSsl = _email_ssl;

                var maFrom = new MailAddress(_email_from);
                var maTo = new MailAddress(_email_to);
                var mmsg = new MailMessage(maFrom.Address, maTo.Address);

                mmsg.To.Add(_email_to2); // добавляем менеджера по продажам


                mmsg.Subject = Request.Form["title"];
                mmsg.SubjectEncoding = Encoding.UTF8;
                mmsg.IsBodyHtml = true;
                mmsg.BodyEncoding = Encoding.UTF8;

                var body = "Со страницы ";
                try
                {
                    body += String.IsNullOrEmpty(Request.Form["url"]) ? (Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : Request.Url.AbsoluteUri) : Request.Form["url"];
                }
                catch { }
                body += " пришёл запрос.<br/>";
                body += "Время сервера " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "<br/>";
                body += "<br/><strong>Имя</strong>: " + Request.Form["name"];
                //body += "<br/><strong>Email</strong>: " + Request.Form["email"];
                body += "<br/><strong>Телефон</strong>: " + Request.Form["phone"];
                body += "<br/><strong>Примечание</strong>: " + Request.Form["what"];
                var ip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
                if (!String.IsNullOrEmpty(ip))
                {
                    body += "<br/><strong>IP адрес</strong>: <a href=\"http://2ip.ru/geoip/?ip=" + ip + "\" target=\"_blank\">" + ip + "</a>";
                }

                mmsg.Body = body;
                client.Send(mmsg);

                return Json(new { status = 1 });
            }
            catch (Exception ee)
            {
                return Json(new { status = 0, error = "Ошибка: " + ee.Message });
            }
        }
    }
}
