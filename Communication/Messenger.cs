
using System;
using System.Collections.Generic;
using System.Linq;

using WhiteWebPit.Communication.Interfaces;


namespace WhiteWebPit.Interfaces
{
    public class Messenger : IMessenger
    {

        private readonly Dictionary<Type, List<object>> actions = new Dictionary<Type, List<object>>();

        public void Send<T>(T message)
        {

            if (actions.ContainsKey(typeof(T)))
            {
                foreach (var action in actions[typeof(T)].Cast<Action<T>>())
                {
                    action(message);
                }
            }

        }

        public void Register<T>(Action<T> action)
        {

            if (!actions.ContainsKey(typeof(T)))
            {
                actions[typeof(T)] = new List<object>();
            }

            actions[typeof(T)].Add(action);

        }

    }

}
