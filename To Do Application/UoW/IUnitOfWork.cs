using To_Do_Application.Repository;
using System;
using System.Data;

namespace To_Do_Application.UoW
{
    public interface IUnitOfWork : IDisposable
    {

        void Connect();
        IDbTransaction Begin();
        void Commit();
        void Rollback();
    }
}

