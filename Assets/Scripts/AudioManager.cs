using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager2 instance;

    void Awake()
    {
        if(intance == null)
            instance = this;
        else
            {
                Destroy(gameObject);
                return;
            }
    
    
    DontDestroyOnLoad(gameObject);

    foreach(Sound s in sounds){
        s.source = gameObject.AddComponent<AudioSource>;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    }
    }
    void Start()
    {
       Play(""); 
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds,sounds => sounds.name == name);
        if(s == null)
        {
            Debug.LogWarning("Efeito:" + name + "Não achei");
            return
        }
        s.source.Play();
    }
}
