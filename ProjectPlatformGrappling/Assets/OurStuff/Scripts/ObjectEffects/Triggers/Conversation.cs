using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Conversation : MonoBehaviour {
    [System.Serializable]
    public class FunctionEvent : UnityEvent { }

    public float startConversationDelay = 1.0f;

    public class CharacterEvent : MonoBehaviour //denne visar ju inte sina medlemsvariabler!!
    {
        [SerializeField]
        public float characterTime = 2.0f;
        public AudioSource characterAudioSource;
        public Animator characterAnimator;

        public AudioClip characterAudioLine;
        public Animation characterAnimation;

        public float eventDelay = 2.0f;
        public FunctionEvent OnEvent; //funktion som ska kallas
    }

    public CharacterEvent[] characterEvents;

    public void BeginConversation()
    {
        StartCoroutine(PlayConversation());
    }

    IEnumerator PlayConversation()
    {
        
        for(int i = 0; i < characterEvents.Length; i++)
        {
            CharacterEvent cEvent = characterEvents[i];
            StartCoroutine(EventDelay(cEvent));

            if (cEvent.characterAudioLine != null)
            {
                cEvent.characterAudioSource.PlayOneShot(cEvent.characterAudioLine);
            }

            if(cEvent.characterAnimation != null)
            {
                cEvent.characterAnimator.CrossFade(cEvent.characterAnimation.ToString(), 0.5f);
            }

            yield return new WaitForSeconds(characterEvents[i].characterTime);
        }
    }

    IEnumerator EventDelay(CharacterEvent cEvent)
    {
        yield return new WaitForSeconds(cEvent.eventDelay);
        cEvent.OnEvent.Invoke();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            BeginConversation();
        }
    }
}
