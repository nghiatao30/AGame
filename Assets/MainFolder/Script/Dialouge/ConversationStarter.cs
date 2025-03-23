using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] NPCConversation npcConversation;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {   
                if( !ConversationManager.Instance.IsConversationActive)
                ConversationManager.Instance.StartConversation(npcConversation);          
            }
        } 
    }


    
}
