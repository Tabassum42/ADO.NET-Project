using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_5
{
    public interface ICrossDataSyncM
    {
        void ReloadData(List<Grocery> grocerys);
        void UpdateGrocery(Grocery g);
        void RemoveGrocery(int id);
        void ReloadProprietor(List<Proprietor> Proprietor);
        void UpdateProprietor(Proprietor p);
        void RemoveProprietor(int id);
        
    }
}
