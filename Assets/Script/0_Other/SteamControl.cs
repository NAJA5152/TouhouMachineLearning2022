using Steamworks;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Steamworks.NET
{
    public class SteamControl : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            if (SteamManager.Initialized)
            {
                string name = SteamFriends.GetPersonaName();

                Debug.LogError(name);
                Debug.LogError(SteamFriends.GetFriendCount( EFriendFlags.k_EFriendFlagAll));
                for (int i = 0; i < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll); i++)
                {
                    Debug.Log(SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll)));
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}