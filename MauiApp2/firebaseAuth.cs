using Firebase.Auth;
using Firebase.Auth.Providers;
using System;
using System.Threading.Tasks;

namespace MauiApp2
{
    public class FirebaseAuthService
    {
        private readonly FirebaseAuthClient _client;

        public FirebaseAuthService()
        {
            FirebaseAuthConfig config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyApBnueP3dQerQuCZ5ZWQPuBmPURNvMNqc",
                AuthDomain = "veritabani-92c66.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            };

            _client = new FirebaseAuthClient(config);
        }
        public async Task<(bool Success, string Message)> RegisterUserAsync(string email, string password)
        {
            try
            {
                var response = await _client.CreateUserWithEmailAndPasswordAsync(email, password);
                return (true, response.User.Uid);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, ex.Reason.ToString());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> SignInUserAsync(string email, string password)
        {
            try
            {
                var auth = await _client.SignInWithEmailAndPasswordAsync(email, password);

                return (true, auth.User.Uid);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, ex.Reason.ToString());
            }
            catch (Exception ex)
            {

                return (false, ex.Message);
            }
        }


        public async Task SignOutAsync()
        {
            await Task.CompletedTask;
        }
    }
}