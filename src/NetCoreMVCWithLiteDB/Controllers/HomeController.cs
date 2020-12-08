using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiteDB;
using NetCoreMVCWithLiteDB.Models;

namespace NetCoreMVCWithLiteDB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Makale> returnData = new List<Makale>();

            using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db"))
            {
                var dbMakaleKayitlari = db.GetCollection<Makale>("Makale");
                foreach (Makale item in dbMakaleKayitlari.FindAll())
                {
                    returnData.Add(item);
                }
            }

            return View(returnData);

        }

        [HttpGet]
        public IActionResult MakaleEkle()
        {

            return View();
        }

        [HttpPost]
        public IActionResult MakaleEkle(Makale model) {

            using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db"))
            {
                var dbMakaleler = db.GetCollection<Makale>("Makale");

                model.YazilmaTarihi = DateTime.Now;

                dbMakaleler.Insert(model);

            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MakaleGuncelle(int id)
        {
            Makale returnData;

            using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db"))
            {
                var dbMakaleler = db.GetCollection<Makale>("Makale");

                returnData= dbMakaleler.Find(x => x.ID == id).FirstOrDefault();

            }

            return View(returnData);
        }

        [HttpPost]
        public IActionResult MakaleGuncelle(Makale model)
        {
            model.YazilmaTarihi = DateTime.Now;

            using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db"))
            {
                var dbMakaleler = db.GetCollection<Makale>("Makale");

                dbMakaleler.Update(model);

            }

            return RedirectToAction("Index");
        }


        public IActionResult MakaleSil(int id)
        {

            using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db"))
            {
                var dbMakaleler = db.GetCollection<Makale>("Makale");

                dbMakaleler.DeleteMany(x=>x.ID == id);

            }

            return RedirectToAction("Index");
        }

    }
}
