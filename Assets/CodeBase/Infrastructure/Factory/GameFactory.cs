using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class GameFactory : IGameFactory
    {
        public Task<GameObject> CreateHud()
        {
            throw new System.NotImplementedException();
        }

        public Task<GameObject> CreateHero(Vector3 playerSpawnPoint)
        {
            throw new System.NotImplementedException();
        }
    }
}