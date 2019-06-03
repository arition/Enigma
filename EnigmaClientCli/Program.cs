using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using EnigmaLib.API;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaClientCli
{
    class Program
    {
        private static string KeyPath => "privateKey.json";
        static void Main(string[] args)
        {
            if (File.Exists(KeyPath))
            {
                try
                {
                    ReadKey();
                }
                catch
                {
                    Console.WriteLine("Key corrupted.");
                    GenerateKey();
                    ReadKey();
                }
            }
            else
            {
                GenerateKey();
                ReadKey();
            }
            var client = new Client();
            client.Run();

            var autoResetEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                autoResetEvent.Set();
            };
            autoResetEvent.WaitOne();
        }

        static void ReadKey()
        {
            using (var reader = File.OpenText(KeyPath))
            {
                var json = reader.ReadToEnd();
                Global.APIBase.PrivateKey = JsonConvert.DeserializeObject<RSAParameters>(json);
            }
        }

        static void GenerateKey()
        {
            while (true)
            {
                Console.Write("Username: ");
                var name = Console.ReadLine();
                var user = new User {Username = name};
                using (var rsa = RSA.Create())
                {
                    user.PublicKey = rsa.ExportParameters(false);
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(user, new ValidationContext(user), validationResults))
                    {
                        Console.WriteLine("Create User Failed: ");
                        Console.WriteLine(string.Join(Environment.NewLine, validationResults));
                        continue;
                    }
                    using (var writer = File.CreateText(KeyPath))
                    {
                        writer.Write(JsonConvert.SerializeObject(rsa.ExportParameters(true)));
                    }
                }

                try
                {
                    Global.APIBase.CreateUserAPI().CreateUserAsync(user).Wait();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Create User Failed: ");
                    Console.WriteLine(e);
                    continue;
                }

                break;
            }
        }
    }
}
