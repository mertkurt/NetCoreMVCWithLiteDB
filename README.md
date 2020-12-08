Merhaba Arkadaşlar,

 

Bu yazımda sizlere Asp.NET Core MVC ile basit, kullanışlı ve günlük küçük verilerinizi saklayabileceğiniz ve de kurulum gerektirmeyen bir veri tabanı sisteminden bahsedeceğim.Bahsettiğim bu NoSql veri tabanı sisteminin ismi LiteDB.

 

LiteDB

LiteDB .NET uygulamaları için kurulum gerektirmeden kullanılabilen küçük veri kümelerini saklamamıza yardımcı olan bir veritabanı sistemidir. Veri saklama şekli aynı MongoDB deki gibidir.(LiteDB de eklenen herbir kayıt document olarak ifade edilir.Documentlar LiteDB de json formatında saklanır.Veri tabanlarında tablo olarak adlandırdığımız  yapılar ise LiteDB de collection, satır yapıları document ve sütun yapıları ise field olarak isimlendirilerek kullanılır.)

 

Kısa bilgilendirmelerimizden sonra herzamanki gibi öncelikle Asp.NET Core MVC projemizi Visual Studio üzerinden başlatıyoruz. Projemizi açtıktan sonra ilk olarak projemize sağ tıklayıp Manage Nuget Packages seçerek aşağıdaki görseldeki gibi LiteDB eklentisini aratarak projemize eklentimizi Install diyerek dahil ediyoruz.

 

LiteDB Manage Nuget Packages Kurulum Görseli

 

Projemizi ve kullanacağımız ekstra kütüphanemiz olan LiteDB yi de hazırladığımıza göre oluşturacağımız küçük uygulamanın kodlamasına herzamanki gibi gene modelimizi Models klasörümüzün içerisine yaratarak başlayabiliriz. Ben basit, hazırladığımız yazılarımı saklayabileceğimiz bir proje kodlayacağım için modelimin ismi Makale şeklinde yaparak kodlama işlemine başlıyorum.

 

    public class Makale

    {

        public int ID { get; set; }

        public string Baslik { get; set; }

        public string Icerik { get; set; }

        public DateTime? YazilmaTarihi { get; set; }

 

    }

 

Modelimi kodlarken kendi yazdığım yazıları kaydedeceğimi düşünerek bir veri tabanı klasiği olan ilk parametremiz olarak ID isimli integer bir alan tanımladım. Sonra sırası ile yazılarımızın başlığı, içeriği ve kayıt edildiği tarihi barındıran 3 property daha ekleyerek modelimi hazırlamış oldum.

 

Modelimiz hazır olduğuna göre hemen Controller'ımızı oluşturup veri tabanı etkileşimimizi kodlayarak verilerimizi nasıl saklayacağımızı kodlamaya başlayabiliriz. Ben controller olarak HomeController isimli ilk controller'ımı ekleyip kodlamaya başlıyorum.

 

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

 

Controller içerindelki ilk View Metodumuz olan Index metodumuzda eklediğimiz makalelerin listelenmesini sağlıyoruz.

MakaleEkle,MakaleGuncelle ve MakaleSil metotlarımız da isimlerinden de anlaşılabileceği gibi veri tabanına kayıt eklememizi ve eklediğimiz kaydı düzenlememizi veya silmemizi sağlayan metotlarımızdır.

 

Controller üzerinde kodladığımız en önemli bölüm ' using (var db = new LiteDatabase(@"veritabaniMakalelerDB.db")) ' şeklinde yazdığımız kod bloğumuz. Burda LiteDB ile oluşturduğumuz veri tabanımızın ismini ve nereye kaydedileceği bilgisini veriyoruz. Eğer sizde benim gibi bir klasör veya dosya yolu belirtmezseniz direk projenizin içerisine verdiğiniz isimli bir .db dosyası oluşturur.

Birde unutmadan modelimizde bulunan 'YazilmaTarihi' isimli property mizi her bir metotdumuz çalışırken anlık olarak işlemin yapıldığı zaman bilgisini veren Datetime.Now yapısına eşitliyoruzki eklenme zamanı bilgimizi güncel tutabilelim. 

 

Controller kodlamamızda bittiğine göre Viewlarımızı aşağıdaki gibi kodlayarak devam edebiliriz.

 

Index View (Index.cshtml)

 

@{

    ViewData["Title"] = "Makale Listesi";

}

@model List<NetCoreMVCWithLiteDB.Models.Makale>

<div class="row">

    <div class="col-md-3">

        <h2>MAKALE LİSTESİ</h2>

        <br />

        <a href="~/Home/MakaleEkle">MAKALE EKLE</a>

        <br />

        <table width="500">

            <thead>

                <tr>

                    <th>ID</th>

                    <th>Başlık</th>

                    <th>Yazılma Tarihi</th>

                    <th>İşlemler</th>

                </tr>

            </thead>

            <tbody>

                @foreach (NetCoreMVCWithLiteDB.Models.Makale m in Model)

                {

                     <tr>

                         <td>@m.ID</td>

                         <td>@m.Baslik</td>

                         <td>@m.YazilmaTarihi</td>

                         <td>

                             <a href="~/Home/MakaleGuncelle/@m.ID">Güncelle</a>

                             &nbsp;

                             <a href="~/Home/MakaleSil/@m.ID">Sil</a>

                         </td>

                     </tr>  

                }

            </tbody>

        </table>

    </div>

</div>

 

 

 

Index view'ımızda ekranımıza model olarak gönderdiğimiz verimizi bir table yardımı ile gösteriyoruz. Gösterdiğimiz her bir kayıt için İşlemler bölümü oluşturup verilerimizin güncellenmesi ve silinmesi için linklerimizi ayarlıyoruz.

 

MakaleEkle View (MakaleEkle.cshtml)

 

@{

    ViewData["Title"] = "Makale Ekle";

}

@model NetCoreMVCWithLiteDB.Models.Makale

<div class="row">

    <div class="col-md-12">

        <h2>Makale Ekle</h2>

        <form asp-controller="Home" asp-action="MakaleEkle" method="post" enctype="multipart/form-data">

            <input type="number" asp-for="ID" placeholder="ID değeri" />

            <br />

            <input type="text" asp-for="Baslik" placeholder="Başlık Yazılacak" />

            <br />

            <input type="text" multiple="multiple" asp-for="Icerik" placeholder="İçerik Yazılacak" />

            <br />

            <button type="submit">EKLE</button>

        </form>

    </div>

</div>

 

 

 

 

MakaleEkle View'ımızda ise bir model yaratıp o modeli ekranda yer verdiğimiz html elementlerimizle dolduruyoruz ve controller üzerine gönderdiğimiz verinin anlaşılabilir bir bütün olmasını sağlıyoruz.

 

MakaleGuncelle View (MakaleGuncelle.cshtml)

 

@{

    ViewData["Title"] = "Makale Güncelle";

}

@model NetCoreMVCWithLiteDB.Models.Makale

<div class="row">

    <div class="col-md-12">

        <h2>Makale Güncelle</h2>

        <form asp-controller="Home" asp-action="MakaleGuncelle" method="post" enctype="multipart/form-data">

            <input type="number" asp-for="ID" value="@Model.ID" placeholder="ID değeri" />

            <br />

            <input type="text" asp-for="Baslik" value="@Model.Baslik" placeholder="Başlık Yazılacak" />

            <br />

            <input type="text" multiple="multiple" asp-for="Icerik" value="@Model.Icerik" placeholder="İçerik Yazılacak" />

            <br />

            <button type="submit">GÜNCELLE</button>

        </form>

    </div>

</div>

 

 

 

MakaleGuncelle Veiw'ımızda ise güncellemek için ekranımıza gönderdiğimiz Modelimizin html elementlerinde gösterilip düzenlenmesini sağlıyoruz. Daha sonrada aynı MakaleEkle View'ında olduğu gibi verilerimizin controller'ımıza bir bütün halinde istediğimiz formatta geçmesi için html elementlerine ' asp-for ' attributesi yardımıyla modelimizdeki hangi alanlara referans ettiklerini yazıyoruz.

 

MakaleSil metodu için bir view hazırlamıyoruz. Onun yerine MakaleSil metoduna veri gönderdiğimizde üzerine düşen silme işlemini yaptıktan sonra Liste sayfasını açması için bir yönlendirme kodu koyduğumuz için direk kendi üzerine düşen işlemden sonra Index view'ımızı çalıştırıyor.

 

 

Kodlamalarımızı bu şekilde bitirdikten sonra artık projemizi derleyip çalıştırabiliriz. İlk baştada bahsettiğimiz gibi projemizi çalıştırdığımızda bütün veri tabanı işlemlerini gerçekleştirebilirsiniz. Bir makale kaydı ekleyip sonra o kaydı güncelleyip gerekli değil ise silebilirsiniz. İşlemleri deneme sırası sizin insiyatifinizdedir. 

 
