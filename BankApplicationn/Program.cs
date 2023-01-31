using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBankaUygulamasi
{
    class Program
    {
        static void Main(string[] args)
        {
            List<client> musteriler = new List<client>();

            client yenimusteri = new client("oguz");
            yenimusteri.name = "oguz";
            yenimusteri.tc = "1";
            yenimusteri.password = "abc123";
            yenimusteri.birthdate = new DateTime(2000, 1, 1);

            account a1 = new account();
            a1.accountid = 11;
            a1.balance = 1000;
            yenimusteri.hesaplar.Add(a1);

            musteriler.Add(yenimusteri);
            musteriler.Add(new client("2", "hasan", "abc123", new DateTime(1999, 2, 2)));

            account a2 = new account();
            a2.accountid = 12;
            a2.balance = 3000;
            musteriler[1].hesaplar.Add(a2);

            client aktifmusteri = null;
            account aktifhesap = null;

            bool islogin = false;

            menugoster();

            void menugoster()
            {
                Console.WriteLine("menuden rakamlarla bir işlem seçiniz:");
                Console.WriteLine("1.Kullanıcı oluştur");//
                Console.WriteLine("2.Kullanıcı giriş yap");//
                Console.WriteLine("3.Hesap oluştur");
                Console.WriteLine("4. Hesap seç");
                Console.WriteLine("5.hesaptan para çek");
                Console.WriteLine("6.Farklı bir hesaba para transferi yap.");
                Console.WriteLine("7.Hesabı görüntüle");
                Console.WriteLine("8.Hesabı döviz cinsinden göster"); // API
                Console.WriteLine("9.Kullanıcı hesabını kapat");
                Console.WriteLine("0.Uygulamayı kapat");
                int secim = int.Parse(Console.ReadLine());


                switch (secim)
                {
                    case 1:
                        yenimusteriolustur();
                        break;
                    case 2:
                        login();
                        break;
                    case 3:
                        if (logincontrol()) hesapolustur();
                        break;
                    case 4:

                        break;
                    case 5:
                        break;
                    case 6:
                        if(logincontrol()) hesabaTransferYap();
                        break;
                    case 7:
                        if (logincontrol()) hesabigoruntule();
                        break;
                    case 8:
                        if(logincontrol()) dövizcinsindengoster();
                        break;
                    case 9:
                        if (logincontrol()) kullanicilogout();
                        break;
                    case 0:
                        break;
                }
                if (secim != 0) menugoster();
            }
            void yenimusteriolustur()
            {
                client newclient = new client();
                Console.Write("adınız: ");
                newclient.name = Console.ReadLine();
                Console.Write("sifre: ");
                newclient.password = Console.ReadLine();
                Console.Write("TC: ");
                newclient.tc = Console.ReadLine();
                Console.Write("Dogum yili(yil/ay/gün): ");
                newclient.birthdate = DateTime.Parse(Console.ReadLine());
                musteriler.Add(newclient);
                Console.WriteLine("Yeni müşteri başarıyla oluşturuldu");
                //yeni musteriyi oluşturun. 
            }
            void login()
            {
                Console.Write("TC: ");
                string tc = Console.ReadLine();

                Console.Write("sifre: ");
                string pss = Console.ReadLine();

                if (musteriler.Where(musteri => musteri.tc == tc && musteri.password == pss).ToList().Count > 0)
                {
                    aktifmusteri = musteriler.Where(musteri => musteri.tc == tc && musteri.password == pss).FirstOrDefault();
                    islogin = true;
                }
                else
                {
                    islogin = false;
                    aktifmusteri = null;
                    Console.WriteLine("Girişte hata var, kullanıcı adı yada sifre yanlış");
                }
            }
            bool logincontrol()
            {
                if (!islogin)
                {
                    Console.WriteLine("Önce giriş yapmanız gerekir, girişe yönlendiriliyorsunuz...");
                    login();
                    return false;
                }
                else
                    return true;
            }
            void hesapolustur()
            {
                Console.WriteLine("hesap oluşturuluyor");
                account newaccount = new account();
                Console.Write("hesapno: ");
                newaccount.accountid = int.Parse(Console.ReadLine());
                Console.Write("hesapmiktarı: ");
                newaccount.balance = int.Parse(Console.ReadLine());
                aktifmusteri.hesaplar.Add(newaccount);
                Console.WriteLine("Hesap başarıyla oluşturuldu");
            }
            void hesabigoruntule()
            {
                for (int i = 0; i < aktifmusteri.hesaplar.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. hesap:  {aktifmusteri.hesaplar[i].accountid} - {aktifmusteri.hesaplar[i].balance}");
                }
                Console.WriteLine("seçmek istediğiniz hesabın sırasını girin:");
                int hesapsecimid = int.Parse(Console.ReadLine());
                aktifhesap = aktifmusteri.hesaplar[hesapsecimid - 1];
                Console.WriteLine("hesap seçimi yapıldı işleme devam edebilirsiniz");
            }
            void hesabaTransferYap()
            {
                

                Console.WriteLine("transfer yapılacak hesabın id sini giriniz: ");
                int transferHesapNo = int.Parse(Console.ReadLine());
                Console.WriteLine("transfer yapılacak miktar: ");
                float transferMiktar = float.Parse(Console.ReadLine());

                //client gonderimusteri = musteriler.Where(musteri => musteri.hesaplar.Contains(musteri.hesaplar.Where(hesap => hesap.accountid == transferHesapNo).FirstOrDefault())).FirstOrDefault();
                //account gonderihesabi = gonderimusteri.hesaplar.Where(hesap => hesap.accountid == transferHesapNo).FirstOrDefault();
                
                
                account gonderihesabi = null;
                bool bulundu = false;
                foreach (client musteri in musteriler)
                {
                    
                    foreach (account hesap in musteri.hesaplar)
                    {
                        
                        if (transferHesapNo == hesap.accountid)
                        {
                            gonderihesabi = hesap;
                            bulundu = true;
                            break;
                        }
                    }
                    if (bulundu)
                    {
                        break;
                    }
                }
                if (bulundu)
                {

                    if (transferMiktar < aktifhesap.balance)
                    {
                        aktifhesap.balance -= transferMiktar;
                        gonderihesabi.balance += transferMiktar;
                        Console.WriteLine($"{DateTime.Now} tarihinde {aktifhesap.accountid} hesabından " +
                            $"{gonderihesabi.accountid} hesabına {transferMiktar} TL gönderimi gerçekleşti.");

                    }
                    else { Console.WriteLine("Transfer edecek kadar bakiye bulunmuyor."); }
                }
                else
                {
                    Console.WriteLine("Transfer edilecek hesap bulunamadı.");
                }
                
                
            }
            async void dövizcinsindengoster()
            {
                var newclient = new HttpClient();
                
                string veri = await newclient.GetStringAsync("https://api.genelpara.com/embed/para-birimleri.json");
                dynamic kurlar = JValue.Parse(veri);
                Console.WriteLine(kurlar.USD.satis);
                Console.WriteLine($"{aktifhesap.accountid} hesabının dolar karşılığı: {aktifhesap.balance / (float)kurlar.USD.satis}");


            }
            void kullanicilogout()
            {
                aktifmusteri = null;
                islogin = false;
                aktifhesap = null;
                Console.WriteLine("Oturumunuz kapatıldı.");
            }

        }
    }
    class client
    {
        public client()//constructor
        {
            //Console.WriteLine("yeni müşteri oluşturuldu");
        }
        public client(string _name)//constructor
        {
            //Console.WriteLine(_name+ " müşterisi oluşturuldu");
        }
        public client(string _tc, string _name, string _password, DateTime _birth)//constructor
        {
            tc = _tc;
            name = _name;
            password = _password;
            birthdate = _birth;
        }
        public List<account> hesaplar = new List<account>();
        public string tc;
        public string name;
        public string password;
        public DateTime birthdate;
    }
    class account // a client has one or many accounts
    {
        public account()
        {
        }
        public int accountid;
        public float balance;//hesaptaki para.
        //public client accountholder;
    }
}
