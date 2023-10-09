using System.Collections.Generic;
using UnityEngine;


namespace antoinegleisberg.SaveSystem
{
    [System.Serializable]
    public class GameData
    {
        public PlayerSaveData PlayerData;

        public GameData(object _)
        {
            // Adding a dummy argument is necessary because as GameData is Serializable,
            // the constructor is called on game startup, and the PokemonBuilder is not
            // ready yet as it needs to load some data first


            Vector3 defaultPosition = new Vector3(1.5f, 0.5f, 0);

            PlayerData = new PlayerSaveData(defaultPosition);
        }
    }

    [System.Serializable]
    public struct PlayerSaveData
    {
        public Vector3 PlayerPosition;

        public PlayerSaveData(Vector3 position)
        {
            PlayerPosition = position;
        }
    }

}
