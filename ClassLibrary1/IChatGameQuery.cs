using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotInterfaces
{
    public interface IChatGameQuery
    {
        event EventHandler<EventArgs> OnGameList;
        void SearchGame(String name);
        List<KeyValuePair<String, String>> GameList
        {
            get;
            set;
        }
    }
}
