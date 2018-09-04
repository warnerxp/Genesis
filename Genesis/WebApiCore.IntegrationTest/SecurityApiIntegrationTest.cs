using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using WebApiCore.Models;
using Xunit;

namespace WebApiCore.IntegrationTest
{
    public class SecurityApiIntegrationTest
    {
        #region Variables

        public string emaillSignIn = "wagner123@gmail.com";
        public string passwordSignIn = "123456";

        #endregion

        #region TestMethods
        [Fact]
        public async Task Test_SignUp()
        {
            using (var client = new TestClientProvider().Client)
            {
                try
                {
                    var response =
                        await client.PostAsync("/api/Security/SignUp", new StringContent(JsonConvert.SerializeObject(new AccountSecurity()
                        {
                            Name = GenerateName(),
                            Email = GenerateEmail(),
                            Password = "123456",
                            Phones = GeneratePhones(),
                        }), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    //implementation logger
                    Console.WriteLine(e);   
                }
            }
        }

        [Fact]
        public async Task Static_Test_SignUp()
        {
            using (var client = new TestClientProvider().Client)
            {
                try
                {
                    var response =
                        await client.PostAsync("/api/Security/SignUp", new StringContent(JsonConvert.SerializeObject(new AccountSecurity()
                        {
                            Name = GenerateName(),
                            Email = emaillSignIn,
                            Password = passwordSignIn,
                            Phones = GeneratePhones(),
                        }), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    //implementation logger
                    Console.WriteLine(e);
                }
            }
        }

        [Fact]
        public async Task Test_SignIn()
        {
            using (var client = new TestClientProvider().Client)
            {
                try
                {
                    var response =
                        await client.PostAsync("/api/Security/SignIn", new StringContent(JsonConvert.SerializeObject(new AccountSecurity()
                        {
                            Email = emaillSignIn,
                            Password = passwordSignIn,
                        }), Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                }
                catch (Exception e)
                {
                    //implemtantion logger
                    Console.WriteLine(e);

                }


            }
        }

        [Fact]
        public async Task Test_SignUp_BadRequest()
        {
            using (var client = new TestClientProvider().Client)
            {
                try
                {
                    var response =
                        await client.PostAsync("/api/Security/SignUp", new StringContent(JsonConvert.SerializeObject(new AccountSecurity()
                        {
                            Name = GenerateName(),
                            Email = emaillSignIn,
                            Password = "123456",
                            Phones = GeneratePhones(),
                        }), Encoding.UTF8, "application/json"));
                    
                    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                }
                catch (Exception e)
                {
                    //implementation logger
                    Console.WriteLine(e);
                }
            }
        }

        [Fact]
        public async Task Test_SignIn_BadRequest()
        {
            using (var client = new TestClientProvider().Client)
            {
                try
                {
                    var response =
                        await client.PostAsync("/api/Security/SignIn", new StringContent(JsonConvert.SerializeObject(new AccountSecurity()
                        {
                            Email = GenerateEmail(),
                            Password = "123456",
                        }), Encoding.UTF8, "application/json"));

                    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                }
                catch (Exception e)
                {
                    //implementation logger
                    Console.WriteLine(e);
                }
            }
        }

        #endregion

        #region PrivateMethods

        private string GenerateEmail()
        {
            Random randomGenerator = new Random();
            return "test" + randomGenerator.Next(1000) + "@gmail.com";
        }

        private string GenerateName()
        {
            Random randomGenerator = new Random();
            return "Wagner" + randomGenerator.Next(1000);
        }

        private List<Phone> GeneratePhones()
        {
            List<Phone> phones = new List<Phone>();
            for (int i = 0; i < 2; i++)
            {
                StringBuilder number = new StringBuilder();
                for (int j = 0; j < 9; j++)
                {
                    Random rnd = new Random();
                    number.Append(rnd.Next(0, 9));
                }
                Phone phone = new Phone { Value = number.ToString() };
                phones.Add(phone);
            }

            return phones;
        }

        #endregion

    }
}
