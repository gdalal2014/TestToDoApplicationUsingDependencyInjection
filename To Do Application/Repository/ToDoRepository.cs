using Dapper;
using To_Do_Application.UoW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using To_Do_Application.Entities;
using Autofac;

namespace To_Do_Application.Repository
{
    public class ToDoRepository : IToDoRepository
    {
        private ILifetimeScope _autofacContainer { get; }

        private IUnitOfWork _uow;
        public ToDoRepository(ILifetimeScope autofacContainer)
        {
            _autofacContainer = autofacContainer;

        }

        public IEnumerable<ToDoEntity> All()
        {
            using (var scope = _autofacContainer.BeginLifetimeScope())
            {
                _uow = scope.Resolve<IUnitOfWork>();
                _uow.Connect();
                var _newTransaction = _uow.Begin();
                return _newTransaction.Connection.Query<ToDoEntity>(
                "SELECT list_id, user_id, item, create_date, modified_date, is_completed, date_format(completion_date, '%m/%d/%Y') AS completion_date FROM todolist.list_tbl",
                transaction: _newTransaction
            ).ToList();
            }
        }

        public IEnumerable<ToDoEntity> Find(string user_id)
        {
            using (var scope = _autofacContainer.BeginLifetimeScope())
            {
                _uow = scope.Resolve<IUnitOfWork>();
                _uow.Connect();
                var _newTransaction = _uow.Begin();
                return _newTransaction.Connection.Query<ToDoEntity>(
                "SELECT list_id, user_id, item, create_date, modified_date, is_completed, date_format(completion_date, '%m/%d/%Y') AS completion_date FROM todolist.list_tbl WHERE user_id = @userId",
                param: new { userId = user_id },
                transaction: _newTransaction
            ).ToList();
            }
        }

        public void Add(ToDoEntity entity)
        {
            using (var scope = _autofacContainer.BeginLifetimeScope())
            {
                _uow = scope.Resolve<IUnitOfWork>();
                _uow.Connect();
                var _newTransaction = _uow.Begin();
                entity.list_id = _newTransaction.Connection.ExecuteScalar<string>(
                "INSERT INTO todolist.list_tbl(user_id, item, create_date, modified_date, is_completed, completion_date) VALUES(@userId, @item, sysdate(), sysdate(), @completed, STR_TO_DATE(@completionDate,'%Y-%m-%d %H:%i:%s'));",

                param: new { userId = entity.user_id, item = entity.item, completed = entity.is_completed.ToUpper(), completionDate = entity.completion_date },
                transaction: _newTransaction
            );
                _uow.Commit();
                _uow.Dispose();
            }
        }

        public void Update(ToDoEntity entity)
        {
            using (var scope = _autofacContainer.BeginLifetimeScope())
            {
                _uow = scope.Resolve<IUnitOfWork>();
                _uow.Connect();
                var _newTransaction = _uow.Begin();
                _newTransaction.Connection.Execute(
                "UPDATE todolist.list_tbl SET item = @item, modified_date = sysdate(), is_completed = @completed, completion_date = STR_TO_DATE(@completionDate,'%Y-%m-%d %H:%i:%s') WHERE user_id = @userId and list_id = @listId",
                param: new { Item = entity.item, completed = entity.is_completed, completionDate = entity.completion_date, userId = entity.user_id, listId = entity.list_id },
                transaction: _newTransaction
            );
                _uow.Commit();
                _uow.Dispose();

            }

        }

        public void UpdateCompletedEntry(ToDoEntity entity)
        {
            using (var scope = _autofacContainer.BeginLifetimeScope())
            {
                _uow = scope.Resolve<IUnitOfWork>();
                _uow.Connect();
                var _newTransaction = _uow.Begin();
                _newTransaction.Connection.Execute(
                "UPDATE todolist.list_tbl SET modified_date = sysdate(), is_completed = 'Y' WHERE user_id = @userId and list_id = @listId",
                param: new { userId = entity.user_id, listId = entity.list_id },
                transaction: _newTransaction
            );
            _uow.Commit(); 
            _uow.Dispose();

            }
        }

    }
}

