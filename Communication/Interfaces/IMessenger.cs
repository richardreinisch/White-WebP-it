
using System;


namespace WhiteWebPit.Communication.Interfaces
{
    public interface IMessenger
    {

        void Send<T>(T message);

        void Register<T>(Action<T> action);

    }

}
