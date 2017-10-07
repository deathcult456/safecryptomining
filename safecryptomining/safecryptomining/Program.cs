using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Configuration;

namespace miner_arduino2
{
     
    class Program
    {
        //miner
        static string Worker_name = ConfigurationManager.AppSettings["Worker_name"];
        static string adresse_BTC = ConfigurationManager.AppSettings["adresse_BTC"];
        static int interval_temps = int.Parse(ConfigurationManager.AppSettings["interval_temps"]);
        static bool Restart_miner = bool.Parse(ConfigurationManager.AppSettings["Restart_miner"]);
        //SMS
        static bool Utiliser_free_sms = bool.Parse(ConfigurationManager.AppSettings["Utiliser_free_sms"]);
        static string free_user = ConfigurationManager.AppSettings["free_user"];
        static string free_pass = ConfigurationManager.AppSettings["free_pass"];
        //---------------------------------
        static int i = 0;
        static int reseter = 0;
        static string nicehash_dl = "https://api.nicehash.com/api?method=stats.provider.workers&addr=" + adresse_BTC;

        static void Main(string[] args)
        {
            TimerCallback callback = new TimerCallback(Tick);
            Console.WriteLine("---------------");
            Console.WriteLine("By_deathcult456");
            Console.WriteLine("BTC donation: 15WnvJt7GCasvWuntB6AJSwVfuG7p5r7Ev");
            Console.WriteLine("---------------");
            Console.WriteLine(" ");
            Console.WriteLine("Interval temps:{0}", interval_temps);
            Console.WriteLine("Adresse BTC de minage:{0}", adresse_BTC);
            if (Restart_miner) { Console.WriteLine("Restart miner = true"); }
            if (Utiliser_free_sms) { Console.WriteLine("Utiliser_free_sms = true"); }
            Console.WriteLine(" ");
            Console.WriteLine("Creating timer: {0}\n", DateTime.Now.ToString("hh:mm:ss"));

            // create a one second timer tick
            Timer stateTimer = new Timer(callback, null, 0, interval_temps);

            // loop here forever
            for (;;)
            {
                // add a sleep for 4000 mSec to reduce CPU usage
                Thread.Sleep(interval_temps-1000);
            }
        }
        static public void Tick(Object stateInfo)
        {
           

            WebClient client = new WebClient();

            string nicehash_text = client.DownloadString(nicehash_dl); 
            Console.WriteLine("Tick: {0},  Reseter: {1}, i={2}", DateTime.Now.ToString("h:mm:ss"),reseter,i);
            if (nicehash_text.IndexOf(Worker_name) != -1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Reponse:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(nicehash_text);
                Console.WriteLine(" ");
                Console.ResetColor();
                i = 0;
                reseter++;
                if (reseter>= 20000) { reseter = 100; } // on evite de dépasser le int mais on garde un reseter superieur a 25(25x5second)
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Problem rig");
                i++;

                if (reseter >= 25 && i >= 10) //si reseter superieur a 20 pour être sure que le rig tourne deja depuis klk minute // si i superieur a 3 pour verrifier 3 probleme de rig
                {

                    if (Utiliser_free_sms) { string SMS = client.DownloadString("https://smsapi.free-mobile.fr/sendmsg?user=" + free_user + "&pass=" + free_pass + "&msg=RIG%20probleme:%20" + Worker_name); }
                    Console.WriteLine("Problem rig_SMS_send");
                    i = 0;
                    reseter = 0;
                    if (Restart_miner) { System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0"); }
                }

                Console.ResetColor();

            }
        }
    }
}
 
