using ApiTestRuleta.Models;
using ApiTestRuleta.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace ApiTestRuleta.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApiController : System.Web.Http.ApiController
    {
        private readonly ApplicationDbContext db;

        public ApiController()
        {
            this.db = new ApplicationDbContext();
        }
        
        public ActionResult GetRandomNumberAndColor()
        {
            Random random = new Random();
            int number = random.Next(0, 36);

            var ele = Elements.elements.FirstOrDefault(a => a.Key == number);

            return new JsonResult()
            {
                Data = new { Number = ele.Key, Color = ele.Value },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json;charset=utf-8"
            };
        }

        public ActionResult GetUser(string userName)
        {
            try
            {
                var users = db.Users.ToList();
                var user = users.Where(a => a.Name == userName).FirstOrDefault();
                return new JsonResult() { Data = new { state = true, user }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json;charset=utf-8" };
            }
            catch (Exception exec)
            {
                return new JsonResult() { Data = new { state = false, errorMessage = exec.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json;charset=utf-8" };
            }
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddUser([FromBody] UserDTO user)
        {
            try
            {
                if (db.Users.Where(a => a.Name == user.Name).ToList().Count() > 0)
                {
                    var userSaved = db.Users.ToList().Where(a => a.Name == user.Name).FirstOrDefault();
                    userSaved.Monto = user.Monto;
                    db.Entry(userSaved).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.Users.Add(new User()
                    {
                        Name = user.Name,
                        Monto = user.Monto,
                    });
                    db.SaveChanges();
                }
                return new JsonResult() { Data = new { state = true, value = "value" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json;charset=utf-8" };
            }
            catch (Exception exec)
            {
                return new JsonResult() { Data = new { state = false, errorMessage = exec.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json;charset=utf-8" };
            }
        }

        [System.Web.Http.HttpGet]
        public ActionResult GetMontoApuesta(int TipoApuesta, int Monto, string Color, string TipoNumber, int? Numero, int RandomNumber)
        {
            try
            {
                JsonResult jsonResult = new JsonResult() { Data = new { }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentType = "application/json;charset=utf-8" };
                var ele = Elements.elements.FirstOrDefault(a => a.Key == RandomNumber);
                double Resultado = 0;

                if (Monto <= 0 && Color == "")
                {
                    jsonResult.Data = new { state = false, errorMessage = "EL monto debe ser mayor que 0 y el color es obligatorio" };
                    return jsonResult;
                };

                switch (TipoApuesta)
                {
                    case 1:
                        if (Color == ele.Value) Resultado = Monto / 2;
                        break;
                    case 2:
                        if (TipoNumber.ToUpper() != "PAR" && TipoNumber.ToUpper() != "INPAR")
                        {
                            jsonResult.Data = new { state = false, errorMessage = "" };
                            return jsonResult;
                        };

                        if (Color == ele.Value)
                        {
                            if (TipoNumber.ToUpper() == "PAR" && ele.Key % 2 == 0) Resultado = Monto;
                            if (TipoNumber.ToUpper() == "INPAR" && ele.Key % 2 != 0) Resultado = Monto;
                        }
                        break;
                    case 3:

                        if (Numero < 0 || Numero > 36)
                        {
                            jsonResult.Data = new { state = false, errorMessage = "El numero de la apuesta esta fuera de rango" };
                            return jsonResult;
                        };

                        if (Color == ele.Value && Numero == ele.Key) Resultado = Monto * 3;

                        break;
                }

                return new JsonResult()
                {
                    Data = new { state = true, montoGanado = Resultado },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json;charset=utf-8"
                };
            }
            catch (Exception exec)
            {
                return new JsonResult()
                {
                    Data = new { state = false, errorMessage = exec.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json;charset=utf-8"
                };
            }
        }

    }
}
