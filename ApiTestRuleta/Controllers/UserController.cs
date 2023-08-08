using ApiTestRuleta.Models.DTOs;
using ApiTestRuleta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace ApiTestRuleta.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly ApplicationDbContext db;

        public UserController()
        {
            this.db = new ApplicationDbContext();
        }

        public async Task<ActionResult> GetUser(string userName)
        {
            try
            {
                var user = await db.Users.FirstOrDefaultAsync(a => a.Name == userName);
                return new JsonResult()
                {
                    Data = new { state = true, user },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json;charset=utf-8"
                };
            }
            catch (Exception exec)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, exec.Message);
            }
        }

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> AddOrUpdateUser([FromBody] UserDTO userDTO)
        {
            try
            {
                var userDb = await db.Users.FirstOrDefaultAsync(a => a.Name == userDTO.Name);
                if (userDb != null)
                {
                    userDb.Monto = userDTO.Monto;
                    db.Entry(userDb).State = EntityState.Modified;
                }
                else
                {
                    db.Users.Add(new User()
                    {
                        Name = userDTO.Name,
                        Monto = userDTO.Monto,
                    });
                }

                db.SaveChanges();
                
                return new JsonResult()
                {
                    Data = new { state = true, value = "value" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json;charset=utf-8"
                };
            }
            catch (Exception exec)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, exec.Message);
            }
        }

    }
}