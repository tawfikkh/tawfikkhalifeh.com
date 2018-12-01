using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using tawfikkhalifeh.Entity;
using tawfikkhalifeh.Models;

namespace tawfikkhalifeh.Controllers
{
    public class FormController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> PostContact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await SaveToDb(model);
                //await SendEmail(model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static async Task SaveToDb(ContactViewModel model)
        {
            // save to db
            using (var ctx = new AppDbContext())
            {
                ctx.Contact.Add(new Contact()
                {
                    Email = model.Email,
                    Message = model.Message,
                    Name = model.Name,
                    Subject = model.Subject
                });

                await ctx.SaveChangesAsync();
            }
        }

        private async Task SendEmail(ContactViewModel model)
        {
            // send email
            var emailTo = WebConfigurationManager.AppSettings["emailTo"];
            var smptSection =
                (SmtpSection)WebConfigurationManager.GetWebApplicationSection("system.net/mailSettings/smtp");

            MailAddress fromMail = new MailAddress(smptSection.From);
            MailMessage mail = new MailMessage
            {
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                From = fromMail
            };

            string mailBody = GetTemplateContentFromCache("simple.html");

            mail.Body = mailBody
                .Replace("{{name}}", model.Name)
                .Replace("{{message}}", model.Message)
                .Replace("{{email}}", model.Email);

            mail.To.Add(emailTo);
            mail.Subject = "Contact us - " + model.Subject;
            mail.IsBodyHtml = true;

            using (var client = new SmtpClient())
            {
                await client.SendMailAsync(mail);
            }
        }

        private string GetTemplateContentFromCache(string templateName)
        {
            var path = HostingEnvironment.MapPath("~/email-templates");
            if (path == null) throw new InvalidOperationException();

            var cacheKey = templateName;
            var html = (string)MemoryCache.Default.Get(cacheKey);
            if (html == null)
            {
                html = File.ReadAllText(Path.Combine(path, templateName));
                MemoryCache.Default.Add(cacheKey, html, new CacheItemPolicy()
                {
                    SlidingExpiration = TimeSpan.FromHours(4)
                });
            }

            return html;
        }
    }
}
