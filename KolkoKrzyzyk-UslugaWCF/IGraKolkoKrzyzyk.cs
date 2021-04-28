using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KolkoKrzyzyk_UslugaWCF
{
    [ServiceContract]
    public interface IGraKolkoKrzyzyk
    {
        [OperationContract]
        void Start();
        [OperationContract]
        bool WykonajRuch(int wiersz, int kolumna,
        out int wierszServer, out int kolumnaServer);
        [OperationContract]
        int SparwdzWygrana(int wiersz, int kolumna);
    }
}
