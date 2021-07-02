using System.ServiceModel;

namespace WCFGame
{
    [ServiceContract]
    public interface IGameService
    {
        [OperationContract]
        void Start();
        [OperationContract]
        bool Move(int row, int col,
        out int serverRow, out int serverCol);
        [OperationContract]
        int CheckWin(int row, int column);
    }
}
