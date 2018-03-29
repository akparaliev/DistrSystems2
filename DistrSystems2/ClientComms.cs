using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrSystems2
{
    public class ClientComms : MarshalByRefObject, ICallsToServer
    {
        public string SomeSimpleFunction(int n)
        {
            Console.Write("  Client sent : {0}      \r", n);
            return "Server says : " + n.ToString();
        }

        private static event NotifyCallback s_notify;

        public event NotifyCallback Notify
        {
            add { s_notify += value; }
            remove { s_notify -= value; }
        }

        public static void FireNewBroadcastedMessageEvent(string s)
        {
            Console.WriteLine("Broadcasting... Sending : {0}", s);
            s_notify(s);
        }

        private readonly List<ActionEmitter> _emitters;
        private readonly Dictionary<long, History> _historyTimespan;
        private List<BankAccount> _bankAccounts;


        public ClientComms()
        {
            _emitters = new List<ActionEmitter>();
            _historyTimespan = new Dictionary<long, History>();
            _bankAccounts = new List<BankAccount>();
            Task.Run(() => RunListeningActions());
        }

        private void RunListeningActions()
        {
            bool stop = false;
            long localTimespan = 0;
            while (!stop)
            {
                ActionEmitter actionEmitter;
                lock (_emitters)
                {
                    if (_emitters.Count.Equals(0))
                    {
                        continue;
                    }

                    actionEmitter = _emitters.FirstOrDefault(x => x.TimeSpanActions.Any() && x.TimeSpanActions.First().Value.TimeSpan ==
                    _emitters.Where(xx => xx.TimeSpanActions.Any()).Min(y => y.TimeSpanActions.First().Value.TimeSpan));
                }
                if (actionEmitter == null)
                {
                    continue;
                }
                var action = actionEmitter.TimeSpanActions.First().Value;
                if (action.TimeSpan < localTimespan)
                {
                    RestoreState(action.TimeSpan);
                }
                action.Action.Invoke();
                actionEmitter.TimeSpanActions.Remove(action.TimeSpan);
                var temp = localTimespan;
                localTimespan = action.TimeSpan;
                SaveState(localTimespan, action);
                if (localTimespan < temp)
                {
                    localTimespan = RestoreActions(localTimespan);
                }
            }
        }

        public void CreateNewAccount(string id, string name, string accountId)
        {

            ConnectEmitter(id, name);
            InvokeAction(id, new TimeSpanAction(DateTime.Now.Ticks, () => ConnectBankAccount(accountId, name)));
        }



        private void ConnectBankAccount(string id, string name)
        {
            lock (_bankAccounts)
            {
                if (_bankAccounts.Any(x => x.Id == id))
                {
                    Console.WriteLine($"Bank account already connected with id = {id}");
                }
                else
                {
                    _bankAccounts.Add(new BankAccount(id, name));
                }
            }
        }
        private void ConnectEmitter(string id, string name)
        {
            lock (_emitters)
            {
                if (_emitters.Any(x => x.Id == id))
                {
                    Console.WriteLine($"Emmiter already connected with id = {id}");
                }
                else
                {
                    _emitters.Add(new ActionEmitter(id, name));
                }
            }
        }



        private void InvokeAction(string emitterId, TimeSpanAction action)
        {
            lock (_emitters)
            {
                var emitter = _emitters.FirstOrDefault(x => x.Id == emitterId);
                if (emitter != null)
                {
                    emitter.TimeSpanActions.Add(action.TimeSpan, action);
                }
            }
        }

        public void Withdraw(string emitterId, string bankAccountId, long createDate, int amount)
        {
            InvokeAction(emitterId, new TimeSpanAction(createDate, () =>
            {
                var result = $"message to {emitterId}: ";
                var emitter = _emitters.FirstOrDefault(x => x.Id == emitterId);
                if (emitter == null)
                {
                    result += "emitter not connected, withdraw failed";
                    FireNewBroadcastedMessageEvent(result);
                    return;
                }
                var bankAccount = _bankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                if (bankAccount == null)
                {
                    result += "bank account not connected, withdraw failed";
                    FireNewBroadcastedMessageEvent(result);
                    return;
                }
                if (bankAccount.Money < amount)
                {
                    result += $"add money to account of {bankAccount.Name}";
                    FireNewBroadcastedMessageEvent(result);
                    return;
                }
                bankAccount.Money -= amount;
                result += $"Withdraw with amount {amount} completed successfully from {emitter.Name} to {bankAccount.Name}";
                FireNewBroadcastedMessageEvent(result);
            }));
        }

        public void Deposit(string emitterId, string bankAccountId, long createDate, int amount)
        {
        
            InvokeAction(emitterId, new TimeSpanAction(createDate, () =>
            {
                var result = $"message to {emitterId}: ";
                var emitter = _emitters.FirstOrDefault(x => x.Id == emitterId);
                if (emitter == null)
                {
                    result += "emitter not connected, withdraw failed";
                    FireNewBroadcastedMessageEvent(result);
                    return;
                }
                var bankAccount = _bankAccounts.FirstOrDefault(x => x.Id == bankAccountId);
                if (bankAccount == null)
                {
                    result += "bank account not connected, withdraw failed";
                    FireNewBroadcastedMessageEvent(result);
                    return;
                }
                bankAccount.Money += amount;
                result += $"Deposit completed successfully from {emitter.Name} to {bankAccount.Name}";
                FireNewBroadcastedMessageEvent( result);
            }));
        }

        private void SaveState(long timespan, TimeSpanAction action)
        {
            Console.WriteLine($"Saving timespan {timespan}");
            var copy = _bankAccounts.Select(x => new BankAccount(x.Id, x.Name) { Money = x.Money }).ToList();


            if (_historyTimespan.ContainsKey(timespan))
            {
                _historyTimespan[timespan] = new History(copy, action);
            }
            else
            {
                _historyTimespan.Add(timespan, new History(copy, action));
            }
        }
        private void RestoreState(long timespan)
        {
            Console.WriteLine($"Restore timespan {timespan}");
            var historiesBelowTimespan = _historyTimespan.Where(x => x.Key < timespan);
            if (historiesBelowTimespan == null || historiesBelowTimespan.Count() == 0)
            {
                lock (_bankAccounts)
                {
                    _bankAccounts = new List<BankAccount>();
                }
            }
            else
            {
                lock (_bankAccounts)
                {
                    _bankAccounts = _historyTimespan[historiesBelowTimespan.Max(x => x.Key)].BankAccounts;
                }

            }


        }

        private long RestoreActions(long timespan)
        {
            var actions = _historyTimespan.Where(x => x.Key > timespan).ToList();
            foreach (var x in actions)
            {
                x.Value.Action.Action.Invoke();
                SaveState(x.Key, x.Value.Action);
            }
            return _historyTimespan.Max(x => x.Key);
        }

    }
}
