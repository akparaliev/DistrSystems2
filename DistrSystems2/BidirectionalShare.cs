using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
        public delegate void NotifyCallback(string s);

        public interface ICallsToServer
        {
            void CreateNewAccount(string id, string name, string accountId);
            void Withdraw(string emitterId, string bankAccountId, long createDate, int amount);
            void Deposit(string emitterId, string bankAccountId, long createDate, int amount);
            string SomeSimpleFunction(int n);
            event NotifyCallback Notify;
        }
 
    public abstract class NotifyCallbackSink : MarshalByRefObject
    {
        /// <summary>
        /// Called by the server to fire the call back to the client
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        public void FireNotifyCallback(string s)
        {
            Console.WriteLine("Activating callback");
            OnNotifyCallback(s);
        }

        /// <summary>
        /// Client overrides this method to receive the callback events from the server
        /// </summary>
        /// <param name="s">Pass a string for testing</param>
        protected abstract void OnNotifyCallback(string s);
    }
}
