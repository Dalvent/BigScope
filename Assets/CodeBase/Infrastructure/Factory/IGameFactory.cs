using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IGameFactory
    {
        Task<GameObject> CreateHud();
        Task<GameObject> CreateHero(Vector3 playerSpawnPoint);
    }
}