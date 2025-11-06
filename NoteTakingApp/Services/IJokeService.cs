using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp.Services
{
    public interface IJokeService
    {
        Task<string> GetProgrammingJokeAsync();
    }
}
