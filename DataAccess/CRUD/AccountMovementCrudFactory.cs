using DataAccess.DAO;
using DataAccess.DAO.DataAccess.DAO;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CRUD
{
    public class AccountMovementCrudFactory : CrudFactory
    {
        public AccountMovementCrudFactory()
        {
            _sqlDao = SqlDAO.GetInstance();
        }

        public override void Create(BaseDTO baseDTO)
        {
            var movement = baseDTO as AccountMovement;
            var sqlOperation = new SqlOperation() { ProcedureName = "CREATE_ACCOUNT_MOVEMENT_PR" };

            sqlOperation.AddIntParam("@P_TransactionID", movement.TransactionID);
            sqlOperation.AddStringParameter("@P_SourceAccountDescription", movement.SourceAccountDescription);
            sqlOperation.AddStringParameter("@P_DestinationAccountDescription", movement.DestinationAccountDescription);
            sqlOperation.AddStringParameter("@P_MovementType", movement.MovementType);
            sqlOperation.AddDoubleParam("@P_Amount", movement.Amount);

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var movement = baseDTO as AccountMovement;
            var sqlOperation = new SqlOperation() { ProcedureName = "DELETE_ACCOUNT_MOVEMENT_PR" };
            sqlOperation.AddIntParam("@P_MovementID", movement.ID);

            _sqlDao.ExecuteQueryProcedure(sqlOperation);
        }

        public override List<T> RetrieveAll<T>()
        {
            var movements = new List<T>();
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ALL_ACCOUNT_MOVEMENTS_PR" };
            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            foreach (var row in results)
            {
                var movement = BuildAccountMovement(row);
                movements.Add((T)Convert.ChangeType(movement, typeof(T)));
            }

            return movements;
        }

        public override T RetrieveById<T>(int ID)
        {
            var sqlOperation = new SqlOperation() { ProcedureName = "RET_ACCOUNT_MOVEMENT_BY_ID_PR" };
            sqlOperation.AddIntParam("@P_MovementID", ID);

            var results = _sqlDao.ExecuteQueryProcedure(sqlOperation);

            if (results.Count > 0)
            {
                var movement = BuildAccountMovement(results[0]);
                return (T)Convert.ChangeType(movement, typeof(T));
            }

            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var movement = baseDTO as AccountMovement;
            var sqlOperation = new SqlOperation() { ProcedureName = "UPDATE_ACCOUNT_MOVEMENT_PR" };

            sqlOperation.AddIntParam("@P_TransactionID", movement.TransactionID);
            sqlOperation.AddStringParameter("@P_SourceAccountDescription", movement.SourceAccountDescription);
            sqlOperation.AddStringParameter("@P_DestinationAccountDescription", movement.DestinationAccountDescription);
            sqlOperation.AddStringParameter("@P_MovementType", movement.MovementType);
            sqlOperation.AddDoubleParam("@P_Amount", movement.Amount);
  

            _sqlDao.ExecuteProcedure(sqlOperation);
        }

        private AccountMovement BuildAccountMovement(Dictionary<string, object> row)
        {
            return new AccountMovement
            {
                ID = (int)row["MovementID"],
                TransactionID = (int)row["TransactionID"],
                SourceAccountDescription = (string)row["SourceAccountDescription"],
                DestinationAccountDescription = (string)row["DestinationAccountDescription"],
                MovementType = (string)row["MovementType"],
                Amount = (double)row["Amount"]
            };
        }
    }
}
