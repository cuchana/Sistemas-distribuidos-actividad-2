using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private string usersURL =
        "https://my-json-server.typicode.com/cuchana/Sistemas-distribuidos-actividad-2/users";

    [SerializeField]
    private string characterURL =
        "https://rickandmortyapi.com/api/character";

    [SerializeField] private TMP_Text userNameText;

    [SerializeField] private RawImage[] characterImages;
    [SerializeField] private TMP_Text[] characterNames;
    [SerializeField] private TMP_Text[] characterSpecies;
    [SerializeField] private TMP_Text[] characterStatus;

    void Start()
    {
        LoadUserByID(1);
    }

    public void LoadUserByID(int userID)
    {
        StopAllCoroutines();
        StartCoroutine(GetUser(userID));
    }

    IEnumerator GetUser(int id)
    {
        UnityWebRequest www = UnityWebRequest.Get(usersURL + "/" + id);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        User user = JsonUtility.FromJson<User>(www.downloadHandler.text);

        userNameText.text = user.username;

        // Cargar personajes del usuario
        for (int i = 0; i < user.characters.Length && i < characterImages.Length; i++)
        {
            StartCoroutine(GetCharacter(user.characters[i], i));
        }
    }

    IEnumerator GetCharacter(int characterID, int slot)
    {
        UnityWebRequest www = UnityWebRequest.Get(characterURL + "/" + characterID);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        Apiclases character = JsonUtility.FromJson<Apiclases>(www.downloadHandler.text);

        characterNames[slot].text = character.name;
        characterSpecies[slot].text = "Especie: " + character.species;
        characterStatus[slot].text = "Estado: " + character.status;

        StartCoroutine(GetTexture(character.image, slot));
    }

    IEnumerator GetTexture(string url, int slot)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            characterImages[slot].texture =
                DownloadHandlerTexture.GetContent(uwr);
        }
    }
}

[Serializable]
public class User
{
    public int id;
    public string username;
    public int[] characters; // IMPORTANTE: ahora se llama characters
}