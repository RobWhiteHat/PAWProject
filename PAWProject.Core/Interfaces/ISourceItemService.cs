using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using PAWProject.Data.Models;


namespace PAWProject.Core.Interfaces
{
    public interface ISourceItemService
    {
        Task<bool> SaveItemAsync(SourceItem item);
    }
}